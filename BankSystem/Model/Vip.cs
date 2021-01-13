using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace BankSystem.Model
{
    [DataContract]
    [KnownType(typeof(Customer))]
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

        public Vip()
        { }



        public Vip(string Name, string OtherName, string LegalId, string Phone)
            : base(Name, OtherName, LegalId, Phone)
        {
           
        }

        [JsonConstructor]
        public Vip(string Id, string Name, string OtherName, string LegalId, string Phone)
            : base(Id, Name, OtherName, LegalId, Phone)
        { }
    }
}