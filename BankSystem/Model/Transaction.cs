using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BankSystem.Model
{
    /// <summary>
    /// Перечислитель статусов транзакции
    /// </summary>
    public enum TransactionStatus
    {
        Pending,
        Done,
        Failed
    }
    
    /// <summary>
    /// Перечислитель типов транзакции
    /// </summary>
    public enum TransactionType
    {
        Internal,
        Transfer
    }
    /// <summary>
    /// Запись в журнале банковских операций
    /// </summary>
    [DataContract]
    public class Transaction
    {
        [DataMember]
        public string Id { get; private set; }
        /// <summary>
        /// Счет отправителя
        /// </summary>
        [DataMember]
        public string SenderBic { get; private set; }
        /// <summary>
        /// Счет получателя
        /// </summary>
        [DataMember]
        public string BeneficiaryBic { get; private set; }
        /// <summary>
        /// Сумма тразакции
        /// </summary>
        [DataMember]
        public decimal Sum { get; private set; }
        /// <summary>
        /// Детали платежа
        /// </summary>
        [DataMember]
        public string Detailes { get;  set; }
        [DataMember]
        public TransactionType Type { get; private set; }
        [DataMember]
        public TransactionStatus Status { get; set; }
        
        public Transaction(string Id, string Sender, string Beneficiary, decimal Sum, string Detailes, TransactionType Type = TransactionType.Internal)
        {
            this.Id = Id;
            this.SenderBic = Sender;
            this.BeneficiaryBic = Beneficiary;
            this.Sum = Sum;
            this.Detailes = Detailes;
            this.Status = TransactionStatus.Pending;
            this.Type = Type;
        }

        public Transaction()
        { }
        public Transaction(string Sender, string Beneficiary, decimal Sum, string Detailes, TransactionType Type = TransactionType.Internal)
            : this (Guid.NewGuid().ToString().Substring(0,6), Sender, Beneficiary, Sum, Detailes, Type)
        {
            
        }
    }
}
