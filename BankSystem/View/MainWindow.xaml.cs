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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using BankSystem.Model;
using BankSystem.ViewModel;

namespace BankSystem.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Repository repository;
        BankVM bank;
        //public BankVM bankVM;
        public MainWindow()
        {
            InitializeComponent();
            //bank = (BankVM)this.FindResource("bank");

            repository = new Repository();
            bank = new BankVM(repository.bank);
            DataContext = bank;
           
        }

        private async void Customers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Customers.SelectedItem != null)
            {
                
                CustomerInfo newWindow = new CustomerInfo(bank.SelectedItem);
                //newWindow.DataContext = bank.SelectedItem;
                await Task.Delay(100);
                newWindow.ShowDialog();
                bank.ClearSelectedCustomer();
            }
            return;
        }

        private void Serialize_Button_Click(object sender, RoutedEventArgs e)
        {
            repository.SerializeJsonBank();
        }

        private void Deserialize_Button_Click(object sender, RoutedEventArgs e)
        {
            repository.DeserializeJsonBank();
            bank = new BankVM(repository.bank);
            DataContext = bank;
            //var binding = DataContextProperty;
            //mainWindow1.GetBindingExpression(binding).UpdateTarget();
        }

        private void New_Client_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Divisions.SelectedItem!=null 
                && !String.IsNullOrEmpty(NewName.Text)
                && !String.IsNullOrEmpty(NewOtherName.Text)
                && !String.IsNullOrEmpty(NewLegalId.Text)
                && !String.IsNullOrEmpty(NewPhone.Text))
            {
                ((DivisionVM)Divisions.SelectedItem).CreateCustomer(NewName.Text, NewOtherName.Text, NewLegalId.Text, NewPhone.Text);
            }
            else
            {
                MessageBox.Show("Выберите отдел и заполните все данные клиента");
                return;
            } 

        }

        private void MonthlyCharge_Button_Click(object sender, RoutedEventArgs e)
        {
            bank.MonthlyCharge();
        }

        private void Transactions_Click(object sender, RoutedEventArgs e)
        {
            Transactions tWindow = new Transactions(repository.bank);
            //DataContext = repository.bank; // - зачем? слетал датаконтекст главного окна!
           // await Task.Delay(100);
            tWindow.ShowDialog();
        }
    }
}
