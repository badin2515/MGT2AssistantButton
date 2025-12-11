using UnityEngine;
using UnityEngine.UI;
using System;
using MGT2AssistantButton.Core;

namespace MGT2AssistantButton.UI
{
    /// <summary>
    /// Multi-select dropdown สำหรับกรอง Engine
    /// ใช้ Weighted Score System
    /// </summary>
    public class EngineFilterDropdown : BaseMultiSelectDropdown
    {
        private static EngineFilterConfig currentConfig = new EngineFilterConfig();
        private Action<EngineFilterConfig> onConfigApplied;
        
        protected override string PanelName => "EngineFilterPanel";
        protected override Color AccentColor => new Color(0.6f, 0.4f, 0.8f, 1f); // Purple
        protected override bool DropdownOnLeft => false;
        
        /// <summary>
        /// Initialize dropdown with callback
        /// </summary>
        public void Setup(Button button, Action<EngineFilterConfig> callback)
        {
            onConfigApplied = callback;
            Initialize(button, null);
        }
        
        protected override void DefineFilterItems()
        {
            // Priority (ให้คะแนน)
            filterItems.Add(new FilterItem("▸ Priority", "header_priority", false, true));
            filterItems.Add(new FilterItem("Genre Match (+50)", "priority_genre", currentConfig.PriorityGenreMatch));
            filterItems.Add(new FilterItem("Own Engine (+30)", "priority_own", currentConfig.PriorityOwnEngine));
            filterItems.Add(new FilterItem("High Tech (+10/lv)", "priority_tech", currentConfig.PriorityHighTech));
            
            // Filters
            filterItems.Add(new FilterItem("▸ Filter", "header_filter", false, true));
            filterItems.Add(new FilterItem("Own Only", "filter_own", currentConfig.OwnOnly));
            filterItems.Add(new FilterItem("No Royalty", "filter_royalty", currentConfig.NoRoyalty));
        }
        
        protected override void OnApplyFilters()
        {
            // Priority
            currentConfig.PriorityGenreMatch = GetFilterValue("priority_genre");
            currentConfig.PriorityOwnEngine = GetFilterValue("priority_own");
            currentConfig.PriorityHighTech = GetFilterValue("priority_tech");
            
            // Filters
            currentConfig.OwnOnly = GetFilterValue("filter_own");
            currentConfig.NoRoyalty = GetFilterValue("filter_royalty");
            
            // Invoke callback with current config
            onConfigApplied?.Invoke(currentConfig);
        }
        
        public static EngineFilterConfig GetCurrentConfig() => currentConfig;
    }
}
