using UnityEngine;
using UnityEngine.UI;

namespace MGT2AssistantButton.Core.Handlers
{
    public static class NamingHandler
    {
        public static void ApplyRealName(Menu_DevGame menu)
        {
            if (menu == null) return;

            try
            {
                if (menu.uiObjects != null && menu.uiObjects.Length > 0)
                {
                    InputField nameInput = menu.uiObjects[0]?.GetComponent<InputField>();
                    if (nameInput != null && !nameInput.interactable) return;
                }

                string randomName = null;

                // Check if sequel
                if (menu.typ_nachfolger && menu.g_originalGameID >= 0 && !string.IsNullOrEmpty(menu.g_myNameTeil1))
                {
                    randomName = $"{menu.g_myNameTeil1} {menu.g_teil}";
                }

                // Generate new name
                if (string.IsNullOrEmpty(randomName))
                {
                    int attempts = 0;
                    int maxAttempts = 50;

                    GameObject mainObj = GameObject.Find("Main");
                    if (mainObj == null) return;

                    games gamesScript = mainObj.GetComponent<games>();
                    if (gamesScript == null) return;

                    int mainGenre = menu.g_GameMainGenre;
                    if (mainGenre >= 0)
                    {
                        while (attempts < maxAttempts)
                        {
                            randomName = Data.GameNamesDatabase.GetRandomNameForGenre(mainGenre);
                            if (string.IsNullOrEmpty(randomName)) break;
                            if (!IsGameNameTaken(randomName, gamesScript)) break;
                            attempts++;
                        }
                        if (attempts >= maxAttempts) randomName = null;
                    }

                    // Fallback to default names
                    if (string.IsNullOrEmpty(randomName))
                    {
                        textScript tS = mainObj.GetComponent<textScript>();
                        if (tS == null) return;

                        attempts = 0;
                        while (attempts < maxAttempts)
                        {
                            randomName = tS.GetRandomGameName();
                            if (!IsGameNameTaken(randomName, gamesScript)) break;
                            attempts++;
                        }
                    }
                }
                
                // Apply name
                if (menu.uiObjects != null && menu.uiObjects.Length > 0)
                {
                    InputField nameInput = menu.uiObjects[0]?.GetComponent<InputField>();
                    if (nameInput != null)
                        nameInput.text = randomName;
                }
            }
            catch (System.Exception) { }
        }

        private static bool IsGameNameTaken(string name, games gamesScript)
        {
            if (string.IsNullOrEmpty(name) || gamesScript == null) return false;

            for (int i = 0; i < gamesScript.arrayGamesScripts.Length; i++)
            {
                gameScript game = gamesScript.arrayGamesScripts[i];
                if (game != null && game.GetNameSimple() == name)
                    return true;
            }
            return false;
        }
    }
}
