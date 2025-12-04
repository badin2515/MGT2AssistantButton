using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace MGT2AssistantButton.Core.Handlers
{
    /// <summary>
    /// Handles language selection for game development
    /// Languages from UI Dump (uiObjects[107+i]):
    /// 0=US(English), 1=German, 2=French, 3=Italian, 4=Spanish,
    /// 5=Polish, 6=Swedish, 7=Russian, 8=Chinese, 9=Korean, 10=Japanese
    /// </summary>
    public static class LanguageHandler
    {
        // Language names for logging (matching game's button order)
        private static readonly string[] LanguageNames = new string[]
        {
            "English",  // 0 - Button_US
            "German",   // 1 - Button_GER
            "French",   // 2 - Button_FRANCE
            "Italian",  // 3 - Button_ITALIA
            "Spanish",  // 4 - Button_SPAIN
            "Polish",   // 5 - Button_POLAND
            "Swedish",  // 6 - Button_SWEDEN
            "Russian",  // 7 - Button_RUSSIA
            "Chinese",  // 8 - Button_CHINA
            "Korean",   // 9 - Button_KOREA
            "Japanese"  // 10 - Button_JAPAN
        };

        // Estimated market share percentages for each language (based on gaming market)
        // These are approximate values representing global gaming market share
        private static readonly float[] LanguageMarketShare = new float[]
        {
            30f,   // 0 - English (USA, UK, Australia, etc.)
            8f,    // 1 - German
            6f,    // 2 - French
            4f,    // 3 - Italian
            7f,    // 4 - Spanish
            3f,    // 5 - Polish
            2f,    // 6 - Swedish
            5f,    // 7 - Russian
            18f,   // 8 - Chinese
            5f,    // 9 - Korean
            15f    // 10 - Japanese
        };

        /// <summary>
        /// Apply language selection based on mode
        /// </summary>
        public static void ApplyLanguageMode(Menu_DevGame menu, LanguageMode mode)
        {
            switch (mode)
            {
                case LanguageMode.AllLanguages:
                    ApplyAllLanguages(menu);
                    break;
                case LanguageMode.MotherTongueOnly:
                    ApplyMotherTongueOnly(menu);
                    break;
                case LanguageMode.TopMarketLanguages:
                    ApplyTopMarketLanguages(menu);
                    break;
                case LanguageMode.WesternLanguages:
                    ApplyWesternLanguages(menu);
                    break;
                case LanguageMode.AsianLanguages:
                    ApplyAsianLanguages(menu);
                    break;
            }
        }

        /// <summary>
        /// Apply best language (All Languages)
        /// </summary>
        public static void ApplyBestLanguage(Menu_DevGame menu)
        {
            ApplyAllLanguages(menu);
        }

        /// <summary>
        /// Get mainScript reference
        /// </summary>
        private static mainScript GetMainScript()
        {
            GameObject mainObj = GameObject.Find("Main");
            return mainObj?.GetComponent<mainScript>();
        }

        /// <summary>
        /// Check if a language is the player's mother tongue (free to localize)
        /// </summary>
        private static bool IsMotherTongue(int languageIndex)
        {
            mainScript mS = GetMainScript();
            if (mS == null) return false;
            return mS.Muttersprache(languageIndex);
        }

        /// <summary>
        /// Check if language button is interactable (not locked)
        /// Language buttons are uiObjects[107 + languageIndex]
        /// </summary>
        private static bool IsLanguageButtonInteractable(Menu_DevGame menu, int languageIndex)
        {
            if (menu == null || menu.uiObjects == null) return false;
            
            int buttonIndex = 107 + languageIndex;
            if (buttonIndex < 0 || buttonIndex >= menu.uiObjects.Length) return false;
            
            var buttonObj = menu.uiObjects[buttonIndex];
            if (buttonObj == null) return false;
            
            var button = buttonObj.GetComponent<Button>();
            return button != null && button.interactable;
        }

        /// <summary>
        /// Set a specific language on or off
        /// </summary>
        private static void SetLanguage(Menu_DevGame menu, int languageIndex, bool enable)
        {
            if (menu == null || menu.g_GameLanguage == null) return;
            if (languageIndex < 0 || languageIndex >= menu.g_GameLanguage.Length) return;

            // Check if button is interactable (not locked)
            if (!IsLanguageButtonInteractable(menu, languageIndex))
            {
                Plugin.Logger.LogInfo($"  Skipping {LanguageNames[languageIndex]}: Button is locked/disabled");
                return;
            }

            // Only toggle if current state is different
            if (menu.g_GameLanguage[languageIndex] != enable)
            {
                menu.SetLanguage(languageIndex);
            }
        }

        /// <summary>
        /// Enable all languages
        /// </summary>
        private static void ApplyAllLanguages(Menu_DevGame menu)
        {
            Plugin.Logger.LogInfo("=== All Languages: Enabling All Languages ===");

            if (menu == null || menu.g_GameLanguage == null) return;

            for (int i = 0; i < menu.g_GameLanguage.Length; i++)
            {
                SetLanguage(menu, i, true);
                Plugin.Logger.LogInfo($"  Enabled: {LanguageNames[i]}");
            }

            Plugin.Logger.LogInfo($"Total: {menu.g_GameLanguage.Length} languages enabled");
        }

        /// <summary>
        /// Only enable mother tongue (free localization)
        /// If no mother tongue is defined, English is used as default (free)
        /// </summary>
        private static void ApplyMotherTongueOnly(Menu_DevGame menu)
        {
            Plugin.Logger.LogInfo("=== Mother Tongue Only: Free Languages Only ===");

            if (menu == null || menu.g_GameLanguage == null) return;

            // First, find if there's any mother tongue
            bool hasMotherTongue = false;
            for (int i = 0; i < menu.g_GameLanguage.Length; i++)
            {
                if (IsMotherTongue(i))
                {
                    hasMotherTongue = true;
                    break;
                }
            }

            int enabledCount = 0;
            for (int i = 0; i < menu.g_GameLanguage.Length; i++)
            {
                bool shouldEnable = IsMotherTongue(i);
                
                // If no mother tongue defined (e.g., Thailand), use English as default
                if (!hasMotherTongue && i == 0)
                {
                    shouldEnable = true;
                    Plugin.Logger.LogInfo("  No mother tongue defined, defaulting to English");
                }
                
                SetLanguage(menu, i, shouldEnable);

                if (shouldEnable)
                {
                    Plugin.Logger.LogInfo($"  Enabled (Free): {LanguageNames[i]}");
                    enabledCount++;
                }
            }

            Plugin.Logger.LogInfo($"Total: {enabledCount} free languages enabled");
        }

        /// <summary>
        /// Enable top 5 market languages (English, Chinese, Japanese, German, Spanish)
        /// </summary>
        private static void ApplyTopMarketLanguages(Menu_DevGame menu)
        {
            Plugin.Logger.LogInfo("=== Top Market Languages: Top 5 by Market Share ===");

            if (menu == null || menu.g_GameLanguage == null) return;

            // Get top 5 languages by market share
            var top5Indices = Enumerable.Range(0, LanguageMarketShare.Length)
                .OrderByDescending(i => LanguageMarketShare[i])
                .Take(5)
                .ToList();
            
            HashSet<int> topLanguages = new HashSet<int>(top5Indices);

            // Also always include mother tongue (free)
            for (int i = 0; i < menu.g_GameLanguage.Length; i++)
            {
                if (IsMotherTongue(i))
                    topLanguages.Add(i);
            }

            for (int i = 0; i < menu.g_GameLanguage.Length; i++)
            {
                bool shouldEnable = topLanguages.Contains(i);
                SetLanguage(menu, i, shouldEnable);

                if (shouldEnable)
                {
                    string freeTag = IsMotherTongue(i) ? " [FREE]" : "";
                    Plugin.Logger.LogInfo($"  Enabled: {LanguageNames[i]} ({LanguageMarketShare[i]}%){freeTag}");
                }
            }

            Plugin.Logger.LogInfo($"Total: {topLanguages.Count} languages enabled");
        }

        /// <summary>
        /// Enable Western languages (English, German, French, Italian, Spanish, Polish, Swedish, Russian)
        /// </summary>
        private static void ApplyWesternLanguages(Menu_DevGame menu)
        {
            Plugin.Logger.LogInfo("=== Western Languages: Europe & Americas ===");

            if (menu == null || menu.g_GameLanguage == null) return;

            // Western language indices based on UI dump:
            // English(0), German(1), French(2), Italian(3), Spanish(4), Polish(5), Swedish(6), Russian(7)
            HashSet<int> westernLanguages = new HashSet<int> { 0, 1, 2, 3, 4, 5, 6, 7 };

            // Also always include mother tongue (free)
            for (int i = 0; i < menu.g_GameLanguage.Length; i++)
            {
                if (IsMotherTongue(i))
                    westernLanguages.Add(i);
            }

            for (int i = 0; i < menu.g_GameLanguage.Length; i++)
            {
                bool shouldEnable = westernLanguages.Contains(i);
                SetLanguage(menu, i, shouldEnable);

                if (shouldEnable)
                {
                    string freeTag = IsMotherTongue(i) ? " [FREE]" : "";
                    Plugin.Logger.LogInfo($"  Enabled: {LanguageNames[i]}{freeTag}");
                }
            }

            Plugin.Logger.LogInfo($"Total: {westernLanguages.Count} languages enabled");
        }

        /// <summary>
        /// Enable Asian languages (Chinese, Korean, Japanese) + English
        /// </summary>
        private static void ApplyAsianLanguages(Menu_DevGame menu)
        {
            Plugin.Logger.LogInfo("=== Asian Languages: Asia Focus ===");

            if (menu == null || menu.g_GameLanguage == null) return;

            // Asian language indices based on UI dump:
            // English(0), Chinese(8), Korean(9), Japanese(10)
            HashSet<int> asianLanguages = new HashSet<int> { 0, 8, 9, 10 };

            // Also always include mother tongue (free)
            for (int i = 0; i < menu.g_GameLanguage.Length; i++)
            {
                if (IsMotherTongue(i))
                    asianLanguages.Add(i);
            }

            for (int i = 0; i < menu.g_GameLanguage.Length; i++)
            {
                bool shouldEnable = asianLanguages.Contains(i);
                SetLanguage(menu, i, shouldEnable);

                if (shouldEnable)
                {
                    string freeTag = IsMotherTongue(i) ? " [FREE]" : "";
                    Plugin.Logger.LogInfo($"  Enabled: {LanguageNames[i]}{freeTag}");
                }
            }

            Plugin.Logger.LogInfo($"Total: {asianLanguages.Count} languages enabled");
        }

        /// <summary>
        /// Get the count of currently enabled languages
        /// </summary>
        public static int GetEnabledLanguageCount(Menu_DevGame menu)
        {
            if (menu == null || menu.g_GameLanguage == null) return 0;

            return menu.g_GameLanguage.Count(l => l);
        }

        /// <summary>
        /// Get estimated total market coverage percentage
        /// </summary>
        public static float GetTotalMarketCoverage(Menu_DevGame menu)
        {
            if (menu == null || menu.g_GameLanguage == null) return 0f;

            float total = 0f;
            for (int i = 0; i < menu.g_GameLanguage.Length && i < LanguageMarketShare.Length; i++)
            {
                if (menu.g_GameLanguage[i])
                    total += LanguageMarketShare[i];
            }
            return Mathf.Min(total, 100f);
        }
    }
}
