using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace BankApp
{
    class AccountDatabse : Dictionary<long,Account>
    {
        const string path = "data.txt";

        public bool Load()
        {
            if (File.Exists(path))
            {
                string[] logs = File.ReadAllLines(path);

                foreach (var item in logs)
                {
                    string[] data = item.Split(';');

                    long accNumber = long.Parse(data[0]);
                    string name = data[1];
                    string surname = data[2];
                    DateTime birthdate = DateTime.Parse(data[3]);
                    AccountTypes accType = (AccountTypes)int.Parse(data[4]);
                    double moneyValue = double.Parse(data[5]);

                    if (accType == AccountTypes.Debetní) Add(accNumber, new DebetAccount(accNumber, name, surname, birthdate, accType, moneyValue));
                    else if (accType == AccountTypes.Kreditní) Add(accNumber, new CreditAccount(accNumber, name, surname, birthdate, accType, moneyValue));
                    else if (accType == AccountTypes.Studentský) Add(accNumber, new StudentAccount(accNumber, name, surname, birthdate, accType, moneyValue));
                }
                return true;
            }
            return false;
        }

        public bool Save()
        {
            File.Create(path).Close();

            string data = "";

            foreach (var item in this)
            {
                Account a = item.Value;

                data += $"{a.AccountNumber};{a.Name};{a.Surname};{a.Birthdate};{(int)a.AccountType};{a.MoneyValue}\n";
            }

            File.WriteAllText(path, data);

            return true;
        }
    }
}
