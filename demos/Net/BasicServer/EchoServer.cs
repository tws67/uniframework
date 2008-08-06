using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Uniframework.Net;

namespace BasicServer
{
    public class EchoServer : TcpServerBase<TcpSession>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EchoServer"/> class.
        /// </summary>
        public EchoServer()
        {
            this.Capacity = 5;
        }

        /// <summary>
        /// 接收数据，此处只是简单的将收到的消息又重新发回给客户端
        /// </summary>
        /// <param name="session">会话</param>
        /// <param name="dataBlock">数据</param>
        protected override void OnReceivedData(TcpSession session, DataBlock dataBlock)
        {
            base.OnReceivedData(session, dataBlock);
            Send(session, dataBlock);
            dataBlock.Reset();
        }
    }
}
