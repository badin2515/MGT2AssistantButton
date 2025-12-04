using System.Text;
using UnityEngine;

namespace MGT2AssistantButton.Helpers
{
    public static class GenreDebugHelper
    {
        public static void LogAllGenreNames()
        {
            try
            {
                GameObject mainObj = GameObject.Find("Main");
                if (mainObj == null)
                {
                    Plugin.Logger.LogError("Main GameObject not found!");
                    return;
                }

                genres genresScript = mainObj.GetComponent<genres>();
                if (genresScript == null)
                {
                    Plugin.Logger.LogError("genres script not found!");
                    return;
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("=== ALL GENRE NAMES AND IDS ===");

                for (int i = 0; i < genresScript.genres_LEVEL.Length; i++)
                {
                    string name = genresScript.GetName(i);
                    sb.AppendLine($"[{i}] = {name}");
                }

                Plugin.Logger.LogInfo(sb.ToString());
            }
            catch (System.Exception ex)
            {
                Plugin.Logger.LogError($"Error logging genre names: {ex.Message}");
            }
        }
    }
}
