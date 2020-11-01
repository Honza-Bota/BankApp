using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp
{
    class DebetAccount : Account
    {
        public int Interest { get; set; }
        public int LimitWitherdraw { get; set; }
        public DebetAccount(long accountNumber,
                            string name,
                            string surname,
                            DateTime birthdate,
                            AccountTypes accountType,
                            int moneyValue = 0) : base(accountNumber, name, surname, birthdate, accountType, moneyValue)
        {
            Interest = Settings.DebetInterest;
            LimitWitherdraw = Settings.DebetLimitWitherdraw;
        }
    }
}
