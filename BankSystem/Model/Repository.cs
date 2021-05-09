﻿using Newtonsoft.Json;
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
                    streamWriter.Write(text);
            }
            catch (Exception)
            {

                throw new SaveDataErrorException();
            }
        }

        public void DeserializeJsonBank()
        {
            string text;
            using (StreamReader streamReader = new StreamReader("Bank.json"))
                text = streamReader.ReadToEnd();

            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.TypeNameHandling = TypeNameHandling.All;
            this.bank = Newtonsoft.Json.JsonConvert.DeserializeObject<Bank>(text, jss);
        }

        public void UniteTransactions()
        {
            _ = this.bank.UniteTrahsactions(HistoryFolder + HistoryFileName);
        }


        //public void SaveTransactionsHistory()
        //{
        //    if (!Directory.Exists(HistoryFolder))
        //        Directory.CreateDirectory(HistoryFolder);
        //    Task saveHistoryAsync = this.bank.SaveTransactionsAsync(HistoryFolder + HistoryFileName);
        //}

        //public void LoadTransactionHistory()
        //{
        //    Task loadHistoryAsync;
        //    if (File.Exists(HistoryFolder + HistoryFileName))
        //        loadHistoryAsync = this.bank.LoadTransactionsAsync(HistoryFolder + HistoryFileName);
        //}

    }
}
