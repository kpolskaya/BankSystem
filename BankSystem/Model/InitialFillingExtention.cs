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
        /// <param name="amt">количество генерируемых клиентов</param>
        public static void FillWithRandom<TCustomer>(this Department<TCustomer> dept, int amt) where TCustomer : Customer, new()
        {
            if (dept.Customers.Count > 0)
                throw new Exception("Невозможно автоматически заполнить непустой департамент");
            
            for (int i = 0; i < amt; i++)
            {
                string Name =$"N{dept.Id}{i}";
                string OtherName = $"NN{dept.Id}{i}";

                string LegalId = dept.Id.ToString() + (1000000 + i).ToString();
                string Phone = dept.Id.ToString() + (20000000000 + i*3).ToString();

                dept.CreateCustomer(Name, OtherName, LegalId, Phone);

                foreach (var accountType in GetEnumValues<AccountType>())
                {
                    dept.OpenAccount(accountType, dept.Customers[i]);
                    dept.Put(dept.Accounts.Last().Bic, rnd.Next(100, 10001) * 100);
                }
            }

        }

    }
}
