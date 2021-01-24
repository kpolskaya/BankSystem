using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;


namespace BankSystem.Model
{
    public delegate void BankBalanceChanged(string name);

    [DataContract]
    
    public class Bank
    {[DataMember]
        public decimal Profit { get; private set; } //  bic 99 - прибыль/убыток
        [DataMember]
        public decimal Cash { get; private set; } // bic 00 - касса банка
        [DataMember]
        public ObservableCollection<Division> Departments { get; private set; }
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public ObservableCollection<Transaction> TransactionHistory { get; private set; }

        public Bank() //убрать или что
        {

        }

        [JsonConstructor]
        public Bank(string Name, ObservableCollection<Division> Departments, ObservableCollection<Transaction> TransactionHistory,
                    decimal Cash, decimal Profit)
        {
            this.Name = Name;
            this.Departments = Departments;
            this.TransactionHistory = TransactionHistory;
            this.Cash = Cash;
            this.Profit = Profit;
            foreach (var item in this.Departments) //подписка на эвенты каждого департамента
            {
                item.TransactionRaised += ProcessPayment;
            }

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

            ExampleCustomers();
            
            this.Cash = 1_000_000; //собственный капитал при открытии
            
            foreach (var item in this.Departments) //подписка на эвенты каждого департамента
            {
                item.TransactionRaised += ProcessPayment;
            }

            this.TransactionHistory = new ObservableCollection<Transaction>();
           
        }
        
        public event BankBalanceChanged BankBalanceChanged;

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

            else if (t.Status != TransactionStatus.Failed && t.Type ==TransactionType.Transfer) //если транзакция между счетами и подтверждена департаментом
            {
                Division receiver = GetDepartmentByBic(t.BeneficiaryBic);
                if (receiver != null && receiver.TryToCredit(t.BeneficiaryBic, t.Sum, t.Detailes)) // если получилось зачислить деньги получателю
                    t.Status = TransactionStatus.Done;
                else
                {
                    t.Status = TransactionStatus.Failed;
                    sender.Refund(t);                                                   // если не получилось, нужно вернуть деньги отправителю
                }
            }

            this.TransactionHistory.Add(t);
            
            //TODO save history
        }

        private void OnBalanceChanged(string name)
        {
            BankBalanceChanged?.Invoke(name);
        }

        private Division GetDepartmentByBic(string bic)
        {
            if (bic.Length != 18)
                return null;
            string id = bic.Substring(0, 2);
            return Departments.FirstOrDefault(d => d.Id == id);
        }

        public void Save(string path)
        {
            throw new NotImplementedException();
        }

        public void Load(string path)
        {
            throw new NotImplementedException();
        }

        public void MonthlyCharge()
        {
            foreach (var department in Departments)
            {
                department.CalculateCharges();
            }
        }

        public void ExampleCustomers()
        {
            foreach (var department in Departments)
            {
                //var dType = department.GetType();
                //Type[] dTypeParameters = dType.GetGenericArguments();
                                                                            
                department.CustomersForExample();
               
            }
        }
    
        public decimal ClientsFunds()
        {
            decimal total = 0;
            foreach (var item in this.Departments)
            {
                total += item.ClientsFunds();
            }
            return total;
        }
    }
}
