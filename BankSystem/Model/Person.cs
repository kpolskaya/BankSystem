using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace BankSystem.Model
{
    /// <summary>
    /// Клиент - Физическое лицо
    /// </summary>
    [DataContract]
    [KnownType(typeof(Customer))]
    public class Person : Customer
    {
        /// <summary>
        /// Плата за обслуживание для данного типа
        /// </summary>
        public static decimal Fee;
        /// <summary>
        /// Базовая ставка по депозиту для данного типа
        /// </summary>
        public static decimal Rate;
        
        static Person()
        {
            Fee = 10; // 40 30
            Rate = 0.12m; // 24 10
        }
        

        public Person()
        { 
        
        }

        public Person(string Name, string OtherName, string LegalId, string Phone) 
            : base(Name, OtherName, LegalId, Phone)
        {
           
        }

        [JsonConstructor]
        public Person(string Id, string Name, string OtherName, string LegalId, string Phone)
           : base(Id, Name, OtherName, LegalId, Phone)
        { 
        
        }
    }
}