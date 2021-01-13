using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace BankSystem.Model
{
    [DataContract]
    [KnownType(typeof(Customer))]

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
        //public bool IsReliable { get; set; } // надежный ли заемщик - опция для физлиц

        public Person()
        { }

        public Person(string Name, string OtherName, string LegalId, string Phone) 
            : base(Name, OtherName, LegalId, Phone)
        {
            //this.IsReliable = Customer.random.Next(3) == 0;
        }

        [JsonConstructor]
        public Person(string Id, string Name, string OtherName, string LegalId, string Phone)
           : base(Id, Name, OtherName, LegalId, Phone)
        { }
    }
}