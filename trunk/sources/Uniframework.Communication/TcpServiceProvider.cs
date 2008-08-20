using System;
using System.IO;

using Uniframework.Services;

namespace Uniframework.Communication
{
    /// <summary>
    /// Tcp服务提供者
    /// </summary>
    public class TcpServiceProvider : ICloneable
    {
        static Serializer serializer = new Serializer();
        static ILogger logger;

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new TcpServiceProvider();
        }

        /// <summary>
        /// Called when [accept connection].
        /// </summary>
        /// <param name="state">The state.</param>
        public void OnAcceptConnection(ConnectionState state)
        {
            if (logger == null) logger = DefaultHttpApplication.LoggerFactory.CreateLogger<TcpServiceProvider>("Framework");
        }

        #region Assistant functions

        /// <summary>
        /// Recieves the data from client.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private byte[] RecieveDataFromClient(ConnectionState state)
        {
            byte[] lengthData = new byte[4];
            if (state.Read(lengthData, 0, 4) < 4)
            {
                throw new InvalidDataException("数据包的长度信息不正确");
            }
            int length = BitConverter.ToInt32(lengthData, 0);
            MemoryStream ms = new MemoryStream();
            byte[] buffer = new byte[1024];
            int counter = 0;
            while (counter < length)
            {
                int bytesRead = state.Read(buffer, 0, 1024);
                ms.Write(buffer, 0, bytesRead);
                counter += bytesRead;
            }
            return ms.ToArray();
        }

        #endregion

        /// <summary>
        /// Called when [receive data].
        /// </summary>
        /// <param name="state">The state.</param>
        public void OnReceiveData(ConnectionState state)
        {
            try
            {
                byte[] data = RecieveDataFromClient(state);
                byte[] returns = null;
                try
                {
                    byte[] result = CommonService.Invoke(data);
                    if (result == null)
                    {
                        state.EndConnection();
                        return;
                    }
                    else
                    {
                        returns = new byte[result.Length + 1];
                        returns[0] = 0;
                        result.CopyTo(returns, 1);
                    }
                }
                catch (Exception e)
                {
                    logger.Error("服务调用错误", e);
                    Exception ex = new Exception("服务调用错误", e);
                    byte[] exceptionReturns = serializer.Serialize<Exception>(ex);
                    returns = new byte[exceptionReturns.Length + 1];
                    returns[0] = 1;
                    exceptionReturns.CopyTo(returns, 1);
                }
                state.Write(returns, 0, returns.Length);
                state.EndConnection();
            }
            catch (Exception ex)
            {
                logger.Error("TCP发生错误", ex);
            }
        }

        /// <summary>
        /// Called when [drop connection].
        /// </summary>
        /// <param name="state">The state.</param>
        public void OnDropConnection(ConnectionState state)
        {
        }
    }
}
