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
       
        public CustomerInfo(object DataContext) 
        {
            this.DataContext = DataContext;
            InitializeComponent();
            department = this.DataContext as DivisionVM;
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
                try
                {
                    department.Put(((AccountVM)AccountsList.SelectedItem).Bic, Convert.ToDecimal(PutSum.Text));
                }
                catch (Exception ex) // ошибка при автосохранении
                {
                    MessageBox.Show(ex.Message);
                }
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

                try
                {
                    department.Withdraw(((AccountVM)AccountsList.SelectedItem).Bic, Convert.ToDecimal(WithdrawSum.Text));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

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
                try
                {
                    department.CloseAccount(((AccountVM)AccountsList.SelectedItem).Bic);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
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
                try
                {
                    department.Transfer(((AccountVM)AccountsList.SelectedItem).Bic, TransferAccount.Text, Convert.ToDecimal(TransferSum.Text));
                }
                catch (Exception ex)  
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
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
                MyWindow w = new MyWindow("Ошибка ввода",800);
                w.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen; //а нужно ли выводить окно по поводу 3-го символа после запятой?
                w.ShowDialog();

            }
            string s = Regex.Replace(((TextBox)sender).Text, @"[^\d,]", ""); // вот этот блок всегда выполняется, джаже если нет ошибок
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
    }
}
