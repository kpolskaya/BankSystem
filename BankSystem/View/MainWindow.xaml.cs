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


namespace BankSystem.View
{
    
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Repository repository;
        BankVM bank;
        public static bool NoClose { get { return (FlagInputRestriction || !Processing.TransactionsQueue.IsEmpty || !SpamBot.MessageQueue.IsEmpty || SpamBot.OnLine || Processing.IsActive); } }
        public static bool FlagInputRestriction = false;
        static int qEntity = 1000;
        static int qPerson = 1000;
        static int qVip = 1000;

        public MainWindow()
        {
            InitializeComponent();
            //try
            //{
            //    repository = new Repository();

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Работа программы невозможна. При чтении данных из файла возникло исключение: {ex.Message}"); //нужно что-то с этим сделать
            //    Application.Current.Shutdown();
            //}

            repository = new Repository();
            //подписки на события, которые генерит репозиторий...

            bank = new BankVM(repository.bank);
            DataContext = bank;
        }

        /// <summary>
        /// Открывает окно информации о выбранном клиенте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Customers_SelectionChanged(object sender, SelectionChangedEventArgs e) // этот метод переписан и его нужно будет удалить!!!
        {
            if (Customers.SelectedItem != null && !FlagInputRestriction)
            {
                CustomerInfo newWindow = new CustomerInfo(bank.SelectedItem)
                {
                    Owner = this
                };
                Thread.Sleep(100);// это зачем-то нужно
                newWindow.ShowDialog();
                bank.ClearSelectedCustomer(); //вот тоже непонятно 
            }
            return;
        }

