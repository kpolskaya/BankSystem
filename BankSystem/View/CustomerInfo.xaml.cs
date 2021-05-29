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
using BankSystemLib;
using System.Text.RegularExpressions;

namespace BankSystem.View
{
    /// <summary>
    /// Логика взаимодействия для CustomerInfo.xaml
    /// </summary>
    public partial class CustomerInfo : Window
    {
        DivisionVM department;
        BankVM bank;
       
        public CustomerInfo(object MainDataContext) 
        {
            this.bank = (MainDataContext as BankVM);
            this.DataContext = bank.SelectedItem;
            department = this.DataContext as DivisionVM;
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик кнопки открытия счета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Обработчик кнопки внесения средств
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Обработчик кнопки снятия средств
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Обработчик кнопки закрытия счета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Обработчик кнопки перевода средств
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Transfer(object sender, RoutedEventArgs e)
        {
            if (AccountsList.SelectedItem != null)
            {
                department.Transfer(((AccountVM)AccountsList.SelectedItem).Bic, TransferAccount.Text, Convert.ToDecimal(TransferSum.Text));
            }
            else
            {
                MessageBox.Show("Выберите счет");
                return;
            }
        }

        /// <summary>
        /// Проверка поля на корректное значение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Regex.IsMatch(((TextBox)sender).Text, @"[^\d,]") || Regex.IsMatch(((TextBox)sender).Text, @"(\d*,\d{2})\d+") || Regex.IsMatch(((TextBox)sender).Text, @"(\d*[,]\d*)[,].*")) 
            {
                MyWindow w = new MyWindow("Ошибка ввода", 800)
                {
                    WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen
                };
                w.ShowDialog();                                                             //а нужно ли выводить окно по поводу 3-го символа после запятой?

            }
            string s = Regex.Replace(((TextBox)sender).Text, @"[^\d,]", "");                // вот этот блок всегда выполняется, джаже если нет ошибок
            s = Regex.Replace((s), @"(\d*,\d{2})\d+", @"$1");
            s = Regex.Replace((s), @"(\d*[,]\d*)[,].*", @"$1");

            ((TextBox)sender).Text = s;

            ((TextBox)sender).SelectionStart = ((TextBox)sender).Text.Length;
        }

        /// <summary>
        /// Копирование содержимого поля в буфер обмена
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                catch (Exception)
                {
                    MessageBox.Show("Ошибка буфера обмена");
                    return;
                }
            }
        }

        /// <summary>
        /// Вывод виписки по счету
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Statement_Click(object sender, RoutedEventArgs e)
        {
            if (AccountsList.SelectedItem != null)
            {
                Transactions tWindow = new Transactions(bank, AccountsList.SelectedItem as AccountVM);
                tWindow.ShowDialog();
            }
            else
                MessageBox.Show("Выберите счет");

        }
    }
}
