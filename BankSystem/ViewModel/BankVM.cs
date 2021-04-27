using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankSystem.Model;
using BankSystemLib;

namespace BankSystem.ViewModel
{
    public class BankVM : INotifyPropertyChanged
    {
        public ObservableCollection<DivisionVM> Departments { get; set; }

        public ReadOnlyCollection<Transaction> TransactionHistory { get { return bank.TransactionHistory; } }

        private readonly Bank bank;
        /// <summary>
        /// Прибыль банка
        /// </summary>
        public decimal Profit {get { return this.bank.Profit;} }
        /// <summary>
        /// Собственный капитал банка
        /// </summary>
        public decimal Capital { get { return this.bank.Cash - ClientsFunds; } }
        /// <summary>
        /// Деньги на клиентских счетах (обязательства)
        /// </summary>
        public decimal ClientsFunds { get { return this.bank.ClientsFunds(); } }
        /// <summary>
        /// Всего денег в банке
        /// </summary>
        public decimal Cash { get { return this.bank.Cash; } }
        public BankVM(Bank Bank)
        {
            bank = Bank;
            this.Departments = new ObservableCollection<DivisionVM>();
            foreach (var item in bank.Departments)
            {
                this.Departments.Add(new DivisionVM(item));
            }
            bank.BankBalanceChanged += UpdateTarget;  //подписка на изменения статей баланса
        }

        /// <summary>
        /// Выбранный пользователем департамент
        /// </summary>
        public DivisionVM SelectedItem
        {
            get
            {
                return this.Departments.FirstOrDefault(e => e.IsSelected);
            }
        }

        /// <summary>
        /// Сброс значения выбранного пользователем клиента
        /// </summary>
        public void ClearSelectedCustomer()
        {
            if (SelectedItem != null && SelectedItem.SelectedCustomer != null)
                SelectedItem.SelectedCustomer.IsSelected = false;
        }

        /// <summary>
        ///  Начисляет ежемесячные платежи и проценты по клиентским счетам
        /// </summary>
        public void MonthlyCharge(/*IProgress<int> progress*/)
        {
            bank.MonthlyCharge(/*progress*/);
            OnPropertyChanged("ClientsFunds");
            OnPropertyChanged("Capital");
        }


        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Обновляет значения полей банковского баланса
        /// </summary>
        /// <param name="name"></param>
        private void UpdateTarget (string name)
        {
            OnPropertyChanged(name);
            if (name == "Cash")
                OnPropertyChanged("ClientsFunds"); //меняются оба поля
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoyaltyProg()
        {

            (bank.Departments[0] as Department<Entity>).LoyaltyProgramExtension();
            (bank.Departments[1] as Department<Person>).LoyaltyProgramExtension();
            (bank.Departments[2] as Department<Vip>).LoyaltyProgramExtension();

        }
    
    }
}
