using System;
using System.IO;
using UnityEngine;

namespace MGT2AssistantButton.Helpers
{
    public static class SpriteHelper
    {
        public static Sprite LoadSprite(string filePath)
        {
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
                    return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Failed to load sprite: {ex.Message}");
            }

            return null;
        }
    }
}
