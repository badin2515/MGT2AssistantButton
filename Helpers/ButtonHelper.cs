using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using System.Collections.Generic;

namespace MGT2AssistantButton.Helpers
{
    /// <summary>
    /// Helper class สำหรับการสร้างและโคลนปุ่มพื้นฐาน
    /// </summary>
    public static class ButtonHelper
    {
        /// <summary>
        /// โคลนปุ่มพื้นฐานจาก source button โดยเอาแค่ Image และ Shadow components
        /// </summary>
        /// <param name="sourceButton">ปุ่มต้นฉบับที่จะโคลน</param>
        /// <param name="parent">Parent transform ที่จะใส่ปุ่มใหม่</param>
        /// <param name="buttonName">ชื่อของปุ่มใหม่</param>
        /// <returns>GameObject ของปุ่มที่โคลนแล้ว</returns>
        public static GameObject CloneBasicButton(GameObject sourceButton, Transform parent, string buttonName)
        {
            if (sourceButton == null)
            {
                Plugin.Logger.LogError("Source button is null!");
                return null;
            }

            // สร้าง GameObject ใหม่
            GameObject newButton = new GameObject(buttonName);
            
            // Set parent
            newButton.transform.SetParent(parent, false);
            
            // คัดลอก RectTransform
            RectTransform sourceRect = sourceButton.GetComponent<RectTransform>();
            RectTransform newRect = newButton.AddComponent<RectTransform>();
            
            if (sourceRect != null)
            {
                newRect.anchorMin = sourceRect.anchorMin;
                newRect.anchorMax = sourceRect.anchorMax;
                newRect.pivot = sourceRect.pivot;
                newRect.sizeDelta = sourceRect.sizeDelta;
                newRect.anchoredPosition = sourceRect.anchoredPosition;
                newRect.localScale = sourceRect.localScale;
                newRect.localRotation = sourceRect.localRotation;
            }
            
            // เพิ่ม CanvasRenderer (จำเป็นสำหรับ UI)
            newButton.AddComponent<CanvasRenderer>();
            
            // คัดลอก Image component
            Image sourceImage = sourceButton.GetComponent<Image>();
            if (sourceImage != null)
            {
                Image newImage = newButton.AddComponent<Image>();
                newImage.sprite = sourceImage.sprite;
                newImage.color = sourceImage.color;
                newImage.material = sourceImage.material;
                newImage.raycastTarget = sourceImage.raycastTarget;
                newImage.type = sourceImage.type;
                
                // Logging disabled for performance
            }
            
            // คัดลอก Shadow component
            Shadow sourceShadow = sourceButton.GetComponent<Shadow>();
            if (sourceShadow != null)
            {
                Shadow newShadow = newButton.AddComponent<Shadow>();
                newShadow.effectColor = sourceShadow.effectColor;
                newShadow.effectDistance = sourceShadow.effectDistance;
                newShadow.useGraphicAlpha = sourceShadow.useGraphicAlpha;
                
                // Logging disabled for performance
            }
            
            // Logging disabled for performance
            // Copy tooltips from source
            tooltip sourceTooltip = sourceButton.GetComponent<tooltip>();
            if (sourceTooltip != null)
            {
                tooltip newTooltip = newButton.AddComponent<tooltip>();
                newTooltip.c = sourceTooltip.c;
                newTooltip.textID = sourceTooltip.textID;
                newTooltip.textArray = sourceTooltip.textArray;
                newTooltip.shortcut = sourceTooltip.shortcut;
            }

            return newButton;
        }
        
