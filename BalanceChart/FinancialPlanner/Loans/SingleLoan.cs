using FinancialPlanner.Helpers;
using System;

namespace FinancialPlanner.Loans
{
    public class SingleLoan : ILoan
    {
        public enum Statuses
        {
            Closed,
            Active
        }

        public delegate bool DeductInterestSignal(SingleLoan self);
        public delegate void LoanEventHandler(SingleLoan self);

        private double _currentAnualRate;
        private double _accumulatedInterest;

        public SingleLoan(DeductInterestSignal deductInterest)
        {
            DeductInterest = deductInterest;
        }

        public double InitBalance { get; set; }
        public DateTime InitDate { get; set; }

        public double CurrentBalance { get; set; }
        public DateTime CurrentDate { get; set; }

        public double CurrentAnualRate
        {
            get { return _currentAnualRate; }
            set
            {
                if (_currentAnualRate != value)
                {
                    _currentAnualRate = value;
                    RecalculateDayRate();
                }
            }
        }

        public double CurrentDayRate
        {
            get; private set;
        }

        public Statuses Status { get; set; }

        public event LoanEventHandler LoanDailyEvent;
        public DeductInterestSignal DeductInterest { get; private set; }

        #region ILoan members

        public void ReassignStart(DateTime date)
        {
            InitDate = date;
            Reset();
        }

        public void SetToDate(DateTime date)
        {
            if (date < InitDate)
            {
                CurrentDate = date;
                CurrentBalance = 0;
            }
            else if (date < CurrentDate)
            {
                Reset();
            }
            while (CurrentDate < date)
            {
                RunOneDay(0);
            }
        }

        public void RunOneDay(double netIncome)
        {
            if (CurrentDate < InitDate || IsClosed())
            {
                CurrentBalance = 0;
                if (!IsClosed())
                {
                    CurrentDate = CurrentDate.AddDays(1);
                    if (CurrentDate >= InitDate)
                    {
                        CurrentBalance = InitBalance;
                    }
                }
            }
            else
            {
                if (DeductInterest(this))
                {
                    CurrentBalance += _accumulatedInterest;
                    _accumulatedInterest = 0;
                }
                else if (CurrentBalance > 0) // loan account doesn't accrue interest
                {
                    _accumulatedInterest += CurrentDayRate * CurrentBalance;
                }
                CurrentBalance -= netIncome;
                CurrentDate = CurrentDate.AddDays(1);
            }
            LoanDailyEvent?.Invoke(this);
        }

        public ILoan Clone(ILoan buf = null)
        {
            var clone = buf as SingleLoan;
            if (clone == null || clone == this)
            {
                clone = new SingleLoan(DeductInterest);
            }
            CopyTo(clone);
            return clone;
        }

        #endregion

        public void CopyTo(SingleLoan other)
        {
            other.InitBalance = InitBalance;
            other.InitDate = InitDate;

            other.CurrentBalance = CurrentBalance;
            other.CurrentDate = CurrentDate;

            other.CurrentAnualRate = CurrentAnualRate;
            other.DeductInterest = DeductInterest;
            other.LoanDailyEvent = LoanDailyEvent; // TODO is this right?

            other.Status = Status;
        }

        public void Reset()
        {
            CurrentBalance = InitBalance;
            CurrentDate = InitDate;
            Status = Statuses.Active;
        }

        public bool IsClosed() => Status == Statuses.Closed;
        private void RecalculateDayRate()
        {
            var numDaysInYear = DateHelper.GetNumDaysInYear(CurrentDate);
            CurrentDayRate = RateHelper.AnualRateToDayRate(CurrentAnualRate, numDaysInYear);
        }

    }
}
