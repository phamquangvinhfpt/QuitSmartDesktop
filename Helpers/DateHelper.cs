using System;

namespace QuitSmartApp.Helpers
{
    /// <summary>
    /// Date and time calculation utilities for quit smoking app
    /// </summary>
    public static class DateHelper
    {
        /// <summary>
        /// Calculate days between two dates
        /// </summary>
        public static int CalculateDaysBetween(DateOnly startDate, DateOnly endDate)
        {
            return (endDate.ToDateTime(TimeOnly.MinValue) - startDate.ToDateTime(TimeOnly.MinValue)).Days;
        }

        /// <summary>
        /// Calculate total days since quit start date
        /// </summary>
        public static int CalculateDaysSinceQuit(DateOnly quitStartDate)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return Math.Max(0, CalculateDaysBetween(quitStartDate, today));
        }

        /// <summary>
        /// Get week start date (Monday) for a given date
        /// </summary>
        public static DateOnly GetWeekStartDate(DateOnly date)
        {
            var dayOfWeek = (int)date.DayOfWeek;
            var daysToSubtract = dayOfWeek == 0 ? 6 : dayOfWeek - 1; // Monday = 1, Sunday = 0
            return date.AddDays(-daysToSubtract);
        }

        /// <summary>
        /// Get month start date for a given date
        /// </summary>
        public static DateOnly GetMonthStartDate(DateOnly date)
        {
            return new DateOnly(date.Year, date.Month, 1);
        }
    }
}
