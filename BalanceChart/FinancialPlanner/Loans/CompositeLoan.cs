using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancialPlanner.Loans
{
    public class CompositeLoan : ILoan
    {
        public delegate IList<double> IncomeSplitterDelegate(CompositeLoan self, double income);

        public readonly List<ILoan> Loans = new List<ILoan>();

        public CompositeLoan(IncomeSplitterDelegate splitter)
        {
            IncomeSplitter = splitter;
        }

        public IncomeSplitterDelegate IncomeSplitter;

        public double CurrentBalance => Loans.Sum(x => x.CurrentBalance);

        #region ILoan members

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

        public ILoan Clone()
        {
            var clone = new CompositeLoan(IncomeSplitter);
            CopyTo(clone);
            return clone;
        }

        #endregion

        public T At<T>(int index) where T : ILoan
        {
            return (T)Loans[index];
        }

        public void CopyTo(CompositeLoan other)
        {
            other.IncomeSplitter = IncomeSplitter;
            other.Loans.Clear();
            foreach (var loan in Loans)
            {
                other.Loans.Add(loan.Clone());
            }
        }

        public void Add(SingleLoan loan)
        {
            Loans.Add(loan);
        }
    }
}
