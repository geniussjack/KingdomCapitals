using KingdomCapitals.Core;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Localization;

namespace KingdomCapitals.Models.GameModels
{
    /// <summary>
    /// Custom building construction model that allows capitals to build up to level 5.
    /// Vanilla game limits buildings to level 3.
    /// </summary>
    public class CapitalBuildingConstructionModel : DefaultBuildingConstructionModel
    {
        private const int CapitalMaxBuildingLevel = 5;
        private const int VanillaMaxBuildingLevel = 3;

        /// <summary>
        /// Override to allow capitals to build up to level 5 instead of vanilla level 3.
        /// </summary>
        public override int GetMaxLevel(BuildingType buildingType, Town town)
        {
            // Check if this is a capital
            if (town?.Settlement != null && CapitalManager.IsCapital(town.Settlement))
            {
                return CapitalMaxBuildingLevel;
            }

            // Non-capitals use vanilla logic
            return base.GetMaxLevel(buildingType, town);
        }

        /// <summary>
        /// Override to allow construction of level 4 and 5 buildings in capitals.
        /// </summary>
        public override bool CanBuildingBeBuilt(Town town, BuildingType buildingType, out TextObject explanation, out bool disableInputs, out BuildingLocation buildingLocation)
        {
            // First check vanilla logic
            bool canBuild = base.CanBuildingBeBuilt(town, buildingType, out explanation, out disableInputs, out buildingLocation);

            if (!canBuild)
            {
                return false;
            }

            // If this is a capital, allow building up to level 5
            if (town?.Settlement != null && CapitalManager.IsCapital(town.Settlement))
            {
                Building existingBuilding = town.Buildings.FirstOrDefault(b => b.BuildingType == buildingType);

                if (existingBuilding != null)
                {
                    // Allow upgrade if current level is less than 5
                    if (existingBuilding.CurrentLevel >= CapitalMaxBuildingLevel)
                    {
                        explanation = new TextObject("{=capital_max_level}Maximum building level reached for capitals (Level {LEVEL})");
                        explanation.SetTextVariable("LEVEL", CapitalMaxBuildingLevel);
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
