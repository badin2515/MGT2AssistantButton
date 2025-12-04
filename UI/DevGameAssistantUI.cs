using UnityEngine;
using UnityEngine.UI;
using MGT2AssistantButton.Helpers;
using MGT2AssistantButton.Core;

namespace MGT2AssistantButton.UI
{
    public static class DevGameAssistantUI
    {
        // References to created buttons
        private static GameObject targetGroupButton;
        private static GameObject mainGenreButton;
        private static GameObject subgenreButton;
        private static GameObject mainThemeButton;
        private static GameObject subthemeButton;
        private static GameObject randomNameButton;
        private static GameObject engineButton;
        private static GameObject platformButton;
        private static GameObject engineFeatureButton;
        private static GameObject antiCheatButton;
        private static GameObject copyProtectButton;
        private static GameObject languageButton;
        private static GameObject sliderButton;
        private static GameObject gameplayFeatureButton;

        private static bool buttonsCreated = false;

        public static void Init(Menu_DevGame menu)
        {
            try
            {
                // Initialize Core with the menu instance
                MGT2AssistantButton.Core.AssistantCore.Initialize(menu);
                
                // If buttons already exist, just return (reuse them)
                if (buttonsCreated && targetGroupButton != null)
                {
                    return;
                }
                
                
                DestroyExistingButtons();
                
                // Find Source Buttons
                GameObject descSource = FindButtonDesc(menu);
                GameObject randomNameSource = FindButtonRandomName(menu);
                
                if (descSource == null)
                {
                    Plugin.Logger.LogError("Could not find Button_Desc source!");
                    return;
                }

                // Page 1
                Transform seite1 = FindPage(menu, "Seite1");
                if (seite1 != null)
                {
                    CreatePage1Buttons(descSource, randomNameSource, seite1);
                }

                // Page 2
                Transform seite2 = FindPage(menu, "Seite2");
                if (seite2 != null)
                {
                    CreatePage2Buttons(descSource, seite2);
                }

                // Page 3
                Transform seite3 = FindPage(menu, "Seite3");
                if (seite3 != null)
                {
                    CreatePage3Buttons(descSource, randomNameSource, seite3, menu);
                }

                // Page 4
                Transform seite4 = FindPage(menu, "Seite4");
                if (seite4 != null)
                {
                    CreatePage4Buttons(descSource, seite4, menu);
                }

                // Page 5
                Transform seite5 = FindPage(menu, "Seite5");
                if (seite5 != null)
                {
                    CreatePage5Buttons(descSource, seite5, menu);
                }

                buttonsCreated = true;
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"Error creating dev game buttons: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private static void CreatePage1Buttons(GameObject descSource, GameObject randomNameSource, Transform parent)
        {
            // Desc Style Buttons
            targetGroupButton = CreateDescButton(descSource, parent, "Button_Assist_TargetGroup", new Vector2(295f, 51f), OnTargetGroupButtonClick);
            mainGenreButton = CreateDescButton(descSource, parent, "Button_Assist_MainGenre", new Vector2(-310f, -33f), OnMainGenreButtonClick);
            subgenreButton = CreateDescButton(descSource, parent, "Button_Assist_Subgenre", new Vector2(295f, -33f), OnSubgenreButtonClick);
            mainThemeButton = CreateDescButton(descSource, parent, "Button_Assist_MainTheme", new Vector2(-310f, -130f), OnMainThemeButtonClick);
            subthemeButton = CreateDescButton(descSource, parent, "Button_Assist_Subtheme", new Vector2(295f, -130f), OnSubthemeButtonClick);

            // RandomName Style Button
            if (randomNameSource != null)
            {
                ModifyOriginalRandomNameButton(randomNameSource);
                randomNameButton = CreateRandomNameButton(randomNameSource, parent, "Button_Assist_RandomName", new Vector2(80f, 193f), "Real Name", OnRandomNameButtonClick);
            }
        }

        private static void CreatePage2Buttons(GameObject descSource, Transform parent)
        {
            engineButton = CreateDescButton(descSource, parent, "Button_Assist_Engine", new Vector2(240f, 235f), OnEngineButtonClick);
            
            // Platform button with dropdown - don't set onClick yet
            platformButton = CreateDescButton(descSource, parent, "Button_Assist_Platform", new Vector2(300f, -75f), null);
            
            // Add dropdown menu to platform button
            if (platformButton != null)
            {
                var button = platformButton.GetComponent<Button>();
                if (button != null)
                {
                    PlatformDropdownMenu.Create(button, (mode) => {
                        MGT2AssistantButton.Core.Handlers.PlatformHandler.ApplyPlatformMode(AssistantCore.GetMenu(), mode);
                    });
                }
            }
        }

        private static void CreatePage3Buttons(GameObject descSource, GameObject randomNameSource, Transform parent, Menu_DevGame menu)
        {
            engineFeatureButton = CreateDescButton(descSource, parent, "Button_Assist_EngineFeature", new Vector2(300f, 133f), OnEngineFeatureButtonClick);
            antiCheatButton = CreateDescButton(descSource, parent, "Button_Assist_AntiCheat", new Vector2(-320f, -203f), OnAntiCheatButtonClick);
            copyProtectButton = CreateDescButton(descSource, parent, "Button_Assist_CopyProtect", new Vector2(300f, -205f), OnCopyProtectButtonClick);

            if (randomNameSource != null)
            {
                // Language button with dropdown - don't set onClick directly
                languageButton = CreateRandomNameButton(randomNameSource, parent, "Button_Assist_Language", new Vector2(200f, -112f), "Language", null);
                
                // Add dropdown menu to language button
                if (languageButton != null)
                {
                    var button = languageButton.GetComponent<Button>();
                    if (button != null)
                    {
                        LanguageDropdownMenu.Create(button, (mode) => {
                            MGT2AssistantButton.Core.AssistantCore.ApplyLanguageMode(mode);
                        });
                    }
                }
            }

            GameObject alleSprachen = FindButtonByName(menu, "Button_AlleSprachen");
            if (alleSprachen != null) ModifyOriginalAlleSprachenButton(alleSprachen);
        }

        private static void CreatePage4Buttons(GameObject descSource, Transform parent, Menu_DevGame menu)
        {
            sliderButton = CreateDescButton(descSource, parent, "Button_Assist_Slider", new Vector2(235f, 30f), OnSliderButtonClick);
            
            // Slider button needs specific size adjustment if not 40x40
            if (sliderButton != null)
            {
                sliderButton.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 50f);
            }

            GameObject autoDesign = FindButtonByName(menu, "Button_AutoDesingSettings");
            if (autoDesign != null) ModifyOriginalAutoDesignSettingsButton(autoDesign);
        }

        private static void CreatePage5Buttons(GameObject descSource, Transform parent, Menu_DevGame menu)
        {
            // Gameplay Feature is now Desc style with custom icon
            // Updated position to (0, -259) based on user image
            gameplayFeatureButton = CreateDescButton(descSource, parent, "Button_Assist_GameplayFeature", new Vector2(0f, -259f), OnGameplayFeatureButtonClick);
        }

        // --- Helper Creation Methods ---

        private static GameObject CreateDescButton(GameObject source, Transform parent, string name, Vector2 pos, UnityEngine.Events.UnityAction onClick)
        {
            GameObject btn = ButtonHelper.CloneBasicButton(source, parent, name);
            if (btn != null)
            {
                RectTransform rect = btn.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = pos;
                rect.sizeDelta = new Vector2(40f, 40f);
                
                // Load and add icon as child
                string iconPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "ButtonIcon.png");
                Sprite iconSprite = SpriteHelper.LoadSprite(iconPath);
                if (iconSprite != null)
                {
                    // Create a new GameObject for the icon
                    GameObject iconObj = new GameObject("Icon");
                    iconObj.transform.SetParent(btn.transform, false);
                    
                    // Add Image component
                    Image iconImage = iconObj.AddComponent<Image>();
                    iconImage.sprite = iconSprite;
                    iconImage.preserveAspect = true; // Keep aspect ratio
                    iconImage.raycastTarget = false; // Let clicks pass through to the button
                    
                    // Center the icon
                    RectTransform iconRect = iconObj.GetComponent<RectTransform>();
                    iconRect.anchorMin = new Vector2(0.5f, 0.5f);
                    iconRect.anchorMax = new Vector2(0.5f, 0.5f);
                    iconRect.pivot = new Vector2(0.5f, 0.5f);
                    iconRect.anchoredPosition = Vector2.zero;
                    iconRect.sizeDelta = new Vector2(30f, 30f); // Slightly smaller than the button (40x40)
                }
                
                ButtonHelper.AddButtonComponent(btn, onClick);
                Plugin.Logger.LogInfo($"{name} created at {pos} with custom icon child");
            }
            return btn;
        }

