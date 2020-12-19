using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace BankSystem.Model
{
    public delegate void TransactionHandler(Object sender, Transaction t);

    public class Department<TCustomer> : IDivision
    {
        public ObservableCollection<TCustomer> Customers { get; private set; }

        List<Account> accounts = new List<Account>();

        public ObservableCollection<string> Accounts
        { 
            get
            {
                return new ObservableCollection<string>(
                        (from a in accounts
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
            ///

            return executed;
        }
            
        public void Credit(string bic, decimal sum)
        {
            ///
        }

        public void OpenAccount(AccountType Type, string DepartmentId, string CustomerId)
        {
            accounts.Add(new Account(Type, DepartmentId, CustomerId));
        }

        public event TransactionHandler TransactionRaised;

        protected virtual void OnTransactionRaised(Object sender, Transaction t)
        {
            //Type type = sender.GetType();
            
            sender = this;
            TransactionRaised?.Invoke(sender, t);
        }


    }
}

