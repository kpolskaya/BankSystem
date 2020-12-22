using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Model
{
    public delegate void TransactionHandler(Division sender, Transaction t);
    public abstract class Division // добавить конструктор
    {
        public string Id { get; }
        public string Name { get; }
        
        List<Account> accounts = new List<Account>();

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



        public void OpenAccount(AccountType Type, string DepartmentId, string CustomerId, decimal DepositAmount = 0)
        {
            switch (Type)
            {
                case AccountType.DebitAccount:
                    accounts.Add(new DebitAccount( DepartmentId, CustomerId));
                    break;
                case AccountType.DepositAccount:
                    accounts.Add(new DepositAccount(DepartmentId, CustomerId, DepositAmount));
                    break;
                case AccountType.DepositAccountCapitalized:
                    accounts.Add(new DepositAccountСapitalized (DepartmentId, CustomerId));
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

       
        public void CalculationCharge()
        {
            foreach (var account in accounts)
            {
                decimal Interest = account.GetInterest();
                account.Credit(Interest);
                OnTransactionRaised(this, new Transaction("00", account.Bic, Interest));
                
                decimal Fee = account.Fee;
                if (account.Debit(Fee))
                    OnTransactionRaised(this, new Transaction(account.Bic, "00", Fee));
               
            }

            throw new NotImplementedException();
        }

        public abstract void CustomersForExample();

        public abstract void AccountsForExample();

        public abstract void RefillAccounts();
    }
}
