using HarmonyLib;
using MGT2AssistantButton.UI;

namespace MGT2AssistantButton.Patches
{
    [HarmonyPatch(typeof(Menu_Marketing_GameKampagne))]
    public class Patch_Menu_Marketing_GameKampagne
    {
        [HarmonyPatch("Init")]
        [HarmonyPostfix]
        public static void Init_Postfix(Menu_Marketing_GameKampagne __instance)
        {
            MarketingCampaignUI.InitGame(__instance);
        }
    }
    
    [HarmonyPatch(typeof(Menu_Marketing_KonsoleKampagne))]
    public class Patch_Menu_Marketing_KonsoleKampagne
    {
        [HarmonyPatch("Init")]
        [HarmonyPostfix]
        public static void Init_Postfix(Menu_Marketing_KonsoleKampagne __instance)
        {
            MarketingCampaignUI.InitConsole(__instance);
        }
    }
}
