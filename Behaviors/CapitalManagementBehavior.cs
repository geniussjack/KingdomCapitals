using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using KingdomCapitals.Core;
using KingdomCapitals.Utils;

namespace KingdomCapitals.Behaviors
{
    /// <summary>
    /// Handles automatic transfer of capital ownership when rulers change.
    /// </summary>
    public class CapitalManagementBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, OnHeroKilled);
            CampaignEvents.OnClanChangedKingdomEvent.AddNonSerializedListener(this, OnClanChangedKingdom);
        }

        public override void SyncData(IDataStore dataStore)
        {
            // No data to sync
        }

        /// <summary>
        /// Called when a hero is killed. Handles capital transfer if the deceased was a ruler.
        /// </summary>
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

                ModLogger.Log($"Ruler {victim.Name} of {kingdom.Name} was killed. Capital transfer scheduled.");
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in OnHeroKilled", ex);
            }
        }

        /// <summary>
        /// Transfers capital to the new ruler of a kingdom.
        /// </summary>
        private void TransferCapitalToNewRuler(Settlement capital, Kingdom kingdom)
        {
            try
            {
                if (kingdom == null || kingdom.IsEliminated)
                {
                    ModLogger.Log($"Kingdom eliminated, capital {capital.Name} transfer cancelled");
                    return;
                }

                Hero newRuler = kingdom.Leader;
                if (newRuler == null)
                {
                    ModLogger.Warning($"No new ruler found for {kingdom.Name}, capital transfer delayed");
                    return;
                }

                // Transfer capital to new ruler
                if (capital.OwnerClan != newRuler.Clan)
                {
                    CapitalManager.TransferCapitalOwnership(newRuler, capital);
                    ModLogger.Log($"Capital {capital.Name} transferred to new ruler {newRuler.Name}");
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Error transferring capital {capital?.Name} to new ruler", ex);
            }
        }

        /// <summary>
        /// Called when a clan changes kingdom (e.g., ruler's clan leaves).
        /// </summary>
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
