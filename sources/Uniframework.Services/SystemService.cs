using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Web.Security;

using Castle.MicroKernel;

namespace Uniframework.Services
{
    /// <summary>
    /// ϵͳ�������
    /// </summary>
    public class SystemService : ISystemService
    {
        private ILogger logger;
        private ISessionService sessionService;
        private IKernel kernal;
        private Dictionary<string, ServiceInfo> subsystems;
        private Dictionary<MethodInfo, DynamicInvoker> invokers;

        private readonly string WORKSTATION_PATH = "System/Workstations/"; // �ͻ�������·��

        /// <summary>
        /// ϵͳ��������
        /// </summary>
        /// <param name="kernal"></param>
        /// <param name="log">��־����</param>
        /// <param name="sessionService">�Ự����</param>
        /// <param name="registerService">ע�����</param>
        public SystemService(IKernel kernal, ILoggerFactory log, ISessionService sessionService)
        {
            this.logger = log.CreateLogger<SystemService>("Framework");
            this.subsystems = new Dictionary<string, ServiceInfo>();
            this.invokers = new Dictionary<MethodInfo, DynamicInvoker>();
            this.sessionService = sessionService;
            this.kernal = kernal;
        }

        #region Assistant function

        /// <summary>
        /// ע�����
        /// </summary>
        /// <param name="serviceKey">�����ʶ</param>
        /// <param name="serviceName">����</param>
        /// <param name="description">������Ϣ</param>
        /// <param name="serviceType">��������</param>
        /// <param name="type">��������</param>
        private void RegisterService(string serviceKey, string serviceName, string description, ServiceType serviceType, Type type, ServiceScope serviceScope)
        {
            logger.Debug("���յ���ϵͳ[" + serviceName + "]��ע������");
            if (subsystems.ContainsKey(serviceKey)) throw new ArgumentException("SystemManager���Ѿ�����KeyΪ[" + serviceKey + "]����ϵͳ��Ϣ", "subsystemKey");
            subsystems.Add(serviceKey, new ServiceInfo(serviceKey, serviceName, description, serviceType, type, serviceScope));
        }

        /// <summary>
        /// ע�᷽��
        /// </summary>
        /// <param name="functionKey">������ʶ</param>
        /// <param name="serviceKey">�����ʶ</param>
        /// <param name="serviceName">����</param>
        /// <param name="description">������Ϣ</param>
        /// <param name="offline">���߱�־</param>
        /// <param name="methodInfo">������Ϣ����</param>
        /// <param name="dataUpdateEvent">���ݸ����¼�ǩ��</param>
        private void RegisterFunction(string functionKey, string serviceKey, string serviceName, string description, bool offline, MethodInfo methodInfo, string dataUpdateEvent)
        {
            if (!subsystems.ContainsKey(serviceKey)) throw new ArgumentException("��ע��[" + serviceName + "]����ʱ����SystemManager�����Ѿ�����KeyΪ[" + serviceKey + "]����ϵͳ��Ϣ", "subsystemKey");
            ServiceInfo subsystem = subsystems[serviceKey];
            subsystem.AddService(new RemoteMethodInfo(functionKey, serviceKey, serviceName, description, offline, methodInfo, dataUpdateEvent));
            invokers.Add(methodInfo, DynamicCaller.GetMethodInvoker(methodInfo));
        }

        #endregion

        #region ISystemService Members

        public void RegisterSession(string sessionID, string userName, string ipAddress, string encryptKey)
        {
            sessionService.Register(sessionID, userName, ipAddress, encryptKey);
        }

        /// <summary>
        /// ���ָ�����͵ķ��񲢸����Զ������Ա�ǩ��ϵͳ�н���ע��
        /// </summary>
        /// <param name="type">��������</param>
        public void InspectService(Type type)
        {
            if (type.IsDefined(typeof(RemoteServiceAttribute), true))
            {
                // ͨ���������Ա�ǩ��ע�����
                RemoteServiceAttribute serviceAttribute = type.GetCustomAttributes(typeof(RemoteServiceAttribute), true)[0] as RemoteServiceAttribute;
                string serviceKey = SecurityUtility.HashObject(type);
                RegisterService(serviceKey, type.Name,
                    serviceAttribute.Description,
                    serviceAttribute.ServiceType,
                    type,
                    serviceAttribute.ServiceScope);
                
                // ������з�����ע��
                MethodInfo[] methods = type.GetMethods();
                foreach (MethodInfo method in methods)
                {
                    MethodInfo mi = method;
                    if (method.IsGenericMethod)
                        mi = method.MakeGenericMethod(method.GetGenericArguments());

                    if (mi.IsDefined(typeof(RemoteMethodAttribute), true))
                    {
                        string dataUpdateEvent = null;
                        if (mi.IsDefined(typeof(ClientCacheAttribute), true))
                        {
                            if (mi.ReturnType == typeof(void))
                            {
                                logger.Warn("���� [" + mi.Name + "] ������ [ClientCacheAttribute] ���ԣ�����ȴû�з���ֵ���ѱ����ԡ�");
                            }
                            else
                            {
                                ClientCacheAttribute cacheAttribute = mi.GetCustomAttributes(typeof(ClientCacheAttribute), true)[0] as ClientCacheAttribute;
                                dataUpdateEvent = cacheAttribute.DataUpdateEvent;
                            }
                        }
                        // ע�᷽��
                        RemoteMethodAttribute functionAttribute = mi.GetCustomAttributes(typeof(RemoteMethodAttribute), true)[0] as RemoteMethodAttribute;
                        RegisterFunction(SecurityUtility.HashObject(mi),
                            serviceKey,
                            mi.Name,
                            functionAttribute.Description,
                            functionAttribute.Offline,
                            mi,
                            dataUpdateEvent);
                    }
                    else if (method.IsDefined(typeof(ClientCacheAttribute), true))
                    {
                        logger.Warn("���� [" + method.Name + "] ������ [ClientCacheAttribute] ���ԣ�����ȴû�ж��� [RemoteMethodAttribute]���ѱ����ԡ�");
                    }
                }
            }
        }

