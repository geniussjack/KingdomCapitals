using HarmonyLib;
using KingdomCapitals.Utils;
using KingdomCapitals.ViewModels;
using SandBox.ViewModelCollection.Nameplate;
using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace KingdomCapitals.Patches
{
    /// <summary>
    /// Harmony patch that replaces standard settlement nameplates with our custom version.
    /// Uses MBBindingList interceptor to swap ViewModels when they're added to collection.
    /// </summary>
    [HarmonyPatch(typeof(SettlementNameplatesVM))]
    [HarmonyPatch(MethodType.Constructor)]
    [HarmonyPatch(new Type[] { typeof(Camera), typeof(Action<Vec2>) })]
    public static class SettlementNameplatesVMPatch
    {
        static bool Prepare()
        {
            ModLogger.Log("SettlementNameplatesVMPatch: Preparing to inject capital nameplate system");
            return true;
        }

        /// <summary>
        /// Postfix patch - replaces Nameplates collection with our interceptor.
        /// </summary>
        static void Postfix(SettlementNameplatesVM __instance)
        {
            try
            {
                if (__instance == null)
                {
                    ModLogger.Log("SettlementNameplatesVM instance is null");
                    return;
                }

                // Replace the collection with our interceptor
                var interceptor = new CapitalNameplateInterceptor();
                __instance.Nameplates = interceptor;

                ModLogger.Log("Successfully injected CapitalNameplateInterceptor");
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in SettlementNameplatesVMPatch", ex);
            }
        }
    }

    /// <summary>
    /// Interceptor that replaces standard SettlementNameplateVM with CapitalSettlementNameplateVM.
    /// </summary>
    public class CapitalNameplateInterceptor : MBBindingList<SettlementNameplateVM>
    {
        protected override void InsertItem(int index, SettlementNameplateVM item)
        {
            try
            {
                if (item == null)
                {
                    base.InsertItem(index, item);
                    return;
                }

                // Use Traverse to extract private fields from the original ViewModel
                Traverse traverse = Traverse.Create(item);

                Settlement settlement = item.Settlement;  // Public property
                GameEntity targetEntity = traverse.Field("_targetEntity").GetValue<GameEntity>();
                Camera camera = traverse.Field("_camera").GetValue<Camera>();
                Action<Vec2> fastMoveCameraToPosition = traverse.Field("_fastMoveCameraToPosition").GetValue<Action<Vec2>>();

                // Create our custom ViewModel if we successfully extracted all fields
                if (settlement != null && targetEntity != null && camera != null && fastMoveCameraToPosition != null)
                {
                    var customVM = new CapitalSettlementNameplateVM(
                        settlement,
                        targetEntity,
                        camera,
                        fastMoveCameraToPosition
                    );

                    base.InsertItem(index, customVM);
                    ModLogger.Log($"Replaced nameplate for: {settlement.Name.ToString()} (IsCapital: {customVM.IsCapital})");
                }
                else
                {
                    // Fallback to original if extraction failed
                    base.InsertItem(index, item);
                    string settlementName = settlement != null ? settlement.Name.ToString() : "unknown";
                    ModLogger.Log($"Using original nameplate for: {settlementName}");
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in CapitalNameplateInterceptor.InsertItem", ex);
                base.InsertItem(index, item);
            }
        }
    }
}
