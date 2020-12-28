using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Model
{
    public delegate void TransactionHandler(Division sender, Transaction t);
    public abstract class Division 
    {
        public string Id { get; }
        public string Name { get; }

        public ObservableCollection<Customer> Customers;
        
        protected decimal fee;
        protected decimal rate;
        
        protected ObservableCollection<Account> accounts = new ObservableCollection<Account>();


        public Division(string Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
            this.accounts = new ObservableCollection<Account>();
           
        }
       
        public ReadOnlyObservableCollection<string> Accounts
        {
            get
            {
                return new ReadOnlyObservableCollection<string>(
                        new ObservableCollection<string>(from a in accounts
                         select a.Bic));
            }
        }

        public Account GetAccountByBic(string Bic)
        {
            Account account = accounts.FirstOrDefault(x => x.Bic == Bic);

            return account;
        }

        public bool Debit(string bic, decimal sum) //возможно не нужна или нужна только приватная
        {
            bool executed = false;
            Account account = GetAccountByBic(bic);
            account.Debit(sum);
            return executed;
        }

        public void Credit(string bic, decimal sum) // нужна публичная чтобы банк мог инициировать зачисление средств клиенту
        {
            Account account = GetAccountByBic(bic);
            account.Credit(sum);
        }



        public void OpenAccount(AccountType type, string departmentId, string customerId, decimal depositAmount = 0)
        {
            switch (type)
            {
                case AccountType.DebitAccount:
                    accounts.Add(new DebitAccount( departmentId, customerId));
                    break;
                case AccountType.DepositAccount:
                    accounts.Add(new DepositAccount(departmentId, customerId));
                    break;
                case AccountType.DepositAccountCapitalized:
                    accounts.Add(new DepositAccountСapitalized (departmentId, customerId));
                    break;
                default:
                    break;
            }

        }

        public decimal GetInterest(Account account)
        {             
            decimal interest = account.Balance * rate / 12 * 100;
            return  interest;
        }

        public void CalculateCharge() // нужно наверное сделать универсальный метод, который в зависимости от типа счета считает fee | interest
                                               // и может лучше его реализовать прямо в классе Division без абстракции?
        {
            foreach (var account in accounts)
            {
                //Type myType = Type.GetType(Customer, false, true);
                //var myType = GetType();
                //myType.GetFields();

                // Console.WriteLine($"{field.FieldType} {field.Name}");
                decimal Interest = GetInterest(account); // нужно убрать из Account этот метод
                account.Credit(Interest);
                OnTransactionRaised(this, new Transaction("00", account.Bic, Interest));

                decimal Fee = fee; // fee и rate брать прямо из this.rate | this.fee
                if (account.Debit(Fee))
                    OnTransactionRaised(this, new Transaction(account.Bic, "00", Fee));

            }


        }


        public event TransactionHandler TransactionRaised;

        protected virtual void OnTransactionRaised(Division sender, Transaction t)
        {

            sender = this;
            TransactionRaised?.Invoke(sender, t);
        }

       
        public abstract void CreateCustomer(string name, string otherName, string legalId, string phone);

        public abstract void CustomersForExample();

        public abstract void AccountsForExample();

        //public abstract void RefillAccounts();
    }
}
