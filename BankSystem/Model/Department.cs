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
        
        public Department(string Id, string Name) : base(Id, Name)
        {
            this.Customers = new ObservableCollection<TCustomer>();
            Type cType = typeof(TCustomer);
            //  этот код нужно поместить в конструктор для json  или переделать на автосвойство { get { return (decimal)typeof(TCustomer).GetField(...); } }
            this.rate = (decimal)cType.GetField("Rate", BindingFlags.Static | BindingFlags.Public).GetValue(null);
            this.fee = (decimal)cType.GetField("Fee", BindingFlags.Static | BindingFlags.Public).GetValue(null);
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

        public override decimal ClientsFunds()
        {
            decimal total = 0;
            foreach (var item in this.accounts)
            {
                total += item.FullBalance();
            }
            return total;
        }
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

