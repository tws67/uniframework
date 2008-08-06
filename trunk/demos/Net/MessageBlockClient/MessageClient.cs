using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Uniframework.Net;

namespace MessageBlockClient
{
    public class MessageClient : MessageBlockClient<MessageBlockSession>
    {
        /// <summary>
        /// 数据连接已经建立
        /// </summary>
        protected override void OnBuildDataConnection()
        {
            base.OnBuildDataConnection();

            while (IsConnected) {
                byte[] data = Encoding.Unicode.GetBytes("Message client.");
                Send(data);
                System.Threading.Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Called when [received message block].
        /// </summary>
        /// <param name="mb">The mb.</param>
        protected override void OnReceivedMessageBlock(MessageBlock mb)
        {
            base.OnReceivedMessageBlock(mb);
            NetDebuger.PrintDebugMessage(mb.ToString());
        }

        /// <summary>
        /// 收到数据
        /// </summary>
        /// <param name="dataBlock">数据块</param>
        protected override void OnReceivedData(DataBlock dataBlock)
        {
            base.OnReceivedData(dataBlock);
            NetDebuger.PrintDebugMessage(Encoding.Unicode.GetString(dataBlock.ToArray()));
        }
    }
}
