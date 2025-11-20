using HarmonyLib;
using KingdomCapitals.Core;
using KingdomCapitals.Models;
using KingdomCapitals.Utils;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace KingdomCapitals.Patches
{
    /// <summary>
    /// Harmony patch to make capital garrison wages free (zero cost).
    /// Patches MobileParty.TotalWage property getter to return 0 for capital garrisons.
    /// </summary>
    [HarmonyPatch(typeof(MobileParty), "TotalWage", MethodType.Getter)]
    public static class CapitalGarrisonWagePatch
    {
        private static bool Prepare()
        {
            // Always enable this patch as it's core functionality
            ModLogger.Log("CapitalGarrisonWagePatch: ENABLED - Capital garrisons will be free");
            return true;
        }

        /// <summary>
        /// Postfix patch - sets garrison wage to 0 for capital settlements.
        /// </summary>
        /// <param name="__instance">The MobileParty instance (garrison party).</param>
        /// <param name="__result">The total wage amount (will be set to 0 for capitals).</param>
        private static void Postfix(MobileParty __instance, ref int __result)
        {
            try
            {
                // Only modify garrison parties
                if (!__instance.IsGarrison)
                {
                    return;
                }

                // Get the settlement this garrison belongs to
                Settlement settlement = __instance.CurrentSettlement;
                if (settlement == null)
                {
                    return;
                }

                // Check if this settlement is a capital
                if (!CapitalManager.IsCapital(settlement))
                {
                    return;
                }

                // Store original wage for logging
                int originalWage = __result;

                // Set wage to 0 for capital garrisons
                __result = 0;

                if (ModSettings.Instance?.EnableDebugLogging == true && originalWage > 0)
                {
                    ModLogger.Log($"Capital {settlement.Name}: Garrison wage reduced from {originalWage} to 0 (FREE)");
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in CapitalGarrisonWagePatch", ex);
            }
        }
    }

    /// <summary>
    /// Alternative patch for garrison wage calculation via PartyWageModel.
    /// This patches the model method that calculates total party wages.
    /// </summary>
    [HarmonyPatch(typeof(DefaultPartyWageModel), "GetTotalWage")]
    public static class DefaultPartyWageModel_GetTotalWage_Patch
    {
        private static bool Prepare()
        {
            ModLogger.Log("DefaultPartyWageModel_GetTotalWage_Patch: ENABLED - Backup patch for capital garrison wages");
            return true;
        }

        /// <summary>
        /// Postfix patch - ensures garrison wage is 0 for capitals.
        /// This is a backup in case the MobileParty.TotalWage patch doesn't cover all cases.
        /// </summary>
        private static void Postfix(MobileParty party, ref int __result)
        {
            try
            {
                // Only modify garrison parties
                if (party == null || !party.IsGarrison)
                {
                    return;
                }

                // Get the settlement this garrison belongs to
                Settlement settlement = party.CurrentSettlement;
                if (settlement == null)
                {
                    return;
                }

                // Check if this settlement is a capital
                if (!CapitalManager.IsCapital(settlement))
                {
                    return;
                }

                // Set wage to 0 for capital garrisons
                int originalWage = __result;
                __result = 0;

                if (ModSettings.Instance?.EnableDebugLogging == true && originalWage > 0)
                {
                    ModLogger.Log($"[PartyWageModel] Capital {settlement.Name}: Garrison wage reduced from {originalWage} to 0 (FREE)");
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in DefaultPartyWageModel_GetTotalWage_Patch", ex);
            }
        }
    }
}
