using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MGT2AssistantButton.UI
{
    /// <summary>
    /// Dropdown menu for language selection modes
    /// </summary>
    public class LanguageDropdownMenu : MonoBehaviour
    {
        private GameObject menuPanel;
        private Core.LanguageMode currentMode = Core.LanguageMode.AllLanguages;
        private System.Action<Core.LanguageMode> onModeSelected;
        private Button triggerButton;

        public static LanguageDropdownMenu Create(Button button, System.Action<Core.LanguageMode> callback)
        {
            var dropdown = button.gameObject.AddComponent<LanguageDropdownMenu>();
            dropdown.triggerButton = button;
            dropdown.onModeSelected = callback;
            dropdown.CreateDropdownMenu();
            
            // Add click listener to toggle menu
            button.onClick.AddListener(() => dropdown.ToggleMenu());
            
            return dropdown;
        }

        private void CreateDropdownMenu()
        {
            // Create panel for dropdown menu
            menuPanel = new GameObject("LanguageDropdownPanel");
            menuPanel.transform.SetParent(triggerButton.transform.root, false); // Use root (Canvas)
            menuPanel.SetActive(false);

            // Add RectTransform
            var rect = menuPanel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f); // Left center
            rect.sizeDelta = new Vector2(200f, 200f);
            
            // Position to the right of the button (in screen space)
            var buttonRect = triggerButton.GetComponent<RectTransform>();
            Vector3 buttonWorldPos = buttonRect.position;
            rect.position = new Vector3(buttonWorldPos.x + 100f, buttonWorldPos.y, buttonWorldPos.z);

            // Add Canvas component for independent rendering
            var canvas = menuPanel.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 1000; // High value to render on top

            // Add GraphicRaycaster to make it clickable
            menuPanel.AddComponent<GraphicRaycaster>();

            // Add background
            var bg = menuPanel.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);

            // Add outline
            var outline = menuPanel.AddComponent<Outline>();
            outline.effectColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            outline.effectDistance = new Vector2(1, -1);

            // Create menu items
            CreateMenuItem("✓ All Languages", Core.LanguageMode.AllLanguages, 0);
            CreateMenuItem("  Mother Tongue (Free)", Core.LanguageMode.MotherTongueOnly, 1);
            CreateMenuItem("  Top 5 Markets", Core.LanguageMode.TopMarketLanguages, 2);
            CreateMenuItem("  Western Markets", Core.LanguageMode.WesternLanguages, 3);
            CreateMenuItem("  Asian Markets", Core.LanguageMode.AsianLanguages, 4);
        }

        private void CreateMenuItem(string label, Core.LanguageMode mode, int index)
        {
            GameObject item = new GameObject($"MenuItem_{mode}");
            item.transform.SetParent(menuPanel.transform, false);

            var rect = item.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(0, 32f);
            rect.anchoredPosition = new Vector2(0, -5f - (index * 35f));

            // Add button component
            var button = item.AddComponent<Button>();
            var img = item.AddComponent<Image>();
            img.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);

            // Hover effect
            var colors = button.colors;
            colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            colors.highlightedColor = new Color(0.3f, 0.6f, 0.4f, 0.8f); // Greenish for language
            colors.pressedColor = new Color(0.2f, 0.5f, 0.3f, 1f);
            button.colors = colors;

            // Add text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(item.transform, false);
            
            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;

            var text = textObj.AddComponent<Text>();
            text.text = label;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 13;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleLeft;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            
            // Padding left
            textRect.offsetMin = new Vector2(10, 0);

            // Add click handler
            button.onClick.AddListener(() => {
                SelectMode(mode);
                UpdateMenuItems();
                HideMenu();
            });
        }

        private void SelectMode(Core.LanguageMode mode)
        {
            currentMode = mode;
            onModeSelected?.Invoke(mode);
        }

        private void UpdateMenuItems()
        {
            // Update checkmarks
            for (int i = 0; i < menuPanel.transform.childCount; i++)
            {
                var child = menuPanel.transform.GetChild(i);
                var textObj = child.Find("Text");
                if (textObj != null)
                {
                    var text = textObj.GetComponent<Text>();
                    if (text != null)
                    {
                        var modeName = child.name.Replace("MenuItem_", "");
                        if (System.Enum.TryParse<Core.LanguageMode>(modeName, out var mode))
                        {
                            text.text = mode == currentMode 
                                ? "✓ " + GetModeLabel(mode)
                                : "  " + GetModeLabel(mode);
                        }
                    }
                }
            }
        }

        private string GetModeLabel(Core.LanguageMode mode)
        {
            return mode switch
            {
                Core.LanguageMode.AllLanguages => "All Languages",
                Core.LanguageMode.MotherTongueOnly => "Mother Tongue (Free)",
                Core.LanguageMode.TopMarketLanguages => "Top 5 Markets",
                Core.LanguageMode.WesternLanguages => "Western Markets",
                Core.LanguageMode.AsianLanguages => "Asian Markets",
                _ => mode.ToString()
            };
        }

        private void ToggleMenu()
        {
            if (menuPanel.activeSelf)
                HideMenu();
            else
                ShowMenu();
        }

        private void ShowMenu()
        {
            menuPanel.SetActive(true);
            // Bring to front
            menuPanel.transform.SetAsLastSibling();
        }

        private void HideMenu()
        {
            menuPanel.SetActive(false);
        }

        void Update()
        {
            // Close menu when clicking outside
            if (menuPanel.activeSelf && Input.GetMouseButtonDown(0))
            {
                if (!RectTransformUtility.RectangleContainsScreenPoint(
                    menuPanel.GetComponent<RectTransform>(), 
                    Input.mousePosition) &&
                    !RectTransformUtility.RectangleContainsScreenPoint(
                    triggerButton.GetComponent<RectTransform>(), 
                    Input.mousePosition))
                {
                    HideMenu();
                }
            }
        }
    }
}
