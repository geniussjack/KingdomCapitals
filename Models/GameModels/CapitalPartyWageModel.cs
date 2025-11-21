using KingdomCapitals.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace KingdomCapitals.Models.GameModels
{
    /// <summary>
    /// Custom wage model that makes capital garrison wages free (0 denars).
    /// Also removes wage limit so capitals can have unlimited garrison size.
    /// </summary>
    public class CapitalPartyWageModel : DefaultPartyWageModel
    {
        public override ExplainedNumber GetTotalWage(MobileParty mobileParty, bool includeDescriptions = false)
        {
            // Get base wage calculation
            ExplainedNumber result = base.GetTotalWage(mobileParty, includeDescriptions);

            // Only modify garrison parties
            if (mobileParty == null || !mobileParty.IsGarrison)
            {
                return result;
            }

            // Check if this garrison belongs to a capital
            if (mobileParty.CurrentSettlement != null &&
                CapitalManager.IsCapital(mobileParty.CurrentSettlement))
            {
                // Zero out the wage - capital garrisons are free
                float originalWage = result.ResultNumber;
                if (originalWage > 0)
                {
                    result.Add(-originalWage, new TextObject("{=capital_free_garrison}Capital Garrison (Free)"));
                }
            }

            return result;
        }

        /// <summary>
        /// Override wage limit for capital garrisons - return unlimited (int.MaxValue).
        /// This prevents the "Party / Payment Size Reach" penalty.
        /// </summary>
        public override int GetPartyWageLimitForSettlement(Settlement settlement)
        {
            // Check if this is a capital
            if (settlement != null && CapitalManager.IsCapital(settlement))
            {
                // Return unlimited wage limit for capitals
                // This prevents garrison size penalties
                return int.MaxValue;
            }

            // For non-capitals, use vanilla logic
            return base.GetPartyWageLimitForSettlement(settlement);
        }
    }
}
