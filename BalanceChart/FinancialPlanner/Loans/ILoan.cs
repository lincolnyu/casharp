using System;

namespace FinancialPlanner.Loans
{
    public interface ILoan
    {
        double CurrentBalance { get; }
        DateTime CurrentDate { get; }

        void ReassignStart(DateTime date);
        void SetToDate(DateTime date);
        void RunOneDay(double netIncome);

        ILoan Clone(ILoan buf = null);
    }
}
