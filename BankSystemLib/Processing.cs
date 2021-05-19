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
        public static ConcurrentQueue<Transaction> TransactionsQueue { get; set; }

        public static Action<Transaction> Pay;

        private static Task process;

        /// <summary>
        /// Показывает, происходит ли в данный момент обработка платежей
        /// </summary>
        public static bool IsActive { get; private set; }

        static Processing()
        {
            TransactionsQueue = new ConcurrentQueue<Transaction>();
            IsActive = false;
            process = new Task(Schedule);
            process.Start();
        }

        private static void Schedule()
        {
            while (true)
            {
                Thread.Sleep(10000);
                if (!IsActive && !TransactionsQueue.IsEmpty)                      
                {
                    IsActive = true;
                    _ = DoProcessingAsync(); 
                }
            }
        }

        private static async Task DoProcessingAsync()
        {
            if (IsActive)
            {
                Debug.WriteLine("Processing started");
                await Task.Run(() => ProsessQueue());
                IsActive = false;
                Debug.WriteLine("Processing stopped");
            }
        }

        private static void ProsessQueue()
        {
            Transaction t;
            while (!TransactionsQueue.IsEmpty)
            {
                if (TransactionsQueue.TryDequeue(out t)) Pay(t);
            }
        }
    }
}
