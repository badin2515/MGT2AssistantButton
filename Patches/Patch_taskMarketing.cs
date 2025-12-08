using HarmonyLib;
using MGT2AssistantButton.Core;
using UnityEngine;

namespace MGT2AssistantButton.Patches
{
    [HarmonyPatch(typeof(taskMarketing))]
    public class Patch_taskMarketing
    {
        /// <summary>
        /// Before Complete() runs, check if we should switch campaigns
        /// </summary>
        [HarmonyPatch("Complete")]
        [HarmonyPrefix]
        public static void Complete_Prefix(taskMarketing __instance)
        {
            if (!MarketingAutomationManager.IsAutoSwitch(__instance.myID))
                return;
                
            if (__instance.typ != 0) return; // Only game marketing
            
            gameScript gS = __instance.gS_;
            if (gS == null) return;
            
            // Get menu for campaign info
            GUI_Main guiMain = GameObject.Find("CanvasInGameMenu").GetComponent<GUI_Main>();
            Menu_Marketing_GameKampagne menu = guiMain.uiObjects[89].GetComponent<Menu_Marketing_GameKampagne>();
            
            float currentHype = gS.hype;
            float hypeGain = menu.hypeProKampagne[__instance.kampagne];
            float maxHype = menu.maxHype[__instance.kampagne];
            
            // After this campaign completes, will we hit the max or 90?
            float afterHype = currentHype + hypeGain;
            if (afterHype > maxHype)
                afterHype = maxHype;
            
            // Store info for Postfix - stop at 90 to save money (for Overhype)
            _needsSwitch = afterHype >= maxHype && afterHype < 90f;
            _afterHype = afterHype;
            _taskId = __instance.myID;
            _gameId = gS.myID;
        }
        
        private static bool _needsSwitch = false;
        private static float _afterHype = 0f;
        private static int _taskId = -1;
        private static int _gameId = -1;
        
        [HarmonyPatch("Complete")]
        [HarmonyPostfix]
        public static void Complete_Postfix(taskMarketing __instance)
        {
            if (!_needsSwitch || _taskId != __instance.myID)
                return;
            
            _needsSwitch = false;
            
            // Find next campaign
            TryStartNextCampaign(__instance, _afterHype);
            
            // Clean up auto-switch for old task  
            MarketingAutomationManager.SetAutoSwitch(__instance.myID, false);
        }
        
        private static void TryStartNextCampaign(taskMarketing oldTask, float currentHype)
        {
            if (currentHype >= 90f) return;
            
            GameObject mainObj = GameObject.FindGameObjectWithTag("Main");
            mainScript mS = mainObj.GetComponent<mainScript>();
            unlockScript unlock = mainObj.GetComponent<unlockScript>();
            GUI_Main guiMain = GameObject.Find("CanvasInGameMenu").GetComponent<GUI_Main>();
            Menu_Marketing_GameKampagne menu = guiMain.uiObjects[89].GetComponent<Menu_Marketing_GameKampagne>();
            
            // Find next campaign that can add hype
            int nextCampaign = -1;
            int[] campaignOrder = { 0, 1, 2, 3, 4, 5 };
            
            foreach (int i in campaignOrder)
            {
                if (i == 3 && !unlock.Get(56)) continue;
                if (i == 4 && !unlock.Get(57)) continue;
                
                if (currentHype < menu.maxHype[i])
                {
                    nextCampaign = i;
                    break;
                }
            }
            
            if (nextCampaign == -1) return;
            
            // Check money
            long price = menu.preise[nextCampaign];
            if (mS.NotEnoughMoney(price)) return;
            
            // Find the room that was working on this task
            roomScript rS = null;
            foreach (GameObject roomObj in GameObject.FindGameObjectsWithTag("Room"))
            {
                roomScript rs = roomObj.GetComponent<roomScript>();
                if (rs && rs.taskID == oldTask.myID)
                {
                    rS = rs;
                    break;
                }
            }
            
            if (rS == null) return;
            
            // Pay and start new task
            mS.Pay(price, 12);
            
            taskMarketing newTask = guiMain.AddTask_Marketing();
            newTask.Init(false);
            newTask.targetID = oldTask.gS_.myID;
            newTask.typ = 0;
            newTask.kampagne = nextCampaign;
            newTask.automatic = true;
            newTask.stopAutomatic = false;
            newTask.disableWarten = true;
            newTask.points = (float)menu.workPoints[nextCampaign];
            newTask.pointsLeft = newTask.points;
            
            rS.taskID = newTask.myID;
            
            MarketingAutomationManager.SetAutoSwitch(newTask.myID, true);
        }
    }
}
