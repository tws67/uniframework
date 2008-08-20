using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Client
{
    public enum CommunicationChannel
    {
        WebService,
        Socket,
        Auto
    }

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

        private static SocketChannel CreateSocketChannel()
        {
            SocketChannel socket = new SocketChannel(server, port);
            return socket;
        }

        private static bool CanSocketUse()
        {
            SocketChannel socket = CreateSocketChannel();
            return socket.Available;
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

        public static ICommunicationChannel GetCommunicationChannel()
        {
            switch (communicationChannel)
            {
                case CommunicationChannel.WebService:
                    return CreateWebServiceChannel();
                case CommunicationChannel.Socket:
                    return CreateSocketChannel();
                default:
                    if (CanSocketUse())
                        return CreateSocketChannel();
                    else
                        return CreateWebServiceChannel();
            }
        }
    }
}
