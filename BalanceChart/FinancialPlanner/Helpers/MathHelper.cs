using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancialPlanner.Helpers
{
    public static class MathHelper
    {
        public static double GetTemporalMean(this IEnumerable<Tuple<double, double>> sequence)
        {
            var sum1 = 0.0;
            var sum2 = 0.0;
            foreach (var t in sequence)
            {
                sum1 += t.Item1;
                sum2 += t.Item1 * t.Item2;
            }
            return sum2/sum1;
        }
    }
}
