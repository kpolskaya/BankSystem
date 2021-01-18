using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Model
{
    public delegate void TransactionHandler(Division sender, Transaction t);

    [System.Runtime.Serialization.DataContract]
    [KnownType(typeof(Department<>))]
    public abstract class Division //TODO  везде проверки на null (GetAccountByBic) и расставить Exeptions
    {
        [DataMember]
        public string Id { get; protected set; }
        [DataMember]
        public string Name { get; protected set; }
        [DataMember]
        public ObservableCollection<Customer> Customers;
        [DataMember]
        protected decimal fee;
        [DataMember]
        protected decimal rate;
        [DataMember]
        protected ObservableCollection<Account> accounts; //
        
        public ReadOnlyObservableCollection<Account> Accounts { get { return new ReadOnlyObservableCollection<Account>(this.accounts); } }//TODO подумать как вообще избавиться от этого свойства
        
        [JsonConstructor]
        public Division(string Id, string Name, ObservableCollection<Account> accounts)
        {
            this.Id = Id;
            this.Name = Name;
            this.accounts = accounts;
            //this.Accounts = new ReadOnlyObservableCollection<Account>(this.accounts);
        }

        public Division()
        { }

        public Division(string Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
            this.accounts = new ObservableCollection<Account>();
            //this.Accounts = new ReadOnlyObservableCollection<Account>(this.accounts);

        }

        


        public Account GetAccountByBic(string Bic) //приватная?
        {
            return accounts.FirstOrDefault(x => x.Bic == Bic);
        }

      
        public void OpenAccount(AccountType type, Customer customer)
        {
            switch (type)
            {
                
                case AccountType.DebitAccount:
                    accounts.Add(new DebitAccount( this.Id, customer));
                    break;
                case AccountType.DepositAccount:
                    accounts.Add(new DepositAccount(this.Id, customer));
                    break;
                case AccountType.DepositAccountCapitalized:
                    accounts.Add(new DepositAccountСapitalized (this.Id, customer));
                    break;
                default:
                    break;
            }
        }

        public bool TryToCredit(string bic, decimal sum, string detailes)
        {
            Account account = GetAccountByBic(bic);
            bool executed = (account != null);
            if (executed)
                account.Credit(sum, detailes);
            return executed;
        }

        public void Refund(Transaction t)
        {
            
            if (!TryToCredit(t.SenderBic, t.Sum, "Возврат платежа (получатель не найден): " + t.Detailes))
                throw new Exception("Неизвестный счет получателя");
        }

        /// <summary>
        /// Закрывает месяц: рассчитывает и проводит все платежи и проценты
        /// </summary>
        public void CalculateCharges() 
        {
            foreach (var account in accounts)
            {

                if (account.Type != AccountType.DebitAccount)
                {
                    decimal Interest = account.ChargeInterest(rate);
                    OnTransactionRaised(this, new Transaction("99", account.Bic, Interest, "Начисление процентов"));
                }

                else
                    if (account.Debit(fee, "Плата за обслуживание")) 
                        OnTransactionRaised(this, new Transaction(account.Bic, "99", fee, "Плата за обслуживание"));
            }
        }

        /// <summary>
        /// Вносит деньги на счет
        /// </summary>
        /// <param name="bic">номер счета</param>
        /// <param name="sum">сумма</param>
        public void Put(string bic, decimal sum)
        {
            Account account = GetAccountByBic(bic);
            if (account == null)
                throw new Exception("Несуществующий счет");
            account.Credit(sum, "Пополнение через кассу");
            OnTransactionRaised(this, new Transaction("00", bic, sum, "Пополнение через кассу"));
        }

        /// <summary>
        /// Снимает деньги со счета
        /// </summary>
        /// <param name="bic">номер счета</param>
        /// <param name="sum">сумма</param>
        /// <returns>true -  успех, или false - недостаточно средств</returns>
        public bool Withdraw(string bic, decimal sum)
        {
            string detailes = "Выдача наличных";
            Account account = GetAccountByBic(bic);
            bool executed = (account != null && account.Debit(sum, detailes));
            if (executed)
                OnTransactionRaised(this, new Transaction(bic, "00", sum, detailes));
            return executed;
        }

        public void CloseAccount(string bic) 
        {
            string detailes = "Закрытие счета";
            Account account = GetAccountByBic(bic);
            if (account == null)
                throw new Exception("Несуществующий счет");
            //decimal sum = account.FullBalance();
            //account.Debit(sum);
            
            OnTransactionRaised(this, new Transaction(bic, "00", account.FullBalance(), detailes));
            this.accounts.Remove(account);
        }
           
     


        public void Transfer(string senderBic, string beneficiaryBic, decimal sum, string detailes = "")
        {
            Account senderAccount = GetAccountByBic(senderBic);
            if (senderAccount == null)
                throw new Exception("Несуществующий счет");
            Transaction t; 
            if (senderAccount is DebitAccount && senderAccount.Debit(sum, detailes))
                t = new Transaction(senderBic, beneficiaryBic, sum, detailes, TransactionType.Transfer);
            else
            {
                t = new Transaction(senderBic, beneficiaryBic, sum, detailes + " -- Отказано в операции", TransactionType.Transfer);
                t.Status = TransactionStatus.Failed;
            }
            OnTransactionRaised(this, t);
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
