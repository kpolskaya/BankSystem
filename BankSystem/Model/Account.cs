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
    /// <summary>
    /// Делегат для события "Операция по счету"
    /// </summary>
    /// <param name="sender">Счет, по которому проведена операция</param>
    /// <param name="message">Передаваемое сообщение о деталях операции</param>
    public delegate void OperationInfoHandler(Account sender, string message);
    
    /// <summary>
    /// Абстрактный класс для всех типов клиентских счетов
    /// </summary>
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
            this.Movement += Customer.SendMessage; // нужно при создании счета подписать владельца на события

        }
        public Account ()
        { }

        //public Account(string Bic, decimal Balance, AccountType Type, decimal AccruedInterest) //что будет если удалить этот конструктор? - Ничего!
        //{
        //    this.Bic = Bic;
        //    this.Balance = Balance;
        //    this.Type = Type;
        //    this.AccruedInterest = AccruedInterest;
        //}

        /// <summary>
        /// Операция списания средств
        /// </summary>
        /// <param name="sum">Сумма платежа</param>
        /// <param name="detailes">Детали операции</param>
        /// <returns> true - если операция успешна, false - если недостаточно средств на счете</returns>
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

        /// <summary>
        /// Зачисляет средства на счет
        /// </summary>
        /// <param name="sum">Сумма прихода</param>
        /// <param name="detailes">Детали операции</param>
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
        
        /// <summary>
        /// Событие, возникающее при проведении операции по счету
        /// </summary>
        public event OperationInfoHandler Movement; 

        /// <summary>
        /// Вызов обработчика события "Операция по счету"
        /// </summary>
        /// <param name="sender">Счет операции</param>
        /// <param name="message">Сообщение владельцу счета</param>
        protected virtual void OnMovement(Account sender, string message)
        {
            Movement?.Invoke(sender, message);
        }

    }
}