        /// <summary>
        /// โคลนปุ่มแบบมี Outline + Blend child (เหมือนปุ่ม Button_RandomName)
        /// </summary>
        /// <param name="sourceButton">ปุ่มต้นฉบับที่จะโคลน</param>
        /// <param name="parent">Parent transform ที่จะใส่ปุ่มใหม่</param>
        /// <param name="buttonName">ชื่อของปุ่มใหม่</param>
        /// <returns>GameObject ของปุ่มที่โคลนแล้ว</returns>
        public static GameObject CloneButtonWithOutline(GameObject sourceButton, Transform parent, string buttonName)
        {
            if (sourceButton == null)
            {
                Plugin.Logger.LogError("Source button is null!");
                return null;
            }

            // สร้าง GameObject ใหม่
            GameObject newButton = new GameObject(buttonName);
            
            // Set parent
            newButton.transform.SetParent(parent, false);
            
            // คัดลอก RectTransform
            RectTransform sourceRect = sourceButton.GetComponent<RectTransform>();
            RectTransform newRect = newButton.AddComponent<RectTransform>();
            
            if (sourceRect != null)
            {
                newRect.anchorMin = sourceRect.anchorMin;
                newRect.anchorMax = sourceRect.anchorMax;
                newRect.pivot = sourceRect.pivot;
                newRect.sizeDelta = sourceRect.sizeDelta;
                newRect.anchoredPosition = sourceRect.anchoredPosition;
                newRect.localScale = sourceRect.localScale;
                newRect.localRotation = sourceRect.localRotation;
            }
            
            // เพิ่ม CanvasRenderer (จำเป็นสำหรับ UI)
            newButton.AddComponent<CanvasRenderer>();
            
            // คัดลอก Image component
            Image sourceImage = sourceButton.GetComponent<Image>();
            if (sourceImage != null)
            {
                Image newImage = newButton.AddComponent<Image>();
                newImage.sprite = sourceImage.sprite;
                newImage.color = sourceImage.color;
                newImage.material = sourceImage.material;
                newImage.raycastTarget = sourceImage.raycastTarget;
                newImage.type = sourceImage.type;
                
                // Logging disabled for performance
            }
            
            // คัดลอก Outline component
            Outline sourceOutline = sourceButton.GetComponent<Outline>();
            if (sourceOutline != null)
            {
                Outline newOutline = newButton.AddComponent<Outline>();
                newOutline.effectColor = sourceOutline.effectColor;
                newOutline.effectDistance = sourceOutline.effectDistance;
                newOutline.useGraphicAlpha = sourceOutline.useGraphicAlpha;
                
                // Logging disabled for performance
            }
            
            // คัดลอก Shadow component
            Shadow sourceShadow = sourceButton.GetComponent<Shadow>();
            if (sourceShadow != null)
            {
                Shadow newShadow = newButton.AddComponent<Shadow>();
                newShadow.effectColor = sourceShadow.effectColor;
                newShadow.effectDistance = sourceShadow.effectDistance;
                newShadow.useGraphicAlpha = sourceShadow.useGraphicAlpha;
                
                // Logging disabled for performance
            }
            
            // หาและโคลน Blend child
            Transform sourceBlend = sourceButton.transform.Find("Blend");
            if (sourceBlend != null)
            {
                GameObject newBlendObj = new GameObject("Blend");
                newBlendObj.transform.SetParent(newButton.transform, false);
                
                // คัดลอก RectTransform ของ Blend
                RectTransform sourceBlendRect = sourceBlend.GetComponent<RectTransform>();
                RectTransform newBlendRect = newBlendObj.AddComponent<RectTransform>();
                
                if (sourceBlendRect != null)
                {
                    newBlendRect.anchorMin = sourceBlendRect.anchorMin;
                    newBlendRect.anchorMax = sourceBlendRect.anchorMax;
                    newBlendRect.pivot = sourceBlendRect.pivot;
                    newBlendRect.sizeDelta = sourceBlendRect.sizeDelta;
                    newBlendRect.anchoredPosition = sourceBlendRect.anchoredPosition;
                    newBlendRect.localScale = sourceBlendRect.localScale;
                }
                
                // เพิ่ม CanvasRenderer
                newBlendObj.AddComponent<CanvasRenderer>();
                
                // คัดลอก Image component ของ Blend
                Image sourceBlendImage = sourceBlend.GetComponent<Image>();
                if (sourceBlendImage != null)
                {
                    Image newBlendImage = newBlendObj.AddComponent<Image>();
                    newBlendImage.sprite = sourceBlendImage.sprite;
                    newBlendImage.color = sourceBlendImage.color;
                    newBlendImage.material = sourceBlendImage.material;
                    newBlendImage.raycastTarget = sourceBlendImage.raycastTarget;
                    newBlendImage.type = sourceBlendImage.type;
                }
                
                // Logging disabled for performance
            }
            
            // Logging disabled for performance
            return newButton;
        }
            
        /// <summary>
        /// เพิ่ม Button component และกำหนด onClick event
        /// </summary>
        public static Button AddButtonComponent(GameObject buttonObject, UnityEngine.Events.UnityAction onClick)
        {
            Button button = buttonObject.GetComponent<Button>();
            if (button == null)
            {
                button = buttonObject.AddComponent<Button>();
            }
            
            if (onClick != null)
            {
                button.onClick.AddListener(onClick);
            }
            
            return button;
        }
        
