using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BasicServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());

            // 加载服务器
            using (EchoServer server = new EchoServer())
            {
                server.Port = 8088;
                server.Start();

                Console.WriteLine("Press enter key to exit...");
                Console.ReadLine();
                server.Stop();
            }
        }
    }
}