using KingdomCapitals.Constants;
using KingdomCapitals.Patches;
using KingdomCapitals.Services;
using KingdomCapitals.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace KingdomCapitals.Core
{
    /// <summary>
    /// Central manager for capital city logic and state management.
    /// </summary>
    public static class CapitalManager
    {
        private static Dictionary<string, Settlement> _activeCapitals;
        private static HashSet<Settlement> _recentlyCapturedCapitals;
        private static bool _isInitialized = false;

        /// <summary>
        /// Initializes the capital management system.
        /// Registers all default capitals and clears the settlement name cache.
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized)
            {
                ModLogger.Warning(Messages.Warnings.AlreadyInitialized);
                return;
            }

            _activeCapitals = new Dictionary<string, Settlement>();
            _recentlyCapturedCapitals = new HashSet<Settlement>();

            // Register all default capitals
            foreach (Kingdom kingdom in Kingdom.All)
            {
                Settlement capital = CapitalData.GetDefaultCapital(kingdom);
                if (capital != null)
                {
                    _activeCapitals[kingdom.StringId] = capital;
                }
            }

            _isInitialized = true;

            // Clear the settlement name cache to ensure fresh color markup
            SettlementNameColorPatch.ClearCache();

            ModLogger.Log(string.Format(Messages.Log.CapitalManagerInitializedFormat, _activeCapitals.Count));
        }

        /// <summary>
        /// Checks if a settlement is currently registered as a capital.
        /// </summary>
        /// <param name="settlement">The settlement to check.</param>
        /// <returns>True if the settlement is a capital, false otherwise.</returns>
        public static bool IsCapital(Settlement settlement)
        {
            if (settlement == null || !_isInitialized)
                return false;

            return _activeCapitals.ContainsValue(settlement);
        }

        /// <summary>
        /// Gets the current capital of a kingdom.
        /// </summary>
        /// <param name="kingdom">The kingdom whose capital to retrieve.</param>
        /// <returns>The capital settlement or null if not found.</returns>
        public static Settlement GetCapital(Kingdom kingdom)
        {
            if (kingdom == null || !_isInitialized)
                return null;

            _activeCapitals.TryGetValue(kingdom.StringId, out Settlement capital);
            return capital;
        }

        /// <summary>
        /// Gets all currently active capitals.
        /// </summary>
        /// <returns>A collection of all active capital settlements.</returns>
        public static IEnumerable<Settlement> GetAllCapitals()
        {
            return _isInitialized ? _activeCapitals.Values : Enumerable.Empty<Settlement>();
        }

        /// <summary>
        /// Unregisters a settlement as a capital (called when kingdom is destroyed).
        /// </summary>
        /// <param name="settlement">The settlement to unregister.</param>
        /// <param name="defeatedKingdom">The kingdom being defeated.</param>
        public static void UnregisterCapital(Settlement settlement, Kingdom defeatedKingdom)
        {
            if (settlement == null || defeatedKingdom == null || !_isInitialized)
                return;

            if (_activeCapitals.Remove(defeatedKingdom.StringId))
            {
                ModLogger.LogCapitalStatusRemoval(settlement, defeatedKingdom);
            }
        }

        /// <summary>
        /// Marks a capital as recently captured to prevent voting for distribution.
        /// The mark is automatically removed after one in-game day.
        /// </summary>
        /// <param name="capital">The capital settlement to mark.</param>
        public static void MarkAsRecentlyCaptured(Settlement capital)
        {
            if (capital == null || !_isInitialized)
                return;

            _recentlyCapturedCapitals.Add(capital);
            ModLogger.Log(string.Format(Messages.Log.MarkedAsRecentlyCapturedFormat, capital.Name));

            // Remove mark after 1 in-game day
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(null, () =>
            {
                _recentlyCapturedCapitals.Remove(capital);
            });
        }

        /// <summary>
        /// Checks if a settlement was recently captured as a capital.
        /// </summary>
        /// <param name="settlement">The settlement to check.</param>
        /// <returns>True if the settlement was recently captured as a capital, false otherwise.</returns>
        public static bool WasRecentlyCapturedCapital(Settlement settlement)
        {
            if (!_isInitialized)
                return false;

            return _recentlyCapturedCapitals.Contains(settlement);
        }

        /// <summary>
        /// Transfers capital ownership to a new ruler.
        /// </summary>
        /// <param name="newOwner">The hero who will become the new owner.</param>
        /// <param name="capital">The capital settlement to transfer.</param>
        /// <returns>True if transfer was successful, false otherwise.</returns>
        public static bool TransferCapitalOwnership(Hero newOwner, Settlement capital)
        {
            return SettlementTransferService.TransferSettlement(capital, newOwner);
        }

        /// <summary>
        /// Handles the complete conquest of a kingdom when its capital is captured.
        /// Transfers the capital, removes capital status, and notifies the player.
        /// </summary>
        /// <param name="capital">The capital settlement that was captured.</param>
        /// <param name="attackerKingdom">The kingdom that captured the capital.</param>
        /// <param name="defenderKingdom">The kingdom that lost the capital.</param>
        /// <param name="capturerHero">The hero who captured the capital.</param>
        public static void HandleCapitalConquest(Settlement capital, Kingdom attackerKingdom, Kingdom defenderKingdom, Hero capturerHero)
        {
            if (capital == null || attackerKingdom == null || defenderKingdom == null)
            {
                ModLogger.Error(Messages.Errors.HandleCapitalConquestNullParameters);
                return;
            }

            try
            {
                ModLogger.LogCapitalConquest(capital, defenderKingdom, attackerKingdom, capturerHero);

                // Mark as recently captured to prevent voting
                MarkAsRecentlyCaptured(capital);

                // Transfer capital directly to ruling clan
                SettlementTransferService.TransferCapitalToRulingClan(capital, attackerKingdom);

                // Remove capital status
                UnregisterCapital(capital, defenderKingdom);

                // Notify player
                ConquestNotificationService.NotifyKingdomConquest(capital, defenderKingdom, attackerKingdom, capturerHero);
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Error in HandleCapitalConquest for {capital.Name}", ex);
            }
        }

        /// <summary>
        /// Creates a new kingdom for a player who captured a capital without having one.
        /// </summary>
        /// <param name="playerHero">The player hero who captured the capital.</param>
        /// <param name="capital">The capital settlement that was captured.</param>
        public static void CreatePlayerKingdomFromCapital(Hero playerHero, Settlement capital)
        {
            if (playerHero == null || capital == null || playerHero.Clan == null)
            {
                ModLogger.Error(Messages.Errors.CreatePlayerKingdomNullParameters);
                return;
            }

            try
            {
                // Use player's clan name for kingdom
                string kingdomName = playerHero.Clan.Name.ToString();

                // Create kingdom using the base game action
                Kingdom newKingdom = Kingdom.All.FirstOrDefault(k => k.Leader == playerHero);

                if (newKingdom == null)
                {
                    ModLogger.Warning(string.Format(Messages.Warnings.PlayerCreatedKingdomManualAction, playerHero.Name, capital.Name));
                    InformationManager.DisplayMessage(new InformationMessage(
                        string.Format(Messages.Conquest.PlayerCapturedCapitalFoundKingdomFormat, capital.Name),
                        UIConstants.MessageColors.Success
                    ));
                }
                else
                {
                    _activeCapitals[newKingdom.StringId] = capital;
                    ModLogger.Log(string.Format(Messages.Log.CreatedPlayerKingdomFormat, kingdomName, capital.Name));
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Failed to create player kingdom from capital: {capital.Name}", ex);
            }
        }
    }
}
