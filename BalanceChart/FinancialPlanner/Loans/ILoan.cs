using System;

namespace FinancialPlanner.Loans
{
    public interface ILoan
    {
        double CurrentBalance { get; }

        void ReassignStart(DateTime date);
        void SetToDate(DateTime date);
        void RunOneDay(double netIncome);
    }
}
