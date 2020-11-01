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
        AccountDatabse dtbAcounts1;
        public BankInterfaceForm()
        {
            InitializeComponent();

            //naplnění comboboxu
            foreach (AccountTypes item in Enum.GetValues(typeof(AccountTypes)))
            {
                cbAccountType.Items.Add(item); 
            }
            
            //inicializace "databáze"
            dtbAcounts1 = new AccountDatabse();
        }

        private void butCreate_Click(object sender, RoutedEventArgs e)
        {
            //proměné kontrolující korektnost vstupů
            bool correctInputAccountNumber = true;
            bool correctInputNameSurname = true;
            bool correctInputDeposite = true;

            //oveření vstupu čísla, případně doplnění
            if (tbAccountNumber.Text == "") tbAccountNumber.Text = Account.Count.ToString();
            long accountNumber;
            correctInputAccountNumber = long.TryParse(tbAccountNumber.Text, out accountNumber);

            //ověření jména, musí nějaké být
            string name = tbName.Text;
            string surname = tbSurname.Text;
            if (name == "" || surname == "") correctInputNameSurname = false;

            //ověření data, pokud není vybráno, doplní se aktuální
            DateTime birthdate = (DateTime)(dtpBirthdate.SelectedDate == null ? DateTime.Now : dtpBirthdate.SelectedDate);

            //ověření a výběr typu účtu, při nevybrání vybrán debetní
            if (cbAccountType.SelectedIndex == -1) cbAccountType.SelectedIndex = 0;
            AccountTypes accountType = (AccountTypes)cbAccountType.SelectedIndex;

            //ověření zadání vkladu, při nevyplnění doplněna 0
            if (tbDeposit.Text == "") tbDeposit.Text = "0";
            int deposit;
            correctInputDeposite = int.TryParse(tbDeposit.Text, out deposit);

            //pokud je vše správně, vytvoří se příslušný účet a zapíše do "databáze"
            if (correctInputAccountNumber && correctInputNameSurname && correctInputDeposite)
            {
                Account newAccount = new DebetAccount(0,"","",new DateTime(),AccountTypes.Debetní);

                if (accountType == AccountTypes.Debetní) newAccount = new DebetAccount(accountNumber,name,surname,birthdate,accountType,deposit);
                else if (accountType == AccountTypes.Kreditní) newAccount = new CreditAccount(accountNumber, name, surname, birthdate, accountType, deposit);
                else if (accountType == AccountTypes.Studentský) newAccount = new StudentAccount(accountNumber, name, surname, birthdate, accountType, deposit);

                
                dtbAcounts1.Add(newAccount.AccountNumber,newAccount);
            }
            else
            {
                //pokud dojde k vyplnění špatných hodnot, vyvolán messageBox s chybami
                string err = $@"Přehled chybně zadaných údajů:

- Číslo účtu: {(correctInputAccountNumber == false ? "Chyba" : "Správně"), 5}
- Jméno a příjmení: {(correctInputNameSurname == false ? "Chyba" : "Správně"), 5}
- Počáteční vklad: {(correctInputDeposite == false ? "Chyba" : "Správně"), 5}
";
                MessageBox.Show(err, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
            }

        }
    }

}
