using HarmonyLib;
using KingdomCapitals.Core;
using KingdomCapitals.Utils;
using KingdomCapitals.ViewModels;
using SandBox.ViewModelCollection.Nameplate;
using System;
using System.Collections.Generic;
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
        private static bool Prepare()
        {
            ModLogger.Log("SettlementNameplatesVMPatch: Preparing to inject capital nameplate system");
            return true;
        }

        /// <summary>
        /// Postfix patch - DISABLED.
        /// Cannot extract private fields (_targetEntity, _camera) needed for CapitalSettlementNameplateVM constructor.
        /// Using alternative approach: programmatic widget injection in View layer.
        /// </summary>
        private static void Postfix(SettlementNameplatesVM __instance)
        {
            // DISABLED: Field extraction fails because fields are null or have different names
            // Even when extraction succeeds, replacing Nameplates breaks UI rendering
            ModLogger.Log("SettlementNameplatesVMPatch.Postfix: DISABLED (using View layer widget injection)");
            return;
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

                // Skip if already our custom ViewModel
                if (item is CapitalSettlementNameplateVM)
                {
                    base.InsertItem(index, item);
                    return;
                }

                // Use Traverse to extract private fields from the original ViewModel
                Traverse traverse = Traverse.Create(item);

                Settlement settlement = item.Settlement;  // Public property

                // DEBUG: List all available fields (only for first settlement)
                if (index == 0)
                {
                    List<string> fields = traverse.Fields();
                    ModLogger.Log($"[DEBUG] Available fields in SettlementNameplateVM: {string.Join(", ", fields)}");
                }

                // Try to extract private fields - try multiple possible field names
                GameEntity targetEntity = traverse.Field("_targetEntity").GetValue<GameEntity>();
                if (targetEntity == null)
                {
                    targetEntity = traverse.Field("targetEntity").GetValue<GameEntity>();
                }

                if (targetEntity == null)
                {
                    targetEntity = traverse.Field("m_targetEntity").GetValue<GameEntity>();
                }

                Camera camera = traverse.Field("_camera").GetValue<Camera>();
                if (camera == null)
                {
                    camera = traverse.Field("camera").GetValue<Camera>();
                }

                if (camera == null)
                {
                    camera = traverse.Field("m_camera").GetValue<Camera>();
                }

                Action<Vec2> fastMoveCameraToPosition = traverse.Field("_fastMoveCameraToPosition").GetValue<Action<Vec2>>();
                fastMoveCameraToPosition ??= traverse.Field("fastMoveCameraToPosition").GetValue<Action<Vec2>>();
                fastMoveCameraToPosition ??= traverse.Field("m_fastMoveCameraToPosition").GetValue<Action<Vec2>>();

                // Debug logging
                string settlementName = settlement != null ? settlement.Name.ToString() : "NULL";
                bool isCapital = settlement != null && CapitalManager.IsCapital(settlement);
                ModLogger.Log($"[DEBUG] {settlementName}: targetEntity={targetEntity != null}, camera={camera != null}, fastMove={fastMoveCameraToPosition != null}, IsCapital={isCapital}");

                // Create our custom ViewModel if we successfully extracted all fields
                if (settlement != null && targetEntity != null && camera != null && fastMoveCameraToPosition != null)
                {
                    CapitalSettlementNameplateVM customVM = new(
                        settlement,
                        targetEntity,
                        camera,
                        fastMoveCameraToPosition
                    );

                    base.InsertItem(index, customVM);
                    ModLogger.Log($"✓ Created CapitalNameplateVM for: {settlement.Name} (IsCapital: {customVM.IsCapital})");
                }
                else
                {
                    // Fallback to original if extraction failed
                    base.InsertItem(index, item);
                    ModLogger.Log($"✗ Using original nameplate for: {settlementName} (field extraction failed)");
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
