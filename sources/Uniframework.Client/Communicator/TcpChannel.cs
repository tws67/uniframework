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

        private byte[] buffer = null;
        private int timeOut = DEFAULT_TIMEOUT;
        private AsyncAssistant assistant = new AsyncAssistant();
        private Serializer serializer = new Serializer();

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
                if (buffer.Length == 0)
                    return null;

                byte[] result = new byte[buffer.Length - 1];
                Array.Copy(buffer, 1, result, 0, result.Length);
                if (result[0] == 1) {
                    Exception ex = serializer.Deserialize<Exception>(result);
                    throw ex;
                }
                return result;
            }
            catch (SoapException ex) {
                throw ExceptionHelper.UnWrapException<Exception>(ex);
            }
            catch (Exception) {
                throw new UniframeworkException("服务器响应超时");
            }
        }

        /// <summary>
        /// 注册本次会话
        /// </summary>
        /// <param name="data">调用信息</param>
        public void RegisterSession(byte[] data)
        {
            try {
                Send(data);
            }
            catch (Exception ex) {
                throw ex;
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
            buffer = dataBlock.ToArray();
            assistant.EndAsyncOperation(); // 完成调用准备返回
            dataBlock.Reset();
        }
    }
}
