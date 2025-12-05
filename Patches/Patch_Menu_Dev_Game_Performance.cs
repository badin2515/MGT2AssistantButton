using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

namespace MGT2AssistantButton.Patches
{
    /// <summary>
    /// High-performance platform caching with IL Transpiler
    /// Replaces all GameObject.Find calls with PlatformCache.GetPlatform
    /// Based on MGT2QoL's proven implementation
    /// </summary>
    public static class PlatformCache
    {
        private static Dictionary<string, GameObject> cache = new Dictionary<string, GameObject>();
        private static Dictionary<string, platformScript> scriptCache = new Dictionary<string, platformScript>();
        private static Dictionary<GameObject, platformScript> goToScriptCache = new Dictionary<GameObject, platformScript>();

        public static bool IsEmpty => cache.Count == 0;

        public static void Clear()
        {
            cache.Clear();
            scriptCache.Clear();
            goToScriptCache.Clear();
        }

        public static GameObject GetPlatform(string name)
        {
            if (cache.TryGetValue(name, out var result))
            {
                return result;
            }

            // Fallback (shouldn't happen after Refresh)
            Plugin.Logger.LogWarning($"[Cache] MISS: {name}");
            var go = GameObject.Find(name);
            if (go != null)
            {
                cache[name] = go;
                var script = go.GetComponent<platformScript>();
                if (script != null)
                {
                    scriptCache[name] = script;
                    goToScriptCache[go] = script;
                }
            }
            return go;
        }

        public static platformScript GetPlatformScript(string name)
        {
            if (scriptCache.TryGetValue(name, out var script))
            {
                return script;
            }

            var go = GetPlatform(name);
            return go?.GetComponent<platformScript>();
        }

        public static platformScript GetScriptFromGO(GameObject go)
        {
            if (go == null) return null;
            if (goToScriptCache.TryGetValue(go, out var script))
            {
                return script;
            }
            return go.GetComponent<platformScript>();
        }

        public static void Refresh(mainScript mS = null)
        {
            if (!IsEmpty) return;

            if (mS == null)
            {
                var mainObj = GameObject.FindGameObjectWithTag("Main");
                if (mainObj != null)
                {
                    mS = mainObj.GetComponent<mainScript>();
                }
            }

            if (mS != null && mS.arrayPlatformsScripts != null)
            {
                int count = 0;
                foreach (var script in mS.arrayPlatformsScripts)
                {
                    if (script != null && script.gameObject != null)
                    {
                        string name = script.gameObject.name;
                        cache[name] = script.gameObject;
                        scriptCache[name] = script;
                        goToScriptCache[script.gameObject] = script;
                        count++;
                    }
                }

                return;
            }

            Plugin.Logger.LogWarning("[Cache] Failed to refresh");
        }
    }

    // ============ Transpiler Patches ============

    [HarmonyPatch(typeof(Menu_DevGame))]
    public static class Patch_MenuDevGame_Transpilers
    {
        /// <summary>
        /// Generic transpiler to replace GameObject.Find with PlatformCache.GetPlatform
        /// </summary>
        private static IEnumerable<CodeInstruction> ReplaceFindWithCache(IEnumerable<CodeInstruction> instructions, string methodName)
        {
            var codes = new List<CodeInstruction>(instructions);
            int replacedCount = 0;

            var findMethod = AccessTools.Method(typeof(GameObject), "Find", new System.Type[] { typeof(string) });
            var cacheMethod = AccessTools.Method(typeof(PlatformCache), nameof(PlatformCache.GetPlatform));

            for (int i = 0; i < codes.Count; i++)
            {
                var inst = codes[i];

                // Replace GameObject.Find with PlatformCache.GetPlatform
                if (inst.opcode == OpCodes.Call && 
                    inst.operand is MethodInfo method &&
                    method == findMethod)
                {
                    inst.operand = cacheMethod;
                    replacedCount++;
                }

                yield return inst;
            }


        }

        [HarmonyPatch("SetPlatform")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler_SetPlatform(IEnumerable<CodeInstruction> instructions)
        {
            return ReplaceFindWithCache(instructions, "SetPlatform");
        }

        [HarmonyPatch("GetGesamtMarktanteil")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler_GetGesamtMarktanteil(IEnumerable<CodeInstruction> instructions)
        {
            return ReplaceFindWithCache(instructions, "GetGesamtMarktanteil");
        }

        [HarmonyPatch("GetLowestPlatformTechLevel")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler_GetLowestPlatformTechLevel(IEnumerable<CodeInstruction> instructions)
        {
            return ReplaceFindWithCache(instructions, "GetLowestPlatformTechLevel");
        }

        [HarmonyPatch("GetPlatformTechLevel")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler_GetPlatformTechLevel(IEnumerable<CodeInstruction> instructions)
        {
            return ReplaceFindWithCache(instructions, "GetPlatformTechLevel");
        }

        [HarmonyPatch("GetExklusivGameBonus")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler_GetExklusivGameBonus(IEnumerable<CodeInstruction> instructions)
        {
            return ReplaceFindWithCache(instructions, "GetExklusivGameBonus");
        }

        [HarmonyPatch("DROPDOWN_PlattformTyp")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler_DROPDOWN_PlattformTyp(IEnumerable<CodeInstruction> instructions)
        {
            return ReplaceFindWithCache(instructions, "DROPDOWN_PlattformTyp");
        }

        [HarmonyPatch("CalcDevCosts_Sonstiges")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler_CalcDevCosts_Sonstiges(IEnumerable<CodeInstruction> instructions)
        {
            return ReplaceFindWithCache(instructions, "CalcDevCosts_Sonstiges");
        }

        [HarmonyPatch("GetGesamtDevPoints")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler_GetGesamtDevPoints(IEnumerable<CodeInstruction> instructions)
        {
            return ReplaceFindWithCache(instructions, "GetGesamtDevPoints");
        }
    }

    // ============ Cache Management ============

    [HarmonyPatch(typeof(Menu_DevGame), "FindScripts")]
    public class Patch_FindScripts_RefreshCache
    {
        [HarmonyPostfix]
        public static void Postfix(Menu_DevGame __instance)
        {
            if (PlatformCache.IsEmpty)
            {
                var mS = AccessTools.Field(typeof(Menu_DevGame), "mS_").GetValue(__instance) as mainScript;
                PlatformCache.Refresh(mS);
            }
        }
    }

    [HarmonyPatch(typeof(Menu_DevGame), "OnDisable")]
    public class Patch_OnDisable_ClearCache
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            PlatformCache.Clear();
        }
    }
}
