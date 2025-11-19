using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Settlements;
using KingdomCapitals.Core;
using KingdomCapitals.Utils;
using KingdomCapitals.Constants;

namespace KingdomCapitals.Patches
{
    /// <summary>
    /// Harmony patch to prevent voting on captured capital settlements.
    /// Ensures capitals are transferred directly to the ruling clan without democratic decision.
    /// </summary>
    [HarmonyPatch(typeof(Kingdom), "AddDecision")]
    public static class Kingdom_AddDecision_Patch
    {
        /// <summary>
        /// Prefix patch to intercept decision creation.
        /// Returns false to prevent original method execution if this is a capital settlement decision.
        /// </summary>
        /// <param name="__instance">The Kingdom instance.</param>
        /// <param name="decision">The kingdom decision being added.</param>
        /// <param name="ignoreInfluenceCost">Whether to ignore influence cost for the decision.</param>
        /// <returns>False if the decision should be blocked, true to allow normal execution.</returns>
        static bool Prefix(Kingdom __instance, KingdomDecision decision, bool ignoreInfluenceCost = false)
        {
            try
            {
                // Check if this is a settlement distribution decision
                if (decision is SettlementClaimantDecision settlementDecision)
                {
                    Settlement settlement = settlementDecision.Settlement;

                    if (settlement == null)
                        return true; // Continue with normal flow

                    // Check if this settlement was recently captured as a capital
                    if (CapitalManager.WasRecentlyCapturedCapital(settlement))
                    {
                        ModLogger.Log(string.Format(Messages.Log.BlockedVotingFormat, settlement.Name));

                        // Transfer directly to ruling clan (should already be done by CapitalConquestBehavior)
                        if (__instance.RulingClan != null && settlement.OwnerClan != __instance.RulingClan)
                        {
                            CapitalManager.TransferCapitalOwnership(__instance.RulingClan.Leader, settlement);
                        }

                        return false; // Prevent voting
                    }
                }

                return true; // Allow normal execution for non-capital settlements
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in Kingdom_AddDecision_Patch", ex);
                return true; // Allow execution on error to prevent game breaking
            }
        }
    }

    /// <summary>
    /// Alternative patch for settlement claiming decision proposals.
    /// Prevents settlement claimant proposals for recently captured capitals.
    /// </summary>
    [HarmonyPatch(typeof(SettlementClaimantDecision), MethodType.Constructor, new Type[] { typeof(Clan), typeof(Settlement) })]
    public static class SettlementClaimantDecision_Constructor_Patch
    {
        /// <summary>
        /// Prefix patch to prevent construction of settlement claimant decisions for capitals.
        /// </summary>
        /// <param name="proposerClan">The clan proposing the settlement claim.</param>
        /// <param name="settlement">The settlement being claimed.</param>
        /// <returns>False if the decision should be blocked, true to allow normal execution.</returns>
        static bool Prefix(Clan proposerClan, Settlement settlement)
        {
            try
            {
                if (settlement == null)
                    return true;

                // Block decision creation for recently captured capitals
                if (CapitalManager.WasRecentlyCapturedCapital(settlement))
                {
                    ModLogger.Log(string.Format(Messages.Log.PreventedDecisionCreationFormat, settlement.Name));
                    return false; // Prevent decision creation
                }

                return true;
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in SettlementClaimantDecision_Constructor_Patch", ex);
                return true;
            }
        }
    }

    /// <summary>
    /// Patch to prevent AI lords from requesting captured capitals in diplomatic discussions.
    /// </summary>
    [HarmonyPatch(typeof(SettlementClaimantDecision), "DetermineSupport")]
    public static class SettlementClaimantDecision_DetermineSupport_Patch
    {
        /// <summary>
        /// Prefix patch to prevent support determination for capital settlements.
        /// </summary>
        /// <param name="__instance">The SettlementClaimantDecision instance.</param>
        /// <param name="__result">The supporter result to be modified.</param>
        /// <returns>False if support determination should be blocked, true to allow normal execution.</returns>
        static bool Prefix(SettlementClaimantDecision __instance, ref Supporter __result)
        {
            try
            {
                Settlement settlement = __instance.Settlement;

                if (settlement != null && CapitalManager.WasRecentlyCapturedCapital(settlement))
                {
                    // Return null supporter to indicate no support
                    __result = null;
                    return false; // Skip original method
                }

                return true;
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in SettlementClaimantDecision_DetermineSupport_Patch", ex);
                return true;
            }
        }
    }
}
