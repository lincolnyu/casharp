using FinancialPlanner.Helpers;
using FinancialPlanner.Loans;
using System;
using System.Collections.Generic;

namespace FinancialPlanner
{
    /// <summary>
    ///  Differential Loan Field with daily timestep
    /// </summary>
    public class DailyDlf
    {
        public delegate double IncomdDelegate(DailyDlf simulator);
        public delegate bool CheckDelegate(DailyDlf simulator);

        public DailyDlf(ILoan loan, IncomdDelegate getIncome)
        {
            Loan = loan;
            GetIncome = getIncome;
        }
        public ILoan Loan { get; }
        public DateTime CurrentTime { get; private set; }

        public IncomdDelegate GetIncome { get; }
        public CheckDelegate Check { get; set; }

        /// <summary>
        ///  Simulates over time 
        /// </summary>
        /// <param name="income">The method that returns the income at spcified timestep</param>
        public void Simulate(DateTime start)
        {
            CurrentTime = start;
            Loan.SetToDate(start);
            for (; Check(this); CurrentTime = CurrentTime.AddDays(1))
            {
                var income = GetIncome(this);
                Loan.RunOneDay(income);
            }
        }
        
        public static DailyDlf CreateMonthlyPaySimulator(double anualIncome, double monthlyExpense,
            double variableLoan, double variableRate, double fixedLoan, double fixedRate, int fixedYears, int dayOfPay = 15, int dayOfInterestCharge = 21)
        {
            IncomdDelegate getIncome = ds =>
            {
                if (ds.CurrentTime.Day == dayOfPay)
                {
                    var daysAYear = DateHelper.GetNumDaysInYear(ds.CurrentTime);
                    var daysAMonth = DateHelper.GetNumDaysInMonth(ds.CurrentTime);
                    var income = anualIncome * daysAMonth / daysAYear;
                    return income - monthlyExpense;
                }
                return 0;
            };
            SingleLoan.DeductInterestSignal dis = sl => sl.CurrentDate.Day == dayOfInterestCharge;
            var vl = new SingleLoan(dis) { InitBalance = variableLoan, CurrentAnualRate = variableRate };
            var fl = new SingleLoan(dis) { InitBalance = fixedLoan, CurrentAnualRate = fixedRate };
            fl.LoanDailyEvent += l =>
            {
                if (DateHelper.YearsPast(l.InitDate, l.CurrentDate, fixedYears))
                {
                    l.CurrentAnualRate = variableRate;
                }
            };
            var compLoan = new CompositeLoan((cl, ic) =>
            {
                var varloanRepay = Math.Min(cl.Loans[0].CurrentBalance, ic);
                var fixedLoanRepay = ic - varloanRepay;
                if (!DateHelper.YearsPast(cl.At<SingleLoan>(1).InitDate, cl.At<SingleLoan>(1).CurrentDate, fixedYears) && fixedLoanRepay > 0)
                {
                    varloanRepay += fixedLoanRepay;
                    fixedLoanRepay = 0;
                }
                return new[] { varloanRepay, fixedLoanRepay };
            });
            compLoan.Add(vl);
            compLoan.Add(fl);
            return new DailyDlf(compLoan, getIncome);
        }
    }
}
