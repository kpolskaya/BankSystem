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
    public static class SpamBot // сделала паблик чтобы проверить очередь на закрытие
    {
        /// <summary>
        /// Очередь сообщений на отправку
        /// </summary>
        public static ConcurrentQueue<string> MessageQueue { get; set; }

        /// <summary>
        /// Возвращает значение, указывающее, работает ли в данный момент рассылка сообщений
        /// </summary>
        public static bool OnLine { get; private set; }

        static Task spam;

        static string folder;

        static string fileName;

        static SpamBot()
        {
            MessageQueue = new ConcurrentQueue<string>();
            if (!Directory.Exists(@"log"))
                Directory.CreateDirectory(@"log");
            folder = @"log\";
            fileName = $@"{Guid.NewGuid()}.log";
            OnLine = false;
            spam = new Task(Schedule);
            spam.Start();
        }

        private static void Start() 
        {
            //if (!OnLine)
            //{
            //    OnLine = true;
            //    SendThemAsync();
            //}
        }

        private static void Stop() // нужно что-то с этим делать
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
                Thread.Sleep(3000);
                if (!OnLine && !MessageQueue.IsEmpty)
                {
                    OnLine = true;
                    _ = SendThemAsync(); // так можно вместо Task sendThemAll = SendThemAsync(); если сам объект Task никак не будет использоваться
                }
            }
        }

        private static async Task SendThemAsync()
        {
            if (OnLine)
            {
                Debug.WriteLine("SpamBot started");

                int blockSize = 10000;
                int count = 0;
                StringBuilder block = new StringBuilder("", 100000); //существенно быстрее при многократной конкатенации, чем String

                if (File.Exists(folder + fileName) && (new FileInfo(folder + fileName)).Length > 10000000)
                    fileName = $@"{Guid.NewGuid()}.log";

                var logStream = 
                    new FileStream(folder + fileName, 
                    FileMode.Append, 
                    FileAccess.Write, FileShare.Read, 
                    bufferSize: 4096, useAsync: true);
                Debug.WriteLine($"There is {MessageQueue.Count} messages in the queue.");
                Debug.WriteLine("Creating block...");

                while (!MessageQueue.IsEmpty  && count < blockSize)
                {
                    if (MessageQueue.TryDequeue(out string message))
                    {
                        block.Append($"{message}\n");
                        count++;
                    }

                }

                if (count > 0) //есть что писать
                {
                    Debug.WriteLine($"There is {count} messages in the block.");
                    Debug.WriteLine("Sending messages...");
                    using (var logWriter = new StreamWriter(logStream))               
                        await logWriter.WriteAsync(block.ToString());
                }

                logStream.Dispose();
                OnLine = false;
                Debug.WriteLine("SpamBot stopped");
            }
        }
    }
}
