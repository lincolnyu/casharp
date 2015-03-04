using System;
using BalanceChart.Data;

namespace BalanceChart.Presentation
{
    public class ChartingEngine
    {
        #region Fields

        private double _minX;

        private double _maxX;

        private double _minY;

        private double _maxY;

        private double _timeStep;

        private AccountManager _accountManager;

        #endregion

        #region Methods

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

        public double MinX
        {
            get { return _minX; }
            set
            {
                if (Math.Abs(_minX - value) > double.Epsilon)
                {
                    _minX = value;
                    UpdateView();
                }
            }
        }

        public double MaxX
        {
            get { return _maxX; }
            set
            {
                if (Math.Abs(_maxX - value) > double.Epsilon)
                {
                    _maxX = value;
                    UpdateView();
                }
            }
        }

        public double MinY
        {
            get { return _minY; }
            set
            {
                if (Math.Abs(_minY - value) > double.Epsilon)
                {
                    _minY = value;
                    UpdateView();
                }
            }
        }

        public double MaxY
        {
            get { return _maxY; }
            set
            {
                if (Math.Abs(_maxY - value) > double.Epsilon)
                {
                    _maxY = value;
                    UpdateView();
                }
            }
        }

        public double TimeStep
        {
            get { return _timeStep; }
            set
            {
                if (Math.Abs(_timeStep - value) > double.Epsilon)
                {
                    _timeStep = value;
                    UpdateView();
                }
            }
        }

        #endregion

        #region Methods

        public void FitAll()
        {
            throw new NotImplementedException();
            //UpdateView();
        }

        private void UpdateView()
        {
            foreach (var account in AccountManager.Accounts)
            {
                for (var x = MinX; x < MaxX; x += TimeStep)
                {
                    
                }
            }
        }

        #endregion
    }
}
