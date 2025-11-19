using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;

namespace KingdomCapitals.Utils
{
    /// <summary>
    /// Stores and manages capital city data for all kingdoms.
    /// </summary>
    public static class CapitalData
    {
        /// <summary>
        /// Mapping of kingdom StringId to capital settlement StringId.
        /// Based on Bannerlord v1.2.12+ default capitals.
        /// </summary>
        private static readonly Dictionary<string, string> KingdomCapitalMap = new Dictionary<string, string>
        {
            { "battania", "town_B1" },      // Marunath
            { "vlandia", "town_V5" },       // Galend
            { "aserai", "town_A1" },        // Quyaz
            { "sturgia", "town_S2" },       // Balgard
            { "khuzait", "town_K3" },       // Makeb
            { "empire_w", "town_EW3" },     // Jalmarys (Western Empire)
            { "empire", "town_EN2" },       // Diathma (Northern Empire)
            { "empire_s", "town_ES4" }      // Lycaron (Southern Empire)
        };

        /// <summary>
        /// Checks if a settlement is designated as a capital.
        /// </summary>
        public static bool IsDefaultCapital(Settlement settlement)
        {
            if (settlement == null || !settlement.IsTown)
                return false;

            return KingdomCapitalMap.ContainsValue(settlement.StringId);
        }

        /// <summary>
        /// Gets the default capital settlement for a kingdom.
        /// </summary>
        public static Settlement GetDefaultCapital(Kingdom kingdom)
        {
            if (kingdom == null)
                return null;

            if (!KingdomCapitalMap.TryGetValue(kingdom.StringId, out string capitalStringId))
                return null;

            return Settlement.Find(capitalStringId);
        }

        /// <summary>
        /// Gets all default capital settlements.
        /// </summary>
        public static IEnumerable<Settlement> GetAllDefaultCapitals()
        {
            return KingdomCapitalMap.Values
                .Select(Settlement.Find)
                .Where(s => s != null);
        }

        /// <summary>
        /// Gets the kingdom that should own this capital by default.
        /// </summary>
        public static Kingdom GetDefaultKingdomForCapital(Settlement settlement)
        {
            if (settlement == null)
                return null;

            var entry = KingdomCapitalMap.FirstOrDefault(kvp => kvp.Value == settlement.StringId);
            if (entry.Key == null)
                return null;

            return Kingdom.All.FirstOrDefault(k => k.StringId == entry.Key);
        }
    }
}
