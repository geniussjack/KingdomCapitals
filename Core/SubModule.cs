using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using KingdomCapitals.Utils;
using KingdomCapitals.Behaviors;
using KingdomCapitals.Constants;

namespace KingdomCapitals.Core
{
    /// <summary>
    /// Main entry point for the Kingdom Capitals mod.
    /// Handles mod initialization, Harmony patching, and campaign behavior registration.
    /// </summary>
    public class SubModule : MBSubModuleBase
    {
        private Harmony _harmony;

        /// <summary>
        /// Called when the mod is first loaded.
        /// Initializes Harmony patches for all mod functionality.
        /// </summary>
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            try
            {
                // Initialize Harmony patches
                _harmony = new Harmony(ModConstants.HarmonyId);
                _harmony.PatchAll();

                ModLogger.Log(Messages.Mod.LoadedSuccessfully);
                ModLogger.Log(string.Format(Messages.Log.HarmonyPatchesAppliedFormat, ModConstants.HarmonyId));
            }
            catch (Exception ex)
            {
                ModLogger.Error(Messages.Errors.FailedToLoad, ex);
            }
        }

        /// <summary>
        /// Called when a new game starts or a save is loaded.
        /// Initializes the capital management system and registers campaign behaviors.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="gameStarterObject">The game starter object for registering behaviors.</param>
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

                    ModLogger.Log(Messages.Mod.BehaviorsRegistered);
                    ModLogger.Log(Messages.Mod.StartedSuccessfully);
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error(Messages.Errors.FailedToStart, ex);
            }
        }

        /// <summary>
        /// Called when the game ends.
        /// Performs cleanup operations for the current game session.
        /// </summary>
        /// <param name="game">The game instance that is ending.</param>
        public override void OnGameEnd(Game game)
        {
            base.OnGameEnd(game);

            try
            {
                ModLogger.Log(Messages.Mod.Ended);
            }
            catch (Exception ex)
            {
                ModLogger.Error(Messages.Errors.ErrorDuringShutdown, ex);
            }
        }

        /// <summary>
        /// Called when the mod is unloaded.
        /// Removes all Harmony patches applied by this mod.
        /// </summary>
        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();

            try
            {
                // Unpatch Harmony modifications
                _harmony?.UnpatchAll(ModConstants.HarmonyId);
                ModLogger.Log(Messages.Mod.Unloaded);
            }
            catch (Exception ex)
            {
                ModLogger.Error(Messages.Errors.ErrorDuringUnload, ex);
            }
        }
    }
}
