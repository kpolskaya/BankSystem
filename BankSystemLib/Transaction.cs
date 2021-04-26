using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace BankSystemLib
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
    
    public class Transaction
    {
        
        public string Id { get; private set; }
        /// <summary>
        /// Счет отправителя
        /// </summary>
        
        public string SenderBic { get; private set; }
        /// <summary>
        /// Счет получателя
        /// </summary>
       
        public string BeneficiaryBic { get; private set; }
        /// <summary>
        /// Сумма тразакции
        /// </summary>
        
        public decimal Sum { get; private set; }
        /// <summary>
        /// Детали платежа
        /// </summary>
        
        public string Detailes { get; set; }
       
        public TransactionType Type { get; private set; }
        
        public TransactionStatus Status { get; set; }

        public Transaction(string Id, string SenderBic, string BeneficiaryBic, decimal Sum, string Detailes, TransactionType Type = TransactionType.Internal)
        {
            this.Id = Id;
            this.SenderBic = SenderBic;
            this.BeneficiaryBic = BeneficiaryBic;
            this.Sum = Sum;
            this.Detailes = Detailes;
            this.Status = TransactionStatus.Pending;
            this.Type = Type;
        }

        public Transaction()
        { }
        public Transaction(string SenderBic, string BeneficiaryBic, decimal Sum, string Detailes, TransactionType Type = TransactionType.Internal)
            : this(Guid.NewGuid().ToString().Substring(0, 4) + DateTime.Now.ToLongTimeString(), SenderBic, BeneficiaryBic, Sum, Detailes, Type)
        {

        }
    }
}
