using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

namespace MGT2AssistantButton.UI
{
    /// <summary>
    /// Multi-select dropdown menu ที่สามารถติ๊กเลือกได้หลายตัวเลือก
    /// </summary>
    public abstract class BaseMultiSelectDropdown : MonoBehaviour
    {
        protected GameObject menuPanel;
        protected Button triggerButton;
        protected List<FilterItem> filterItems = new List<FilterItem>();
        protected Action onApply;
        
        protected class FilterItem
        {
            public string Label;
            public string Key;
            public bool IsSelected;
            public bool IsHeader;
            public GameObject UIElement;
            public Toggle Toggle;
            
            public FilterItem(string label, string key, bool isSelected = false, bool isHeader = false)
            {
                Label = label;
                Key = key;
                IsSelected = isSelected;
                IsHeader = isHeader;
            }
        }
        
        protected abstract string PanelName { get; }
        protected abstract Color AccentColor { get; }
        protected abstract void DefineFilterItems();
        protected abstract void OnApplyFilters();
        
        protected virtual bool DropdownOnLeft => true;
        
        protected void Initialize(Button button, Action applyCallback)
        {
            triggerButton = button;
            onApply = applyCallback;
            
            DefineFilterItems();
            CreateDropdownMenu();
            
            // Left Click: Apply current filters
            button.onClick.AddListener(() => {
                OnApplyFilters();
                onApply?.Invoke();
            });
            
            // Right Click: Toggle Menu
            EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
            if (trigger == null) trigger = button.gameObject.AddComponent<EventTrigger>();
            
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => {
                PointerEventData pData = (PointerEventData)data;
                if (pData.button == PointerEventData.InputButton.Right)
                {
                    ToggleMenu();
                }
            });
            trigger.triggers.Add(entry);
        }
        
        private void CreateDropdownMenu()
        {
            menuPanel = new GameObject(PanelName);
            menuPanel.transform.SetParent(triggerButton.transform.root, false);
            menuPanel.SetActive(false);
            
            // Calculate dimensions
            float itemHeight = 32f;  // เพิ่มความสูงเพื่อรองรับ text ที่ใหญ่ขึ้น
            float menuHeight = 10f + (filterItems.Count * itemHeight) + 45f; // +45 for Apply button
            float menuWidth = 200f;  // เพิ่มความกว้าง
            
            var rect = menuPanel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(menuWidth, menuHeight);
            
            var buttonRect = triggerButton.GetComponent<RectTransform>();
            
            if (DropdownOnLeft)
            {
                rect.pivot = new Vector2(1f, 0.5f);
                rect.position = new Vector3(buttonRect.position.x - 25f, buttonRect.position.y, buttonRect.position.z);
            }
            else
            {
                rect.pivot = new Vector2(0f, 0.5f);
                rect.position = new Vector3(buttonRect.position.x + 25f, buttonRect.position.y, buttonRect.position.z);
            }
            
            var canvas = menuPanel.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 1000;
            
            menuPanel.AddComponent<GraphicRaycaster>();
            
            var bg = menuPanel.AddComponent<Image>();
            bg.color = new Color(0.12f, 0.12f, 0.15f, 0.98f);
            
            var outline = menuPanel.AddComponent<Outline>();
            outline.effectColor = AccentColor;
            outline.effectDistance = new Vector2(2, -2);
            
            // Create items
            for (int i = 0; i < filterItems.Count; i++)
            {
                CreateFilterItem(filterItems[i], i, itemHeight);
            }
            
            // Create Apply button
            CreateApplyButton(menuHeight - 35f, menuWidth);
        }
        
