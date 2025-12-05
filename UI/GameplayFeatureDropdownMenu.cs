using UnityEngine;
using UnityEngine.UI;
using MGT2AssistantButton.Core;

namespace MGT2AssistantButton.UI
{
    public class GameplayFeatureDropdownMenu : BaseDropdownMenu<GameplayFeatureMode>
    {
        protected override string PanelName => "GameplayFeatureDropdownPanel";
        protected override Color HighlightColor => new Color(0.7f, 0.5f, 0.3f, 1f);
        protected override GameplayFeatureMode DefaultMode => GameplayFeatureMode.PlatformOnly;

        public static GameplayFeatureDropdownMenu Create(Button button, System.Action<GameplayFeatureMode> callback)
        {
            var dropdown = button.gameObject.AddComponent<GameplayFeatureDropdownMenu>();
            dropdown.Initialize(button, callback);
            return dropdown;
        }

        protected override void DefineMenuItems()
        {
            menuItems.Add(new MenuItemData("Platform Only", GameplayFeatureMode.PlatformOnly));
            menuItems.Add(new MenuItemData("Best (GOOD + Neutral)", GameplayFeatureMode.Best));
            menuItems.Add(new MenuItemData("All Features", GameplayFeatureMode.All));
        }

        protected override string GetModeLabel(GameplayFeatureMode mode)
        {
            return mode switch
            {
                GameplayFeatureMode.PlatformOnly => "Platform Only",
                GameplayFeatureMode.Best => "Best (GOOD + Neutral)",
                GameplayFeatureMode.All => "All Features",
                _ => mode.ToString()
            };
        }
    }
}
