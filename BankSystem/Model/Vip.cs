using System;

namespace BankSystem.Model
{
    public class Vip : Customer
    {
        static Vip()
        {
            fee = 40; // 40 30
            rate = 0.24m; // 24 10
        }

        public bool IsReliable { get; set; } // надежный ли заемщик - опция для физлиц

        public Vip(string Name, string OtherName, string LegalId, string Phone)
            : base(Name, OtherName, LegalId, Phone)
        {
            this.IsReliable = Customer.random.Next(3) == 0;
        }
    }
}