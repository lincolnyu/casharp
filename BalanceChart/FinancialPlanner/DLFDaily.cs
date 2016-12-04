using FinancialPlanner.Helpers;
using FinancialPlanner.Loans;
using System;

namespace FinancialPlanner
{
    public class DLFDaily
    {
        public delegate double IncomdDelegate(DLFDaily simulator);
        public delegate bool CheckDelegate(DLFDaily simulator);

        public DLFDaily(ILoan loan, IncomdDelegate getIncome)
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

        public static DLFDaily CreateMonthlyPaySimulator(double anualIncome, double monthlyExpense,
            double variableLoan, double variableRate, double fixedLoan, double fixedRate, double fixedYears, int dayOfPay = 15, int dayOfInterestCharge = 21)
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
                if ((l.CurrentDate - l.InitDate).TotalDays > 365*5+1)
                {
                    l.CurrentAnualRate = variableRate;
                }
            };
            var compLoan = new CompositeLoan((cl, ic) => new[] { ic, 0 });
            compLoan.Add(vl);
            compLoan.Add(fl);
            return new DLFDaily(compLoan, getIncome);
        }
    }
}
