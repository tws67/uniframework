using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SyncCallServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());

            using (SyncServer server = new SyncServer()) {
                server.Port = 8088;
                server.Start();

                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
                server.Stop();
            }
        }
    }
}
