using System;

namespace BankSystem.Model
{
    public class Person : Customer
    {
        
        public bool IsReliable { get; set; } // надежный ли заемщик - опция для физлиц
        
        public Person(string Name, string OtherName, string LegalId, string Phone) 
            : base(Name, OtherName, LegalId, Phone)
        {
            this.IsReliable = Customer.random.Next(3) == 0;
        }
    }
}