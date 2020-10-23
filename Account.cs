﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp
{
    public abstract class Account
    {
        public int AccountNumber { get; private set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthdate { get; set; }
        public AccountTypes AccountType { get; private set; }
        public int MoneyValue { get; set; }
        public List<string> TransactionLog { get; set; }

        public Account(int accountNumber, string name, string surname, DateTime birthdate, AccountTypes accountType, int moneyValue = 0)
        {
            AccountNumber = accountNumber;
            Name = name;
            Surname = surname;
            Birthdate = birthdate;
            AccountType = accountType;
            MoneyValue = moneyValue;
        }


        public virtual void Witherdraw() { }

        public virtual void Deposit() { }

        protected virtual void LogTransaction() { }

    }
}