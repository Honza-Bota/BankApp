using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BankApp
{
    public abstract class Account
    {
     
        //základní vlastnosti účtu
        public static int CreatedCount { get; set; } = 0;
        public long AccountNumber { get; private set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthdate { get; set; }
        public AccountTypes AccountType { get; private set; }
        public double MoneyValue { get; set; }
        public List<string> TransactionLog { get; set; }

        public Account(long accountNumber, string name, string surname, DateTime birthdate, AccountTypes accountType, double moneyValue = 0)
        {
            AccountNumber = accountNumber;
            Name = name;
            Surname = surname;
            Birthdate = birthdate;
            AccountType = accountType;
            MoneyValue = moneyValue;

            CreatedCount++;
            TransactionLog = new List<string>();
        }

        public override string ToString()
        {
            return $"~ Číslo účtu: {AccountNumber,2}; " +
                $"Jméno a příjmení: {Name,2} {Surname,0}; " +
                $"Datum narození: {Birthdate.ToShortDateString(),2}; " +
                $"Typ účtu: {AccountType,2}; " +
                $"Finance: {MoneyValue,2} Kč";
        }


        public virtual bool Witherdraw(int amount, string message) { return false; }

        public virtual bool Deposit(int amount, string message) { return false; }

        protected virtual void LogTransaction(string log) => TransactionLog.Add(log);

        public void ShowLog()
        {
            string data = $"Záznamy: \n";
            foreach (var item in TransactionLog)
            {
                data += $"~ {item}\n";
            }
            MessageBox.Show(data);
        }

        public virtual void MakeInterest()
        {
        }
    }
}