        /// <summary>
        /// ע���Ự
        /// </summary>
        /// <param name="sessionID">�Ự��ʶ</param>
        public void UnRegisterSession(string sessionID)
        {
            sessionService.UnloadSession(sessionID);
        }

        /// <summary>
        /// ��ȡ�ͻ������е�ģ��
        /// </summary>
        /// <returns>���ص�ǰ�û����Լ��ص�ģ����ϸ����</returns>
        public ClientModuleInfo[] GetClientModules()
        {
            string user = sessionService.CurrentSession[ServerVariables.CURRENT_USER].ToString();
            List<ClientModuleInfo> list = new List<ClientModuleInfo>();

            IConfigurationService configService = kernal[typeof(IConfigurationService)] as IConfigurationService;
            IConfiguration workstations = new XMLConfiguration(configService.GetItem(WORKSTATION_PATH));
            foreach (IConfiguration workstation in workstations.Children) { 
                // ���ӵ��ÿ����ϵͳ�Ľ�ɫ
                bool hasPower = false;
                if (workstation.Attributes["allowroles"] != null) {
                    string[] allowRoles = workstation.Attributes["allowroles"].Split(new char[]{' ', ',', ';'});
                    if (allowRoles.Length == 1 && allowRoles[0].ToLower() == "all")
                        hasPower = true;
                    else {
                        foreach (string role in allowRoles) {
                            if (Roles.IsUserInRole(user, role))
                                hasPower = true;
                        }
                    }
                    if (!hasPower) continue; // �����ǰ�û�û�����Ȩ��������һ����ϵͳ

                    // ���������ϵͳ�е�ÿ��ģ��
                    foreach (IConfiguration module in workstation.Children) {
                        if (module.Attributes["assemblyfile"] == null)
                            throw new ArgumentException("û�ж���ģ��ĳ�������[assemblyfile]��");
                        string assemblyFile = module.Attributes["assemblyfile"];
                        string updateLocation = module.Attributes["updatelocation"] != null ? module.Attributes["updatelocation"] : String.Empty;
                        ClientModuleInfo client = new ClientModuleInfo(assemblyFile, updateLocation, module.Value == "MainModule");
                        list.Add(client);
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// �������еķ����б�
        /// </summary>
        /// <returns>����ϵͳ���õ�����Զ�̷����б�</returns>
        public List<ServiceInfo> GetAllServices()
        {
            List<ServiceInfo> systems = new List<ServiceInfo>();
            foreach (ServiceInfo system in subsystems.Values)
            {
                systems.Add(system);
            }
            return systems;
        }

        /// <summary>
        /// ���ݿͻ������ͻ�ȡָ���ķ���
        /// </summary>
        /// <param name="sessionID">�Ự��ʶ</param>
        /// <param name="clientType">�ͻ�������</param>
        /// <returns>Զ�̷����б�</returns>
        public List<ServiceInfo> GetServices(string sessionID, ClientType clientType)
        {
            List<ServiceInfo> systems = new List<ServiceInfo>();
            foreach (ServiceInfo system in subsystems.Values)
            {
                switch (clientType)
                { 
                    case ClientType.SmartClient :
                        if (system.ServiceScope == ServiceScope.Global || system.ServiceScope == ServiceScope.SmartClient)
                            systems.Add(system);
                        break;

                    case ClientType.Mobile :
                        if (system.ServiceScope == ServiceScope.Global || system.ServiceScope == ServiceScope.Mobile)
                            systems.Add(system);
                        break;
                }
            }
            return systems;
        }

        /// <summary>
        /// ���ط���ԭ���Ա����
        /// </summary>
        /// <param name="methodInfo">��������</param>
        /// <returns></returns>
        public DynamicInvoker GetInvoker(MethodInfo methodInfo)
        {
            return invokers[methodInfo];
        }

        #endregion
    }
}
