using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using KingdomCapitals.Constants;

namespace KingdomCapitals.Services
{
    /// <summary>
    /// Service responsible for displaying conquest-related notifications to the player.
    /// </summary>
    public static class ConquestNotificationService
    {
        /// <summary>
        /// Notifies the player about a kingdom conquest.
        /// </summary>
        /// <param name="capital">The capital that was captured.</param>
        /// <param name="defeatedKingdom">The kingdom that was defeated.</param>
        /// <param name="conquererKingdom">The kingdom that conquered.</param>
        /// <param name="capturerHero">The hero who captured the capital.</param>
        public static void NotifyKingdomConquest(Settlement capital, Kingdom defeatedKingdom,
            Kingdom conquererKingdom, Hero capturerHero)
        {
            // General conquest message
            string message = string.Format(Messages.Conquest.KingdomFallenFormat, capital.Name.ToString(), defeatedKingdom.Name.ToString());
            InformationManager.DisplayMessage(new InformationMessage(message, UIConstants.MessageColors.Error));

            // Player-specific message if they were the capturer
            if (capturerHero == Hero.MainHero)
            {
                string playerMessage = string.Format(Messages.Conquest.PlayerConqueredKingdomFormat, defeatedKingdom.Name.ToString());
                InformationManager.DisplayMessage(new InformationMessage(playerMessage, UIConstants.MessageColors.Success));
            }
        }

        /// <summary>
        /// Notifies the player when they capture a capital without having a kingdom.
        /// </summary>
        /// <param name="capital">The capital that was captured.</param>
        /// <param name="defeatedKingdom">The kingdom that was defeated.</param>
        /// <param name="playerHero">The player hero.</param>
        public static void NotifyPlayerCapitalWithoutKingdom(Settlement capital, Kingdom defeatedKingdom, Hero playerHero)
        {
            string message = string.Format(Messages.Conquest.PlayerCapturedCapitalWithoutKingdomFormat,
                capital.Name.ToString(), defeatedKingdom.Name.ToString());
            InformationManager.DisplayMessage(new InformationMessage(message, UIConstants.MessageColors.Success));
        }

        /// <summary>
        /// Notifies the player to found a kingdom after capturing a capital.
        /// </summary>
        /// <param name="capital">The capital that was captured.</param>
        public static void NotifyFoundKingdom(Settlement capital)
        {
            string message = string.Format(Messages.Conquest.PlayerCapturedCapitalFoundKingdomFormat, capital.Name.ToString());
            InformationManager.DisplayMessage(new InformationMessage(message, UIConstants.MessageColors.Success));
        }
    }
}
