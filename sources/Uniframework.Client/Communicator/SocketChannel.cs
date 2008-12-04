using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Uniframework.Client
{
    /// <summary>
    /// SocketÍ¨µÀ
    /// </summary>
    public class SocketChannel : ICommunicationChannel
    {
        private string host;
        private int port;
        private Serializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketChannel"/> class.
        /// </summary>
        public SocketChannel(string host, int port)
        {
            this.host = host;
            this.port = port;
            serializer = new Serializer();
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="SocketChannel"/> is available.
        /// </summary>
        /// <value><c>true</c> if available; otherwise, <c>false</c>.</value>
        public bool Available
        {
            get
            {
                try
                {
                    TcpClient client = new TcpClient();
                    client.Connect(host, port);
                    client.Close();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        #region ICommunicationChannel Members

        /// <summary>
        /// Invokes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public byte[] Invoke(byte[] data)
        {
            TcpClient client = new TcpClient();
            client.Connect(host, port);
            NetworkStream ns = client.GetStream();

            byte[] sendData = ArrayHelper.ArrayMerge<byte>(BitConverter.GetBytes(data.Length), data);
            ns.Write(sendData, 0, sendData.Length);

            byte[] buf = ArrayHelper.ReadAllBytesFromStream(ns);
            if (buf.Length == 0) return null;
            byte[] result = new byte[buf.Length - 1];
            Array.Copy(buf, 1, result, 0, result.Length);
            if (buf[0] == 1)
            {
                Exception exception = serializer.Deserialize<Exception>(result);
                throw exception;
            }
            return result;
        }

        #endregion
    }
}
