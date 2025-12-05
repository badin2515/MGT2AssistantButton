using UnityEngine;
using UnityEngine.UI;
using MGT2AssistantButton.Core;

namespace MGT2AssistantButton.UI
{
    public class EngineFeatureDropdownMenu : BaseDropdownMenu<EngineFeatureMode>
    {
        protected override string PanelName => "EngineFeatureDropdownPanel";
        protected override Color HighlightColor => new Color(0.4f, 0.6f, 0.8f, 1f);
        protected override EngineFeatureMode DefaultMode => EngineFeatureMode.Best;

        public static EngineFeatureDropdownMenu Create(Button button, System.Action<EngineFeatureMode> callback)
        {
            var dropdown = button.gameObject.AddComponent<EngineFeatureDropdownMenu>();
            dropdown.Initialize(button, callback);
            return dropdown;
        }

        protected override void DefineMenuItems()
        {
            menuItems.Add(new MenuItemData("Best (Expensive)", EngineFeatureMode.Best));
            menuItems.Add(new MenuItemData("Cheapest", EngineFeatureMode.Cheapest));
        }

        protected override string GetModeLabel(EngineFeatureMode mode)
        {
            return mode switch
            {
                EngineFeatureMode.Best => "Best (Expensive)",
                EngineFeatureMode.Cheapest => "Cheapest",
                _ => mode.ToString()
            };
        }
    }
}
