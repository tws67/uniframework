using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace BasicClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());

            using (EchoClient client = new EchoClient()) {
                client.Port = 8088;
                client.Host = "localhost";
                client.Start();

                Console.WriteLine("Please press enter key to exists...");
                Console.ReadLine();
                client.Stop();
            }
        }
    }
}
