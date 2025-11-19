using TaleWorlds.Library;

namespace KingdomCapitals.Constants
{
    /// <summary>
    /// UI-related constants including colors and display settings.
    /// </summary>
    public static class UIConstants
    {
        /// <summary>
        /// Golden color hex code for capital settlement names.
        /// RGB: 205, 175, 134
        /// </summary>
        public const string CapitalGoldenColorHex = "#cdaf86";

        /// <summary>
        /// Color tag format for Bannerlord UI system.
        /// Use with string.Format(ColorTagFormat, hexColor, text)
        /// </summary>
        public const string ColorTagFormat = "<color={0}>{1}</color>";

        /// <summary>
        /// Pre-defined colors for different message types.
        /// </summary>
        public static class MessageColors
        {
            public static readonly Color Success = Colors.Green;
            public static readonly Color Error = Colors.Red;
            public static readonly Color Warning = Colors.Yellow;
            public static readonly Color Info = Colors.White;
        }
    }
}
