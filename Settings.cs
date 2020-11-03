using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp
{
    public static class Settings
    {
        public static double DebetInterest { get; set; } = 1;
        public static double CreditInterest { get; set; } = 15;
        public static int DebetLimitWitherdraw { get; set; } = 200_000;
        public static int CreditLimit { get; set; } = 150_000;
        public static int StudentLimitWitherdraw { get; set; } = 5_000;
        public static int TimeInterval { get; set; } = 100;

    }
}
