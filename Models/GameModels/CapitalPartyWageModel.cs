using KingdomCapitals.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;

namespace KingdomCapitals.Models.GameModels
{
    /// <summary>
    /// Custom wage model that makes capital garrison wages free (0 denars).
    /// </summary>
    public class CapitalPartyWageModel : DefaultPartyWageModel
    {
        public override int GetTotalWage(MobileParty mobileParty, bool includeDescriptions = false)
        {
            // Get base wage calculation
            int baseWage = base.GetTotalWage(mobileParty, includeDescriptions);

            // Only modify garrison parties
            if (mobileParty == null || !mobileParty.IsGarrison)
            {
                return baseWage;
            }

            // Check if this garrison belongs to a capital
            if (mobileParty.CurrentSettlement != null &&
                CapitalManager.IsCapital(mobileParty.CurrentSettlement))
            {
                return 0; // Free garrison for capitals
            }

            return baseWage;
        }

        public override ExplainedNumber GetTotalWage(MobileParty mobileParty, bool includeDescriptions = false, int dayOfYear = -1)
        {
            // Get base wage calculation
            ExplainedNumber result = base.GetTotalWage(mobileParty, includeDescriptions, dayOfYear);

            // Only modify garrison parties
            if (mobileParty == null || !mobileParty.IsGarrison)
            {
                return result;
            }

            // Check if this garrison belongs to a capital
            if (mobileParty.CurrentSettlement != null &&
                CapitalManager.IsCapital(mobileParty.CurrentSettlement))
            {
                // Zero out the wage
                float originalWage = result.ResultNumber;
                if (originalWage > 0)
                {
                    result.Add(-originalWage, new TextObject("Capital Garrison (Free)"));
                }
            }

            return result;
        }
    }
}
