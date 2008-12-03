using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Uniframework.Services;

namespace Uniframework.Client
{
    /// <summary>
    /// 服务器配置加载器
    /// </summary>
    public static class InterfaceConfigLoader 
    {
        private static Dictionary<Type, ServiceInfo> subsystems = new Dictionary<Type, ServiceInfo>();
        private static Dictionary<MethodInfo, RemoteMethodInfo> services = new Dictionary<MethodInfo, RemoteMethodInfo>();

        /// <summary>
        /// 从服务器端加载所有接口定义信息
        /// </summary>
        public static void LoadInterfaceConfig()
        {
            List<ServiceInfo> subsystemInfos = CommunicateProxy.GetRemoteInterfaceCatalog();
            foreach (ServiceInfo subsystemInfo in subsystemInfos)
            {
                if (subsystemInfo != null && subsystemInfo.Type != null)
                {
                    subsystems.Add(subsystemInfo.Type, subsystemInfo);
                    foreach (RemoteMethodInfo rmInfo in subsystemInfo.RemoteMethods)
                    {
                        if (rmInfo != null && rmInfo.MethodInfo != null)
                            services.Add(rmInfo.MethodInfo, rmInfo);
                    }
                }
            }
        }

        /// <summary>
        /// 获取所有子系统所定义的类型
        /// </summary>
        /// <returns>子系统类型数组</returns>
        public static Type[] GetAllSubSystem()
        {
            List<Type> ss = new List<Type>();
            foreach (Type t in subsystems.Keys) {
                ss.Add(t);
            }
            return ss.ToArray();
        }

        /// <summary>
        /// 获取指定接口所定义的子系统
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <returns>返回指定的服务类型信息</returns>
        public static ServiceInfo GetSubSystemInfo(Type interfaceType)
        {
            return subsystems[interfaceType];
        }

        /// <summary>
        /// 获取指定接口方法所对应的远程方法
        /// </summary>
        /// <param name="interfaceMethodInfo">接口方法</param>
        /// <returns>远程方法</returns>
        public static RemoteMethodInfo GetServiceInfo(MethodInfo interfaceMethodInfo)
        {
            if (services.ContainsKey(interfaceMethodInfo))
                return services[interfaceMethodInfo];
            else
                try {
                    services.Add(interfaceMethodInfo, GetRemoteMethod(interfaceMethodInfo));
                    return services[interfaceMethodInfo];
                }
                catch {
                    throw new ArgumentException("无法找到服务 [" + interfaceMethodInfo.DeclaringType.Name + "] 的方法 [" +
                        interfaceMethodInfo.Name + "]，请确定已经在服务器上配置了该服务，并且已经标记上正确的Attribute");
                }
        }

        private static RemoteMethodInfo GetRemoteMethod(MethodInfo method)
        {
            string serviceKey = SecurityUtility.HashObject(method.DeclaringType);
            RemoteMethodAttribute rmAttribute = method.GetCustomAttributes(typeof(RemoteMethodAttribute), true)[0] as RemoteMethodAttribute;
            return new RemoteMethodInfo(SecurityUtility.HashObject(method), serviceKey, method.Name, rmAttribute.Description, rmAttribute.Offline, method, String.Empty);
        }
    }
}
