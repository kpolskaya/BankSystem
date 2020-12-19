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
        public ObservableCollection<IDivision> Departments { get; private set; }
        public string Name { get; }

        public Bank()
        {

        }

        public Bank(string Name)
        {
            this.Name = Name;
            this.Departments.Add (new Department<Entity>());
            this.Departments.Add(new Department<Person>());
            this.Departments.Add(new Department<Vip>());
            //this.Departments.Add(new Department<BankBalance>());
            this.BankBalance = 0;
            foreach (var item in this.Departments)
            {
                item.TransactionRaised += ProcessPayment;
            }

        }

        private void ProcessPayment(Object sender, Transaction t)
        {
           Type type = sender.GetType();
           (sender as type) 

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

        }
    }
}
