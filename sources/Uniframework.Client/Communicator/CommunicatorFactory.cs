using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Client
{
    /// <summary>
    /// ͨ������ö������
    /// </summary>
    public enum CommunicationChannel
    {
        /// <summary>
        /// Web����
        /// </summary>
        WebService,
        /// <summary>
        /// Socketͨ��
        /// </summary>
        Socket,
        /// <summary>
        /// Wcfͨ��
        /// </summary>
        Wcf,
        /// <summary>
        /// �Զ�����
        /// </summary>
        Auto
    }

    /// <summary>
    /// ͨ������
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
                    // ����WebServiceͨ��
                case CommunicationChannel.WebService :
                    return CreateWebServiceChannel();

                    // ����Tcpͨ��
                case CommunicationChannel.Socket :
                    return CreateTcpChannel();

                    // ����Wcfͨ��
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
