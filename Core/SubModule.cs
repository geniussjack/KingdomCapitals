using KingdomCapitals.Behaviors;
using KingdomCapitals.Constants;
using KingdomCapitals.Models.GameModels;
using KingdomCapitals.Utils;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace KingdomCapitals.Core
{
    /// <summary>
    /// Main entry point for the Kingdom Capitals mod.
    /// Handles mod initialization using native GameModel overrides and campaign behaviors.
    /// No Harmony patches required - uses TaleWorlds' official modding API.
    /// </summary>
    public class SubModule : MBSubModuleBase
    {
        /// <summary>
        /// Called when the mod is first loaded.
        /// Initializes logging system.
        /// </summary>
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            try
            {
                // Clear log file at start of new session
                ModLogger.ClearLog();

                ModLogger.Log("Kingdom Capitals mod loaded - using native GameModel API (no Harmony patches)");
                ModLogger.Log(Messages.Mod.LoadedSuccessfully);
            }
            catch (Exception ex)
            {
                ModLogger.Error(Messages.Errors.FailedToLoad, ex);
            }
        }

        /// <summary>
        /// Called when a new game starts or a save is loaded.
        /// Initializes the capital management system, registers custom GameModels, and campaign behaviors.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="gameStarterObject">The game starter object for registering behaviors and models.</param>
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);

            try
            {
                if (game.GameType is Campaign)
                {
                    CampaignGameStarter campaignStarter = (CampaignGameStarter)gameStarterObject;

                    // Register custom GameModels (replaces Harmony patches)
                    ModLogger.Log("Registering custom GameModels...");

                    // Core models
                    campaignStarter.AddModel(new CapitalGarrisonModel());
                    campaignStarter.AddModel(new CapitalPartyWageModel());

                    // Building models (doubled effects for daily projects)
                    campaignStarter.AddModel(new CapitalBuildingEffectModel());

                    // Daily project models (doubled bonuses)
                    campaignStarter.AddModel(new CapitalFoodModel());
                    campaignStarter.AddModel(new CapitalProsperityModel());
                    campaignStarter.AddModel(new CapitalLoyaltyModel());
                    campaignStarter.AddModel(new CapitalMilitiaModel());

                    ModLogger.Log("7 custom GameModels registered successfully");

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
        /// No cleanup needed - GameModels are automatically unloaded with the campaign.
        /// </summary>
        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();

            try
            {
                ModLogger.Log(Messages.Mod.Unloaded);
            }
            catch (Exception ex)
            {
                ModLogger.Error(Messages.Errors.ErrorDuringUnload, ex);
            }
        }
    }
}
