namespace KingdomCapitals.Constants
{
    /// <summary>
    /// Message templates and text constants for user notifications.
    /// </summary>
    public static class Messages
    {
        /// <summary>
        /// General mod messages.
        /// </summary>
        public static class Mod
        {
            public const string LoadedSuccessfully = "Kingdom Capitals mod loaded successfully";
            public const string StartedSuccessfully = "Kingdom Capitals mod started successfully";
            public const string Ended = "Kingdom Capitals mod ended";
            public const string Unloaded = "Kingdom Capitals mod unloaded";
            public const string BehaviorsRegistered = "Campaign behaviors registered";
        }

        /// <summary>
        /// Error messages.
        /// </summary>
        public static class Errors
        {
            public const string FailedToLoad = "Failed to load Kingdom Capitals mod";
            public const string FailedToStart = "Failed to start Kingdom Capitals mod";
            public const string ErrorDuringShutdown = "Error during mod shutdown";
            public const string ErrorDuringUnload = "Error during mod unload";
            public const string FailedToCreateLogDirectory = "[Kingdom Capitals] Failed to create log directory";
            public const string LoggingFailed = "[Kingdom Capitals] Logging failed";
            public const string TransferCapitalOwnershipNullParameters = "TransferCapitalOwnership: null parameters";
            public const string HandleCapitalConquestNullParameters = "HandleCapitalConquest: null parameters";
            public const string CreatePlayerKingdomNullParameters = "CreatePlayerKingdomFromCapital: null parameters";
            public const string TransferToRulingClanNullRulingClan = "TransferCapitalToRulingClan: null ruling clan";
        }

        /// <summary>
        /// Conquest-related messages.
        /// </summary>
        public static class Conquest
        {
            public const string CapitalConqueredFormat = "{0} conquered! {1} has fallen!";
            public const string PlayerConqueredKingdomFormat = "You have conquered {0} by capturing their capital!";
            public const string KingdomFallenFormat = "{0} has fallen! {1} is no more!";
            public const string PlayerCapturedCapitalWithoutKingdomFormat = "You have captured {0}, the capital of {1}! Found your own kingdom to complete the conquest.";
            public const string PlayerCapturedCapitalFoundKingdomFormat = "You have captured {0}! Found your kingdom to claim it as your capital.";
        }

        /// <summary>
        /// Warning messages.
        /// </summary>
        public static class Warnings
        {
            public const string AlreadyInitialized = "CapitalManager already initialized";
            public const string CapitalConqueredOldKingdomNull = "Capital {0} conquered but old kingdom is null";
            public const string NoRulerFound = "No new ruler found for {0}, capital transfer delayed";
            public const string PlayerCreatedKingdomManualAction = "Player {0} captured capital {1} but kingdom creation requires manual action";
            public const string CouldNotFindTroopType = "Could not find troop type for {0} (tier {1})";
        }

        /// <summary>
        /// Log messages.
        /// </summary>
        public static class Log
        {
            public const string CapitalManagerInitializedFormat = "CapitalManager initialized with {0} capitals";
            public const string MarkedAsRecentlyCapturedFormat = "Marked {0} as recently captured capital";
            public const string TransferredCapitalFormat = "Transferred capital {0} to {1}";
            public const string CapitalNoFood = "Capital {0} has no food, skipping garrison reinforcement";
            public const string BlockedVotingFormat = "Blocked voting for recently captured capital: {0}";
            public const string PreventedDecisionCreationFormat = "Prevented settlement claimant decision creation for capital: {0}";
            public const string AppliedGoldenColorFormat = "Applied golden color markup to capital name: {0}";
            public const string CapitalNameCacheCleared = "Capital name cache cleared";
            public const string KingdomEliminatedTransferCancelled = "Kingdom eliminated, capital {0} transfer cancelled";
            public const string CapitalTransferredToNewRulerFormat = "Capital {0} transferred to new ruler {1}";
            public const string RulerKilledTransferScheduledFormat = "Ruler {0} of {1} was killed. Capital transfer scheduled.";
            public const string PlayerCapturedCapitalWithoutKingdomLog = "Player {0} captured capital {1} without a kingdom";
            public const string CapitalConquestDisabled = "Capital conquest disabled in settings, treating {0} as normal settlement";
            public const string CapitalTransferredToRulingClanFormat = "Capital {0} transferred to ruling clan {1}";
            public const string TransferredSettlementFormat = "Transferred {0} from {1} to {2}";
            public const string VassalizedClanFormat = "Vassalized clan {0} to {1}";
            public const string KingdomDestroyedFormat = "Kingdom {0} has been destroyed";
            public const string CreatedPlayerKingdomFormat = "Created new kingdom for player: {0} with capital {1}";
        }
    }
}
