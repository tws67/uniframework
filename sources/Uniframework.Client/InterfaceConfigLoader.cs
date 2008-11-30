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
    /// ���������ü�����
    /// </summary>
    public static class InterfaceConfigLoader 
    {
        private static Dictionary<Type, ServiceInfo> subsystems = new Dictionary<Type, ServiceInfo>();
        private static Dictionary<MethodInfo, RemoteMethodInfo> services = new Dictionary<MethodInfo, RemoteMethodInfo>();

        /// <summary>
        /// �ӷ������˼������нӿڶ�����Ϣ
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
        /// ��ȡ������ϵͳ�����������
        /// </summary>
        /// <returns>��ϵͳ��������</returns>
        public static Type[] GetAllSubSystem()
        {
            List<Type> ss = new List<Type>();
            foreach (Type t in subsystems.Keys)
            {
                ss.Add(t);
            }
            return ss.ToArray();
        }

        /// <summary>
        /// ��ȡָ���ӿ����������ϵͳ
        /// </summary>
        /// <param name="interfaceType">�ӿ�����</param>
        /// <returns>����ָ���ķ���������Ϣ</returns>
        public static ServiceInfo GetSubSystemInfo(Type interfaceType)
        {
            return subsystems[interfaceType];
        }

        /// <summary>
        /// ��ȡָ���ӿڷ�������Ӧ��Զ�̷���
        /// </summary>
        /// <param name="interfaceMethodInfo">�ӿڷ���</param>
        /// <returns>Զ�̷���</returns>
        public static RemoteMethodInfo GetServiceInfo(MethodInfo interfaceMethodInfo)
        {
            if (!services.ContainsKey(interfaceMethodInfo))
                throw new ArgumentException("�޷��ҵ����� [" + interfaceMethodInfo.DeclaringType.Name + "] �ķ��� [" +
                    interfaceMethodInfo.Name + "]����ȷ���Ѿ��ڷ������������˸÷��񣬲����Ѿ��������ȷ��Attribute");
            return services[interfaceMethodInfo];
        }
    }
}
