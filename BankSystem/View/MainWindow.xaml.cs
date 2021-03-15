using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using BankSystemLib;

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
            try
            {
                repository = new Repository();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Работа программы невозможна. При чтении данных из файла возникло исключение: {ex.Message}"); //нужно что-то с этим сделать
                Application.Current.Shutdown();
            }

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
                MessageBox.Show(ex.Message);
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
            try
            {
                //Task task = new Task(bank.MonthlyCharge);
                //task.Start();

                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = true;
                worker.DoWork += worker_DoWork;

                void worker_DoWork(object o, DoWorkEventArgs e1)
                {
                    bank.MonthlyCharge(worker);
                    //task.Start();
                }
                ProgressBar(worker);

                // MessageBox.Show("Закрытие месяца завершено");
                // bank.MonthlyCharge();
            }
            catch (Exception ex) //при автосохранении возможно исключение
            {
                MessageBox.Show(ex.Message);
            }
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
                    bank.LoyaltyProg();
        }

        

        private void InitialFilling_Button_Click(object sender, RoutedEventArgs e)
        {

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            
            void worker_DoWork(object o, DoWorkEventArgs e1)
                {
                    InitialFilling.InitialFillingExtension(repository.bank.Departments[0] as Department<Entity>, 100, worker);
                    InitialFilling.InitialFillingExtension(repository.bank.Departments[1] as Department<Person>, 100, worker);
                    InitialFilling.InitialFillingExtension(repository.bank.Departments[2] as Department<Vip>, 100, worker);
                    //System.Threading.Thread.Sleep(100);
                }
            ProgressBar(worker);

        }

        private void ProgressBar(BackgroundWorker progressBarWorker)
        {
            progressBarWorker.ProgressChanged += worker_ProgressChanged;
            progressBarWorker.RunWorkerCompleted += worker_RunWorkerCompleted;
            progressBarWorker.RunWorkerAsync(10000);

            void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
            {
                pbCalculationProgress.Value = e.ProgressPercentage;
            }

            void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
            {
                Divisions.SelectedIndex = 0; // лучше,если будет выбран отдел - сразу видно, что что-то произошло
                MessageBox.Show("Процесс закончен");
                pbCalculationProgress.Value = 0;
            }

        }
    }
}
