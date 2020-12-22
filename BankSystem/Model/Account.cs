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
        public decimal Balance { get; private set; }
        public AccountType Type { get; }

        public decimal Fee { get; set; } //месячная плата за обслуживание
        public decimal Rate { get; set; } //процент на остаток

        public Account(AccountType Type, string DepartmentId, string CustomerId)
        {
            this.Type = Type;
            this.Bic =  DepartmentId + CustomerId + Guid.NewGuid().ToString().Remove(8);
            this.Balance = 0;
            
        }
        public Account( string DepartmentId, string CustomerId)
        {
            this.Bic = DepartmentId + CustomerId + Guid.NewGuid().ToString().Remove(8);
            this.Balance = 0;

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

        public abstract decimal GetInterest();
        

    }
}