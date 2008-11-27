using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.ObjectBuilder;

using Uniframework.Services;
using Uniframework.SmartClient;

namespace Uniframework.Switch
{
    /// <summary>
    /// ��������ģ�����
    /// </summary>
    public class Module : ModuleInit
    {
        private WorkItem workItem;

        [InjectionConstructor]
        public Module([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem;
        }

        public WorkItem WorkItem
        {
            get { return workItem; }
        }

        public override void AddServices()
        {
            base.AddServices();

            string filename = string.Empty;
            if (HttpContext.Current != null)
                filename = HttpContext.Current.Request.PhysicalApplicationPath + "Web.config";
            else
                filename = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            if (filename == string.Empty)
                throw new Exception("û���ҵ�ϵͳ�����ļ��޷���� Uniframework �ĳ�ʼ��������");

            workItem.Services.Add(typeof(IConfigurationService), new XMLConfigurationService(filename));
            workItem.Services.Add(typeof(IEventService), new EventService());
        }

        public override void Load()
        {
            base.Load();

            ControlledWorkItem<SwitchController> switchWorkItem = WorkItem.WorkItems.AddNew<ControlledWorkItem<SwitchController>>();
            switchWorkItem.Run();
            //SwitchWorkItem switchWorkItem = workItem.WorkItems.AddNew<SwitchWorkItem>();
            //switchWorkItem.Init();
        }
    }
}
