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
                var daily = Solve(loan, ct, next, target, true);
                var ts = next - ct;
                yield return new KeyValuePair<TimeSpan, double>(ts, daily);
                ct = next;
            }
        }

        public double Solve(ILoan loan, DateTime start, DateTime complete, double target = 0, bool update = false)
        {
            var minIncome = 0.0;
            var maxIncome = double.MaxValue;
            var income = InitIncome;
            var l = loan;
            while (true)
            {
                l = loan.Clone(l);
                var finish = RunToTarget(l, target, income);
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
                    if (update)
                    {
                        l.Clone(loan);
                    }
                    return income;
                }
            }
        }

        public static DateTime? RunToTarget(ILoan loan, double target, double dailyIncome)
        {
            var ct = loan.CurrentDate;
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