        private static GameObject CreateRandomNameButton(GameObject source, Transform parent, string name, Vector2 pos, string text, UnityEngine.Events.UnityAction onClick)
        {
            GameObject btn = ButtonHelper.CloneButtonWithOutline(source, parent, name);
            if (btn != null)
            {
                RectTransform rect = btn.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = pos;
                rect.sizeDelta = new Vector2(150f, 23.25f);
                
                ButtonHelper.AddButtonComponent(btn, onClick);
                ButtonHelper.AddButtonText(btn, text, 14, Color.black);
                
                // Add Icon to distinguish as mod button
                string iconPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "ButtonIcon.png");
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
                    // Position icon on the left side
                    iconRect.anchorMin = new Vector2(0f, 0.5f); 
                    iconRect.anchorMax = new Vector2(0f, 0.5f);
                    iconRect.pivot = new Vector2(0f, 0.5f);
                    iconRect.anchoredPosition = new Vector2(5f, 0f); // 5px padding from left
                    iconRect.sizeDelta = new Vector2(18f, 18f); // Small icon
                }

                Plugin.Logger.LogInfo($"{name} created at {pos} with icon");
            }
            return btn;
        }

        // --- Cleanup ---

        public static void DestroyExistingButtons()
        {
            if (targetGroupButton) Object.Destroy(targetGroupButton);
            if (mainGenreButton) Object.Destroy(mainGenreButton);
            if (subgenreButton) Object.Destroy(subgenreButton);
            if (mainThemeButton) Object.Destroy(mainThemeButton);
            if (subthemeButton) Object.Destroy(subthemeButton);
            if (randomNameButton) Object.Destroy(randomNameButton);
            if (engineButton) Object.Destroy(engineButton);
            if (platformButton) Object.Destroy(platformButton);
            if (engineFeatureButton) Object.Destroy(engineFeatureButton);
            if (antiCheatButton) Object.Destroy(antiCheatButton);
            if (copyProtectButton) Object.Destroy(copyProtectButton);
            if (languageButton) Object.Destroy(languageButton);
            if (sliderButton) Object.Destroy(sliderButton);
            if (gameplayFeatureButton) Object.Destroy(gameplayFeatureButton);
        }

        // --- Finders ---

        private static Transform FindPage(Menu_DevGame menu, string pageName)
        {
            Transform[] children = menu.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child.name == pageName) return child;
            }
            Plugin.Logger.LogWarning($"{pageName} not found!");
            return null;
        }

        private static GameObject FindButtonDesc(Menu_DevGame menu)
        {
            if (menu.uiObjects != null)
            {
                foreach (GameObject obj in menu.uiObjects)
                {
                    if (obj != null && obj.name.Contains("Button_Desc")) return obj;
                }
            }
            
            Transform[] children = menu.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child.name.Contains("Button_Desc")) return child.gameObject;
            }

            // Fallback
            if (menu.uiObjects != null)
            {
                foreach (GameObject obj in menu.uiObjects)
                {
                    if (obj != null && obj.GetComponent<Image>() != null && obj.GetComponent<Shadow>() != null)
                    {
                        return obj;
                    }
                }
            }
            return null;
        }

        private static GameObject FindButtonRandomName(Menu_DevGame menu)
        {
            if (menu.uiObjects != null)
            {
                foreach (GameObject obj in menu.uiObjects)
                {
                    if (obj != null && obj.name.Contains("Button_RandomName")) return obj;
                }
            }
            
            Transform[] children = menu.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child.name.Contains("Button_RandomName") || child.name.Contains("RandomName")) return child.gameObject;
            }
            return null;
        }

        private static GameObject FindButtonByName(Menu_DevGame menu, string namePart)
        {
             Transform[] children = menu.GetComponentsInChildren<Transform>(true);
             foreach (Transform child in children)
             {
                 if (child.name.Contains(namePart)) return child.gameObject;
             }
             return null;
        }

        // --- Modifiers ---

        private static void ModifyOriginalRandomNameButton(GameObject originalButton)
        {
            RectTransform rect = originalButton.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = new Vector2(218f, -167.08f);
                rect.sizeDelta = new Vector2(150f, 23.25f);
            }
        }

        private static void ModifyOriginalAlleSprachenButton(GameObject originalButton)
        {
            RectTransform rect = originalButton.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.sizeDelta = new Vector2(240f, 23.25f);
            }
        }

        private static void ModifyOriginalAutoDesignSettingsButton(GameObject originalButton)
        {
            RectTransform rect = originalButton.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = new Vector2(490f, -328.4f);
            }
        }

        private static void ModifyOriginalAlleGameplayFeaturesButton(GameObject originalButton)
        {
            RectTransform rect = originalButton.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = new Vector2(-220f, -618.8f);
                rect.sizeDelta = new Vector2(200f, 23.25f);
            }
            ButtonHelper.SetButtonText(originalButton, "All Features");
        }

        private static void ModifyOriginalAllePassendenGameplayFeaturesButton(GameObject originalButton)
        {
            RectTransform rect = originalButton.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = new Vector2(220f, -618.8f);
                rect.sizeDelta = new Vector2(200f, 23.25f);
            }
            ButtonHelper.SetButtonText(originalButton, "Good Features");
        }

        // --- Event Handlers ---

        // --- Event Handlers ---

        private static void OnTargetGroupButtonClick() 
        { 
            AssistantCore.ApplyBestTargetGroup(); 
        }
        
        private static void OnMainGenreButtonClick() 
        { 
            AssistantCore.ApplyBestMainGenre(); 
        }
        
        private static void OnSubgenreButtonClick() 
        { 
            AssistantCore.ApplyBestSubGenre(); 
        }
        
        private static void OnMainThemeButtonClick() 
        { 
            AssistantCore.ApplyBestMainTheme(); 
        }
        
        private static void OnSubthemeButtonClick() 
        { 
            AssistantCore.ApplyBestSubTheme(); 
        }
        
        private static void OnRandomNameButtonClick() 
        { 
            AssistantCore.ApplyRealName(); 
        }
        
        private static void OnEngineButtonClick() 
        { 
            AssistantCore.ApplyBestEngine(); 
        }
        
        private static void OnPlatformButtonClick() 
        { 
            AssistantCore.ApplyBestPlatform(); 
        }
        
        private static void OnEngineFeatureButtonClick() 
        { 
            AssistantCore.ApplyBestEngineFeatures(); 
        }

        private static void OnAntiCheatButtonClick() 
        { 
            AssistantCore.ApplyAntiCheat(); 
        }

        private static void OnCopyProtectButtonClick() 
        { 
            AssistantCore.ApplyCopyProtect(); 
        }

        private static void OnLanguageButtonClick() 
        { 
            AssistantCore.ApplyBestLanguage(); 
        }

        private static void OnSliderButtonClick() 
        { 
            AssistantCore.ApplyOptimalSliders(); 
        }

        private static void OnGameplayFeatureButtonClick() 
        { 
            AssistantCore.ApplyBestGameplayFeatures(); 
        }
    }
}
