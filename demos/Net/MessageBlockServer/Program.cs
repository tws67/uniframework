using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MessageBlockServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());

            // 启动服务器
            using (MessageServer server = new MessageServer()) {
                server.Port = 8088;
                server.Start();

                Console.WriteLine("Press enter key to exit...");
                Console.ReadLine();
                server.Stop();
            }
        }
    }
}