        /// <summary>
        /// Открывает окно информации о выбранном клиенте
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomerLeftBtnClick(object sender, RoutedEventArgs e)
        {
            if (Customers.SelectedItem != null && !FlagInputRestriction)
            {
                CustomerInfo newWindow = new CustomerInfo(bank.SelectedItem)
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
            try
            {
                pbCalculationProgress.IsIndeterminate = true;
                InputRestriction(false);
                await Task.Run(() => repository.SerializeJsonBank());
                pbCalculationProgress.IsIndeterminate = false;
                pbCalculationProgress.Value = 0;
                MessageBox.Show("Файл сохранен");
                InputRestriction(true);
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
        private async void Deserialize_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pbCalculationProgress.IsIndeterminate = true;
                InputRestriction(false);
                await Task.Run(() => repository.DeserializeJsonBank());
                pbCalculationProgress.IsIndeterminate = false;
                pbCalculationProgress.Value = 0;
                MessageBox.Show("Файл прочитан");
                InputRestriction(true);
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
        {                                                                                   //запрет проводок по счетам?

            pbCalculationProgress.IsIndeterminate = true;

            await Task.Run(() => bank.MonthlyCharge());
            pbCalculationProgress.IsIndeterminate = false;
            pbCalculationProgress.Value = 0;
            MessageBox.Show("Закрытие месяца выполнено");


            //try
            //{
            //    pbCalculationProgress.IsIndeterminate = true;
            //    await DoMonthlyCharge();

            //}
            //catch (Exception ex) //при автосохранении возможно исключение
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }


        //private async Task DoMonthlyCharge()
        //{
        //    pbCalculationProgress.IsIndeterminate = true;

        //    await Task.Run(() => bank.MonthlyCharge()); //Можно же просто Task.Run - тут только один процесс вроде? Нужно сделать закрытие месяца awaitable!
        //    pbCalculationProgress.IsIndeterminate = false;
        //    pbCalculationProgress.Value = 0;
        //    MessageBox.Show("Закрытие месяца выполнено");

        //}

        /// <summary>
        /// Открытие окна истории транзакций
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Transactions_Click(object sender, RoutedEventArgs e) //TODO решить проблему с изменением списка транзакций во время просмотра!
        {
            Transactions tWindow = new Transactions(DataContext);
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

        private void LoyalityProgram_Button_Click(object sender, RoutedEventArgs e) // долго работает. Изменение коллекций в это время повлечёт крах (наверное) блокировка? оптимизация?
        {
            pbCalculationProgress.IsIndeterminate = true;
            // Task.Run(()=>bank.LoyaltyProg());
            Task[] tasks = new Task[3];
            tasks[0] = Task.Run(() => (repository.bank.Departments[0] as Department<Entity>).LoyaltyProgramExtension());
            tasks[1] = Task.Run(() => (repository.bank.Departments[1] as Department<Person>).LoyaltyProgramExtension());
            tasks[2] = Task.Run(() => (repository.bank.Departments[2] as Department<Vip>).LoyaltyProgramExtension());
            Task.Factory.ContinueWhenAll(tasks, completedTasks => this.Dispatcher.Invoke(() =>
            {
                pbCalculationProgress.IsIndeterminate = false;
                InputRestriction(true);
                MessageBox.Show("Программа лояльности закончена. Призы вручены.");
                pbCalculationProgress.Value = 0;
            }));
        }


        //private async void InitialFilling_Button_Click(object sender, RoutedEventArgs e)
        //{
        //    MonthlyCharge.IsEnabled = false;
        //    LoyalityProgram.IsEnabled = false;
        //    await DoFilling();

        //}

        private void InitialFilling_Button_Click(object sender, RoutedEventArgs e)
        {
            InputRestriction(false);
            pbCalculationProgress.IsIndeterminate = true;
            Task[] tasks = new Task[3];
            tasks[0] = Task.Run(() => InitialFillingExtention.FillWithRandom(repository.bank.Departments[0] as Department<Entity>, qEntity));
            tasks[1] = Task.Run(() => InitialFillingExtention.FillWithRandom(repository.bank.Departments[1] as Department<Person>, qPerson));
            tasks[2] = Task.Run(() => InitialFillingExtention.FillWithRandom(repository.bank.Departments[2] as Department<Vip>, qVip));
            Task.Factory.ContinueWhenAll(tasks, completedTasks => this.Dispatcher.Invoke(() =>
            {
                pbCalculationProgress.IsIndeterminate = false;
                InputRestriction(true);
                MessageBox.Show("Начальное заполнение закончено");
                pbCalculationProgress.Value = 0;
            }));
        }



        //private void DoFilling()
        //{
        //    Task[] tasks = new Task[3];
        //    tasks[0] = Task.Factory.StartNew(() => InitialFillingExtention.FillWithRandom(repository.bank.Departments[0] as Department<Entity>, qEntity));
        //    tasks[1] = Task.Factory.StartNew(() => InitialFillingExtention.FillWithRandom(repository.bank.Departments[1] as Department<Person>, qPerson));
        //    tasks[2] = Task.Factory.StartNew(() => InitialFillingExtention.FillWithRandom(repository.bank.Departments[2] as Department<Vip>, qVip));
        //    Task.Factory.ContinueWhenAll(tasks, completedTasks => this.Dispatcher.Invoke(() =>
        //    {
        //        pbCalculationProgress.IsIndeterminate = false;
        //        InputRestriction(true);
        //        MessageBox.Show("Начальное заполнение закончено");
        //        pbCalculationProgress.Value = 0;
        //    }));
        //}

        private void InputRestriction(bool flag)
        {
            MonthlyCharge.IsEnabled = flag;
            LoyalityProgram.IsEnabled = flag;
            NewClient.IsEnabled = flag;
            OpenFile.IsEnabled = flag;
            SaveFile.IsEnabled = flag;
            InitialFilling.IsEnabled = flag;
            FlagInputRestriction = !flag;
        }

        //private void SetCanClose()
        //{

        //    NoClose = (FlagInputRestriction /*|| !Processing.Transactions.IsEmpty*/ || !SpamBot.MessageQueue.IsEmpty || SpamBot.OnLine || Processing.IsActive);//сделать поле вместо метода

        //}

        void DataWindow_Closing(object sender, CancelEventArgs e)
        {

            if (NoClose)
            {

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

        private void OnWindowClosing() //надо еще установить запрет на все действия, а то закрытие растянется на долго
        {
            this.Dispatcher.Invoke(() =>
            {
                PBtext.Text = "Программа закрывается. Подождите. ";
                MyWindow w = new MyWindow("Идет сохранение данных", 1000);
                w.Owner = this;
                w.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                w.ShowDialog();
              
            });
         
            while (NoClose)
            {
            }
            
            this.Dispatcher.Invoke(() =>
            {
                Application.Current.Shutdown();
            });
        }
    }
}
