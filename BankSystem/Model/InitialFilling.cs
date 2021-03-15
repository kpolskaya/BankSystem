using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BankSystemLib;
using BankSystem;
using BankSystem.ViewModel;
namespace BankSystem.Model
{
    public static class InitialFilling
    {
        public static IEnumerable<T> GetEnumValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static void InitialFillingExtension<TCustomer>(this Department<TCustomer> d, int q, BackgroundWorker worker) where TCustomer : Customer, new()
        {
           
           
                for (int i = 0; i < q; i++)
                {
                    
                    string Name =$"N{d.Id}{i}";
                    string OtherName = $"NN{d.Id}{i}";
                    string LegalId = RandomString(8);
                    Thread.Sleep(10);
                    string Phone = RandomString(12);
                    Thread.Sleep(10);
                    d.CreateCustomer(Name, OtherName, LegalId, Phone);
                       
                    
                    int progressPercentage = Convert.ToInt32(Decimal.Round(i / q) * 30); //проще можно?

                    worker.ReportProgress(progressPercentage);
                }

            for (int i = 0; i < q; i++)
            {
                foreach (var AccountType in GetEnumValues<AccountType>())
                {
                    d.OpenAccount(AccountType, d.Customers[i]);
                    int progressPercentage = Convert.ToInt32(((double)i / q) * 60); //проще можно?

                    worker.ReportProgress(progressPercentage);

                }
            }

            for (int i = 0; i < q*3; i++)
            {
                Random RNG = new Random();
                Thread.Sleep(15);
                d.Put(d.Accounts[i].Bic, RNG.Next(0, 10000)*100);
                
                int progressPercentage = Convert.ToInt32(((double)i / (3*q)) * 100); //проще можно?

                worker.ReportProgress(progressPercentage);

            }

        }

        static string RandomString(int l)
        {
            Random RNG = new Random();
            int length = l;
            var rString = "";
            for (var i = 0; i < length; i++)
            {
                rString += RNG.Next(0, 10).ToString();
            }
            return rString;

        }

    }
}
