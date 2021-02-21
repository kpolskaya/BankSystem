using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public MainWindow()
        {
            InitializeComponent();
            repository = new Repository();
            bank = new BankVM(repository.bank);
            DataContext = bank;
        }

        /// <summary>
        /// Открывает окно информации о выбранном клиенте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Customers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Customers.SelectedItem != null)
            {
                CustomerInfo newWindow = new CustomerInfo(bank.SelectedItem);
                await Task.Delay(100);
                newWindow.ShowDialog();
                bank.ClearSelectedCustomer();
            }
            return;
        }

        /// <summary>
        /// Сохранение данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Serialize_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                repository.SerializeJsonBank();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка записи файла данных. " + ex.Message);
            }
        }

        /// <summary>
        /// Чтение данных из файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Deserialize_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                repository.DeserializeJsonBank();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка чтения файла данных. " + ex.Message);
                return;
            }
            
            bank = new BankVM(repository.bank);
            DataContext = bank;
        }

        private void New_Client_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Divisions.SelectedItem!=null 
                && !String.IsNullOrEmpty(NewName.Text)
                && !String.IsNullOrEmpty(NewOtherName.Text)
                && !String.IsNullOrEmpty(NewLegalId.Text)
                && !String.IsNullOrEmpty(NewPhone.Text))
            {
                try
                {
                    ((DivisionVM)Divisions.SelectedItem).CreateCustomer(NewName.Text, NewOtherName.Text, NewLegalId.Text, NewPhone.Text);
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
                
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

        /// <summary>
        /// Открытие окна истории транзакций
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Transactions_Click(object sender, RoutedEventArgs e)
        {
            Transactions tWindow = new Transactions(repository.bank);
            tWindow.ShowDialog();
        }

        /// <summary>
        /// Проверка поля на корректное значение - только цифры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            {
                string s = Regex.Replace(((TextBox)sender).Text, @"[^\d]", "");
                ((TextBox)sender).Text = s;

                ((TextBox)sender).SelectionStart = ((TextBox)sender).Text.Length;
        
            }
        }

        private void LoyalityProgram_Button_Click(object sender, RoutedEventArgs e)
        {
                    bank.LoyalityProg();
        }
    }
}
