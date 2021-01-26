using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BankSystem.Model
{[DataContract]
    public enum TransactionStatus
    {
        Pending,
        Done,
        Failed
    }
    [DataContract]
    public enum TransactionType
    {
        Internal,
        Transfer
    }

    [DataContract]
    public class Transaction
    {
        [DataMember]
        public string Id { get; private set; }
        [DataMember]
        public string SenderBic { get; private set; }
        [DataMember]
        public string BeneficiaryBic { get; private set; }
        [DataMember]
        public decimal Sum { get; private set; }
        [DataMember]
        public string Detailes { get; private set; }
        [DataMember]
        public TransactionType Type { get; private set; }
        [DataMember]
        public TransactionStatus Status { get; set; }
        [JsonConstructor]
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
