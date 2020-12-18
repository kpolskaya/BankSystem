using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Model
{
    public enum TransactionStatus
    {
        Demanded,
        Done,
        Failed
    }
    public class Transaction
    {
        public string Id { get; }

        public string Sender { get; }

        public string Beneficiary { get; }

        public decimal Sum { get; }

        public TransactionStatus Status { get; set; }

        public Transaction(string Id, string Sender, string Beneficiary, decimal Sum)
        {
            this.Id = Id;
            this.Sender = Sender;
            this.Beneficiary = Beneficiary;
            this.Sum = Sum;
            this.Status = TransactionStatus.Demanded;
        }
    }
}
