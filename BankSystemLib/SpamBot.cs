using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;
using System.Diagnostics;
using System.Timers;

namespace BankSystemLib
{
    
    public delegate void SpamBotErrorEventHandler(object sender, SpamBot.ErrorEventArgs e);

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

        /// <summary>
        /// Указывает на состояние ошибки и приостановку работы
        /// </summary>
        public static bool Error { get; private set; }

        /// <summary>
        /// Событие ошибки при попытке рассылки сообщений
        /// </summary>
        public static event SpamBotErrorEventHandler SendingError;
         
        private static string folder;

        private static string fileName;

        private static System.Timers.Timer timer;

        static SpamBot()
        {
            MessageQueue = new ConcurrentQueue<string>();
            folder = @"log\";
            fileName = $@"{Guid.NewGuid()}.log";
            OnLine = false;
            Error = false;
            timer = new System.Timers.Timer(5000);
            timer.Elapsed +=  Start;
            timer.Start();
        }

        /// <summary>
        /// Перезапуск рассылки сообщений после ошибки.
        /// </summary>
        public static void ReStart()
        {
            if (Error)
            {
                Error = false;
                timer.Start();
            }
        }

        /// <summary>
        /// Асинхронный стартер работы бота 
        /// </summary>
        private static async void Start(Object sender, ElapsedEventArgs e) 
        {
            try
            {
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                if (!Error && !OnLine && !MessageQueue.IsEmpty)
                    await SendThemAsync();
            }
            catch (Exception ex)
            {
                timer.Stop();
                Error = true;
                OnSendingError("Рассылка сообщений остановлена, устраните проблему и перезапустите рассылку. " +
                    "Дополнительная информация: " + ex.Message);
            }
        }

        private static void OnSendingError(string message)
        {
            string m = message;
            SendingError?.Invoke(null, new ErrorEventArgs(m));
        }

        private static async Task SendThemAsync()
        {
            OnLine = true;
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
                    OnLine = false;
                    logStream.Close();
                    throw ex;
                }
                finally
                {
                    logWriter.Close();
                }
            }

            logStream.Close();
            OnLine = false;
            Debug.WriteLine("SpamBot finished");
        }

        /// <summary>
        /// Данные события ошибки при рассылки сообщений
        /// </summary>
        public class ErrorEventArgs : EventArgs
        {
            public string Message { get; }
            public ErrorEventArgs() : base() { }
            public ErrorEventArgs(string Message) : this()
            {
                this.Message = Message;
            }
        }
    }
}