        /// <summary>
        /// เพิ่ม Text child เพื่อแสดงข้อความบนปุ่ม
        /// </summary>
        public static Text AddButtonText(GameObject button, string text, int fontSize = 14, Color? textColor = null)
        {
            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(button.transform, false);
            
            RectTransform textRect = textObject.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            
            Text textComponent = textObject.AddComponent<Text>();
            textComponent.text = text;
            textComponent.fontSize = fontSize;
            textComponent.color = textColor ?? Color.white;
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            
            return textComponent;
        }

        /// <summary>
        /// แก้ไขข้อความของปุ่มที่มีอยู่แล้ว (รองรับทั้ง Text และ TMPro)
        /// </summary>
        public static void SetButtonText(GameObject button, string newText)
        {
            if (button == null) return;

            // 1. ลองหา Text (Standard Unity UI)
            Text textComp = button.GetComponentInChildren<Text>(true); // true = include inactive
            if (textComp != null)
            {
                textComp.text = newText;
                return;
            }

            // 2. ลองหา TextMeshProUGUI (ผ่าน Reflection เพื่อไม่ต้อง reference DLL)
            Component[] components = button.GetComponentsInChildren<Component>(true);
            foreach (Component comp in components)
            {
                // เช็คชื่อ Type ว่าเป็น TMPro หรือไม่
                if (comp.GetType().Name.Contains("TextMeshPro") || comp.GetType().Name.Contains("TMP_Text"))
                {
                    var prop = comp.GetType().GetProperty("text");
                    if (prop != null)
                    {
                        prop.SetValue(comp, newText, null);
                        return;
                    }
                }
            }
            
            // 3. ถ้าไม่เจออะไรเลย ลองหา GameObject ชื่อ "Text" หรือ "Label" แล้วดูว่ามี component อะไรบ้าง
            Transform textObj = button.transform.Find("Text");
            if (textObj == null) textObj = button.transform.Find("Label");
            
            if (textObj != null)
            {
                Plugin.Logger.LogWarning($"Found text object {textObj.name} but no known text component. Components: {string.Join(", ", textObj.GetComponents<Component>().Select(c => c.GetType().Name).ToArray())}");
            }
            else
            {
                Plugin.Logger.LogWarning($"Could not find any Text component or Text GameObject on button {button.name}");
            }
        }
        public static void SetTooltip(GameObject button, string text)
        {
            if (button == null) return;
            tooltip t = button.GetComponent<tooltip>();
            if (t == null) t = button.AddComponent<tooltip>();
            
            t.c = text;
            t.textID = -1;
            t.textArray = "";
        }

        /// <summary>
        /// เพิ่ม Dropdown Menu ให้กับปุ่ม - แสดงเมื่อคลิกขวา
        /// </summary>
        /// <param name="button">Button component ที่จะเพิ่ม dropdown</param>
        /// <param name="menuItems">รายการ items ใน dropdown</param>
        /// <param name="defaultIndex">Index ของ item ที่เลือกเริ่มต้น (0-based)</param>
        /// <param name="onLeft">true = แสดง dropdown ทางซ้าย, false = แสดงทางขวา (default)</param>
        /// <param name="highlightColor">สี highlight (optional)</param>
        public static void AddDropdown(Button button, List<DropdownItem> menuItems, int defaultIndex = 0, bool onLeft = false, Color? highlightColor = null)
        {
            if (button == null || menuItems == null || menuItems.Count == 0) return;

            Color highlight = highlightColor ?? new Color(0.3f, 0.6f, 0.9f, 1f);
            int currentIndex = defaultIndex;
            GameObject menuPanel = null;

            // Calculate width based on longest text
            float maxWidth = 100f; // minimum width
            foreach (var item in menuItems)
            {
                // Estimate width: roughly 7 pixels per character + padding
                float estimatedWidth = (item.Label.Length * 7f) + 40f;
                if (estimatedWidth > maxWidth) maxWidth = estimatedWidth;
            }
            maxWidth = Mathf.Min(maxWidth, 300f); // cap at 300

            // Create dropdown panel
            menuPanel = new GameObject("DropdownPanel");
            menuPanel.transform.SetParent(button.transform.root, false);
            menuPanel.SetActive(false);

            var rect = menuPanel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(maxWidth, 5f + menuItems.Count * 35f);

            var buttonRect = button.GetComponent<RectTransform>();
            
            // Position dropdown on left or right
            if (onLeft)
            {
                rect.pivot = new Vector2(1f, 0.5f); // Pivot at right edge
                rect.position = new Vector3(buttonRect.position.x - 25f, buttonRect.position.y, buttonRect.position.z);
            }
            else
            {
                rect.pivot = new Vector2(0f, 0.5f); // Pivot at left edge
                rect.position = new Vector3(buttonRect.position.x + 25f, buttonRect.position.y, buttonRect.position.z);
            }

            var canvas = menuPanel.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 1000;

            menuPanel.AddComponent<GraphicRaycaster>();

            var bg = menuPanel.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);

