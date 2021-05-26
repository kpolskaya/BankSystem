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
        public static ConcurrentQueue<string> MessageQueue { get; set; }

        /// <summary>
        /// Возвращает значение, указывающее, работает ли в данный момент рассылка сообщений
        /// </summary>
        public static bool OnLine { get; private set; }

        public static bool Error { get; private set; }

        //static Task spam;

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
            Error = false;
            //spam = new Task(Schedule);
            //spam.Start();
            SpamBot.Start();
        }

        /// <summary>
        /// Асинхронный стартер планировщика работы бота (для отлова исключений) 
        /// </summary>
        private static async void Start() 
        {
            try
            {
                await Task.Run(Schedule);
            }
            catch (Exception ex)
            {
                throw new Exception("Рассылка невозможна, процесс остановлен, все пропало, бегите из города - " + ex.Message);
            }
        }

        /// <summary>
        /// Простой планировщик работы бота
        /// </summary>
        private static void Schedule() //не могу использовать await в цикле
        {
            while (true)
            {
                Thread.Sleep(10000); 
                if (!OnLine && !MessageQueue.IsEmpty)
                {
                    OnLine = true;
                    try
                    {
                        Task send = SendThemAsync();
                        send.Wait();
                    }
                    catch (AggregateException ex)
                    {
                        throw ex.InnerException;
                    }
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
                StringBuilder block = new StringBuilder("", 100000); 

                if (File.Exists(folder + fileName) && (new FileInfo(folder + fileName)).Length > 10000000)
                    fileName = $@"{Guid.NewGuid()}.log";
                FileStream logStream;
                try
                {
                   logStream =
                   new FileStream(folder + fileName,
                   FileMode.Append,
                   FileAccess.Write, FileShare.Read,
                   bufferSize: 4096, useAsync: true);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Some file error, SpamBot halted");
                    Error = true;
                    OnLine = false;
                    throw ex;
                }

                Debug.WriteLine($"There are {MessageQueue.Count} messages in the queue");
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
                    Debug.WriteLine($"There are {count} messages in the block");
                    Debug.WriteLine("Sending messages...");
                    var logWriter = new StreamWriter(logStream);

                    try
                    {
                        await logWriter.WriteAsync(block.ToString());
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("File write error");
                        Error = true;
                        OnLine = false;
                        throw ex;
                    }
                    logWriter.Close();
                }

                logStream.Close();
                OnLine = false;
                Debug.WriteLine("SpamBot stopped");
            }
        }

    }
}
