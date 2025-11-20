using HarmonyLib;
using KingdomCapitals.Core;
using KingdomCapitals.Utils;
using SandBox.GauntletUI.Map;
using SandBox.ViewModelCollection.Nameplate;
using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace KingdomCapitals.Patches
{
    /// <summary>
    /// Harmony patch that programmatically adds crown icon widgets to capital settlement nameplates.
    /// Patches the View layer (GauntletMapSettlementNameplateView) to inject ImageWidget after nameplate creation.
    /// </summary>
    [HarmonyPatch(typeof(GauntletMapSettlementNameplateView))]
    public static class SettlementNameplateViewPatch
    {
        private static readonly Dictionary<Settlement, Widget> _crownWidgets = new();

        private static bool Prepare()
        {
            ModLogger.Log("SettlementNameplateViewPatch: Preparing to inject crown icons via View layer");
            return true;
        }

        /// <summary>
        /// Postfix patch on CreateLayout - adds crown icons after nameplate widgets are created.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch("CreateLayout")]
        private static void CreateLayout_Postfix(GauntletMapSettlementNameplateView __instance)
        {
            try
            {
                // Get the GauntletLayer from the view
                FieldInfo layerField = AccessTools.Field(typeof(GauntletMapSettlementNameplateView), "_gauntletLayer");
                if (layerField == null)
                {
                    ModLogger.Log("CreateLayout_Postfix: Cannot find _gauntletLayer field");
                    return;
                }

                if (layerField.GetValue(__instance) is not GauntletLayer gauntletLayer)
                {
                    ModLogger.Log("CreateLayout_Postfix: GauntletLayer is null");
                    return;
                }

                // Get the ViewModel (SettlementNameplatesVM)
                PropertyInfo dataSourceField = AccessTools.Property(typeof(GauntletMapSettlementNameplateView), "DataSource");
                if (dataSourceField == null)
                {
                    ModLogger.Log("CreateLayout_Postfix: Cannot find DataSource property");
                    return;
                }

                if (dataSourceField.GetValue(__instance, null) is not SettlementNameplatesVM nameplatesVM || nameplatesVM.Nameplates == null)
                {
                    ModLogger.Log("CreateLayout_Postfix: SettlementNameplatesVM or Nameplates is null");
                    return;
                }

                ModLogger.Log($"CreateLayout_Postfix: Found {nameplatesVM.Nameplates.Count} nameplates");

                // Inject crown icons for capitals
                int crownCount = 0;
                foreach (SettlementNameplateVM nameplate in nameplatesVM.Nameplates)
                {
                    if (nameplate.Settlement != null && CapitalManager.IsCapital(nameplate.Settlement))
                    {
                        InjectCrownIcon(gauntletLayer, nameplate);
                        crownCount++;
                    }
                }

                ModLogger.Log($"CreateLayout_Postfix: Injected {crownCount} crown icons");
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in CreateLayout_Postfix", ex);
            }
        }

        /// <summary>
        /// Injects a crown icon widget for the given settlement nameplate.
        /// </summary>
        private static void InjectCrownIcon(GauntletLayer layer, SettlementNameplateVM nameplate)
        {
            try
            {
                // Skip if already has crown
                if (_crownWidgets.ContainsKey(nameplate.Settlement))
                {
                    return;
                }

                // Find the widget for this nameplate
                // This is tricky - we need to find the widget bound to this ViewModel
                var rootWidget = layer.RootWidget;
                if (rootWidget == null)
                {
                    ModLogger.Log($"InjectCrownIcon: Root widget is null for {nameplate.Settlement.Name}");
                    return;
                }

                // Create crown icon widget programmatically
                Widget crownContainer = new(layer.UIContext)
                {
                    Id = $"capital_crown_{nameplate.Settlement.StringId}",
                    WidthSizePolicy = SizePolicy.Fixed,
                    HeightSizePolicy = SizePolicy.Fixed,
                    SuggestedWidth = 20f,
                    SuggestedHeight = 20f,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    PositionYOffset = -28f,
                    DoNotAcceptEvents = true,
                    DoNotPassEventsToChildren = true
                };

                // Try to get sprite
                SpriteData spriteData = layer.UIContext.SpriteData;
                Sprite sprite = spriteData.GetSprite("ui_kingdomcapitals\\capital_crown_icon");

                if (sprite == null)
                {
                    ModLogger.Log($"InjectCrownIcon: Sprite 'ui_kingdomcapitals\\capital_crown_icon' not found");
                    return;
                }

                // Create the actual image widget with crown sprite
                ImageWidget crownImage = new(layer.UIContext)
                {
                    WidthSizePolicy = SizePolicy.Fixed,
                    HeightSizePolicy = SizePolicy.Fixed,
                    SuggestedWidth = 20f,
                    SuggestedHeight = 20f,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Sprite = sprite
                };

                crownContainer.AddChild(crownImage);
                rootWidget.AddChild(crownContainer);

                _crownWidgets[nameplate.Settlement] = crownContainer;

                ModLogger.Log($"✓ Injected crown icon for capital: {nameplate.Settlement.Name}");
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Error injecting crown for {nameplate.Settlement?.Name}", ex);
            }
        }

        /// <summary>
        /// Cleanup when view is finalized.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch("OnFinalize")]
        private static void OnFinalize_Postfix()
        {
            _crownWidgets.Clear();
        }
    }
}
