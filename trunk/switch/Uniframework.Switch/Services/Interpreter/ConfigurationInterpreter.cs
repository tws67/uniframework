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
    /// 系统配置信息解析组件方便系统从配置文件中反序列化相关组件
    /// </summary>
    internal class ConfigurationInterpreter
    {
        private WorkItem workItem;
        private IConfiguration config = null;
        private ILog logger = null;

        /// <summary>
        /// 配置信息解析组件构造函数
        /// </summary>
        /// <param name="workItem">组件容器</param>
        /// <param name="config">配置信息</param>
        public ConfigurationInterpreter(WorkItem workItem, IConfiguration config)
        {
            this.workItem = workItem;
            this.config = config;
            this.logger = this.workItem.Services.Get<ILog>();
        }

        /// <summary>
        /// 逐项分析系统配置项
        /// </summary>
        public void Parse()
        {
            //logger.Info("********************************************************************************");
            //logger.Info("***                       Uniframework is Starting!!!                      ***");
            //logger.Info("********************************************************************************");
            logger.Info("开始从系统配置文件中加载LightweightCTi组件……");

            IVirtualCTI virtualCTI = workItem.Services.Get<IVirtualCTI>();
            
            // 读取系统配置的全局变量
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

            // 注册事件侦听、分配器组件
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

                            logger.Info("向系统注册事件分配器 " + dispname);
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

            // 注册事件订阅者组件
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
                            object sub = Activator.CreateInstance(subscripterType); // 此处必须支持默认的构造函数
                            if (sub is IConfigurable)
                                ((IConfigurable)sub).Configuration(conf);
                            eventService.RegisterSubscripter(sub as ISubscripterRegister);
                            logger.Debug("注册事件订阅器，注册于 " + conf.Attributes["EventDispatcher"]);
                        }
                    }
                    else if(conf.Name.ToLower() == "remove")
                    {
                        eventService.UnRegisterSubscripter(conf.Attributes["name"]);
                    }
                }
            }

            // 逐个初始化适配器
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
