using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Model
{
    class DepositAccountСapitalized : Account
    {
       
        public DepositAccountСapitalized(string DepartmentId, Customer Customer)
        : base(AccountType.DepositAccountCapitalized, DepartmentId, Customer)
        {
            

        }

        public override decimal ChargeInterest(decimal rate)
        {
            decimal i = this.FullBalance() * rate / 12;
            this.AccruedInterest += i;
            return i;
        }
    }
}
