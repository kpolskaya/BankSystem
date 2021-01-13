using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;


namespace BankSystem.Model
{
    [DataContract]
    [KnownType(typeof(Customer))]

    public class Entity : Customer 
    {
        //public static decimal Fee() { return fee; }
        //public static decimal Rate() { return rate; }
        public static decimal Fee;
        public static decimal Rate;

        static Entity()
        {
            Fee = 30; // 40 30
            Rate = 0.1m; // 24 10
        }


        public Entity()
        { }


        public Entity(string Name, string OtherName, string LegalId, string Phone)
            : base(Name, OtherName, LegalId, Phone)
        {
           
        }

        [JsonConstructor]
        public Entity(string Id, string Name, string OtherName, string LegalId, string Phone)
           : base(Id, Name, OtherName, LegalId, Phone)
        { }
    }
}
