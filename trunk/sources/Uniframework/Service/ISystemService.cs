using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Uniframework.Services
{
    #region ServiceInfo
    /// <summary>
    /// 系统服务信息类
    /// </summary>
    [Serializable]
    public class ServiceInfo
    {
        private string key;
        private string name;
        private string description;
        private ServiceType systemType;
        private ServiceScope serviceScope = ServiceScope.Global;
        private Type type;
        private List<RemoteMethodInfo> remoteMethods;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public ServiceInfo()
        {
            remoteMethods = new List<RemoteMethodInfo>();
        }

        /// <summary>
        /// 系统服务信息类构造方法
        /// </summary>
        /// <param name="serviceKey">系统标识</param>
        /// <param name="name">系统名称</param>
        /// <param name="description">系统描述</param>
        /// <param name="serviceType">系统类型</param>
        /// <param name="type">类型标识</param>
        public ServiceInfo(string serviceKey, string name, string description, ServiceType serviceType, Type type)
            : this()
        {
            this.key = serviceKey;
            this.name = name;
            this.description = description;
            this.systemType = serviceType;
            this.type = type;
        }

        /// <summary>
        /// 构造函数（重载）
        /// </summary>
        /// <param name="serviceKey"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="serviceType"></param>
        /// <param name="type"></param>
        /// <param name="serviceScope"></param>
        public ServiceInfo(string serviceKey, string name, string description, ServiceType serviceType, Type type, ServiceScope serviceScope)
            : this(serviceKey, name, description, serviceType, type)
        {
            this.serviceScope = serviceScope;
        }

        /// <summary>
        /// 获取系统标识
        /// </summary>
        public string Key { get { return key; } }

        /// <summary>
        /// 获取系统名称
        /// </summary>
        public string Name { get { return name; } }

        /// <summary>
        /// 获取系统描述
        /// </summary>
        public string Description 
        { 
            get { return description; }
        }

        /// <summary>
        /// 获取系统类型
        /// </summary>
        public ServiceType SystemType 
        { 
            get { return systemType; }
        }

        /// <summary>
        /// 服务发布范围
        /// </summary>
        public ServiceScope ServiceScope
        {
            get { return serviceScope; }
        }

        /// <summary>
        /// 获取类型标识
        /// </summary>
        public Type Type 
        { 
            get { return type; }
        }

        /// <summary>
        /// 获取操作列表
        /// </summary>
        public List<RemoteMethodInfo> RemoteMethods { get { return remoteMethods; } }

        /// <summary>
        /// 增加系统操作
        /// </summary>
        /// <param name="service">系统操作</param>
        public void AddService(RemoteMethodInfo service)
        {
            remoteMethods.Add(service);
        }
    }
    #endregion

    #region RemoteMethodInfo

    /// <summary>
    /// 系统操作
    /// </summary>
    [Serializable]
    public class RemoteMethodInfo
    {
        private string key;
        private string serviceKey;
        private string name;
        private string description;
        private bool offline;
        private MethodInfo methodInfo;
        private string dataUpdateEvent;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public RemoteMethodInfo()
        { }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="functionKey">操作标识</param>
        /// <param name="serviceKey">子系统标识</param>
        /// <param name="name">操作名称</param>
        /// <param name="description">操作描述</param>
        /// <param name="offline">是否可离线操作</param>
        /// <param name="methodInfo">MethodInfo实例</param>
        /// <param name="dataUpdateEvent">数据更新事件名</param>
        public RemoteMethodInfo(string functionKey, string serviceKey, string name, string description, bool offline, MethodInfo methodInfo, string dataUpdateEvent)
            : this()
        {
            this.key = functionKey;
            this.serviceKey = serviceKey;
            this.name = name;
            this.description = description;
            this.offline = offline;
            this.methodInfo = methodInfo;
            this.dataUpdateEvent = dataUpdateEvent;
        }

        #region RemoteMethodInfo Members

        /// <summary>
        /// 操作标识
        /// </summary>
        public string Key { get { return key; } }

        /// <summary>
        /// 子系统标识
        /// </summary>
        public string SubSystemKey { get { return serviceKey; } }

        /// <summary>
        /// 操作名称
        /// </summary>
        public string Name { get { return name; } }

        /// <summary>
        /// 操作描述
        /// </summary>
        public string Description { get { return description; } }

        /// <summary>
        /// 是否可离线操作
        /// </summary>
        public bool Offline { get { return offline; } }

        /// <summary>
        /// MethodInfo实例
        /// </summary>
        public MethodInfo MethodInfo { get { return methodInfo; } }

        /// <summary>
        /// 数据更新事件名
        /// </summary>
        public string DataUpdateEvent { get { return dataUpdateEvent; } }
        
        #endregion
    }

    #endregion

    #region ClientModuleInfo
    /// <summary>
    /// 客户端模块信息
    /// </summary>
    [Serializable]
    public class ClientModuleInfo
    {
        private bool isMainModule = false;
        private string assemblyFile = string.Empty;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public ClientModuleInfo()
        { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="allowRoles">允许操作的角色数组</param>
        /// <param name="assemblyFile">程序集文件名</param>
        /// <param name="isMainModule">是否主要模块</param>
        public ClientModuleInfo(string assemblyFile, bool isMainModule)
            : this()
        {
            this.assemblyFile = assemblyFile;
            this.isMainModule = isMainModule;
        }

        /// <summary>
        /// 获取装配件文件
        /// </summary>
        public string AssemblyFile { get { return assemblyFile; } set { assemblyFile = value; } }

        /// <summary>
        /// 获取是否主要模块
        /// </summary>
        public bool IsMainModule { get { return isMainModule; } }
    }

    #endregion

    /// <summary>
    /// 系统管理服务
    /// </summary>
    [RemoteService("系统管理服务", ServiceType.System)]
    public interface ISystemService
    {
        /// <summary>
        /// 向系统中注册会话
        /// </summary>
        /// <param name="sessionID">会话标识</param>
        /// <param name="userName">用户名称</param>
        /// <param name="ipAddress">调用方IP地址</param>
        /// <param name="encryptKey">密钥</param>
        void RegisterSession(string sessionID, string userName, string ipAddress, string encryptKey);

        /// <summary>
        /// 检查指定类型的服务
        /// </summary>
        /// <param name="type">服务类型</param>
        void InspectService(Type type);

        /// <summary>
        /// 注销会话
        /// </summary>
        /// <param name="sessionID">会话标识</param>
        [RemoteMethod]
        void UnRegisterSession(string sessionID);

        /// <summary>
        /// 获取客户端配置信息
        /// </summary>
        /// <returns>返回当前用户可以加载的客户模块列表</returns>
        [RemoteMethod]
        ClientModuleInfo[] GetClientModules();

        /// <summary>
        /// 获取方法的动态调用器
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        DynamicInvokerHandler GetDynamicInvoker(MethodInfo method);

        /// <summary>
        /// 获取远程服务
        /// </summary>
        /// <returns>返回系统可用的所有远程服务列表</returns>
        List<ServiceInfo> GetAllServices();

        /// <summary>
        /// 获取远程服务
        /// </summary>
        /// <param name="sessionID">会话标识</param>
        /// <param name="clientType">客户端类型</param>
        /// <returns></returns>
        List<ServiceInfo> GetServices(string sessionID, ClientType clientType);
    }
}
