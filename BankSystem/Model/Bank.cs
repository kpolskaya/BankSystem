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
        public decimal BankBalance { get; private set; }
        public ObservableCollection<Division> Departments { get; private set; }
        public string Name { get; }

        public Bank()
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
            
            this.BankBalance = 0;
            
            foreach (var item in this.Departments) //подписка на эвенты каждого департамента
            {
                item.TransactionRaised += ProcessPayment;
            }

        }

        private void ProcessPayment(Division sender, Transaction t)
        {
            if (t.BeneficiaryBic == "00")
                BankBalance += t.Sum;

            if (t.SenderBic == "00")
                BankBalance -= t.Sum;

            // давай 2 символа на Id Департамента оставим!
            //логика, если получатель - сам банк ( t.SenderBic.Substring(2) = "00" ) ... return;

            //логика, если плательщик - сам банк ... return;

            // логика проверки получателя средств

            //этот код можно выполить только после того, как мы убедимся, что бик получателя валиден!
            if (sender.Debit(t.SenderBic, t.Sum)) 
            {
                Departments.FirstOrDefault(d => d.Id == t.BeneficiaryBic.Substring(2)).Credit(t.BeneficiaryBic, t.Sum);

            }

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
                department.CalculationCharge();
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
