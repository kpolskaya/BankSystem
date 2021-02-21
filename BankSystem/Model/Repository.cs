using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BankSystem.Model
{
    public class Repository
    {
        public Bank bank { get; private set; } 

        public Repository()
        {
            if (File.Exists("Bank.json"))
                DeserializeJsonBank();
            else
                this.bank = new Bank("Банк");

            bank.Autosave = SerializeJsonBank;  //код, который нужно вызывать классу Bank для автосохранения
        }

        /// <summary>
        /// Сохранение информации в json
        /// </summary>
        public void SerializeJsonBank()
        {
            var settings = new JsonSerializerSettings 
            { 
                  TypeNameHandling = TypeNameHandling.All,
                  Formatting = Formatting.Indented
            };

            var text = JsonConvert.SerializeObject(bank, settings);
            File.WriteAllText("Bank.json", text);
        }

        /// <summary>
        /// Восстановление информации из файла .json
        /// </summary>
        public void DeserializeJsonBank()
        {
            string text;
            text = File.ReadAllText("Bank.json");
            
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.TypeNameHandling = TypeNameHandling.All;
            this.bank = Newtonsoft.Json.JsonConvert.DeserializeObject<Bank>(text, jss);
            
            bank.Autosave = SerializeJsonBank;
        }
    }
}
