using System;
using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace BankSystem.Model
{

    public enum AccountType : byte 
    {
        DebitAccount = 0,
        DepositAccount = 1,
        DepositAccountCapitalized = 2,

    }



    public delegate void OperationInfoHandler(Account sender, string message);
    [DataContract]
    [KnownType(typeof(DebitAccount))]
    [KnownType(typeof(DepositAccount))]
    [KnownType(typeof(DepositAccountСapitalized))]
    public abstract class Account
    {
       [DataMember]
        public string Bic { get; protected set; }
        [DataMember]
        public decimal Balance { get; protected set; }
        [DataMember]
        public AccountType Type { get; protected set; }
        [DataMember]
        public decimal AccruedInterest { get; protected set; }

              
        public Account(AccountType Type, string DepartmentId, Customer Customer) 
        {
            this.Type = Type;
            this.Bic =  DepartmentId + Customer.Id + Guid.NewGuid().ToString().Remove(8);
            this.Balance = 0;
            this.AccruedInterest = 0;
            this.Movement += Customer.SendMessage;

            //this.interest = InterestFunc;
        }
        public Account ()
        { }

        [JsonConstructor]
         public Account(string Bic, decimal Balance, AccountType Type, decimal AccruedInterest)
         {
            this.Bic = Bic;
            this.Balance = Balance;
            this.Type = Type;
            this.AccruedInterest = AccruedInterest;
            //this.Movement += Customer.SendMessage; - не работает, потому что неизвестен клиент!
            //сделать в департаменте публичный метод переподписки клиентов после загрузки базы.

        }

        public virtual bool Debit(decimal sum, string detailes)
        {
            string message; 

            if (FullBalance() >= sum)
            {
                Balance -= sum;
                message = String.Format(
                             "Списание на сумму {0: 0.00}, основание: {1}. Остаток средств {2 : 0.00}",
                              sum, detailes, FullBalance());
                OnMovement(this, message);
                return true;
            }
            
            else
            {
                message = String.Format("Отказ - недостаточно средств.Текущий остаток: {0: 0.00}", FullBalance());
                OnMovement(this, message);
                return false;
            }

        }


        public  void Credit(decimal sum, string detailes)
        {
            Balance += sum;

            string message = String.Format(
                             "Зачисление на сумму {0: 0.00}, основание: {1}. Остаток средств {2 : 0.00}",
                              sum, detailes, FullBalance());

            OnMovement(this, message);
        }

        /// <summary>
        /// Начисляет и проводит проценты по счету за месяц
        /// </summary>
        /// <param name="rate">процентная ставка</param>
        /// <returns>Начисленные за месяц проценты</returns>
        public abstract decimal ChargeInterest(decimal rate);
        
        public decimal FullBalance()
        {
            return this.Balance + this.AccruedInterest;
        }
        
       
        public event OperationInfoHandler Movement; //подписка на движение по счетам пропадает после загрузки из json - конструкторЁ!ЁЁ!

        protected virtual void OnMovement(Account sender, string message)
        {
            Movement?.Invoke(sender, message);
        }

    }
}