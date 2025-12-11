using UnityEngine;
using UnityEngine.UI;
using System;
using MGT2AssistantButton.Core;

namespace MGT2AssistantButton.UI
{
    /// <summary>
    /// Radio-select dropdown สำหรับ Engine Feature
    /// Auto-match platform + Cost Strategy (เลือกได้อันเดียว)
    /// </summary>
    public class EngineFeatureFilterDropdown : BaseMultiSelectDropdown
    {
        private static EngineFeatureFilterConfig currentConfig = new EngineFeatureFilterConfig();
        private Action<EngineFeatureFilterConfig> onConfigApplied;
        
        protected override string PanelName => "EngineFeatureFilterPanel";
        protected override Color AccentColor => new Color(0.4f, 0.6f, 0.8f, 1f); // Light Blue
        protected override bool DropdownOnLeft => false;
        
        /// <summary>
        /// Initialize dropdown with callback
        /// </summary>
        public void Setup(Button button, Action<EngineFeatureFilterConfig> callback)
        {
            onConfigApplied = callback;
            Initialize(button, null);
        }
        
        protected override void DefineFilterItems()
        {
            // Cost Strategy - Radio style (only one can be selected)
            filterItems.Add(new FilterItem("▸ Cost Strategy", "header_cost", false, true));
            filterItems.Add(new FilterItem("● Best Quality", "cost_best", currentConfig.UseBestQuality));
            filterItems.Add(new FilterItem("○ Cheapest", "cost_cheap", currentConfig.UseCheapest));
        }
        
        protected override void OnFilterItemClicked(string key)
        {
            // Radio button behavior for cost items
            if (key == "cost_best")
            {
                SetFilterValue("cost_best", true);
                SetFilterValue("cost_cheap", false);
            }
            else if (key == "cost_cheap")
            {
                SetFilterValue("cost_best", false);
                SetFilterValue("cost_cheap", true);
            }
        }
        
        protected override void OnApplyFilters()
        {
            bool wantBest = GetFilterValue("cost_best");
            
            currentConfig.UseBestQuality = wantBest;
            currentConfig.UseCheapest = !wantBest;
            
            onConfigApplied?.Invoke(currentConfig);
        }
        
        public static EngineFeatureFilterConfig GetCurrentConfig() => currentConfig;
    }
}

