using UnityEngine;
using UnityEngine.UI;
using MGT2AssistantButton.Core;

namespace MGT2AssistantButton.UI
{
    /// <summary>
    /// Dropdown menu for gameplay features selection modes
    /// </summary>
    public class GameplayFeatureDropdownMenu : MonoBehaviour
    {
        private GameObject menuPanel;
        private GameplayFeatureMode currentMode = GameplayFeatureMode.Best;
        private System.Action<GameplayFeatureMode> onModeSelected;
        private Button triggerButton;

        public static GameplayFeatureDropdownMenu Create(Button button, System.Action<GameplayFeatureMode> callback)
        {
            var dropdown = button.gameObject.AddComponent<GameplayFeatureDropdownMenu>();
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
            menuPanel = new GameObject("GameplayFeatureDropdownPanel");
            menuPanel.transform.SetParent(triggerButton.transform.root, false); // Use root (Canvas)
            menuPanel.SetActive(false);

            // Add RectTransform
            var rect = menuPanel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f); // Left center
            rect.sizeDelta = new Vector2(200f, 115f); // Taller for 3 items
            
            // Position to the right of the button (in screen space)
            var buttonRect = triggerButton.GetComponent<RectTransform>();
            Vector3 buttonWorldPos = buttonRect.position;
            rect.position = new Vector3(buttonWorldPos.x + 25f, buttonWorldPos.y, buttonWorldPos.z);

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
            CreateMenuItem("✓ Best (Good+Neutral)", GameplayFeatureMode.Best, 0);
            CreateMenuItem("  All (Fill to limit)", GameplayFeatureMode.All, 1);
            CreateMenuItem("  Platform Only", GameplayFeatureMode.PlatformOnly, 2);
        }

        private void CreateMenuItem(string label, GameplayFeatureMode mode, int index)
        {
            GameObject item = new GameObject("MenuItem_" + mode.ToString());
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
            colors.highlightedColor = new Color(0.5f, 0.4f, 0.6f, 0.8f); // Purple for gameplay
            colors.pressedColor = new Color(0.4f, 0.3f, 0.5f, 1f);
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

        private void SelectMode(GameplayFeatureMode mode)
        {
            currentMode = mode;
            if (onModeSelected != null)
                onModeSelected.Invoke(mode);
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
                        GameplayFeatureMode mode;
                        if (System.Enum.TryParse<GameplayFeatureMode>(modeName, out mode))
                        {
                            text.text = mode == currentMode 
                                ? "✓ " + GetModeLabel(mode)
                                : "  " + GetModeLabel(mode);
                        }
                    }
                }
            }
        }

        private string GetModeLabel(GameplayFeatureMode mode)
        {
            switch (mode)
            {
                case GameplayFeatureMode.Best:
                    return "Best (Good+Neutral)";
                case GameplayFeatureMode.All:
                    return "All (Fill to limit)";
                case GameplayFeatureMode.PlatformOnly:
                    return "Platform Only";
                default:
                    return mode.ToString();
            }
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
