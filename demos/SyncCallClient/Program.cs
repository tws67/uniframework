using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SyncCallClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());

            using (SyncClient client = new SyncClient()) {
                client.Port = 8088;
                client.Host = "127.0.0.1";
                client.Start();

                for (int i = 0; i < 10; i++) {

                    Console.Write(i + " + " + 5 + " = ");
                    Console.WriteLine(client.Invoke(i, 5));
                }
                Console.ReadLine();
            }
        }
    }
}
