namespace MGT2AssistantButton.Core
{
    /// <summary>
    /// Modes for Gameplay Features selection
    /// </summary>
    public enum GameplayFeatureMode
    {
        /// <summary>
        /// Select only features required by platforms (+ their dependencies)
        /// </summary>
        PlatformOnly,

        /// <summary>
        /// Select required + GOOD + Neutral features up to the limit (no BAD)
        /// </summary>
        Best,

        /// <summary>
        /// Select all features (GOOD + Neutral + BAD) up to the limit
        /// </summary>
        All
    }
}
