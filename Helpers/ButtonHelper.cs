using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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
    }
}
