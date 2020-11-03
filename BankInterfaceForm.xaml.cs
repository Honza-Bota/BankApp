using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BankApp
{
    /// <summary>
    /// Interakční logika pro BankInterfaceForm.xaml
    /// </summary>
    public partial class BankInterfaceForm : Window
    {
        AccountDatabse dtbAcounts1;
        Account account;
        DateTime date;
        DispatcherTimer timer;
        bool simulationOn;
        int currentMonth;
        int nextMonth;

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

            //inicializace timeru a data
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1 * (1 / Settings.TimeMultiple));
            timer.Tick += Timer_Tick;
            date = DateTime.Today;
            currentMonth = date.Month;
            simulationOn = false;

            //vytvoření rozhraní datagridu
            //LoadDtgAccounts();

            //načtení hodnot nastavení do textboxů
            LoadSettings();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            date = date.AddDays(1);
            nextMonth = date.Month;
            if (currentMonth != nextMonth) NewMonth();
            currentMonth = nextMonth;
            lblTime.Content = "Aktuální datum: " + date.ToShortDateString();
        }

        private void NewMonth()
        {
            foreach (var item in dtbAcounts1)
            {
                item.Value.MakeInterest();
            }
            MessageBox.Show("Proběhlo měsíční úročení!");
        }

        private void butCreate_Click(object sender, RoutedEventArgs e)
        {
            //proměné kontrolující korektnost vstupů
            bool correctInputAccountNumber = true;
            bool correctInputNameSurname = true;
            bool correctInputDeposite = true;

            //oveření vstupu čísla, případně doplnění
            if (tbAccountNumber.Text == "") tbAccountNumber.Text = Account.CreatedCount.ToString();
            long accountNumber;
            correctInputAccountNumber = long.TryParse(tbAccountNumber.Text, out accountNumber);
            foreach (var item in dtbAcounts1)
            {
                if (item.Key == accountNumber) correctInputAccountNumber = false;
            }
            if (accountNumber < 0) correctInputAccountNumber = false;

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
                //vytvoření konkrétního účtu se zadanými údaji
                Account newAccount = new DebetAccount(0, "", "", new DateTime(), AccountTypes.Debetní);
                Account.CreatedCount--;

                if (accountType == AccountTypes.Debetní) newAccount = new DebetAccount(accountNumber, name, surname, birthdate, accountType, deposit);
                else if (accountType == AccountTypes.Kreditní) newAccount = new CreditAccount(accountNumber, name, surname, birthdate, accountType, deposit);
                else if (accountType == AccountTypes.Studentský) newAccount = new StudentAccount(accountNumber, name, surname, birthdate, accountType, deposit);

                //uložení údajů do vnitřního systému i do WPF
                dtbAcounts1.Add(newAccount.AccountNumber, newAccount);

                string newLog = $"Číslo účtu: \"{newAccount.AccountNumber,-40}\" - Jméno a příjmení: {newAccount.Name} {newAccount.Surname}";
                lbAccountsList.Items.Add(newLog);

                //vymazání zadaných hodnot
                tbAccountNumber.Text = "";
                tbName.Text = "";
                tbSurname.Text = "";
                dtpBirthdate.SelectedDate = null;
                cbAccountType.SelectedIndex = -1;
                tbDeposit.Text = "";

                //dtgAccountsUpdate();
                lbAccountsUpdate();
            }
            else
            {
                //pokud dojde k vyplnění špatných hodnot, vyvolán messageBox s chybami
                string err = $@"Přehled chybně zadaných údajů:

- Číslo účtu: {(correctInputAccountNumber == false ? "Chyba" : "Správně"),5}
- Jméno a příjmení: {(correctInputNameSurname == false ? "Chyba" : "Správně"),5}
- Počáteční vklad: {(correctInputDeposite == false ? "Chyba" : "Správně"),5}
";
                MessageBox.Show(err, "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
            }

        }

        private void lbAccountsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //získání čísla účtu podle výběru uživatele
                string selectedItem = lbAccountsList.SelectedItem.ToString();
                string accNumberStr = selectedItem.Split('"')[1].Trim();
                long accountNumber = Convert.ToInt64(accNumberStr);

                //nalezení daného záznamu v "databázi" a uložení do prměnné
                dtbAcounts1.TryGetValue(accountNumber, out account);

                //výpis údajů do textBoxů
                tbAccountNumber.Text = account.AccountNumber.ToString();
                tbName.Text = account.Name;
                tbSurname.Text = account.Surname;
                dtpBirthdate.SelectedDate = account.Birthdate;
                cbAccountType.SelectedIndex = (int)account.AccountType;
            }
            catch (Exception)
            {

            }

        }

        private void butDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lbAccountsList.SelectedIndex != -1)
            {
                //dotaz na smazání účtu
                MessageBoxResult msbr = MessageBox.Show($"Opravdu si přejete smazat účet '{account.AccountNumber}' majitele '{account.Name} {account.Surname}'?",
                    "Ověření smazání",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    MessageBoxResult.No);

                //pokud je smazání potvrzeno, vymaže záznam z "databáze" i listboxu
                if (msbr == MessageBoxResult.Yes)
                {
                    dtbAcounts1.Remove(account.AccountNumber);
                    lbAccountsList.Items.RemoveAt(lbAccountsList.SelectedIndex);
                    MessageBox.Show($"Záznam smazán. Aktuální počet účtů je: {dtbAcounts1.Count}",
                                    "Informace",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                    tbAccountNumber.Clear();
                    tbName.Clear();
                    tbSurname.Clear();
                    dtpBirthdate.SelectedDate = null;
                    cbAccountType.SelectedIndex = -1;
                    tbDeposit.Clear();
                    //dtgAccountsUpdate();
                    lbAccountsUpdate();
                }
            }
        }

        private void butUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (lbAccountsList.SelectedIndex != -1)
            {
                //dotaz na editaci účtu
                MessageBoxResult msbr = MessageBox.Show($"Opravdu si přejete upravit účet '{account.AccountNumber}' majitele '{account.Name} {account.Surname}'?",
                    "Ověření úpravy",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    MessageBoxResult.No);

                //pokud je úprava potvrzena, vymaže záznam z "databáze" a nahradí novým
                if (msbr == MessageBoxResult.Yes)
                {

                    string name = tbName.Text;
                    string surname = tbSurname.Text;
                    DateTime birthdate = dtpBirthdate.DisplayDate;
                    if (name != "" && surname != "" && birthdate != null)
                    {
                        account.Name = name;
                        account.Surname = surname;
                        account.Birthdate = birthdate;

                        dtbAcounts1.Remove(account.AccountNumber);
                        lbAccountsList.Items.Remove(lbAccountsList.SelectedItem);
                        dtbAcounts1.Add(account.AccountNumber, account);
                        string newLog = $"Číslo účtu: \"{account.AccountNumber,-40}\" - Jméno a příjmení: {account.Name} {account.Surname}";
                        lbAccountsList.Items.Add(newLog);
                        //dtgAccountsUpdate();
                        lbAccountsUpdate();
                    }
                }
            }
        }

        /* Obslužné metody bro datagrid Accounts
        private void dtgAccountsUpdate()
        {
            //vyčistí datagrid
            dtgAccounts.Items.Clear();

            //naplnění datagridu hodnotami z "databáze"
            foreach (var item in dtbAcounts1)
            {
                dtgAccounts.Items.Add(item);
            }
        }

        private void LoadDtgAccounts()
        {
            //vytvoření rozhraní datagridu účtů
            DataGridTextColumn col1 = new DataGridTextColumn();
            DataGridTextColumn col2 = new DataGridTextColumn();
            DataGridTextColumn col3 = new DataGridTextColumn();
            DataGridTextColumn col4 = new DataGridTextColumn();
            DataGridTextColumn col5 = new DataGridTextColumn();
            DataGridTextColumn col6 = new DataGridTextColumn();
            dtgAccounts.Columns.Add(col1);
            dtgAccounts.Columns.Add(col2);
            dtgAccounts.Columns.Add(col3);
            dtgAccounts.Columns.Add(col4);
            dtgAccounts.Columns.Add(col5);
            dtgAccounts.Columns.Add(col6);
            col1.Binding = new Binding("accountNumber");
            col2.Binding = new Binding("name");
            col3.Binding = new Binding("surname");
            col4.Binding = new Binding("birthdate");
            col5.Binding = new Binding("accountType");
            col6.Binding = new Binding("money");
            col1.Header = "Číslo účtu";
            col2.Header = "Jméno";
            col3.Header = "Příjmení";
            col4.Header = "Datum narození";
            col5.Header = "Typ účtu";
            col6.Header = "Zůstatek";

            dtgAccountsUpdate();
        }
        */

        private void lbAccountsUpdate()
        {
            //smaže listbox
            lbAccounts.Items.Clear();

            //nahraje všechny záznamy v "databázi" do listboxu
            foreach (var item in dtbAcounts1)
            {
                lbAccounts.Items.Add(item.Value);
            }
        }

        private void LoadSettings()
        {
            //načtení údajů z vlastností do textboxů
            tbSettingsDebitInterest.Text = Settings.DebetInterest.ToString();
            tbSettingsCreditInterest.Text = Settings.CreditInterest.ToString();
            tbSettingsDebitLimitWitherdraw.Text = Settings.DebetLimitWitherdraw.ToString();
            tbSettingsCreditLimit.Text = Settings.CreditLimit.ToString();
            tbSettingsStudentLimitWitherdraw.Text = Settings.StudentLimitWitherdraw.ToString();
            tbSettingsTimeMultiple.Text = Settings.TimeMultiple.ToString();
        }

        private void butSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            //uložení hodnot z textboxů do vnitřních promněných

            //promněná kontrolující korektnost vstupů
            bool cI1, cI2, cI3, cI4, cI5, cI6;
            cI1 = cI2 = cI3 = cI4 = cI5 = cI6 = true;

            //promněné pro uložení převedených hodnot
            int debetInterest;
            int creditInterest;
            int debetLimitWitherdraw;
            int creditLimit;
            int studentLimitWitherdraw;
            int timeMultiple;

            //kontrola a konverze zadaných hodnot
            cI1 = int.TryParse(tbSettingsDebitInterest.Text, out debetInterest);
            cI2 = int.TryParse(tbSettingsCreditInterest.Text, out creditInterest);
            cI3 = int.TryParse(tbSettingsDebitLimitWitherdraw.Text, out debetLimitWitherdraw);
            cI4 = int.TryParse(tbSettingsCreditLimit.Text, out creditLimit);
            cI5 = int.TryParse(tbSettingsStudentLimitWitherdraw.Text, out studentLimitWitherdraw);
            cI6 = int.TryParse(tbSettingsTimeMultiple.Text, out timeMultiple);

            //nahrání nových hodnot do promněné nastavení
            if(cI1 && cI2 && cI3 && cI4 && cI5 && cI6)
            {
                Settings.DebetInterest = debetInterest;
                Settings.CreditInterest = creditInterest;
                Settings.DebetLimitWitherdraw = debetLimitWitherdraw;
                Settings.CreditLimit = creditLimit;
                Settings.StudentLimitWitherdraw = studentLimitWitherdraw;
                Settings.TimeMultiple = timeMultiple;

                MessageBox.Show("Hodnoty uloženy.");
            }
        }

        private void butDeposit_Click(object sender, RoutedEventArgs e)
        {
            Account selectedAccount;

            //načtení hodnot ze vstupů
            string _accNumber = tbDepositAccountNumber.Text;
            string _amount = tbDepositAmount.Text;

            //promněné s údajemi o vkladu
            string message = tbDepositMessage.Text;
            long accNumber;
            int amount;

            //kontrola a konverze zadaných hodnot
            if (!long.TryParse(_accNumber, out accNumber)) MessageBox.Show("Bylo zadáno číslo účtu v nesprávném formátu!");
            if (!int.TryParse(_amount, out amount)) MessageBox.Show("Vklad byl zadán v nesprávném formátu!");

            //kontrola existence účtu
            bool accExist = false;
            foreach (var item in dtbAcounts1)
            {
                if (item.Key == accNumber) accExist = true;
            }
            if (!accExist)
            {
                MessageBox.Show("Tento účet neexistuje!");
                return;
            }

            //nalezení účtu a provedení operace
            foreach (var item in dtbAcounts1)
            {
                if (item.Key == accNumber)
                {
                    selectedAccount = item.Value;
                    bool inserted = selectedAccount.Deposit(amount, message); 
                    //selectedAccount.ShowLog();
                    if (inserted)
                    {
                        MessageBox.Show($"Částka '{amount}' byla vložena na účet '{accNumber}'."); 
                        selectedAccount = null;
                        lbAccountsUpdate();
                        tbDepositAccountNumber.Clear();
                        tbDepositAmount.Clear();
                        tbDepositMessage.Clear();
                        return;
                    }
                    else
                    {
                        MessageBox.Show($"Částka '{amount}' nebyla vložena na účet '{accNumber}'.");
                        selectedAccount = null;
                        return;
                    }
                }
            }
        }

        private void butWithdrawa_Click(object sender, RoutedEventArgs e)
        {
            Account selectedAccount;

            //načtení hodnot ze vstupů
            string _accNumber = tbWithdrawAccountNumber.Text;
            string _amount = tbWithdrawAmount.Text;

            //promněné s údajemi o vkladu
            string message = tbWithdrawMessage.Text;
            long accNumber;
            int amount;

            //kontrola a konverze zadaných hodnot
            if (!long.TryParse(_accNumber, out accNumber)) MessageBox.Show("Bylo zadáno číslo účtu v nesprávném formátu!");
            if (!int.TryParse(_amount, out amount)) MessageBox.Show("Výběr byl zadán v nesprávném formátu!");

            //kontrola existence účtu
            bool accExist = false;
            foreach (var item in dtbAcounts1)
            {
                if (item.Key == accNumber) accExist = true;
            }
            if (!accExist)
            {
                MessageBox.Show("Tento účet neexistuje!");
                return;
            }

            //nalezení účtu a provedení operace
            foreach (var item in dtbAcounts1)
            {
                if (item.Key == accNumber)
                {
                    selectedAccount = item.Value;
                    bool witherdrawed = selectedAccount.Witherdraw(amount, message);
                    if (witherdrawed)
                    {
                        MessageBox.Show($"Částka '{amount}' byla vybrána z účtu '{accNumber}'.");
                        selectedAccount = null;
                        lbAccountsUpdate();
                        tbWithdrawAccountNumber.Clear();
                        tbWithdrawAmount.Clear();
                        tbWithdrawMessage.Clear();
                        return;
                    }
                    else
                    {
                        MessageBox.Show($"Částka '{amount}' nebyla vybrána z účtu '{accNumber}'.");
                        selectedAccount = null;
                        return;
                    }
                }
            }
        }

        private void butSimulation_Click(object sender, RoutedEventArgs e)
        {
            timer.Interval = TimeSpan.FromSeconds(1 * (1/Settings.TimeMultiple));

            if (!simulationOn) simulationOn = true;
            else simulationOn = false;

            if (simulationOn)
            {
                timer.Start();
                butSimulation.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
            else
            {
                timer.Stop();
                butSimulation.Background = new SolidColorBrush(Color.FromRgb(40, 185, 25));
            }            
        }
    }
}