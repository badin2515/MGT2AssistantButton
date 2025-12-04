using UnityEngine;
using UnityEngine.UI;

namespace MGT2AssistantButton.Core.Handlers
{
    /// <summary>
    /// Handles engine selection and engine features
    /// </summary>
    public static class EngineHandler
    {
        public static void ApplyBestEngine(Menu_DevGame menu)
        {
            if (menu == null)
            {
                Plugin.Logger.LogError("Menu instance is null!");
                return;
            }

            try
            {
                Plugin.Logger.LogInfo("=== Applying Best Engine ===");
                
                // Check if Engine button is interactable (index 126)
                if (menu.uiObjects != null && menu.uiObjects.Length > 126)
                {
                    Button engineButton = menu.uiObjects[126]?.GetComponent<Button>();
                    if (engineButton != null && !engineButton.interactable)
                    {
                        Plugin.Logger.LogInfo("Engine button is not interactable - skipping");
                        return;
                    }
                }
                
                GameObject mainObj = GameObject.Find("Main");
                if (mainObj == null)
                {
                    Plugin.Logger.LogError("Main GameObject not found!");
                    return;
                }

                mainScript mS = mainObj.GetComponent<mainScript>();
                if (mS == null)
                {
                    Plugin.Logger.LogError("mainScript not found!");
                    return;
                }

                int selectedGenre = menu.g_GameMainGenre;
                
                // Find all available engines
                GameObject[] engineObjects = GameObject.FindGameObjectsWithTag("Engine");
                engineScript bestEngine = null;
                int highestTechLevel = -1;
                bool bestMatchesGenre = false;
                bool bestIsOwn = false;

                foreach (GameObject engineObj in engineObjects)
                {
                    if (engineObj == null) continue;

                    engineScript engine = engineObj.GetComponent<engineScript>();
                    if (engine == null) continue;

                    // Check if engine is available (same logic as Menu_DevGame_Engine)
                    bool isAvailable = false;
                    
                    // Check: Unlocked and purchased
                    if (engine.isUnlocked && engine.gekauft)
                        isAvailable = true;
                    
                    // Check: Own engine and completed
                    if (engine.ownerID == mS.myID && engine.devPointsStart <= 0.0f)
                        isAvailable = true;
                    
                    // Check: Own engine and updating
                    if (engine.ownerID == mS.myID && engine.updating)
                        isAvailable = true;

                    // Skip if not available or archived
                    if (!isAvailable || engine.archiv_engine)
                        continue;

                    int engineTechLevel = engine.GetTechLevel();
                    bool matchesGenre = (selectedGenre >= 0 && engine.spezialgenre == selectedGenre);
                    bool isOwn = (engine.ownerID == mS.myID);

                    // Selection logic with priority:
                    // 1. Higher tech level is always best
                    // 2. Same tech level + genre match is better
                    // 3. Same tech level + no existing genre match -> prefer own engine
                    // 4. Otherwise pick any with highest tech level
                    bool shouldSelect = false;

                    if (engineTechLevel > highestTechLevel)
                    {
                        // New best tech level - always select
                        shouldSelect = true;
                    }
                    else if (engineTechLevel == highestTechLevel)
                    {
                        // Same tech level - apply priority rules
                        if (matchesGenre && !bestMatchesGenre)
                        {
                            // Genre match is better
                            shouldSelect = true;
                        }
                        else if (!bestMatchesGenre && isOwn && !bestIsOwn)
                        {
                            // No genre matches exist, prefer own engine
                            shouldSelect = true;
                        }
                    }

                    if (shouldSelect)
                    {
                        bestEngine = engine;
                        highestTechLevel = engineTechLevel;
                        bestMatchesGenre = matchesGenre;
                        bestIsOwn = isOwn;
                    }
                }

                if (bestEngine != null)
                {
                    menu.SetEngine(bestEngine.myID);
                    Plugin.Logger.LogInfo($"Selected engine: {bestEngine.GetName()} (Tech: {highestTechLevel}, Genre Match: {bestMatchesGenre}, Own: {bestIsOwn})");
                }
                else
                {
                    Plugin.Logger.LogWarning("No available engines found!");
                }
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"Error in ApplyBestEngine: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public static void ApplyBestEngineFeatures(Menu_DevGame menu)
        {
            ApplyEngineFeatures(menu, EngineFeatureMode.Best);
        }

        /// <summary>
        /// Apply engine features based on selected mode
        /// Both modes select features at max tech level supported by platform.
        /// Best: picks most expensive feature at that tech level
        /// Cheapest: picks least expensive feature at that tech level
        /// </summary>
        public static void ApplyEngineFeatures(Menu_DevGame menu, EngineFeatureMode mode)
        {
            if (menu == null)
            {
                Plugin.Logger.LogError("Menu instance is null!");
                return;
            }

            if (menu.g_GameEngineScript_ == null)
            {
                Plugin.Logger.LogWarning("No engine selected - cannot apply engine features");
                return;
            }

            try
            {
                string modeName = mode == EngineFeatureMode.Best ? "Best (Expensive)" : "Cheapest";
                Plugin.Logger.LogInfo($"=== Applying Engine Features: {modeName} Mode ===");

                // Get mainScript and engineFeatures
                GameObject mainObj = GameObject.Find("Main");
                if (mainObj == null)
                {
                    Plugin.Logger.LogError("Main GameObject not found!");
                    return;
                }

                engineFeatures eF = mainObj.GetComponent<engineFeatures>();
                if (eF == null)
                {
                    Plugin.Logger.LogError("engineFeatures component not found!");
                    return;
                }

                // Find minimum platform tech level (same as game's logic)
                int minPlatformTech = 99;
                for (int i = 0; i < menu.g_GamePlatform.Length; i++)
                {
                    if (menu.g_GamePlatform[i] != -1)
                    {
                        int platformTech = GetPlatformTechLevel(menu.g_GamePlatform[i]);
                        if (platformTech < minPlatformTech)
                            minPlatformTech = platformTech;
                    }
                }

                Plugin.Logger.LogInfo($"Max supported Tech Level: {minPlatformTech}");

                // For each feature type (0=Grafik, 1=Sound, 2=KI, 3=Physik)
                for (int featureType = 0; featureType < menu.g_GameEngineFeature.Length; featureType++)
                {
                    // Step 1: Find the max tech level available for this feature type
                    int maxTechForType = -1;
                    for (int featureIdx = 0; featureIdx < menu.g_GameEngineScript_.features.Length; featureIdx++)
                    {
                        if (!menu.g_GameEngineScript_.features[featureIdx])
                            continue;
                        if (eF.engineFeatures_TYP[featureIdx] != featureType)
                            continue;
                        if (eF.engineFeatures_TECH[featureIdx] > minPlatformTech)
                            continue;

                        int techLevel = eF.engineFeatures_TECH[featureIdx];
                        if (techLevel > maxTechForType)
                            maxTechForType = techLevel;
                    }

                    if (maxTechForType < 0)
                        continue; // No features available for this type

                    // Step 2: Find best/cheapest feature at that max tech level
                    int bestFeatureIndex = -1;
                    int bestDevCost = mode == EngineFeatureMode.Best ? -1 : int.MaxValue;

                    for (int featureIdx = 0; featureIdx < menu.g_GameEngineScript_.features.Length; featureIdx++)
                    {
                        if (!menu.g_GameEngineScript_.features[featureIdx])
                            continue;
                        if (eF.engineFeatures_TYP[featureIdx] != featureType)
                            continue;
                        // Only consider features at max tech level
                        if (eF.engineFeatures_TECH[featureIdx] != maxTechForType)
                            continue;

                        int devCost = eF.GetDevCosts(featureIdx);

                        if (mode == EngineFeatureMode.Best)
                        {
                            // Best mode: prefer most expensive at max tech level
                            if (devCost > bestDevCost)
                            {
                                bestDevCost = devCost;
                                bestFeatureIndex = featureIdx;
                            }
                        }
                        else // Cheapest mode
                        {
                            // Cheapest mode: prefer least expensive at max tech level
                            if (devCost < bestDevCost)
                            {
                                bestDevCost = devCost;
                                bestFeatureIndex = featureIdx;
                            }
                        }
                    }

                    // Set the feature if found
                    if (bestFeatureIndex >= 0)
                    {
                        menu.SetEngineFeatureSimple(featureType, bestFeatureIndex);
                        string typeName = featureType switch
                        {
                            0 => "Grafik",
                            1 => "Sound",
                            2 => "KI",
                            3 => "Physik",
                            _ => $"Type{featureType}"
                        };
                        
                        Plugin.Logger.LogInfo($"  {typeName}: {eF.GetName(bestFeatureIndex)} (Tech: {maxTechForType}, Cost: ${bestDevCost})");
                    }
                }

                // Recalculate dev points (the UI will update automatically)
                menu.GetGesamtDevPoints();

                Plugin.Logger.LogInfo("Engine features applied successfully");
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"Error in ApplyEngineFeatures: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Get platform tech level by looking up the platform GameObject
        /// Replicates the private Menu_DevGame.GetPlatformTechLevel method
        /// </summary>
        private static int GetPlatformTechLevel(int platformId)
        {
            GameObject platformObj = GameObject.Find("PLATFORM_" + platformId.ToString());
            if (platformObj != null)
            {
                platformScript platform = platformObj.GetComponent<platformScript>();
                if (platform != null)
                    return platform.tech;
            }
            return 0;
        }
    }
}
