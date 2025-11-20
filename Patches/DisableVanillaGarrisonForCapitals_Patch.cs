using HarmonyLib;
using KingdomCapitals.Core;
using KingdomCapitals.Utils;
using System;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace KingdomCapitals.Patches
{
    /// <summary>
    /// Disables vanilla automatic garrison recruitment (+1 troop per day) for capitals.
    /// Allows our custom CapitalGarrisonBehavior to be the ONLY source of garrison growth.
    /// </summary>
    [HarmonyPatch(typeof(DefaultSettlementGarrisonModel), "CalculateGarrisonChangeAutoRecruitment")]
    public static class DisableVanillaGarrisonForCapitals_Patch
    {
        private static bool Prepare()
        {
            ModLogger.Log("DisableVanillaGarrisonForCapitals_Patch: ENABLED - Vanilla garrison growth disabled for capitals");
            return true;
        }

        /// <summary>
        /// Prefix patch - prevents vanilla garrison recruitment for capitals.
        /// Returns false to skip vanilla method execution for capitals.
        /// </summary>
        /// <param name="town">The town being processed.</param>
        /// <param name="__result">The garrison change amount (will be set to 0 for capitals).</param>
        /// <returns>False if capital (skip vanilla), true otherwise (execute vanilla).</returns>
        private static bool Prefix(Town town, ref int __result)
        {
            try
            {
                // Check if this town is a capital
                if (town == null || town.Settlement == null)
                {
                    return true; // Execute vanilla
                }

                if (!CapitalManager.IsCapital(town.Settlement))
                {
                    return true; // Not a capital, execute vanilla (+1 troop)
                }

                // This is a capital - disable vanilla recruitment
                // Our CapitalGarrisonBehavior will handle recruitment with +3 troops
                __result = 0; // No vanilla recruitment for capitals
                return false; // Skip vanilla method execution
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in DisableVanillaGarrisonForCapitals_Patch", ex);
                return true; // On error, execute vanilla to avoid breaking game
            }
        }
    }
}
