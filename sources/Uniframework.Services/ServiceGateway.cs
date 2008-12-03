using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Xml;
using Castle.MicroKernel;
using Castle.Windsor;

namespace Uniframework.Services
{
    /// <summary>
    /// ϵͳ�������
    /// </summary>
    public class ServiceGateway
    {
        private Serializer dataProcessor;
        private static ILogger logger;
        private IKernel kernel;
        private ISessionService sessionService;
        private Serializer serializer;

        public event EventHandler<EventArgs<InvokeInfo>> ServiceInvoking;
        public event EventHandler<EventArgs<SessionRegisterInfo>> SessionRegistering;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceGateway"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="kernel">The kernel.</param>
        /// <param name="sessionService">The session service.</param>
        public ServiceGateway(ILoggerFactory loggerFactory, IKernel kernel, ISessionService sessionService)
        {
            this.kernel = kernel;
            dataProcessor = new Serializer();
            logger = loggerFactory.CreateLogger<ServiceGateway>("Framework");
            this.sessionService = sessionService;
            serializer = new Serializer();
        }

        #region Assistant function 

        private string GetCryptKey(string sessionId)
        {
            ISessionService service = kernel[typeof(ISessionService)] as ISessionService;
            return service.GetSession(sessionId)[ServerVariables.ENCRYPT_KEY].ToString();
        }

        private byte[] GetRemoteServices(NetworkInvokePackage pk)
        {
            try
            {
                ISystemService systemManager = kernel[typeof(ISystemService)] as ISystemService;
                List<ServiceInfo> subsystems = systemManager.GetServices(pk.SessionID, pk.ClientType);
                byte[] data = dataProcessor.Serialize<List<ServiceInfo>>(subsystems);
                return data;
            }
            catch (Exception ex)
            {
                logger.Debug("���Զ�̷���ʧ�ܣ�" + ex.Message);
                return serializer.Serialize<List<ServiceInfo>>(new List<ServiceInfo>());
            }
        }

        private void RegisterSession(string sessionId, string user, string password, string ipAddress, string encryptKey)
        {
            string un = SecurityUtility.DESDecrypt(user, encryptKey);
            string pw = SecurityUtility.DESDecrypt(password, encryptKey);
            logger.Info("���յ��û�" + un + "ע��Ự������");
            if (!Membership.ValidateUser(un, pw)) throw new System.Security.SecurityException("�û��������벻��ȷ!");
            try
            {
                ISystemService systemManager = kernel[typeof(ISystemService)] as ISystemService;
                systemManager.RegisterSession(sessionId, un, ipAddress, encryptKey);
            }
            catch (Exception ex)
            {
                logger.Error("ע��Ựʱ��������", ex);
                throw;
            }
        }

        /// <summary>
        /// ���÷������˵ķ���
        /// </summary>
        /// <param name="package">Զ�̵��ð�</param>
        /// <returns>�������ܺ�ķ������</returns>
        private byte[] InvokeCommand(NetworkInvokePackage package)
        {
            try {
                ActiviteSessioin(package.SessionID);
                String encryptKey = GetCryptKey(package.SessionID);
                MethodInfo method;
                MethodInvokeInfo invokeInfo; 
                object[] parameters;
                bool isGenericMethod = (bool)package.Context[PackageUtility.METHOD_ISGENERIC];

                // ������ð�
                if (isGenericMethod) {
                    PackageUtility.DecodeInvoke(package, encryptKey, out invokeInfo, out parameters);
                    ISystemService system = (ISystemService)kernel[typeof(ISystemService)];
                    method = system.GetMethod(invokeInfo);
                }
                else
                    PackageUtility.DecodeInvoke(package, encryptKey, out method, out parameters); 

                // ����
                object result = Invoke(method, parameters);

                // ���ط������˷���ִ�н��
                if (result == null)
                    return null;
                SerializaionDataSet(result);
                byte[] buffer = SecurityUtility.DESEncrypt(dataProcessor.Serialize<object>(result), encryptKey);
                return buffer;
            }
            catch (Exception ex) {
                logger.Error(String.Format("���÷���ʱ��������, SessionId : {0}", package.SessionID), ex);
                throw;
            }
        }

