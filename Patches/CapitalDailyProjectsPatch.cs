using HarmonyLib;
using KingdomCapitals.Core;
using KingdomCapitals.Models;
using KingdomCapitals.Utils;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;

namespace KingdomCapitals.Patches
{
    /// <summary>
    /// Doubles the food bonus from daily defaults (Irrigation) for capitals.
    /// Daily defaults only work when no building construction is in progress.
    /// </summary>
    [HarmonyPatch(typeof(DefaultSettlementFoodModel), "CalculateTownFoodStocksChange")]
    public static class CapitalDailyFoodBonus_Patch
    {
        private static bool Prepare()
        {
            ModLogger.Log("CapitalDailyFoodBonus_Patch: ENABLED - Daily food projects doubled for capitals");
            return true;
        }

        /// <summary>
        /// Postfix patch - doubles daily default food bonus for capitals.
        /// Only affects the Irrigation daily default project.
        /// </summary>
        private static void Postfix(Town town, ref float __result)
        {
            try
            {
                Settlement settlement = town?.Settlement;
                if (settlement == null || !CapitalManager.IsCapital(settlement))
                {
                    return;
                }

                // Check if there's active construction - daily defaults only work when nothing is being built
                if (town.CurrentBuilding != null)
                {
                    return; // Construction in progress, daily defaults not active
                }

                // Check if Irrigation daily default is selected
                if (town.CurrentDefaultBuilding != null &&
                    town.CurrentDefaultBuilding.BuildingType.StringId.Contains("daily_food"))
                {
                    // Double the food bonus from daily default
                    float originalBonus = __result;
                    __result *= 2f;

                    if (ModSettings.Instance?.EnableDebugLogging == true)
                    {
                        ModLogger.Log($"Capital {settlement.Name}: Doubled daily food bonus from {originalBonus} to {__result}");
                    }
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in CapitalDailyFoodBonus_Patch", ex);
            }
        }
    }

    /// <summary>
    /// Doubles the prosperity bonus from daily defaults (Housing) for capitals.
    /// </summary>
    [HarmonyPatch(typeof(DefaultSettlementProsperityModel), "CalculateProsperityChange")]
    public static class CapitalDailyProsperityBonus_Patch
    {
        private static bool Prepare()
        {
            ModLogger.Log("CapitalDailyProsperityBonus_Patch: ENABLED - Daily prosperity projects doubled for capitals");
            return true;
        }

        /// <summary>
        /// Postfix patch - doubles daily default prosperity bonus for capitals.
        /// Only affects the Housing daily default project.
        /// </summary>
        private static void Postfix(Town town, ref float __result)
        {
            try
            {
                Settlement settlement = town?.Settlement;
                if (settlement == null || !CapitalManager.IsCapital(settlement))
                {
                    return;
                }

                // Check if there's active construction
                if (town.CurrentBuilding != null)
                {
                    return;
                }

                // Check if Housing daily default is selected
                if (town.CurrentDefaultBuilding != null &&
                    town.CurrentDefaultBuilding.BuildingType.StringId.Contains("daily_prosperity"))
                {
                    float originalBonus = __result;
                    __result *= 2f;

                    if (ModSettings.Instance?.EnableDebugLogging == true)
                    {
                        ModLogger.Log($"Capital {settlement.Name}: Doubled daily prosperity bonus from {originalBonus} to {__result}");
                    }
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in CapitalDailyProsperityBonus_Patch", ex);
            }
        }
    }

    /// <summary>
    /// Doubles the loyalty bonus from daily defaults (Festival and Games) for capitals.
    /// </summary>
    [HarmonyPatch(typeof(DefaultSettlementLoyaltyModel), "CalculateLoyaltyChange")]
    public static class CapitalDailyLoyaltyBonus_Patch
    {
        private static bool Prepare()
        {
            ModLogger.Log("CapitalDailyLoyaltyBonus_Patch: ENABLED - Daily loyalty projects doubled for capitals");
            return true;
        }

        /// <summary>
        /// Postfix patch - doubles daily default loyalty bonus for capitals.
        /// Only affects the Festival and Games daily default project.
        /// </summary>
        private static void Postfix(Town town, ref float __result)
        {
            try
            {
                Settlement settlement = town?.Settlement;
                if (settlement == null || !CapitalManager.IsCapital(settlement))
                {
                    return;
                }

                // Check if there's active construction
                if (town.CurrentBuilding != null)
                {
                    return;
                }

                // Check if Festival and Games daily default is selected
                if (town.CurrentDefaultBuilding != null &&
                    town.CurrentDefaultBuilding.BuildingType.StringId.Contains("daily_loyalty"))
                {
                    float originalBonus = __result;
                    __result *= 2f;

                    if (ModSettings.Instance?.EnableDebugLogging == true)
                    {
                        ModLogger.Log($"Capital {settlement.Name}: Doubled daily loyalty bonus from {originalBonus} to {__result}");
                    }
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in CapitalDailyLoyaltyBonus_Patch", ex);
            }
        }
    }

    /// <summary>
    /// Doubles the militia bonus from daily defaults (Militia Training) for capitals.
    /// </summary>
    [HarmonyPatch(typeof(DefaultSettlementMilitiaModel), "CalculateMilitiaChange")]
    public static class CapitalDailyMilitiaBonus_Patch
    {
        private static bool Prepare()
        {
            ModLogger.Log("CapitalDailyMilitiaBonus_Patch: ENABLED - Daily militia projects doubled for capitals");
            return true;
        }

        /// <summary>
        /// Postfix patch - doubles daily default militia bonus for capitals.
        /// Only affects the Militia Training daily default project.
        /// </summary>
        private static void Postfix(Settlement settlement, ref float __result)
        {
            try
            {
                if (settlement == null || !settlement.IsTown || !CapitalManager.IsCapital(settlement))
                {
                    return;
                }

                // Check if there's active construction
                if (settlement.Town.CurrentBuilding != null)
                {
                    return;
                }

                // Check if Militia Training daily default is selected
                if (settlement.Town.CurrentDefaultBuilding != null &&
                    settlement.Town.CurrentDefaultBuilding.BuildingType.StringId.Contains("daily_militia"))
                {
                    float originalBonus = __result;
                    __result *= 2f;

                    if (ModSettings.Instance?.EnableDebugLogging == true)
                    {
                        ModLogger.Log($"Capital {settlement.Name}: Doubled daily militia bonus from {originalBonus} to {__result}");
                    }
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in CapitalDailyMilitiaBonus_Patch", ex);
            }
        }
    }
}
