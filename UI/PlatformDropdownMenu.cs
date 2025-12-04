using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MGT2AssistantButton.UI
{
    /// <summary>
    /// Dropdown menu for platform selection modes
    /// </summary>
    public class PlatformDropdownMenu : MonoBehaviour
    {
        private GameObject menuPanel;
        private Core.PlatformMode currentMode = Core.PlatformMode.ByMarket;
        private System.Action<Core.PlatformMode> onModeSelected;
        private Button triggerButton;

        public static PlatformDropdownMenu Create(Button button, System.Action<Core.PlatformMode> callback)
        {
            var dropdown = button.gameObject.AddComponent<PlatformDropdownMenu>();
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
            menuPanel = new GameObject("PlatformDropdownPanel");
            menuPanel.transform.SetParent(triggerButton.transform.root, false); // Use root (Canvas)
            menuPanel.SetActive(false);

            // Add RectTransform
            var rect = menuPanel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f); // Left center
            rect.sizeDelta = new Vector2(180f, 200f);
            
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
            CreateMenuItem("✓ Best Market Share", Core.PlatformMode.ByMarket, 0);
            CreateMenuItem("  Console Focus", Core.PlatformMode.ConsoleOnly, 1);
            CreateMenuItem("  PC Focus", Core.PlatformMode.PCOnly, 2);
            CreateMenuItem("  Own Platforms First", Core.PlatformMode.OurConsoleFirst, 3);
            CreateMenuItem("  Highest Tech Only", Core.PlatformMode.HighestTechOnly, 4);
        }

        private void CreateMenuItem(string label, Core.PlatformMode mode, int index)
        {
            GameObject item = new GameObject($"MenuItem_{mode}");
            item.transform.SetParent(menuPanel.transform, false);

            var rect = item.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(0, 30f);
            rect.anchoredPosition = new Vector2(0, -5f - (index * 32f));

            // Add button component
            var button = item.AddComponent<Button>();
            var img = item.AddComponent<Image>();
            img.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);

            // Hover effect
            var colors = button.colors;
            colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            colors.highlightedColor = new Color(0.3f, 0.5f, 0.8f, 0.8f);
            colors.pressedColor = new Color(0.2f, 0.4f, 0.7f, 1f);
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

        private void SelectMode(Core.PlatformMode mode)
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
                        if (System.Enum.TryParse<Core.PlatformMode>(modeName, out var mode))
                        {
                            text.text = mode == currentMode 
                                ? "✓ " + GetModeLabel(mode)
                                : "  " + GetModeLabel(mode);
                        }
                    }
                }
            }
        }

        private string GetModeLabel(Core.PlatformMode mode)
        {
            return mode switch
            {
                Core.PlatformMode.ByMarket => "Best Market Share",
                Core.PlatformMode.ConsoleOnly => "Console Focus",
                Core.PlatformMode.PCOnly => "PC Focus",
                Core.PlatformMode.OurConsoleFirst => "Own Platforms First",
                Core.PlatformMode.HighestTechOnly => "Highest Tech Only",
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
