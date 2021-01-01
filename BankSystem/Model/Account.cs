using System;

namespace BankSystem.Model
{
    public enum AccountType : byte
    {
        DebitAccount = 0,
        DepositAccount = 1,
        DepositAccountCapitalized = 2,

    }

    public abstract class Account
    {
       
        public string Bic { get;}
        public decimal Balance { get; protected set; }
        public AccountType Type { get; }

        public decimal AccruedInterest { get; protected set; }

        //protected Func<decimal, decimal> interest;
      
        public Account(AccountType Type, string DepartmentId, string CustomerId) 
        {
            this.Type = Type;
            this.Bic =  DepartmentId + CustomerId + Guid.NewGuid().ToString().Remove(8);
            this.Balance = 0;
            this.AccruedInterest = 0;
            //this.interest = InterestFunc;
        }
       

        public virtual bool Debit(decimal sum)
        {
            if (Balance < sum)
                return false;
            Balance -= sum;
            return true;
                       
        }


        public void Credit(decimal sum)
        {
            Balance += sum;
        }

        /// <summary>
        /// Начисляет и проводит проценты по счету за месяц
        /// </summary>
        /// <param name="rate">процентная ставка</param>
        /// <returns>Начисленные за месяц проценты</returns>
        public abstract decimal ChargeInterest(decimal rate);
        
        public decimal FullBalance()
        {
            return this.Balance + this.AccruedInterest;
        }
        //protected abstract decimal InterestFunc(decimal rate);
        
        

    }
}