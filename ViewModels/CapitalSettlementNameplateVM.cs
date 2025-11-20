using HarmonyLib;
using KingdomCapitals.Core;
using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar.MapNameplates;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace KingdomCapitals.ViewModels
{
    /// <summary>
    /// Custom ViewModel that extends SettlementNameplateVM to add capital city crown icon.
    /// </summary>
    public class CapitalSettlementNameplateVM : SettlementNameplateVM
    {
        private bool _isCapital;

        /// <summary>
        /// Indicates whether this settlement is a capital city.
        /// When true, a crown icon will be displayed over the settlement nameplate.
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
        /// Constructor that initializes the capital settlement nameplate.
        /// </summary>
        /// <param name="settlement">The settlement entity</param>
        /// <param name="targetEntity">The game entity for the nameplate</param>
        /// <param name="camera">The map camera</param>
        /// <param name="fastMoveCameraToPosition">Callback for camera movement</param>
        public CapitalSettlementNameplateVM(
            Settlement settlement,
            GameEntity targetEntity,
            Camera camera,
            Action<Vec2, float> fastMoveCameraToPosition)
            : base(settlement, targetEntity, camera, fastMoveCameraToPosition)
        {
            // Check if this settlement is a capital
            IsCapital = CapitalManager.IsCapital(settlement);
        }

        /// <summary>
        /// Refresh method to update capital status dynamically.
        /// Called when game state changes (e.g., capital conquest).
        /// </summary>
        public void RefreshCapitalStatus()
        {
            if (Settlement != null)
            {
                IsCapital = CapitalManager.IsCapital(Settlement);
            }
        }
    }
}
