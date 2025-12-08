using UnityEngine;
using UnityEngine.UI;
using MGT2AssistantButton.Core;
using MGT2AssistantButton.Helpers;
using HarmonyLib;

namespace MGT2AssistantButton.UI
{
    public static class MarketingCampaignUI
    {
        private static GameObject autoButtonGame;
        private static GameObject autoButtonConsole;

        public static void InitGame(Menu_Marketing_GameKampagne menu)
        {
            if (menu == null) return;
            if (autoButtonGame != null) Object.Destroy(autoButtonGame);

            // Find WindowMain child
            Transform windowMain = menu.transform.Find("WindowMain");
            if (windowMain == null) windowMain = menu.transform;

            // Find Button_Desc source from DevGame menu
            GameObject descSource = FindButtonDesc();
            if (descSource == null)
            {
                Plugin.Logger.LogWarning("Could not find Button_Desc for Marketing button");
                return;
            }

            // Clone using same pattern as DevGame
            autoButtonGame = ButtonHelper.CloneBasicButton(descSource, windowMain, "Button_Auto90");
            if (autoButtonGame == null) return;
            
            // Set position
            RectTransform rect = autoButtonGame.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-75f, -472f);
            rect.sizeDelta = new Vector2(50f, 50f);
            
            // Add icon as child (same as DevGame)
            AddIcon(autoButtonGame);
            
            // Add click handler
            ButtonHelper.AddButtonComponent(autoButtonGame, () => {
                StartBestCampaignSequential(menu);
            });
            ButtonHelper.SetTooltip(autoButtonGame, "Auto: Select best campaign and loop until 90 Hype");
        }
        
        public static void InitConsole(Menu_Marketing_KonsoleKampagne menu)
        {
            if (menu == null) return;
            if (autoButtonConsole != null) Object.Destroy(autoButtonConsole);

            // Find WindowMain child
            Transform windowMain = menu.transform.Find("WindowMain");
            if (windowMain == null) windowMain = menu.transform;

            // Find Button_Desc source
            GameObject descSource = FindButtonDesc();
            if (descSource == null) return;

            // Clone using same pattern as DevGame
            autoButtonConsole = ButtonHelper.CloneBasicButton(descSource, windowMain, "Button_Auto90_Console");
            if (autoButtonConsole == null) return;
            
            // Set position
            RectTransform rect = autoButtonConsole.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-75f, -472f);
            rect.sizeDelta = new Vector2(50f, 50f);
            
            // Add icon
            AddIcon(autoButtonConsole);
            
