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
using System.Text.RegularExpressions;

namespace BankSystem.View
{
    /// <summary>
    /// Логика взаимодействия для CustomerInfo.xaml
    /// </summary>
    public partial class CustomerInfo : Window
    {
        DivisionVM department;
       
        public CustomerInfo(object DataContext) 
        {
            this.DataContext = DataContext;
            InitializeComponent();
            department = this.DataContext as DivisionVM;
        }

        private void Button_Click_OpenAccount(object sender, RoutedEventArgs e)
        {
            if (TypeAccountOpen.SelectedValue != null)
            {
                department.OpenAccount((AccountType)TypeAccountOpen.SelectedValue, department.SelectedCustomer);
            }
            else
            {
                MessageBox.Show("Выберите тип счета");
                return;
            } 
        }

        private void Button_Click_Put(object sender, RoutedEventArgs e)
        {
            if (AccountsList.SelectedItem != null && PutSum.Text != "")
            {
                 department.Put(((AccountVM)AccountsList.SelectedItem).Bic, Convert.ToDecimal(PutSum.Text));
            }
            else
            {
                MessageBox.Show("Выберите счет и введите сумму операции");
                return;
            }
        }

        private void Button_Click_Withdraw(object sender, RoutedEventArgs e)
        {
            if (AccountsList.SelectedItem != null && WithdrawSum.Text != "")
            {
                department.Withdraw(((AccountVM)AccountsList.SelectedItem).Bic, Convert.ToDecimal(WithdrawSum.Text));
                               
            }
            else
            {
                MessageBox.Show("Выберите счет и введите сумму операции");
                return;
            }

        }

        private void Button_Click_CloseAccount(object sender, RoutedEventArgs e)
        {
            if (AccountsList.SelectedItem != null)
            {
                department.CloseAccount(((AccountVM)AccountsList.SelectedItem).Bic);
            }
            else
            {
                MessageBox.Show("Выберите счет");
                return;
            }

        }

        private void Button_Click_Transfer(object sender, RoutedEventArgs e)
        {
            if (AccountsList.SelectedItem != null)
            {
                try
                {
                    department.Transfer(((AccountVM)AccountsList.SelectedItem).Bic, TransferAccount.Text, Convert.ToDecimal(TransferSum.Text));
                }
                catch (Exception)
                {
                    MessageBox.Show("Несуществующий счет");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Выберите счет");
                return;
            }
        }

      
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            {
                string s = Regex.Replace(((TextBox)sender).Text, @"[^\d,]", "");
                ((TextBox)sender).Text = s;
               
                ((TextBox)sender).SelectionStart = ((TextBox)sender).Text.Length;
                try
                {
                   Convert.ToDecimal(string.Format("{0:.##}", s));
                }
                catch (Exception)
                {

                    MessageBox.Show("Недопустимый формат");
                    return;
                }
            }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (((TextBlock)sender).Text == string.Empty)
                return;
            else
            {
                try
                {
                    Clipboard.SetText(((TextBlock)sender).Text);
                }
                catch (Exception )
                {
                    MessageBox.Show("Ошибка");
                    return;
                }
            }
        }
    }
}
