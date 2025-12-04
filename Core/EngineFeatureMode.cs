namespace MGT2AssistantButton.Core
{
    /// <summary>
    /// Available engine feature selection modes
    /// </summary>
    public enum EngineFeatureMode
    {
        /// <summary>
        /// Select best features for each type (highest tech level within platform limit)
        /// Same as game's Auto button
        /// </summary>
        Best,

        /// <summary>
        /// Select cheapest features for each type (lowest dev cost within platform limit)
        /// Saves development cost while still being compatible with platforms
        /// </summary>
        Cheapest
    }
}