            var outline = menuPanel.AddComponent<Outline>();
            outline.effectColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            outline.effectDistance = new Vector2(1, -1);


            // Create menu items
            List<Text> itemTexts = new List<Text>();
            for (int i = 0; i < menuItems.Count; i++)
            {
                int index = i;
                string label = menuItems[i].Label;
                Action action = menuItems[i].Action;

                GameObject item = new GameObject("MenuItem_" + i);
                item.transform.SetParent(menuPanel.transform, false);

                var itemRect = item.AddComponent<RectTransform>();
                itemRect.anchorMin = new Vector2(0, 1);
                itemRect.anchorMax = new Vector2(1, 1);
                itemRect.pivot = new Vector2(0.5f, 1f);
                itemRect.sizeDelta = new Vector2(0, 32f);
                itemRect.anchoredPosition = new Vector2(0, -5f - (i * 35f));

                var itemButton = item.AddComponent<Button>();
                var img = item.AddComponent<Image>();
                img.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);

                var colors = itemButton.colors;
                colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
                colors.highlightedColor = highlight;
                colors.pressedColor = new Color(highlight.r * 0.8f, highlight.g * 0.8f, highlight.b * 0.8f, 1f);
                itemButton.colors = colors;

                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(item.transform, false);

                var textRect = textObj.AddComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.sizeDelta = Vector2.zero;
                textRect.anchoredPosition = Vector2.zero;
                textRect.offsetMin = new Vector2(10, 0);

                var text = textObj.AddComponent<Text>();
                text.text = (i == currentIndex ? "✓ " : "  ") + label;
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.fontSize = 13;
                text.color = Color.white;
                text.alignment = TextAnchor.MiddleLeft;
                text.horizontalOverflow = HorizontalWrapMode.Overflow;

                itemTexts.Add(text);

                itemButton.onClick.AddListener(() => {
                    currentIndex = index;
                    if (action != null) action.Invoke();
                    // Update checkmarks
                    for (int j = 0; j < itemTexts.Count; j++)
                    {
                        itemTexts[j].text = (j == currentIndex ? "✓ " : "  ") + menuItems[j].Label;
                    }
                    menuPanel.SetActive(false);
                });
            }

            // Left Click: Execute current action
            button.onClick.AddListener(() => {
                if (currentIndex >= 0 && currentIndex < menuItems.Count)
                {
                    if (menuItems[currentIndex].Action != null)
                        menuItems[currentIndex].Action.Invoke();
                }
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
                    menuPanel.SetActive(!menuPanel.activeSelf);
                    if (menuPanel.activeSelf) menuPanel.transform.SetAsLastSibling();
                }
            });
            trigger.triggers.Add(entry);

            // Add auto-close behavior
            var closer = button.gameObject.AddComponent<DropdownAutoCloser>();
            closer.Setup(menuPanel, button.GetComponent<RectTransform>());
        }
    }

    /// <summary>
    /// Item สำหรับ Dropdown Menu
    /// </summary>
    public class DropdownItem
    {
        public string Label { get; set; }
        public Action Action { get; set; }

        public DropdownItem(string label, Action action)
        {
            Label = label;
            Action = action;
        }
    }

    /// <summary>
    /// Helper component to auto-close dropdown when clicking outside
    /// </summary>
    public class DropdownAutoCloser : MonoBehaviour
    {
        private GameObject panel;
        private RectTransform buttonRect;

        public void Setup(GameObject menuPanel, RectTransform buttonRectTransform)
        {
            panel = menuPanel;
            buttonRect = buttonRectTransform;
        }

        void Update()
        {
            if (panel != null && panel.activeSelf && Input.GetMouseButtonDown(0))
            {
                if (!RectTransformUtility.RectangleContainsScreenPoint(
                    panel.GetComponent<RectTransform>(), Input.mousePosition) &&
                    !RectTransformUtility.RectangleContainsScreenPoint(
                    buttonRect, Input.mousePosition))
                {
                    panel.SetActive(false);
                }
            }
        }
    }
}

