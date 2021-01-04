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
            department = (DivisionVM)customerInfo.DataContext;
        }

        private void Button_Click_OpenAccount(object sender, RoutedEventArgs e)
        {
            department.OpenAccount((AccountType)TypeAccountOpen.SelectedValue, department.SelectedCustomer.Id);

        }
    }
}
