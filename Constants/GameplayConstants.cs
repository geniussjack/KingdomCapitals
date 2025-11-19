namespace KingdomCapitals.Constants
{
    /// <summary>
    /// Gameplay-related constants for game mechanics.
    /// </summary>
    public static class GameplayConstants
    {
        /// <summary>
        /// Maximum troop tier in Bannerlord (tier 6 is max).
        /// </summary>
        public const int MaxTroopTier = 6;

        /// <summary>
        /// Minimum troop tier (tier 0 is basic).
        /// </summary>
        public const int MinTroopTier = 0;

        /// <summary>
        /// Base food consumption per party member per day.
        /// Used for calculating garrison food requirements.
        /// </summary>
        public const float BaseFoodConsumptionPerMember = 1.0f;

        /// <summary>
        /// Number of in-game days before a recently captured capital can be voted on.
        /// </summary>
        public const int CapitalVotingCooldownDays = 1;
    }
}
