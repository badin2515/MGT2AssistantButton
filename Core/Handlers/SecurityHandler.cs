using UnityEngine;
using UnityEngine.UI;

namespace MGT2AssistantButton.Core.Handlers
{
    /// <summary>
    /// Handles Anti-Cheat and Copy Protection selection
    /// </summary>
    public static class SecurityHandler
    {
        public static void ApplyAntiCheat(Menu_DevGame menu)
        {
            if (menu == null)
            {
                Plugin.Logger.LogError("Menu instance is null!");
                return;
            }

            try
            {
                Plugin.Logger.LogInfo("=== Applying Best Anti-Cheat ===");
                
                // Check platform type - no anti-cheat for mobile/browser games (platform type 4)
                if (menu.uiObjects != null && menu.uiObjects.Length > 146)
                {
                    Dropdown platformDropdown = menu.uiObjects[146]?.GetComponent<Dropdown>();
                    if (platformDropdown != null && platformDropdown.value == 4)
                    {
                        Plugin.Logger.LogInfo("Platform type is mobile/browser - no anti-cheat available");
                        menu.SetAntiCheat(-1);
                        return;
                    }
                }
                
                // Find best anti-cheat (highest effekt)
                GameObject[] antiCheatObjects = GameObject.FindGameObjectsWithTag("AntiCheat");
                antiCheatScript bestAntiCheat = null;
                float highestEffect = 0f;

                foreach (GameObject acObj in antiCheatObjects)
                {
                    if (acObj == null) continue;

                    antiCheatScript ac = acObj.GetComponent<antiCheatScript>();
                    if (ac != null && ac.inBesitz && ac.effekt > highestEffect)
                    {
                        bestAntiCheat = ac;
                        highestEffect = ac.effekt;
                    }
                }

                if (bestAntiCheat != null)
                {
                    menu.SetAntiCheat(bestAntiCheat.myID);
                    Plugin.Logger.LogInfo($"Selected Anti-Cheat: {bestAntiCheat.name} (Effect: {highestEffect})");
                }
                else
                {
                    menu.SetAntiCheat(-1);
                    Plugin.Logger.LogInfo("No Anti-Cheat available");
                }
                
                Plugin.Logger.LogInfo("Auto Anti-Cheat applied");
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"Error in ApplyAntiCheat: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public static void ApplyCopyProtect(Menu_DevGame menu)
        {
            if (menu == null)
            {
                Plugin.Logger.LogError("Menu instance is null!");
                return;
            }

            try
            {
                Plugin.Logger.LogInfo("=== Applying Best Copy Protection ===");
                
                // Check if Copy Protection button is interactable (index 84)
                if (menu.uiObjects != null && menu.uiObjects.Length > 84)
                {
                    Button copyProtectButton = menu.uiObjects[84]?.GetComponent<Button>();
                    if (copyProtectButton != null && !copyProtectButton.interactable)
                    {
                        Plugin.Logger.LogInfo("Copy Protection button is not interactable - skipping");
                        return;
                    }
                }
                
                // Check platform type - no copy protection for mobile/browser games (platform type 4)
                if (menu.uiObjects != null && menu.uiObjects.Length > 146)
                {
                    Dropdown platformDropdown = menu.uiObjects[146]?.GetComponent<Dropdown>();
                    if (platformDropdown != null && platformDropdown.value == 4)
                    {
                        Plugin.Logger.LogInfo("Platform type is mobile/browser - no copy protection available");
                        menu.SetCopyProtect(-1);
                        return;
                    }
                }
                
                // Find best copy protection (highest effekt)
                GameObject[] copyProtectObjects = GameObject.FindGameObjectsWithTag("CopyProtect");
                copyProtectScript bestCopyProtect = null;
                float highestEffect = 0f;

                foreach (GameObject cpObj in copyProtectObjects)
                {
                    if (cpObj == null) continue;

                    copyProtectScript cp = cpObj.GetComponent<copyProtectScript>();
                    if (cp != null && cp.inBesitz && cp.effekt > highestEffect)
                    {
                        bestCopyProtect = cp;
                        highestEffect = cp.effekt;
                    }
                }

                if (bestCopyProtect != null)
                {
                    menu.SetCopyProtect(bestCopyProtect.myID);
                    Plugin.Logger.LogInfo($"Selected Copy Protection: {bestCopyProtect.name} (Effect: {highestEffect})");
                }
                else
                {
                    menu.SetCopyProtect(-1);
                    Plugin.Logger.LogInfo("No Copy Protection available");
                }
                
                Plugin.Logger.LogInfo("Auto Copy Protection applied");
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"Error in ApplyCopyProtect: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
