using System;

namespace QuitSmartApp.Helpers
{
    // Date and time calculation utilities for quit smoking app
    public static class DateHelper
    {
        // Calculate days between two dates
        public static int CalculateDaysBetween(DateOnly startDate, DateOnly endDate)
        {
            return (endDate.ToDateTime(TimeOnly.MinValue) - startDate.ToDateTime(TimeOnly.MinValue)).Days;
        }

        // Calculate total days since quit start date
        public static int CalculateDaysSinceQuit(DateOnly quitStartDate)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return Math.Max(0, CalculateDaysBetween(quitStartDate, today));
        }

        // Get week start date (Monday) for a given date
        public static DateOnly GetWeekStartDate(DateOnly date)
        {
            var dayOfWeek = (int)date.DayOfWeek;
            var daysToSubtract = dayOfWeek == 0 ? 6 : dayOfWeek - 1; // Monday = 1, Sunday = 0
            return date.AddDays(-daysToSubtract);
        }

        // Get month start date for a given date
        public static DateOnly GetMonthStartDate(DateOnly date)
        {
            return new DateOnly(date.Year, date.Month, 1);
        }
    }
}
