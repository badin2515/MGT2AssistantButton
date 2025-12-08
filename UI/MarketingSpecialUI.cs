using UnityEngine;
using UnityEngine.UI;
using MGT2AssistantButton.Core;
using MGT2AssistantButton.Helpers;
using HarmonyLib;

namespace MGT2AssistantButton.UI
{
    public static class MarketingSpecialUI
    {
        private static GameObject autoButton;

        public static void Init(Menu_MarketingSpezial menu)
        {
            if (menu == null) return;
            if (autoButton != null) Object.Destroy(autoButton);

            // Find WindowMain child
            Transform windowMain = menu.transform.Find("WindowMain");
            if (windowMain == null) windowMain = menu.transform;

            // Find Button_Desc source from DevGame menu
            GameObject descSource = FindButtonDesc();
            if (descSource == null)
            {
                Plugin.Logger.LogWarning("Could not find Button_Desc for Special Marketing button");
                return;
            }

            // Clone using same pattern as DevGame
            autoButton = ButtonHelper.CloneBasicButton(descSource, windowMain, "Button_AutoAllSpecial");
            if (autoButton == null) return;
            
            // Set position
            RectTransform rect = autoButton.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-75f, -680f);
            rect.sizeDelta = new Vector2(50f, 50f);
            
            // Add icon
            AddIcon(autoButton);
            
            // Add click handler
            ButtonHelper.AddButtonComponent(autoButton, () => {
                RunAllSpecialMarketing(menu);
            });
            ButtonHelper.SetTooltip(autoButton, "Auto: Run all available special marketing tasks");
        }
        
        private static GameObject FindButtonDesc()
        {
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
        
        private static void RunAllSpecialMarketing(Menu_MarketingSpezial menu)
        {
            gameScript gS = AccessTools.Field(typeof(Menu_MarketingSpezial), "selectedGame").GetValue(menu) as gameScript;
            if (gS == null) return;
            
            roomScript rS = AccessTools.Field(typeof(Menu_MarketingSpezial), "rS_").GetValue(menu) as roomScript;
            if (rS == null) return;
            
            GameObject mainObj = GameObject.FindGameObjectWithTag("Main");
            mainScript mS = mainObj.GetComponent<mainScript>();
            GUI_Main guiMain = GameObject.Find("CanvasInGameMenu").GetComponent<GUI_Main>();
            sfxScript sfx = GameObject.Find("SFX").GetComponent<sfxScript>();
            textScript tS = mainObj.GetComponent<textScript>();
            
            bool anyStarted = false;
            
            if (gS.inDevelopment || gS.schublade)
            {
                for (int i = 0; i <= 2; i++)
                {
                    if (gS.specialMarketing[i] != 0) continue;
                    if (IsBeingProcessed(gS.myID, i)) continue;
                    if (i == 2 && gS.hype < 90f) continue;
                    
                    if ((i == 0 || i == 1) && !gS.GetEinePlattformReleased())
                    {
                        guiMain.MessageBox(tS.GetText(2438), false);
                        break;
                    }
                    
                    long price = menu.preise[i];
                    if (mS.NotEnoughMoney(price))
                    {
                        guiMain.ShowNoMoney();
                        break;
                    }
                    
                    mS.Pay(price, 12);
                    taskMarketingSpezial task = guiMain.AddTask_MarketingSpezial();
                    task.Init(false);
                    task.targetID = gS.myID;
                    task.kampagne = i;
                    task.points = (float)menu.workPoints[i];
                    task.pointsLeft = task.points;
                    
                    rS.taskID = task.myID;
                    MarketingAutomationManager.SetAutoSpecialMarketing(gS.myID, true);
                    
                    anyStarted = true;
                    break;
                }
            }
            
            if (anyStarted)
            {
                sfx.PlaySound(3, true);
                guiMain.CloseMenu();
                menu.gameObject.SetActive(false);
            }
        }
        
        private static bool IsBeingProcessed(int gameId, int kampagneId)
        {
            GameObject[] tasks = GameObject.FindGameObjectsWithTag("Task");
            foreach (GameObject obj in tasks)
            {
                taskMarketingSpezial t = obj.GetComponent<taskMarketingSpezial>();
                if (t != null && t.targetID == gameId && t.kampagne == kampagneId)
                    return true;
            }
            return false;
        }
        
        public static void UpdateState(Menu_MarketingSpezial menu) { }
    }
}
