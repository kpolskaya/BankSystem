using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace BankSystemLib
{
   
    [DataContract]
    [KnownType(typeof(Entity))]
    [KnownType(typeof(Person))]
    [KnownType(typeof(Vip))]
    public  class Customer 
    {
        //public static Random random;
        static uint lastId;
        static Customer()
        {
            lastId = 0;
            //random = new Random();
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
        /// <summary>
        /// Имя / Название фирмы
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Фамилия / Форма собственности юр. лица
        /// </summary>
        [DataMember]
        public string OtherName { get; set; }
        
        /// <summary>
        /// Номер удостоверения личности или регистрации фирмы
        /// </summary>
        [DataMember]
        public string LegalId { get; set; } 
        [DataMember]
        public string Phone { get; set; }

        public Customer()
        {
            this.Id = NextId().ToString("00000000"); 
        }

        public Customer (string Name, string OtherName, string LegalId, string Phone)
        {
            this.Name = Name;
            this.OtherName = OtherName;
            this.LegalId = LegalId;
            this.Phone = Phone;
            this.Id = NextId().ToString("00000000");
        }

        /// <summary>
        /// Конструктор необходим для правильной нумерации клиентов после восстановления из .json
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Name"></param>
        /// <param name="OtherName"></param>
        /// <param name="LegalId"></param>
        /// <param name="Phone"></param>
        public Customer(string Id, string Name, string OtherName, string LegalId, string Phone) 
        {
            this.Name = Name;
            this.OtherName = OtherName;
            this.LegalId = LegalId;
            this.Phone = Phone;
            try
            {
                uint id = uint.Parse(Id);
                this.Id = id.ToString("00000000");
                SetLastId(id);
            }
            catch (Exception)
            {

                throw new CustomerIdFormatException();
            }
        }

        /// <summary>
        /// Отправляет сообщение клиенту об операциях по его счету
        /// </summary>
        /// <param name="account">Счет операции</param>
        /// <param name="message">Сообщение</param>
        public virtual void SendMessage(Account account, string message)
        {
            System.Media.SystemSounds.Asterisk.Play();
            Debug.WriteLine($"SMS to: {Phone} subj: Движение по счету {account.Bic} " +
                            $"message: {message}");
        }

    }

}
