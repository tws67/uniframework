using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Client
{
    /// <summary>
    /// 通道类型枚举类型
    /// </summary>
    public enum CommunicationChannel
    {
        /// <summary>
        /// Web服务
        /// </summary>
        WebService,
        /// <summary>
        /// Socket通道
        /// </summary>
        Socket,
        /// <summary>
        /// Wcf通道
        /// </summary>
        Wcf,
        /// <summary>
        /// 自动创建
        /// </summary>
        Auto
    }

    /// <summary>
    /// 通道工厂
    /// </summary>
    public class ChannelFactory
    {
        static string url;
        static int port;
        static string server;
        static CommunicationChannel communicationChannel;

        #region Assistant function

        private static WebServiceChannel CreateWebServiceChannel()
        {
            WebServiceChannel webservice = new WebServiceChannel();
            webservice.Url = url;
            return webservice;
        }

        private static TcpChannel CreateTcpChannel()
        {
            return new TcpChannel(server, port);
        }

        private static bool WcfChannelCanUse()
        {
            WcfChannel channel = CreateWcfChannel();
            NetworkInvokePackage pk = new NetworkInvokePackage(NetworkInvokeType.Ping, Guid.NewGuid().ToString());
            try {
                Serializer serializer = new Serializer();
                byte[] data = channel.Invoke(serializer.Serialize<NetworkInvokePackage>(pk));
                return serializer.Deserialize<bool>(data);
            }
            catch {
                return false;
            }
        }

        private static WcfChannel CreateWcfChannel()
        {
            return new WcfChannel();
        }

        #endregion

        #region ChannelFactory Members

        public static string Url
        {
            get { return url; }
            set
            {
                url = value;
            }
        }

        public static int Port
        {
            set
            {
                port = value;
            }
        }

        public static string ServerAddress
        {
            set
            {
                server = value;
            }
        }

        public static CommunicationChannel CommunicationChannel
        {
            set
            {
                communicationChannel = value;
            }
        }
        
        #endregion

        /// <summary>
        /// Gets the communication channel.
        /// </summary>
        /// <returns></returns>
        public static ICommunicationChannel GetCommunicationChannel()
        {
            switch (communicationChannel) {
                    // 创建WebService通道
                case CommunicationChannel.WebService :
                    return CreateWebServiceChannel();

                    // 创建Tcp通道
                case CommunicationChannel.Socket :
                    return CreateTcpChannel();

                    // 创建Wcf通道
                case CommunicationChannel.Wcf :
                    return CreateWcfChannel();

                default:
                    if (WcfChannelCanUse())
                        return CreateWcfChannel();
                    else
                        return CreateWebServiceChannel();
            }
        }
    }
}
