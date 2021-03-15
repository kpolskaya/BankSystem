using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace BankSystemLib
{
    /// <summary>
    /// Клиент - Физическое лицо VIP
    /// </summary>
    [DataContract]
    [KnownType(typeof(Customer))]
    public class Vip : Customer
    {

        /// <summary>
        /// Плата за обслуживание для данного типа
        /// </summary>
        public static decimal Fee;

        /// <summary>
        /// Базовая ставка по депозиту для данного типа
        /// </summary>
        public static decimal Rate;

        static Vip()
        {
            Fee = 3000;
            Rate = 0.018m;
        }

        public Vip()
        {

        }

        public Vip(string Name, string OtherName, string LegalId, string Phone)
            : base(Name, OtherName, LegalId, Phone)
        {

        }

        [JsonConstructor]
        public Vip(string Id, string Name, string OtherName, string LegalId, string Phone)
            : base(Id, Name, OtherName, LegalId, Phone)
        {

        }
    }
}
