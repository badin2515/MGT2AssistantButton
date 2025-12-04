namespace MGT2AssistantButton.Core
{
    /// <summary>
    /// Platform selection modes
    /// </summary>
    public enum PlatformMode
    {
        ByMarket,           // 1. ตาม Market Share
        ConsoleOnly,        // 2. เน้น Console
        PCOnly,             // 3. เน้น PC
        OurConsoleFirst,    // 4. เน้น Console เราก่อน (Manufacturer Exclusive)
        HighestTechOnly     // 5. เน้น Tech สูงสุด (ห้ามมีรอง)
    }
}
