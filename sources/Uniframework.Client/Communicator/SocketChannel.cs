using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Uniframework.Net;

namespace Uniframework.Client
{
    /// <summary>
    /// Socket 通道
    /// </summary>
    public class SocketChannel : TcpClientBase<TcpSession>, ICommunicationChannel
    {
        private Serializer serializer = new Serializer();

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketChannel"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        public SocketChannel(string host, int port)
            : base(host, port)
        { }

        #region ICommunicationChannel Members

        /// <summary>
        /// Invokes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public byte[] Invoke(byte[] data)
        {
            return null;
        }

        #endregion
    }
}
