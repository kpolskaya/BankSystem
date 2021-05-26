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
using BankSystem.View;
using BankSystemLib;
using System.IO;
using System.Diagnostics;



namespace BankSystem.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Repository repository;
        BankVM bank;

        bool NoClose 
        { 
            get 
            {
                return (repository.IsBusy); 
            } 
        }

        bool inputRestricted = false;
        int qEntity = 1000;
        int qPerson = 1000;
        int qVip = 1000;

        public MainWindow()
        {
            InitializeComponent();
            repository = new Repository();
 
            bank = new BankVM(repository.Bank);
            DataContext = bank;
        }
 
        /// <summary>
        /// Открывает окно информации о выбранном клиенте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomerLeftBtnClick(object sender, RoutedEventArgs e)
        {
            if (Customers.SelectedItem != null && !inputRestricted)
            {
                CustomerInfo newWindow = new CustomerInfo(DataContext)    //(bank.SelectedItem) //datacontext нужен для вывода транзакций
                {
                    Owner = this
                };
                newWindow.ShowDialog();
            }
            return;
        }

        /// <summary>
        /// Сохранение данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Serialize_Button_Click(object sender, RoutedEventArgs e)
        {
            pbCalculationProgress.IsIndeterminate = true;
            SetInputRestrictions(true);
            try
            {
                await repository.SaveBankAsync();
                MessageBox.Show("Файл сохранен");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                pbCalculationProgress.IsIndeterminate = false;
                pbCalculationProgress.Value = 0;
                SetInputRestrictions(false);
            }
        }

        /// <summary>
        /// Чтение данных из файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Deserialize_Button_Click(object sender, RoutedEventArgs e)
        {
            pbCalculationProgress.IsIndeterminate = true;
            SetInputRestrictions(true);
            try
            {
                await repository.LoadBankAsync();
             }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                pbCalculationProgress.IsIndeterminate = false;
                pbCalculationProgress.Value = 0;
                SetInputRestrictions(false);
            }
            bank = new BankVM(repository.Bank);
            DataContext = bank;
            MessageBox.Show("Файл прочитан");
        }


        private void New_Client_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Divisions.SelectedItem != null
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

        private async void MonthlyCharge_Button_Click(object sender, RoutedEventArgs e)
        {
            SetInputRestrictions(true);
            pbCalculationProgress.IsIndeterminate = true;
            await Task.Run(() => bank.MonthlyCharge());
            pbCalculationProgress.IsIndeterminate = false;
            pbCalculationProgress.Value = 0;
            SetInputRestrictions(false);
            MessageBox.Show("Закрытие месяца выполнено");
        }

        /// <summary>
        /// Открытие окна истории транзакций
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Transactions_Click(object sender, RoutedEventArgs e) //Перенести в окно клиента
        {
            //Transactions tWindow = new Transactions(DataContext);
            //tWindow.ShowDialog();
        }

        /// <summary>
        /// Проверка поля на корректное значение - только цифры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) //Где наш замечательный метод????
        {
            {
                string s = Regex.Replace(((TextBox)sender).Text, @"[^\d]", "");
                ((TextBox)sender).Text = s;

                ((TextBox)sender).SelectionStart = ((TextBox)sender).Text.Length;

            }
        }

        private void LoyalityProgram_Button_Click(object sender, RoutedEventArgs e) // долго работает. 
        {
            SetInputRestrictions(true);
            pbCalculationProgress.IsIndeterminate = true;
            Task[] tasks = new Task[3];
            tasks[0] = Task.Run(() => (repository.Bank.Departments[0] as Department<Entity>).LoyalityProgramExtension());
            tasks[1] = Task.Run(() => (repository.Bank.Departments[1] as Department<Person>).LoyalityProgramExtension());
            tasks[2] = Task.Run(() => (repository.Bank.Departments[2] as Department<Vip>).LoyalityProgramExtension());
            Task.Factory.ContinueWhenAll(tasks, completedTasks => this.Dispatcher.Invoke(() =>
            {
                pbCalculationProgress.IsIndeterminate = false;
                SetInputRestrictions(false);
                pbCalculationProgress.Value = 0;
                MessageBox.Show("Программа лояльности закончена. Призы вручены.");
            }));
        }


        private void InitialFilling_Button_Click(object sender, RoutedEventArgs e) //Запретим заполнение если база не пустая? (или решать с задвоением айди как-то)
        {
            SetInputRestrictions(true);
            pbCalculationProgress.IsIndeterminate = true;
            Task[] tasks = new Task[3];
            tasks[0] = Task.Run(() => InitialFillingExtention.FillWithRandom(repository.Bank.Departments[0] as Department<Entity>, qEntity));
            tasks[1] = Task.Run(() => InitialFillingExtention.FillWithRandom(repository.Bank.Departments[1] as Department<Person>, qPerson));
            tasks[2] = Task.Run(() => InitialFillingExtention.FillWithRandom(repository.Bank.Departments[2] as Department<Vip>, qVip));
          
            Task.Factory.ContinueWhenAll(tasks, completedTasks => this.Dispatcher.Invoke(() =>
            {
                pbCalculationProgress.IsIndeterminate = false;
                SetInputRestrictions(false);
                pbCalculationProgress.Value = 0;
                MessageBox.Show("Начальное заполнение закончено");
            }));
        }

        private void SetInputRestrictions(bool flag)
        {
            MonthlyCharge.IsEnabled = !flag;
            LoyalityProgram.IsEnabled = !flag;
            NewClient.IsEnabled = !flag;
            OpenFile.IsEnabled = !flag;
            SaveFile.IsEnabled = !flag;
            SyncHistory.IsEnabled = !flag;
            InitialFilling.IsEnabled = !flag;
            inputRestricted = flag;
        }

        private void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            if (NoClose)
            {
                SetInputRestrictions(true);
                MessageBoxResult result =
                MessageBox.Show(
                       "Завершение программы. Выйти без сохранения?",
                       "Завершение программы",
                       MessageBoxButton.YesNo,
                       MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    Task.Run(OnWindowClosing);
                    return;
                }
            }
            e.Cancel = false;
        }

        private void OnWindowClosing()
        {
            this.Dispatcher.Invoke(() =>
            {
                PBtext.Text = "Программа закрывается. Подождите. ";
                MyWindow w = new MyWindow("Идет сохранение данных", 1000)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                w.ShowDialog();
            });
         
            while (NoClose) { Task.Delay(100); }
            
            this.Dispatcher.Invoke(() =>
            {
                PBtext.Text = "";
                Application.Current.Shutdown();
            });
        }

        /// <summary>
        ///  Синхронизирует журнал транзакций в памяти с файлом истории
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SyncHistory_Click(object sender, RoutedEventArgs e)
        {
            pbCalculationProgress.IsIndeterminate = true;
            int count = bank.TransactionHistory.Count;
            SyncHistory.IsEnabled = false;
            var ts = new Stopwatch();
            ts.Start();
            try
            {
                var t = repository.UniteTransactionsAsync();
                await t;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); 
            }
            finally
            {
                pbCalculationProgress.IsIndeterminate = false;
                pbCalculationProgress.Value = 0;
                SyncHistory.IsEnabled = true;
            }

            ts.Stop();
            MessageBox.Show(
                $"Журнал транзакций синхронизирован.\n" +
                $"Было {count} записей, стало {bank.TransactionHistory.Count} записей.\n" +
                $"Понадобилось {ts.Elapsed.Minutes} минут {ts.Elapsed.Seconds} секунд.");
        }
    }
}