            // Add click handler
            ButtonHelper.AddButtonComponent(autoButtonConsole, () => {
                StartBestConsoleCampaign(menu);
            });
            ButtonHelper.SetTooltip(autoButtonConsole, "Auto: Select best console campaign and loop until 90 Hype");
        }
        
        private static GameObject FindButtonDesc()
        {
            // Find from Menu_DevGame (same source as DevGameAssistantUI)
            GUI_Main guiMain = GameObject.Find("CanvasInGameMenu")?.GetComponent<GUI_Main>();
            if (guiMain != null && guiMain.uiObjects != null && guiMain.uiObjects.Length > 56)
            {
                Menu_DevGame devGameMenu = guiMain.uiObjects[56]?.GetComponent<Menu_DevGame>();
                if (devGameMenu != null && devGameMenu.uiObjects != null)
                {
                    foreach (GameObject obj in devGameMenu.uiObjects)
                    {
                        if (obj != null && obj.name.Contains("Button_Desc")) return obj;
                    }
                    
                    // Fallback: search children
                    Transform[] children = devGameMenu.GetComponentsInChildren<Transform>(true);
                    foreach (Transform child in children)
                    {
                        if (child.name.Contains("Button_Desc")) return child.gameObject;
                    }
                }
            }
            return null;
        }
        
        private static void AddIcon(GameObject btn)
        {
            string iconPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "ButtonIcon.png");
            Sprite iconSprite = SpriteHelper.LoadSprite(iconPath);
            if (iconSprite != null)
            {
                GameObject iconObj = new GameObject("Icon");
                iconObj.transform.SetParent(btn.transform, false);
                
                Image iconImage = iconObj.AddComponent<Image>();
                iconImage.sprite = iconSprite;
                iconImage.preserveAspect = true;
                iconImage.raycastTarget = false;
                
                RectTransform iconRect = iconObj.GetComponent<RectTransform>();
                iconRect.anchorMin = new Vector2(0.5f, 0.5f);
                iconRect.anchorMax = new Vector2(0.5f, 0.5f);
                iconRect.pivot = new Vector2(0.5f, 0.5f);
                iconRect.anchoredPosition = Vector2.zero;
                iconRect.sizeDelta = new Vector2(30f, 30f);
            }
        }
        
        private static int FindBestCampaign(int[] maxHype, float currentHype, unlockScript unlock)
        {
            int[] campaignOrder = { 0, 1, 2, 3, 4, 5 };
            
            foreach (int i in campaignOrder)
            {
                if (i == 3 && !unlock.Get(56)) continue;
                if (i == 4 && !unlock.Get(57)) continue;
                
                if (currentHype < maxHype[i])
                {
                    return i;
                }
            }
            
            return -1;
        }
        
        private static void StartBestCampaignSequential(Menu_Marketing_GameKampagne menu)
        {
            gameScript gS = AccessTools.Field(typeof(Menu_Marketing_GameKampagne), "selectedGame").GetValue(menu) as gameScript;
            if (gS == null) return;
            
            roomScript rS = AccessTools.Field(typeof(Menu_Marketing_GameKampagne), "rS_").GetValue(menu) as roomScript;
            if (rS == null) return;
            
            GameObject mainObj = GameObject.FindGameObjectWithTag("Main");
            mainScript mS = mainObj.GetComponent<mainScript>();
            unlockScript unlock = mainObj.GetComponent<unlockScript>();
            GUI_Main guiMain = GameObject.Find("CanvasInGameMenu").GetComponent<GUI_Main>();
            sfxScript sfx = GameObject.Find("SFX").GetComponent<sfxScript>();
            
            float currentHype = gS.hype;
            
            if (currentHype >= 90f) return;
            
            int bestCampaign = FindBestCampaign(menu.maxHype, currentHype, unlock);
            if (bestCampaign == -1) return;
            
            long price = menu.preise[bestCampaign];
            if (mS.NotEnoughMoney(price))
            {
                guiMain.ShowNoMoney();
                return;
            }
            
            sfx.PlaySound(3, true);
            mS.Pay(price, 12);
            
            taskMarketing task = guiMain.AddTask_Marketing();
            task.Init(false);
            task.targetID = gS.myID;
            task.typ = 0;
            task.kampagne = bestCampaign;
            task.automatic = true;
            task.stopAutomatic = false;
            task.disableWarten = true;
            task.points = (float)menu.workPoints[bestCampaign];
            task.pointsLeft = task.points;
            
            rS.taskID = task.myID;
            
            MarketingAutomationManager.SetAutoSwitch(task.myID, true);
            
            guiMain.CloseMenu();
            guiMain.uiObjects[88].SetActive(false);
            menu.gameObject.SetActive(false);
        }
        
        private static void StartBestConsoleCampaign(Menu_Marketing_KonsoleKampagne menu)
        {
            platformScript pS = AccessTools.Field(typeof(Menu_Marketing_KonsoleKampagne), "selectedKonsole").GetValue(menu) as platformScript;
            if (pS == null) return;
            
            roomScript rS = AccessTools.Field(typeof(Menu_Marketing_KonsoleKampagne), "rS_").GetValue(menu) as roomScript;
            if (rS == null) return;
            
            GameObject mainObj = GameObject.FindGameObjectWithTag("Main");
            mainScript mS = mainObj.GetComponent<mainScript>();
            unlockScript unlock = mainObj.GetComponent<unlockScript>();
            GUI_Main guiMain = GameObject.Find("CanvasInGameMenu").GetComponent<GUI_Main>();
            sfxScript sfx = GameObject.Find("SFX").GetComponent<sfxScript>();
            
            float currentHype = pS.GetHype();
            
            if (currentHype >= 90f) return;
            
            int bestCampaign = FindBestCampaign(menu.maxHype, currentHype, unlock);
            if (bestCampaign == -1) return;
            
            long price = menu.preise[bestCampaign];
            if (mS.NotEnoughMoney(price))
            {
                guiMain.ShowNoMoney();
                return;
            }
            
            sfx.PlaySound(3, true);
            mS.Pay(price, 12);
            
            taskMarketing task = guiMain.AddTask_Marketing();
            task.Init(false);
            task.targetID = pS.myID;
            task.typ = 1;
            task.kampagne = bestCampaign;
            task.automatic = true;
            task.stopAutomatic = false;
            task.disableWarten = true;
            task.points = (float)menu.workPoints[bestCampaign];
            task.pointsLeft = task.points;
            
            rS.taskID = task.myID;
            
            MarketingAutomationManager.SetAutoSwitch(task.myID, true);
            
            guiMain.CloseMenu();
            guiMain.uiObjects[88].SetActive(false);
            menu.gameObject.SetActive(false);
        }
        
        // Legacy
        public static void Init(Menu_Marketing_GameKampagne menu) => InitGame(menu);
    }
}
