namespace MGT2AssistantButton.Helpers
{
    /// <summary>
    /// Debug logging helper - only logs when debug mode is enabled
    /// </summary>
    public static class DebugLog
    {
        /// <summary>
        /// Enable/disable debug logging (set via config or code)
        /// </summary>
        public static bool Enabled = false;

        /// <summary>
        /// Log debug info (only when enabled)
        /// </summary>
        public static void Info(string message)
        {
            if (Enabled)
                Plugin.Logger.LogInfo(message);
        }

        /// <summary>
        /// Log warning (always logs)
        /// </summary>
        public static void Warning(string message)
        {
            Plugin.Logger.LogWarning(message);
        }

        /// <summary>
        /// Log error (always logs)
        /// </summary>
        public static void Error(string message)
        {
            Plugin.Logger.LogError(message);
        }
    }
}
