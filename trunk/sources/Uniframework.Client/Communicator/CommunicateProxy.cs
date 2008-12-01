using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Reflection;
using System.Xml;

using Uniframework.Services;

namespace Uniframework.Client
{
    public class CommunicateProxy
    {
        private static ICommunicationChannel communicator;
        private static Serializer serializer = new Serializer();
        private static string sessionID = null;
        private static string encryptKey = null;
        private static string username = null;
        private static string password = null;
        private static bool serverReady = false;

        #region Members

        public static string EncryptKey
        {
            get
            {
                return encryptKey;
            }
            set
            {
                encryptKey = value;
            }
        }

        public static string UserName
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
            }
        }

        public static bool ServerReady
        {
            get
            {
                return serverReady;
            }
        }

        public static void SetCredential(string userName, string passWord)
        {
            username = userName;
            password = passWord;
        }

        public static string SessionID
        {
            get
            {
                return sessionID;
            }
            set
            {
                sessionID = value;
            }
        }

        public static ICommunicationChannel Communicator
        {
            get
            {
                return ChannelFactory.GetCommunicationChannel();
            }
        }

        #endregion

        /// <summary>
        /// Ping服务器
        /// </summary>
        /// <returns>如果连得通返回true，否则为false</returns>
        public static bool Ping()
        {
            NetworkInvokePackage package = GetPackage(NetworkInvokeType.Ping);
            try
            {
                byte[] result = Communicator.Invoke(serializer.Serialize<NetworkInvokePackage>(package));
                serverReady = serializer.Deserialize<bool>(result);
            }
            catch { serverReady = false; }
            return serverReady;
        }

        /// <summary>
        /// Invokes the command.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        public static object InvokeCommand(MethodInfo method, object parameter)
        {
            Guard.ArgumentNotNull(method, "method");
            Guard.ArgumentNotNull(parameter, "parameter");

            ClientEventDispatcher.Instance.Log.Debug("提交请求[" + method.Name + "]，SessionID 为 [" + sessionID + "]，用户 [" + username + "] ");
            byte[] buf = serializer.Serialize<MethodInfo>(method);
            byte[] par = serializer.Serialize<object>(parameter);
            byte[] encryptData = SecurityUtility.DESEncrypt(par, encryptKey); // 加密参数值
            NetworkInvokePackage package = GetPackage(NetworkInvokeType.Invoke);
            PackageUtility.EncodeInvoke(package, buf, encryptData);
            byte[] result = Communicator.Invoke(serializer.Serialize<NetworkInvokePackage>(package));
            return serializer.Deserialize<object>(SecurityUtility.DESDecrypt(result, encryptKey)); // 解密返回结果
        }

        /// <summary>
        /// Gets the remote interface catalog.
        /// </summary>
        /// <returns></returns>
        public static List<ServiceInfo> GetRemoteInterfaceCatalog()
        {
            NetworkInvokePackage pk = GetPackage(NetworkInvokeType.RemoteService);
            byte[] buf = Communicator.Invoke(serializer.Serialize<NetworkInvokePackage>(pk));
            return serializer.Deserialize<List<ServiceInfo>>(buf);
        }

        /// <summary>
        /// Registers the session.
        /// </summary>
        public static void RegisterSession()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            string ip = host.AddressList[0].ToString();
            string un = SecurityUtility.DESEncrypt(username, encryptKey);
            string pw = SecurityUtility.DESEncrypt(password, encryptKey);
            NetworkInvokePackage pk = GetPackage(NetworkInvokeType.Register);
            pk.Context.Add("UserName", un);
            pk.Context.Add("Password", pw);
            pk.Context.Add("IPAddress", ip);
            pk.Context.Add("EncryptKey", encryptKey);
            Communicator.Invoke(serializer.Serialize<NetworkInvokePackage>(pk));
        }

        #region Assistant functions

        private static NetworkInvokePackage GetPackage(NetworkInvokeType type)
        { 
#if PocketPC || WindowsCE
            return new NetworkInvokePackage(type, sessionID, ClientType.Mobile);
#else
            return new NetworkInvokePackage(type, sessionID, ClientType.SmartClient);
#endif
        }

        #endregion
    }
}
