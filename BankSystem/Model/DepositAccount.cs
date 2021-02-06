using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BankSystem.Model
{   
    /// <summary>
    /// Депозитный счет клиента без капитализации процентов
    /// </summary>
    [DataContract]
    [KnownType(typeof(Account))]
    public class DepositAccount : Account
    {
       
        public DepositAccount(string DepartmentId, Customer Customer)
        : base(AccountType.DepositAccount, DepartmentId, Customer)
        {

        }

        public DepositAccount()
        { }

        public override decimal ChargeInterest(decimal rate)
        {
            if (this.Balance > 0)
            {
                decimal i = Math.Round(this.Balance * rate / 12, 2, MidpointRounding.ToEven);//банковское округление
                this.AccruedInterest += i;
                string message = String.Format(
                                "Начислены проценты - {0: 0.00}. Остаток средств на счете {1 : 0.00}",
                                 i, FullBalance());
                OnMovement(message);
                return i;
            }
            else return 0;
        }


        public override bool Debit(decimal sum, string detailes)
        {
            if (sum <= 0)
                return false;
            
            string message;
            if (FullBalance() == sum)
            {
                Balance -= (sum - AccruedInterest);
                AccruedInterest = 0;
                message = String.Format(
                             "Списание на сумму {0: 0.00}, основание: {1}. Остаток средств {2 : 0.00}",
                              sum, detailes, FullBalance());
                
                OnMovement(message);
                return true;
            }
            else
            {
                OnMovement("Отказ - по данному счету расходные операции не разрешены.");
                return false;
            }
        }
    }
}
