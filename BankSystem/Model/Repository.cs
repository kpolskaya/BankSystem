using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using BankSystemLib;
using System.Threading.Tasks;
using System.Text;
using System.Windows.Controls;
using System.Threading;

namespace BankSystem.Model
{
    public class Repository
    {
        public Bank bank { get; private set; }

        public string HistoryFolder { get; private set; }
        public string HistoryFileName { get; private set; }

        public Repository()
        {
            //if (File.Exists("Bank.json"))
            //    DeserializeJsonBank();
            //else
            //    this.bank = new Bank("Банк");

            this.HistoryFileName = @"transactions.json";
            this.HistoryFolder = @"transactions\";

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
            try
            {
                using (StreamWriter streamWriter = new StreamWriter("Bank.json"))

                    //File.WriteAllText("Bank.json", text);
                    streamWriter.Write(text);

                //streamWriter.Close();
            }
            catch (Exception)
            {

                throw new SaveDataErrorException();
            }
        }


        /// <summary>
        /// Восстановление информации из файла .json
        /// </summary>
        //public async void DeserializeJsonBank() //ВОЗМОЖНО ТУТ НАДО НАПИСАТЬ НЕ void А Task???
        //{
            
        //    //Поубирать все окна и прочую Xamlовскую муть и вставить проверку существования файла
        //    //и либо вызывать код загрузки из файла в новом потоке, либо кидать Exception
        //    //события поднимать, наверное, в коде загрузки, но можно и тут?
            
        //    //BankSystem.View.MainWindow.PbOpen();
        //   // View.ProgressBar pbWindow = new View.ProgressBar();
        //    //pbWindow.Show();


        //    //var progress = new Progress<bool>(ReportProgress);

        //    await Task.Factory.StartNew(() => Deserialiation(progress)); //а можно Task.Run ?

        //    //pbWindow.Close();



        //    //bank.Autosave = SerializeJsonBank;
        //}

        public void DeserializeJsonBank()
        {
            //progress.Report(true);
            string text;
            using (StreamReader streamReader = new StreamReader("Bank.json"))
                //text = File.ReadAllText("Bank.json");
                text = streamReader.ReadToEnd();
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.TypeNameHandling = TypeNameHandling.All;
            this.bank = Newtonsoft.Json.JsonConvert.DeserializeObject<Bank>(text, jss);
           // progress.Report(false);
        }

        //public void ReportProgress(bool value) //АП Закомментил 
        //{
        //  // View.ProgressBar.pbStatus.IsIndeterminate = value;

        //}

        public void SaveTransactionsHistory()
        {
            if (!Directory.Exists(HistoryFolder))
                Directory.CreateDirectory(HistoryFolder);
            Task saveHistoryAsync = this.bank.SaveTransactionsAsync(HistoryFolder + HistoryFileName);
        }

    }
}
