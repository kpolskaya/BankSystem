using System;
using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace BankSystem.Model
{
    /// <summary>
    /// Перечислитель типов счетов
    /// </summary>
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
        /// <summary>
        /// Номер счета
        /// </summary>
        [DataMember]
        public string Bic { get; protected set; }
        /// <summary>
        /// Текущий баланс счета
        /// </summary>
        [DataMember]
        public decimal Balance { get; protected set; }
        /// <summary>
        /// Тип счета
        /// </summary>
        [DataMember]
        public AccountType Type { get; protected set; }
        /// <summary>
        /// Сумма начисленных по счету процентов
        /// </summary>
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

        
        /// <summary>
        /// Операция списания средств
        /// </summary>
        /// <param name="sum">Сумма платежа</param>
        /// <param name="detailes">Детали операции</param>
        /// <returns> true - если операция успешна, false - если недостаточно средств на счете или некорректная сумма</returns>
        public virtual bool Debit(decimal sum, string detailes)
        {
            string message;

            if (FullBalance() >= sum && sum > 0)
            {
                Balance -= sum;
                message = String.Format(
                             "Списание на сумму {0: 0.00}, основание: {1}. Остаток средств {2 : 0.00}",
                              sum, detailes, FullBalance());
                OnMovement(message);
                return true;
            }
            else if (sum > 0)
            {
                message = String.Format("Отказ - недостаточно средств.Текущий остаток: {0: 0.00}", FullBalance());
                OnMovement(message);
                return false;
            }
            else return false;
        }

        /// <summary>
        /// Зачисляет средства на счет
        /// </summary>
        /// <param name="sum">Сумма прихода</param>
        /// <param name="detailes">Детали операции</param>
        public  void Credit(decimal sum, string detailes)
        {
            if (sum > 0)
            {
                Balance += sum;

                string message = String.Format(
                                 "Зачисление на сумму {0: 0.00}, основание: {1}. Остаток средств {2 : 0.00}",
                                  sum, detailes, FullBalance());
                OnMovement(message);
            }
            
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

        public void NotifyIfRemoved()
        {
            OnMovement($"Счет {this.Bic} закрыт.");
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
        protected virtual void OnMovement(string message)
        {
            Account sender = this;
            Movement?.Invoke(sender, message);
        }

    }
}