using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using KingdomCapitals.Core;
using KingdomCapitals.Utils;
using KingdomCapitals.Constants;

namespace KingdomCapitals.Behaviors
{
    /// <summary>
    /// Handles automatic transfer of capital ownership when rulers change.
    /// Monitors hero deaths and clan kingdom changes to ensure capitals remain with ruling clans.
    /// </summary>
    public class CapitalManagementBehavior : CampaignBehaviorBase
    {
        /// <summary>
        /// Registers event listeners for hero deaths and clan kingdom changes.
        /// </summary>
        public override void RegisterEvents()
        {
            CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, OnHeroKilled);
            CampaignEvents.OnClanChangedKingdomEvent.AddNonSerializedListener(this, OnClanChangedKingdom);
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
        /// Called when a hero is killed. Handles capital transfer if the deceased was a ruler.
        /// Schedules capital transfer to the new ruler on the next daily tick.
        /// </summary>
        /// <param name="victim">The hero who was killed.</param>
        /// <param name="killer">The hero who killed the victim (may be null).</param>
        /// <param name="detail">Details about how the character was killed.</param>
        /// <param name="showNotification">Whether to show notification to the player.</param>
        private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
        {
            try
            {
                if (victim?.Clan?.Kingdom == null)
                    return;

                Kingdom kingdom = victim.Clan.Kingdom;

                // Check if the victim was the ruler
                if (kingdom.Leader != victim)
                    return;

                Settlement capital = CapitalManager.GetCapital(kingdom);
                if (capital == null)
                    return;

                // Wait for new ruler to be appointed
                CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, () =>
                {
                    TransferCapitalToNewRuler(capital, kingdom);
                });

                ModLogger.Log(string.Format(Messages.Log.RulerKilledTransferScheduledFormat, victim.Name, kingdom.Name));
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in OnHeroKilled", ex);
            }
        }

        /// <summary>
        /// Transfers capital to the new ruler of a kingdom.
        /// Called after a ruler dies or the ruling clan changes.
        /// </summary>
        /// <param name="capital">The capital settlement to transfer.</param>
        /// <param name="kingdom">The kingdom whose capital needs transfer.</param>
        private void TransferCapitalToNewRuler(Settlement capital, Kingdom kingdom)
        {
            try
            {
                if (kingdom == null || kingdom.IsEliminated)
                {
                    ModLogger.Log(string.Format(Messages.Log.KingdomEliminatedTransferCancelled, capital.Name));
                    return;
                }

                Hero newRuler = kingdom.Leader;
                if (newRuler == null)
                {
                    ModLogger.Warning(string.Format(Messages.Warnings.NoRulerFound, kingdom.Name));
                    return;
                }

                // Transfer capital to new ruler
                if (capital.OwnerClan != newRuler.Clan)
                {
                    CapitalManager.TransferCapitalOwnership(newRuler, capital);
                    ModLogger.Log(string.Format(Messages.Log.CapitalTransferredToNewRulerFormat, capital.Name, newRuler.Name));
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Error transferring capital {capital?.Name} to new ruler", ex);
            }
        }

        /// <summary>
        /// Called when a clan changes kingdom (e.g., ruler's clan leaves).
        /// Ensures capital is transferred if the ruling clan leaves their kingdom.
        /// </summary>
        /// <param name="clan">The clan that changed kingdoms.</param>
        /// <param name="oldKingdom">The kingdom the clan left.</param>
        /// <param name="newKingdom">The kingdom the clan joined.</param>
        /// <param name="detail">Details about the kingdom change.</param>
        /// <param name="showNotification">Whether to show notification to the player.</param>
        private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
        {
            try
            {
                // If the ruling clan left the kingdom, handle capital transfer
                if (oldKingdom != null && clan == oldKingdom.RulingClan)
                {
                    Settlement capital = CapitalManager.GetCapital(oldKingdom);
                    if (capital != null && !oldKingdom.IsEliminated)
                    {
                        TransferCapitalToNewRuler(capital, oldKingdom);
                    }
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in OnClanChangedKingdom", ex);
            }
        }
    }
}
