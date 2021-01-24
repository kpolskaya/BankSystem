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
        public Bank bank { get; private set; } //для сериализации сет

        public Repository()
        {
            if (File.Exists("Bank.json"))
                DeserializeJsonBank();
            else
                this.bank = new Bank("Банк");
        }

        public void SerializeJsonBank()
        {
            //Bank bsObj = bank;
            //DataContractJsonSerializer j = new DataContractJsonSerializer(typeof(Bank));

            //MemoryStream msObj = new MemoryStream();
            //j.WriteObject(msObj, bsObj);
            //msObj.Position = 0;
            //StreamReader sr = new StreamReader(msObj);

            //string json = sr.ReadToEnd();
            //File.WriteAllText("Bank.json", json);
            //sr.Close();
            //msObj.Close();
            var settings = new JsonSerializerSettings 
            { 
              TypeNameHandling = TypeNameHandling.All,
              Formatting = Formatting.Indented
             // PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };
            var text = JsonConvert.SerializeObject(bank, settings);
            // string jsonString = JsonConvert.SerializeObject(bank);
            File.WriteAllText("Bank.json", text);

        }

        public void DeserializeJsonBank()
        {
            string text = File.ReadAllText("Bank.json");
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.TypeNameHandling = TypeNameHandling.All;
            //jss.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            this.bank = Newtonsoft.Json.JsonConvert.DeserializeObject<Bank>(text, jss);

            //using (StreamReader file = File.OpenText("Bank.json"))
            //{
            //    Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            //    JsonSerializerSettings jss = new JsonSerializerSettings();
            //    jss.TypeNameHandling = TypeNameHandling.All;
            //    Bank bank = (Bank)JsonConvert.DeserializeObject(file, typeof(Bank), jss);

            //}
        }
    }
}
