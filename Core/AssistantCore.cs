using UnityEngine;
using UnityEngine.UI;

namespace MGT2AssistantButton.Core
{
    public static class AssistantCore
    {
        private static Menu_DevGame _currentMenu;

        public static void Initialize(Menu_DevGame menu) => _currentMenu = menu;

        public static bool IsButtonInteractable(int index)
        {
            if (_currentMenu?.uiObjects == null) return false;
            if (index < 0 || index >= _currentMenu.uiObjects.Length) return false;
            Button button = _currentMenu.uiObjects[index]?.GetComponent<Button>();
            return button != null && button.interactable;
        }

        public static Menu_DevGame GetMenu() => _currentMenu;

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
        public static void ApplyBestEngineFeatures() => Handlers.EngineHandler.ApplyBestEngineFeatures(_currentMenu);
        public static void ApplyEngineFeatureMode(EngineFeatureMode mode) => Handlers.EngineHandler.ApplyEngineFeatures(_currentMenu, mode);
        public static void ApplyBestLanguage() => Handlers.LanguageHandler.ApplyBestLanguage(_currentMenu);
        public static void ApplyLanguageMode(LanguageMode mode) => Handlers.LanguageHandler.ApplyLanguageMode(_currentMenu, mode);
        public static void ApplyOptimalSliders() => Handlers.SliderHandler.ApplyOptimalSliders(_currentMenu);
        public static void ApplyBestGameplayFeatures() => Handlers.GameplayFeatureHandler.ApplyGameplayFeatures(_currentMenu, GameplayFeatureMode.Best);
        public static void ApplyGameplayFeatureMode(GameplayFeatureMode mode) => Handlers.GameplayFeatureHandler.ApplyGameplayFeatures(_currentMenu, mode);
    }
}
