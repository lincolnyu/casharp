using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancialPlanner.Loans
{
    public class CompositeLoan : ILoan
    {
        public delegate IList<double> IncomeSplitterDelegate(CompositeLoan self, double income);

        public readonly List<SingleLoan> Loans = new List<SingleLoan>();

        public CompositeLoan(IncomeSplitterDelegate splitter)
        {
            IncomeSplitter = splitter;
        }

        public IncomeSplitterDelegate IncomeSplitter;

        public double CurrentBalance => Loans.Sum(x => x.CurrentBalance);

        public void ReassignStart(DateTime date)
        {
            for (var i = 0; i < Loans.Count; i++)
            {
                Loans[i].ReassignStart(date);
            }
        }
        public void SetToDate(DateTime date)
        {
            for (var i = 0; i < Loans.Count; i++)
            {
                Loans[i].SetToDate(date);
            }
        }
        public void RunOneDay(double netIncome)
        {
            var allots = IncomeSplitter(this, netIncome);
            for (var i = 0; i < Loans.Count; i++)
            {
                var allot = allots[i];
                Loans[i].RunOneDay(allot);
            }
        }
        public void Add(SingleLoan loan)
        {
            Loans.Add(loan);
        }
    }
}
