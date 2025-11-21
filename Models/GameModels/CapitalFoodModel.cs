using KingdomCapitals.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace KingdomCapitals.Models.GameModels
{
    /// <summary>
    /// Custom food model that doubles daily food project bonuses for capitals.
    /// Only affects Irrigation daily default project (daily_food).
    /// </summary>
    public class CapitalFoodModel : DefaultSettlementFoodModel
    {
        public override ExplainedNumber CalculateTownFoodStocksChange(
            Town town,
            bool includeMarketStocks = true,
            bool includeDescriptions = false)
        {
            ExplainedNumber result = base.CalculateTownFoodStocksChange(town, includeMarketStocks, includeDescriptions);

            // Check if this is a capital
            if (town?.Settlement == null || !CapitalManager.IsCapital(town.Settlement))
            {
                return result;
            }

            // Only apply bonus when:
            // 1. No building construction is in progress
            // 2. Irrigation daily default is selected
            if (town.CurrentBuilding == null &&
                town.CurrentDefaultBuilding != null &&
                town.CurrentDefaultBuilding.BuildingType.StringId.Contains("daily_food"))
            {
                // Double the daily food bonus (+1 base becomes +2)
                // Daily project gives +1, we add another +1 to make it +2
                result.Add(1f, new TextObject("{=capital_daily_bonus}Capital Daily Project Bonus"));
            }

            return result;
        }
    }
}
