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
    /// Interakční logika pro LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void butOk_Click(object sender, RoutedEventArgs e)
        {
            string name = tbName.Text;
            string password = tbPassword.Password;

            if (name == "admin" && password == "root")
            {
                BankInterfaceForm bank = new BankInterfaceForm();
                bank.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Špatné přihlašovací údaje",
                            "Chyba v ověření",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning,
                            MessageBoxResult.OK);
            }
        }

        private void butCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult rslt = MessageBox.Show("Chcete opravdu ukončit aplikaci?",
                                                    "Ukončování",
                                                    MessageBoxButton.YesNo,
                                                    MessageBoxImage.Question,
                                                    MessageBoxResult.No);

            if (rslt == MessageBoxResult.Yes) this.Close();
        }
    }
}
