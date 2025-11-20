using HarmonyLib;
using KingdomCapitals.Utils;
using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar.MapNameplates;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace KingdomCapitals.ViewModels
{
    /// <summary>
    /// Intercepts the MBBindingList to replace standard SettlementNameplateVM
    /// with our custom CapitalSettlementNameplateVM.
    /// </summary>
    public class CapitalNameplateListInterceptor : MBBindingList<SettlementNameplateVM>
    {
        /// <summary>
        /// Overrides InsertItem to replace standard ViewModel with custom one.
        /// </summary>
        protected override void InsertItem(int index, SettlementNameplateVM item)
        {
            try
            {
                if (item == null)
                {
                    base.InsertItem(index, item);
                    return;
                }

                // Use HarmonyLib's Traverse to access private fields via reflection
                Traverse traverse = Traverse.Create(item);

                // Extract the settlement entity
                Settlement settlement = traverse.Field("Settlement").GetValue<Settlement>();

                if (settlement == null)
                {
                    // If we can't get settlement, just use the original item
                    base.InsertItem(index, item);
                    return;
                }

                // Try to extract the private fields needed for constructor
                // Field names based on decompiled SettlementNameplateVM
                GameEntity targetEntity = traverse.Field("_targetEntity").GetValue<GameEntity>();
                Camera camera = traverse.Field("_camera").GetValue<Camera>();
                Action<Vec2, float> fastMoveCameraToPosition = traverse.Field("_fastMoveCameraToPosition").GetValue<Action<Vec2, float>>();

                // If we successfully extracted all parameters, create custom ViewModel
                if (targetEntity != null && camera != null && fastMoveCameraToPosition != null)
                {
                    CapitalSettlementNameplateVM customItem = new CapitalSettlementNameplateVM(
                        settlement,
                        targetEntity,
                        camera,
                        fastMoveCameraToPosition
                    );

                    base.InsertItem(index, customItem);
                    ModLogger.Log($"Replaced nameplate for {settlement.Name} (IsCapital: {customItem.IsCapital})");
                }
                else
                {
                    // If reflection failed, fallback to original item
                    ModLogger.Log($"Reflection failed for {settlement.Name}, using original nameplate");
                    base.InsertItem(index, item);
                }
            }
            catch (Exception ex)
            {
                // If anything goes wrong, use the original item to avoid crashes
                ModLogger.Error($"Error in CapitalNameplateListInterceptor.InsertItem", ex);
                base.InsertItem(index, item);
            }
        }
    }
}
