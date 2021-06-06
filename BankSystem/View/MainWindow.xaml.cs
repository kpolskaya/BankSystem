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

        bool NoClose { get { return repository.IsBusy; } }

        bool inputRestricted = false;
        int qClients = 4000;                                    //количество клиентов в департаменте для автогенерации

        public MainWindow()
        {
            repository = new Repository();
            bank = new BankVM(repository.Bank);
            DataContext = bank;
            InitializeComponent();
        }
 
        /// <summary>
        /// Открывает окно информации о выбранном клиенте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomerLeftBtnClick(object sender, RoutedEventArgs e)
        {
            if (!inputRestricted)
            {
                CustomerInfo newWindow = new CustomerInfo(DataContext)    //datacontext нужен для вывода транзакций
                {
                    Owner = this
                };
                newWindow.ShowDialog();
            }
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
                MessageBox.Show("Ошибка сохранения базы: "
                    + ex.Message, "Банк - исключения", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("Ошибка загрузки базы: "
                    + ex.Message, "Банк - исключения", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                pbCalculationProgress.IsIndeterminate = false;
                pbCalculationProgress.Value = 0;
                SetInputRestrictions(false);
            }
            bank = new BankVM(repository.Bank);
            DataContext = bank;
            Divisions.SelectedItem = bank.Departments[0];
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
                    MessageBox.Show("Возникла ошибка: "
                        + ex.Message, "Банк - исключения", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Выберите отдел и заполните все данные клиента");
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
            SetInputRestrictions(true);
            pbCalculationProgress.IsIndeterminate = true;
            Task[] tasks = new Task[bank.Departments.Count];
            for (int i = 0; i < bank.Departments.Count; i++)
            {
                int n = i;
                tasks[n] = Task.Run(() => bank.Departments[n].LoyalityProgram());
            }

            Task.Factory.ContinueWhenAll(tasks, completedTasks => this.Dispatcher.Invoke(() =>
            {
                pbCalculationProgress.IsIndeterminate = false;
                SetInputRestrictions(false);
                pbCalculationProgress.Value = 0;
                MessageBox.Show("Программа лояльности закончена. Призы вручены.");
            }));
        }

        /// <summary>
        /// Автоматическое заполнение клиентами
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitialFilling_Button_Click(object sender, RoutedEventArgs e) 
        {
            foreach (var item in bank.Departments)
            {
                if (item.Customers.Count > 0)
                {
                    MessageBox.Show("База не пустая. Автоматическое заполнение невозможно");
                    return;
                }
            }

            PBtext.Text = "Идет первоначальное заполнение базы. Этот процесс может занять несколько минут.";
            bank.SafeMode = true;
            SetInputRestrictions(true);
            pbCalculationProgress.IsIndeterminate = true;
            Task[] tasks = new Task[bank.Departments.Count];

            for (int i = 0; i < bank.Departments.Count; i++)
            {
                int n = i;
                tasks[n] = Task.Run(() => bank.Departments[n].InitialFilling(qClients));
            }

            Task.Factory.ContinueWhenAll(tasks, completedTasks => this.Dispatcher.Invoke(() =>
            {
                pbCalculationProgress.IsIndeterminate = false;
                SetInputRestrictions(false);
                pbCalculationProgress.Value = 0;
                bank.SafeMode = false;
                PBtext.Text = "";
                Divisions.SelectedItem = bank.Departments[0];
                MessageBox.Show("Начальное заполнение закончено");
            }));
        }

        /// <summary>
        /// Генерирует миллион транзакций по счетам клиентов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SimOperation_Click(object sender, RoutedEventArgs e)
        {
            int count = 1000000 / (qClients * 9);            // количество закрытых месяцев = 1 000 000 /(клиентов * 3 счета * 3 департамента)
            SetInputRestrictions(true);
            pbCalculationProgress.IsIndeterminate = true;

            await Task.Run(() =>
            {
                for (int i = 0; i < count; i++)
                    bank.MonthlyCharge();
            }
            );

            pbCalculationProgress.IsIndeterminate = false;
            SetInputRestrictions(false);
            pbCalculationProgress.Value = 0;
            MessageBox.Show($"Операции сгенерированы. Синхронизируйте журнал транзакций.");
        }

        /// <summary>
        /// Запрещающие флаги на время критических операций
        /// </summary>
        /// <param name="flag"></param>
        private void SetInputRestrictions(bool flag)
        {
            MonthlyCharge.IsEnabled = !flag;
            LoyalityProgram.IsEnabled = !flag;
            NewClient.IsEnabled = !flag;
            OpenFile.IsEnabled = !flag;
            SaveFile.IsEnabled = !flag;
            SyncHistory.IsEnabled = !flag;
            InitialFilling.IsEnabled = !flag;
            SimOperation.IsEnabled = !flag;
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
            PBtext.Text = "Вы можете продолжать работу, однако некоторые транзакции могут не войти в эту файловую копию";
            int count = bank.TransactionHistory.Count;
            SyncHistory.IsEnabled = false;
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            try
            {
                var t = repository.UniteTransactionsAsync();
                await t;
                PBtext.Text = "";
                stopWatch.Stop();
                MessageBox.Show(
                    $"Журнал транзакций синхронизирован.\n" +
                    $"Было {count} записей, стало {bank.TransactionHistory.Count} записей.\n" +
                    $"Понадобилось {stopWatch.Elapsed.Minutes} минут {stopWatch.Elapsed.Seconds} секунд.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при синхронизации журнала операций: "
                + ex.Message, "Банк - исключения", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                pbCalculationProgress.IsIndeterminate = false;
                pbCalculationProgress.Value = 0;
                SyncHistory.IsEnabled = true; 
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            repository = new Repository();
            bank = new BankVM(repository.Bank);
            DataContext = bank;
            Divisions.SelectedItem = bank.Departments[0];
            MessageBox.Show("База очищена");
        }
    }
}
