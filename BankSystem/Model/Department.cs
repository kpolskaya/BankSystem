using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BankSystem.Model
{
    /// <summary>
    /// Департамент банка
    /// </summary>
    /// <typeparam name="TCustomer">Тип клиентов департамента</typeparam>
    [DataContract]
    public class Department<TCustomer>  : Division where TCustomer : Customer, new()

    {
        /// <summary>
        /// Список клиентов департамента
        /// </summary>
        [DataMember]
        public ObservableCollection<TCustomer> Customers { get; private set; }

        /// <summary>
        /// Ставка по депозитам для данного типа клиентов
        /// </summary>
        public decimal Rate 
        { get 
            { return (decimal)typeof(TCustomer).GetField("Rate", BindingFlags.Static | BindingFlags.Public).GetValue(null); } 
        }
        /// <summary>
        /// Плата за обслуживание для данного типа клиентов
        /// </summary>
        public decimal Fee 
        { get 
            { return (decimal)typeof(TCustomer).GetField("Fee", BindingFlags.Static | BindingFlags.Public).GetValue(null); } 
        }
        
        public Department(string Id, string Name) : base(Id, Name)
        {
            this.Customers = new ObservableCollection<TCustomer>();
        }

        public Department()
        { 
        
        }

        [JsonConstructor]
        public Department(string Id, string Name, ObservableCollection<Account> Accounts, ObservableCollection<TCustomer> Customers)
            : base(Id, Name, Accounts)
        {
            this.Customers = Customers;
        }
            
        // логика, которая зависит от типа клиента...

        /// <summary>
        /// Создает клиента департамента с типом, соответствующим аргументу типа департамента
        /// </summary>
        /// <param name="name">Имя/наименование</param>
        /// <param name="otherName">Фамилия/форма собственности</param>
        /// <param name="legalId">Паспорт/рег. номер</param>
        /// <param name="phone">Телефон</param>
        public override void CreateCustomer(string name, string otherName, string legalId, string phone) 
        { 
            TCustomer customer = new TCustomer
            {
                Name = name,
                OtherName = otherName,
                LegalId = legalId,
                Phone = phone
            };
            this.Customers.Add(customer);
        }
        
        /// <summary>
        /// Обновляет подписки клиентов на движения по счетам после десериализации
        /// </summary>
        public override void RefreshSubscriptions()
        {
            foreach (var customer in this.Customers)
            {
                foreach (var acc in this.accounts)
                {
                    if (customer.Id == acc.Bic.Substring(2, 8))
                        acc.Movement += customer.SendMessage;
                }
            }
        }

        /// <summary>
        /// Рассчитывает ежемесячные платежи за обслуживание и проценты по депозитам
        /// </summary>
        public override void CalculateCharges()
        {
            foreach (var account in accounts)
            {

                if (account.Type != AccountType.DebitAccount)
                {
                    decimal interest = account.ChargeInterest(Rate);
                    if (interest > 0)
                        OnTransactionRaised(new Transaction("99", account.Bic, interest, "Начисление процентов"));
                }

                else
                    if (account.Debit(Fee, "Плата за обслуживание"))
                    OnTransactionRaised(new Transaction(account.Bic, "99", Fee, "Плата за обслуживание"));
            }
        }

        #region ForExample
        public void CustomersForExample()
        {
            for (int i = 0; i < 1; i++) //3!!!!!
            {
                string name = ($"Клиент  {this.Id}-{i + 1}");
                string othername = ($"Имярек  {this.Id}-{i + 1}");
                string legalId = ($"{this.Id}-00000-{i + 1}");
                string phone = ($"+7 499 {this.Id}0000{i + 1}");
                CreateCustomer(name, othername, legalId, phone);

            }
            AccountsForExample();
        }

        private void AccountsForExample()
        {
            foreach (var customer in Customers)
            {
                OpenAccount(AccountType.DebitAccount, customer);
            }
        }
        #endregion
    }
}

