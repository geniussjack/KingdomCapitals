using KingdomCapitals.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace KingdomCapitals.Models.GameModels
{
    /// <summary>
    /// Custom prosperity model that doubles daily prosperity project bonuses for capitals.
    /// Only affects Festival and Games daily default project (daily_prosperity).
    /// </summary>
    public class CapitalProsperityModel : DefaultSettlementProsperityModel
    {
        public override ExplainedNumber CalculateProsperityChange(
            Town town,
            bool includeDescriptions = false)
        {
            ExplainedNumber result = base.CalculateProsperityChange(town, includeDescriptions);

            // Check if this is a capital
            if (town?.Settlement == null || !CapitalManager.IsCapital(town.Settlement))
            {
                return result;
            }

            // Only apply bonus when:
            // 1. No building construction is in progress
            // 2. Festival and Games daily default is selected
            if (town.CurrentBuilding == null &&
                town.CurrentDefaultBuilding != null &&
                town.CurrentDefaultBuilding.BuildingType.StringId.Contains("daily_prosperity"))
            {
                // Double the daily prosperity bonus (+1 base becomes +2)
                // Daily project gives +1, we add another +1 to make it +2
                result.Add(1f, new TextObject("{=capital_daily_bonus}Capital Daily Project Bonus"));
            }

            return result;
        }
    }
}
