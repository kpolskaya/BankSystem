using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Model
{
    public class DepositAccount : Account
    {
        
        public DepositAccount(string DepartmentId, string CustomerId)
        : base(AccountType.DepositAccount, DepartmentId, CustomerId)
        {

        }
        
        public override decimal ChargeInterest(decimal rate)
        {
            decimal i = this.Balance * rate / 12;
            this.AccruedInterest += i;
            return i;
        }

    }
}
