using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

using Uniframework.Net;

namespace MessageBlockClient
{
    class Program
    {
        private static MessageClient client = null;

        static void Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            using (MessageClient client = new MessageClient())
            {
                client.Host = "127.0.0.1";
                client.Port = 8088;
                client.HeartBeatPeriod = 10000; //10 second
                client.Start();
                Program.client = client;

                Console.WriteLine("Press enter to exit...");
                Thread.Sleep(1000);
                Thread t1 = new Thread(SendData1);
                t1.Start();
                Thread t2 = new Thread(SendData2);
                t2.Start();
                Console.ReadLine();
            }
            Console.ReadLine();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
        }

        static void SendData1()
        {
            while (client.IsConnected)
            {
                string data = "hello from thread 1";
                client.Send(new MessageBlock(Encoding.Unicode.GetBytes(data)));
                Thread.Sleep(1000);
            }
        }

        static void SendData2()
        {
            while (client.IsConnected)
            {
                string data = "hello from thread 2";
                client.Send(new MessageBlock(Encoding.Unicode.GetBytes(data)));
                Thread.Sleep(1000);
            }
        }
    }
}
