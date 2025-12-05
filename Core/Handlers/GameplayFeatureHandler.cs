using UnityEngine;
using System.Collections.Generic;

namespace MGT2AssistantButton.Core.Handlers
{
    /// <summary>
    /// Handles Gameplay Features selection
    /// </summary>
    public static class GameplayFeatureHandler
    {
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

        public static void ApplyGameplayFeatures(Menu_DevGame menu, GameplayFeatureMode mode)
        {
            if (menu == null) return;

            try
            {
                int mainGenre = menu.g_GameMainGenre;
                int subGenre = menu.g_GameSubGenre;
                int gameSize = menu.g_GameSize;

                if (mainGenre < 0) return;

                GameObject mainObj = GameObject.Find("Main");
                if (mainObj == null) return;

                gameplayFeatures gfScript = mainObj.GetComponent<gameplayFeatures>();
                if (gfScript == null) return;

                ClearAllFeatures(menu);

                List<int> requiredFeatures = GetRequiredFeaturesFromPlatforms(menu, gfScript);
                int maxFeatures = GetMaxFeatures(menu, gameSize);
                List<int> selectedFeatures = new List<int>();

                // Step 1: Required features
                foreach (int featureId in requiredFeatures)
                {
                    SelectFeatureWithDependencies(menu, gfScript, featureId, selectedFeatures, maxFeatures);
                }

                // Step 2: Based on mode
                if (mode == GameplayFeatureMode.Best || mode == GameplayFeatureMode.All)
                {
                    SelectFeaturesByBonus(menu, gfScript, mainGenre, subGenre, selectedFeatures, maxFeatures, 1.5f, 1.5f);
                    SelectFeaturesByBonus(menu, gfScript, mainGenre, subGenre, selectedFeatures, maxFeatures, 1.0f, 1.0f);

                    if (mode == GameplayFeatureMode.All)
                    {
                        SelectFeaturesByBonus(menu, gfScript, mainGenre, subGenre, selectedFeatures, maxFeatures, 0.0f, 0.9f);
                    }
                }
            }
            catch (System.Exception) { }
        }

        private static void ClearAllFeatures(Menu_DevGame menu)
        {
            for (int i = 0; i < menu.g_GameGameplayFeatures.Length; i++)
            {
                if (menu.g_GameGameplayFeatures[i])
                {
                    menu.DisableGameplayFeature(i);
                }
            }
        }

        private static List<int> GetRequiredFeaturesFromPlatforms(Menu_DevGame menu, gameplayFeatures gfScript)
        {
            List<int> required = new List<int>();

            for (int i = 0; i < menu.g_GamePlatform.Length; i++)
            {
                if (menu.g_GamePlatform[i] == -1) continue;

                int platformId = menu.g_GamePlatform[i];
                GameObject platformObj = GameObject.Find("PLATFORM_" + platformId);
                if (platformObj == null) continue;

                platformScript pScript = platformObj.GetComponent<platformScript>();
                if (pScript == null || pScript.needFeatures == null) continue;

                for (int j = 0; j < pScript.needFeatures.Length; j++)
                {
                    int featureId = pScript.needFeatures[j];
                    if (featureId != -1 && !required.Contains(featureId))
                    {
                        required.Add(featureId);
                    }
                }
            }

            return required;
        }

        private static int GetMaxFeatures(Menu_DevGame menu, int gameSize)
        {
            if (menu.maxFeatures_gameSize != null && gameSize < menu.maxFeatures_gameSize.Length)
            {
                return menu.maxFeatures_gameSize[gameSize];
            }
            switch (gameSize)
            {
                case 0: return 3;
                case 1: return 5;
                case 2: return 8;
                case 3: return 12;
                case 4: return 16;
                case 5: return 999;
                default: return 10;
            }
        }

        private static void SelectFeaturesByBonus(Menu_DevGame menu, gameplayFeatures gfScript,
            int mainGenre, int subGenre, List<int> selectedFeatures, int maxFeatures,
            float minBonus, float maxBonus)
        {
            List<FeatureInfo> features = new List<FeatureInfo>();

            for (int i = 0; i < gfScript.gameplayFeatures_UNLOCK.Length; i++)
            {
                if (selectedFeatures.Contains(i)) continue;

                float bonus = gfScript.GetBonus(i, mainGenre, subGenre);
                if (bonus < minBonus || bonus > maxBonus) continue;

                int depCount = CountDependencies(gfScript, i, selectedFeatures);
                features.Add(new FeatureInfo(i, bonus, depCount));
            }

            features.Sort((a, b) => {
                int depCompare = a.DependencyCount.CompareTo(b.DependencyCount);
                if (depCompare != 0) return depCompare;
                return b.Bonus.CompareTo(a.Bonus);
            });

            foreach (FeatureInfo featureInfo in features)
            {
                if (selectedFeatures.Count >= maxFeatures) break;

                int roomNeeded = featureInfo.DependencyCount + 1;
                int roomLeft = maxFeatures - selectedFeatures.Count;

                if (roomNeeded > roomLeft) continue;

                SelectFeatureWithDependencies(menu, gfScript, featureInfo.Id, selectedFeatures, maxFeatures);
            }
        }

        private static bool SelectFeatureWithDependencies(Menu_DevGame menu, gameplayFeatures gfScript, 
            int featureId, List<int> selectedFeatures, int maxFeatures)
        {
            if (selectedFeatures.Contains(featureId)) return true;
            if (!gfScript.IsErforscht(featureId)) return false;
            if (IsLockedByPlatform(menu, gfScript, featureId)) return false;

            int dependencyId = gfScript.gameplayFeatures_NEED_GAMEPLAY_FEATURE[featureId];
            if (dependencyId != -1)
            {
                if (!SelectFeatureWithDependencies(menu, gfScript, dependencyId, selectedFeatures, maxFeatures))
                    return false;
            }

            if (selectedFeatures.Count >= maxFeatures) return false;

            if (!menu.g_GameGameplayFeatures[featureId])
            {
                menu.SetGameplayFeature(featureId);
            }
            selectedFeatures.Add(featureId);

            return true;
        }

        private static bool IsLockedByPlatform(Menu_DevGame menu, gameplayFeatures gfScript, int featureId)
        {
            for (int i = 0; i < menu.g_GamePlatform.Length; i++)
            {
                if (menu.g_GamePlatform[i] == -1) continue;

                int platformId = menu.g_GamePlatform[i];
                GameObject platformObj = GameObject.Find("PLATFORM_" + platformId);
                if (platformObj == null) continue;

                platformScript pScript = platformObj.GetComponent<platformScript>();
                if (pScript == null) continue;

                int platformType = pScript.typ;
                if (gfScript.gameplayFeatures_LOCKPLATFORM[featureId, platformType])
                {
                    return true;
                }
            }

            return false;
        }

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
                if (count > 10) break;
            }

            return count;
        }
    }
}
