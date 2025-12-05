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
            if (menu == null) return;

            try
            {
                int mainGenre = menu.g_GameMainGenre;
                int subGenre = menu.g_GameSubGenre;

                if (mainGenre < 0) return;

                GameObject mainObj = GameObject.Find("Main");
                if (mainObj == null) return;

                genres genresScript = mainObj.GetComponent<genres>();
                if (genresScript == null) return;

                ApplyDesignFocus(menu, genresScript, mainGenre, subGenre);
                ApplyDesignDirection(menu, genresScript, mainGenre, subGenre);
                ApplyDesignPriority(menu, genresScript, mainGenre);
                menu.UpdateDesignSettings();
            }
            catch (System.Exception) { }
        }

        private static void ApplyDesignFocus(Menu_DevGame menu, genres genresScript, int mainGenre, int subGenre)
        {
            for (int slot = 0; slot < menu.g_Designschwerpunkt.Length; slot++)
            {
                int optimalValue = genresScript.GetFocus(slot, mainGenre, subGenre);
                menu.g_Designschwerpunkt[slot] = optimalValue;
                
                if (menu.uiDesignschwerpunkte != null && slot < menu.uiDesignschwerpunkte.Length)
                {
                    Slider slider = menu.uiDesignschwerpunkte[slot]?.transform.GetChild(1)?.GetComponent<Slider>();
                    if (slider != null)
                        slider.value = optimalValue;
                }
            }
        }

        private static void ApplyDesignDirection(Menu_DevGame menu, genres genresScript, int mainGenre, int subGenre)
        {
            for (int slot = 0; slot < menu.g_Designausrichtung.Length; slot++)
            {
                int optimalValue = genresScript.GetAlign(slot, mainGenre, subGenre);
                menu.g_Designausrichtung[slot] = optimalValue;
                
                if (menu.uiDesignausrichtung != null && slot < menu.uiDesignausrichtung.Length)
                {
                    Slider slider = menu.uiDesignausrichtung[slot]?.transform.GetChild(1)?.GetComponent<Slider>();
                    if (slider != null)
                        slider.value = optimalValue;
                }
            }
        }

        private static void ApplyDesignPriority(Menu_DevGame menu, genres genresScript, int mainGenre)
        {
            float gameplay = genresScript.genres_GAMEPLAY[mainGenre];
            float grafik = genresScript.genres_GRAPHIC[mainGenre];
            float sound = genresScript.genres_SOUND[mainGenre];
            float control = genresScript.genres_CONTROL[mainGenre];

            int gameplayVal = Mathf.RoundToInt(gameplay / 5f);
            int grafikVal = Mathf.RoundToInt(grafik / 5f);
            int soundVal = Mathf.RoundToInt(sound / 5f);
            int controlVal = Mathf.RoundToInt(control / 5f);

            // Ensure total equals 20 (100%)
            int total = gameplayVal + grafikVal + soundVal + controlVal;
            if (total != 20)
            {
                int diff = 20 - total;
                while (diff != 0)
                {
                    int maxVal = Mathf.Max(gameplayVal, Mathf.Max(grafikVal, Mathf.Max(soundVal, controlVal)));
                    if (diff > 0)
                    {
                        if (gameplayVal == maxVal && gameplayVal < 20) { gameplayVal++; diff--; }
                        else if (grafikVal == maxVal && grafikVal < 20) { grafikVal++; diff--; }
                        else if (soundVal == maxVal && soundVal < 20) { soundVal++; diff--; }
                        else if (controlVal < 20) { controlVal++; diff--; }
                        else break;
                    }
                    else
                    {
                        if (gameplayVal == maxVal && gameplayVal > 0) { gameplayVal--; diff++; }
                        else if (grafikVal == maxVal && grafikVal > 0) { grafikVal--; diff++; }
                        else if (soundVal == maxVal && soundVal > 0) { soundVal--; diff++; }
                        else if (controlVal > 0) { controlVal--; diff++; }
                        else break;
                    }
                }
            }

            gameplayVal = Mathf.Clamp(gameplayVal, 0, 20);
            grafikVal = Mathf.Clamp(grafikVal, 0, 20);
            soundVal = Mathf.Clamp(soundVal, 0, 20);
            controlVal = Mathf.Clamp(controlVal, 0, 20);

            menu.g_GameAP_Gameplay = gameplayVal;
            if (menu.uiObjects[97] != null)
            {
                Slider slider = menu.uiObjects[97].GetComponent<Slider>();
                if (slider != null) slider.value = gameplayVal;
            }
            if (menu.uiObjects[101] != null)
                menu.uiObjects[101].GetComponent<Text>().text = (gameplayVal * 5).ToString() + "%";

            menu.g_GameAP_Grafik = grafikVal;
            if (menu.uiObjects[98] != null)
            {
                Slider slider = menu.uiObjects[98].GetComponent<Slider>();
                if (slider != null) slider.value = grafikVal;
            }
            if (menu.uiObjects[102] != null)
                menu.uiObjects[102].GetComponent<Text>().text = (grafikVal * 5).ToString() + "%";

            menu.g_GameAP_Sound = soundVal;
            if (menu.uiObjects[99] != null)
            {
                Slider slider = menu.uiObjects[99].GetComponent<Slider>();
                if (slider != null) slider.value = soundVal;
            }
            if (menu.uiObjects[103] != null)
                menu.uiObjects[103].GetComponent<Text>().text = (soundVal * 5).ToString() + "%";

            menu.g_GameAP_Technik = controlVal;
            if (menu.uiObjects[100] != null)
            {
                Slider slider = menu.uiObjects[100].GetComponent<Slider>();
                if (slider != null) slider.value = controlVal;
            }
            if (menu.uiObjects[104] != null)
                menu.uiObjects[104].GetComponent<Text>().text = (controlVal * 5).ToString() + "%";
        }
    }
}