        private void CreateFilterItem(FilterItem item, int index, float itemHeight)
        {
            GameObject itemObj = new GameObject($"FilterItem_{item.Key}");
            itemObj.transform.SetParent(menuPanel.transform, false);
            item.UIElement = itemObj;
            
            var rect = itemObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(-10, itemHeight - 2);
            rect.anchoredPosition = new Vector2(0, -5f - (index * itemHeight));
            
            if (item.IsHeader)
            {
                // Header style
                var headerBg = itemObj.AddComponent<Image>();
                headerBg.color = new Color(AccentColor.r * 0.3f, AccentColor.g * 0.3f, AccentColor.b * 0.3f, 0.5f);
                
                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(itemObj.transform, false);
                
                var textRect = textObj.AddComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.sizeDelta = Vector2.zero;
                textRect.offsetMin = new Vector2(8, 0);
                
                var text = textObj.AddComponent<Text>();
                text.text = item.Label;
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.fontSize = 14;  // ปรับใหญ่ขึ้น
                text.fontStyle = FontStyle.Bold;
                text.color = AccentColor;
                text.alignment = TextAnchor.MiddleLeft;
            }
            else
            {
                // Toggle item style
                var toggleBg = itemObj.AddComponent<Image>();
                toggleBg.color = new Color(0.2f, 0.2f, 0.22f, 0.3f);
                
                var toggle = itemObj.AddComponent<Toggle>();
                toggle.isOn = item.IsSelected;
                item.Toggle = toggle;
                
                // Checkmark
                GameObject checkmarkObj = new GameObject("Checkmark");
                checkmarkObj.transform.SetParent(itemObj.transform, false);
                
                var checkRect = checkmarkObj.AddComponent<RectTransform>();
                checkRect.anchorMin = new Vector2(0, 0.5f);
                checkRect.anchorMax = new Vector2(0, 0.5f);
                checkRect.pivot = new Vector2(0, 0.5f);
                checkRect.sizeDelta = new Vector2(18, 18);
                checkRect.anchoredPosition = new Vector2(8, 0);
                
                var checkBg = checkmarkObj.AddComponent<Image>();
                checkBg.color = new Color(0.3f, 0.3f, 0.35f, 1f);
                
                GameObject tickObj = new GameObject("Tick");
                tickObj.transform.SetParent(checkmarkObj.transform, false);
                
                var tickRect = tickObj.AddComponent<RectTransform>();
                tickRect.anchorMin = Vector2.zero;
                tickRect.anchorMax = Vector2.one;
                tickRect.sizeDelta = new Vector2(-4, -4);
                tickRect.anchoredPosition = Vector2.zero;
                
                var tickImg = tickObj.AddComponent<Image>();
                tickImg.color = AccentColor;
                
                toggle.graphic = tickImg;
                toggle.targetGraphic = checkBg;
                
                var toggleColors = toggle.colors;
                toggleColors.normalColor = new Color(0.3f, 0.3f, 0.35f, 1f);
                toggleColors.highlightedColor = new Color(0.4f, 0.4f, 0.45f, 1f);
                toggle.colors = toggleColors;
                
                // Label
                GameObject labelObj = new GameObject("Label");
                labelObj.transform.SetParent(itemObj.transform, false);
                
                var labelRect = labelObj.AddComponent<RectTransform>();
                labelRect.anchorMin = Vector2.zero;
                labelRect.anchorMax = Vector2.one;
                labelRect.sizeDelta = Vector2.zero;
                labelRect.offsetMin = new Vector2(32, 0);
                
                var label = labelObj.AddComponent<Text>();
                label.text = item.Label;
                label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                label.fontSize = 13;  // ปรับใหญ่ขึ้น
                label.color = Color.white;
                label.alignment = TextAnchor.MiddleLeft;
                
                // Update item state on toggle
                string key = item.Key;
                toggle.onValueChanged.AddListener((isOn) => {
                    var fi = filterItems.Find(f => f.Key == key);
                    if (fi != null) fi.IsSelected = isOn;
                });
            }
        }
        
        private void CreateApplyButton(float yPos, float width)
        {
            GameObject btnObj = new GameObject("ApplyButton");
            btnObj.transform.SetParent(menuPanel.transform, false);
            
            var rect = btnObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0);
            rect.anchorMax = new Vector2(0.5f, 0);
            rect.pivot = new Vector2(0.5f, 0);
            rect.sizeDelta = new Vector2(width - 20, 28);
            rect.anchoredPosition = new Vector2(0, 5);
            
            var img = btnObj.AddComponent<Image>();
            img.color = AccentColor;
            
            var btn = btnObj.AddComponent<Button>();
            var colors = btn.colors;
            colors.normalColor = AccentColor;
            colors.highlightedColor = new Color(AccentColor.r * 1.2f, AccentColor.g * 1.2f, AccentColor.b * 1.2f, 1f);
            colors.pressedColor = new Color(AccentColor.r * 0.8f, AccentColor.g * 0.8f, AccentColor.b * 0.8f, 1f);
            btn.colors = colors;
            
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);
            
            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            
            var text = textObj.AddComponent<Text>();
            text.text = "✓ Apply & Select";
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 14;  // ปรับใหญ่ขึ้น
            text.fontStyle = FontStyle.Bold;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            
            btn.onClick.AddListener(() => {
                OnApplyFilters();
                onApply?.Invoke();
                HideMenu();
            });
        }
        
        protected bool GetFilterValue(string key)
        {
            var item = filterItems.Find(f => f.Key == key);
            return item?.IsSelected ?? false;
        }
        
        protected void SetFilterValue(string key, bool value)
        {
            var item = filterItems.Find(f => f.Key == key);
            if (item != null)
            {
                item.IsSelected = value;
                if (item.Toggle != null) item.Toggle.isOn = value;
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
            if (menuPanel != null && menuPanel.activeSelf && Input.GetMouseButtonDown(0))
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
