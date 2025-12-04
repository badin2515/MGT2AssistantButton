using UnityEngine;
using UnityEngine.UI;

namespace MGT2AssistantButton.Core.Handlers
{
    /// <summary>
    /// Handles all design sliders (Focus, Direction, Priority)
    /// </summary>
    public static class SliderHandler
    {
        /// <summary>
        /// Apply optimal settings for all sliders based on selected genre
        /// Uses the game's internal optimal values directly (no research needed)
        /// </summary>
        public static void ApplyOptimalSliders(Menu_DevGame menu)
        {
            if (menu == null)
            {
                Plugin.Logger.LogError("Menu instance is null!");
                return;
            }

            try
            {
                Plugin.Logger.LogInfo("=== Applying Optimal Sliders ===");

                // Check if main genre is selected
                int mainGenre = menu.g_GameMainGenre;
                int subGenre = menu.g_GameSubGenre;

                if (mainGenre < 0)
                {
                    Plugin.Logger.LogWarning("Main Genre not selected - cannot apply optimal sliders");
                    return;
                }

                // Get genres script
                GameObject mainObj = GameObject.Find("Main");
                if (mainObj == null)
                {
                    Plugin.Logger.LogError("Main GameObject not found!");
                    return;
                }

                genres genresScript = mainObj.GetComponent<genres>();
                if (genresScript == null)
                {
                    Plugin.Logger.LogError("genres script not found!");
                    return;
                }

                // 1. Apply Design Focus (8 sliders)
                ApplyDesignFocus(menu, genresScript, mainGenre, subGenre);

                // 2. Apply Design Direction (3 sliders)
                ApplyDesignDirection(menu, genresScript, mainGenre, subGenre);

                // 3. Apply Design Priority (4 sliders)
                ApplyDesignPriority(menu, genresScript, mainGenre);

                // Update UI
                menu.UpdateDesignSettings();

                Plugin.Logger.LogInfo("All sliders applied successfully!");
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"Error in ApplyOptimalSliders: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Apply Design Focus sliders (Game Length, Game Depth, etc.)
        /// Uses game's GetFocus method to get optimal values based on genre data
        /// </summary>
        private static void ApplyDesignFocus(Menu_DevGame menu, genres genresScript, int mainGenre, int subGenre)
        {
            Plugin.Logger.LogInfo("  Applying Design Focus...");
            
            string[] focusNames = { "Game Length", "Game Depth", "Beginner Friendliness", "Innovation", 
                                    "Story", "Character Design", "Level Design", "Mission Design" };

            for (int slot = 0; slot < menu.g_Designschwerpunkt.Length; slot++)
            {
                // GetFocus calculates optimal value from genres_FOCUS data
                // It combines main genre + subgenre values and ensures total = 40
                int optimalValue = genresScript.GetFocus(slot, mainGenre, subGenre);
                menu.g_Designschwerpunkt[slot] = optimalValue;
                
                // Update UI slider
                if (menu.uiDesignschwerpunkte != null && slot < menu.uiDesignschwerpunkte.Length)
                {
                    Slider slider = menu.uiDesignschwerpunkte[slot]?.transform.GetChild(1)?.GetComponent<Slider>();
                    if (slider != null)
                        slider.value = optimalValue;
                }
                
                Plugin.Logger.LogInfo($"    {focusNames[slot]}: {optimalValue}");
            }
        }

        /// <summary>
        /// Apply Design Direction sliders (Core/Casual, Violence, Difficulty)
        /// Uses game's GetAlign method to get optimal values based on genre data
        /// </summary>
        private static void ApplyDesignDirection(Menu_DevGame menu, genres genresScript, int mainGenre, int subGenre)
        {
            Plugin.Logger.LogInfo("  Applying Design Direction...");
            
            string[] directionNames = { "Core ↔ Casual", "Nonviolent ↔ Explicit", "Easy ↔ Hard" };

            for (int slot = 0; slot < menu.g_Designausrichtung.Length; slot++)
            {
                // GetAlign calculates optimal value from genres_ALIGN data
                // It combines main genre + subgenre values
                int optimalValue = genresScript.GetAlign(slot, mainGenre, subGenre);
                menu.g_Designausrichtung[slot] = optimalValue;
                
                // Update UI slider
                if (menu.uiDesignausrichtung != null && slot < menu.uiDesignausrichtung.Length)
                {
                    Slider slider = menu.uiDesignausrichtung[slot]?.transform.GetChild(1)?.GetComponent<Slider>();
                    if (slider != null)
                        slider.value = optimalValue;
                }
                
                Plugin.Logger.LogInfo($"    {directionNames[slot]}: {optimalValue}");
            }
        }

        /// <summary>
        /// Apply Design Priority sliders (Gameplay%, Graphics%, Sound%, Technical%)
        /// Uses genre's optimal distribution values (genres_GAMEPLAY, genres_GRAPHIC, etc.)
        /// Values are 0-20 (displayed as 0%-100% in 5% steps)
        /// Total should equal 20 (100%)
        /// </summary>
        private static void ApplyDesignPriority(Menu_DevGame menu, genres genresScript, int mainGenre)
        {
            Plugin.Logger.LogInfo("  Applying Design Priority...");

            // Get optimal values from genre (these are percentages 0-100)
            float gameplay = genresScript.genres_GAMEPLAY[mainGenre];
            float grafik = genresScript.genres_GRAPHIC[mainGenre];
            float sound = genresScript.genres_SOUND[mainGenre];
            float control = genresScript.genres_CONTROL[mainGenre]; // Technical

            Plugin.Logger.LogInfo($"    Genre optimal - Gameplay:{gameplay}%, Graphics:{grafik}%, Sound:{sound}%, Technical:{control}%");

            // Convert from percentage to slider value (0-20 range, 5% per step)
            int gameplayVal = Mathf.RoundToInt(gameplay / 5f);
            int grafikVal = Mathf.RoundToInt(grafik / 5f);
            int soundVal = Mathf.RoundToInt(sound / 5f);
            int controlVal = Mathf.RoundToInt(control / 5f);

            // Ensure total equals 20 (100%)
            int total = gameplayVal + grafikVal + soundVal + controlVal;
            if (total != 20)
            {
                // Adjust to make it equal 20
                int diff = 20 - total;
                
                // Find which one to adjust (prefer adjusting the largest value)
                if (diff > 0)
                {
                    // Need to add points
                    while (diff > 0)
                    {
                        int maxVal = Mathf.Max(gameplayVal, Mathf.Max(grafikVal, Mathf.Max(soundVal, controlVal)));
                        if (gameplayVal == maxVal && gameplayVal < 20) { gameplayVal++; diff--; }
                        else if (grafikVal == maxVal && grafikVal < 20) { grafikVal++; diff--; }
                        else if (soundVal == maxVal && soundVal < 20) { soundVal++; diff--; }
                        else if (controlVal < 20) { controlVal++; diff--; }
                        else break;
                    }
                }
                else
                {
                    // Need to remove points
                    while (diff < 0)
                    {
                        int maxVal = Mathf.Max(gameplayVal, Mathf.Max(grafikVal, Mathf.Max(soundVal, controlVal)));
                        if (gameplayVal == maxVal && gameplayVal > 0) { gameplayVal--; diff++; }
                        else if (grafikVal == maxVal && grafikVal > 0) { grafikVal--; diff++; }
                        else if (soundVal == maxVal && soundVal > 0) { soundVal--; diff++; }
                        else if (controlVal > 0) { controlVal--; diff++; }
                        else break;
                    }
                }
            }

            // Clamp values to valid range
            gameplayVal = Mathf.Clamp(gameplayVal, 0, 20);
            grafikVal = Mathf.Clamp(grafikVal, 0, 20);
            soundVal = Mathf.Clamp(soundVal, 0, 20);
            controlVal = Mathf.Clamp(controlVal, 0, 20);

            // Set values and update UI
            // uiObjects[97-100] are the sliders, uiObjects[101-104] are the text displays
            
            // Gameplay (uiObjects[97])
            menu.g_GameAP_Gameplay = gameplayVal;
            if (menu.uiObjects[97] != null)
            {
                Slider slider = menu.uiObjects[97].GetComponent<Slider>();
                if (slider != null) slider.value = gameplayVal;
            }
            if (menu.uiObjects[101] != null)
                menu.uiObjects[101].GetComponent<Text>().text = (gameplayVal * 5).ToString() + "%";

            // Graphics (uiObjects[98])
            menu.g_GameAP_Grafik = grafikVal;
            if (menu.uiObjects[98] != null)
            {
                Slider slider = menu.uiObjects[98].GetComponent<Slider>();
                if (slider != null) slider.value = grafikVal;
            }
            if (menu.uiObjects[102] != null)
                menu.uiObjects[102].GetComponent<Text>().text = (grafikVal * 5).ToString() + "%";

            // Sound (uiObjects[99])
            menu.g_GameAP_Sound = soundVal;
            if (menu.uiObjects[99] != null)
            {
                Slider slider = menu.uiObjects[99].GetComponent<Slider>();
                if (slider != null) slider.value = soundVal;
            }
            if (menu.uiObjects[103] != null)
                menu.uiObjects[103].GetComponent<Text>().text = (soundVal * 5).ToString() + "%";

            // Technical (uiObjects[100])
            menu.g_GameAP_Technik = controlVal;
            if (menu.uiObjects[100] != null)
            {
                Slider slider = menu.uiObjects[100].GetComponent<Slider>();
                if (slider != null) slider.value = controlVal;
            }
            if (menu.uiObjects[104] != null)
                menu.uiObjects[104].GetComponent<Text>().text = (controlVal * 5).ToString() + "%";

            Plugin.Logger.LogInfo($"    Applied - Gameplay:{gameplayVal*5}%, Graphics:{grafikVal*5}%, Sound:{soundVal*5}%, Technical:{controlVal*5}%");
        }
    }
}
