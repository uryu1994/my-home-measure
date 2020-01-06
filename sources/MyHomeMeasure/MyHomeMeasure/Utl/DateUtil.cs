using System;
using TimeZoneConverter;

namespace MyHomeMeasure.Utl
{
    public static class DateUtil
    {
        private static readonly TimeZoneInfo _jstTimeZone = TZConvert.GetTimeZoneInfo("Tokyo Standard Time");

        public static DateTime ConvertUtcToJst(this DateTime utc)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utc, _jstTimeZone);
        }

        public static DateTime ConvertJstToUtc(this DateTime jst)
        {
            return TimeZoneInfo.ConvertTimeToUtc(jst, _jstTimeZone);
        }
    }
}
