using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace BankSystem.Model
{
    

    public class Department<TCustomer>  : Division where TCustomer : Customer, new()
                                                          
    {
        public ObservableCollection<TCustomer> Customers { get; private set; }

        public Department(string Id, string Name) : base(Id, Name)
        {
            this.Customers = new ObservableCollection<TCustomer>();
        }
            
        // логика, которая зависит от типа клиента...

        public void CreateCustomer(string name, string otherName, string legalId, string phone)
        {
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
      

        public override void RefillAccounts()
        {
            foreach (var account in Accounts)
            {
                ExternalOperation(, 1000);
            }
        }

    }
}

