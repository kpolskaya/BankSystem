using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Model
{
    public class Bank
    {
        public decimal Profit { get; private set; } //  bic 99 - прибыль/убыток
        public decimal Cash { get; private set; } // bic 00 - касса банка
        public ObservableCollection<Division> Departments { get; private set; }
        public string Name { get; }
        public ObservableCollection<Transaction> TransactionHistory { get; private set; }

        public Bank() //убрать или что
        {

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

            this.TransactionHistory = new ObservableCollection<Transaction>();
        }

        private void ProcessPayment(Division sender, Transaction t)
        {
            if (t.Type == TransactionType.Internal)
            {
                if (t.BeneficiaryBic == "99") // прибыль банка
                {
                    Profit += t.Sum;
                }

                if (t.BeneficiaryBic == "00") // выдача денег клиенту
                {
                    Cash -= t.Sum;
                }

                if (t.SenderBic == "99") // убыток банка
                {
                    Profit -= t.Sum;
                }

                if (t.SenderBic == "00") // внесение денег клиентом
                {
                    Cash += t.Sum;
                }
                t.Status = TransactionStatus.Done;
            }

            else if (t.Status != TransactionStatus.Failed) //если транзакция между счетами и подтверждена департаментом
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

        private Division GetDepartmentByBic(string bic)
        {
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

       
    }
}
