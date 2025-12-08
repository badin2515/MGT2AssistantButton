using UnityEngine;
using MGT2AssistantButton.Core;

namespace MGT2AssistantButton.Core.Handlers
{
    public static class MarketingSpecialHandler
    {
        public static void TryStartNextTask(taskMarketingSpezial completedTask)
        {
            if (!MarketingAutomationManager.IsAutoSpecialMarketing(completedTask.targetID))
                return;

            gameScript gS = completedTask.gS_;
            if (gS == null) return;
            
            // If game is no longer in development or schublade, stop automation
            if (!gS.inDevelopment && !gS.schublade)
            {
                MarketingAutomationManager.SetAutoSpecialMarketing(gS.myID, false);
                return;
            }

            // Determine the next task (0 -> 1 -> 2)
            int nextCampaignId = -1;

            // Check sequential order - only if not already done
            for (int i = 0; i <= 2; i++)
            {
                if (gS.specialMarketing[i] == 0)
                {
                    // Additional checks for each task type
                    if (i == 2 && gS.hype < 90f) continue; // Overhype needs 90+ hype
                    if ((i == 0 || i == 1) && !gS.GetEinePlattformReleased()) continue; // Beta/Press need platform
                    
                    nextCampaignId = i;
                    break;
                }
            }

            if (nextCampaignId == -1)
            {
                MarketingAutomationManager.SetAutoSpecialMarketing(gS.myID, false);
                return;
            }
             
            GameObject guiMainObj = GameObject.Find("CanvasInGameMenu");
            if (guiMainObj == null) return;
            GUI_Main guiMain = guiMainObj.GetComponent<GUI_Main>();
            if (guiMain == null) return;
             
            GameObject menuObj = guiMain.uiObjects[294];
            Menu_MarketingSpezial menuScript = menuObj.GetComponent<Menu_MarketingSpezial>();

            if (menuScript == null)
            {
                return;
            }
            
            long price = menuScript.preise[nextCampaignId];
            
            GameObject mainObj = GameObject.FindGameObjectWithTag("Main");
            mainScript mainS = mainObj.GetComponent<mainScript>();

            if (mainS.NotEnoughMoney(price))
            {
                MarketingAutomationManager.SetAutoSpecialMarketing(gS.myID, false);
                return;
            }

            mainS.Pay(price, 12);
            
            taskMarketingSpezial newTask = guiMain.AddTask_MarketingSpezial();
            newTask.Init(false);
            newTask.targetID = gS.myID;
            newTask.kampagne = nextCampaignId;
            newTask.points = (float)menuScript.workPoints[nextCampaignId];
            newTask.pointsLeft = newTask.points;
            
            // Find room that was working on the completed task
            foreach (GameObject roomObj in GameObject.FindGameObjectsWithTag("Room"))
            {
                roomScript rS = roomObj.GetComponent<roomScript>();
                if (rS && rS.taskID == completedTask.myID)
                {
                    rS.taskID = newTask.myID;
                    break;
                }
            }
        }
    }
}
