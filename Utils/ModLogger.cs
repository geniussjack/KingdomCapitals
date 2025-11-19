using System;
using System.IO;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace KingdomCapitals.Utils
{
    /// <summary>
    /// Provides logging functionality for the Kingdom Capitals mod.
    /// </summary>
    public static class ModLogger
    {
        private static readonly string LogFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Mount and Blade II Bannerlord",
            "logs",
            "KingdomCapitals.log"
        );

        private static readonly object _lockObject = new object();

        static ModLogger()
        {
            try
            {
                string logDirectory = Path.GetDirectoryName(LogFilePath);
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }
            }
            catch (Exception ex)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[Kingdom Capitals] Failed to create log directory: {ex.Message}",
                    Colors.Red
                ));
            }
        }

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        public static void Log(string message)
        {
            LogToFile($"[INFO] {message}");
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        public static void Warning(string message)
        {
            LogToFile($"[WARNING] {message}");
            InformationManager.DisplayMessage(new InformationMessage(
                $"[Kingdom Capitals] {message}",
                Colors.Yellow
            ));
        }

        /// <summary>
        /// Logs an error message with optional exception details.
        /// </summary>
        public static void Error(string message, Exception ex = null)
        {
            string logMessage = ex != null
                ? $"[ERROR] {message}\nException: {ex.GetType().Name}\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}"
                : $"[ERROR] {message}";

            LogToFile(logMessage);
            InformationManager.DisplayMessage(new InformationMessage(
                $"[Kingdom Capitals ERROR] {message}",
                Colors.Red
            ));
        }

        /// <summary>
        /// Logs capital conquest event details.
        /// </summary>
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
        public static void LogCapitalStatusRemoval(Settlement formerCapital, Kingdom defeatedKingdom)
        {
            string message = $"CAPITAL STATUS REMOVED: {formerCapital.Name} | " +
                           $"Defeated Kingdom: {defeatedKingdom?.Name?.ToString() ?? "Unknown"}";
            Log(message);
        }

        /// <summary>
        /// Logs garrison reinforcement event.
        /// </summary>
        public static void LogGarrisonReinforcement(Settlement capital, int troopCount, string troopType)
        {
            string message = $"GARRISON REINFORCED: {capital.Name} | " +
                           $"Added: {troopCount}x {troopType}";
            Log(message);
        }

        /// <summary>
        /// Writes a message to the log file with timestamp.
        /// </summary>
        private static void LogToFile(string message)
        {
            try
            {
                lock (_lockObject)
                {
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string logEntry = $"[{timestamp}] {message}";
                    File.AppendAllText(LogFilePath, logEntry + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                // Fallback to console if file logging fails
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[Kingdom Capitals] Logging failed: {ex.Message}",
                    Colors.Red
                ));
            }
        }
    }
}
