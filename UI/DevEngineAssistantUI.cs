using UnityEngine;
using UnityEngine.UI;
using MGT2AssistantButton.Helpers;
using MGT2AssistantButton.Core.Handlers;
using MGT2AssistantButton.Core;
using System.Collections.Generic;

namespace MGT2AssistantButton.UI
{
    /// <summary>
    /// UI สำหรับปุ่ม Assistant ในหน้า Develop new engine
    /// </summary>
    public static class DevEngineAssistantUI
    {
        // References to created buttons
        private static GameObject nameButton;
        private static GameObject genreButton;
        private static GameObject platformButton;
        private static GameObject featureButton;
        private static GameObject marketButton;

        private static Menu_Dev_Engine currentMenu;
        private static bool buttonsCreated = false;

        public static Menu_Dev_Engine GetMenu() => currentMenu;

        public static void Init(Menu_Dev_Engine menu)
        {
            try
            {
                currentMenu = menu;
                
                // If buttons already exist and are valid, don't recreate
                if (buttonsCreated && genreButton != null) return;

                // Cleanup any stale references
                DestroyExistingButtons();

                // Find a template button
                GameObject sourceButton = FindSourceButton(menu);
                if (sourceButton == null)
                {
                    Plugin.Logger.LogWarning("Could not find source button for DevEngine menu!");
                    return;
                }

                CreateButtons(sourceButton, menu);
                buttonsCreated = true;
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"Error creating dev engine buttons: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private static void CreateButtons(GameObject sourceButton, Menu_Dev_Engine menu)
        {
            float spacing = 10f; // ระยะห่างจาก UI target

            // 1. Name Button (ซ้าย)
            RectTransform nameRect = GetRect(menu.uiObjects, 4);
            nameButton = CreateAndPosition(sourceButton, "Button_Assist_EngineName", nameRect, spacing, 
               null, new Vector2(295f, 193f), menu.transform, isLeft: true);
            if (nameButton != null)
            {
                var btn = nameButton.GetComponent<Button>();
                if (btn != null)
                {
                    ButtonHelper.AddDropdown(btn, new System.Collections.Generic.List<DropdownItem> {
                        new DropdownItem("Random Real Name", () => OnNameButtonClick())
                    }, 0, true); // onLeft = true
                }
                ButtonHelper.SetTooltip(nameButton, "Random Real Engine Name (Right-click for options)");
            }

            // 2. Genre Button (ซ้าย)
            RectTransform genreRect = GetRect(menu.uiObjects, 15);
            genreButton = CreateAndPosition(sourceButton, "Button_Assist_EngineGenre", genreRect, spacing, 
                null, new Vector2(-95f, -89f), menu.transform, isLeft: true);
            if (genreButton != null)
            {
                var btn = genreButton.GetComponent<Button>();
                if (btn != null)
                {
                    ButtonHelper.AddDropdown(btn, new System.Collections.Generic.List<DropdownItem> {
                        new DropdownItem("Auto-select Best Genre", () => OnGenreButtonClick())
                    }, 0, true); // onLeft = true
                }
                ButtonHelper.SetTooltip(genreButton, "Auto-select Best Genre (Right-click for options)");
            }

            // 3. Platform Button (ขวา)
            RectTransform platformRect = FindPlatformTarget(menu);
            platformButton = CreateAndPosition(sourceButton, "Button_Assist_EnginePlatform", platformRect, spacing, 
                null, new Vector2(295f, -89f), menu.transform);
            if (platformButton != null)
            {
                var btn = platformButton.GetComponent<Button>();
                if (btn != null)
                {
                    ButtonHelper.AddDropdown(btn, new System.Collections.Generic.List<DropdownItem> {
                        new DropdownItem("Auto-select Best Platform", () => OnPlatformButtonClick())
                    });
                }
                ButtonHelper.SetTooltip(platformButton, "Auto-select Best Platform (Right-click for options)");
            }

            // 4. Feature Button (ขวา)
            RectTransform featureRect = FindFeatureTarget(menu);
            featureButton = CreateAndPosition(sourceButton, "Button_Assist_EngineFeature", featureRect, spacing, 
                null, new Vector2(295f, 72f), menu.transform);
            if (featureButton != null)
            {
                var btn = featureButton.GetComponent<Button>();
                if (btn != null)
                {
                    ButtonHelper.AddDropdown(btn, new System.Collections.Generic.List<DropdownItem> {
                        new DropdownItem("Auto-select Best Features", () => OnFeatureButtonClick())
                    });
                }
                ButtonHelper.SetTooltip(featureButton, "Auto-select Best Features (Right-click for options)");
            }

            // 5. Market Button (ขวา)
            RectTransform saleToggleRect = FindSaleToggle(menu);
            Vector2 marketPos = new Vector2(295f, -200f);
            RectTransform targetRect = saleToggleRect != null ? saleToggleRect : GetRect(menu.uiObjects, 2);
            
            marketButton = CreateAndPosition(sourceButton, "Button_Assist_Market", targetRect, spacing, 
                null, marketPos, menu.transform);
            if (marketButton != null)
            {
                var btn = marketButton.GetComponent<Button>();
                if (btn != null)
                {
                    ButtonHelper.AddDropdown(btn, new System.Collections.Generic.List<DropdownItem> {
                        new DropdownItem("Auto-configure Market Settings", () => OnMarketButtonClick())
                    });
                }
                ButtonHelper.SetTooltip(marketButton, "Auto-configure Market Settings (Right-click for options)");
            }
        }



        private static GameObject CreateAndPosition(GameObject source, string name, RectTransform target, float spacing, UnityEngine.Events.UnityAction onClick, Vector2 fallbackPos, Transform fallbackParent, bool isLeft = false)
        {
            Transform parent = (target != null) ? target.parent : fallbackParent;
            
            GameObject btn = ButtonHelper.CloneBasicButton(source, parent, name);
            if (btn == null) return null;

            RectTransform btnRect = btn.GetComponent<RectTransform>();
            
            // Setup Icon
            SetupIcon(btn);

            if (target != null)
            {
                // ใช้การจัดตำแหน่งแบบสัมพัทธ์กับ Target
                btnRect.anchorMin = target.anchorMin;
                btnRect.anchorMax = target.anchorMax;
                btnRect.pivot = new Vector2(0.5f, 0.5f); // Center pivot for the button

                // คำนวณตำแหน่ง X
                float xOffset;
                float targetWidth = target.rect.width;

                if (isLeft)
                {
                    // Position to the LEFT of the target
                    xOffset = -(targetWidth * target.pivot.x) - (35f / 2f) - spacing;
                }
                else
                {
                    // Position to the RIGHT of the target
                    xOffset = (targetWidth * (1.0f - target.pivot.x)) + (35f / 2f) + spacing;
                }
                
                btnRect.anchoredPosition = new Vector2(
                    target.anchoredPosition.x + xOffset,
                    target.anchoredPosition.y
                );
            }
            else
            {
                // Fallback position
                btnRect.anchorMin = new Vector2(0.5f, 0.5f);
                btnRect.anchorMax = new Vector2(0.5f, 0.5f);
                btnRect.pivot = new Vector2(0.5f, 0.5f);
                btnRect.anchoredPosition = fallbackPos;
            }

            // Fix size
            btnRect.sizeDelta = new Vector2(35f, 35f);
            
            // Add click listener
            ButtonHelper.AddButtonComponent(btn, onClick);

            return btn;
        }

        private static void SetupIcon(GameObject btn)
        {
            // Load and add icon as child
            string iconPath = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), 
                "ButtonIcon.png");
            Sprite iconSprite = SpriteHelper.LoadSprite(iconPath);
            
            if (iconSprite != null)
            {
                GameObject iconObj = new GameObject("Icon");
                iconObj.transform.SetParent(btn.transform, false);

                Image iconImage = iconObj.AddComponent<Image>();
                iconImage.sprite = iconSprite;
                iconImage.preserveAspect = true;
                iconImage.raycastTarget = false;

                RectTransform iconRect = iconObj.GetComponent<RectTransform>();
                iconRect.anchorMin = new Vector2(0.5f, 0.5f);
                iconRect.anchorMax = new Vector2(0.5f, 0.5f);
                iconRect.pivot = new Vector2(0.5f, 0.5f);
                iconRect.anchoredPosition = Vector2.zero;
                iconRect.sizeDelta = new Vector2(28f, 28f);
            }
        }

