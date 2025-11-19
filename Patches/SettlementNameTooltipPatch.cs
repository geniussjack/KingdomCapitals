using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using KingdomCapitals.Core;
using KingdomCapitals.Utils;

namespace KingdomCapitals.Patches
{
    /// <summary>
    /// Harmony patches to display capital settlement names with golden background color.
    ///
    /// IMPORTANT: These patches are version-dependent and may require adjustment for Bannerlord v1.2.12
    /// The exact UI classes and methods may differ between game versions.
    ///
    /// If these patches cause compilation errors:
    /// 1. Check the actual UI class names using a decompiler (ILSpy, dnSpy)
    /// 2. Update the class names and namespaces accordingly
    /// 3. Or disable these patches if UI modification is not critical
    /// </summary>

    /* COMMENTED OUT: UI classes may not exist in v1.2.12 - requires version-specific implementation

    [HarmonyPatch(typeof(TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar.MapBarSettlementItem), "RefreshBinding")]
    public static class SettlementNameTooltipPatch_RefreshBinding
    {
        static void Postfix(object __instance)
        {
            try
            {
                // Use reflection to access Settlement property
                var settlementProperty = __instance.GetType().GetProperty("Settlement");
                if (settlementProperty == null)
                    return;

                Settlement settlement = settlementProperty.GetValue(__instance) as Settlement;
                if (settlement == null)
                    return;

                if (CapitalManager.IsCapital(settlement))
                {
                    Color goldColor = Color.FromUint(0xFFCBAE79);
                    ModLogger.Log($"Applied golden background to capital: {settlement.Name}");
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in SettlementNameTooltipPatch", ex);
            }
        }
    }
    */

    /* COMMENTED OUT: SettlementNameplateVM may not exist in v1.2.12

    [HarmonyPatch(typeof(TaleWorlds.CampaignSystem.ViewModelCollection.Map.SettlementNameplateVM), "RefreshDynamicProperties")]
    public static class SettlementNameplatePatch_RefreshDynamicProperties
    {
        static void Postfix(object __instance)
        {
            try
            {
                var settlementProperty = __instance.GetType().GetProperty("Settlement");
                if (settlementProperty == null)
                    return;

                Settlement settlement = settlementProperty.GetValue(__instance) as Settlement;
                if (settlement == null || !CapitalManager.IsCapital(settlement))
                    return;

                Color goldColor = Color.FromUint(0xFFCBAE79);

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
    */

    /// <summary>
    /// Patch for settlement name display using reflection-based approach.
    /// This is a safer alternative that doesn't depend on specific UI classes.
    /// Note: May not work due to TextObject immutability - kept as reference.
    /// </summary>
    [HarmonyPatch(typeof(Settlement), "Name", MethodType.Getter)]
    public static class SettlementNameColorPatch
    {
        /// <summary>
        /// Attempts to add golden color markup to capital settlement names.
        /// WARNING: This may not work due to TextObject being immutable.
        /// </summary>
        static void Postfix(Settlement __instance, ref TaleWorlds.Localization.TextObject __result)
        {
            try
            {
                if (__instance == null || __result == null)
                    return;

                if (CapitalManager.IsCapital(__instance))
                {
                    // This is a placeholder - TextObject modification may not work
                    // Alternative: Implement a custom ViewModel or use a different approach
                    // For now, we just log that this is a capital
                    // ModLogger.Log($"Capital detected: {__instance.Name}");
                }
            }
            catch (Exception)
            {
                // Suppress errors to avoid log spam
            }
        }
    }

    /// <summary>
    /// TODO: Implement working UI patches for Bannerlord v1.2.12
    ///
    /// Recommended approach:
    /// 1. Use ILSpy to decompile TaleWorlds.CampaignSystem.ViewModelCollection.dll
    /// 2. Find the actual ViewModel classes used for settlement tooltips
    /// 3. Identify the methods called when hovering over settlements
    /// 4. Create patches targeting those specific methods
    ///
    /// Alternative approaches:
    /// - Create a custom ViewModel that extends the base settlement display
    /// - Use reflection at runtime to find and patch the correct classes
    /// - Modify settlement names directly (may cause save compatibility issues)
    /// </summary>
}
