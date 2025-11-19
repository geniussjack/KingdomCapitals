using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using KingdomCapitals.Core;
using KingdomCapitals.Utils;
using KingdomCapitals.Models;

namespace KingdomCapitals.Behaviors
{
    /// <summary>
    /// Handles daily garrison reinforcement for capital cities.
    /// Adds troops based on prosperity and reduces food consumption.
    /// </summary>
    public class CapitalGarrisonBehavior : CampaignBehaviorBase
    {
        // Get settings from MCM
        private ModSettings Settings => ModSettings.Instance;

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, OnDailyTickSettlement);
        }

        public override void SyncData(IDataStore dataStore)
        {
            // No data to sync
        }

        /// <summary>
        /// Called daily for each settlement. Adds troops to capital garrisons.
        /// </summary>
        private void OnDailyTickSettlement(Settlement settlement)
        {
            try
            {
                // Only process towns that are capitals
                if (!settlement.IsTown || !CapitalManager.IsCapital(settlement))
                    return;

                // Check if settlement has food available
                if (settlement.Town.FoodStocks <= 0)
                {
                    ModLogger.Log($"Capital {settlement.Name} has no food, skipping garrison reinforcement");
                    return;
                }

                // Add troops to garrison
                AddDailyGarrisonReinforcement(settlement);

                // Reduce food consumption for capital garrison
                ReduceCapitalGarrisonFoodConsumption(settlement);
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Error in OnDailyTickSettlement for {settlement?.Name}", ex);
            }
        }

        /// <summary>
        /// Adds daily garrison reinforcement based on prosperity.
        /// </summary>
        private void AddDailyGarrisonReinforcement(Settlement settlement)
        {
            try
            {
                if (settlement.Town?.Owner?.Culture == null)
                    return;

                // Determine troop tier based on prosperity
                int troopTier = CalculateTroopTier(settlement.Town.Prosperity);

                // Get appropriate troop type for the faction
                CharacterObject troopType = GetTroopForFactionAndTier(settlement.Town.Owner.Culture, troopTier);

                if (troopType == null)
                {
                    ModLogger.Warning($"Could not find troop type for {settlement.Name} (tier {troopTier})");
                    return;
                }

                // Add troops to garrison
                int reinforcementCount = Settings?.DailyGarrisonReinforcement ?? 3;
                settlement.Town.GarrisonParty?.MemberRoster.AddToCounts(troopType, reinforcementCount, false, 0, 0, true, -1);

                ModLogger.LogGarrisonReinforcement(settlement, reinforcementCount, troopType.Name.ToString());
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Error adding garrison reinforcement to {settlement.Name}", ex);
            }
        }

        /// <summary>
        /// Calculates troop tier based on settlement prosperity.
        /// Every X prosperity = +1 tier (max tier 6), where X is configured in MCM.
        /// </summary>
        private int CalculateTroopTier(float prosperity)
        {
            int prosperityPerTier = Settings?.ProsperityPerTroopTier ?? 2500;
            int tier = (int)(prosperity / prosperityPerTier);
            return (int)MathF.Clamp(tier, 0, 6); // Bannerlord max tier is 6
        }

        /// <summary>
        /// Gets the appropriate troop type for a culture and tier.
        /// Handles troop tree branching by selecting randomly.
        /// </summary>
        private CharacterObject GetTroopForFactionAndTier(CultureObject culture, int targetTier)
        {
            try
            {
                // Get basic troop for culture
                CharacterObject basicTroop = culture.BasicTroop;
                if (basicTroop == null)
                    return null;

                CharacterObject currentTroop = basicTroop;

                // Upgrade to target tier
                for (int currentTier = 0; currentTier < targetTier; currentTier++)
                {
                    var upgrades = currentTroop.UpgradeTargets;

                    if (upgrades == null || upgrades.Length == 0)
                        break; // No more upgrades available

                    // Randomly select upgrade path if branching occurs
                    currentTroop = upgrades[MBRandom.RandomInt(upgrades.Length)];
                }

                return currentTroop;
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Error getting troop for culture {culture?.Name} tier {targetTier}", ex);
                return culture?.BasicTroop; // Fallback to basic troop
            }
        }

        /// <summary>
        /// Reduces food consumption for capital garrisons.
        /// This is applied daily to counteract the increased garrison size.
        /// </summary>
        private void ReduceCapitalGarrisonFoodConsumption(Settlement settlement)
        {
            try
            {
                if (settlement.Town?.GarrisonParty == null)
                    return;

                float consumptionMultiplier = Settings?.GarrisonFoodConsumptionMultiplier ?? 0.5f;

                // Calculate food consumption reduction
                // Base consumption is approximately 1 food per party member per day
                float normalConsumption = settlement.Town.GarrisonParty.Party.NumberOfAllMembers * 1.0f;
                float reducedConsumption = normalConsumption * consumptionMultiplier;
                float foodSavings = normalConsumption - reducedConsumption;

                // Add food back to compensate for reduced consumption
                // Note: This is a workaround since we can't directly modify consumption rates
                if (foodSavings > 0)
                {
                    settlement.Town.FoodStocks += foodSavings;
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Error reducing food consumption for {settlement.Name}", ex);
            }
        }
    }
}
