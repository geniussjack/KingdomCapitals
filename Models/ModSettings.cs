using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace KingdomCapitals.Models
{
    /// <summary>
    /// MCM (Mod Configuration Menu) settings for Kingdom Capitals.
    /// Allows players to customize mod behavior through the game's mod options menu.
    /// </summary>
    public class ModSettings : AttributeGlobalSettings<ModSettings>
    {
        public override string Id => "KingdomCapitals_v1";
        public override string DisplayName => "Kingdom Capitals";
        public override string FolderName => "KingdomCapitals";
        public override string FormatType => "json";

        #region Garrison Settings

        [SettingPropertyInteger(
            "Daily Garrison Reinforcement",
            0, 10,
            Order = 0,
            RequireRestart = false,
            HintText = "Number of troops added daily to capital garrisons (requires food). Default: 3")]
        [SettingPropertyGroup("Garrison Settings", GroupOrder = 0)]
        public int DailyGarrisonReinforcement { get; set; } = 3;

        [SettingPropertyFloatingInteger(
            "Garrison Food Consumption Multiplier",
            0.1f, 2.0f,
            Order = 1,
            RequireRestart = false,
            HintText = "Multiplier for capital garrison food consumption. Lower = less food consumed. Default: 0.5 (50% reduction)")]
        [SettingPropertyGroup("Garrison Settings", GroupOrder = 0)]
        public float GarrisonFoodConsumptionMultiplier { get; set; } = 0.5f;

        [SettingPropertyInteger(
            "Prosperity Per Troop Tier",
            1000, 5000,
            Order = 2,
            RequireRestart = false,
            HintText = "Prosperity required per troop tier upgrade. Default: 2500 (2500 = Tier 1, 5000 = Tier 2, etc.)")]
        [SettingPropertyGroup("Garrison Settings", GroupOrder = 0)]
        public int ProsperityPerTroopTier { get; set; } = 2500;

        #endregion

        #region Conquest Settings

        [SettingPropertyBool(
            "Enable Capital Conquest Mechanics",
            Order = 0,
            RequireRestart = false,
            HintText = "When enabled, capturing a capital destroys the entire kingdom. Default: true")]
        [SettingPropertyGroup("Conquest Settings", GroupOrder = 1)]
        public bool EnableCapitalConquest { get; set; } = true;

        [SettingPropertyBool(
            "Transfer Capital to Ruling Clan",
            Order = 1,
            RequireRestart = false,
            HintText = "Captured capitals are transferred directly to ruling clan without voting. Default: true")]
        [SettingPropertyGroup("Conquest Settings", GroupOrder = 1)]
        public bool TransferCapitalToRulingClan { get; set; } = true;

        [SettingPropertyBool(
            "Vassalize Defeated Kingdom Clans",
            Order = 2,
            RequireRestart = false,
            HintText = "Clans from defeated kingdom become vassals instead of going independent. Default: true")]
        [SettingPropertyGroup("Conquest Settings", GroupOrder = 1)]
        public bool VassalizeDefeatedClans { get; set; } = true;

        #endregion

        #region UI Settings

        [SettingPropertyBool(
            "Show Golden Capital Names",
            Order = 0,
            RequireRestart = false,
            HintText = "Display capital settlement names with golden background on campaign map. Default: true")]
        [SettingPropertyGroup("UI Settings", GroupOrder = 2)]
        public bool ShowGoldenCapitalNames { get; set; } = true;

        [SettingPropertyBool(
            "Enable Capital Conquest Notifications",
            Order = 1,
            RequireRestart = false,
            HintText = "Show notifications when capitals are conquered. Default: true")]
        [SettingPropertyGroup("UI Settings", GroupOrder = 2)]
        public bool EnableConquestNotifications { get; set; } = true;

        #endregion

        #region Advanced Settings

        [SettingPropertyBool(
            "Enable Debug Logging",
            Order = 0,
            RequireRestart = false,
            HintText = "Write detailed debug information to log file. Default: false")]
        [SettingPropertyGroup("Advanced Settings", GroupOrder = 3)]
        public bool EnableDebugLogging { get; set; } = false;

        [SettingPropertyBool(
            "Allow Capital Level 4-5 Buildings",
            Order = 1,
            RequireRestart = true,
            HintText = "Enable construction of level 4-5 buildings in capitals. Requires game restart. Default: true")]
        [SettingPropertyGroup("Advanced Settings", GroupOrder = 3)]
        public bool AllowCapitalBuildingExtensions { get; set; } = true;

        #endregion
    }
}
