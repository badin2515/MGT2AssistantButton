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
            if (menu == null) return;

            try
            {
                // Check platform type - no anti-cheat for mobile/browser games (type 4)
                if (menu.uiObjects != null && menu.uiObjects.Length > 146)
                {
                    Dropdown platformDropdown = menu.uiObjects[146]?.GetComponent<Dropdown>();
                    if (platformDropdown != null && platformDropdown.value == 4)
                    {
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

                menu.SetAntiCheat(bestAntiCheat != null ? bestAntiCheat.myID : -1);
            }
            catch (System.Exception) { }
        }

        public static void ApplyCopyProtect(Menu_DevGame menu)
        {
            if (menu == null) return;

            try
            {
                // Check if Copy Protection button is interactable
                if (menu.uiObjects != null && menu.uiObjects.Length > 84)
                {
                    Button copyProtectButton = menu.uiObjects[84]?.GetComponent<Button>();
                    if (copyProtectButton != null && !copyProtectButton.interactable)
                        return;
                }
                
                // Check platform type - no copy protection for mobile/browser games (type 4)
                if (menu.uiObjects != null && menu.uiObjects.Length > 146)
                {
                    Dropdown platformDropdown = menu.uiObjects[146]?.GetComponent<Dropdown>();
                    if (platformDropdown != null && platformDropdown.value == 4)
                    {
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

                menu.SetCopyProtect(bestCopyProtect != null ? bestCopyProtect.myID : -1);
            }
            catch (System.Exception) { }
        }
    }
}
