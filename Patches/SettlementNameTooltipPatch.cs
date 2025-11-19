using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;
using KingdomCapitals.Core;
using KingdomCapitals.Utils;
using KingdomCapitals.Constants;

namespace KingdomCapitals.Patches
{
    /// <summary>
    /// Harmony patch to display capital settlement names with golden color markup.
    /// Patches Settlement.Name property getter to add HTML color tags to capital names.
    /// Uses Bannerlord's built-in color tag support for visual distinction.
    /// </summary>
    [HarmonyPatch(typeof(Settlement), "Name", MethodType.Getter)]
    public static class SettlementNameColorPatch
    {
        /// <summary>
        /// Cache to track which settlements have already been marked as capitals.
        /// Prevents redundant text modifications and improves performance.
        /// Key: Settlement.StringId, Value: modified TextObject
        /// </summary>
        private static readonly System.Collections.Generic.Dictionary<string, TextObject> _capitalNameCache
            = new System.Collections.Generic.Dictionary<string, TextObject>();

        /// <summary>
        /// Postfix patch for Settlement.Name getter.
        /// Adds golden color markup to capital settlement names.
        /// </summary>
        /// <param name="__instance">The Settlement instance.</param>
        /// <param name="__result">The original name TextObject (will be modified for capitals).</param>
        static void Postfix(Settlement __instance, ref TextObject __result)
        {
            try
            {
                // Skip if settlement is null or result is null
                if (__instance == null || __result == null)
                    return;

                // Check if this settlement is a capital
                if (!CapitalManager.IsCapital(__instance))
                    return;

                // Check cache first to avoid redundant modifications
                if (_capitalNameCache.TryGetValue(__instance.StringId, out TextObject cachedName))
                {
                    __result = cachedName;
                    return;
                }

                // Get the original name text
                string originalName = __result.ToString();

                // Skip if name is empty or already contains color tags
                if (string.IsNullOrEmpty(originalName) || originalName.Contains("<color="))
                    return;

                // Create new TextObject with golden color markup
                string goldColoredName = string.Format(UIConstants.ColorTagFormat, UIConstants.CapitalGoldenColorHex, originalName);
                TextObject coloredTextObject = new TextObject(goldColoredName);

                // Cache the modified name
                _capitalNameCache[__instance.StringId] = coloredTextObject;

                // Return the colored name
                __result = coloredTextObject;

                ModLogger.Log(string.Format(Messages.Log.AppliedGoldenColorFormat, originalName));
            }
            catch (Exception ex)
            {
                // Suppress errors to avoid log spam and game crashes
                // Only log critical errors
                ModLogger.Error("Error in SettlementNameColorPatch", ex);
            }
        }

        /// <summary>
        /// Clears the capital name cache.
        /// Call this when capitals change or when starting a new game.
        /// </summary>
        public static void ClearCache()
        {
            _capitalNameCache.Clear();
            ModLogger.Log(Messages.Log.CapitalNameCacheCleared);
        }
    }
}
