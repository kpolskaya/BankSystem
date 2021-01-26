using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace BankSystem.Model
{
    [DataContract]
    [KnownType(typeof(Entity))]
    [KnownType(typeof(Person))]
    [KnownType(typeof(Vip))]

    public abstract class Customer
    {
        //public static decimal Fee;
        //public static decimal Rate;

        public static Random random;
        static uint lastId;
        static Customer()
        {
            lastId = 0;
            random = new Random();
        }

        static uint NextId()
        {
            return ++lastId;
        }

        static void SetLastId(uint id)
        {
            if (lastId < id)
                lastId = id;
        }

        [DataMember]
        public string Id { get; private set; }
        [DataMember]
        public string Name { get; set; } //имя/название фирмы
        [DataMember]
        public string OtherName { get; set; } //фамилия/форма собственности юр. лица
        [DataMember]
        public string LegalId { get; set; } // номер удостоверения личности или регистрации фирмы
        [DataMember]
        public string Phone { get; set; } //контактный телефон


        public Customer() { this.Id = NextId().ToString("00000000"); }

        public Customer (string Name, string OtherName, string LegalId, string Phone)
        {
            this.Name = Name;
            this.OtherName = OtherName;
            this.LegalId = LegalId;
            this.Phone = Phone;
            this.Id = NextId().ToString("00000000");

        }

        [JsonConstructor]
        public Customer(string Id, string Name, string OtherName, string LegalId, string Phone)
        {
            this.Name = Name;
            this.OtherName = OtherName;
            this.LegalId = LegalId;
            this.Phone = Phone;
            this.Id = Id;
            try
            {
                uint id = uint.Parse(Id);
                SetLastId(id);
            }
            catch (Exception)
            {

                throw new Exception("Несоответствующий формат поля Id клиента");
            }
        }

       //public Customer()
       // { }

        public void SendMessage(Account account, string message)
        {
            System.Media.SystemSounds.Asterisk.Play();
            Debug.WriteLine($"SMS to: {Phone} subj: Движение по счету {account.Bic} " +
                            $"message: {message}");
        }

    }
}