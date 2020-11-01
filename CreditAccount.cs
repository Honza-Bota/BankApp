using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp
{
    class CreditAccount : Account
    {
        public CreditAccount(long accountNumber, string name, string surname, DateTime birthdate, AccountTypes accountType, int moneyValue = 0) : base(accountNumber, name, surname, birthdate, accountType, moneyValue)
        {
        }
    }
}
