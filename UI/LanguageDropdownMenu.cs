using UnityEngine;
using UnityEngine.UI;
using MGT2AssistantButton.Core;

namespace MGT2AssistantButton.UI
{
    public class LanguageDropdownMenu : BaseDropdownMenu<LanguageMode>
    {
        protected override string PanelName => "LanguageDropdownPanel";
        protected override Color HighlightColor => new Color(0.3f, 0.7f, 0.5f, 1f);
        protected override LanguageMode DefaultMode => LanguageMode.AllLanguages;

        public static LanguageDropdownMenu Create(Button button, System.Action<LanguageMode> callback)
        {
            var dropdown = button.gameObject.AddComponent<LanguageDropdownMenu>();
            dropdown.Initialize(button, callback);
            return dropdown;
        }

        protected override void DefineMenuItems()
        {
            menuItems.Add(new MenuItemData("All Languages", LanguageMode.AllLanguages));
            menuItems.Add(new MenuItemData("Mother Tongue (Free)", LanguageMode.MotherTongueOnly));
            menuItems.Add(new MenuItemData("Top 5 Market", LanguageMode.TopMarketLanguages));
            menuItems.Add(new MenuItemData("Western", LanguageMode.WesternLanguages));
            menuItems.Add(new MenuItemData("Asian", LanguageMode.AsianLanguages));
        }

        protected override string GetModeLabel(LanguageMode mode)
        {
            return mode switch
            {
                LanguageMode.AllLanguages => "All Languages",
                LanguageMode.MotherTongueOnly => "Mother Tongue (Free)",
                LanguageMode.TopMarketLanguages => "Top 5 Market",
                LanguageMode.WesternLanguages => "Western",
                LanguageMode.AsianLanguages => "Asian",
                _ => mode.ToString()
            };
        }
    }
}
