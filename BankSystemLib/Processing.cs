using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Timers;

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

        /// <summary>
        ///  Получение метода обработки транзакций
        /// </summary>
        public static Action<Transaction> Pay;
 
        /// <summary>
        /// Показывает, происходит ли в данный момент обработка платежей
        /// </summary>
        public static bool IsActive { get; private set; }

        private static System.Timers.Timer timer;

        static Processing()
        {
            TransactionsQueue = new ConcurrentQueue<Transaction>();
            IsActive = false;
            timer = new System.Timers.Timer(5000);
            timer.Elapsed += async (sender, e) => await DoProcessingAsync();
            timer.Start();
        }

        private static async Task DoProcessingAsync()
        {
            if (!IsActive && !TransactionsQueue.IsEmpty)
            {
                IsActive = true;
                Debug.WriteLine("Processing started");
                Transaction t;
                await Task.Run(() =>
                {
                    while (!TransactionsQueue.IsEmpty)
                    {
                        if (TransactionsQueue.TryDequeue(out t)) Pay(t);
                    }
                });
                IsActive = false;
                Debug.WriteLine("Processing stopped");
            }
        }
    }
}
