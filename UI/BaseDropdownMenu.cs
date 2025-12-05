using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace MGT2AssistantButton.UI
{
    /// <summary>
    /// Base class for dropdown menus with common functionality
    /// </summary>
    public abstract class BaseDropdownMenu<TMode> : MonoBehaviour where TMode : Enum
    {
        protected GameObject menuPanel;
        protected TMode currentMode;
        protected Action<TMode> onModeSelected;
        protected Button triggerButton;
        protected class MenuItemData
        {
            public string Label;
            public TMode Mode;
            public MenuItemData(string label, TMode mode) { Label = label; Mode = mode; }
        }
        protected List<MenuItemData> menuItems = new List<MenuItemData>();

        protected abstract string PanelName { get; }
        protected abstract Color HighlightColor { get; }
        protected abstract TMode DefaultMode { get; }
        protected abstract void DefineMenuItems();
        protected abstract string GetModeLabel(TMode mode);

        protected void Initialize(Button button, Action<TMode> callback)
        {
            triggerButton = button;
            onModeSelected = callback;
            currentMode = DefaultMode;
            DefineMenuItems();
            CreateDropdownMenu();
            button.onClick.AddListener(ToggleMenu);
        }

        private void CreateDropdownMenu()
        {
            menuPanel = new GameObject(PanelName);
            menuPanel.transform.SetParent(triggerButton.transform.root, false);
            menuPanel.SetActive(false);

            var rect = menuPanel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.sizeDelta = new Vector2(180f, 5f + menuItems.Count * 35f);
            
            var buttonRect = triggerButton.GetComponent<RectTransform>();
            rect.position = new Vector3(buttonRect.position.x + 25f, buttonRect.position.y, buttonRect.position.z);

            var canvas = menuPanel.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 1000;

            menuPanel.AddComponent<GraphicRaycaster>();

            var bg = menuPanel.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);

            var outline = menuPanel.AddComponent<Outline>();
            outline.effectColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            outline.effectDistance = new Vector2(1, -1);

            for (int i = 0; i < menuItems.Count; i++)
            {
                CreateMenuItem(menuItems[i].Label, menuItems[i].Mode, i);
            }
        }

        private void CreateMenuItem(string label, TMode mode, int index)
        {
            GameObject item = new GameObject($"MenuItem_{mode}");
            item.transform.SetParent(menuPanel.transform, false);

            var rect = item.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(0, 32f);
            rect.anchoredPosition = new Vector2(0, -5f - (index * 35f));

            var button = item.AddComponent<Button>();
            var img = item.AddComponent<Image>();
            img.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);

            var colors = button.colors;
            colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            colors.highlightedColor = HighlightColor;
            colors.pressedColor = new Color(HighlightColor.r * 0.8f, HighlightColor.g * 0.8f, HighlightColor.b * 0.8f, 1f);
            button.colors = colors;

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(item.transform, false);
            
            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            textRect.offsetMin = new Vector2(10, 0);

            var text = textObj.AddComponent<Text>();
            text.text = label;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 13;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleLeft;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;

            button.onClick.AddListener(() => {
                SelectMode(mode);
                UpdateMenuItems();
                HideMenu();
            });
        }

        protected void SelectMode(TMode mode)
        {
            currentMode = mode;
            onModeSelected?.Invoke(mode);
        }

        protected void UpdateMenuItems()
        {
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
                        try
                        {
                            var mode = (TMode)Enum.Parse(typeof(TMode), modeName);
                            text.text = mode.Equals(currentMode) 
                                ? "✓ " + GetModeLabel(mode)
                                : "  " + GetModeLabel(mode);
                        }
                        catch { }
                    }
                }
            }
        }

        protected void ToggleMenu()
        {
            if (menuPanel.activeSelf) HideMenu();
            else ShowMenu();
        }

        protected void ShowMenu()
        {
            menuPanel.SetActive(true);
            menuPanel.transform.SetAsLastSibling();
        }

        protected void HideMenu() => menuPanel.SetActive(false);

        void Update()
        {
            if (menuPanel.activeSelf && Input.GetMouseButtonDown(0))
            {
                if (!RectTransformUtility.RectangleContainsScreenPoint(
                    menuPanel.GetComponent<RectTransform>(), Input.mousePosition) &&
                    !RectTransformUtility.RectangleContainsScreenPoint(
                    triggerButton.GetComponent<RectTransform>(), Input.mousePosition))
                {
                    HideMenu();
                }
            }
        }
    }
}
