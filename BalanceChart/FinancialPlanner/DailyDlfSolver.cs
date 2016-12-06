using FinancialPlanner.Loans;
using System;
using System.Collections.Generic;

namespace FinancialPlanner
{
    public class DailyDlfSolver
    {
        public TimeSpan Tol { get; set; } = TimeSpan.FromDays(15);
        public double InitIncome { get; set; } = 2000;

        public IEnumerable<KeyValuePair<TimeSpan, double>> Fit(ILoan loan, DateTime start, IList<KeyValuePair<DateTime, double>> points)
        {
            var ct = start;
            for (var i = 0; i < points.Count; i++)
            {
                var next = points[i].Key;
                var target = points[i].Value;
                var daily = Solve(loan, ct, next, target);
                var ts = next - ct;
                yield return new KeyValuePair<TimeSpan, double>(ts, daily);
                ct = next;
            }
        }

        public double Solve(ILoan loan, DateTime start, DateTime complete, double target = 0)
        {
            var minIncome = 0.0;
            var maxIncome = double.MaxValue;
            var income = InitIncome;
            while (true)
            {
                var finish = RunToTarget(loan, start, target, income);
                if (finish + Tol < complete)
                {
                    maxIncome = income;
                    income = (income + minIncome) / 2;
                }
                else if (finish > complete + Tol)
                {
                    minIncome = income;
                    if (maxIncome == double.MaxValue)
                    {
                        income *= 2;
                    }
                    else
                    {
                        income = (income + maxIncome) / 2;
                    }
                }
                else
                {
                    return income;
                }
            }
        }

        public static DateTime? RunToTarget(ILoan loan, DateTime start, double target, double dailyIncome)
        {
            var ct = start;
            loan.SetToDate(start);

            if (target == loan.CurrentBalance) return ct;

            double diff;
            if (target < loan.CurrentBalance)
            {
                var iniDiff = loan.CurrentBalance - target;
                do
                {
                    loan.RunOneDay(dailyIncome);
                    ct = ct.AddDays(1);
                    diff = loan.CurrentBalance - target;
                    if (diff > iniDiff) return null;
                } while (diff > 0);
            }
            else
            {
                var iniDiff = target - loan.CurrentBalance;
                do
                {
                    loan.RunOneDay(dailyIncome);
                    ct = ct.AddDays(1);
                    diff = target - loan.CurrentBalance;
                    if (diff > iniDiff) return null;
                } while (diff > 0);
            }
            return ct;
        }
    }
}
