using FinancialPlanner;
using FinancialPlanner.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancialPlannerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var startDate = new DateTime(2017, 1, 1);
            var sim = DailyDlf.CreateMonthlyPaySimulator(60000, 2500,
                220000, 0.041, 150000, 0.0437, 5);
            sim.Check += s =>
            {
                if (s.Loan.CurrentBalance <= 0)
                {
                    Console.WriteLine($"{s.CurrentTime}: Loan clear");
                    return false;
                }
                else
                {
                    if (s.CurrentTime.Day == 1)
                    {
                        Console.WriteLine($"{s.CurrentTime}:  {s.Loan.CurrentBalance}");
                    }
                }
                return true;
            };
            sim.Loan.ReassignStart(startDate);
            var startBalance = sim.Loan.CurrentBalance;
            var clone = sim.Loan.Clone();
            sim.Simulate(startDate);

            var solver = new DailyDlfSolver();
            var reqni = solver.Solve(clone.Clone(), startDate, startDate.AddYears(20));
            Console.WriteLine($"To repay in 20 years, required daily net income is {reqni}");

            KeyValuePair<DateTime, double>[] balseq =
            {
                new KeyValuePair<DateTime, double>( startDate.AddYears(1), startBalance * 0.95 ),
                new KeyValuePair<DateTime, double>( startDate.AddYears(2), startBalance * 0.89 ),
            };
            var seq = solver.Fit(clone.Clone(), startDate, balseq).ToList();
            foreach (var item in seq)
            {
                Console.WriteLine($"{item.Key}->{item.Value}");
            }
            var mean = seq.Select(x => new Tuple<double, double>(x.Key.Days, x.Value)).GetTemporalMean();
            Console.WriteLine($"Mean net income: {mean}");
        }
    }
}
