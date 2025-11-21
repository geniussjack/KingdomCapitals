using KingdomCapitals.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace KingdomCapitals.Models.GameModels
{
    /// <summary>
    /// Custom militia model that doubles daily militia project bonuses for capitals.
    /// Only affects Militia Training daily default project (daily_militia).
    /// </summary>
    public class CapitalMilitiaModel : DefaultSettlementMilitiaModel
    {
        public override ExplainedNumber CalculateMilitiaChange(
            Settlement settlement,
            bool includeDescriptions = false)
        {
            ExplainedNumber result = base.CalculateMilitiaChange(settlement, includeDescriptions);

            // Check if this is a capital with a town
            if (settlement?.Town == null || !CapitalManager.IsCapital(settlement))
            {
                return result;
            }

            Town town = settlement.Town;

            // Only apply bonus when:
            // 1. No building construction is in progress
            // 2. Militia Training daily default is selected
            if (town.CurrentBuilding == null &&
                town.CurrentDefaultBuilding != null &&
                town.CurrentDefaultBuilding.BuildingType.StringId.Contains("daily_militia"))
            {
                // Double the daily militia bonus (+1 base becomes +2)
                // Daily project gives +1, we add another +1 to make it +2
                result.Add(1f, new TextObject("{=capital_daily_bonus}Capital Daily Project Bonus"));
            }

            return result;
        }
    }
}
