using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar;
using TaleWorlds.Library;
using KingdomCapitals.Core;
using KingdomCapitals.Utils;

namespace KingdomCapitals.Patches
{
    /// <summary>
    /// Harmony patches to display capital settlement names with golden background color.
    ///
    /// Patches MapInfoVM class to override settlement color for capitals.
    /// Golden color: #cbae79 (RGB: 203, 174, 121)
    ///
    /// Based on Bannerlord v1.2.12 DLL analysis:
    /// - Class: TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar.MapInfoVM
    /// - Method: UpdateTargetSettlement(Settlement settlement)
    /// - Property: SettlementColor (Color type)
    /// </summary>
    [HarmonyPatch(typeof(MapInfoVM))]
    public static class SettlementNameTooltipPatch
    {
        /// <summary>
        /// Golden color for capital settlements (#cbae79)
        /// </summary>
        private static readonly Color GoldColor = new Color(203f / 255f, 174f / 255f, 121f / 255f, 1f);

        /// <summary>
        /// Patch for UpdateTargetSettlement method to apply golden color to capitals.
        /// This is the primary patch that modifies the settlement color when hovering over settlements on the campaign map.
        /// </summary>
        [HarmonyPatch("UpdateTargetSettlement")]
        [HarmonyPostfix]
        public static void UpdateTargetSettlement_Postfix(MapInfoVM __instance, Settlement settlement)
        {
            try
            {
                // Check if settlement is a capital
                if (settlement != null && CapitalManager.IsCapital(settlement))
                {
                    // Apply golden color to capital settlement
                    __instance.SettlementColor = GoldColor;

                    ModLogger.Log($"Applied golden color to capital tooltip: {settlement.Name}");
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in UpdateTargetSettlement_Postfix", ex);
            }
        }

        /// <summary>
        /// Alternative patch for GetSettlementColor private method.
        /// This provides a fallback in case the primary patch doesn't work.
        /// Directly overrides the color calculation for capital settlements.
        /// </summary>
        [HarmonyPatch("GetSettlementColor")]
        [HarmonyPostfix]
        public static void GetSettlementColor_Postfix(MapInfoVM __instance, Settlement settlement, ref Color __result)
        {
            try
            {
                // Override color for capitals
                if (settlement != null && CapitalManager.IsCapital(settlement))
                {
                    __result = GoldColor;
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in GetSettlementColor_Postfix", ex);
            }
        }
    }
}
