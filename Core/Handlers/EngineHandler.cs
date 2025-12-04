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
            Plugin.Logger.LogInfo("Core: Applying Best Engine Features...");
            // TODO: Implement engine features selection
        }
    }
}
