using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MGT2AssistantButton.Core.Handlers
{
    public static class ContentHandler
    {
        public static void ApplyBestTargetGroup(Menu_DevGame menu, bool checkInteractable)
        {
            if (menu == null) return;

            try
            {
                if (checkInteractable && !IsButtonInteractable(menu, 127)) return;
                
                int mainGenre = menu.g_GameMainGenre;
                if (mainGenre < 0) return;

                GameObject mainObj = GameObject.Find("Main");
                if (mainObj == null) return;
                
                genres genresScript = mainObj.GetComponent<genres>();
                if (genresScript == null) return;

                List<int> compatibleTargetGroups = new List<int>();
                for (int i = 0; i < 5; i++)
                {
                    if (genresScript.IsTargetGroup(mainGenre, i))
                        compatibleTargetGroups.Add(i);
                }

                if (compatibleTargetGroups.Count == 0)
                {
                    menu.SetZielgruppe(0);
                    return;
                }

                int selectedTargetGroup = compatibleTargetGroups[Random.Range(0, compatibleTargetGroups.Count)];
                menu.SetZielgruppe(selectedTargetGroup);
            }
            catch (System.Exception) { }
        }

        public static void ApplyBestMainGenre(Menu_DevGame menu, bool checkInteractable)
        {
            if (menu == null) return;

            try
            {
                if (checkInteractable && !IsButtonInteractable(menu, 128)) return;
                
                GameObject mainObj = GameObject.Find("Main");
                if (mainObj == null) return;
                
                genres genresScript = mainObj.GetComponent<genres>();
                if (genresScript == null) return;

                int currentSubGenre = menu.g_GameSubGenre;
                List<int> researchedGenres = new List<int>();
                
                for (int i = 0; i < genresScript.genres_LEVEL.Length; i++)
                {
                    if (genresScript.IsErforscht(i) && i != currentSubGenre)
                        researchedGenres.Add(i);
                }

                if (researchedGenres.Count == 0)
                {
                    menu.SetMainGenre(-1);
                    return;
                }

                int selectedGenre = researchedGenres[Random.Range(0, researchedGenres.Count)];
                menu.SetMainGenre(selectedGenre);
            }
            catch (System.Exception) { }
        }

        public static void ApplyBestSubGenre(Menu_DevGame menu, bool checkInteractable)
        {
            if (menu == null) return;

            try
            {
                if (checkInteractable && !IsButtonInteractable(menu, 15)) return;
                
                int mainGenre = menu.g_GameMainGenre;
                if (mainGenre < 0) return;

                GameObject mainObj = GameObject.Find("Main");
                if (mainObj == null) return;
                
                genres genresScript = mainObj.GetComponent<genres>();
                if (genresScript == null) return;

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
                    menu.SetSubGenre(-1);
                    return;
                }

                int selectedGenre = compatibleGenres[Random.Range(0, compatibleGenres.Count)];
                menu.SetSubGenre(selectedGenre);
            }
            catch (System.Exception) { }
        }

        public static void ApplyBestMainTheme(Menu_DevGame menu, bool checkInteractable)
        {
            if (menu == null) return;

            try
            {
                if (checkInteractable && !IsButtonInteractable(menu, 129)) return;
                
                int mainGenre = menu.g_GameMainGenre;
                int currentSubTheme = menu.g_GameSubTheme;
                
                if (mainGenre < 0) return;

                GameObject mainObj = GameObject.Find("Main");
                if (mainObj == null) return;
                
                themes themesScript = mainObj.GetComponent<themes>();
                if (themesScript == null) return;

                List<int> availableThemes = new List<int>();
                for (int i = 0; i < themesScript.themes_LEVEL.Length; i++)
                {
                    if (i == currentSubTheme) continue;
                    if (themesScript.IsErforscht(i) && themesScript.IsThemesFitWithGenre(i, mainGenre))
                        availableThemes.Add(i);
                }

                if (availableThemes.Count == 0)
                {
                    menu.SetMainTheme(-1);
                    return;
                }

                int selectedTheme = availableThemes[Random.Range(0, availableThemes.Count)];
                menu.SetMainTheme(selectedTheme);
            }
            catch (System.Exception) { }
        }

        public static void ApplyBestSubTheme(Menu_DevGame menu, bool checkInteractable)
        {
            if (menu == null) return;

            try
            {
                if (checkInteractable && !IsButtonInteractable(menu, 16)) return;
                
                int mainGenre = menu.g_GameMainGenre;
                int currentMainTheme = menu.g_GameMainTheme;
                
                if (mainGenre < 0) return;

                GameObject mainObj = GameObject.Find("Main");
                if (mainObj == null) return;
                
                themes themesScript = mainObj.GetComponent<themes>();
                if (themesScript == null) return;

                List<int> availableThemes = new List<int>();
                for (int i = 0; i < themesScript.themes_LEVEL.Length; i++)
                {
                    if (i == currentMainTheme) continue;
                    if (themesScript.IsErforscht(i) && themesScript.IsThemesFitWithGenre(i, mainGenre))
                        availableThemes.Add(i);
                }

                if (availableThemes.Count == 0)
                {
                    menu.SetSubTheme(-1);
                    return;
                }

                int selectedTheme = availableThemes[Random.Range(0, availableThemes.Count)];
                menu.SetSubTheme(selectedTheme);
            }
            catch (System.Exception) { }
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