        private static RectTransform GetRect(GameObject[] uiObjects, int index)
        {
            if (uiObjects != null && uiObjects.Length > index && uiObjects[index] != null)
            {
                return uiObjects[index].GetComponent<RectTransform>();
            }
            return null;
        }

        private static RectTransform FindPlatformTarget(Menu_Dev_Engine menu)
        {
            // Try to find Platform Container from uiObjects[17] (TextPlatform)
            if (menu.uiObjects != null && menu.uiObjects.Length > 17 && menu.uiObjects[17] != null)
            {
                Transform t = menu.uiObjects[17].transform;
                if (t.parent != null)
                {
                    // Check if parent is a container (larger than text)
                    RectTransform parentRect = t.parent.GetComponent<RectTransform>();
                    if (parentRect != null && parentRect.rect.width > 100)
                    {
                        return parentRect;
                    }
                }
                return t.GetComponent<RectTransform>();
            }
            return null;
        }

        private static RectTransform FindFeatureTarget(Menu_Dev_Engine menu)
        {
            // Based on user screenshot:
            // [11] Button_EngineFeatures
            
            // 1. Try uiObjects[11] if it matches name
            if (menu.uiObjects != null && menu.uiObjects.Length > 11 && menu.uiObjects[11] != null)
            {
                if (menu.uiObjects[11].name.Contains("Button_EngineFeatures"))
                {
                    return menu.uiObjects[11].GetComponent<RectTransform>();
                }
            }

            // 2. Search exact name "Button_EngineFeatures"
            Transform[] children = menu.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child.name == "Button_EngineFeatures")
                {
                    return child.GetComponent<RectTransform>();
                }
            }

