using HarmonyLib;
using MGT2AssistantButton.UI;
using UnityEngine;

namespace MGT2AssistantButton.Patches
{
    [HarmonyPatch(typeof(Menu_MarketingSpezial))]
    public class Patch_Menu_MarketingSpezial
    {
        [HarmonyPatch("Init")]
        [HarmonyPostfix]
        public static void Init_Postfix(Menu_MarketingSpezial __instance)
        {
            MarketingSpecialUI.Init(__instance);
        }

        [HarmonyPatch("SetData")]
        [HarmonyPostfix]
        public static void SetData_Postfix(Menu_MarketingSpezial __instance)
        {
            MarketingSpecialUI.UpdateState(__instance);
        }
        
        // Also might need to update when game changes via SetGame
        [HarmonyPatch("SetGame")]
        [HarmonyPostfix]
        public static void SetGame_Postfix(Menu_MarketingSpezial __instance)
        {
            MarketingSpecialUI.UpdateState(__instance);
        }
    }
}
