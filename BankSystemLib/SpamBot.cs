using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Diagnostics;

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

        static Task spam;

        static string path;

        static SpamBot()
        {
            MessageQueue = new ConcurrentQueue<string>();
            path = @"messagelog.txt";
            OnLine = false;
            spam = new Task(Schedule);
            spam.Start();
        }

        public static void Start() //заприватить?
        {
            if (!OnLine)
            {
                OnLine = true;
                SendThemAsync();
            }
        }

        public static void Stop() // нужно что-то с этим делать
        {
            //if (OnLine)
            //{
            //    OnLine = false;
            //}
        }

        /// <summary>
        /// Простой планировщик работы бота
        /// </summary>
        private static void Schedule()
        {
            while (true)
            {
                Thread.Sleep(10000);
                if (!OnLine)
                {
                    OnLine = true;
                    SendThemAsync();
                }
            }
        }

        private static async void SendThemAsync()
        {
            if (OnLine)
            {
                Debug.WriteLine("SpamBot started");
                //string message;
                //StreamWriter logWriter = new StreamWriter(path, true);
                int blockSize = 10000;
                int count = 0;
                string block = "";

                while (!MessageQueue.IsEmpty  && count < blockSize)
                {
                    if (MessageQueue.TryDequeue(out string message))
                    {
                        block += (message + "\n");
                        
                    }
                    else
                    {
                        block += "<REQUEST FAILED\n>";
                    }
                    count++;
                }

                if (count > 0) //есть что писать
                {
                    Debug.WriteLine("Sending messages...");
                    using (var logWriter = new StreamWriter(path, true))
                        await logWriter.WriteAsync(block);
                }

                //logWriter.Dispose();
                OnLine = false;
                Debug.WriteLine("SpamBot stopped");
            }
        }
    }
}
