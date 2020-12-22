using System;

namespace BankSystem.Model
{
    public abstract class Customer
    {
        public static decimal fee;
        public static decimal rate;

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
 
        public string Id { get;}
        public string Name { get; set; } //имя/название фирмы
        public string OtherName { get; set; } //фамилия/форма собственности юр. лица
        public string LegalId { get; set; } // номер удостоверения личности или регистрации фирмы
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



    }
}