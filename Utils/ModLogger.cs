using KingdomCapitals.Constants;
using System;
using System.IO;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace KingdomCapitals.Utils
{
    /// <summary>
    /// Provides logging functionality for the Kingdom Capitals mod.
    /// Writes timestamped log entries to file and displays critical messages in-game.
    /// </summary>
    public static class ModLogger
    {
        private static readonly string LogFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            LogConstants.LogSubdirectory,
            LogConstants.LogsFolderName,
            ModConstants.LogFileName
        );

        private static readonly object _lockObject = new();

        static ModLogger()
        {
            try
            {
                string logDirectory = Path.GetDirectoryName(LogFilePath);
                if (!Directory.Exists(logDirectory))
                {
                    _ = Directory.CreateDirectory(logDirectory);
                }
            }
            catch (Exception ex)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"{Messages.Errors.FailedToCreateLogDirectory}: {ex.Message}",
                    UIConstants.MessageColors.Error
                ));
            }
        }

        /// <summary>
        /// Clears the log file at the start of a new session.
        /// </summary>
        public static void ClearLog()
        {
            try
            {
                lock (_lockObject)
                {
                    if (File.Exists(LogFilePath))
                    {
                        File.Delete(LogFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                // If we can't clear the log, just continue - not critical
                InformationManager.DisplayMessage(new InformationMessage(
                    $"Failed to clear log file: {ex.Message}",
                    UIConstants.MessageColors.Warning
                ));
            }
        }

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Log(string message)
        {
            LogToFile($"{LogConstants.LogLevel.Info} {message}");
        }

        /// <summary>
        /// Logs a warning message and displays it in-game.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        public static void Warning(string message)
        {
            LogToFile($"{LogConstants.LogLevel.Warning} {message}");
            InformationManager.DisplayMessage(new InformationMessage(
                $"[{ModConstants.ModName}] {message}",
                UIConstants.MessageColors.Warning
            ));
        }

        /// <summary>
        /// Logs an error message with optional exception details and displays it in-game.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        /// <param name="ex">Optional exception object for additional details.</param>
        public static void Error(string message, Exception ex = null)
        {
            string logMessage = ex != null
                ? $"{LogConstants.LogLevel.Error} {message}\nException: {ex.GetType().Name}\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}"
                : $"{LogConstants.LogLevel.Error} {message}";

            LogToFile(logMessage);
            InformationManager.DisplayMessage(new InformationMessage(
                $"[{ModConstants.ModName} ERROR] {message}",
                UIConstants.MessageColors.Error
            ));
        }

        /// <summary>
        /// Logs capital conquest event details.
        /// </summary>
        /// <param name="capital">The capital settlement that was conquered.</param>
        /// <param name="oldKingdom">The kingdom that lost the capital.</param>
        /// <param name="newKingdom">The kingdom that captured the capital.</param>
        /// <param name="capturerHero">The hero who captured the capital.</param>
        public static void LogCapitalConquest(Settlement capital, Kingdom oldKingdom, Kingdom newKingdom, Hero capturerHero)
        {
            string message = $"CAPITAL CONQUERED: {capital.Name} | " +
                           $"Old Owner: {oldKingdom?.Name?.ToString() ?? "None"} | " +
                           $"New Owner: {newKingdom?.Name?.ToString() ?? "None"} | " +
                           $"Capturer: {capturerHero?.Name?.ToString() ?? "Unknown"}";
            Log(message);
        }

        /// <summary>
        /// Logs capital status removal event.
        /// </summary>
        /// <param name="formerCapital">The settlement that is no longer a capital.</param>
        /// <param name="defeatedKingdom">The kingdom that was defeated.</param>
        public static void LogCapitalStatusRemoval(Settlement formerCapital, Kingdom defeatedKingdom)
        {
            string message = $"CAPITAL STATUS REMOVED: {formerCapital.Name} | " +
                           $"Defeated Kingdom: {defeatedKingdom?.Name?.ToString() ?? "Unknown"}";
            Log(message);
        }

        /// <summary>
        /// Logs garrison reinforcement event.
        /// </summary>
        /// <param name="capital">The capital settlement that received reinforcements.</param>
        /// <param name="troopCount">The number of troops added.</param>
        /// <param name="troopType">The type of troops added.</param>
        public static void LogGarrisonReinforcement(Settlement capital, int troopCount, string troopType)
        {
            string message = $"GARRISON REINFORCED: {capital.Name} | " +
                           $"Added: {troopCount}x {troopType}";
            Log(message);
        }

        /// <summary>
        /// Writes a message to the log file with timestamp.
        /// Thread-safe operation using lock.
        /// </summary>
        /// <param name="message">The message to write to the log file.</param>
        private static void LogToFile(string message)
        {
            try
            {
                lock (_lockObject)
                {
                    string timestamp = DateTime.Now.ToString(LogConstants.TimestampFormat);
                    string logEntry = $"[{timestamp}] {message}";
                    File.AppendAllText(LogFilePath, logEntry + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                // Fallback to in-game notification if file logging fails
                InformationManager.DisplayMessage(new InformationMessage(
                    $"{Messages.Errors.LoggingFailed}: {ex.Message}",
                    UIConstants.MessageColors.Error
                ));
            }
        }
    }
}
