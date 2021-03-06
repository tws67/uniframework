using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Web.Security;
using Castle.MicroKernel;
using Uniframework.Security;

namespace Uniframework.Services
{
    /// <summary>
    /// 系统管理服务
    /// </summary>
    public class SystemService : ISystemService
    {
        private ILogger logger;
        private ISessionService sessionService;
        private IKernel kernal;
        private Dictionary<string, ServiceInfo> subsystems;
        private Dictionary<MethodInfo, DynamicInvokerHandler> invokers;
        private Dictionary<MethodInvokeInfo, MethodInfo> methodInvokers; // 缓存客户端发过来的泛型方法调用

        private readonly string WORKSTATION_PATH = "System/Workstations/"; // 客户端配置路径

        /// <summary>
        /// 系统服务构造器
        /// </summary>
        /// <param name="kernal"></param>
        /// <param name="log">日志对象</param>
        /// <param name="sessionService">会话服务</param>
        /// <param name="registerService">注册服务</param>
        public SystemService(IKernel kernal, ILoggerFactory log, ISessionService sessionService)
        {
            this.logger = log.CreateLogger<SystemService>("Framework");
            this.subsystems = new Dictionary<string, ServiceInfo>();
            this.invokers = new Dictionary<MethodInfo, DynamicInvokerHandler>();
            this.methodInvokers = new Dictionary<MethodInvokeInfo, MethodInfo>();
            this.sessionService = sessionService;
            this.kernal = kernal;
        }

        #region ISystemService Members

        public void RegisterSession(string sessionId, string userName, string ipAddress, string encryptKey)
        {
            sessionService.Register(sessionId, userName, ipAddress, encryptKey);
        }

        /// <summary>
        /// 检查指定类型的服务并根据自定义属性标签在系统中进行注册
        /// </summary>
        /// <param name="type">服务类型</param>
        public void InspectService(Type type)
        {
            if (type.IsDefined(typeof(RemoteServiceAttribute), true))
            {
                // 通过服务属性标签来注册服务
                RemoteServiceAttribute serviceAttribute = type.GetCustomAttributes(typeof(RemoteServiceAttribute), true)[0] as RemoteServiceAttribute;
                string serviceKey = SecurityUtility.HashObject(type);
                RegisterService(serviceKey, type.Name,
                    serviceAttribute.Description,
                    serviceAttribute.ServiceType,
                    type,
                    serviceAttribute.ServiceScope);
                
                // 检查所有方法并注册
                MethodInfo[] methods = type.GetMethods();
                foreach (MethodInfo method in methods)
                {
                    MethodInfo mi = method;
                    if (method.IsGenericMethod)
                        mi = method.GetGenericMethodDefinition();

                    if (mi.IsDefined(typeof(RemoteMethodAttribute), true))
                    {
                        string dataUpdateEvent = null;
                        if (mi.IsDefined(typeof(ClientCacheAttribute), true))
                        {
                            if (mi.ReturnType == typeof(void))
                            {
                                logger.Warn("方法 [" + mi.Name + "] 定义了 [ClientCacheAttribute] 特性，但是却没有返回值，已被忽略。");
                            }
                            else
                            {
                                ClientCacheAttribute cacheAttribute = mi.GetCustomAttributes(typeof(ClientCacheAttribute), true)[0] as ClientCacheAttribute;
                                dataUpdateEvent = cacheAttribute.DataUpdateEvent;
                            }
                        }
                        // 注册方法
                        RemoteMethodAttribute rmAttribute = mi.GetCustomAttributes(typeof(RemoteMethodAttribute), true)[0] as RemoteMethodAttribute;
                        RegisterFunction(SecurityUtility.HashObject(mi),
                            serviceKey,
                            mi.Name,
                            rmAttribute.Description,
                            rmAttribute.Offline,
                            mi,
                            dataUpdateEvent);
                    }
                    else if (method.IsDefined(typeof(ClientCacheAttribute), true))
                    {
                        logger.Warn("方法 [" + method.Name + "] 定义了 [ClientCacheAttribute] 特性，但是却没有定义 [RemoteMethodAttribute]，已被忽略。");
                    }
                }
            }
        }

        /// <summary>
        /// 注销会话
        /// </summary>
        /// <param name="sessionId">会话标识</param>
        public void UnRegisterSession(string sessionId)
        {
            sessionService.UnloadSession(sessionId);
        }

