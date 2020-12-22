using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Model
{
    class DebitAccount: Account
    {
        public decimal DepositAmount { get; private set; }

        //public new decimal Rate { get; }

        //public new decimal Fee { get; }

        public DebitAccount(string DepartmentId, string CustomerId)
        : base(DepartmentId, CustomerId)
        {
            this.Rate = 0;

            this.Fee = Customer.fee;

        }

        public override decimal GetInterest()
        {
            decimal interest = Balance * Rate / 12 * 100;
            return interest;

            throw new NotImplementedException();
        }
    }
}
