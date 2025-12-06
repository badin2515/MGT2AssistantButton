using UnityEngine;
using UnityEngine.UI;

namespace MGT2AssistantButton.Core.Handlers
{
    /// <summary>
    /// Handler สำหรับ logic การเลือกค่าต่างๆ ใน Menu_Dev_Engine
    /// </summary>
    public static class DevEngineHandler
    {
        /// <summary>
        /// สร้างชื่อ Engine แบบสุ่ม (Old version)
        /// </summary>
        public static void ApplyRandomName(Menu_Dev_Engine menu)
        {
            if (menu == null) return;

            try
            {
                // เรียกใช้ method ที่มีอยู่แล้วใน menu
                menu.BUTTON_RandomEngineName();
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"Error applying random engine name: {ex.Message}");
            }
        }

        /// <summary>
        /// สุ่มชื่อ Engine จากรายชื่อ Engine จริง
        /// </summary>
        public static void ApplyRealEngineName(Menu_Dev_Engine menu)
        {
            if (menu == null) return;

            try
            {
                string[] realEngineNames = new string[] {
                    "Unreal Engine", "Unity", "CryEngine", "Godot", "Frostbite", 
                    "Source", "id Tech", "RE Engine", "GameMaker", "Construct",
                    "RAGE", "Anvil", "Dunia", "Redengine", "Decima", 
                    "Luminous", "Essence", "Creation", "IW Engine", "Snowdrop",
                    "Havok", "PhyreEngine", "X-Ray", "Torque", "Ogre",
                    "Panda3D", "Lumberyard", "Defold", "Cocos2d", "MonoGame",
                    "RPG Maker", "Ren'Py", "Twine", "Stencyl", "GDevelop"
                };

                string randomName = realEngineNames[Random.Range(0, realEngineNames.Length)];
                
                // Find InputField for Engine Name (usually uiObjects[0] or [1] or check children)
                // In Menu_Dev_Engine, uiObjects[4] is usually the InputFieldName
                if (menu.uiObjects != null && menu.uiObjects.Length > 4)
                {
                    InputField input = menu.uiObjects[4]?.GetComponent<InputField>();
                    if (input != null)
                    {
                        input.text = randomName;
                        return; // Done
                    }
                }
                
                // Fallback: Find InputField in children
                InputField[] inputs = menu.GetComponentsInChildren<InputField>(true);
                if (inputs != null && inputs.Length > 0)
                {
                    inputs[0].text = randomName;
                }
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"Error applying real engine name: {ex.Message}");
            }
        }

        /// <summary>
        /// เลือก Genre ที่ดีที่สุดสำหรับ Engine
        /// พิจารณาจาก: Genre ที่มี popularity สูง และ player มี experience สูง
        /// </summary>
        public static void ApplyBestGenre(Menu_Dev_Engine menu)
        {
            if (menu == null) return;

            try
            {
                GameObject mainObj = GameObject.Find("Main");
                if (mainObj == null) return;

                genres genresScript = mainObj.GetComponent<genres>();
                if (genresScript == null) return;

                int bestGenreId = -1;
                int highestScore = -1;

                // วนหาทุก genre ที่ unlock แล้ว
                for (int i = 0; i < genresScript.genres_UNLOCK.Length; i++)
                {
                    if (!genresScript.genres_UNLOCK[i]) continue;

                    // คำนวณ score จาก level (experience) - ยิ่งมี experience สูง ยิ่งดี
                    int level = genresScript.genres_LEVEL[i];
                    int score = level * 10;

                    if (score > highestScore)
                    {
                        highestScore = score;
                        bestGenreId = i;
                    }
                }

                if (bestGenreId >= 0)
                {
                    menu.SetSpezialgenre(bestGenreId);
                }
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"Error applying best genre: {ex.Message}");
            }
        }

        /// <summary>
        /// เลือก Platform ที่ดีที่สุดสำหรับ Engine
        /// พิจารณาจาก: Tech level สูงสุด, มี DevKit, และ experience สูง
        /// </summary>
        public static void ApplyBestPlatform(Menu_Dev_Engine menu)
        {
            if (menu == null) return;

            try
            {
                GameObject mainObj = GameObject.Find("Main");
                if (mainObj == null) return;

                mainScript mS = mainObj.GetComponent<mainScript>();
                if (mS == null) return;

                GameObject[] platformObjects = GameObject.FindGameObjectsWithTag("Platform");
                int bestPlatformId = -1;
                int highestScore = -1;

                foreach (GameObject platformObj in platformObjects)
                {
                    if (platformObj == null) continue;
                    platformScript platform = platformObj.GetComponent<platformScript>();
                    if (platform == null) continue;

                    // ตรวจสอบว่า unlock และมี DevKit (inBesitz = true หมายความว่าซื้อ DevKit แล้ว)
                    if (!platform.isUnlocked) continue;
                    if (!platform.inBesitz) continue;

                    // คำนวณ score: tech level + experience + market share
                    int tech = platform.tech;
                    int experience = platform.erfahrung;
                    float marketShare = platform.GetMarktanteil();

                    int score = (tech * 20) + (experience * 5) + (int)(marketShare * 10);

                    if (score > highestScore)
                    {
                        highestScore = score;
                        bestPlatformId = platform.myID;
                    }
                }

                if (bestPlatformId >= 0)
                {
                    menu.SetSpezialplatform(bestPlatformId);
                }
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"Error applying best platform: {ex.Message}");
            }
        }

        /// <summary>
        /// เลือก Engine Features ที่ดีที่สุด
        /// เลือกทุก feature ที่ unlock แล้ว และ tech level ไม่เกิน platform ที่เลือก
        /// </summary>
        public static void ApplyBestFeatures(Menu_Dev_Engine menu)
        {
            if (menu == null) return;

            try
            {
                GameObject mainObj = GameObject.Find("Main");
                if (mainObj == null) return;

                engineFeatures eF = mainObj.GetComponent<engineFeatures>();
                if (eF == null) return;

                // หา tech level ของ platform ที่เลือก
                int maxTechLevel = 99;
                if (menu.spezialplatform >= 0)
                {
                    GameObject platformObj = GameObject.Find("PLATFORM_" + menu.spezialplatform.ToString());
                    if (platformObj != null)
                    {
                        platformScript platform = platformObj.GetComponent<platformScript>();
                        if (platform != null)
                        {
                            maxTechLevel = platform.tech;
                        }
                    }
                }

                // วนหาทุก feature type และเลือก feature ที่ tech level สูงสุดที่ไม่เกิน platform
                for (int i = 0; i < eF.engineFeatures_UNLOCK.Length; i++)
                {
                    // ตรวจสอบว่า unlock แล้ว
                    if (!eF.engineFeatures_UNLOCK[i]) continue;

                    // ตรวจสอบว่า tech level ไม่เกิน platform
                    if (eF.engineFeatures_TECH[i] > maxTechLevel) continue;

                    // ตรวจสอบว่ายังไม่ได้เลือก (ไม่ใช่ locked feature)
                    if (menu.featuresLock[i]) continue;

                    // เลือก feature นี้
                    menu.SetFeature(i, true);
                }
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"Error applying best features: {ex.Message}");
            }
        }

        /// <summary>
        /// ตั้งค่า Market settings อัตโนมัติ
        /// - เปิด Offer engine for sale
        /// - ตั้งราคาและ profit sharing ที่เหมาะสม
        /// </summary>
        public static void ApplyMarketSettings(Menu_Dev_Engine menu)
        {
            if (menu == null) return;

            try
            {
                // เปิด Offer engine for sale
                if (menu.uiObjects != null && menu.uiObjects.Length > 13)
                {
                    Toggle sellToggle = menu.uiObjects[13]?.GetComponent<Toggle>();
                    if (sellToggle != null)
                    {
                        sellToggle.isOn = true;
                    }
                }

                // ตั้งราคา - คำนวณจาก tech level และจำนวน features
                int techLevel = menu.techLevel;
                int featureCount = menu.featureAnzahl;
                
                // สูตรราคา: base price + (tech * multiplier) + (features * bonus)
                int suggestedPrice = 50 + (techLevel * 30) + (featureCount * 10);
                suggestedPrice = Mathf.Clamp(suggestedPrice, 10, 500); // Max 500k

                if (menu.uiObjects != null && menu.uiObjects.Length > 2)
                {
                    Slider priceSlider = menu.uiObjects[2]?.GetComponent<Slider>();
                    if (priceSlider != null)
                    {
                        priceSlider.value = suggestedPrice;
                        menu.SLIDER_Preis();
                    }
                }

                // ตั้ง Profit sharing - 10-20% เป็นค่าเริ่มต้นที่ดี
                int suggestedProfit = 15;

                if (menu.uiObjects != null && menu.uiObjects.Length > 3)
                {
                    Slider profitSlider = menu.uiObjects[3]?.GetComponent<Slider>();
                    if (profitSlider != null)
                    {
                        profitSlider.value = suggestedProfit;
                        menu.SLIDER_Gewinnbeteiligung();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"Error applying market settings: {ex.Message}");
            }
        }
    }
}
