using FinancialPlanner;
using System;

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
            sim.Simulate(startDate);

            var reqni = sim.Solve(startDate, startDate.AddYears(20));
            Console.WriteLine($"To repay in 20 years, required daily net income is {reqni}");
        }
    }
}
