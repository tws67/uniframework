using System;
using System.Collections.Generic;
using System.Text;

using log4net;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using Uniframework.Services;
using Uniframework.SmartClient;

namespace Uniframework.Switch
{
    /// <summary>
    /// ϵͳ������Ϣ�����������ϵͳ�������ļ��з����л�������
    /// </summary>
    internal class ConfigurationInterpreter
    {
        private WorkItem workItem;
        private IConfiguration config = null;
        private ILog logger = null;

        /// <summary>
        /// ������Ϣ����������캯��
        /// </summary>
        /// <param name="workItem">�������</param>
        /// <param name="config">������Ϣ</param>
        public ConfigurationInterpreter(WorkItem workItem, IConfiguration config)
        {
            this.workItem = workItem;
            this.config = config;
            this.logger = this.workItem.Services.Get<ILog>();
        }

        /// <summary>
        /// �������ϵͳ������
        /// </summary>
        public void Parse()
        {
            //logger.Info("********************************************************************************");
            //logger.Info("***                       Uniframework is Starting!!!                      ***");
            //logger.Info("********************************************************************************");
            logger.Info("��ʼ��ϵͳ�����ļ��м���LightweightCTi�������");

            IVirtualCTI virtualCTI = workItem.Services.Get<IVirtualCTI>();
            
            // ��ȡϵͳ���õ�ȫ�ֱ���
            if (config.Children["Variables"].Children.Count > 0)
            {
                foreach (IConfiguration conf in config.Children["Variables"].Children)
                {
                    if (conf.Name.ToLower() == "clear")
                        virtualCTI.GlobalVars.Clear();
                    else
                    {
                        if (conf.Name.ToLower() == "add")
                            virtualCTI.GlobalVars.Add(conf.Attributes["key"], conf.Attributes["value"]);
                        else if(conf.Name.ToLower() == "remove")
                            virtualCTI.GlobalVars.Remove(conf.Attributes["key"]);
                    }
                }
            }

            // ע���¼����������������
            IEventService eventService = workItem.Services.Get<IEventService>();
            if (config.Children["EventDispatchers"].Children.Count > 0 && eventService != null)
            {
                foreach (IConfiguration conf in config.Children["EventDispatchers"].Children)
                {
                    if (conf.Name.ToLower() == "clear")
                        eventService.ClearEventDispatcher();
                    else
                    {
                        if (conf.Name.ToLower() == "add")
                        {
                            string dispname = conf.Attributes["name"];
                            Type dispType = Type.GetType(conf.Attributes["type"]);
                            Type serviceType = Type.GetType(conf.Attributes["servicetype"]);
                            Type baseType = Type.GetType(conf.Attributes["basetype"]);
                            EventDispatcher dispatcher = null;
                            if (serviceType != null && baseType != null)
                            {
                                dispatcher = Activator.CreateInstance(dispType, new object[] { serviceType, baseType }) as EventDispatcher;
                            }
                            else
                                if (serviceType != null)
                                {
                                    
                                    dispatcher = Activator.CreateInstance(dispType, new object[] { serviceType }) as EventDispatcher;
                                }

                            logger.Info("��ϵͳע���¼������� " + dispname);
                            if (dispatcher is IConfigurable)
                                ((IConfigurable)dispatcher).Configuration(conf);
                            eventService.RegisterEventDispatcher(dispname, dispatcher);
                        }
                        else if(conf.Name.ToLower() == "remove")
                        {
                            eventService.UnRegisterEventDispatcher(conf.Attributes["name"]);
                        }
                    }
                }
            }

            // ע���¼����������
            if (config.Children["Subscripters"].Children.Count > 0 && eventService != null)
            {
                foreach (IConfiguration conf in config.Children["Subscripters"].Children)
                {
                    if (conf.Name.ToLower() == "add")
                    {
                        string name = conf.Attributes["name"];
                        Type subscripterType = Type.GetType(conf.Attributes["type"]);
                        if ((conf.Attributes["EventDispatcher"] != null || conf.Attributes["EventDispatcher"] != string.Empty) && subscripterType != null)
                        {
                            object sub = Activator.CreateInstance(subscripterType); // �˴�����֧��Ĭ�ϵĹ��캯��
                            if (sub is IConfigurable)
                                ((IConfigurable)sub).Configuration(conf);
                            eventService.RegisterSubscripter(sub as ISubscripterRegister);
                            logger.Debug("ע���¼���������ע���� " + conf.Attributes["EventDispatcher"]);
                        }
                    }
                    else if(conf.Name.ToLower() == "remove")
                    {
                        eventService.UnRegisterSubscripter(conf.Attributes["name"]);
                    }
                }
            }

            // �����ʼ��������
            if (config.Children["Adapters"].Children.Count < 1) return;

            foreach (IConfiguration conf in config.Children["Adapters"].Children)
            {
                if (conf.Name.ToLower() == "clear")
                    workItem.Services.Remove<ICTIDriver>();
                else
                {
                    if (conf.Name.ToLower() == "add")
                    {
                        string adapterName = conf.Attributes["name"];
                        bool active = bool.Parse(conf.Attributes["active"]);
                        Type adapterType = Type.GetType(conf.Attributes["type"]);
                        if (adapterType == null)
                            continue;

                        AbstractCTIDriver driver = Activator.CreateInstance(adapterType, new object[] { workItem }) as AbstractCTIDriver;
                        workItem.Services.Add(typeof(ICTIDriver), driver);
                        if (driver is IConfigurable)
                            ((IConfigurable)driver).Configuration(conf);
                    }
                }
            }
        }
    }
}
