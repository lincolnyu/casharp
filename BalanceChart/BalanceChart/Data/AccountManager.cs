using System.Collections.Generic;

namespace BalanceChart.Data
{
    public class AccountManager
    {
        #region Constructors

        public AccountManager()
        {
            Accounts = new List<Account>();
        }

        #endregion

        #region Properties

        public List<Account> Accounts { get; private set; } 

        #endregion
    }
}
