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
using BankSystem.ViewModel;

namespace BankSystem.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BankVM bank;
        public BankVM bankVM;
        public MainWindow()
        {
            InitializeComponent();
            bank = (BankVM)this.FindResource("bank");
            DataContext = bank;

        }

        private void Customers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CustomerInfo newWindow = new CustomerInfo();
            newWindow.Show();
            newWindow.DataContext = bank.SelectedItem;
            
        }

        
    }
}
