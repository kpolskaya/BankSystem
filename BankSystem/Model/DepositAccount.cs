using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Model
{
    public class DepositAccount : Account
    {
        
        public DepositAccount(string DepartmentId, Customer Customer)
        : base(AccountType.DepositAccount, DepartmentId, Customer)
        {

        }
        
        public override decimal ChargeInterest(decimal rate)
        {
            decimal i = this.Balance * rate / 12;
            this.AccruedInterest += i;
            return i;
        }


        public override bool Debit(decimal sum, string detailes)
        {
            string message = String.Format(
                             "Списание на сумму {0: 0.00}, основание: {1}. Остаток средств {2 : 0.00}",
                              sum, detailes, Balance);

            if (Balance + AccruedInterest == sum)
            {
                Balance -= sum;
                OnMovement(this, message);
                return true;
            }

            else
            {
                OnMovement(this, "Отказ - по данному счету расходные операции не разрешены: " + message);
                return false;
            }

        }
    }
}
