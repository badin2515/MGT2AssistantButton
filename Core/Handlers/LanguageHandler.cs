using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace MGT2AssistantButton.Core.Handlers
{
    public static class LanguageHandler
    {
        private static readonly string[] LanguageNames = {
            "English", "German", "French", "Italian", "Spanish",
            "Polish", "Swedish", "Russian", "Chinese", "Korean", "Japanese"
        };

        private static readonly float[] LanguageMarketShare = {
            30f, 8f, 6f, 4f, 7f, 3f, 2f, 5f, 18f, 5f, 15f
        };

        public static void ApplyLanguageMode(Menu_DevGame menu, LanguageMode mode)
        {
            switch (mode)
            {
                case LanguageMode.AllLanguages: ApplyAllLanguages(menu); break;
                case LanguageMode.MotherTongueOnly: ApplyMotherTongueOnly(menu); break;
                case LanguageMode.TopMarketLanguages: ApplyTopMarketLanguages(menu); break;
                case LanguageMode.WesternLanguages: ApplyWesternLanguages(menu); break;
                case LanguageMode.AsianLanguages: ApplyAsianLanguages(menu); break;
            }
        }

        public static void ApplyBestLanguage(Menu_DevGame menu) => ApplyAllLanguages(menu);

        private static mainScript GetMainScript()
        {
            GameObject mainObj = GameObject.Find("Main");
            return mainObj?.GetComponent<mainScript>();
        }

        private static bool IsMotherTongue(int languageIndex)
        {
            mainScript mS = GetMainScript();
            return mS != null && mS.Muttersprache(languageIndex);
        }

        private static bool IsLanguageButtonInteractable(Menu_DevGame menu, int languageIndex)
        {
            if (menu?.uiObjects == null) return false;
            int buttonIndex = 107 + languageIndex;
            if (buttonIndex < 0 || buttonIndex >= menu.uiObjects.Length) return false;
            var button = menu.uiObjects[buttonIndex]?.GetComponent<Button>();
            return button != null && button.interactable;
        }

        private static void SetLanguage(Menu_DevGame menu, int languageIndex, bool enable)
        {
            if (menu?.g_GameLanguage == null) return;
            if (languageIndex < 0 || languageIndex >= menu.g_GameLanguage.Length) return;
            if (!IsLanguageButtonInteractable(menu, languageIndex)) return;
            if (menu.g_GameLanguage[languageIndex] != enable)
                menu.SetLanguage(languageIndex);
        }

        private static void ApplyAllLanguages(Menu_DevGame menu)
        {
            if (menu?.g_GameLanguage == null) return;
            for (int i = 0; i < menu.g_GameLanguage.Length; i++)
                SetLanguage(menu, i, true);
        }

        private static void ApplyMotherTongueOnly(Menu_DevGame menu)
        {
            if (menu?.g_GameLanguage == null) return;

            bool hasMotherTongue = false;
            for (int i = 0; i < menu.g_GameLanguage.Length; i++)
            {
                if (IsMotherTongue(i)) { hasMotherTongue = true; break; }
            }

            for (int i = 0; i < menu.g_GameLanguage.Length; i++)
            {
                bool shouldEnable = IsMotherTongue(i) || (!hasMotherTongue && i == 0);
                SetLanguage(menu, i, shouldEnable);
            }
        }

        private static void ApplyTopMarketLanguages(Menu_DevGame menu)
        {
            if (menu?.g_GameLanguage == null) return;

            var top5Indices = Enumerable.Range(0, LanguageMarketShare.Length)
                .OrderByDescending(i => LanguageMarketShare[i])
                .Take(5)
                .ToList();
            
            HashSet<int> topLanguages = new HashSet<int>(top5Indices);
            for (int i = 0; i < menu.g_GameLanguage.Length; i++)
            {
                if (IsMotherTongue(i)) topLanguages.Add(i);
            }

            for (int i = 0; i < menu.g_GameLanguage.Length; i++)
                SetLanguage(menu, i, topLanguages.Contains(i));
        }

        private static void ApplyWesternLanguages(Menu_DevGame menu)
        {
            if (menu?.g_GameLanguage == null) return;

            HashSet<int> westernLanguages = new HashSet<int> { 0, 1, 2, 3, 4, 5, 6, 7 };
            for (int i = 0; i < menu.g_GameLanguage.Length; i++)
            {
                if (IsMotherTongue(i)) westernLanguages.Add(i);
            }

            for (int i = 0; i < menu.g_GameLanguage.Length; i++)
                SetLanguage(menu, i, westernLanguages.Contains(i));
        }

        private static void ApplyAsianLanguages(Menu_DevGame menu)
        {
            if (menu?.g_GameLanguage == null) return;

            HashSet<int> asianLanguages = new HashSet<int> { 0, 8, 9, 10 };
            for (int i = 0; i < menu.g_GameLanguage.Length; i++)
            {
                if (IsMotherTongue(i)) asianLanguages.Add(i);
            }

            for (int i = 0; i < menu.g_GameLanguage.Length; i++)
                SetLanguage(menu, i, asianLanguages.Contains(i));
        }

        public static int GetEnabledLanguageCount(Menu_DevGame menu)
        {
            return menu?.g_GameLanguage?.Count(l => l) ?? 0;
        }

        public static float GetTotalMarketCoverage(Menu_DevGame menu)
        {
            if (menu?.g_GameLanguage == null) return 0f;
            float total = 0f;
            for (int i = 0; i < menu.g_GameLanguage.Length && i < LanguageMarketShare.Length; i++)
            {
                if (menu.g_GameLanguage[i]) total += LanguageMarketShare[i];
            }
            return Mathf.Min(total, 100f);
        }
    }
}
