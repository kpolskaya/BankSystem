using BankSystem.Model;
using BankSystem.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Model
{
    /// <summary>
    /// Программа лояльности. Выбирает клиентов с максимальным остатком на счетах в каждом департаменте и вручает подарок.
    /// </summary>
    public static class LoyaltyProgram
    {
        public class CustomersFunds : IComparable<CustomersFunds>
        {
           public decimal TotalFunds { get;  private set; }

           public Customer Customer { get;  }

           public CustomersFunds (Customer customer, decimal TotalFunds)
           {
                this.Customer = customer;
                this.TotalFunds = TotalFunds;
            }


            public int CompareTo(CustomersFunds obj)
            {
                if (this.TotalFunds > obj.TotalFunds)
                    return -1; 
                if (this.TotalFunds < obj.TotalFunds)
                    return 1;
                else
                    return 0;
            }
        }


        public static void LoyaltyProgramExtension<TCustomer>(this Department<TCustomer> d) where TCustomer : Customer, new()
        {
            List<CustomersFunds> CustomersFundsList = new List<CustomersFunds>();

            foreach (var customer in d.Customers)
            {
                decimal t = 0;
                foreach (var acc in d.Accounts)
                {
                    if (customer.Id == acc.Bic.Substring(2, 8))
                        t += acc.FullBalance();
                }
                CustomersFunds customerFunds = new CustomersFunds(customer, t);
                CustomersFundsList.Add(customerFunds);
            }

            CustomersFundsList.Sort();
            if (CustomersFundsList.Count > 0 && CustomersFundsList[0].TotalFunds > 0)
            {
            string Phone = CustomersFundsList[0].Customer.Phone;
            System.Media.SystemSounds.Asterisk.Play();
            Debug.WriteLine($"SMS to: {Phone} subj: Поздравляем! Вы являетесь одним из наших лучших клиентов! Ваш ценный подарок ждет Вас в головном офисе банка.");
            }
            //return CustomersFundsList;
            
        }

           
    }
}
