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
using BankSystem.Model;

namespace BankSystem.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Bank bank ;
        public MainWindow()
        {
            InitializeComponent();
            try
            {
               bank = new Bank("BANK");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " Аварийное завершение.");
                Environment.Exit(911);
            }

            bank.ExampleCustomers();

            bank.MonthlyCharge();

           // Accounts.ItemsSource = Accounts;
        }
    }
}
