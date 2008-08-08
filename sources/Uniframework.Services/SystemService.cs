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
        private IConfigurationService configService;
        private IKernel kernal;
        private Dictionary<string, ServiceInfo> subsystems;
        private Dictionary<MethodInfo, FastInvokeHandler> invokers;

        private readonly string WORKSTATION_PATH = "/System/Workstations/"; // �ͻ�������·��

        /// <summary>
        /// ϵͳ��������
        /// </summary>
        /// <param name="kernal"></param>
        /// <param name="log">��־����</param>
        /// <param name="sessionService">�Ự����</param>
        /// <param name="registerService">ע�����</param>
        public SystemService(IKernel kernal, ILoggerFactory log, ISessionService sessionService, IConfigurationService configService)
        {
            this.logger = log.CreateLogger<SystemService>("Framework");
            this.subsystems = new Dictionary<string, ServiceInfo>();
            this.invokers = new Dictionary<MethodInfo, FastInvokeHandler>();
            this.sessionService = sessionService;
            this.configService = configService;
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
            invokers.Add(methodInfo, GetMethodInvoker(methodInfo));
        }

        #region FastInvokeHandler
        /// <summary>
        /// ���ٵķ��䷽��������
        /// </summary>
        /// <param name="methodInfo">����������Ϣ</param>
        /// <returns>���ɵĵ�����</returns>
        private FastInvokeHandler GetMethodInvoker(MethodInfo methodInfo)
        {
            DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object), typeof(object[]) }, methodInfo.DeclaringType.Module);
            ILGenerator il = dynamicMethod.GetILGenerator();
            ParameterInfo[] ps = methodInfo.GetParameters();
            Type[] paramTypes = new Type[ps.Length];
            for (int i = 0; i < paramTypes.Length; i++)
            {
                paramTypes[i] = ps[i].ParameterType;
            }
            LocalBuilder[] locals = new LocalBuilder[paramTypes.Length];
            for (int i = 0; i < paramTypes.Length; i++)
            {
                locals[i] = il.DeclareLocal(paramTypes[i]);
            }
            for (int i = 0; i < paramTypes.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_1);
                EmitFastInt(il, i);
                il.Emit(OpCodes.Ldelem_Ref);
                EmitCastToReference(il, paramTypes[i]);
                il.Emit(OpCodes.Stloc, locals[i]);
            }
            il.Emit(OpCodes.Ldarg_0);
            for (int i = 0; i < paramTypes.Length; i++)
            {
                il.Emit(OpCodes.Ldloc, locals[i]);
            }
            il.EmitCall(OpCodes.Call, methodInfo, null);
            if (methodInfo.ReturnType == typeof(void))
                il.Emit(OpCodes.Ldnull);
            else
                EmitBoxIfNeeded(il, methodInfo.ReturnType);
            il.Emit(OpCodes.Ret);
            FastInvokeHandler invoker = (FastInvokeHandler)dynamicMethod.CreateDelegate(typeof(FastInvokeHandler));
            return invoker;
        }

        private void EmitCastToReference(ILGenerator il, System.Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
        }

        private void EmitBoxIfNeeded(ILGenerator il, System.Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Box, type);
            }
        }

        private void EmitFastInt(ILGenerator il, int value)
        {
            switch (value)
            {
                case -1:
                    il.Emit(OpCodes.Ldc_I4_M1);
                    return;
                case 0:
                    il.Emit(OpCodes.Ldc_I4_0);
                    return;
                case 1:
                    il.Emit(OpCodes.Ldc_I4_1);
                    return;
                case 2:
                    il.Emit(OpCodes.Ldc_I4_2);
                    return;
                case 3:
                    il.Emit(OpCodes.Ldc_I4_3);
                    return;
                case 4:
                    il.Emit(OpCodes.Ldc_I4_4);
                    return;
                case 5:
                    il.Emit(OpCodes.Ldc_I4_5);
                    return;
                case 6:
                    il.Emit(OpCodes.Ldc_I4_6);
                    return;
                case 7:
                    il.Emit(OpCodes.Ldc_I4_7);
                    return;
                case 8:
                    il.Emit(OpCodes.Ldc_I4_8);
                    return;
            }

            if (value > -129 && value < 128)
            {
                il.Emit(OpCodes.Ldc_I4_S, (SByte)value);
            }
            else
            {
                il.Emit(OpCodes.Ldc_I4, value);
            }
        }

        #endregion

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
                    if (method.IsDefined(typeof(RemoteMethodAttribute), true))
                    {
                        string dataUpdateEvent = null;
                        if (method.IsDefined(typeof(ClientCacheAttribute), true))
                        {
                            if (method.ReturnType == typeof(void))
                            {
                                logger.Warn("���� [" + method.Name + "] ������ [ClientCacheAttribute] ���ԣ�����ȴû�з���ֵ���ѱ����ԡ�");
                            }
                            else
                            {
                                ClientCacheAttribute cacheAttribute = method.GetCustomAttributes(typeof(ClientCacheAttribute), true)[0] as ClientCacheAttribute;
                                dataUpdateEvent = cacheAttribute.DataUpdateEvent;
                            }
                        }
                        // ע�᷽��
                        RemoteMethodAttribute functionAttribute = method.GetCustomAttributes(typeof(RemoteMethodAttribute), true)[0] as RemoteMethodAttribute;
                        RegisterFunction(SecurityUtility.HashObject(method),
                            serviceKey,
                            method.Name,
                            functionAttribute.Description,
                            functionAttribute.Offline,
                            method,
                            dataUpdateEvent);
                    }
                    else if (method.IsDefined(typeof(ClientCacheAttribute), true))
                    {
                        logger.Warn("���� [" + method.Name + "] ������ [ClientCacheAttribute] ���ԣ�����ȴû�ж��� [FunctionInfomationAttribute]���ѱ����ԡ�");
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

            IConfiguration workstations = configService.GetChildren(WORKSTATION_PATH) as IConfiguration;
            foreach (IConfiguration workstation in workstations.Children) { 
                // ���ӵ��ÿ����ϵͳ�Ľ�ɫ
                bool hasPower = false;
                if (workstation.Attributes["AllowRoles"] != null) {
                    string[] allowRoles = workstation.Attributes["AllowRoles"].Split(new char[]{' ', ',', ';'});
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
        public FastInvokeHandler GetInvoker(MethodInfo methodInfo)
        {
            return invokers[methodInfo];
        }

        #endregion
    }
}
