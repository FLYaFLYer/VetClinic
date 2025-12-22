using System;

namespace VetClinic.Utils
{
    public static class DateExtensions
    {
        public static bool DateEquals(this DateTime date1, DateTime date2)
        {
            return date1.Year == date2.Year &&
                   date1.Month == date2.Month &&
                   date1.Day == date2.Day;
        }

        public static bool DateEquals(this DateTime? date1, DateTime date2)
        {
            return date1.HasValue && DateEquals(date1.Value, date2);
        }
    }
}