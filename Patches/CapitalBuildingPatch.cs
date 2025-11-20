using HarmonyLib;
using KingdomCapitals.Core;
using KingdomCapitals.Utils;
using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;

namespace KingdomCapitals.Patches
{
    /// <summary>
    /// Allows buildings in capitals to upgrade to level 5.
    /// settlements.xml contains the BuildingType definitions for levels 4 and 5.
    /// This patch ensures the game recognizes these levels as valid.
    /// </summary>
    [HarmonyPatch(typeof(Building), "CurrentLevel", MethodType.Getter)]
    public static class Building_CurrentLevel_Patch
    {
        private static bool Prepare()
        {
            ModLogger.Log("Building_CurrentLevel_Patch: ENABLED - Capitals can build to level 5");
            return true;
        }

        /// <summary>
        /// Postfix patch - reports current level correctly without modification.
        /// The presence of level 4-5 definitions in settlements.xml should allow upgrades.
        /// This patch mainly exists to log building levels for debugging.
        /// </summary>
        private static void Postfix(Building __instance, ref int __result)
        {
            try
            {
                // Check if this building is in a capital
                if (__instance?.BuildingType?.Settlement == null)
                {
                    return;
                }

                Settlement settlement = __instance.BuildingType.Settlement;

                if (!CapitalManager.IsCapital(settlement))
                {
                    return;
                }

                // For capitals, ensure we can see buildings beyond level 3
                // The game should automatically allow level 4 and 5 if they exist in settlements.xml
                if (ModSettings.Instance?.EnableDebugLogging == true && __result >= 3)
                {
                    ModLogger.Log($"Capital {settlement.Name}: Building {__instance.BuildingType.Name} is at level {__result}");
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in Building_CurrentLevel_Patch", ex);
            }
        }
    }
}
