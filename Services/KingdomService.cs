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
        /// Vassalizes all clans from a defeated kingdom to the conquerer.
        /// </summary>
        /// <param name="defeatedKingdom">The kingdom that was defeated.</param>
        /// <param name="conquererKingdom">The kingdom that conquered.</param>
        /// <returns>The number of clans successfully vassalized.</returns>
        public static int VassalizeDefeatedClans(Kingdom defeatedKingdom, Kingdom conquererKingdom)
        {
            try
            {
                int vassalizedCount = 0;
                List<Clan> clansToVassalize = defeatedKingdom.Clans.ToList();

                foreach (Clan clan in clansToVassalize)
                {
                    if (clan != null && !clan.IsEliminated && clan != defeatedKingdom.RulingClan)
                    {
                        // Make clan join conquerer kingdom as vassal
                        ChangeKingdomAction.ApplyByJoinToKingdom(clan, conquererKingdom, false);
                        ModLogger.Log(string.Format(Messages.Log.VassalizedClanFormat, clan.Name.ToString(), conquererKingdom.Name.ToString()));
                        vassalizedCount++;
                    }
                }

                return vassalizedCount;
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Error vassalizing clans from {defeatedKingdom?.Name?.ToString() ?? "Unknown"}", ex);
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
