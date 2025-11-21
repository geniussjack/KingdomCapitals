using KingdomCapitals.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;

namespace KingdomCapitals.Models.GameModels
{
    /// <summary>
    /// Custom garrison model that disables vanilla auto-recruitment for capitals.
    /// CapitalGarrisonBehavior handles recruitment with +3 troops instead of vanilla +1.
    /// </summary>
    public class CapitalGarrisonModel : DefaultSettlementGarrisonModel
    {
        /// <summary>
        /// Override auto-recruitment to prevent vanilla recruitment for capitals.
        /// CapitalGarrisonBehavior handles all recruitment for capitals.
        /// </summary>
        public override ExplainedNumber CalculateGarrisonChangeAutoRecruitment(
            Settlement settlement,
            bool includeDescriptions = false)
        {
            // For capitals, return 0 (CapitalGarrisonBehavior will add +3)
            if (settlement != null && CapitalManager.IsCapital(settlement))
            {
                return new ExplainedNumber(0f, includeDescriptions);
            }

            // For non-capitals, use vanilla logic (+1 troop)
            return base.CalculateGarrisonChangeAutoRecruitment(settlement, includeDescriptions);
        }
    }
}
