using System;
using System.Text;
using GeneralTools.Localization;
using UnityEngine;

namespace GeneralTools.Tools
{
    public struct TimeInfo
    {
        public int Days { get; }
        public int Hours { get; }
        public int Minutes { get; set; }
        public int Seconds { get; set;}

        public TimeInfo(float seconds)
        {
            Seconds = (int)Math.Ceiling(seconds);

            Minutes = Seconds / 60;
            Seconds -= Minutes * 60;

            Hours = Minutes / 60;
            Minutes -= Hours * 60;

            Days = Hours / 24;
            Hours -= Days * 24;
        }
    }
    
    public class TimeAndValuesDrawer
    {
        private static Language _lastLang = Language.None;
        
        private static string
            _localizedDD = "d",
            _localizedHH = "h",
            _localizedMM = "m",
            _localizedSS = "s";
        
        private static void UpdateTokens()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif

            var current = Localization.Localization.CurrentLanguage;
            if (_lastLang == current) return;
            _lastLang = current;

            _localizedDD = "dd".Localized();
            _localizedHH = "hh".Localized();
            _localizedMM = "mm".Localized();
            _localizedSS = "ss".Localized();
        }
        
        public static TimeInfo GetTimeInfo(float timeInSeconds)
        {
            return new TimeInfo(timeInSeconds);
        }

        public static string GetTimeStr(float timeInSeconds, int maxValuesCount = 2, bool printZeros = false)
        {
            var info = new TimeInfo(timeInSeconds);
            return GetTimeStr(info, maxValuesCount, printZeros);
        }

        public static string GetTimeStr(TimeInfo time, int maxValuesCount = 2, bool printZeros = false)
        {
            UpdateTokens();

            const string space = " ";
            var timeBuilder = new StringBuilder();
            var valuesCount = 0;

            if (time.Days != 0)
            {
                timeBuilder.Append($"{time.Days}{_localizedDD}");
                if (++valuesCount == maxValuesCount) return timeBuilder.ToString();
            }

            if (time.Hours != 0 || (printZeros && time.Days > 0))
            {
                if (timeBuilder.Length > 0) timeBuilder.Append(space);
                timeBuilder.Append($"{time.Hours}{_localizedHH}");
                if (++valuesCount == maxValuesCount) return timeBuilder.ToString();
            }

            if (time.Minutes != 0 || (printZeros && (time.Days + time.Hours) > 0))
            {
                if (timeBuilder.Length > 0) timeBuilder.Append(space);
                timeBuilder.Append($"{time.Minutes}{_localizedMM}");
                if (++valuesCount == maxValuesCount) return timeBuilder.ToString();
            }

            if (time.Seconds != 0 || (printZeros && (time.Days + time.Hours + time.Minutes) > 0) || timeBuilder.Length == 0)
            {
                if (timeBuilder.Length > 0) timeBuilder.Append(space);
                timeBuilder.Append($"{time.Seconds}{_localizedSS}");
            }

            return timeBuilder.ToString();
        }
    }
}