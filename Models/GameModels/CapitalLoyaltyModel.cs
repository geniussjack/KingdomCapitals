using KingdomCapitals.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace KingdomCapitals.Models.GameModels
{
    /// <summary>
    /// Custom loyalty model that doubles daily loyalty project bonuses for capitals.
    /// Only affects Housing daily default project (daily_loyalty).
    /// </summary>
    public class CapitalLoyaltyModel : DefaultSettlementLoyaltyModel
    {
        public override ExplainedNumber CalculateLoyaltyChange(
            Town town,
            bool includeDescriptions = false)
        {
            ExplainedNumber result = base.CalculateLoyaltyChange(town, includeDescriptions);

            // Check if this is a capital
            if (town?.Settlement == null || !CapitalManager.IsCapital(town.Settlement))
            {
                return result;
            }

            // Only apply bonus when:
            // 1. No building construction is in progress
            // 2. Housing daily default is selected
            if (town.CurrentBuilding == null &&
                town.CurrentDefaultBuilding != null &&
                town.CurrentDefaultBuilding.BuildingType.StringId.Contains("daily_loyalty"))
            {
                // Double the daily loyalty bonus
                float originalValue = result.ResultNumber;
                result.Add(originalValue, new TextObject("Capital Bonus (x2 Daily Project)"));
            }

            return result;
        }
    }
}
