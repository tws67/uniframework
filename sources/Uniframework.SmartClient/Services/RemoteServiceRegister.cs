using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Practices.CompositeUI;

using Uniframework.Client;
using log4net;

namespace Uniframework.SmartClient
{
    public class RemoteServiceRegister
    {
        /// <summary>
        /// ��CompositeUI��ApplicationHost������ע��Զ�̷���
        /// </summary>
        /// <param name="host"></param>
        /// <param name="serviceRepository"></param>
        public void RegisterRemoteServices(WorkItem host, ServiceRepository serviceRepository)
        {
            ILog logger = host.Services.Get<ILog>();
            InterfaceConfigLoader.LoadInterfaceConfig();
            Type[] services = InterfaceConfigLoader.GetAllSubSystem();
            foreach (Type service in services)
            {
                if (logger != null)
                    logger.Debug("ע��Զ�̷���: " + service.Name);
                host.Services.Add(service, serviceRepository.GetService(service));
            }
        }
    }
}
