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
        static object locker = new object();
        public static bool FlagInitialFilling = false;
        public static int qEntity = 1000;
        public static int qPerson = 1000;
        public static int qVip = 1000;
       // public static int qTotal = qEntity + qPerson + qVip;
       // public static int pbProgress;

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
        private async void Customers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Customers.SelectedItem != null)
            {
                CustomerInfo newWindow = new CustomerInfo(bank.SelectedItem);
                newWindow.Owner = this;
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
        private async void Serialize_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pbCalculationProgress.IsIndeterminate = true;
                this.IsHitTestVisible = false;
                await Task.Run(() => repository.SerializeJsonBank());
                pbCalculationProgress.IsIndeterminate = false;
                pbCalculationProgress.Value = 0;
                MessageBox.Show("Файл сохранен");
                this.IsHitTestVisible = true;
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
                this.IsHitTestVisible = false;
                //PbOpen();


                await Task.Run(() => repository.DeserializeJsonBank());
                pbCalculationProgress.IsIndeterminate = false;
                pbCalculationProgress.Value = 0;
                MessageBox.Show("Файл прочитан");
                this.IsHitTestVisible = true;
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
        {

            try
            {
                pbCalculationProgress.IsIndeterminate = true;
                await DoMonthlyCharge();
                
            }
            catch (Exception ex) //при автосохранении возможно исключение
            {
                MessageBox.Show(ex.Message);
            }
        }


        private async Task DoMonthlyCharge()
        {

            //var progress = new Progress<int>(ReportProgress);
            await Task.Factory.StartNew(() => bank.MonthlyCharge(/*progress*/));
            pbCalculationProgress.IsIndeterminate = false;
            pbCalculationProgress.Value = 0;
            MessageBox.Show("Закрытие месяца выполнено");
            
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

        private async void LoyalityProgram_Button_Click(object sender, RoutedEventArgs e)
        {
            pbCalculationProgress.IsIndeterminate = true;
            await Task.Run(()=>bank.LoyaltyProg());
            pbCalculationProgress.IsIndeterminate = false;
            MessageBox.Show("Призы по программе лояльности вручены");
        }

       
        //private async void InitialFilling_Button_Click(object sender, RoutedEventArgs e)
        //{
        //    MonthlyCharge.IsEnabled = false;
        //    LoyalityProgram.IsEnabled = false;
        //    await DoFilling();
           
        //}

        private void InitialFilling_Button_Click(object sender, RoutedEventArgs e)
        {
            this.IsHitTestVisible = false;
            FlagInitialFilling = true;
            pbCalculationProgress.IsIndeterminate = true;
            DoFilling();

        }
        private void DoFilling()
        {
           
            //var progress = new Progress<int>(ReportProgress);
            Task[] tasks = new Task[3];
            tasks[0] = Task.Factory.StartNew(() => InitialFilling.InitialFillingExtension(repository.bank.Departments[0] as Department<Entity>, qEntity/*, progress*/));
            tasks[1] = Task.Factory.StartNew(() => InitialFilling.InitialFillingExtension(repository.bank.Departments[1] as Department<Person>, qPerson/*, progress*/));
            tasks[2] = Task.Factory.StartNew(() => InitialFilling.InitialFillingExtension(repository.bank.Departments[2] as Department<Vip>, qVip/*, progress*/));
            Task.Factory.ContinueWhenAll(tasks, completedTasks => this.Dispatcher.Invoke(() =>
            {
                pbCalculationProgress.IsIndeterminate = false;
                this.IsHitTestVisible = true;
                FlagInitialFilling = false;
                MessageBox.Show("Начальное заполнение закончено");
                pbCalculationProgress.Value = 0;
            }));

            //pbCalculationProgress.Value = 0;
        }

        //private async Task DoFilling()
        //{
        //    var progress = new Progress<int>(ReportProgress);

        //    await Task.Factory.StartNew(() => InitialFilling.InitialFillingExtension(repository.bank.Departments[0] as Department<Entity>, 100, progress));
        //    await Task.Factory.StartNew(() => InitialFilling.InitialFillingExtension(repository.bank.Departments[1] as Department<Person>, 100, progress));
        //    await Task.Factory.StartNew(() => InitialFilling.InitialFillingExtension(repository.bank.Departments[2] as Department<Vip>, 100, progress));
        //    this.Dispatcher.Invoke(() =>
        //    {
        //      MonthlyCharge.IsEnabled = true;
        //      LoyalityProgram.IsEnabled = true;
        //    });

        //  MessageBox.Show("Начальное заполнение закончено");
        //    pbCalculationProgress.Value = 0;
        //}

        //private void ReportProgress(int value)
        //{
           
        //     pbCalculationProgress.Value = ConvertToPercentage(value, 100);
            
        //}

        //private double ConvertToPercentage(int value, int count)
        //{
        //    return (double)value * 100 / count;
        //}

    }
}
