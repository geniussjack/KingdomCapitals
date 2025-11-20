using HarmonyLib;
using KingdomCapitals.Utils;
using KingdomCapitals.ViewModels;
using SandBox.ViewModelCollection.Nameplate;
using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace KingdomCapitals.Patches
{
    /// <summary>
    /// Harmony patch that injects our custom CapitalNameplateListInterceptor
    /// to replace standard settlement nameplates with custom ones showing crown icons.
    /// </summary>
    [HarmonyPatch(typeof(SettlementNameplatesVM))]
    [HarmonyPatch(MethodType.Constructor)]
    [HarmonyPatch(new Type[] { typeof(Camera), typeof(Action<Vec2, float>) })]
    public static class SettlementNameplatesVMPatch
    {
        /// <summary>
        /// Determines if the patch should be applied.
        /// </summary>
        static bool Prepare()
        {
            ModLogger.Log("SettlementNameplatesVMPatch ENABLED: Will inject custom nameplate ViewModel");
            return true;
        }

        /// <summary>
        /// Postfix patch that runs after SettlementNameplatesVM constructor.
        /// Replaces the Nameplates collection with our interceptor.
        /// </summary>
        static void Postfix(SettlementNameplatesVM __instance)
        {
            try
            {
                if (__instance == null)
                {
                    ModLogger.Log("SettlementNameplatesVM instance is null, skipping patch");
                    return;
                }

                // Replace the Nameplates collection with our interceptor
                // This will cause all subsequent nameplate additions to use our custom ViewModel
                __instance.Nameplates = new CapitalNameplateListInterceptor();

                ModLogger.Log("Successfully replaced Nameplates collection with CapitalNameplateListInterceptor");
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in SettlementNameplatesVMPatch.Postfix", ex);
            }
        }
    }
}
