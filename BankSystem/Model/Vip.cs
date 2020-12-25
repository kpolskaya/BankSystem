using System;

namespace BankSystem.Model
{
    public class Vip : Customer
    {
        //public static decimal Fee() { return fee; }
        //public static decimal Rate() { return rate; }
        public static decimal Fee;
        public static decimal Rate;
        static Vip()
        {
            Fee = 40; // 40 30
            Rate = 0.24m; // 24 10
        }

        public Vip() : base() { }

        public bool IsReliable { get; set; } // надежный ли заемщик - опция для физлиц

        public Vip(string Name, string OtherName, string LegalId, string Phone)
            : base(Name, OtherName, LegalId, Phone)
        {
            this.IsReliable = Customer.random.Next(3) == 0;
        }
    }
}