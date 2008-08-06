using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Uniframework.Net;
using System.Threading;

namespace BasicClient
{
    public class EchoClient : TcpClientBase<TcpSession>
    {
        /// <summary>
        /// 数据连接已经建立
        /// </summary>
        protected override void OnBuildDataConnection()
        {
            base.OnBuildDataConnection();

            byte[] data = Encoding.Unicode.GetBytes("Hello");
            // 如果连接没有中断的话就一直向服务器端发送Hello字符
            while (IsConnected) {
                Send(data);
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 收到数据
        /// </summary>
        /// <param name="dataBlock">数据块</param>
        protected override void OnReceivedData(DataBlock dataBlock)
        {
            base.OnReceivedData(dataBlock);
            NetDebuger.PrintDebugMessage(Encoding.Unicode.GetString(dataBlock.ToArray()));
            dataBlock.Reset(); // 重复使用数据块缓冲区
        }

        /// <summary>
        /// Called when [connect server fail].
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnConnectServerFail(Exception e)
        {
            base.OnConnectServerFail(e);
            NetDebuger.PrintDebugMessage(e.Message);
        }

        protected override void OnDropLine()
        {
            base.OnDropLine();
            NetDebuger.PrintDebugMessage("Client is drop line.");
        }
    }
}
