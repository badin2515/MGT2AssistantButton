using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MGT2AssistantButton.Core.Handlers
{
    /// <summary>
    /// Handles game content selection (Target Group, Genres, Themes)
    /// </summary>
    public static class ContentHandler
    {
        public static void ApplyBestTargetGroup(Menu_DevGame menu, bool checkInteractable)
        {
            if (menu == null)
            {
                Plugin.Logger.LogError("Menu instance is null!");
                return;
            }

            try
            {
                Plugin.Logger.LogInfo("=== Applying Best Target Group ===");
                
                if (checkInteractable && !IsButtonInteractable(menu, 127))
                {
                    Plugin.Logger.LogInfo("Target Group button is not interactable - skipping");
                    return;
                }
                
                int mainGenre = menu.g_GameMainGenre;
                if (mainGenre < 0)
                {
                    Plugin.Logger.LogWarning("Main Genre not selected yet!");
                    return;
                }

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

                List<int> compatibleTargetGroups = new List<int>();
                for (int i = 0; i < 5; i++)
                {
                    if (genresScript.IsTargetGroup(mainGenre, i))
                    {
                        compatibleTargetGroups.Add(i);
                    }
                }

                if (compatibleTargetGroups.Count == 0)
                {
                    Plugin.Logger.LogWarning("No compatible target groups found - clearing selection");
                    menu.SetZielgruppe(0);
                    return;
                }

                int selectedTargetGroup = compatibleTargetGroups[Random.Range(0, compatibleTargetGroups.Count)];
                menu.SetZielgruppe(selectedTargetGroup);
                
                Plugin.Logger.LogInfo($"Selected Target Group: {selectedTargetGroup} from {compatibleTargetGroups.Count} compatible options");
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"Error in ApplyBestTargetGroup: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public static void ApplyBestMainGenre(Menu_DevGame menu, bool checkInteractable)
        {
            if (menu == null)
            {
                Plugin.Logger.LogError("Menu instance is null!");
                return;
            }

            try
            {
                Plugin.Logger.LogInfo("=== Applying Best Main Genre ===");
                
                if (checkInteractable && !IsButtonInteractable(menu, 128))
                {
                    Plugin.Logger.LogInfo("Main Genre button is not interactable - skipping");
                    return;
                }
                
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

                int currentSubGenre = menu.g_GameSubGenre;
                List<int> researchedGenres = new List<int>();
                
                for (int i = 0; i < genresScript.genres_LEVEL.Length; i++)
                {
                    if (genresScript.IsErforscht(i) && i != currentSubGenre)
                    {
                        researchedGenres.Add(i);
                    }
                }

                if (researchedGenres.Count == 0)
                {
                    Plugin.Logger.LogWarning("No researched genres found - clearing selection");
                    menu.SetMainGenre(-1);
                    return;
                }

                int selectedGenre = researchedGenres[Random.Range(0, researchedGenres.Count)];
                menu.SetMainGenre(selectedGenre);
                
                Plugin.Logger.LogInfo($"Selected Main Genre: {selectedGenre} ({genresScript.GetName(selectedGenre)})");
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"Error in ApplyBestMainGenre: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public static void ApplyBestSubGenre(Menu_DevGame menu, bool checkInteractable)
        {
            if (menu == null)
            {
                Plugin.Logger.LogError("Menu instance is null!");
                return;
            }

            try
            {
                Plugin.Logger.LogInfo("=== Applying Best Sub Genre ===");
                
                if (checkInteractable && !IsButtonInteractable(menu, 15))
                {
                    Plugin.Logger.LogInfo("Sub Genre button is not interactable - skipping");
                    return;
                }
                
                int mainGenre = menu.g_GameMainGenre;
                if (mainGenre < 0)
                {
                    Plugin.Logger.LogWarning("Main Genre not selected yet!");
                    return;
                }

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

                List<int> compatibleGenres = new List<int>();
                for (int i = 0; i < genresScript.genres_LEVEL.Length; i++)
                {
                    if (genresScript.IsErforscht(i) 
                        && genresScript.IsGenreCombination(mainGenre, i) 
                        && i != mainGenre)
                    {
                        compatibleGenres.Add(i);
                    }
                }

                if (compatibleGenres.Count == 0)
                {
                    Plugin.Logger.LogWarning("No compatible sub genres found - clearing selection");
                    menu.SetSubGenre(-1);
                    return;
                }

                int selectedGenre = compatibleGenres[Random.Range(0, compatibleGenres.Count)];
                menu.SetSubGenre(selectedGenre);
                
                Plugin.Logger.LogInfo($"Selected Sub Genre: {selectedGenre} ({genresScript.GetName(selectedGenre)})");
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"Error in ApplyBestSubGenre: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public static void ApplyBestMainTheme(Menu_DevGame menu, bool checkInteractable)
        {
            if (menu == null)
            {
                Plugin.Logger.LogError("Menu instance is null!");
                return;
            }

            try
            {
                Plugin.Logger.LogInfo("=== Applying Best Main Theme ===");
                
                if (checkInteractable && !IsButtonInteractable(menu, 129))
                {
                    Plugin.Logger.LogInfo("Main Theme button is not interactable - skipping");
                    return;
                }
                
                int mainGenre = menu.g_GameMainGenre;
                int currentSubTheme = menu.g_GameSubTheme;
                
                if (mainGenre < 0)
                {
                    Plugin.Logger.LogWarning("Main Genre not selected yet!");
                    return;
                }

                GameObject mainObj = GameObject.Find("Main");
                if (mainObj == null)
                {
                    Plugin.Logger.LogError("Main GameObject not found!");
                    return;
                }
                
                themes themesScript = mainObj.GetComponent<themes>();
                if (themesScript == null)
                {
                    Plugin.Logger.LogError("themes script not found!");
                    return;
                }

                List<int> availableThemes = new List<int>();
                for (int i = 0; i < themesScript.themes_LEVEL.Length; i++)
                {
                    if (i == currentSubTheme)
                        continue;

                    if (themesScript.IsErforscht(i) && themesScript.IsThemesFitWithGenre(i, mainGenre))
                    {
                        availableThemes.Add(i);
                    }
                }

                if (availableThemes.Count == 0)
                {
                    Plugin.Logger.LogWarning("No themes found for this genre - clearing selection");
                    menu.SetMainTheme(-1);
                    return;
                }

                int selectedTheme = availableThemes[Random.Range(0, availableThemes.Count)];
                menu.SetMainTheme(selectedTheme);
                
                Plugin.Logger.LogInfo($"Selected Main Theme: {selectedTheme} from {availableThemes.Count} compatible themes");
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"Error in ApplyBestMainTheme: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public static void ApplyBestSubTheme(Menu_DevGame menu, bool checkInteractable)
        {
            if (menu == null)
            {
                Plugin.Logger.LogError("Menu instance is null!");
                return;
            }

            try
            {
                Plugin.Logger.LogInfo("=== Applying Best Sub Theme ===");
                
                if (checkInteractable && !IsButtonInteractable(menu, 16))
                {
                    Plugin.Logger.LogInfo("Sub Theme button is not interactable - skipping");
                    return;
                }
                
                int mainGenre = menu.g_GameMainGenre;
                int currentMainTheme = menu.g_GameMainTheme;
                
                if (mainGenre < 0)
                {
                    Plugin.Logger.LogWarning("Main Genre not selected yet!");
                    return;
                }

                GameObject mainObj = GameObject.Find("Main");
                if (mainObj == null)
                {
                    Plugin.Logger.LogError("Main GameObject not found!");
                    return;
                }
                
                themes themesScript = mainObj.GetComponent<themes>();
                if (themesScript == null)
                {
                    Plugin.Logger.LogError("themes script not found!");
                    return;
                }

                List<int> availableThemes = new List<int>();
                for (int i = 0; i < themesScript.themes_LEVEL.Length; i++)
                {
                    if (i == currentMainTheme)
                        continue;

                    if (themesScript.IsErforscht(i) && themesScript.IsThemesFitWithGenre(i, mainGenre))
                    {
                        availableThemes.Add(i);
                    }
                }

                if (availableThemes.Count == 0)
                {
                    Plugin.Logger.LogWarning("No sub themes found for this genre - clearing selection");
                    menu.SetSubTheme(-1);
                    return;
                }

                int selectedTheme = availableThemes[Random.Range(0, availableThemes.Count)];
                menu.SetSubTheme(selectedTheme);
                
                Plugin.Logger.LogInfo($"Selected Sub Theme: {selectedTheme} from {availableThemes.Count} compatible themes");
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"Error in ApplyBestSubTheme: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private static bool IsButtonInteractable(Menu_DevGame menu, int index)
        {
            if (menu.uiObjects != null && menu.uiObjects.Length > index)
            {
                Button button = menu.uiObjects[index]?.GetComponent<Button>();
                return button != null && button.interactable;
            }
            return false;
        }
    }
}
