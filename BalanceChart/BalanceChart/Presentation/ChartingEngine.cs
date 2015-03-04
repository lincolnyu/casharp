using System;
using BalanceChart.Data;

namespace BalanceChart.Presentation
{
    /// <summary>
    ///  Balance chart data plotting engine
    /// </summary>
    public class ChartingEngine
    {
        #region Delegates

        /// <summary>
        ///  Delegate to plot a data point
        /// </summary>
        /// <param name="accountIndex">The index of the account in the manager for this data point</param>
        /// <param name="x">X value between 0 and 1</param>
        /// <param name="y">Y value between 0 and 1 if visible</param>
        public delegate void PlotDelegate(int accountIndex, double x, double y);

        #endregion

        #region Fields

        /// <summary>
        ///  Backing field for Plot
        /// </summary>
        private PlotDelegate _plot;

        /// <summary>
        ///  Backing field for MinX
        /// </summary>
        private DateTime _minX;

        /// <summary>
        ///  Backing field for MaxX
        /// </summary>
        private DateTime _maxX;

        /// <summary>
        ///  Backing field for MinY
        /// </summary>
        private decimal _minY;

        /// <summary>
        ///  Backing field for MaxY
        /// </summary>
        private decimal _maxY;

        /// <summary>
        ///  Backing field for TimeStep
        /// </summary>
        private TimeSpan _timeStep;

        /// <summary>
        ///  Backing field for AccountManager
        /// </summary>
        private AccountManager _accountManager;

        #endregion

        #region Properties

        /// <summary>
        ///  The plotting method
        /// </summary>
        public PlotDelegate Plot 
        {
            get { return _plot; }
            set
            {
                if (_plot != value)
                {
                    _plot = value;
                    UpdateView();
                }
            }
        }

        /// <summary>
        ///  The account manager whose accounts are to be drawn here
        /// </summary>
        public AccountManager AccountManager
        {
            get { return _accountManager; }
            set
            {
                if (_accountManager != value)
                {
                    _accountManager = value;
                    UpdateView();
                }
            }
        }

        /// <summary>
        ///  Minimum X (time) value of the view scope
        /// </summary>
        public DateTime MinX
        {
            get { return _minX; }
            set
            {
                if (_minX != value)
                {
                    _minX = value;
                    UpdateView();
                }
            }
        }

        /// <summary>
        ///  Maximum X (time) value of the view scope
        /// </summary>
        public DateTime MaxX
        {
            get { return _maxX; }
            set
            {
                if (_maxX != value)
                {
                    _maxX = value;
                    UpdateView();
                }
            }
        }

        /// <summary>
        ///  Minimum Y (balance) value of the view scope
        /// </summary>
        public decimal MinY
        {
            get { return _minY; }
            set
            {
                if (_minY != value)
                {
                    _minY = value;
                    UpdateView();
                }
            }
        }

        /// <summary>
        ///  Maximum Y (balance) value of the view scope
        /// </summary>
        public decimal MaxY
        {
            get { return _maxY; }
            set
            {
                if (_maxY != value)
                {
                    _maxY = value;
                    UpdateView();
                }
            }
        }

        /// <summary>
        ///  The time step on X axis
        /// </summary>
        public TimeSpan TimeStep
        {
            get { return _timeStep; }
            set
            {
                if (_timeStep != value)
                {
                    _timeStep = value;
                    UpdateView();
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Gets the view scope to fit the data
        /// </summary>
        public void FitAll()
        {
            var ftt = true;
            decimal overalMinBalance = 0, overalMaxBalance = 0;
            DateTime overalMinTime = default(DateTime), overalMaxTime = default(DateTime);
            foreach (var account in AccountManager.Accounts)
            {
                decimal min, max;
                var minTime = account.GetMinTime();
                var maxTime = account.GetMaxTime();
                account.GetBalanceRange(out min, out max);
                if (ftt)
                {
                    overalMinBalance = min;
                    overalMaxBalance = max;
                    overalMinTime = minTime;
                    overalMaxTime = maxTime;
                    ftt = false;
                }
                else
                {
                    if (min < overalMinBalance)
                    {
                        overalMinBalance = min;
                    }
                    if (max > overalMaxBalance)
                    {
                        overalMaxBalance = max;
                    }
                    if (minTime < overalMinTime)
                    {
                        overalMinTime = minTime;
                    }
                    if (maxTime > overalMaxTime)
                    {
                        overalMaxTime = maxTime;
                    }
                }
            }
            if (overalMinTime < overalMaxTime)
            {
                _minX = overalMinTime;
                _maxX = overalMaxTime;
            }
            else
            {
                _minX = overalMinTime - TimeSpan.FromDays(1);
                _maxX = overalMinTime + TimeSpan.FromDays(1);
            }
            _minY = overalMinBalance;
            _maxY = overalMaxBalance;
            UpdateView();
        }

        /// <summary>
        ///  Updates the display
        /// </summary>
        private void UpdateView()
        {
            if (Plot == null)
            {
                return;
            }
            var width = (MaxX - MinX).TotalSeconds;
            var height = (double)(_maxY - _minY);
            for (int i = 0; i < AccountManager.Accounts.Count; i++)
            {
                var account = AccountManager.Accounts[i];
                for (var x = MinX; x < MaxX; x += TimeStep)
                {
                    // TODO the data is retrieved in temporal order and therefore can be optmized
                    var balance = account.GetBalance(x);
                    var vy = ((double) (balance - _minY))/height;
                    var vx = (x - MinX).TotalSeconds/width;
                    Plot(i, vx, vy);
                }
            }
        }

        #endregion
    }
}
