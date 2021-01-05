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
        public override bool Debit(decimal sum, string detailes)
        {
            string message;

            if (FullBalance() == sum)
            {

                Balance -= sum; 
                message = String.Format(
                             "Списание на сумму {0: 0.00}, основание: {1}. Остаток средств {2 : 0.00}",
                              sum, detailes, FullBalance());
                
                OnMovement(this, message);
                return true;
            }
            else
            {
                OnMovement(this, "Отказ - по данному счету расходные операции не разрешены.");
                return false;
            }

        }
    }
}
