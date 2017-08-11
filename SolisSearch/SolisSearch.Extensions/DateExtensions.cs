using System;

namespace SolisSearch.Extensions
{
    public static class DateExtensions
    {
        public static DateTime AsLocalKind(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Local)
                return dateTime;
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
        }

        public static DateTime AsUtcKind(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime;
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }
    }
}
