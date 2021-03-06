﻿using System;

namespace FinancialPlanner.Helpers
{
    public static class DateHelper
    {
        public static int GetNumDaysInMonth(DateTime date) => DateTime.DaysInMonth(date.Year, date.Month);

        public static int GetNumDaysInYear(DateTime date) => DateTime.IsLeapYear(date.Year) ? 366 : 365;

        public static bool YearsPast(DateTime a, DateTime b, int years) => b >= a.AddYears(years);
    }
}
