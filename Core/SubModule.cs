using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using KingdomCapitals.Utils;
using KingdomCapitals.Behaviors;

namespace KingdomCapitals.Core
{
    /// <summary>
    /// Main entry point for the Kingdom Capitals mod.
    /// </summary>
    public class SubModule : MBSubModuleBase
    {
        private const string HarmonyId = "com.kingdomcapitals.bannerlord";
        private Harmony _harmony;

        /// <summary>
        /// Called when the mod is first loaded.
        /// </summary>
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            try
            {
                // Initialize Harmony patches
                _harmony = new Harmony(HarmonyId);
                _harmony.PatchAll();

                ModLogger.Log("Kingdom Capitals mod loaded successfully");
                ModLogger.Log($"Harmony patches applied: {HarmonyId}");
            }
            catch (Exception ex)
            {
                ModLogger.Error("Failed to load Kingdom Capitals mod", ex);
            }
        }

        /// <summary>
        /// Called when a new game starts or a save is loaded.
        /// </summary>
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);

            try
            {
                if (game.GameType is Campaign)
                {
                    CampaignGameStarter campaignStarter = (CampaignGameStarter)gameStarterObject;

                    // Initialize capital management system
                    CapitalManager.Initialize();

                    // Register campaign behaviors
                    campaignStarter.AddBehavior(new CapitalManagementBehavior());
                    campaignStarter.AddBehavior(new CapitalGarrisonBehavior());
                    campaignStarter.AddBehavior(new CapitalConquestBehavior());

                    ModLogger.Log("Campaign behaviors registered");
                    ModLogger.Log("Kingdom Capitals mod started successfully");
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error("Failed to start Kingdom Capitals mod", ex);
            }
        }

        /// <summary>
        /// Called when the game ends.
        /// </summary>
        public override void OnGameEnd(Game game)
        {
            base.OnGameEnd(game);

            try
            {
                ModLogger.Log("Kingdom Capitals mod ended");
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error during mod shutdown", ex);
            }
        }

        /// <summary>
        /// Called when the mod is unloaded.
        /// </summary>
        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();

            try
            {
                // Unpatch Harmony modifications
                _harmony?.UnpatchAll(HarmonyId);
                ModLogger.Log("Kingdom Capitals mod unloaded");
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error during mod unload", ex);
            }
        }
    }
}
