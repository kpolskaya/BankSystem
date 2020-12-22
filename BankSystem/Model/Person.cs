using System;

namespace BankSystem.Model
{
    public class Person : Customer
    {
       
        static Person()
        {
            fee = 10; // 40 30
            rate = 0.12m; // 24 10
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