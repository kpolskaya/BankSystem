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
    /// <summary>
    /// Предоставляет расширение Departnent для автоматического заполнения случайными клиентами
    /// </summary>
    public static class InitialFillingExtention
    {
        private static  Random rnd;

        static InitialFillingExtention()
        {
           rnd = new Random();
        }

        private static IEnumerable<T> GetEnumValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Заполняет указанный департамент последовательным набором клиентов соответствующего типа в указанном количестве
        /// </summary>
        /// <typeparam name="TCustomer"></typeparam>
        /// <param name="dept">Департамент</param>
        /// <param name="amt">количество создаваемых клиентов</param>
        public static void FillWithRandom<TCustomer>(this Department<TCustomer> dept, int amt) where TCustomer : Customer, new()
        {
           
            for (int i = 0; i < amt; i++)
            {
                string Name =$"N{dept.Id}{i}";
                string OtherName = $"NN{dept.Id}{i}";

                string LegalId = dept.Id.ToString() + (1000000 + i).ToString();
                string Phone = dept.Id.ToString() + (20000000000 + i*3).ToString();

                //string LegalId = RandomNumString(8);
                //Thread.Sleep(10);
                //string Phone = RandomNumString(12);
                //Thread.Sleep(10);
                dept.CreateCustomer(Name, OtherName, LegalId, Phone);
            }

            for (int i = 0; i < amt; i++)
            {
                foreach (var accountType in GetEnumValues<AccountType>())
                {
                    dept.OpenAccount(accountType, dept.Customers[i]);
                }
            }

            for (int i = 0; i < amt*3; i++)
            {
                dept.Put(dept.Accounts[i].Bic, rnd.Next(100, 10001)*100);
            }
        }

        private static string RandomNumString(int l)
        {
            int length = l;
            var rString = "";
            for (var i = 0; i < length; i++)
            {
                rString += rnd.Next(0, 10).ToString();
            }
            return rString;
        }

    }
}
