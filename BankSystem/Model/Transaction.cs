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
        static uint lastId;
        static Transaction()
        {
            lastId = 0;
 
        }

        static uint NextId()
        {
            return ++lastId;
        }

        public uint Id { get; }

        public string SenderBic { get; }

        public string BeneficiaryBic { get; }

        public decimal Sum { get; }

        public TransactionStatus Status { get; set; }

        public Transaction(uint Id, string Sender, string Beneficiary, decimal Sum)
        {
            this.Id = NextId();
            this.SenderBic = Sender;
            this.BeneficiaryBic = Beneficiary;
            this.Sum = Sum;
            this.Status = TransactionStatus.Demanded;
        }
        public Transaction(string Sender, string Beneficiary, decimal Sum)
        {
            this.Id = NextId();
            this.SenderBic = Sender;
            this.BeneficiaryBic = Beneficiary;
            this.Sum = Sum;
            this.Status = TransactionStatus.Demanded;
        }
    }
}
