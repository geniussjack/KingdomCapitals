using HarmonyLib;
using KingdomCapitals.Core;
using KingdomCapitals.Utils;
using SandBox.GauntletUI.Map;
using SandBox.ViewModelCollection.Nameplate;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace KingdomCapitals.Patches
{
    /// <summary>
    /// Harmony patch that programmatically adds crown icon widgets to capital settlement nameplates.
    /// Patches the View layer (GauntletMapSettlementNameplateView) to inject ImageWidget after nameplate creation.
    /// </summary>
    [HarmonyPatch(typeof(GauntletMapSettlementNameplateView))]
    public static class SettlementNameplateViewPatch
    {
        private static readonly Dictionary<Settlement, Widget> _crownWidgets = new Dictionary<Settlement, Widget>();

        static bool Prepare()
        {
            ModLogger.Log("SettlementNameplateViewPatch: Preparing to inject crown icons via View layer");
            return true;
        }

        /// <summary>
        /// Postfix patch on CreateLayout - adds crown icons after nameplate widgets are created.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch("CreateLayout")]
        static void CreateLayout_Postfix(GauntletMapSettlementNameplateView __instance)
        {
            try
            {
                // Get the GauntletLayer from the view
                var layerField = AccessTools.Field(typeof(GauntletMapSettlementNameplateView), "_gauntletLayer");
                if (layerField == null)
                {
                    ModLogger.Log("CreateLayout_Postfix: Cannot find _gauntletLayer field");
                    return;
                }

                var gauntletLayer = layerField.GetValue(__instance) as GauntletLayer;
                if (gauntletLayer == null)
                {
                    ModLogger.Log("CreateLayout_Postfix: GauntletLayer is null");
                    return;
                }

                // Get the ViewModel (SettlementNameplatesVM)
                var dataSourceField = AccessTools.Property(typeof(GauntletMapSettlementNameplateView), "DataSource");
                if (dataSourceField == null)
                {
                    ModLogger.Log("CreateLayout_Postfix: Cannot find DataSource property");
                    return;
                }

                var nameplatesVM = dataSourceField.GetValue(__instance, null) as SettlementNameplatesVM;
                if (nameplatesVM == null || nameplatesVM.Nameplates == null)
                {
                    ModLogger.Log("CreateLayout_Postfix: SettlementNameplatesVM or Nameplates is null");
                    return;
                }

                ModLogger.Log($"CreateLayout_Postfix: Found {nameplatesVM.Nameplates.Count} nameplates");

                // Inject crown icons for capitals
                int crownCount = 0;
                foreach (var nameplate in nameplatesVM.Nameplates)
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
                    return;

                // Get the movie's root widget via reflection
                // GauntletLayer doesn't have direct RootWidget property
                var movieField = AccessTools.Field(typeof(GauntletLayer), "_movie");
                if (movieField == null)
                {
                    ModLogger.Log($"InjectCrownIcon: Cannot find _movie field");
                    return;
                }

                var movie = movieField.GetValue(layer);
                if (movie == null)
                {
                    ModLogger.Log($"InjectCrownIcon: Movie is null");
                    return;
                }

                // Get RootView from movie via reflection
                var rootViewProperty = AccessTools.Property(movie.GetType(), "RootView");
                if (rootViewProperty == null)
                {
                    ModLogger.Log($"InjectCrownIcon: Cannot find RootView property");
                    return;
                }

                var rootWidget = rootViewProperty.GetValue(movie, null) as Widget;
                if (rootWidget == null)
                {
                    ModLogger.Log($"InjectCrownIcon: Root widget is null for {nameplate.Settlement.Name}");
                    return;
                }

                // Create crown icon widget programmatically
                var crownContainer = new Widget(layer.UIContext)
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
                var spriteData = layer.UIContext.SpriteData;
                var sprite = spriteData.GetSprite("ui_kingdomcapitals\\capital_crown_icon");

                if (sprite == null)
                {
                    ModLogger.Log($"InjectCrownIcon: Sprite 'ui_kingdomcapitals\\capital_crown_icon' not found");
                    return;
                }

                // Create the actual image widget with crown sprite
                var crownImage = new ImageWidget(layer.UIContext)
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
        static void OnFinalize_Postfix()
        {
            _crownWidgets.Clear();
        }
    }
}
