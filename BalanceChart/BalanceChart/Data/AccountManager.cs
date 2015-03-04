using System.Collections.Generic;

namespace BalanceChart.Data
{
    /// <summary>
    ///  A manager that holds manages a couple of accounts
    /// </summary>
    public class AccountManager
    {
        #region Constructors

        /// <summary>
        ///  Constructs an account manager object
        /// </summary>
        public AccountManager()
        {
            Accounts = new List<Account>();
        }

        #endregion

        #region Properties

        /// <summary>
        ///  All accounts the manager manages
        /// </summary>
        public List<Account> Accounts { get; private set; } 

        #endregion
    }
}
