using System;
using Balancy;
using Balancy.Models.BalancyShop;
using Balancy.Models.LiveOps.Store;

namespace BalancyShop
{
    public static class GameUtils
    {
        public static string FormatTime(int totalSeconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(totalSeconds);

            string result = "";
            if (timeSpan.Days > 0)
            {
                result += $"{timeSpan.Days}d ";
            }
            if (timeSpan.Hours > 0)
            {
                result += $"{timeSpan.Hours}h ";
            }
            if (timeSpan.Minutes > 0)
            {
                result += $"{timeSpan.Minutes}m ";
            }
            if (timeSpan.Seconds > 0)
            {
                result += $"{timeSpan.Seconds}s";
            }

            return result.Trim(); // Trim to remove any extra whitespace at the end
        }

        public static BadgeInfo FindBadgeInfo(SlotType slotType)
        {
            if (slotType == SlotType.Default)
                return null;
            
            var allDocs = DataEditor.BalancyShop.BadgeInfos;
            foreach (var doc in allDocs)
            {
                if (doc.Type == slotType)
                    return doc;
            }

            return null;
        }
    }
}

