using KingdomCapitals.Constants;
using KingdomCapitals.Core;
using KingdomCapitals.Models;
using KingdomCapitals.Services;
using KingdomCapitals.Utils;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;

namespace KingdomCapitals.Behaviors
{
    /// <summary>
    /// Handles the conquest mechanics when a capital city is captured.
    /// Implements automatic kingdom destruction and capital transfer to ruling clan.
    /// </summary>
    public class CapitalConquestBehavior : CampaignBehaviorBase
    {
        private ModSettings Settings => ModSettings.Instance;

        /// <summary>
        /// Registers event listeners for settlement ownership changes.
        /// </summary>
        public override void RegisterEvents()
        {
            CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, OnSettlementOwnerChanged);
        }

        /// <summary>
        /// Synchronizes behavior data with save games.
        /// </summary>
        /// <param name="dataStore">The data store for serialization.</param>
        public override void SyncData(IDataStore dataStore)
        {
            // No data to sync
        }

        /// <summary>
        /// Called when a settlement changes ownership (conquest or transfer).
        /// Triggers capital conquest mechanics if a capital city is captured.
        /// </summary>
        /// <param name="settlement">The settlement that changed ownership.</param>
        /// <param name="openToClaim">Whether the settlement is open to claim.</param>
        /// <param name="newOwner">The new owner of the settlement.</param>
        /// <param name="oldOwner">The previous owner of the settlement.</param>
        /// <param name="capturerHero">The hero who captured the settlement.</param>
        /// <param name="detail">Details about how ownership changed.</param>
        private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
        {
            try
            {
                // Only process if this is a capital being conquered
                if (!CapitalManager.IsCapital(settlement))
                {
                    return;
                }

                // Ignore non-conquest transfers (gifts, grants, etc.)
                if (detail is not ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.BySiege and
                    not ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.ByBarter and
                    not ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.ByRebellion)
                {
                    return;
                }

                Kingdom newKingdom = capturerHero?.MapFaction as Kingdom;

                if (oldOwner?.MapFaction is not Kingdom oldKingdom)
                {
                    ModLogger.Warning(string.Format(Messages.Warnings.CapitalConqueredOldKingdomNull, settlement.Name));
                    return;
                }

                ModLogger.LogCapitalConquest(settlement, oldKingdom, newKingdom, capturerHero);

                // Handle different conquest scenarios
                if (newKingdom == null && capturerHero == Hero.MainHero)
                {
                    // Player captured capital without a kingdom
                    HandlePlayerConquestWithoutKingdom(settlement, oldKingdom, capturerHero);
                }
                else if (newKingdom != null)
                {
                    // Standard kingdom vs kingdom conquest
                    HandleKingdomConquest(settlement, oldKingdom, newKingdom, capturerHero);
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Error in OnSettlementOwnerChanged for {settlement?.Name}", ex);
            }
        }

        /// <summary>
        /// Handles capital conquest when player has no kingdom.
        /// Notifies the player to found a kingdom to complete the conquest.
        /// </summary>
        /// <param name="capital">The capital settlement that was captured.</param>
        /// <param name="defeatedKingdom">The kingdom that was defeated.</param>
        /// <param name="playerHero">The player hero who captured the capital.</param>
        private void HandlePlayerConquestWithoutKingdom(Settlement capital, Kingdom defeatedKingdom, Hero playerHero)
        {
            try
            {
                ModLogger.Log(string.Format(Messages.Log.PlayerCapturedCapitalWithoutKingdomLog, playerHero.Name, capital.Name));

                // Notify player to found kingdom
                ConquestNotificationService.NotifyPlayerCapitalWithoutKingdom(capital, defeatedKingdom, playerHero);

                // The player must manually found a kingdom
                // When they do, the capital will automatically be recognized
                CapitalManager.CreatePlayerKingdomFromCapital(playerHero, capital);
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Error handling player conquest without kingdom for {capital.Name}", ex);
            }
        }

        /// <summary>
        /// Handles capital conquest between kingdoms.
        /// Executes the complete conquest sequence including settlement transfers and kingdom destruction.
        /// </summary>
        /// <param name="capital">The capital settlement that was captured.</param>
        /// <param name="defeatedKingdom">The kingdom that lost its capital.</param>
        /// <param name="conquererKingdom">The kingdom that captured the capital.</param>
        /// <param name="capturerHero">The hero who captured the capital.</param>
        private void HandleKingdomConquest(Settlement capital, Kingdom defeatedKingdom, Kingdom conquererKingdom, Hero capturerHero)
        {
            try
            {
                // Check if capital conquest is enabled
                if (Settings?.EnableCapitalConquest == false)
                {
                    ModLogger.Log(string.Format(Messages.Log.CapitalConquestDisabled, capital.Name));
                    return;
                }

                // 1. Mark capital as recently captured (blocks voting)
                CapitalManager.MarkAsRecentlyCaptured(capital);

                // 2. Transfer capital to ruling clan WITHOUT voting (if enabled)
                if (Settings?.TransferCapitalToRulingClan != false)
                {
                    _ = SettlementTransferService.TransferCapitalToRulingClan(capital, conquererKingdom);
                }

                // 3. Vassalize all clans of defeated kingdom BEFORE destroying the kingdom
                // This must happen BEFORE DestroyKingdom, otherwise clans become clanless
                if (Settings?.VassalizeDefeatedClans != false)
                {
                    _ = KingdomService.VassalizeDefeatedClans(defeatedKingdom, conquererKingdom);
                }

                // 4. Remove capital status from conquered settlement
                CapitalManager.UnregisterCapital(capital, defeatedKingdom);

                // 5. Destroy the defeated kingdom
                // NOTE: Other settlements will be distributed via vanilla voting system
                _ = KingdomService.DestroyKingdom(defeatedKingdom);

                // 6. Notify player (if enabled)
                if (Settings?.EnableConquestNotifications != false)
                {
                    ConquestNotificationService.NotifyKingdomConquest(capital, defeatedKingdom, conquererKingdom, capturerHero);
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Error handling kingdom conquest for {capital.Name}", ex);
            }
        }
    }
}
