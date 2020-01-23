using System;
using TimeZoneConverter;

namespace MyHomeMeasure.Utl
{
    public static class DateUtil
    {

        public static DateTime Now => DateTime.UtcNow;

        public static DateTime JstNow => Now.ConvertUtcToJst();

        private static readonly TimeZoneInfo _jstTimeZone = TZConvert.GetTimeZoneInfo("Tokyo Standard Time");

        public static DateTime ConvertUtcToJst(this DateTime utc)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utc, _jstTimeZone);
        }

        public static DateTime ConvertJstToUtc(this DateTime jst)
        {
            return TimeZoneInfo.ConvertTimeToUtc(jst, _jstTimeZone);
        }

        public static DateTime GetFirstDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime GetFirstDayOfWeek(this DateTime date)
        {
            while(date.DayOfWeek == DayOfWeek.Sunday)
            {
                date = GetFirstDayOfMonth(date.AddDays(-1));
            }

            return date;
        }
    }
}
