using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp
{
    class AccountDatabse : Dictionary<long,Account>
    {
        public bool Load()
        {
            throw new Exception();
        }

        public bool Save()
        {
            throw new Exception();
        }
    }
}
