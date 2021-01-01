using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Model
{
    class DebitAccount: Account
    {
        
        public DebitAccount(string DepartmentId, string CustomerId)
        : base(AccountType.DebitAccount, DepartmentId, CustomerId)
        {
            
        }
        public override decimal ChargeInterest(decimal rate)
        {
            decimal i = this.Balance * rate / 12;
            Credit(i);
            return i;
        }
    }
}
