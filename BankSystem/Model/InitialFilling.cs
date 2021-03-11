using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BankSystemLib;
using BankSystem;
namespace BankSystem.Model
{
    public static class InitialFilling
    {
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
                    int progressPercentage = Convert.ToInt32(((double)i / q) * 100); //проще можно?
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
