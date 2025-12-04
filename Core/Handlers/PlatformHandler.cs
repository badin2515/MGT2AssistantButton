using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace MGT2AssistantButton.Core.Handlers
{
    /// <summary>
    /// Handles platform selection using simplified and validated filtering logic
    /// </summary>
    public static class PlatformHandler
    {
        /// <summary>
        /// Apply platform selection based on mode
        /// </summary>
        public static void ApplyPlatformMode(Menu_DevGame menu, PlatformMode mode)
        {
            switch (mode)
            {
                case PlatformMode.ByMarket:
                    ApplyByMarket(menu);
                    break;
                case PlatformMode.ConsoleOnly:
                    ApplyConsoleOnly(menu);
                    break;
                case PlatformMode.PCOnly:
                    ApplyPCOnly(menu);
                    break;
                case PlatformMode.OurConsoleFirst:
                    ApplyOurConsoleFirst(menu);
                    break;
                case PlatformMode.HighestTechOnly:
                    ApplyHighestTechOnly(menu);
                    break;
            }
        }

        public static void ApplyBestPlatform(Menu_DevGame menu)
        {
            ApplyByMarket(menu);
        }

        /// <summary>
        /// Check if platform meets basic validity requirements
        /// </summary>
        private static bool IsPlatformValid(platformScript pS)
        {
            if (pS == null) return false;
            if (!pS.isUnlocked) return false;
            if (!pS.inBesitz) return false;
            return true;
        }

        /// <summary>
        /// Check if button is interactable
        /// </summary>
        private static bool IsButtonInteractable(GameObject obj)
        {
            if (obj == null) return false;
            var btn = obj.GetComponent<Button>();
            return btn != null && btn.interactable;
        }

        /// <summary>
        /// Get available platform slots
        /// </summary>
        private static List<int> GetAvailableSlots(Menu_DevGame menu)
        {
            var slots = new List<int>();
            if (IsButtonInteractable(menu.uiObjects[140])) slots.Add(0);
            if (IsButtonInteractable(menu.uiObjects[33])) slots.Add(1);
            if (IsButtonInteractable(menu.uiObjects[34])) slots.Add(2);
            if (IsButtonInteractable(menu.uiObjects[35])) slots.Add(3);
            return slots;
        }

        /// <summary>
        /// Check if platform matches game mode filtering (pType from dropdown)
        /// </summary>
        private static bool MatchesGameModeFilter(platformScript pS, int pType)
        {
            if (pType == 5) // Mobile
            {
                return pS.typ == 3;
            }
            else if (pType == 4) // Arcade
            {
                return pS.typ == 4 && !pS.vomMarktGenommen;
            }
            else if (pType == 3) // Retro
            {
                return pS.vomMarktGenommen && (pS.typ == 0 || pS.typ == 1 || pS.typ == 2);
            }
            else // Standard (pType == 0), Exclusive (pType == 2), etc.
            {
                if (!pS.vomMarktGenommen && (pS.typ == 0 || pS.typ == 1 || pS.typ == 2))
                {
                    if (pType == 2) // Manufacturer Exclusive
                    {
                        GameObject mainObj = GameObject.Find("Main");
                        if (mainObj != null)
                        {
                            mainScript mS = mainObj.GetComponent<mainScript>();
                            if (mS != null)
                            {
                                return pS.ownerID == mS.myID;
                            }
                        }
                        return false;
                    }
                    return true;
                }
                return false;
            }
        }

        // Helper class for platform data
        private class PlatformData
        {
            public platformScript platform;
            public int id;
            public float score;
        }

        /// <summary>
        /// Select platforms by market share (Best Market Share)
        /// </summary>
        private static void ApplyByMarket(Menu_DevGame menu)
        {
            Plugin.Logger.LogInfo("=== Best Market Share: Selecting Top Platforms ===");
            
            // Clear all platforms first
            for (int slot = 0; slot < 4; slot++)
            {
                menu.SetPlatform(slot, -1);
            }

            // Get available slots
            var availableSlots = GetAvailableSlots(menu);
            if (availableSlots.Count == 0)
            {
                Plugin.Logger.LogWarning("No available platform slots!");
                return;
            }

            // Get platform type from dropdown
            int pType = 0;
            if (menu.uiObjects != null && menu.uiObjects.Length > 146)
            {
                var dropdown = menu.uiObjects[146]?.GetComponent<Dropdown>();
                if (dropdown != null) pType = dropdown.value;
            }

            // Find all valid platforms
            var platformObjects = Object.FindObjectsOfType<platformScript>();
            var candidates = new List<PlatformData>();

            foreach (var pS in platformObjects)
            {
                if (!IsPlatformValid(pS)) continue;
                if (!MatchesGameModeFilter(pS, pType)) continue;

                candidates.Add(new PlatformData
                {
                    platform = pS,
                    id = pS.myID,
                    score = pS.GetMarktanteil()
                });
            }

            // Sort by market share
            candidates = candidates.OrderByDescending(x => x.score).ToList();

            // Apply selection
            int count = Mathf.Min(availableSlots.Count, candidates.Count);
            for (int i = 0; i < count; i++)
            {
                menu.SetPlatform(availableSlots[i], candidates[i].id);
                Plugin.Logger.LogInfo($"Slot {availableSlots[i]}: {candidates[i].platform.GetName()} (Market: {candidates[i].score:F1}%)");
            }
        }

        /// <summary>
        /// Select Console platforms by market share (Console Focus)
        /// </summary>
        private static void ApplyConsoleOnly(Menu_DevGame menu)
        {
            Plugin.Logger.LogInfo("=== Console Focus: Selecting Best Console Platforms ===");
            
            // Clear all platforms first
            for (int slot = 0; slot < 4; slot++)
            {
                menu.SetPlatform(slot, -1);
            }

            // Get available slots
            var availableSlots = GetAvailableSlots(menu);
            if (availableSlots.Count == 0) return;

            // Get platform type from dropdown
            int pType = 0;
            if (menu.uiObjects != null && menu.uiObjects.Length > 146)
            {
                var dropdown = menu.uiObjects[146]?.GetComponent<Dropdown>();
                if (dropdown != null) pType = dropdown.value;
            }

            // Find all valid platforms
            var platformObjects = Object.FindObjectsOfType<platformScript>();
            var candidates = new List<PlatformData>();

            foreach (var pS in platformObjects)
            {
                if (!IsPlatformValid(pS)) continue;
                
                // Console type only (typ == 1 or 2)
                if (pS.typ != 1 && pS.typ != 2) continue;
                
                if (!MatchesGameModeFilter(pS, pType)) continue;

                candidates.Add(new PlatformData
                {
                    platform = pS,
                    id = pS.myID,
                    score = pS.GetMarktanteil()
                });
            }

            // Sort by market share
            candidates = candidates.OrderByDescending(x => x.score).ToList();

            // Apply selection
            int count = Mathf.Min(availableSlots.Count, candidates.Count);
            for (int i = 0; i < count; i++)
            {
                menu.SetPlatform(availableSlots[i], candidates[i].id);
                Plugin.Logger.LogInfo($"Slot {availableSlots[i]}: {candidates[i].platform.GetName()} (Console, Market: {candidates[i].score:F1}%)");
            }
        }

        /// <summary>
        /// Select PC platforms by market share (PC Focus)
        /// </summary>
        private static void ApplyPCOnly(Menu_DevGame menu)
        {
            Plugin.Logger.LogInfo("=== PC Focus: Selecting Best PC Platforms ===");
            
            // Clear all platforms first
            for (int slot = 0; slot < 4; slot++)
            {
                menu.SetPlatform(slot, -1);
            }

            // Get available slots
            var availableSlots = GetAvailableSlots(menu);
            if (availableSlots.Count == 0) return;

            // Get platform type from dropdown
            int pType = 0;
            if (menu.uiObjects != null && menu.uiObjects.Length > 146)
            {
                var dropdown = menu.uiObjects[146]?.GetComponent<Dropdown>();
                if (dropdown != null) pType = dropdown.value;
            }

            // Find all valid platforms
            var platformObjects = Object.FindObjectsOfType<platformScript>();
            var candidates = new List<PlatformData>();

            foreach (var pS in platformObjects)
            {
                if (!IsPlatformValid(pS)) continue;
                
                // PC type only (typ == 0)
                if (pS.typ != 0) continue;
                
                if (!MatchesGameModeFilter(pS, pType)) continue;

                candidates.Add(new PlatformData
                {
                    platform = pS,
                    id = pS.myID,
                    score = pS.GetMarktanteil()
                });
            }

            // Sort by market share
            candidates = candidates.OrderByDescending(x => x.score).ToList();

            // Apply selection
            int count = Mathf.Min(availableSlots.Count, candidates.Count);
            for (int i = 0; i < count; i++)
            {
                menu.SetPlatform(availableSlots[i], candidates[i].id);
                Plugin.Logger.LogInfo($"Slot {availableSlots[i]}: {candidates[i].platform.GetName()} (PC, Market: {candidates[i].score:F1}%)");
            }
        }

        /// <summary>
        /// Select our own platforms first, then others by market (Own Platforms First)
        /// </summary>
        private static void ApplyOurConsoleFirst(Menu_DevGame menu)
        {
            Plugin.Logger.LogInfo("=== Own Platforms First: Prioritizing Owned Platforms ===");
            
            // Clear all platforms first
            for (int slot = 0; slot < 4; slot++)
            {
                menu.SetPlatform(slot, -1);
            }

            // Get available slots
            var availableSlots = GetAvailableSlots(menu);
            if (availableSlots.Count == 0) return;

            GameObject mainObj = GameObject.Find("Main");
            if (mainObj == null) return;

            mainScript mS = mainObj.GetComponent<mainScript>();
            if (mS == null) return;

            // Get platform type
            int pType = 0;
            if (menu.uiObjects != null && menu.uiObjects.Length > 146)
            {
                var dropdown = menu.uiObjects[146]?.GetComponent<Dropdown>();
                if (dropdown != null) pType = dropdown.value;
            }

            // Find all valid platforms
            var platformObjects = Object.FindObjectsOfType<platformScript>();
            var candidates = new List<PlatformData>();

            foreach (var pS in platformObjects)
            {
                if (!IsPlatformValid(pS)) continue;
                if (!MatchesGameModeFilter(pS, pType)) continue;

                float score = pS.GetMarktanteil();
                
                // Boost owned platforms
                if (pS.ownerID == mS.myID)
                {
                    score += 10000f; // Massive boost to ensure they come first
                }

                candidates.Add(new PlatformData
                {
                    platform = pS,
                    id = pS.myID,
                    score = score
                });
            }

            // Sort by score (owned platforms will be first)
            candidates = candidates.OrderByDescending(x => x.score).ToList();

            // Apply selection
            int count = Mathf.Min(availableSlots.Count, candidates.Count);
            for (int i = 0; i < count; i++)
            {
                menu.SetPlatform(availableSlots[i], candidates[i].id);
                string owner = candidates[i].platform.ownerID == mS.myID ? " [OWNED]" : "";
                Plugin.Logger.LogInfo($"Slot {availableSlots[i]}: {candidates[i].platform.GetName()}{owner} (Market: {candidates[i].platform.GetMarktanteil():F1}%)");
            }
        }

        /// <summary>
        /// Select only platforms with the HIGHEST tech level - no lower tech allowed (Highest Tech Only)
        /// </summary>
        private static void ApplyHighestTechOnly(Menu_DevGame menu)
        {
            Plugin.Logger.LogInfo("=== Highest Tech Only: Selecting Maximum Tech Platforms ===");
            
            // Clear all platforms first
            for (int slot = 0; slot < 4; slot++)
            {
                menu.SetPlatform(slot, -1);
            }

            // Get available slots
            var availableSlots = GetAvailableSlots(menu);
            if (availableSlots.Count == 0) return;

            // Get platform type from dropdown
            int pType = 0;
            if (menu.uiObjects != null && menu.uiObjects.Length > 146)
            {
                var dropdown = menu.uiObjects[146]?.GetComponent<Dropdown>();
                if (dropdown != null) pType = dropdown.value;
            }

            // Find all valid platforms
            var platformObjects = Object.FindObjectsOfType<platformScript>();
            var allPlatforms = new List<PlatformData>();

            foreach (var pS in platformObjects)
            {
                if (!IsPlatformValid(pS)) continue;
                if (!MatchesGameModeFilter(pS, pType)) continue;

                allPlatforms.Add(new PlatformData
                {
                    platform = pS,
                    id = pS.myID,
                    score = pS.GetMarktanteil()
                });
            }

            if (allPlatforms.Count == 0)
            {
                Plugin.Logger.LogWarning("No valid platforms found!");
                return;
            }

            // Find the MAXIMUM tech level
            int maxTech = allPlatforms.Max(p => p.platform.tech);
            Plugin.Logger.LogInfo($"Maximum tech level found: {maxTech}");

            // Filter to ONLY platforms with max tech
            var maxTechPlatforms = allPlatforms.Where(p => p.platform.tech == maxTech).ToList();
            Plugin.Logger.LogInfo($"Found {maxTechPlatforms.Count} platforms with tech level {maxTech}");

            // Sort by market share among max tech platforms
            maxTechPlatforms = maxTechPlatforms.OrderByDescending(x => x.score).ToList();

            // Apply selection
            int count = Mathf.Min(availableSlots.Count, maxTechPlatforms.Count);
            for (int i = 0; i < count; i++)
            {
                menu.SetPlatform(availableSlots[i], maxTechPlatforms[i].id);
                Plugin.Logger.LogInfo($"Slot {availableSlots[i]}: {maxTechPlatforms[i].platform.GetName()} (Tech: {maxTech}, Market: {maxTechPlatforms[i].score:F1}%)");
            }
        }
    }
}
