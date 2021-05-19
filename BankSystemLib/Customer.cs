using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace BankSystemLib
{
   
    [DataContract]
    [KnownType(typeof(Entity))]
    [KnownType(typeof(Person))]
    [KnownType(typeof(Vip))]
    public  class Customer 
    {
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
            this.Id = Guid.NewGuid().ToString().Substring(0, 8);
        }

        public Customer (string Name, string OtherName, string LegalId, string Phone)
        {
            this.Name = Name;
            this.OtherName = OtherName;
            this.LegalId = LegalId;
            this.Phone = Phone;
            this.Id = Guid.NewGuid().ToString().Substring(0, 8);
        }

        /// <summary>
        /// Конструктор необходим для восстановления из .json
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
            
            if (Id.Length != 8)
                throw new CustomerIdFormatException();
            else
                this.Id = Id;

        }

        /// <summary>
        /// Отправляет сообщение клиенту об операциях по его счету
        /// </summary>
        /// <param name="account">Счет операции</param>
        /// <param name="message">Сообщение</param>
        public  virtual void SendMessage(Account account, string message)
        {
            string smsText = $"SMS to: {Phone} subj: Движение по счету {account.Bic} " +
                      $"message: {message}";
            
            SpamBot.MessageQueue.Enqueue(smsText);
        }

    }

}
