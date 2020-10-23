using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BankApp
{
    /// <summary>
    /// Interakční logika pro BankInterfaceForm.xaml
    /// </summary>
    public partial class BankInterfaceForm : Window
    {
        public BankInterfaceForm()
        {
            InitializeComponent();
            foreach (AccountTypes item in Enum.GetValues(typeof(AccountTypes)))
            {
                cbAccountType.Items.Add(item); 
            }
        }
    }

}