        private object Invoke(MethodInfo method, object[] parameters)
        {
            IServiceCaller serviceCaller = kernel[typeof(IServiceCaller)] as IServiceCaller;
            if (serviceCaller == null) throw new InvalidOperationException("�޷���ȡ [ServiceCaller] ����");
            return serviceCaller.Invoke(method, parameters);
        }

        /// <summary>
        /// ���л����ݼ������ݱ�
        /// </summary>
        /// <param name="obj">The obj.</param>
        private void SerializaionDataSet(object obj)
        {
            if (obj is DataTable)
            {
                DataTable dt = (DataTable)obj;
                dt.RemotingFormat = SerializationFormat.Binary;
            }
            else if (obj is DataSet)
            {
                DataSet ds = (DataSet)obj;
                ds.RemotingFormat = SerializationFormat.Binary;
            }
        }

        #endregion

        /// <summary>
        /// Activites the sessioin.
        /// </summary>
        /// <param name="sessionId">The session ID.</param>
        public void ActiviteSessioin(string sessionId)
        {
            ISessionService service = kernel[typeof(ISessionService)] as ISessionService;
            service.Activate(sessionId);
        }

        /// <summary>
        /// Executes the specified pk.
        /// </summary>
        /// <param name="pk">The pk.</param>
        /// <returns></returns>
        public byte[] Execute(NetworkInvokePackage pk)
        {
            switch (pk.InvokeType)
            { 
                    // ע��Ự����
                case NetworkInvokeType.Register :
                    string UserName = (string)pk.Context[PackageUtility.SESSION_USERNAME];
                    string Password = (string)pk.Context[PackageUtility.SESSION_PASSWORD];
                    string IpAddress = (string)pk.Context[PackageUtility.SESSION_IPADDRESS];
                    string EncryptKey = (string)pk.Context[PackageUtility.SESSION_ENCTRYPTKEY];
                    RegisterSession(pk.SessionID, UserName, Password, IpAddress, EncryptKey); // ע��Ự
                    return null;

                    // ��ȡԶ�̷���
                case NetworkInvokeType.RemoteService :
                    return GetRemoteServices(pk);

                    // ����Զ�̷���
                case NetworkInvokeType.Invoke :
                    return InvokeCommand(pk);

                default :
                    throw new ArgumentException("�޷�ʶ���Զ�̵������ݰ���ʽ��");
            }
        }
    }

    #region Assistant class

    /// <summary>
    /// ����������Ϣ
    /// </summary>
    public class InvokeInfo
    {
        string sessionId;
        MethodInfo method;
        public InvokeInfo(string sessionId, MethodInfo method)
        {
            this.sessionId = sessionId;
            this.method = method;
        }
        public string SessionId { get { return sessionId; } }
        public MethodInfo Method { get { return method; } }
    }

    /// <summary>
    /// �Ựע����Ϣ
    /// </summary>
    public class SessionRegisterInfo
    {
        string sessionId, username, password, ipAddress, encryptKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionRegisterInfo"/> class.
        /// </summary>
        /// <param name="sessionId">The session ID.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="ipAddress">The ip address.</param>
        /// <param name="encryptKey">The encrypt key.</param>
        public SessionRegisterInfo(string sessionId, string user, string password, string ipAddress, string encryptKey)
        {
            this.sessionId = sessionId;
            this.username = user;
            this.password = password;
            this.ipAddress = ipAddress;
            this.encryptKey = encryptKey;
        }

        public string SessionId { get { return sessionId; } }
        public string Username { get { return username; } }
        public string Password { get { return password; } }
        public string IPAddress { get { return ipAddress; } }
        public string EncryptKey { get { return encryptKey; } }
    }
    #endregion
}
