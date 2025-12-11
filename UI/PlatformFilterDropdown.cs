using UnityEngine;
using UnityEngine.UI;
using System;
using MGT2AssistantButton.Core;

namespace MGT2AssistantButton.UI
{
    /// <summary>
    /// Multi-select dropdown สำหรับกรอง Platform
    /// ทำงานภายในขอบเขตที่เกมอนุญาต (unlock + DevKit)
    /// </summary>
    public class PlatformFilterDropdown : BaseMultiSelectDropdown
    {
        private static PlatformFilterConfig currentConfig = new PlatformFilterConfig();
        private Action<PlatformFilterConfig> onConfigApplied;
        
        protected override string PanelName => "PlatformFilterPanel";
        protected override Color AccentColor => new Color(0.2f, 0.6f, 1f, 1f); // Blue
        protected override bool DropdownOnLeft => false; // Platform button is on right side
        
        /// <summary>
        /// Initialize dropdown with callback
        /// </summary>
        public void Setup(Button button, Action<PlatformFilterConfig> callback)
        {
            onConfigApplied = callback;
            Initialize(button, null);
        }
        
        protected override void DefineFilterItems()
        {
            // ═══════════════════════════════════════
            // Platform Type
            // ═══════════════════════════════════════
            filterItems.Add(new FilterItem("▸ Type", "header_type", false, true));
            filterItems.Add(new FilterItem("PC", "type_pc", currentConfig.FilterComputer));
            filterItems.Add(new FilterItem("Console", "type_console", currentConfig.FilterConsole));
            filterItems.Add(new FilterItem("Handheld", "type_handheld", currentConfig.FilterHandheld));
            filterItems.Add(new FilterItem("Phone", "type_phone", currentConfig.FilterSmartphone));
            filterItems.Add(new FilterItem("Arcade", "type_arcade", currentConfig.FilterArcade));
            
            // ═══════════════════════════════════════
            // Tech Level (Grouped)
            // ═══════════════════════════════════════
            filterItems.Add(new FilterItem("▸ Tech", "header_tech", false, true));
            filterItems.Add(new FilterItem("Low (1-3)", "tech_low", currentConfig.TechLow));
            filterItems.Add(new FilterItem("Mid (4-6)", "tech_mid", currentConfig.TechMid));
            filterItems.Add(new FilterItem("High (7-9)", "tech_high", currentConfig.TechHigh));
            
            // ═══════════════════════════════════════
            // Additional Options
            // ═══════════════════════════════════════
            filterItems.Add(new FilterItem("▸ Options", "header_options", false, true));
            filterItems.Add(new FilterItem("Internet", "opt_internet", currentConfig.RequireInternet));
            filterItems.Add(new FilterItem("My Platform", "opt_own", currentConfig.OwnPlatformOnly));
            
            // ═══════════════════════════════════════
            // Sorting Preference
            // ═══════════════════════════════════════
            filterItems.Add(new FilterItem("▸ Sort By", "header_sort", false, true));
            filterItems.Add(new FilterItem("Market Share", "sort_market", currentConfig.PreferHighMarketShare));
            filterItems.Add(new FilterItem("Experience", "sort_exp", currentConfig.PreferHighExperience));
            filterItems.Add(new FilterItem("Tech Level", "sort_tech", currentConfig.PreferHighTech));
        }
        
        protected override void OnApplyFilters()
        {
            // Platform Types
            currentConfig.FilterComputer = GetFilterValue("type_pc");
            currentConfig.FilterConsole = GetFilterValue("type_console");
            currentConfig.FilterHandheld = GetFilterValue("type_handheld");
            currentConfig.FilterSmartphone = GetFilterValue("type_phone");
            currentConfig.FilterArcade = GetFilterValue("type_arcade");
            
            // Tech Levels (Grouped)
            currentConfig.TechLow = GetFilterValue("tech_low");
            currentConfig.TechMid = GetFilterValue("tech_mid");
            currentConfig.TechHigh = GetFilterValue("tech_high");
            
            // Additional Options
            currentConfig.RequireInternet = GetFilterValue("opt_internet");
            currentConfig.OwnPlatformOnly = GetFilterValue("opt_own");
            
            // Sorting Preferences
            currentConfig.PreferHighMarketShare = GetFilterValue("sort_market");
            currentConfig.PreferHighExperience = GetFilterValue("sort_exp");
            currentConfig.PreferHighTech = GetFilterValue("sort_tech");
            
            // Invoke callback with current config
            onConfigApplied?.Invoke(currentConfig);
        }
        
        /// <summary>
        /// Get current filter config (for debugging)
        /// </summary>
        public static PlatformFilterConfig GetCurrentConfig() => currentConfig;
    }
}
