using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BankSystemLib
{
    /// <summary>
    /// Процессинг платежей
    /// </summary>
    public static class Processing
    {
        /// <summary>
        /// Очередь поднятых транзакций
        /// </summary>
        public static ConcurrentQueue<Transaction> Transactions { get; set; }

        public static Action<Transaction> Pay;

        private static Task process;

        private static bool isActive;

        static Processing()
        {
            Transactions = new ConcurrentQueue<Transaction>();
            isActive = false;
            process = new Task (Schedule);
            process.Start();
        }

        private static void Schedule()
        {
            while (true)
            {
                Thread.Sleep(10000);
                if (!isActive)                      
                {
                    isActive = true;
                    DoProcessingAsync();
                }
            }
        }

        private static async void DoProcessingAsync()
        {
            if (isActive)
            {
                Debug.WriteLine("Processing started");
                await Task.Run(() => ProsessQueue());
                isActive = false;
                Debug.WriteLine("Processing stopped");
            }
        }

        private static void ProsessQueue()
        {
            Transaction t;
            while (!Transactions.IsEmpty)
            {
                if (Transactions.TryDequeue(out t)) Pay(t);

                else throw new Exception("Какая-то проблема с очередью транзакций");
            }
        }
    }
}
