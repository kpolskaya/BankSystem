using System;

namespace BankSystem.Model
{
    public abstract class Account
    {
        public string Bic { get;}
        public decimal Balance { get; private set; }

        readonly decimal fee; //месячная плата за обслуживание
        readonly decimal rate; //процент на остаток

        public Account(string DepartmentId, string CustomerId, decimal Fee, decimal Rate)
        {
            this.Bic = DepartmentId + CustomerId + Guid.NewGuid().ToString().Remove(8);
            this.Balance = 0;
            this.fee = Fee;
            this.rate = Rate;
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