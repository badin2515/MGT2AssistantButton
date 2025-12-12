using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace MGT2AssistantButton.Helpers
{
    public static class SpriteHelper
    {
        // Cache สำหรับเก็บ sprite ที่โหลดแล้ว
        private static Dictionary<string, Sprite> _cache = new Dictionary<string, Sprite>();

        public static Sprite LoadSprite(string filePath)
        {
            // ถ้ามีใน cache แล้ว → return เลย (เร็วมาก)
            if (_cache.TryGetValue(filePath, out Sprite cachedSprite))
            {
                return cachedSprite;
            }

            if (!File.Exists(filePath))
            {
                Plugin.Logger.LogError($"Image file not found: {filePath}");
                return null;
            }

            try
            {
                byte[] fileData = File.ReadAllBytes(filePath);
                Texture2D texture = new Texture2D(2, 2);
                if (texture.LoadImage(fileData))
                {
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    
                    // เก็บใน cache ไว้ใช้ครั้งหน้า
                    _cache[filePath] = sprite;
                    
                    return sprite;
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Failed to load sprite: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// ล้าง cache (ใช้ถ้าต้องการ reload sprite ใหม่)
        /// </summary>
        public static void ClearCache()
        {
            _cache.Clear();
        }
    }
}
