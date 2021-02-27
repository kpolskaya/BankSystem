using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using BankSystemLib;

namespace BankSystem.Model
{
    /// <summary>
    /// Программа лояльности. Выбирает клиентов с максимальным остатком на счетах в каждом департаменте и вручает подарок.
    /// </summary>
    public static class LoyaltyProgram
    {
        /// <summary>
        ///  Содержит информацию о сумме средств на всех счетах данного клиента. 
        ///  Реализует интерфейс IComparable для сортировки клиентов по сумме средств на счетах.
        /// </summary>
        internal class CustomersFunds : IComparable<CustomersFunds>
        {
           internal decimal TotalFunds { get;  private set; }

            internal Customer Customer { get;  }

            internal CustomersFunds (Customer customer, decimal TotalFunds)
            {
                this.Customer = customer;
                this.TotalFunds = TotalFunds;
            }


            public int CompareTo(CustomersFunds obj)
            {
                if (obj == null)
                    return 1;
                if (this.TotalFunds > obj.TotalFunds)
                    return -1; 
                if (this.TotalFunds < obj.TotalFunds)
                    return 1;
                else
                    return 0;
            }
        }

        internal static void SendInvitation(string phone)
        {
            System.Media.SystemSounds.Asterisk.Play();
            Debug.WriteLine($"SMS to: {phone} subj: Поздравляем! Вы являетесь одним из наших лучших клиентов! Ваш ценный подарок ждет Вас в головном офисе банка.");
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
                        t += (decimal)acc;
                }
                CustomersFunds customerFunds = new CustomersFunds(customer, t);
                CustomersFundsList.Add(customerFunds);
            }

            CustomersFundsList.Sort();
            if (CustomersFundsList.Count > 0 && CustomersFundsList[0].TotalFunds > 0)
            {
                SendInvitation(CustomersFundsList[0].Customer.Phone);
            }
        }
           
    }
}
