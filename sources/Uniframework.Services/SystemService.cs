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
    /// 系统管理服务
    /// </summary>
    public class SystemService : ISystemService
    {
        private ILogger logger;
        private ISessionService sessionService;
        private IConfigurationService configService;
        private IKernel kernal;
        private Dictionary<string, ServiceInfo> subsystems;
        private Dictionary<MethodInfo, FastInvokeHandler> invokers;

        private readonly string WORKSTATION_PATH = "/System/Workstations/"; // 客户端配置路径

        /// <summary>
        /// 系统服务构造器
        /// </summary>
        /// <param name="kernal"></param>
        /// <param name="log">日志对象</param>
        /// <param name="sessionService">会话服务</param>
        /// <param name="registerService">注册服务</param>
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
            invokers.Add(methodInfo, GetMethodInvoker(methodInfo));
        }

        #region FastInvokeHandler
        /// <summary>
        /// 快速的反射方法调用器
        /// </summary>
        /// <param name="methodInfo">方法描述信息</param>
        /// <returns>生成的调用器</returns>
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
                    if (method.IsDefined(typeof(RemoteMethodAttribute), true))
                    {
                        string dataUpdateEvent = null;
                        if (method.IsDefined(typeof(ClientCacheAttribute), true))
                        {
                            if (method.ReturnType == typeof(void))
                            {
                                logger.Warn("方法 [" + method.Name + "] 定义了 [ClientCacheAttribute] 特性，但是却没有返回值，已被忽略。");
                            }
                            else
                            {
                                ClientCacheAttribute cacheAttribute = method.GetCustomAttributes(typeof(ClientCacheAttribute), true)[0] as ClientCacheAttribute;
                                dataUpdateEvent = cacheAttribute.DataUpdateEvent;
                            }
                        }
                        // 注册方法
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
                        logger.Warn("方法 [" + method.Name + "] 定义了 [ClientCacheAttribute] 特性，但是却没有定义 [FunctionInfomationAttribute]，已被忽略。");
                    }
                }
            }
        }

        /// <summary>
        /// 注销会话
        /// </summary>
        /// <param name="sessionID">会话标识</param>
        public void UnRegisterSession(string sessionID)
        {
            sessionService.UnloadSession(sessionID);
        }

        /// <summary>
        /// 获取客户端所有的模块
        /// </summary>
        /// <returns>返回当前用户可以加载的模块明细数组</returns>
        public ClientModuleInfo[] GetClientModules()
        {
            string user = sessionService.CurrentSession[ServerVariables.CURRENT_USER].ToString();
            List<ClientModuleInfo> list = new List<ClientModuleInfo>();

            IConfiguration workstations = configService.GetChildren(WORKSTATION_PATH) as IConfiguration;
            foreach (IConfiguration workstation in workstations.Children) { 
                // 检查拥有每个子系统的角色
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
                    if (!hasPower) continue; // 如果当前用户没有相关权限则处理下一个子系统

                    // 逐个处理子系统中的每个模块
                    foreach (IConfiguration module in workstation.Children) {
                        if (module.Attributes["assemblyfile"] == null)
                            throw new ArgumentException("没有定义模块的程序集属性[assemblyfile]。");
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
        /// <param name="sessionID">会话标识</param>
        /// <param name="clientType">客户端类型</param>
        /// <returns>远程服务列表</returns>
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
        /// 返回方法原型以便调用
        /// </summary>
        /// <param name="methodInfo">方法参数</param>
        /// <returns></returns>
        public FastInvokeHandler GetInvoker(MethodInfo methodInfo)
        {
            return invokers[methodInfo];
        }

        #endregion
    }
}
