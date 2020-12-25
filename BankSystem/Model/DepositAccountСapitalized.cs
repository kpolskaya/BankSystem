using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Model
{
    class DepositAccountСapitalized : Account
    {
        public DepositAccountСapitalized(string DepartmentId, string CustomerId)
           : base(DepartmentId, CustomerId)
        {
            //this.Rate = Customer.Rate;

            this.Fee = 0;


        }

        public override decimal GetInterest()
        {
            decimal interest = Balance * Rate / 12 * 100;
            return interest;
            throw new NotImplementedException();
        }
    }
}