        /// <summary>
        /// 获取客户端所有的模块
        /// </summary>
        /// <returns>返回当前用户可以加载的模块明细数组</returns>
        public ClientModuleInfo[] GetClientModules()
        {
            string user = sessionService.CurrentSession[SessionVariables.SESSION_CURRENT_USER].ToString();
            List<ClientModuleInfo> list = new List<ClientModuleInfo>();

            IConfigurationService configService = kernal[typeof(IConfigurationService)] as IConfigurationService;
            IConfiguration workstations = new XMLConfiguration(configService.GetItem(WORKSTATION_PATH));
            foreach (IConfiguration workstation in workstations.Children) { 
                // 检查拥有每个子系统的角色
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
                    if (!hasPower) continue; // 如果当前用户没有相关权限则处理下一个子系统

                    // 逐个处理子系统中的每个模块
                    foreach (IConfiguration module in workstation.Children) {
                        if (module.Attributes["assemblyfile"] == null)
                            throw new ArgumentException("没有定义模块的程序集属性[assemblyfile]。");
                        string assemblyFile = module.Attributes["assemblyfile"];
                        ClientModuleInfo client = new ClientModuleInfo(assemblyFile, module.Value == "MainModule");
                        list.Add(client);
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// 获取方法的动态调用器
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public DynamicInvokerHandler GetDynamicInvoker(MethodInfo method)
        {
            if (invokers.ContainsKey(method))
                return invokers[method];
            else
                try {
                    logger.Debug(String.Format("生成方法 \"{0}\" 的调用器", method));
                    //if (method.IsGenericMethod)
                    //    method = method.GetGenericMethodDefinition();
                    DynamicInvokerHandler invoker = DynamicInvoker.GetMethodInvoker(method);
                    invokers[method] = invoker;
                    return invoker;
                }
                catch (Exception ex) {
                    throw ex;
                }
        }

        /// <summary>
        /// 通过客户端传过来的方法调用信息获取方法的动态调用器
        /// </summary>
        /// <param name="invokeInfo">The invoke info.</param>
        /// <returns></returns>
        public MethodInfo GetMethod(MethodInvokeInfo invokeInfo)
        {
            if (!methodInvokers.ContainsKey(invokeInfo)) {
                MethodInfo method = invokeInfo.GetGenericMethod();
                if (method == null)
                    throw new ArgumentException(String.Format("无此泛型方法 \"{0}\" 定义", invokeInfo.Name));

                methodInvokers.Add(invokeInfo, method);
            }
            return methodInvokers[invokeInfo];
        }

        /// <summary>
        /// 返回所有的服务列表
        /// </summary>
        /// <returns>返回系统可用的所有远程服务列表</returns>
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
        /// 根据客户端类型获取指定的服务
        /// </summary>
        /// <param name="sessionId">会话标识</param>
        /// <param name="clientType">客户端类型</param>
        /// <returns>远程服务列表</returns>
        public List<ServiceInfo> GetServices(string sessionId, ClientType clientType)
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

        #endregion

        #region Assistant function

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="serviceKey">服务标识</param>
        /// <param name="serviceName">名称</param>
        /// <param name="description">描述信息</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="type">反射类型</param>
        private void RegisterService(string serviceKey, string serviceName, string description, ServiceType serviceType, Type type, ServiceScope serviceScope)
        {
            logger.Debug("接收到子系统[" + serviceName + "]的注册请求");
            if (subsystems.ContainsKey(serviceKey)) throw new ArgumentException("SystemManager中已经存在Key为[" + serviceKey + "]的子系统信息", "subsystemKey");
            subsystems.Add(serviceKey, new ServiceInfo(serviceKey, serviceName, description, serviceType, type, serviceScope));
        }

        /// <summary>
        /// 注册方法
        /// </summary>
        /// <param name="functionKey">方法标识</param>
        /// <param name="serviceKey">服务标识</param>
        /// <param name="serviceName">名称</param>
        /// <param name="description">描述信息</param>
        /// <param name="offline">离线标志</param>
        /// <param name="methodInfo">方法信息对象</param>
        /// <param name="dataUpdateEvent">数据更新事件签名</param>
        private void RegisterFunction(string functionKey, string serviceKey, string serviceName, string description, bool offline, MethodInfo methodInfo, string dataUpdateEvent)
        {
            if (!subsystems.ContainsKey(serviceKey)) throw new ArgumentException("在注册[" + serviceName + "]服务时发现SystemManager并不已经存在Key为[" + serviceKey + "]的子系统信息", "subsystemKey");
            ServiceInfo subsystem = subsystems[serviceKey];
            subsystem.AddService(new RemoteMethodInfo(functionKey, serviceKey, serviceName, description, offline, methodInfo, dataUpdateEvent));
            invokers.Add(methodInfo, DynamicInvoker.GetMethodInvoker(methodInfo));
        }

        #endregion

    }
}
