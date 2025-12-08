using System.Collections.Generic;
using UnityEngine;

namespace MGT2AssistantButton.Core
{
    public static class MarketingAutomationManager
    {
        // Tracks Game IDs that have "Auto Development" enabled (Special Marketing)
        public static HashSet<int> AutoSpecialMarketingGames = new HashSet<int>();

        // Tracks Task IDs that have "Auto-Switch" enabled (Marketing Campaign)
        public static HashSet<int> TasksWithAutoSwitch = new HashSet<int>();

        // Special Marketing
        public static void SetAutoSpecialMarketing(int gameId, bool enabled)
        {
            if (enabled)
            {
                if (!AutoSpecialMarketingGames.Contains(gameId))
                    AutoSpecialMarketingGames.Add(gameId);
            }
            else
            {
                if (AutoSpecialMarketingGames.Contains(gameId))
                    AutoSpecialMarketingGames.Remove(gameId);
            }
        }

        public static bool IsAutoSpecialMarketing(int gameId)
        {
            return AutoSpecialMarketingGames.Contains(gameId);
        }

        // Marketing Campaign Auto-Switch
        public static void SetAutoSwitch(int taskId, bool enabled)
        {
            if (enabled)
            {
                if (!TasksWithAutoSwitch.Contains(taskId))
                    TasksWithAutoSwitch.Add(taskId);
            }
            else
            {
                if (TasksWithAutoSwitch.Contains(taskId))
                    TasksWithAutoSwitch.Remove(taskId);
            }
        }

        public static bool IsAutoSwitch(int taskId)
        {
            return TasksWithAutoSwitch.Contains(taskId);
        }

        // Legacy - keep for compatibility but not used
        public static HashSet<int> TasksWithAuto100 = new HashSet<int>();
        public static void SetAuto100(int taskId, bool enabled) { }
        public static bool IsAuto100(int taskId) { return false; }
    }
}
