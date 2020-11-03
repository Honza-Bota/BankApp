using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp
{
    class CreditAccount : Account
    {
        public CreditAccount(long accountNumber,
                             string name,
                             string surname,
                             DateTime birthdate,
                             AccountTypes accountType,
                             double moneyValue = 0) : base(accountNumber, name, surname, birthdate, accountType, moneyValue)
        {
        }

        public override bool Deposit(int amount, string message)
        {
            bool inserted = false;

            if (amount > 0 && MoneyValue+amount <= 0)
            {
                MoneyValue += amount;
                LogTransaction($"Vložena částka {amount} se zprávou {message}");
                inserted = true;
            }
            else
            {
                inserted = false;
            }

            return inserted;
        }

        public override bool Witherdraw(int amount, string message)
        {
            bool witherdrawed = false;

            if (amount > 0 && MoneyValue - amount >= Settings.CreditLimit * -1)
            {
                MoneyValue -= amount;
                LogTransaction($"Vybrána částka {amount} se zprávou {message}");
                witherdrawed = true;
            }
            else
            {
                witherdrawed = false;
            }

            return witherdrawed;
        }

        public override void MakeInterest()
        {
            if (MoneyValue > (Settings.CreditLimit * -1) - 25_000)
            {
                MoneyValue += ((MoneyValue / 100) * Settings.CreditInterest) / 12; 
            }
        }
    }
}
