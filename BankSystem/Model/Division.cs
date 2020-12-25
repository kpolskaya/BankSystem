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
        
        protected decimal fee;
        protected decimal rate;
        
        protected List<Account> accounts = new List<Account>();

        public Division(string Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
            this.accounts = new List<Account>();
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
                    accounts.Add(new DepositAccount(departmentId, customerId, depositAmount));
                    break;
                case AccountType.DepositAccountCapitalized:
                    accounts.Add(new DepositAccountСapitalized (departmentId, customerId));
                    break;
                default:
                    break;
            }

         }


        public event TransactionHandler TransactionRaised;

        protected virtual void OnTransactionRaised(Division sender, Transaction t)
        {

            sender = this;
            TransactionRaised?.Invoke(sender, t);
        }

        public abstract void CalculateCharge();
      

        public abstract void CustomersForExample();

        public abstract void AccountsForExample();

        //public abstract void RefillAccounts();
    }
}
