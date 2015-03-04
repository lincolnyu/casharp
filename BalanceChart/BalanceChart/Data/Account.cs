using System;
using System.Collections.Generic;

namespace BalanceChart.Data
{
    /// <summary>
    ///  The account
    /// </summary>
    public class Account
    {
        #region Enumerations

        public enum States
        {
            Inactive,
            Active,
            Disabled,
        }

        #endregion

        #region Fields

        /// <summary>
        ///  The comparer used to order and search events
        /// </summary>
        public readonly static AccountEventCompareByTime EventsComparerByTime = new AccountEventCompareByTime();
        
        #endregion

        #region Constructors

        /// <summary>
        ///  Instantiates an account
        /// </summary>
        public Account()
        {
            Events = new List<AccountValueChangeEvent>();
        }

        #endregion

        #region Properties

        /// <summary>
        ///  Unique name for this account
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///  Description of this account
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///  All the events that happened
        /// </summary>
        public List<AccountValueChangeEvent> Events { get; private set; }

        /// <summary>
        ///  The time when this account is opened/created
        /// </summary>
        public DateTime InitialTime { get; set; }
        
        /// <summary>
        ///  The balance when this account is opened/created
        /// </summary>
        public double InitialBalance { get; set; }

        /// <summary>
        ///  The current account state
        /// </summary>
        public States State { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///  Get the balance at the specified time
        /// </summary>
        /// <param name="time">The time to get the balance at</param>
        /// <returns>The balance</returns>
        public double GetBalance(DateTime time)
        {
            var dummyEvent = new AccountValueChangeEvent
            {
                Time = time
            };
            var index = Events.BinarySearch(dummyEvent, EventsComparerByTime);
            var exactTime = index >= 0;
            if (!exactTime)
            {
                index = -index - 1;
            }
            if (Events.Count == 0)
            {
                return 0;
            }
            if (index >= Events.Count)
            {
                return Events[Events.Count - 1].Balance;
            }
            if (exactTime)
            {
                return Events[index].Balance;
            }
            return index > 0 ? Events[index - 1].Balance : 0;
        }

        #endregion
    }
}
