namespace KingdomCapitals.Constants
{
    /// <summary>
    /// Constants for logging system including log levels and formats.
    /// </summary>
    public static class LogConstants
    {
        /// <summary>
        /// Log level prefixes.
        /// </summary>
        public static class LogLevel
        {
            public const string Info = "[INFO]";
            public const string Warning = "[WARNING]";
            public const string Error = "[ERROR]";
            public const string Debug = "[DEBUG]";
        }

        /// <summary>
        /// Timestamp format for log entries.
        /// Format: yyyy-MM-dd HH:mm:ss
        /// Example: 2024-01-15 14:30:45
        /// </summary>
        public const string TimestampFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// Log file subdirectory path relative to Bannerlord data folder.
        /// </summary>
        public const string LogSubdirectory = "Mount and Blade II Bannerlord";

        /// <summary>
        /// Logs folder name within the subdirectory.
        /// </summary>
        public const string LogsFolderName = "logs";
    }
}
