﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Model
{
    public delegate void TransactionHandler(Division sender, Transaction t);
    public abstract class Division //TODO  везде проверки на null (GetAccountByBic) и расставить Exeptions
    {
        public string Id { get; }
        public string Name { get; }

        public ObservableCollection<Customer> Customers;
        
        protected decimal fee;
        protected decimal rate;
        
        protected ObservableCollection<Account> accounts;
        public ReadOnlyObservableCollection<Account> Accounts { get; protected set; }

        public Division(string Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
            this.accounts = new ObservableCollection<Account>();
            this.Accounts = new ReadOnlyObservableCollection<Account>(this.accounts);
        
        }
 
        public Account GetAccountByBic(string Bic) //приватная?
        {
            return accounts.FirstOrDefault(x => x.Bic == Bic);
        }

      

        public bool Debit(string bic, decimal sum) //возможно не нужна или нужна только приватная
        {
            return GetAccountByBic(bic).Debit(sum);
        }

        public void Credit(string bic, decimal sum) // нужна публичная чтобы банк мог инициировать зачисление средств клиенту   или не нужна...
        {
            GetAccountByBic(bic).Credit(sum);
        }

        public void OpenAccount(AccountType type, string departmentId, string customerId)
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

        internal bool IsValidBic(string beneficiaryBic)
        {
            throw new NotImplementedException();
        }

        internal bool TryToCredit(string bic, decimal sum)
        {
            Account account = GetAccountByBic(bic);
            bool executed = account != null;
            if (executed)
                account.Credit(sum);
            return executed;
        }

        internal void Refund(Transaction t)
        {
            throw new NotImplementedException();
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
                    if (account.Debit(fee))
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
            Account account = GetAccountByBic(bic);
            bool executed = (account != null && account.Debit(sum));
            if (executed)
                OnTransactionRaised(this, new Transaction(bic, "00", sum, "Выдача наличных"));
            return executed;
        }

        public void CloseAccount(string bic) 
        {
            Account account = GetAccountByBic(bic);
            if (account == null)
                throw new Exception("Несуществующий счет");
            //decimal sum = account.FullBalance();
            //account.Debit(sum);
            
            OnTransactionRaised(this, new Transaction(bic, "00", account.FullBalance(), "Выдача наличных и закрытие счета"));
            this.accounts.Remove(account);
        }
           
                            


        public void Transfer(string senderBic, string beneficiaryBic, decimal sum, string detailes = "")
        {
            Account senderAccount = GetAccountByBic(senderBic);
            if (senderAccount == null)
                throw new Exception("Несуществующий счет");
            Transaction t; 
            if (senderAccount is DebitAccount && senderAccount.Debit(sum))
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
