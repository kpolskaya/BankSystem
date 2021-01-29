using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;


namespace BankSystem.Model
{
    /// <summary>
    /// Клиент - Юридическое лицо
    /// </summary>
    [DataContract]
    [KnownType(typeof(Customer))]
    public class Entity : Customer 
    {
        /// <summary>
        /// Плата за обслуживание для данного типа
        /// </summary>
        public static decimal Fee;
        /// <summary>
        /// Базовая ставка по депозиту для данного типа
        /// </summary>
        public static decimal Rate;

        static Entity()
        {
            Fee = 30; // 40 30
            Rate = 0.1m; // 24 10
        }


        public Entity()
        { 
        
        }


        public Entity(string Name, string OtherName, string LegalId, string Phone)
            : base(Name, OtherName, LegalId, Phone)
        {
           
        }

        [JsonConstructor]
        public Entity(string Id, string Name, string OtherName, string LegalId, string Phone)
           : base(Id, Name, OtherName, LegalId, Phone)
        { 
        
        }
    }
}
