using KingdomCapitals.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace KingdomCapitals.Models.GameModels
{
    /// <summary>
    /// Custom garrison model that modifies garrison changes for capitals.
    /// Capitals get +3 troops per day instead of vanilla +1.
    /// Note: Actual troop addition is handled by CapitalGarrisonBehavior.
    /// This model ensures the UI displays the correct values.
    /// </summary>
    public class CapitalGarrisonModel : DefaultSettlementGarrisonModel
    {
        /// <summary>
        /// Override main garrison change calculation to show +3 for capitals in UI.
        /// For capitals: returns +3 for UI (actual recruitment handled by CapitalGarrisonBehavior).
        /// For non-capitals: uses vanilla logic.
        /// </summary>
        public override void CalculateGarrisonChange(Settlement settlement, out ExplainedNumber result)
        {
            // For capitals, create custom result to show +3 in UI
            if (settlement != null && settlement.IsTown && CapitalManager.IsCapital(settlement))
            {
                // Create new ExplainedNumber with 0 base, then add +3 for UI display
                result = new ExplainedNumber(0f, true);
                result.Add(3f, new TextObject("{=capital_garrison}Capital Garrison Bonus"));
                return;
            }

            // For non-capitals, use vanilla logic
            base.CalculateGarrisonChange(settlement, out result);
        }

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
