using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Model
{
    public enum TransactionStatus
    {
        Pending,
        Done,
        Failed
    }

    public enum TransactionType
    {
        Internal,
        Transfer
    }
    public class Transaction
    {
        public string Id { get; }

        public string SenderBic { get; }

        public string BeneficiaryBic { get; }

        public decimal Sum { get; }

        public string Detailes { get; }

        public TransactionType Type { get; }

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
        public Transaction(string Sender, string Beneficiary, decimal Sum, string Detailes, TransactionType Type = TransactionType.Internal)
            : this (Guid.NewGuid().ToString(), Sender, Beneficiary, Sum, Detailes, Type)
        {
            
        }
    }
}
