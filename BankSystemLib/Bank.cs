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

        private ConcurrentBag<Transaction> transactionHistory; //потокобезопасная коллекция с историей транзакций

        
        //public ObservableCollection<Transaction> TransactionHistory //TODO не нужен здесь ObservableCollection 
        //{ 
        //    get { return new ObservableCollection<Transaction>((transactionHistory)); } 
        //} // создать приватную concurrent collection и перейти в свойство через  ICollection

        public List<Transaction> TransactionHistory 
        {
            get { return new List<Transaction>(transactionHistory); }  
        }

        [JsonConstructor]
        public Bank(string Name, ObservableCollection<Division> Departments, decimal Cash, decimal Profit)
        {
            this.Name = Name;
            this.Departments = Departments;
            this.transactionHistory = new ConcurrentBag<Transaction>();
            this.Cash = Cash;
            this.Profit = Profit;
            foreach (var item in this.Departments) //подписка на эвенты каждого департамента
            {
                item.TransactionRaised += ProcessPayment;
            }
            RefreshSubscriptions();
        }

        public Bank(string Name)
        {
            this.Name = Name;
            this.Departments = new ObservableCollection<Division>
            {
                new Department<Entity>("01", "Отдел по работе с юридическими лицами"),
                new Department<Person>("02", "Отдел по работе с физическими лицами"),
                new Department<Vip>("03", "Отдел по работе с VIP клиентами")
            };

            this.Cash = 1_000_000; //собственный капитал при открытии

            foreach (var item in this.Departments) //подписка на эвенты каждого департамента
            {
                item.TransactionRaised += ProcessPayment;
            }

            this.transactionHistory = new ConcurrentBag<Transaction>();
        }

        /// <summary>
        /// публичное событие изменения статей банковского баланса
        /// </summary>
        public event BankBalanceChanged BankBalanceChanged;

        /// <summary>
        /// Делегат для запуска автосохранения
        /// </summary>
        public Action Autosave;

        /// <summary>
        /// Обрабатывает транзакцию, инициированную департаментом
        /// </summary>
        /// <param name="sender">Департамент - инициатор транзакции</param>
        /// <param name="t">Параметры транзакции</param>
        private void ProcessPayment(Division sender, Transaction t)
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
                        sender.Refund(t);                                                           // то нужно вернуть деньги отправителю
                    }
                    catch (Exception)
                    {
                        Profit += t.Sum;                                                            // если не смогли вернуть деньги клиенту, зачисляем в прибыль банка
                        OnBalanceChanged("Profit");
                        t.Detailes += " - не удалось вернуть отправителю";
                    }
                }
            }
            this.transactionHistory.Add(t);

            //Autosave?.Invoke();
        }
        //
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
          
            for (int i = 0; i < 3; i++)
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
    }
}

