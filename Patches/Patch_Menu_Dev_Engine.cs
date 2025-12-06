using HarmonyLib;
using MGT2AssistantButton.UI;

namespace MGT2AssistantButton.Patches
{
    /// <summary>
    /// Patch สำหรับเมนูสร้าง Engine - เพิ่มปุ่ม Assistant
    /// </summary>
    [HarmonyPatch(typeof(Menu_Dev_Engine), "Init")]
    public class Patch_Menu_Dev_Engine_Init
    {
        [HarmonyPostfix]
        public static void Postfix(Menu_Dev_Engine __instance)
        {
            DevEngineAssistantUI.Init(__instance);
        }
    }

    [HarmonyPatch(typeof(Menu_Dev_Engine), "InitUpdateEngine")]
    public class Patch_Menu_Dev_Engine_Update
    {
        [HarmonyPostfix]
        public static void Postfix(Menu_Dev_Engine __instance)
        {
            DevEngineAssistantUI.Init(__instance);
        }
    }
}
