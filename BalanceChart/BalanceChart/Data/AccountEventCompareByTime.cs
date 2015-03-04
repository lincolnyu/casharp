using System.Collections.Generic;

namespace BalanceChart.Data
{
    /// <summary>
    ///  Compares two events by time
    /// </summary>
    public class AccountEventCompareByTime : IComparer<AccountValueChangeEvent>
    {
        #region Nested types

        /// <summary>
        ///  Compares two account events by time
        /// </summary>
        /// <param name="x">The first event</param>
        /// <param name="y">The second event</param>
        /// <returns>Zero if equal, positive if the first is greater than the second and vice versa</returns>
        public int Compare(AccountValueChangeEvent x, AccountValueChangeEvent y)
        {
            return x.Time.CompareTo(y.Time);
        }

        #endregion
    }
}
