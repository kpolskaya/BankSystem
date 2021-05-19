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
        public Bank Bank { get; private set; }

        private bool isBusy;
        /// <summary>
        /// Показывает состояние процессов, связанных с файловыми операциями
        /// или обработкой платежей
        /// </summary>
        public bool IsBusy 
        { 
            get
            {
                return isBusy || SpamBot.OnLine
                    || !SpamBot.MessageQueue.IsEmpty
                    || Processing.IsActive
                    || !Processing.TransactionsQueue.IsEmpty;
            }
        } 

        public string HistoryFolder { get; set; }
        public string HistoryFileName { get; set; }
        public string BankDataPath { get; set; }

        public Repository()
        {
            this.HistoryFileName = @"transactions.json";
            this.HistoryFolder = @"transactions\";
            this.BankDataPath = @"Bank.json";
            this.Bank = new Bank("Банк");
            this.isBusy = false;
        }

        /// <summary>
        /// Сохранение структуры банка в json
        /// </summary>
        public async Task SaveBankAsync()
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented
            };

            isBusy = true;
            var text = JsonConvert.SerializeObject(Bank, settings);
            var fileStream =
                new FileStream(BankDataPath,
                FileMode.Create,
                FileAccess.Write, FileShare.Read,
                bufferSize: 4096, useAsync: true);

            StreamWriter streamWriter = new StreamWriter(fileStream);
                await streamWriter.WriteAsync(text);
            streamWriter.Close();
            fileStream.Dispose();
            isBusy = false;
        }

        /// <summary>
        /// Загрузка стркуктуры банка из json
        /// </summary>
        /// <returns></returns>
        public async Task LoadBankAsync()
        {
            string text;
            var fileStream =
                new FileStream(BankDataPath,
                FileMode.Open,
                FileAccess.Read, FileShare.Read,
                bufferSize: 4096, useAsync: true);
            isBusy = true;
            StreamReader streamReader = new StreamReader(fileStream);
                text = await streamReader.ReadToEndAsync();
            streamReader.Close();
            fileStream.Dispose();
            try
            {
                JsonSerializerSettings jss = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                };
                this.Bank = JsonConvert.DeserializeObject<Bank>(text, jss);
            }
            catch (Exception)
            {
                throw new FileErrorException();
            }
            finally
            {
                isBusy = false;
            }
        }

        /// <summary>
        /// Объединяет транзакции в памяти с историй транзакций в файле и перезаписывает файл истории
        /// </summary>
        /// <returns></returns>
        public async Task UniteTransactionsAsync()
        {
                await this.Bank.UniteTransactionsAsync(HistoryFolder + HistoryFileName);
        }
    }
}
