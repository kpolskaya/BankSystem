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
    /// Текущий счет клиента
    /// </summary>
    [DataContract]
    [KnownType(typeof(Account))]
    public class DebitAccount: Account
    {
 
        public DebitAccount ()
        { }
        
        public DebitAccount(string DepartmentId, Customer Customer)
        : base(AccountType.DebitAccount, DepartmentId, Customer)
        {
            
        }

        /// <summary>
        /// В этой версии начисление процентов по текущим счетам не реализовано
        /// </summary>
        /// <param name="rate">ставка</param>
        /// <returns> всегда 0</returns>
        public override decimal ChargeInterest(decimal rate)
        {
            return 0;
        }


    }
}
