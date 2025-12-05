using UnityEngine;
using System.Collections.Generic;

namespace MGT2AssistantButton.Core.Handlers
{
    /// <summary>
    /// Handles Gameplay Features selection
    /// </summary>
    public static class GameplayFeatureHandler
    {
        // Helper class to store feature info
        private class FeatureInfo
        {
            public int Id;
            public float Bonus;
            public int DependencyCount;

            public FeatureInfo(int id, float bonus, int depCount)
            {
                Id = id;
                Bonus = bonus;
                DependencyCount = depCount;
            }
        }

        /// <summary>
        /// Apply gameplay features based on the selected mode
        /// </summary>
        public static void ApplyGameplayFeatures(Menu_DevGame menu, GameplayFeatureMode mode)
        {
            if (menu == null)
            {
                Plugin.Logger.LogError("Menu instance is null!");
                return;
            }

            try
            {
                Plugin.Logger.LogInfo($"=== Applying Gameplay Features Mode: {mode} ===");

                // Check prerequisites
                int mainGenre = menu.g_GameMainGenre;
                int subGenre = menu.g_GameSubGenre;
                int gameSize = menu.g_GameSize;

                if (mainGenre < 0)
                {
                    Plugin.Logger.LogWarning("Main Genre not selected - cannot apply gameplay features");
                    return;
                }

                // Get scripts
                GameObject mainObj = GameObject.Find("Main");
                if (mainObj == null)
                {
                    Plugin.Logger.LogError("Main GameObject not found!");
                    return;
                }

                gameplayFeatures gfScript = mainObj.GetComponent<gameplayFeatures>();
                if (gfScript == null)
                {
                    Plugin.Logger.LogError("gameplayFeatures script not found!");
                    return;
                }

                // Clear all current selections first
                ClearAllFeatures(menu);

                // Get platform requirements
                List<int> requiredFeatures = GetRequiredFeaturesFromPlatforms(menu, gfScript);
                Plugin.Logger.LogInfo($"  Platform required features: {requiredFeatures.Count}");

                // Get limit based on game size
                int maxFeatures = GetMaxFeatures(menu, gameSize);
                Plugin.Logger.LogInfo($"  Max features for size {gameSize}: {maxFeatures}");

                // Track selected features
                List<int> selectedFeatures = new List<int>();

                // Step 1: ALWAYS select required features from platforms (with dependencies)
                foreach (int featureId in requiredFeatures)
                {
                    SelectFeatureWithDependencies(menu, gfScript, featureId, selectedFeatures, maxFeatures);
                }
                Plugin.Logger.LogInfo($"  After required: {selectedFeatures.Count} features selected");

                // Step 2: Based on mode, add more features
                if (mode == GameplayFeatureMode.Best || mode == GameplayFeatureMode.All)
                {
                    // Add GOOD features first (1.5x bonus)
                    SelectFeaturesByBonus(menu, gfScript, mainGenre, subGenre, selectedFeatures, maxFeatures, 1.5f, 1.5f);
                    Plugin.Logger.LogInfo($"  After GOOD features: {selectedFeatures.Count} features selected");

                    // Add Neutral features (1.0x bonus)
                    SelectFeaturesByBonus(menu, gfScript, mainGenre, subGenre, selectedFeatures, maxFeatures, 1.0f, 1.0f);
                    Plugin.Logger.LogInfo($"  After Neutral features: {selectedFeatures.Count} features selected");

                    // For All mode, also add BAD features
                    if (mode == GameplayFeatureMode.All)
                    {
                        SelectFeaturesByBonus(menu, gfScript, mainGenre, subGenre, selectedFeatures, maxFeatures, 0.0f, 0.9f);
                        Plugin.Logger.LogInfo($"  After BAD features: {selectedFeatures.Count} features selected");
                    }
                }

                Plugin.Logger.LogInfo($"Total features selected: {selectedFeatures.Count}");
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"Error in ApplyGameplayFeatures: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Clear all gameplay features
        /// </summary>
        private static void ClearAllFeatures(Menu_DevGame menu)
        {
            for (int i = 0; i < menu.g_GameGameplayFeatures.Length; i++)
            {
                // Use the game's method to properly disable and update UI
                if (menu.g_GameGameplayFeatures[i])
                {
                    menu.DisableGameplayFeature(i);
                }
            }
        }

        /// <summary>
        /// Get features required by selected platforms
        /// </summary>
        private static List<int> GetRequiredFeaturesFromPlatforms(Menu_DevGame menu, gameplayFeatures gfScript)
        {
            List<int> required = new List<int>();

            for (int i = 0; i < menu.g_GamePlatform.Length; i++)
            {
                // g_GamePlatform contains platform IDs, -1 means not selected
                if (menu.g_GamePlatform[i] == -1) continue;

                // Find the platform object using the stored platform ID
                int platformId = menu.g_GamePlatform[i];
                GameObject platformObj = GameObject.Find("PLATFORM_" + platformId);
                if (platformObj == null) continue;

                platformScript pScript = platformObj.GetComponent<platformScript>();
                if (pScript == null || pScript.needFeatures == null) continue;

                // Add required features
                for (int j = 0; j < pScript.needFeatures.Length; j++)
                {
                    int featureId = pScript.needFeatures[j];
                    if (featureId != -1 && !required.Contains(featureId))
                    {
                        required.Add(featureId);
                        Plugin.Logger.LogInfo($"    Platform {pScript.GetName()} requires: {gfScript.GetName(featureId)}");
                    }
                }
            }

            return required;
        }

        /// <summary>
        /// Get max features based on game size
        /// </summary>
        private static int GetMaxFeatures(Menu_DevGame menu, int gameSize)
        {
            // maxFeatures_gameSize array from Menu_DevGame
            if (menu.maxFeatures_gameSize != null && gameSize < menu.maxFeatures_gameSize.Length)
            {
                return menu.maxFeatures_gameSize[gameSize];
            }
            // Default fallback values based on typical game sizes
            switch (gameSize)
            {
                case 0: return 3;   // Tiny
                case 1: return 5;   // Small
                case 2: return 8;   // Medium
                case 3: return 12;  // Large
                case 4: return 16;  // AAA
                case 5: return 999; // Massive (no limit)
                default: return 10;
            }
        }

        /// <summary>
        /// Select features within a specific bonus range
        /// </summary>
        private static void SelectFeaturesByBonus(Menu_DevGame menu, gameplayFeatures gfScript,
            int mainGenre, int subGenre, List<int> selectedFeatures, int maxFeatures,
            float minBonus, float maxBonus)
        {
            // Collect features in the bonus range
            List<FeatureInfo> features = new List<FeatureInfo>();

            for (int i = 0; i < gfScript.gameplayFeatures_UNLOCK.Length; i++)
            {
                // Skip already selected
                if (selectedFeatures.Contains(i)) continue;

                // Check bonus range
                float bonus = gfScript.GetBonus(i, mainGenre, subGenre);
                if (bonus < minBonus || bonus > maxBonus) continue;

                // Count dependencies
                int depCount = CountDependencies(gfScript, i, selectedFeatures);

                features.Add(new FeatureInfo(i, bonus, depCount));
            }

            // Sort by: fewest dependencies first, then by bonus (highest first)
            features.Sort((a, b) => {
                int depCompare = a.DependencyCount.CompareTo(b.DependencyCount);
                if (depCompare != 0) return depCompare;
                return b.Bonus.CompareTo(a.Bonus);
            });

            // Select features
            foreach (FeatureInfo featureInfo in features)
            {
                if (selectedFeatures.Count >= maxFeatures) break;

                // Check if we have room for this feature + its dependencies
                int roomNeeded = featureInfo.DependencyCount + 1;
                int roomLeft = maxFeatures - selectedFeatures.Count;

                if (roomNeeded > roomLeft) continue;

                SelectFeatureWithDependencies(menu, gfScript, featureInfo.Id, selectedFeatures, maxFeatures);
            }
        }

        /// <summary>
        /// Select a feature along with its dependencies
        /// </summary>
        private static bool SelectFeatureWithDependencies(Menu_DevGame menu, gameplayFeatures gfScript, 
            int featureId, List<int> selectedFeatures, int maxFeatures)
        {
            // Already selected
            if (selectedFeatures.Contains(featureId))
                return true;

            // Check if researched
            if (!gfScript.IsErforscht(featureId))
            {
                Plugin.Logger.LogInfo($"    Feature {featureId} not researched - skipping");
                return false;
            }

            // Check platform lock
            if (IsLockedByPlatform(menu, gfScript, featureId))
            {
                Plugin.Logger.LogInfo($"    Feature {featureId} locked by platform - skipping");
                return false;
            }

            // Check dependency
            int dependencyId = gfScript.gameplayFeatures_NEED_GAMEPLAY_FEATURE[featureId];
            if (dependencyId != -1)
            {
                // Need to select dependency first
                if (!SelectFeatureWithDependencies(menu, gfScript, dependencyId, selectedFeatures, maxFeatures))
                {
                    Plugin.Logger.LogInfo($"    Feature {featureId} dependency {dependencyId} failed - skipping");
                    return false;
                }
            }

            // Check if we have room
            if (selectedFeatures.Count >= maxFeatures)
            {
                Plugin.Logger.LogInfo($"    Feature {featureId} - limit reached - skipping");
                return false;
            }

            // Select the feature using the game's method (handles UI updates)
            // If it's currently off, toggle it on
            if (!menu.g_GameGameplayFeatures[featureId])
            {
                menu.SetGameplayFeature(featureId);
            }
            selectedFeatures.Add(featureId);
            Plugin.Logger.LogInfo($"    Selected feature {featureId}: {gfScript.GetName(featureId)}");

            return true;
        }

        /// <summary>
        /// Check if feature is locked by current platform selection
        /// </summary>
        private static bool IsLockedByPlatform(Menu_DevGame menu, gameplayFeatures gfScript, int featureId)
        {
            // Check if any selected platform locks this feature
            for (int i = 0; i < menu.g_GamePlatform.Length; i++)
            {
                if (menu.g_GamePlatform[i] == -1) continue;

                int platformId = menu.g_GamePlatform[i];
                GameObject platformObj = GameObject.Find("PLATFORM_" + platformId);
                if (platformObj == null) continue;

                platformScript pScript = platformObj.GetComponent<platformScript>();
                if (pScript == null) continue;

                // Platform types: 0=PC, 1=Console, 2=Handheld, 3=Mobile, 4=Arcade
                int platformType = pScript.typ;

                // Check if this feature is locked for this platform type
                if (gfScript.gameplayFeatures_LOCKPLATFORM[featureId, platformType])
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Count how many unselected dependencies a feature has
        /// </summary>
        private static int CountDependencies(gameplayFeatures gfScript, int featureId, List<int> selectedFeatures)
        {
            int count = 0;
            int current = featureId;

            while (true)
            {
                int dep = gfScript.gameplayFeatures_NEED_GAMEPLAY_FEATURE[current];
                if (dep == -1) break;
                if (selectedFeatures.Contains(dep)) break;

                count++;
                current = dep;

                // Safety check to prevent infinite loops
                if (count > 10) break;
            }

            return count;
        }
    }
}
