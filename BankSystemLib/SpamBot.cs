using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace BankSystemLib
{
    /// <summary>
    /// Робот для рассылки сообщений клиентам банка
    /// </summary>
    public static class SpamBot
    {
        /// <summary>
        /// Очередь сообщений на отправку
        /// </summary>
        public static ConcurrentQueue<string> MessageQueue;

        /// <summary>
        /// Возвращает значение, указывающее, работает ли в данный момент рассылка сообщений
        /// </summary>
        public static bool OnLine { get; private set; }

        

        static string path;

        static SpamBot()
        {
            MessageQueue = new ConcurrentQueue<string>();
            path = @"messagelog.txt";
            OnLine = false;
            Task spamAsync = new Task(mySched);
            spamAsync.Start();
           // Start();
        }

        public static void Start()
        {
            if (!OnLine)
            {
                OnLine = true;
                SendThemAsync();
            }
        }

        public static void Stop()
        {
            //if (OnLine)
            //{
            //    OnLine = false;
            //}
        }

        private static void mySched()
        {
            while (true)
            {
                Thread.Sleep(10000);
                Start();
            }
        }

        private static async void SendThemAsync()
        {
            string message;
            StreamWriter logWriter = new StreamWriter(path, true);
            int blockSize = 10000;

            if (OnLine)
            {
                int count = 1;
                string block = "";

                while (!MessageQueue.IsEmpty  && count <= blockSize)
                {
                    if (!MessageQueue.TryDequeue(out message))
                    {
                        message = "<FAILED MESSAGE>";
                    }
                    block += (message + "\n");
                    count++;
                }
                using (logWriter)
                {
                    await logWriter.WriteAsync(block);
                }

                OnLine = false;
            }
        }
    }
}
