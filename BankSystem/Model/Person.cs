using System;

namespace BankSystem.Model
{

    public class Person : Customer
    {
        //public static decimal Fee() { return fee; }
        //public static decimal Rate() { return rate; }
        public static decimal Fee;
        public static decimal Rate;
        static Person()
        {
            Fee = 10; // 40 30
            Rate = 0.12m; // 24 10
        }
        public bool IsReliable { get; set; } // надежный ли заемщик - опция для физлиц

        public Person() : base() { }
        public Person(string Name, string OtherName, string LegalId, string Phone) 
            : base(Name, OtherName, LegalId, Phone)
        {
            this.IsReliable = Customer.random.Next(3) == 0;
        }
    }
}