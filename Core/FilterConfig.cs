namespace MGT2AssistantButton.Core
{
    /// <summary>
    /// Filter settings for Platform selection
    /// ระบบนี้จะทำงานภายในขอบเขตที่เกมอนุญาตเท่านั้น (unlock + มี DevKit)
    /// </summary>
    public class PlatformFilterConfig
    {
        // Platform Type filters (typ 0-4)
        public bool FilterComputer { get; set; } = false;     // typ 0 - PC
        public bool FilterConsole { get; set; } = true;       // typ 1 - Stationary Console
        public bool FilterHandheld { get; set; } = false;     // typ 2
        public bool FilterSmartphone { get; set; } = false;   // typ 3
        public bool FilterArcade { get; set; } = false;       // typ 4
        
        // Tech Level filters - จัดกลุ่มเพื่อให้อ่านง่าย
        public bool TechLow { get; set; } = false;    // Tech 1-3 (Retro/Early)
        public bool TechMid { get; set; } = false;    // Tech 4-6 (Standard)
        public bool TechHigh { get; set; } = false;   // Tech 7-9 (Modern)
        
        // Additional filters
        public bool RequireInternet { get; set; } = false;    // เฉพาะ platform ที่รองรับ internet
        public bool OwnPlatformOnly { get; set; } = false;    // เฉพาะ platform ที่เราเป็นเจ้าของ
        
        // Sorting preference
        public bool PreferHighMarketShare { get; set; } = true;
        public bool PreferHighExperience { get; set; } = false;
        public bool PreferHighTech { get; set; } = false;
        
        /// <summary>
        /// ตรวจสอบว่า type ใดถูกเลือก (ถ้าไม่มีเลือกเลย = เลือกทั้งหมด)
        /// </summary>
        public bool HasAnyTypeFilter()
        {
            return FilterComputer || FilterConsole || FilterHandheld || FilterSmartphone || FilterArcade;
        }
        
        /// <summary>
        /// ตรวจสอบว่า tech level ใดถูกเลือก (ถ้าไม่มีเลือกเลย = เลือกทั้งหมด)
        /// </summary>
        public bool HasAnyTechLevelFilter()
        {
            return TechLow || TechMid || TechHigh;
        }
        
        /// <summary>
        /// ตรวจสอบว่า platform type ตรงกับ filter หรือไม่
        /// </summary>
        public bool MatchesType(int typ)
        {
            // ถ้าไม่มี filter ใดถูกเลือก = อนุญาตทั้งหมด
            if (!HasAnyTypeFilter()) return true;
            
            switch (typ)
            {
                case 0: return FilterComputer;
                case 1: return FilterConsole;
                case 2: return FilterHandheld;
                case 3: return FilterSmartphone;
                case 4: return FilterArcade;
                default: return false;
            }
        }
        
        /// <summary>
        /// ตรวจสอบว่า tech level ตรงกับ filter หรือไม่ (แบ่งเป็นกลุ่ม)
        /// </summary>
        public bool MatchesTechLevel(int tech)
        {
            // ถ้าไม่มี filter ใดถูกเลือก = อนุญาตทั้งหมด
            if (!HasAnyTechLevelFilter()) return true;
            
            if (tech >= 1 && tech <= 3 && TechLow) return true;
            if (tech >= 4 && tech <= 6 && TechMid) return true;
            if (tech >= 7 && tech <= 9 && TechHigh) return true;
            
            return false;
        }
    }
    
    /// <summary>
    /// Filter settings for Genre selection
    /// </summary>
    public class GenreFilterConfig
    {
        public bool PreferHighLevel { get; set; } = true;
        public bool PreferPopular { get; set; } = false;
    }
    
    /// <summary>
    /// Filter settings for Engine Feature selection
    /// </summary>
    public class EngineFeatureFilterConfig
    {
        public bool PreferHighTech { get; set; } = true;
        public bool PreferLowCost { get; set; } = false;
        public bool PreferHighPoints { get; set; } = false;
    }
}
