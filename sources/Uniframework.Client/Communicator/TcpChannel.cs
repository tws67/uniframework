using System;
using System.Collections.Generic;
using System.Text;

using Uniframework.Net;
using System.Web.Services.Protocols;

namespace Uniframework.Client
{
    /// <summary>
    /// 客户端与服务器连接的TCP协议通道
    /// </summary>
    public class TcpChannel : TcpClientBase<TcpSession>, ICommunicationChannel
    {
        private static readonly int DEFAULT_TIMEOUT = 60000; // 默认的超时值为1分钟

        private byte[] result = null;
        private int timeOut = DEFAULT_TIMEOUT;
        private AsyncAssistant assistant = new AsyncAssistant();

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketChannel"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        public TcpChannel(string host, int port)
            : base(host, port)
        {
            Start();
        }

        #region ICommunicationChannel Members

        public bool Available
        {
            get { return IsConnected; }
        }

        /// <summary>
        /// 调用服务器端的方法
        /// </summary>
        /// <param name="data">方法调用的字节流</param>
        /// <returns>服务器返回的字节流</returns>
        public byte[] Invoke(byte[] data)
        {
            try {
                Send(data);
                assistant.WaitAsyncResult(timeOut); // 等待服务器端返回
                return result;
            }
            catch (SoapException ex) {
                throw ExceptionHelper.UnWrapException<Exception>(ex);
            }
            catch (Exception)
            {
                throw new UniframeworkException("服务器响应超时");
            }
        }

        #endregion

        public int TimeOut
        {
            get { return timeOut; }
            set { timeOut = value; }
        }

        /// <summary>
        /// 收到数据
        /// </summary>
        /// <param name="dataBlock">数据块</param>
        protected override void OnReceivedData(DataBlock dataBlock)
        {
            base.OnReceivedData(dataBlock);
            result = dataBlock.ToArray();
            dataBlock.Reset();
            assistant.EndAsyncOperation(); // 完成调用准备返回
        }
    }
}
