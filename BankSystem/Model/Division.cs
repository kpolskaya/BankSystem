using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Model
{
    public delegate void TransactionHandler(Department<Customer> sender, Transaction t);
    public abstract class Division // добавить конструктор
    {
        public string Id { get; }
        
        List<Account> accounts = new List<Account>();
       
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

        public bool Debit(string bic, decimal sum)
        {
            bool executed = false;
            ///логика

            return executed;
        }

        public void Credit(string bic, decimal sum)
        {
            ///логика
        }

        public void OpenAccount(AccountType Type, string DepartmentId, string CustomerId)
        {
            accounts.Add(new Account(Type, DepartmentId, CustomerId));
        }

        public event TransactionHandler TransactionRaised;

        protected virtual void OnTransactionRaised(Department<Customer> sender, Transaction t)
        {

            sender = this as Department<Customer>;
            TransactionRaised?.Invoke(sender, t);
        }
    }
}
