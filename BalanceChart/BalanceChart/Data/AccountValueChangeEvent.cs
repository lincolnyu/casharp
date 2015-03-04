using System;

namespace BalanceChart.Data
{
    /// <summary>
    ///  An account balance change event
    /// </summary>
    public class AccountValueChangeEvent
    {
        #region Enumerations

        /// <summary>
        ///  Possible types of a change event
        /// </summary>
        public enum EventTypes
        {
            Change,
            SetTo
        }

        #endregion

        #region Properties

        /// <summary>
        /// Type of this event  
        /// </summary>
        public EventTypes EventType { get; set; }

        /// <summary>
        ///  When this occurs
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        ///  The value of change or balance depending on the event type
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        ///  The balance
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        ///  The description of the change
        /// </summary>
        public string Description { get; set; }

        #endregion
    }
}
