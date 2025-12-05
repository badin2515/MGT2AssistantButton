using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace MGT2AssistantButton.Core.Handlers
{
    public static class PlatformHandler
    {
        public static void ApplyPlatformMode(Menu_DevGame menu, PlatformMode mode)
        {
            switch (mode)
            {
                case PlatformMode.ByMarket: ApplyByMarket(menu); break;
                case PlatformMode.ConsoleOnly: ApplyConsoleOnly(menu); break;
                case PlatformMode.PCOnly: ApplyPCOnly(menu); break;
                case PlatformMode.OurConsoleFirst: ApplyOurConsoleFirst(menu); break;
                case PlatformMode.HighestTechOnly: ApplyHighestTechOnly(menu); break;
            }
        }

        public static void ApplyBestPlatform(Menu_DevGame menu) => ApplyByMarket(menu);

        private static bool IsPlatformValid(platformScript pS) =>
            pS != null && pS.isUnlocked && pS.inBesitz;

        private static bool IsButtonInteractable(GameObject obj)
        {
            if (obj == null) return false;
            var btn = obj.GetComponent<Button>();
            return btn != null && btn.interactable;
        }

        private static List<int> GetAvailableSlots(Menu_DevGame menu)
        {
            var slots = new List<int>();
            if (IsButtonInteractable(menu.uiObjects[140])) slots.Add(0);
            if (IsButtonInteractable(menu.uiObjects[33])) slots.Add(1);
            if (IsButtonInteractable(menu.uiObjects[34])) slots.Add(2);
            if (IsButtonInteractable(menu.uiObjects[35])) slots.Add(3);
            return slots;
        }

        private static bool MatchesGameModeFilter(platformScript pS, int pType)
        {
            if (pType == 5) return pS.typ == 3;
            if (pType == 4) return pS.typ == 4 && !pS.vomMarktGenommen;
            if (pType == 3) return pS.vomMarktGenommen && (pS.typ == 0 || pS.typ == 1 || pS.typ == 2);
            
            if (!pS.vomMarktGenommen && (pS.typ == 0 || pS.typ == 1 || pS.typ == 2))
            {
                if (pType == 2)
                {
                    GameObject mainObj = GameObject.Find("Main");
                    if (mainObj != null)
                    {
                        mainScript mS = mainObj.GetComponent<mainScript>();
                        if (mS != null) return pS.ownerID == mS.myID;
                    }
                    return false;
                }
                return true;
            }
            return false;
        }

        private class PlatformData
        {
            public platformScript platform;
            public int id;
            public float score;
        }

        private static void ClearPlatforms(Menu_DevGame menu)
        {
            for (int slot = 0; slot < 4; slot++) menu.SetPlatform(slot, -1);
        }

        private static int GetPlatformType(Menu_DevGame menu)
        {
            if (menu.uiObjects != null && menu.uiObjects.Length > 146)
            {
                var dropdown = menu.uiObjects[146]?.GetComponent<Dropdown>();
                if (dropdown != null) return dropdown.value;
            }
            return 0;
        }

        private static void ApplyByMarket(Menu_DevGame menu)
        {
            ClearPlatforms(menu);
            var availableSlots = GetAvailableSlots(menu);
            if (availableSlots.Count == 0) return;

            int pType = GetPlatformType(menu);
            var candidates = Object.FindObjectsOfType<platformScript>()
                .Where(pS => IsPlatformValid(pS) && MatchesGameModeFilter(pS, pType))
                .Select(pS => new PlatformData { platform = pS, id = pS.myID, score = pS.GetMarktanteil() })
                .OrderByDescending(x => x.score)
                .ToList();

            int count = Mathf.Min(availableSlots.Count, candidates.Count);
            for (int i = 0; i < count; i++)
                menu.SetPlatform(availableSlots[i], candidates[i].id);
        }

        private static void ApplyConsoleOnly(Menu_DevGame menu)
        {
            ClearPlatforms(menu);
            var availableSlots = GetAvailableSlots(menu);
            if (availableSlots.Count == 0) return;

            int pType = GetPlatformType(menu);
            var candidates = Object.FindObjectsOfType<platformScript>()
                .Where(pS => IsPlatformValid(pS) && (pS.typ == 1 || pS.typ == 2) && MatchesGameModeFilter(pS, pType))
                .Select(pS => new PlatformData { platform = pS, id = pS.myID, score = pS.GetMarktanteil() })
                .OrderByDescending(x => x.score)
                .ToList();

            int count = Mathf.Min(availableSlots.Count, candidates.Count);
            for (int i = 0; i < count; i++)
                menu.SetPlatform(availableSlots[i], candidates[i].id);
        }

        private static void ApplyPCOnly(Menu_DevGame menu)
        {
            ClearPlatforms(menu);
            var availableSlots = GetAvailableSlots(menu);
            if (availableSlots.Count == 0) return;

            int pType = GetPlatformType(menu);
            var candidates = Object.FindObjectsOfType<platformScript>()
                .Where(pS => IsPlatformValid(pS) && pS.typ == 0 && MatchesGameModeFilter(pS, pType))
                .Select(pS => new PlatformData { platform = pS, id = pS.myID, score = pS.GetMarktanteil() })
                .OrderByDescending(x => x.score)
                .ToList();

            int count = Mathf.Min(availableSlots.Count, candidates.Count);
            for (int i = 0; i < count; i++)
                menu.SetPlatform(availableSlots[i], candidates[i].id);
        }

        private static void ApplyOurConsoleFirst(Menu_DevGame menu)
        {
            ClearPlatforms(menu);
            var availableSlots = GetAvailableSlots(menu);
            if (availableSlots.Count == 0) return;

            GameObject mainObj = GameObject.Find("Main");
            if (mainObj == null) return;
            mainScript mS = mainObj.GetComponent<mainScript>();
            if (mS == null) return;

            int pType = GetPlatformType(menu);
            var candidates = Object.FindObjectsOfType<platformScript>()
                .Where(pS => IsPlatformValid(pS) && MatchesGameModeFilter(pS, pType))
                .Select(pS => new PlatformData { 
                    platform = pS, 
                    id = pS.myID, 
                    score = pS.GetMarktanteil() + (pS.ownerID == mS.myID ? 10000f : 0f)
                })
                .OrderByDescending(x => x.score)
                .ToList();

            int count = Mathf.Min(availableSlots.Count, candidates.Count);
            for (int i = 0; i < count; i++)
                menu.SetPlatform(availableSlots[i], candidates[i].id);
        }

        private static void ApplyHighestTechOnly(Menu_DevGame menu)
        {
            ClearPlatforms(menu);
            var availableSlots = GetAvailableSlots(menu);
            if (availableSlots.Count == 0) return;

            int pType = GetPlatformType(menu);
            var allPlatforms = Object.FindObjectsOfType<platformScript>()
                .Where(pS => IsPlatformValid(pS) && MatchesGameModeFilter(pS, pType))
                .Select(pS => new PlatformData { platform = pS, id = pS.myID, score = pS.GetMarktanteil() })
                .ToList();

            if (allPlatforms.Count == 0) return;

            int maxTech = allPlatforms.Max(p => p.platform.tech);
            var maxTechPlatforms = allPlatforms
                .Where(p => p.platform.tech == maxTech)
                .OrderByDescending(x => x.score)
                .ToList();

            int count = Mathf.Min(availableSlots.Count, maxTechPlatforms.Count);
            for (int i = 0; i < count; i++)
                menu.SetPlatform(availableSlots[i], maxTechPlatforms[i].id);
        }
    }
}
