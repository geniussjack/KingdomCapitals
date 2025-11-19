using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using KingdomCapitals.Utils;

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
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized)
            {
                ModLogger.Warning("CapitalManager already initialized");
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
            ModLogger.Log($"CapitalManager initialized with {_activeCapitals.Count} capitals");
        }

        /// <summary>
        /// Checks if a settlement is currently registered as a capital.
        /// </summary>
        public static bool IsCapital(Settlement settlement)
        {
            if (settlement == null || !_isInitialized)
                return false;

            return _activeCapitals.ContainsValue(settlement);
        }

        /// <summary>
        /// Gets the current capital of a kingdom.
        /// </summary>
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
        public static IEnumerable<Settlement> GetAllCapitals()
        {
            return _isInitialized ? _activeCapitals.Values : Enumerable.Empty<Settlement>();
        }

        /// <summary>
        /// Unregisters a settlement as a capital (called when kingdom is destroyed).
        /// </summary>
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
        /// </summary>
        public static void MarkAsRecentlyCaptured(Settlement capital)
        {
            if (capital == null || !_isInitialized)
                return;

            _recentlyCapturedCapitals.Add(capital);
            ModLogger.Log($"Marked {capital.Name} as recently captured capital");

            // Remove mark after 1 in-game day
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(null, () =>
            {
                _recentlyCapturedCapitals.Remove(capital);
            });
        }

        /// <summary>
        /// Checks if a settlement was recently captured as a capital.
        /// </summary>
        public static bool WasRecentlyCapturedCapital(Settlement settlement)
        {
            if (!_isInitialized)
                return false;

            return _recentlyCapturedCapitals.Contains(settlement);
        }

        /// <summary>
        /// Transfers capital ownership to a new ruler.
        /// </summary>
        public static void TransferCapitalOwnership(Hero newOwner, Settlement capital)
        {
            if (newOwner == null || capital == null)
            {
                ModLogger.Error("TransferCapitalOwnership: null parameters");
                return;
            }

            try
            {
                ChangeOwnerOfSettlementAction.ApplyByDefault(newOwner, capital);
                ModLogger.Log($"Transferred capital {capital.Name} to {newOwner.Name}");
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Failed to transfer capital ownership: {capital.Name}", ex);
            }
        }

        /// <summary>
        /// Handles the complete conquest of a kingdom when its capital is captured.
        /// </summary>
        public static void HandleCapitalConquest(Settlement capital, Kingdom attackerKingdom, Kingdom defenderKingdom, Hero capturerHero)
        {
            if (capital == null || attackerKingdom == null || defenderKingdom == null)
            {
                ModLogger.Error("HandleCapitalConquest: null parameters");
                return;
            }

            try
            {
                ModLogger.LogCapitalConquest(capital, defenderKingdom, attackerKingdom, capturerHero);

                // Mark as recently captured to prevent voting
                MarkAsRecentlyCaptured(capital);

                // Transfer capital directly to ruling clan
                TransferCapitalToRulingClan(capital, attackerKingdom);

                // Remove capital status
                UnregisterCapital(capital, defenderKingdom);

                // Notify player
                InformationManager.DisplayMessage(new InformationMessage(
                    $"{capital.Name} conquered! {defenderKingdom.Name} has fallen!",
                    Colors.Red
                ));
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Error in HandleCapitalConquest for {capital.Name}", ex);
            }
        }

        /// <summary>
        /// Transfers captured capital directly to the ruling clan without voting.
        /// </summary>
        private static void TransferCapitalToRulingClan(Settlement capital, Kingdom conquererKingdom)
        {
            if (conquererKingdom?.RulingClan == null)
            {
                ModLogger.Error("TransferCapitalToRulingClan: null ruling clan");
                return;
            }

            try
            {
                Hero rulingClanLeader = conquererKingdom.RulingClan.Leader;
                ChangeOwnerOfSettlementAction.ApplyByDefault(rulingClanLeader, capital);

                ModLogger.Log($"Capital {capital.Name} transferred to ruling clan {conquererKingdom.RulingClan.Name}");
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Failed to transfer capital to ruling clan: {capital.Name}", ex);
            }
        }

        /// <summary>
        /// Creates a new kingdom for a player who captured a capital without having one.
        /// </summary>
        public static void CreatePlayerKingdomFromCapital(Hero playerHero, Settlement capital)
        {
            if (playerHero == null || capital == null || playerHero.Clan == null)
            {
                ModLogger.Error("CreatePlayerKingdomFromCapital: null parameters");
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
                    ModLogger.Warning($"Player {playerHero.Name} captured capital {capital.Name} but kingdom creation requires manual action");
                    InformationManager.DisplayMessage(new InformationMessage(
                        $"You have captured {capital.Name}! Found your kingdom to claim it as your capital.",
                        Colors.Green
                    ));
                }
                else
                {
                    _activeCapitals[newKingdom.StringId] = capital;
                    ModLogger.Log($"Created new kingdom for player: {kingdomName} with capital {capital.Name}");
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Failed to create player kingdom from capital: {capital.Name}", ex);
            }
        }
    }
}
