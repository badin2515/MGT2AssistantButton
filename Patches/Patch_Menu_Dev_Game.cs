using HarmonyLib;
using UnityEngine;
using MGT2AssistantButton.UI;

namespace MGT2AssistantButton.Patches
{
    /// <summary>
    /// Patch สำหรับเมนูสร้างเกม - เพิ่มปุ่ม Assistant
    /// </summary>
    [HarmonyPatch(typeof(Menu_DevGame), "Init")]
    public class Patch_Menu_Dev_Game_Init
    {
        [HarmonyPostfix]
        public static void Postfix(Menu_DevGame __instance)
        {
            DevGameAssistantUI.Init(__instance);
        }
    }
}
