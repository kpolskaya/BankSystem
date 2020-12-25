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
        public ObservableCollection<TCustomer> Customers { get; private set; }

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

        public void CreateCustomer(string name, string otherName, string legalId, string phone) // почему у нас нет абстракции этого метода в базовом классе Division? Как потом будем вызывать?
        { //пока это единственный метод, который должен быть явно реализован тут!
            TCustomer customer = new TCustomer();
            
            customer.Name = name;
            customer.OtherName = otherName;
            customer.LegalId = legalId;
            customer.Phone = phone;
            
            this.Customers.Add(customer);
  
        }

        public override void CustomersForExample()
        {
            for (int i = 0; i < 3; i++)
            {
                string name = ($"Клиент  {this.Id}-{i+1}");
                string othername = ($"Имярек  {this.Id}-{i+1}");
                string legalId = ($"{this.Id}-00000-{i+1}");
                string phone = ($"+7 499 {this.Id}00{i+1}");
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
                OpenAccount(AccountType.DepositAccount, this.Id, customer.Id, 1000);
                OpenAccount(AccountType.DepositAccountCapitalized, this.Id, customer.Id);
            }
        }

        public override void CalculateCharge() // нужно наверное сделать универсальный метод, который в зависимости от типа счета считает fee | interest
                                                // и может лучше его реализовать прямо в классе Division без абстракции?
        {
            foreach (var account in accounts)
            {
                //Type myType = Type.GetType(Customer, false, true);
                //var myType = GetType();
                //myType.GetFields();

                // Console.WriteLine($"{field.FieldType} {field.Name}");
                decimal Interest = account.GetInterest(); // нужно убрать из Account этот метод
                account.Credit(Interest);
                OnTransactionRaised(this, new Transaction("00", account.Bic, Interest));

                decimal Fee = account.Fee; // fee и rate брать прямо из this.rate | this.fee
                if (account.Debit(Fee))
                    OnTransactionRaised(this, new Transaction(account.Bic, "00", Fee));

            }


        }


    }
}

