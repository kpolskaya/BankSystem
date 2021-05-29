using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections.Concurrent;
using System.IO;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace BankSystemLib
{
    /// <summary>
    /// Делегат для события изменения банковского баланса
    /// </summary>
    /// <param name="name">Какой счет банка изменился</param>
    public delegate void BankBalanceChanged(string name);

    /// <summary>
    /// Головной офис банка
    /// </summary>
    [DataContract]
    public class Bank
    {
        [DataMember]
        public decimal Profit { get; private set; } //  bic 99 - прибыль/убыток
        [DataMember]
        public decimal Cash { get; private set; } // bic 00 - касса банка
        [DataMember]
        public ObservableCollection<Division> Departments { get; private set; }
        [DataMember]
        public string Name { get; private set; }

        [JsonIgnore]
        public bool IsBusy { get; private set; }
       
        private ConcurrentBag<Transaction> transactionHistory;

        /// <summary>
        /// Сортированный по времени журнал транзакций (есть заметная задержка при обращении)
        /// </summary>
        [JsonIgnore]
        public SortedSet<Transaction> TransactionHistoryByOrder => new SortedSet<Transaction>(transactionHistory, new ByOrder());

        /// <summary>
        /// Несортированный журнал транзакций
        /// </summary>
        [JsonIgnore]
        public List<Transaction> TransactionHistory => transactionHistory.ToList();

        [JsonConstructor]
        public Bank(string Name, ObservableCollection<Division> Departments, decimal Cash, decimal Profit)
        {
            this.Name = Name;
            this.Departments = Departments;
            this.transactionHistory = new ConcurrentBag<Transaction>();
            this.Cash = Cash;
            this.Profit = Profit;
            IsBusy = false;
            Processing.Pay = ProcessPayment; // Даем процессинговому ценрту метод обработки платежей
            RefreshSubscriptions();
        }

        public Bank(string Name)
        {
            this.Name = Name;
            this.transactionHistory = new ConcurrentBag<Transaction>();
            this.Departments = new ObservableCollection<Division>
            {
                new Department<Entity>("01", "Отдел по работе с юридическими лицами"),
                new Department<Person>("02", "Отдел по работе с физическими лицами"),
                new Department<Vip>("03", "Отдел по работе с VIP клиентами")
            };

            IsBusy = false;
            this.Cash = 1_000_000; //собственный капитал при открытии
            Processing.Pay = ProcessPayment;
        }

        /// <summary>
        /// публичное событие изменения статей банковского баланса
        /// </summary>
        public event BankBalanceChanged BankBalanceChanged;
 
        /// <summary>
        /// Обрабатывает транзакцию, инициированную департаментом
        /// </summary>
        /// <param name="sender">Департамент - инициатор транзакции</param>
        /// <param name="t">Параметры транзакции</param>
        private void ProcessPayment(Transaction t)

        {
            if (t.Type == TransactionType.Internal)
            {
                if (t.BeneficiaryBic == "99") // прибыль банка
                {
                    Profit += t.Sum;
                    OnBalanceChanged("Profit");
                }

                if (t.BeneficiaryBic == "00") // выдача денег клиенту
                {
                    Cash -= t.Sum;
                    OnBalanceChanged("Cash");
                }

                if (t.SenderBic == "99") // убыток банка
                {
                    Profit -= t.Sum;
                    OnBalanceChanged("Profit");
                }

                if (t.SenderBic == "00") // внесение денег клиентом
                {
                    Cash += t.Sum;
                    OnBalanceChanged("Cash");
                }
                t.Status = TransactionStatus.Done;
            }

            else if (t.Status != TransactionStatus.Failed && t.Type == TransactionType.Transfer)     //если транзакция между счетами и подтверждена департаментом
            {
                Division receiver = GetDepartmentByBic(t.BeneficiaryBic);
                if (receiver != null && receiver.TryCredit(t.BeneficiaryBic, t.Sum, t.Detailes))     // если получилось зачислить деньги получателю
                    t.Status = TransactionStatus.Done;
                else                                                                                 //если не получилось
                {
                    t.Status = TransactionStatus.Failed;
                    try
                    {
                        GetDepartmentByBic(t.SenderBic).Refund(t);                                   // то нужно вернуть деньги отправителю
                    }
                    catch (Exception  ex)
                    {
                        Profit += t.Sum;                                                            // если не смогли вернуть деньги клиенту, зачисляем в прибыль банка
                        OnBalanceChanged("Profit");
                        t.Detailes = " Получатель не найден, вернуть не удалось - " + ex.Message;
                    }
                }
            }
                this.transactionHistory.Add(t);
        }
  
        /// <summary>
        /// Вызов кода при изменении статей баланса
        /// </summary>
        /// <param name="name">Статья баланса</param>
        private void OnBalanceChanged(string name)
        {
            BankBalanceChanged?.Invoke(name);
        }

        /// <summary>
        /// Получает конкретный департамент по номеру счета клиента
        /// </summary>
        /// <param name="bic">Номер счета</param>
        /// <returns>Экземпляр департамента  или null если департамент не найден</returns>
        private Division GetDepartmentByBic(string bic)
        {
            if (bic.Length != 18)
                return null;
            string id = bic.Substring(0, 2);
            return Departments.FirstOrDefault(d => d.Id == id);
        }

        /// <summary>
        /// Начисляет месячные платежи и проценты по счетам клиентов
        /// </summary>
        public void MonthlyCharge() 
        {
          
            for (int i = 0; i < Departments.Count; i++)
            {
                Departments[i].CalculateCharges();
            }
        }

        /// <summary>
        /// Сумма денег на клиентских счетах (обязательства банка перед клиентами)
        /// </summary>
        /// <returns></returns>
        public decimal ClientsFunds()
        {
            decimal total = 0;
            foreach (var item in this.Departments)
            {
                total += item.ClientsFunds();
            }
            return total;
        }

        /// <summary>
        /// Обновляет подписку клиентов на события по счетам после десериализации
        /// </summary>
        private void RefreshSubscriptions()
        {
            foreach (var item in this.Departments)
            {
                item.RefreshSubscriptions();
            }
        }

        private async Task LoadTransactionsAsync(string path)
        {
            FileStream fileStream;
            fileStream =
                new FileStream(path,
                FileMode.Open,
                FileAccess.Read, FileShare.Read,
                bufferSize: 102400, useAsync: true);

            var stream = new StreamReader(fileStream);
            string js;
            Debug.WriteLine("Reading file...");
            try
            {
                js = await stream.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                stream.Close();
                fileStream.Close();
            }

            IEnumerable<Transaction> savedTs;
            SortedSet<Transaction> sortedTs;
            Debug.WriteLine("Parsing JSON string...");
            try
            {
                savedTs = await Task.Run(() => JsonConvert.DeserializeObject<List<Transaction>>(js));
            }
            catch (Exception)
            {
                throw new FileErrorException();
            }
            Debug.WriteLine("Uniting Records...");

            await Task.Run(() =>
            {
                sortedTs = new SortedSet<Transaction>(savedTs, new ByOrder());
                sortedTs.ExceptWith(transactionHistory);
                foreach (var item in sortedTs)
                {
                    transactionHistory.Add(item);
                }
            });
        }

        private async Task SaveTransactionsAsync(string path)
        {
            FileStream fileStream;
 
            fileStream =
                new FileStream(path,
                FileMode.Create,
                FileAccess.Write, FileShare.Read,
                bufferSize: 102400, useAsync: true);

            string js;
            Debug.WriteLine("Creating JSON string...");
          
            js = await Task.Run(() => JsonConvert.SerializeObject(this.transactionHistory));

            Debug.WriteLine("Writing file...");
            var stream = new StreamWriter(fileStream);
            try
            {
                await stream.WriteAsync(js);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                stream.Close();
                fileStream.Close();
            }
        }

        /// <summary>
        /// Объединяет транзакции в памяти с историй транзакций в файле и перезаписывает файл истории
        /// </summary>
        /// <param name="path">путь к файлу с историей транзакций</param>
        /// <returns></returns>
        public async Task UniteTransactionsAsync(string path)
        {
            string folder = Path.GetDirectoryName(path);
            IsBusy = true;
            try
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                else if (File.Exists(path))
                {
                    await LoadTransactionsAsync(path);
                }

                await SaveTransactionsAsync(path);
            }
            finally                                               // по идее Exception пробросится автоматом?
            {
                IsBusy = false;
            }
        }
    }
}

