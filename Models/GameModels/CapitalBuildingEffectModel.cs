using KingdomCapitals.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;

namespace KingdomCapitals.Models.GameModels
{
    /// <summary>
    /// Custom building effect model that doubles daily project bonuses for capitals.
    /// This affects all daily projects (Housing, Irrigation, Festival and Games, etc.)
    /// </summary>
    public class CapitalBuildingEffectModel : DefaultBuildingEffectModel
    {
        /// <summary>
        /// Override to double the effect of daily projects in capitals.
        /// </summary>
        public override float GetBuildingEffectAmount(Building building, BuildingEffectEnum effect)
        {
            float baseAmount = base.GetBuildingEffectAmount(building, effect);

            // Check if this is a capital and a daily project
            if (building?.Town?.Settlement != null &&
                CapitalManager.IsCapital(building.Town.Settlement) &&
                building.BuildingType.StringId.StartsWith("daily_"))
            {
                // Double the effect for daily projects in capitals
                return baseAmount * 2f;
            }

            return baseAmount;
        }
    }
}
