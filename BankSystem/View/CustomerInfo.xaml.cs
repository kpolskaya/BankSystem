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
using BankSystem.ViewModel;
using BankSystem.Model;

namespace BankSystem.View
{
    /// <summary>
    /// Логика взаимодействия для CustomerInfo.xaml
    /// </summary>
    public partial class CustomerInfo : Window
    {
        DivisionVM department;
       
        public CustomerInfo()
        {
            InitializeComponent();
            
        }

        private void Button_Click_OpenAccount(object sender, RoutedEventArgs e)
        {
            department = (DivisionVM)customerInfo.DataContext;
            department.OpenAccount((AccountType)TypeAccountOpen.SelectedValue, department.SelectedCustomer);
                        
        }

        private void Button_Click_Put(object sender, RoutedEventArgs e)
        {
            department = (DivisionVM)customerInfo.DataContext;
            department.Put(((AccountVM)Accounts.SelectedItem).Bic, Convert.ToDecimal(PutSum.Text));
        }

        private void Button_Click_Withdraw(object sender, RoutedEventArgs e)
        {
            department = (DivisionVM)customerInfo.DataContext;
            department.Withdraw(((AccountVM)Accounts.SelectedItem).Bic, Convert.ToDecimal(WithdrawSum.Text));
        }

        private void Button_Click_CloseAccount(object sender, RoutedEventArgs e)
        {
            department = (DivisionVM)customerInfo.DataContext;
            department.CloseAccount(((AccountVM)Accounts.SelectedItem).Bic);
        }

        private void Button_Click_Transfer(object sender, RoutedEventArgs e)
        {
            department = (DivisionVM)customerInfo.DataContext;
            try
            {
                department.Transfer(((AccountVM)Accounts.SelectedItem).Bic, TransferAccount.Text, Convert.ToDecimal(TransferSum.Text));
            }
            catch (Exception)
            {
                MessageBox.Show("Несуществующий счет");
                return;
            }
            
        }
    }
}
