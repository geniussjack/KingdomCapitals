using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using KingdomCapitals.Constants;
using KingdomCapitals.Utils;

namespace KingdomCapitals.Services
{
    /// <summary>
    /// Service responsible for transferring settlements between kingdoms and clans.
    /// Handles capital transfers, mass settlement transfers, and related operations.
    /// </summary>
    public static class SettlementTransferService
    {
        /// <summary>
        /// Transfers a captured capital directly to the ruling clan without voting.
        /// </summary>
        /// <param name="capital">The capital settlement to transfer.</param>
        /// <param name="conquererKingdom">The kingdom that conquered the capital.</param>
        /// <returns>True if transfer was successful, false otherwise.</returns>
        public static bool TransferCapitalToRulingClan(Settlement capital, Kingdom conquererKingdom)
        {
            try
            {
                if (conquererKingdom?.RulingClan == null)
                {
                    ModLogger.Error(string.Format(Messages.Errors.TransferToRulingClanNullRulingClan, capital?.Name ?? "Unknown"));
                    return false;
                }

                Hero rulingClanLeader = conquererKingdom.RulingClan.Leader;

                // Force transfer to ruling clan
                ChangeOwnerOfSettlementAction.ApplyByDefault(rulingClanLeader, capital);

                ModLogger.Log(string.Format(Messages.Log.CapitalTransferredToRulingClanFormat, capital.Name, conquererKingdom.RulingClan.Name));
                return true;
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Error transferring capital {capital?.Name} to ruling clan", ex);
                return false;
            }
        }

        /// <summary>
        /// Transfers all settlements of a defeated kingdom to the conquerer's ruling clan.
        /// </summary>
        /// <param name="defeatedKingdom">The kingdom that was defeated.</param>
        /// <param name="conquererKingdom">The kingdom that conquered.</param>
        /// <returns>The number of settlements successfully transferred.</returns>
        public static int TransferAllSettlements(Kingdom defeatedKingdom, Kingdom conquererKingdom)
        {
            try
            {
                int transferredCount = 0;
                var settlementsToTransfer = defeatedKingdom.Settlements.ToList();

                foreach (Settlement settlement in settlementsToTransfer)
                {
                    if (settlement.OwnerClan?.Kingdom == defeatedKingdom)
                    {
                        // Transfer to conquerer's ruling clan
                        ChangeOwnerOfSettlementAction.ApplyByDefault(conquererKingdom.RulingClan.Leader, settlement);
                        ModLogger.Log(string.Format(Messages.Log.TransferredSettlementFormat,
                            settlement.Name, defeatedKingdom.Name, conquererKingdom.Name));
                        transferredCount++;
                    }
                }

                return transferredCount;
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Error transferring settlements from {defeatedKingdom?.Name}", ex);
                return 0;
            }
        }

        /// <summary>
        /// Transfers a settlement to a specific owner.
        /// </summary>
        /// <param name="settlement">The settlement to transfer.</param>
        /// <param name="newOwner">The hero who will become the new owner.</param>
        /// <returns>True if transfer was successful, false otherwise.</returns>
        public static bool TransferSettlement(Settlement settlement, Hero newOwner)
        {
            if (newOwner == null || settlement == null)
            {
                ModLogger.Error(Messages.Errors.TransferCapitalOwnershipNullParameters);
                return false;
            }

            try
            {
                ChangeOwnerOfSettlementAction.ApplyByDefault(newOwner, settlement);
                ModLogger.Log(string.Format(Messages.Log.TransferredCapitalFormat, settlement.Name, newOwner.Name));
                return true;
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Failed to transfer settlement: {settlement.Name}", ex);
                return false;
            }
        }
    }
}
