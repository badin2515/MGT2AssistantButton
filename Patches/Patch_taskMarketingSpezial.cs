using HarmonyLib;
using MGT2AssistantButton.Core.Handlers;
using UnityEngine;

namespace MGT2AssistantButton.Patches
{
    [HarmonyPatch(typeof(taskMarketingSpezial))]
    public class Patch_taskMarketingSpezial
    {
        [HarmonyPatch("Complete")]
        [HarmonyPostfix]
        public static void Complete_Postfix(taskMarketingSpezial __instance)
        {
            // Try to start the next task in sequence
            MarketingSpecialHandler.TryStartNextTask(__instance);
        }
    }
}
