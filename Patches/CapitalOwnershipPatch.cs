using System;
using System.Linq;
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
    [HarmonyPatch]
    public static class Kingdom_AddDecision_Patch
    {
        /// <summary>
        /// Manually specifies the target method to patch.
        /// Required for methods with optional parameters.
        /// </summary>
        /// <returns>MethodBase of the AddDecision method.</returns>
        static System.Reflection.MethodBase TargetMethod()
        {
            try
            {
                var method = AccessTools.Method(
                    typeof(Kingdom),
                    "AddDecision",
                    new Type[] { typeof(KingdomDecision), typeof(bool) }
                );

                if (method == null)
                {
                    ModLogger.Error("Failed to find Kingdom.AddDecision method", null);
                }
                else
                {
                    ModLogger.Log("Successfully found Kingdom.AddDecision method for patching");
                }

                return method;
            }
            catch (Exception ex)
            {
                ModLogger.Error("Exception in Kingdom_AddDecision_Patch.TargetMethod", ex);
                return null;
            }
        }

        /// <summary>
        /// Determines if the patch should be applied.
        /// </summary>
        static bool Prepare()
        {
            try
            {
                // Log all methods in Kingdom class that contain "Decision"
                var allMethods = typeof(Kingdom).GetMethods(
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Static
                );

                ModLogger.Log("Searching for AddDecision method in Kingdom class...");
                foreach (var m in allMethods)
                {
                    if (m.Name.Contains("Decision"))
                    {
                        var parameters = m.GetParameters();
                        var paramStr = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
                        ModLogger.Log($"Found method: {m.Name}({paramStr})");
                    }
                }

                // Try to find the specific method
                var method = AccessTools.Method(
                    typeof(Kingdom),
                    "AddDecision",
                    new Type[] { typeof(KingdomDecision), typeof(bool) }
                );

                if (method == null)
                {
                    // Try without the bool parameter
                    method = AccessTools.Method(typeof(Kingdom), "AddDecision", new Type[] { typeof(KingdomDecision) });
                    if (method != null)
                    {
                        ModLogger.Log("Found AddDecision with single parameter only");
                    }
                }

                bool canPatch = method != null;
                ModLogger.Log($"Kingdom_AddDecision_Patch.Prepare: {(canPatch ? "Ready to patch" : "Cannot patch - method not found")}");

                // TEMPORARILY DISABLE THIS PATCH TO TEST OTHER PATCHES
                ModLogger.Log("TEMPORARILY DISABLED Kingdom_AddDecision_Patch to test other patches");
                return false;
            }
            catch (Exception ex)
            {
                ModLogger.Error("Exception in Kingdom_AddDecision_Patch.Prepare", ex);
                return false;
            }
        }

        /// <summary>
        /// Prefix patch to intercept decision creation.
        /// Returns false to prevent original method execution if this is a capital settlement decision.
        /// </summary>
        /// <param name="__instance">The Kingdom instance.</param>
        /// <param name="decision">The kingdom decision being added.</param>
        /// <param name="ignoreInfluenceCost">Whether to ignore influence cost for the decision.</param>
        /// <returns>False if the decision should be blocked, true to allow normal execution.</returns>
        static bool Prefix(Kingdom __instance, KingdomDecision decision, bool ignoreInfluenceCost)
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
    [HarmonyPatch]
    public static class SettlementClaimantDecision_Constructor_Patch
    {
        /// <summary>
        /// Manually specifies the target constructor to patch.
        /// </summary>
        /// <returns>MethodBase of the SettlementClaimantDecision constructor.</returns>
        static System.Reflection.MethodBase TargetMethod()
        {
            return AccessTools.Constructor(
                typeof(SettlementClaimantDecision),
                new Type[] { typeof(Clan), typeof(Settlement) }
            );
        }

        /// <summary>
        /// Determines if the patch should be applied.
        /// </summary>
        static bool Prepare()
        {
            var constructor = AccessTools.Constructor(
                typeof(SettlementClaimantDecision),
                new Type[] { typeof(Clan), typeof(Settlement) }
            );

            bool canPatch = constructor != null;
            ModLogger.Log($"SettlementClaimantDecision_Constructor_Patch.Prepare: {(canPatch ? "ENABLED - Testing constructor patch" : "Cannot patch - constructor not found")}");
            return canPatch;
        }

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
    [HarmonyPatch]
    public static class SettlementClaimantDecision_DetermineSupport_Patch
    {
        /// <summary>
        /// Manually specifies the target method to patch.
        /// </summary>
        /// <returns>MethodBase of the DetermineSupport method.</returns>
        static System.Reflection.MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(SettlementClaimantDecision), "DetermineSupport");
        }

        /// <summary>
        /// Determines if the patch should be applied.
        /// </summary>
        static bool Prepare()
        {
            // TEMPORARILY DISABLE THIS PATCH TO TEST OTHER PATCHES
            ModLogger.Log("TEMPORARILY DISABLED SettlementClaimantDecision_DetermineSupport_Patch to test other patches");
            return false;
        }

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
