using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace MGT2AssistantButton.Patches
{
    /// <summary>
    /// Patch สำหรับล้างค่า platform เมื่อผู้เล่นเปลี่ยน dropdown type
    /// แต่ไม่ล้างตอน Init เพื่อไม่ให้หน่วง
    /// </summary>
    [HarmonyPatch(typeof(Menu_DevGame), "DROPDOWN_PlattformTyp")]
    public class Patch_Menu_Dev_Game_PlatformDropdown
    {
        private static bool isInitializing = false;
        private static int lastDropdownValue = -1;

        /// <summary>
        /// Track when Init starts
        /// </summary>
        [HarmonyPatch(typeof(Menu_DevGame), "Init")]
        [HarmonyPrefix]
        public static void Init_Prefix()
        {
            isInitializing = true;
            lastDropdownValue = -1;
        }

        /// <summary>
        /// Track when Init ends
        /// </summary>
        [HarmonyPatch(typeof(Menu_DevGame), "Init")]
        [HarmonyPostfix]
        public static void Init_Postfix()
        {
            isInitializing = false;
        }

        /// <summary>
        /// Prefix - ล้าง platform เมื่อผู้เล่นเปลี่ยน dropdown (ไม่ใช่ตอน Init)
        /// </summary>
        [HarmonyPrefix]
        public static void Prefix(Menu_DevGame __instance)
        {
            try
            {
                // ข้ามถ้ากำลัง Init
                if (isInitializing)
                {
                    return;
                }

                // ตรวจสอบว่ามี dropdown อยู่
                if (__instance.uiObjects == null || __instance.uiObjects.Length <= 146)
                    return;

                Dropdown platformDropdown = __instance.uiObjects[146]?.GetComponent<Dropdown>();
                if (platformDropdown == null)
                    return;

                int newValue = platformDropdown.value;

                // ถ้าเป็นการเรียกครั้งแรก หรือค่าไม่เปลี่ยน ไม่ต้องล้าง
                if (lastDropdownValue == -1 || lastDropdownValue == newValue)
                {
                    lastDropdownValue = newValue;
                    return;
                }

                // ค่าเปลี่ยนแล้ว! ล้าง platforms
                lastDropdownValue = newValue;

                int platformCount = 0;
                for (int i = 0; i < __instance.g_GamePlatform.Length; i++)
                {
                    if (__instance.g_GamePlatform[i] != -1)
                        platformCount++;
                }

                if (platformCount > 0)
                {
                    string typeName = GetPlatformTypeName(newValue);
                    Plugin.Logger.LogInfo($"User changed platform type to '{typeName}' - clearing {platformCount} platform(s)");

                    // ล้าง platform ทั้งหมด
                    for (int i = 0; i < __instance.g_GamePlatform.Length; i++)
                    {
                        if (__instance.g_GamePlatform[i] != -1)
                        {
                            __instance.SetPlatform(i, -1);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"Error in DROPDOWN_PlattformTyp Prefix: {ex.Message}");
            }
        }

        private static string GetPlatformTypeName(int type)
        {
            return type switch
            {
                0 => "All Platforms",
                1 => "Exclusive",
                2 => "Manufacturer",
                3 => "Retro",
                4 => "Arcade",
                5 => "Mobile/Handheld",
                _ => $"Unknown ({type})"
            };
        }
    }
}
