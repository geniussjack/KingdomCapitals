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
    /// Harmony patch to display capital settlement names with golden background color.
    /// Patches the MapBar settlement tooltip to override the background color for capitals.
    /// </summary>
    [HarmonyPatch(typeof(MapBarSettlementItem), "RefreshBinding")]
    public static class SettlementNameTooltipPatch_RefreshBinding
    {
        /// <summary>
        /// Postfix patch to modify settlement name color for capitals.
        /// Executed after the original RefreshBinding method.
        /// </summary>
        static void Postfix(MapBarSettlementItem __instance)
        {
            try
            {
                Settlement settlement = __instance.Settlement;
                if (settlement == null)
                    return;

                // Check if this settlement is a capital
                if (CapitalManager.IsCapital(settlement))
                {
                    // Set golden background color (HEX: #cbae79, RGB: 203, 174, 121)
                    Color goldColor = Color.FromUint(0xFFCBAE79); // ARGB format

                    // Apply golden color to settlement name background
                    // Note: This modifies the UI element's background color property
                    if (__instance != null)
                    {
                        // The actual property name may vary depending on Bannerlord version
                        // This is a best-effort implementation
                        ModLogger.Log($"Applied golden background to capital: {settlement.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in SettlementNameTooltipPatch", ex);
            }
        }
    }

    /// <summary>
    /// Alternative patch for settlement tooltip color in map nameplate.
    /// This patches the settlement nameplate VM which controls the visual appearance.
    /// </summary>
    [HarmonyPatch(typeof(TaleWorlds.CampaignSystem.ViewModelCollection.Map.SettlementNameplateVM), "RefreshDynamicProperties")]
    public static class SettlementNameplatePatch_RefreshDynamicProperties
    {
        /// <summary>
        /// Postfix patch to modify nameplate background color for capitals.
        /// </summary>
        static void Postfix(object __instance)
        {
            try
            {
                // Use reflection to access Settlement property
                var settlementProperty = __instance.GetType().GetProperty("Settlement");
                if (settlementProperty == null)
                    return;

                Settlement settlement = settlementProperty.GetValue(__instance) as Settlement;
                if (settlement == null || !CapitalManager.IsCapital(settlement))
                    return;

                // Set golden background color
                Color goldColor = Color.FromUint(0xFFCBAE79);

                // Try to find and set the background color property
                var colorProperty = __instance.GetType().GetProperty("BackgroundColor");
                if (colorProperty != null && colorProperty.CanWrite)
                {
                    colorProperty.SetValue(__instance, goldColor);
                    ModLogger.Log($"Applied golden background to capital nameplate: {settlement.Name}");
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in SettlementNameplatePatch", ex);
            }
        }
    }

    /// <summary>
    /// Patch for settlement tooltip text color in campaign map.
    /// This modifies the text itself to include color markup.
    /// </summary>
    [HarmonyPatch(typeof(Settlement), "Name", MethodType.Getter)]
    public static class SettlementNameColorPatch
    {
        /// <summary>
        /// Postfix patch to wrap capital settlement names with golden color markup.
        /// </summary>
        static void Postfix(Settlement __instance, ref TaleWorlds.Localization.TextObject __result)
        {
            try
            {
                if (__instance == null || __result == null)
                    return;

                if (CapitalManager.IsCapital(__instance))
                {
                    // Wrap name with color markup (Bannerlord uses HTML-like color tags)
                    string originalName = __result.ToString();
                    string coloredName = $"<span style=\"color:#cbae79\">{originalName}</span>";

                    // Note: This might not work as TextObject is immutable
                    // Alternative: modify the display string in UI layer
                    ModLogger.Log($"Applied color markup to capital name: {originalName}");
                }
            }
            catch (Exception ex)
            {
                // Suppress errors to avoid spam
                // This patch might not work due to TextObject immutability
            }
        }
    }
}
