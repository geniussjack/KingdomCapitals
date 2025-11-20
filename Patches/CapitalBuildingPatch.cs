using HarmonyLib;
using KingdomCapitals.Core;
using KingdomCapitals.Models;
using KingdomCapitals.Utils;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;

namespace KingdomCapitals.Patches
{
    /// <summary>
    /// Patch for Building.IsCurrentlyDefault property getter.
    /// Allows capitals to build beyond default level 3 limit.
    /// </summary>
    [HarmonyPatch(typeof(Building), "IsCurrentlyDefault", MethodType.Getter)]
    public static class Building_IsCurrentlyDefault_Patch
    {
        private static bool Prepare()
        {
            bool enabled = ModSettings.Instance?.AllowCapitalBuildingExtensions ?? true;
            ModLogger.Log($"Building_IsCurrentlyDefault_Patch: {(enabled ? "ENABLED" : "DISABLED")}");
            return enabled;
        }

        /// <summary>
        /// Postfix patch - modifies the result to allow capitals to exceed level 3.
        /// </summary>
        private static void Postfix(Building __instance, ref bool __result)
        {
            try
            {
                // If already not at default (level > 3), don't change anything
                if (!__result)
                {
                    return;
                }

                // Check if this building belongs to a capital
                Settlement settlement = __instance.Town?.Settlement;
                if (settlement == null || !CapitalManager.IsCapital(settlement))
                {
                    return;
                }

                // Check if building can be upgraded further (level 4 or 5 exists)
                if (__instance.BuildingType == null)
                {
                    return;
                }

                // Check if the building type has more levels available
                int currentLevel = __instance.CurrentLevel;
                int maxPossibleLevel = GetMaxPossibleLevel(__instance.BuildingType);

                // If capital and current level is 3 but more levels exist, allow building
                if (currentLevel == 3 && maxPossibleLevel > 3)
                {
                    __result = false; // Not at default anymore, can build further
                    ModLogger.Log($"Capital {settlement.Name}: Allowing {__instance.BuildingType.Name} to upgrade beyond level 3 (max: {maxPossibleLevel})");
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in Building_IsCurrentlyDefault_Patch", ex);
            }
        }

        /// <summary>
        /// Gets the maximum possible level for a building type by counting building levels.
        /// </summary>
        private static int GetMaxPossibleLevel(BuildingType buildingType)
        {
            try
            {
                // Use Traverse to access private BuildingLevels array
                Traverse traverse = Traverse.Create(buildingType);

                // Try different possible field names
                object buildingLevels = traverse.Field("BuildingLevels").GetValue();

                if (buildingLevels == null)
                {
                    buildingLevels = traverse.Field("_buildingLevels").GetValue();
                }

                if (buildingLevels == null)
                {
                    buildingLevels = traverse.Field("buildingLevels").GetValue();
                }

                if (buildingLevels is Array levels)
                {
                    return levels.Length;
                }

                // Fallback: assume 3 levels if we can't determine
                return 3;
            }
            catch
            {
                return 3;
            }
        }
    }

    /// <summary>
    /// Patch for building effect amounts to provide doubled bonuses for capitals.
    /// Patches the Building.GetBuildingEffectAmount method.
    /// </summary>
    [HarmonyPatch(typeof(Building), "GetBuildingEffectAmount")]
    public static class Building_GetBuildingEffectAmount_Patch
    {
        private static bool Prepare()
        {
            bool enabled = ModSettings.Instance?.AllowCapitalBuildingExtensions ?? true;
            ModLogger.Log($"Building_GetBuildingEffectAmount_Patch: {(enabled ? "ENABLED" : "DISABLED")}");
            return enabled;
        }

        /// <summary>
        /// Postfix patch - doubles the building effect amount for capitals.
        /// </summary>
        private static void Postfix(Building __instance, ref float __result, BuildingEffectEnum effect)
        {
            try
            {
                // Check if this building belongs to a capital
                Settlement settlement = __instance.Town?.Settlement;
                if (settlement == null || !CapitalManager.IsCapital(settlement))
                {
                    return;
                }

                // Double the effect for capitals
                __result *= 2f;

                if (ModSettings.Instance?.EnableDebugLogging == true)
                {
                    ModLogger.Log($"Capital {settlement.Name}: Doubled {effect} bonus from {__instance.BuildingType?.Name} (original: {__result / 2f}, doubled: {__result})");
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in Building_GetBuildingEffectAmount_Patch", ex);
            }
        }
    }
}
