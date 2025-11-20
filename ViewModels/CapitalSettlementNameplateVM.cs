using KingdomCapitals.Core;
using SandBox.ViewModelCollection.Nameplate;
using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace KingdomCapitals.ViewModels
{
    /// <summary>
    /// Extended ViewModel for settlement nameplates that adds capital city crown icon.
    /// Inherits from SettlementNameplateVM and adds IsCapital property for UI binding.
    /// </summary>
    public class CapitalSettlementNameplateVM : SettlementNameplateVM
    {
        private bool _isCapital;

        /// <summary>
        /// Indicates whether this settlement is a kingdom capital.
        /// Bound to XML UI to show/hide crown icon.
        /// </summary>
        [DataSourceProperty]
        public bool IsCapital
        {
            get => _isCapital;
            set
            {
                if (_isCapital != value)
                {
                    _isCapital = value;
                    OnPropertyChangedWithValue(value, "IsCapital");
                }
            }
        }

        /// <summary>
        /// Constructor - creates nameplate ViewModel with capital status.
        /// </summary>
        public CapitalSettlementNameplateVM(
            Settlement settlement,
            GameEntity targetEntity,
            Camera camera,
            Action<Vec2, float> fastMoveCameraToPosition)
            : base(settlement, targetEntity, camera, fastMoveCameraToPosition)
        {
            // Check capital status
            _isCapital = CapitalManager.IsCapital(settlement);
        }

        /// <summary>
        /// Refresh capital status when dynamic properties update.
        /// </summary>
        public override void RefreshDynamicProperties(bool forceUpdate)
        {
            base.RefreshDynamicProperties(forceUpdate);

            // Update capital status
            if (Settlement != null)
            {
                IsCapital = CapitalManager.IsCapital(Settlement);
            }
        }
    }
}
