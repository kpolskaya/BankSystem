using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Model
{
    public class DepositAccount : Account
    {
        //public decimal Interest { get; private set; }

        //public new decimal Rate { get; }

        //public new decimal Fee { get; }

        public DepositAccount(string DepartmentId, string CustomerId)
           : base(DepartmentId, CustomerId)
        {

           
            //this.Rate = Customer.Rate;

            //this.Fee = 0;
        }


        //public override decimal GetInterest()
        //{
        //    decimal interest = (Balance-DepositInterest) * Rate / 12 * 100;
        //    return interest;
            
        //}
    }
}
