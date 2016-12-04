using System;

namespace FinancialPlanner.Helpers
{
    public static class RateHelper
    {
        /// <summary>
        ///  Returns the day rate from anual rate
        /// </summary>
        /// <param name="anualRate">The anual rate</param>
        /// <param name="numDays">The number of days in the year</param>
        /// <returns>The day rate</returns>
        public static double AnualRateToDayRate(double anualRate, double numDays = 365) => Math.Pow(1 + anualRate, 1.0 / numDays) - 1;

        public static double DayRateToMonthRate(double dayRate, double numDaysPerMonth = 30) => Math.Pow(1 + dayRate, numDaysPerMonth) - 1;

    }
}
