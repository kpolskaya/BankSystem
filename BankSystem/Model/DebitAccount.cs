using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Model
{
    class DebitAccount: Account
    {
        public DebitAccount(string DepartmentId, string CustomerId, decimal Fee, decimal Rate)
        : base(DepartmentId, CustomerId)
        {
            
            this.rate = Rate;

        
        }

        public override decimal GetInterest()
        {
            decimal Interest = Balance * rate / 100;

            throw new NotImplementedException();
        }
    }
}
