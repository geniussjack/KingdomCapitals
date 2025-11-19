using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using KingdomCapitals.Core;
using KingdomCapitals.Utils;
using KingdomCapitals.Models;

namespace KingdomCapitals.Behaviors
{
    /// <summary>
    /// Handles the conquest mechanics when a capital city is captured.
    /// Implements automatic kingdom destruction and capital transfer to ruling clan.
    /// </summary>
    public class CapitalConquestBehavior : CampaignBehaviorBase
    {
        private ModSettings Settings => ModSettings.Instance;

        public override void RegisterEvents()
        {
            CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, OnSettlementOwnerChanged);
        }

        public override void SyncData(IDataStore dataStore)
        {
            // No data to sync
        }

        /// <summary>
        /// Called when a settlement changes ownership (conquest or transfer).
        /// </summary>
        private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
        {
            try
            {
                // Only process if this is a capital being conquered
                if (!CapitalManager.IsCapital(settlement))
                    return;

                // Ignore non-conquest transfers (gifts, grants, etc.)
                if (detail != ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.BySiege &&
                    detail != ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.ByBarter &&
                    detail != ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.ByRebellion)
                    return;

                Kingdom oldKingdom = oldOwner?.MapFaction as Kingdom;
                Kingdom newKingdom = capturerHero?.MapFaction as Kingdom;

                if (oldKingdom == null)
                {
                    ModLogger.Warning($"Capital {settlement.Name} conquered but old kingdom is null");
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
        /// Creates a new kingdom for the player.
        /// </summary>
        private void HandlePlayerConquestWithoutKingdom(Settlement capital, Kingdom defeatedKingdom, Hero playerHero)
        {
            try
            {
                ModLogger.Log($"Player {playerHero.Name} captured capital {capital.Name} without a kingdom");

                // Notify player to found kingdom
                InformationManager.DisplayMessage(new InformationMessage(
                    $"You have captured {capital.Name}, the capital of {defeatedKingdom.Name}! Found your own kingdom to complete the conquest.",
                    Colors.Gold
                ));

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
        /// Executes the complete conquest sequence.
        /// </summary>
        private void HandleKingdomConquest(Settlement capital, Kingdom defeatedKingdom, Kingdom conquererKingdom, Hero capturerHero)
        {
            try
            {
                // Check if capital conquest is enabled
                if (Settings?.EnableCapitalConquest == false)
                {
                    ModLogger.Log($"Capital conquest disabled in settings, treating {capital.Name} as normal settlement");
                    return;
                }

                // 1. Mark capital as recently captured (blocks voting)
                CapitalManager.MarkAsRecentlyCaptured(capital);

                // 2. Transfer capital to ruling clan WITHOUT voting (if enabled)
                if (Settings?.TransferCapitalToRulingClan != false)
                {
                    TransferCapitalToRulingClan(capital, conquererKingdom);
                }

                // 3. Transfer all other settlements of defeated kingdom
                TransferAllSettlements(defeatedKingdom, conquererKingdom);

                // 4. Vassalize all clans of defeated kingdom (if enabled)
                if (Settings?.VassalizeDefeatedClans != false)
                {
                    VassalizeClans(defeatedKingdom, conquererKingdom);
                }

                // 5. Remove capital status from conquered settlement
                CapitalManager.UnregisterCapital(capital, defeatedKingdom);

                // 6. Destroy the defeated kingdom
                DestroyKingdom(defeatedKingdom);

                // 7. Notify player (if enabled)
                if (Settings?.EnableConquestNotifications != false)
                {
                    NotifyKingdomConquest(capital, defeatedKingdom, conquererKingdom, capturerHero);
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Error handling kingdom conquest for {capital.Name}", ex);
            }
        }

        /// <summary>
        /// Transfers the captured capital directly to the ruling clan without voting.
        /// </summary>
        private void TransferCapitalToRulingClan(Settlement capital, Kingdom conquererKingdom)
        {
            try
            {
                if (conquererKingdom?.RulingClan == null)
                {
                    ModLogger.Error($"Cannot transfer capital {capital.Name}: ruling clan is null");
                    return;
                }

                Hero rulingClanLeader = conquererKingdom.RulingClan.Leader;

                // Force transfer to ruling clan
                ChangeOwnerOfSettlementAction.ApplyByDefault(rulingClanLeader, capital);

                ModLogger.Log($"Capital {capital.Name} transferred to ruling clan {conquererKingdom.RulingClan.Name}");
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Error transferring capital {capital.Name} to ruling clan", ex);
            }
        }

        /// <summary>
        /// Transfers all settlements of the defeated kingdom to the conquerer.
        /// </summary>
        private void TransferAllSettlements(Kingdom defeatedKingdom, Kingdom conquererKingdom)
        {
            try
            {
                var settlementsToTransfer = defeatedKingdom.Settlements.ToList();

                foreach (Settlement settlement in settlementsToTransfer)
                {
                    if (settlement.OwnerClan?.Kingdom == defeatedKingdom)
                    {
                        // Transfer to conquerer's ruling clan
                        ChangeOwnerOfSettlementAction.ApplyByDefault(conquererKingdom.RulingClan.Leader, settlement);
                        ModLogger.Log($"Transferred {settlement.Name} from {defeatedKingdom.Name} to {conquererKingdom.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Error transferring settlements from {defeatedKingdom?.Name}", ex);
            }
        }

        /// <summary>
        /// Vassalizes all clans from the defeated kingdom to the conquerer.
        /// </summary>
        private void VassalizeClans(Kingdom defeatedKingdom, Kingdom conquererKingdom)
        {
            try
            {
                var clansToVassalize = defeatedKingdom.Clans.ToList();

                foreach (Clan clan in clansToVassalize)
                {
                    if (clan != null && !clan.IsEliminated && clan != defeatedKingdom.RulingClan)
                    {
                        // Make clan join conquerer kingdom as vassal
                        ChangeKingdomAction.ApplyByJoinToKingdom(clan, conquererKingdom, false);
                        ModLogger.Log($"Vassalized clan {clan.Name} to {conquererKingdom.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Error vassalizing clans from {defeatedKingdom?.Name}", ex);
            }
        }

        /// <summary>
        /// Destroys the defeated kingdom completely.
        /// </summary>
        private void DestroyKingdom(Kingdom defeatedKingdom)
        {
            try
            {
                if (defeatedKingdom == null || defeatedKingdom.IsEliminated)
                    return;

                DestroyKingdomAction.Apply(defeatedKingdom);
                ModLogger.Log($"Kingdom {defeatedKingdom.Name} has been destroyed");
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Error destroying kingdom {defeatedKingdom?.Name}", ex);
            }
        }

        /// <summary>
        /// Notifies the player of the kingdom conquest.
        /// </summary>
        private void NotifyKingdomConquest(Settlement capital, Kingdom defeatedKingdom, Kingdom conquererKingdom, Hero capturerHero)
        {
            try
            {
                string message = $"{capital.Name} has fallen! {defeatedKingdom.Name} is no more!";

                InformationManager.DisplayMessage(new InformationMessage(message, Colors.Red));

                if (capturerHero == Hero.MainHero)
                {
                    InformationManager.DisplayMessage(new InformationMessage(
                        $"You have conquered {defeatedKingdom.Name} by capturing their capital!",
                        Colors.Gold
                    ));
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error notifying kingdom conquest", ex);
            }
        }
    }
}