            // 3. Fallback: Search for "Button_Feature" or "Button_Games" (Legacy/Safety)
            foreach (Transform child in children)
            {
                if (child.name.Contains("Button_Feature") || child.name.Contains("Button_Games")) 
                {
                    RectTransform r = child.GetComponent<RectTransform>();
                    // Ensure it's in the upper section (avoiding market dominance icon)
                    if (r != null && r.anchoredPosition.y > -50) 
                    {
                        return r;
                    }
                }
            }
            
            return null;
        }

        private static GameObject FindSourceButton(Menu_Dev_Engine menu)
        {
            // 1. Try to find Button_Desc from Menu_DevGame
            try
            {
                Menu_DevGame[] devGames = Resources.FindObjectsOfTypeAll<Menu_DevGame>();
                if (devGames != null)
                {
                    foreach (var devGame in devGames)
                    {
                        if (devGame == null) continue;
                        
                        // Check uiObjects
                        if (devGame.uiObjects != null)
                        {
                            foreach (GameObject obj in devGame.uiObjects)
                            {
                                if (obj != null && obj.name.Contains("Button_Desc")) return obj;
                            }
                        }
                        
                        // Check children
                        Transform[] children = devGame.GetComponentsInChildren<Transform>(true);
                        foreach (Transform child in children)
                        {
                             if (child.name.Contains("Button_Desc") && child.GetComponent<Image>() != null) 
                                return child.gameObject;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogWarning($"Error finding Button_Desc in Menu_DevGame: {ex.Message}");
            }

            // 2. Fallback: Search in current menu
            if (menu.uiObjects != null)
            {
                foreach (GameObject obj in menu.uiObjects)
                {
                    if (obj != null && (obj.name.Contains("Button_Random") || obj.name.Contains("RandomName")))
                    {
                        if (obj.GetComponent<Image>() != null) return obj;
                    }
                }
                
                foreach (GameObject obj in menu.uiObjects)
                {
                    if (obj != null && obj.name.Contains("Button") && obj.GetComponent<Image>() != null) return obj;
                }
            }

            return null;
        }

        public static void DestroyExistingButtons()
        {
            if (nameButton) Object.Destroy(nameButton);
            if (genreButton) Object.Destroy(genreButton);
            if (platformButton) Object.Destroy(platformButton);
            if (featureButton) Object.Destroy(featureButton);
            if (marketButton) Object.Destroy(marketButton);

            nameButton = null;
            genreButton = null;
            platformButton = null;
            featureButton = null;
            marketButton = null;
            buttonsCreated = false;
        }

        private static RectTransform FindSaleToggle(Menu_Dev_Engine menu)
        {
            // Try to find Toggle_Verkauf [25] or similar
            if (menu.uiObjects != null && menu.uiObjects.Length > 25 && menu.uiObjects[25] != null)
            {
                 return menu.uiObjects[25].GetComponent<RectTransform>();
            }

            // Fallback: search by name
            if (menu.uiObjects != null)
            {
                foreach (GameObject obj in menu.uiObjects)
                {
                    if (obj != null && (obj.name.Contains("Toggle_Verkauf") || obj.name.Contains("Verkauf") || obj.name.Contains("Sale")))
                    {
                        if (obj.GetComponent<Toggle>() != null)
                            return obj.GetComponent<RectTransform>();
                    }
                }
            }

            return null;
        }

        // --- Event Handlers ---

        private static void OnNameButtonClick()
        {
            if (currentMenu == null) return;
            DevEngineHandler.ApplyRealEngineName(currentMenu);
        }

        private static void OnGenreButtonClick()
        {
            if (currentMenu == null) return;
            DevEngineHandler.ApplyBestGenre(currentMenu);
        }

        private static void OnPlatformButtonClick()
        {
            if (currentMenu == null) return;
            DevEngineHandler.ApplyBestPlatform(currentMenu);
        }

        private static void OnFeatureButtonClick()
        {
            if (currentMenu == null) return;
            DevEngineHandler.ApplyBestFeatures(currentMenu);
        }

        private static void OnMarketButtonClick()
        {
            if (currentMenu == null) return;
            DevEngineHandler.ApplyMarketSettings(currentMenu);
        }
    }
}
