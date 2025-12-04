using UnityEngine;
using UnityEngine.UI;

namespace MGT2AssistantButton.Core
{
    /// <summary>
    /// Core logic for the Assistant Mod.
    /// Delegates functionality to specialized handlers.
    /// </summary>
    public static class AssistantCore
    {
        // Reference to the current menu instance
        private static Menu_DevGame _currentMenu;

        public static void Initialize(Menu_DevGame menu)
        {
            _currentMenu = menu;
            Plugin.Logger.LogInfo("AssistantCore initialized with menu instance.");
        }

        /// <summary>
        /// Check if a specific UI button is interactable (not locked)
        /// </summary>
        public static bool IsButtonInteractable(int index)
        {
            if (_currentMenu == null || _currentMenu.uiObjects == null)
                return false;

            if (index < 0 || index >= _currentMenu.uiObjects.Length)
                return false;

            Button button = _currentMenu.uiObjects[index]?.GetComponent<Button>();
            return button != null && button.interactable;
        }

        /// <summary>
        /// Get the current menu instance
        /// </summary>
        public static Menu_DevGame GetMenu()
        {
            return _currentMenu;
        }

        // Delegate to handlers
        public static void ApplyBestTargetGroup() => Handlers.ContentHandler.ApplyBestTargetGroup(_currentMenu, true);
        public static void ApplyBestMainGenre() => Handlers.ContentHandler.ApplyBestMainGenre(_currentMenu, true);
        public static void ApplyBestSubGenre() => Handlers.ContentHandler.ApplyBestSubGenre(_currentMenu, true);
        public static void ApplyBestMainTheme() => Handlers.ContentHandler.ApplyBestMainTheme(_currentMenu, true);
        public static void ApplyBestSubTheme() => Handlers.ContentHandler.ApplyBestSubTheme(_currentMenu, true);
        public static void ApplyRealName() => Handlers.NamingHandler.ApplyRealName(_currentMenu);
        public static void ApplyBestEngine() => Handlers.EngineHandler.ApplyBestEngine(_currentMenu);
        public static void ApplyBestPlatform() => Handlers.PlatformHandler.ApplyBestPlatform(_currentMenu);
        public static void ApplyAntiCheat() => Handlers.SecurityHandler.ApplyAntiCheat(_currentMenu);
        public static void ApplyCopyProtect() => Handlers.SecurityHandler.ApplyCopyProtect(_currentMenu);

        // Placeholders for future implementation
        public static void ApplyBestEngineFeatures()
        {
            Plugin.Logger.LogInfo("Core: Applying Best Engine Features...");
            Handlers.EngineHandler.ApplyBestEngineFeatures(_currentMenu);
        }

        public static void ApplyBestLanguage()
        {
            Plugin.Logger.LogInfo("Core: Applying Best Language...");
            // TODO: Implement language selection
        }

        public static void ApplyOptimalSliders()
        {
            Plugin.Logger.LogInfo("Core: Applying Optimal Sliders...");
            // TODO: Implement slider optimization
        }

        public static void ApplyBestGameplayFeatures()
        {
            Plugin.Logger.LogInfo("Core: Applying Best Gameplay Features...");
            // TODO: Implement gameplay features selection
        }
    }
}
