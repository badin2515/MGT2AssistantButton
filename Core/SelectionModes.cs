namespace MGT2AssistantButton.Core
{
    public enum EngineSelectionMode
    {
        OwnEngine,      // ใช้ engine ของตัวเองที่ดีที่สุด
        BestAvailable,  // ซื้อหรือใช้ engine ที่ดีที่สุด
        Cheapest        // ใช้ engine ที่ถูกที่สุด
    }

    public enum PlatformSelectionMode
    {
        BestMarketShare,  // เลือก platform ที่มี market share สูงสุด
        Latest,           // เลือก platform ล่าสุด
        BestForGenre      // เลือก platform ที่เหมาะกับ genre
    }
}
