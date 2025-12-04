using UnityEngine;
using UnityEngine.UI;

namespace MGT2AssistantButton.Core.Handlers
{
    /// <summary>
    /// Handles game naming functionality
    /// </summary>
    public static class NamingHandler
    {
        public static void ApplyRealName(Menu_DevGame menu)
        {
            if (menu == null)
            {
                Plugin.Logger.LogError("Menu instance is null!");
                return;
            }

            try
            {
                Plugin.Logger.LogInfo("=== Applying Real Name ===");
                
                // Check if InputField[0] is interactable
                if (menu.uiObjects != null && menu.uiObjects.Length > 0)
                {
                    InputField nameInput = menu.uiObjects[0]?.GetComponent<InputField>();
                    if (nameInput != null && !nameInput.interactable)
                    {
                        Plugin.Logger.LogInfo("Name InputField is not interactable - skipping");
                        return;
                    }
                }

                string randomName = null;

                // Check if this is a sequel - use original game name + part number
                if (menu.typ_nachfolger && menu.g_originalGameID >= 0)
                {
                    // Use the base name (without part number) stored in g_myNameTeil1
                    if (!string.IsNullOrEmpty(menu.g_myNameTeil1))
                    {
                        randomName = $"{menu.g_myNameTeil1} {menu.g_teil}";
                        Plugin.Logger.LogInfo($"Sequel detected - using name: {randomName}");
                    }
                }

                // If not a sequel or couldn't find original, generate new name
                if (string.IsNullOrEmpty(randomName))
                {
                    int attempts = 0;
                    int maxAttempts = 50; // Try up to 50 times to find unique name

                    // Get games script for duplicate checking
                    GameObject mainObj = GameObject.Find("Main");
                    if (mainObj == null)
                    {
                        Plugin.Logger.LogError("Main GameObject not found!");
                        return;
                    }

                    games gamesScript = mainObj.GetComponent<games>();
                    if (gamesScript == null)
                    {
                        Plugin.Logger.LogError("games script not found!");
                        return;
                    }

                    // Try to get genre-specific unique name
                    int mainGenre = menu.g_GameMainGenre;
                    if (mainGenre >= 0)
                    {
                        while (attempts < maxAttempts)
                        {
                            randomName = Data.GameNamesDatabase.GetRandomNameForGenre(mainGenre);
                            
                            if (string.IsNullOrEmpty(randomName))
                                break; // No names available for this genre

                            // Check if name already exists
                            if (!IsGameNameTaken(randomName, gamesScript))
                            {
                                Plugin.Logger.LogInfo($"Using genre-specific unique name: {randomName}");
                                break;
                            }

                            attempts++;
                        }

                        if (attempts >= maxAttempts)
                        {
                            Plugin.Logger.LogWarning($"Could not find unique genre-specific name after {maxAttempts} attempts");
                            randomName = null; // Will fall back to default
                        }
                    }

                    // Fall back to game's default random names if no genre-specific name
                    if (string.IsNullOrEmpty(randomName))
                    {
                        textScript tS = mainObj.GetComponent<textScript>();
                        if (tS == null)
                        {
                            Plugin.Logger.LogError("textScript not found!");
                            return;
                        }

                        // Try to get unique default name
                        attempts = 0;
                        while (attempts < maxAttempts)
                        {
                            randomName = tS.GetRandomGameName();
                            
                            if (!IsGameNameTaken(randomName, gamesScript))
                            {
                                Plugin.Logger.LogInfo($"Using default unique name: {randomName}");
                                break;
                            }

                            attempts++;
                        }

                        if (attempts >= maxAttempts)
                        {
                            Plugin.Logger.LogWarning($"Could not find unique default name after {maxAttempts} attempts, using anyway");
                        }
                    }
                }
                
                // Apply to the input field
                if (menu.uiObjects != null && menu.uiObjects.Length > 0)
                {
                    InputField nameInput = menu.uiObjects[0]?.GetComponent<InputField>();
                    if (nameInput != null)
                    {
                        nameInput.text = randomName;
                        Plugin.Logger.LogInfo($"Set game name to: {randomName}");
                    }
                    else
                    {
                        Plugin.Logger.LogWarning("Could not find name InputField!");
                    }
                }
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"Error in ApplyRealName: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Check if a game name is already taken by existing games
        /// </summary>
        private static bool IsGameNameTaken(string name, games gamesScript)
        {
            if (string.IsNullOrEmpty(name) || gamesScript == null)
                return false;

            // Check all existing games
            for (int i = 0; i < gamesScript.arrayGamesScripts.Length; i++)
            {
                gameScript game = gamesScript.arrayGamesScripts[i];
                if (game != null && game.GetNameSimple() == name)
                {
                    return true; // Name is taken
                }
            }

            return false; // Name is available
        }
    }
}
