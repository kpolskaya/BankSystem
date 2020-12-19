﻿using System;
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
            this.Departments.Add (new Department<Entity>());
            this.Departments.Add(new Department<Person>());
            this.Departments.Add(new Department<Vip>());
            //this.Departments.Add(new Department<BankBalance>());
            this.BankBalance = 0;
            
            foreach (var item in this.Departments) //подписка на эвенты каждого департамента
            {
                item.TransactionRaised += ProcessPayment;
            }

        }

        private void ProcessPayment(Department<Customer> sender, Transaction t)
        {
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

        }
    }
}
