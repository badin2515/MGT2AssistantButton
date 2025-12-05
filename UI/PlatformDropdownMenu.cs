using UnityEngine;
using UnityEngine.UI;
using MGT2AssistantButton.Core;

namespace MGT2AssistantButton.UI
{
    public class PlatformDropdownMenu : BaseDropdownMenu<PlatformMode>
    {
        protected override string PanelName => "PlatformDropdownPanel";
        protected override Color HighlightColor => new Color(0.6f, 0.4f, 0.7f, 1f);
        protected override PlatformMode DefaultMode => PlatformMode.ByMarket;

        public static PlatformDropdownMenu Create(Button button, System.Action<PlatformMode> callback)
        {
            var dropdown = button.gameObject.AddComponent<PlatformDropdownMenu>();
            dropdown.Initialize(button, callback);
            return dropdown;
        }

        protected override void DefineMenuItems()
        {
            menuItems.Add(new MenuItemData("By Market Share", PlatformMode.ByMarket));
            menuItems.Add(new MenuItemData("Console Only", PlatformMode.ConsoleOnly));
            menuItems.Add(new MenuItemData("PC Only", PlatformMode.PCOnly));
            menuItems.Add(new MenuItemData("Our Console First", PlatformMode.OurConsoleFirst));
            menuItems.Add(new MenuItemData("Highest Tech Only", PlatformMode.HighestTechOnly));
        }

        protected override string GetModeLabel(PlatformMode mode)
        {
            return mode switch
            {
                PlatformMode.ByMarket => "By Market Share",
                PlatformMode.ConsoleOnly => "Console Only",
                PlatformMode.PCOnly => "PC Only",
                PlatformMode.OurConsoleFirst => "Our Console First",
                PlatformMode.HighestTechOnly => "Highest Tech Only",
                _ => mode.ToString()
            };
        }
    }
}
