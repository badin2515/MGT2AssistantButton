using UnityEngine;
using UnityEngine.UI;

namespace MGT2AssistantButton.Core.Handlers
{
    public static class EngineHandler
    {
        public static void ApplyBestEngine(Menu_DevGame menu)
        {
            if (menu == null) return;

            try
            {
                if (menu.uiObjects != null && menu.uiObjects.Length > 126)
                {
                    Button engineButton = menu.uiObjects[126]?.GetComponent<Button>();
                    if (engineButton != null && !engineButton.interactable) return;
                }
                
                GameObject mainObj = GameObject.Find("Main");
                if (mainObj == null) return;

                mainScript mS = mainObj.GetComponent<mainScript>();
                if (mS == null) return;

                int selectedGenre = menu.g_GameMainGenre;
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

                    bool isAvailable = (engine.isUnlocked && engine.gekauft) ||
                                       (engine.ownerID == mS.myID && engine.devPointsStart <= 0.0f) ||
                                       (engine.ownerID == mS.myID && engine.updating);

                    if (!isAvailable || engine.archiv_engine) continue;

                    int engineTechLevel = engine.GetTechLevel();
                    bool matchesGenre = (selectedGenre >= 0 && engine.spezialgenre == selectedGenre);
                    bool isOwn = (engine.ownerID == mS.myID);

                    bool shouldSelect = false;
                    if (engineTechLevel > highestTechLevel)
                        shouldSelect = true;
                    else if (engineTechLevel == highestTechLevel)
                    {
                        if (matchesGenre && !bestMatchesGenre)
                            shouldSelect = true;
                        else if (!bestMatchesGenre && isOwn && !bestIsOwn)
                            shouldSelect = true;
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
                    menu.SetEngine(bestEngine.myID);
            }
            catch (System.Exception) { }
        }

        public static void ApplyBestEngineFeatures(Menu_DevGame menu) => ApplyEngineFeatures(menu, EngineFeatureMode.Best);

        public static void ApplyEngineFeatures(Menu_DevGame menu, EngineFeatureMode mode)
        {
            if (menu == null || menu.g_GameEngineScript_ == null) return;

            try
            {
                GameObject mainObj = GameObject.Find("Main");
                if (mainObj == null) return;

                engineFeatures eF = mainObj.GetComponent<engineFeatures>();
                if (eF == null) return;

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

                for (int featureType = 0; featureType < menu.g_GameEngineFeature.Length; featureType++)
                {
                    int maxTechForType = -1;
                    for (int featureIdx = 0; featureIdx < menu.g_GameEngineScript_.features.Length; featureIdx++)
                    {
                        if (!menu.g_GameEngineScript_.features[featureIdx]) continue;
                        if (eF.engineFeatures_TYP[featureIdx] != featureType) continue;
                        if (eF.engineFeatures_TECH[featureIdx] > minPlatformTech) continue;

                        int techLevel = eF.engineFeatures_TECH[featureIdx];
                        if (techLevel > maxTechForType)
                            maxTechForType = techLevel;
                    }

                    if (maxTechForType < 0) continue;

                    int bestFeatureIndex = -1;
                    int bestDevCost = mode == EngineFeatureMode.Best ? -1 : int.MaxValue;

                    for (int featureIdx = 0; featureIdx < menu.g_GameEngineScript_.features.Length; featureIdx++)
                    {
                        if (!menu.g_GameEngineScript_.features[featureIdx]) continue;
                        if (eF.engineFeatures_TYP[featureIdx] != featureType) continue;
                        if (eF.engineFeatures_TECH[featureIdx] != maxTechForType) continue;

                        int devCost = eF.GetDevCosts(featureIdx);

                        if (mode == EngineFeatureMode.Best)
                        {
                            if (devCost > bestDevCost) { bestDevCost = devCost; bestFeatureIndex = featureIdx; }
                        }
                        else
                        {
                            if (devCost < bestDevCost) { bestDevCost = devCost; bestFeatureIndex = featureIdx; }
                        }
                    }

                    if (bestFeatureIndex >= 0)
                        menu.SetEngineFeatureSimple(featureType, bestFeatureIndex);
                }

                menu.GetGesamtDevPoints();
            }
            catch (System.Exception) { }
        }

        private static int GetPlatformTechLevel(int platformId)
        {
            GameObject platformObj = GameObject.Find("PLATFORM_" + platformId.ToString());
            if (platformObj != null)
            {
                platformScript platform = platformObj.GetComponent<platformScript>();
                if (platform != null) return platform.tech;
            }
            return 0;
        }
    }
}
