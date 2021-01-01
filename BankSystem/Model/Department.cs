using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace BankSystem.Model
{
    

    public class Department<TCustomer>  : Division where TCustomer : Customer, new()
                                                          
    {
        public new ObservableCollection<TCustomer> Customers { get; private set; }

        //decimal rate;
        //decimal fee;

        public Department(string Id, string Name) : base(Id, Name)
        {
            this.Customers = new ObservableCollection<TCustomer>();
                     
            Type cType = typeof(TCustomer);

            //var rateMethod = cType.GetMethod("Rate", BindingFlags.Static | BindingFlags.Public);
            //this.rate = (decimal)rateMethod.Invoke(null, null);
          
            this.rate = (decimal)cType.GetField("Rate", BindingFlags.Static | BindingFlags.Public).GetValue(null);
            this.fee = (decimal)cType.GetField("Fee", BindingFlags.Static | BindingFlags.Public).GetValue(null);

            Debug.WriteLine("For type {0} Rate is {1}, Fee is {2}", cType.Name, rate, fee); //  вывод для отладки
        }


            
        // логика, которая зависит от типа клиента...

        public override void CreateCustomer(string name, string otherName, string legalId, string phone) // почему у нас нет абстракции этого метода в базовом классе Division? Как потом будем вызывать?
        { //пока это единственный метод, который должен быть явно реализован тут!
            TCustomer customer = new TCustomer
            {
                Name = name,
                OtherName = otherName,
                LegalId = legalId,
                Phone = phone
            };

            this.Customers.Add(customer);
  
        }

        public override void CustomersForExample()
        {
            for (int i = 0; i < 3; i++)
            {
                string name = ($"Клиент  {this.Id}-{i+1}");
                string othername = ($"Имярек  {this.Id}-{i+1}");
                string legalId = ($"{this.Id}-00000-{i+1}");
                string phone = ($"+7 499 {this.Id}0000{i+1}");
                CreateCustomer(name, othername, legalId, phone);
            }

            foreach (var item in Customers) 
            {
                AccountsForExample();
            }
        }
        
        public override void AccountsForExample()
        {
            foreach (var customer in Customers)
            {
                OpenAccount(AccountType.DebitAccount, this.Id, customer.Id);
                OpenAccount(AccountType.DepositAccount, this.Id, customer.Id);
                OpenAccount(AccountType.DepositAccountCapitalized, this.Id, customer.Id);
            }
        }

       


    }
}

