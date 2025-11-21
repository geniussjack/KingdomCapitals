using KingdomCapitals.Constants;
using KingdomCapitals.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace KingdomCapitals.Services
{
    /// <summary>
    /// Service responsible for kingdom-level operations such as destruction and vassalization.
    /// </summary>
    public static class KingdomService
    {
        /// <summary>
        /// Vassalizes independent clans (after their kingdom was destroyed) to the conquering kingdom.
        /// This must be called AFTER DestroyKingdom to work properly.
        /// </summary>
        /// <param name="clans">List of clans to vassalize (should be independent after kingdom destruction).</param>
        /// <param name="conquererKingdom">The kingdom that will accept these clans as vassals.</param>
        /// <returns>The number of clans successfully vassalized.</returns>
        public static int VassalizeIndependentClans(List<Clan> clans, Kingdom conquererKingdom)
        {
            try
            {
                if (clans == null || conquererKingdom == null)
                {
                    ModLogger.Error("VassalizeIndependentClans: null parameters provided");
                    return 0;
                }

                int vassalizedCount = 0;

                foreach (Clan clan in clans)
                {
                    // Only vassalize clans that became independent after kingdom destruction
                    if (clan != null && !clan.IsEliminated && clan.Kingdom == null)
                    {
                        // Make independent clan join conquerer kingdom as vassal
                        ChangeKingdomAction.ApplyByJoinToKingdom(clan, conquererKingdom, false);
                        ModLogger.Log(string.Format(Messages.Log.VassalizedClanFormat, clan.Name.ToString(), conquererKingdom.Name.ToString()));
                        vassalizedCount++;
                    }
                    else if (clan != null && clan.Kingdom != null)
                    {
                        // Clan is still in a kingdom (shouldn't happen if DestroyKingdom was called)
                        ModLogger.Warning($"Clan {clan.Name.ToString()} is still in kingdom {clan.Kingdom.Name.ToString()}, cannot vassalize");
                    }
                }

                ModLogger.Log($"Total clans vassalized: {vassalizedCount} to {conquererKingdom.Name.ToString()}");
                return vassalizedCount;
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Error vassalizing independent clans to {conquererKingdom?.Name?.ToString() ?? "Unknown"}", ex);
                return 0;
            }
        }

        /// <summary>
        /// Destroys a kingdom completely.
        /// </summary>
        /// <param name="kingdom">The kingdom to destroy.</param>
        /// <returns>True if destruction was successful, false otherwise.</returns>
        public static bool DestroyKingdom(Kingdom kingdom)
        {
            try
            {
                if (kingdom == null || kingdom.IsEliminated)
                {
                    return false;
                }

                DestroyKingdomAction.Apply(kingdom);
                ModLogger.Log(string.Format(Messages.Log.KingdomDestroyedFormat, kingdom.Name.ToString()));
                return true;
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Error destroying kingdom {kingdom?.Name?.ToString() ?? "Unknown"}", ex);
                return false;
            }
        }
    }
}
