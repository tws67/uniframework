using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Uniframework.Net;
using Uniframework.Services;

namespace Uniframework.Communication
{
    /// <summary>
    /// Tcp 服务器，利用tcp/ip协议来传送相关的服务请求以提高系统的性能
    /// </summary>
    public class TcpServer : TcpServerBase<TcpSession>
    {
        private Serializer serializer;
        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpServer"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="capacity">The capacity.</param>
        /// <param name="logger">The logger.</param>
        public TcpServer(int port, int capacity, ILogger logger)
            : base(port, capacity)
        {
            serializer = new Serializer();
            this.logger = logger;
        }

        /// <summary>
        /// 接收数据,逻辑处理需要重载该函数
        /// </summary>
        /// <param name="session">会话</param>
        /// <param name="dataBlock">数据</param>
        protected override void OnReceivedData(TcpSession session, DataBlock dataBlock)
        {
            try {
                base.OnReceivedData(session, dataBlock); // 接收客户端的请求

                byte[] returns = null;
                try {
                    // 调用服务器端的服务处理客户端的请求
                    byte[] result = CommonService.Invoke(dataBlock.ToArray());
                    if (result == null) {
                        return;
                    }
                    else {
                        returns = new byte[result.Length + 1];
                        returns[0] = 0;
                        result.CopyTo(returns, 1);
                        Send(session, returns); // 向客户端回传处理结果
                    }
                    dataBlock.Reset();
                }
                catch (Exception ex)
                {
                    logger.Error("调用服务错误", ex);
                    Exception exp = new Exception("调用服务错误", ex);
                    byte[] expResults = serializer.Serialize<Exception>(exp);
                    returns = new byte[expResults.Length + 1];
                    returns[0] = 1;
                    expResults.CopyTo(returns, 1);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Tcp服务发生错误", ex);
            }
        }
    }
}