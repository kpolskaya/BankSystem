﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BankSystem.Model
{
    [DataContract]
    [KnownType(typeof(Account))]
    public class DebitAccount: Account
    {
        [JsonConstructor]
        public DebitAccount(string Bic, decimal Balance, AccountType Type, decimal AccruedInterest)
           : base(Bic, Balance, Type, AccruedInterest)
        { }

        public DebitAccount ()
        { }
        
        public DebitAccount(string DepartmentId, Customer Customer)
        : base(AccountType.DebitAccount, DepartmentId, Customer)
        {
            
        }

       
        public override decimal ChargeInterest(decimal rate)
        {
            decimal i = this.Balance * rate / 12;
            Credit(i, "Начислены проценты");
            return i;
        }


    }
}
