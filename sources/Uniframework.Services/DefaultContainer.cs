using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Castle.Core;
using Castle.MicroKernel.Facilities;
using Castle.Windsor;
using Castle.Windsor.Configuration;
using Castle.Windsor.Configuration.Interpreters;
using Uniframework.Services.Facilities;

namespace Uniframework.Services
{
    public class DefaultContainer : WindsorContainer
    {
        private ILogger logger;
        private static bool systemReady = false;
        private static ILoggerFactory loggerFactory;

        #region Members

        /// <summary>
        /// Gets or sets the logger factory.
        /// </summary>
        /// <value>The logger factory.</value>
        public static ILoggerFactory LoggerFactory
        {
            get
            {
                return loggerFactory;
            }
            set
            {
                loggerFactory = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [system ready].
        /// </summary>
        /// <value><c>true</c> if [system ready]; otherwise, <c>false</c>.</value>
        public static bool SystemReady
        {
            get
            {
                return systemReady;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultContainer"/> class.
        /// </summary>
        public DefaultContainer()
            : base()
        {
            // 初始化日志系统的相关组件
            try
            {
                if (loggerFactory == null) throw new Exception("日志对象没有被初始化");
                IConfigurationInterpreter interpreter = new XmlInterpreter();
                interpreter.ProcessResource(interpreter.Source, Kernel.ConfigurationStore);
                this.Kernel.AddComponentInstance("logger", typeof(ILoggerFactory), loggerFactory);
                logger = loggerFactory.CreateLogger<DefaultContainer>("Framework");
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("Framework", "日志启动错误：" + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }

            // 加载服务器端的服务
            try
            {
                logger.Info("开始加载注册表服务");
                //AddComponent("RegisterService", typeof(IRegisterService), typeof(XmlRegisterService));
                AddComponent("ConfigurationService", typeof(IConfigurationService), typeof(XMLConfigurationService));

                logger.Info("开始加载嵌入式对象数据库服务");
                AddComponent("ObjectDatabaseService", typeof(IObjectDatabaseService), typeof(db4oDatabaseService));

                logger.Info("开始加载事件分发服务");
                AddFacility("eventautowiring", new EventAutoWiringFacility());

                logger.Info("开始加载会话管理服务");
                AddComponent("SessionService", typeof(ISessionService), typeof(SessionService));

                logger.Info("开始加载系统管理服务");
                AddFacility("System.Facility", new SystemFacility());

                logger.Info("开始加载客户端初始化服务");
                AddComponent("InitializeService", typeof(IInitializeService), typeof(InitializeService));

                logger.Info("开始加载远程调用服务");
                AddComponent("DefaultServiceCaller", typeof(IServiceCaller), typeof(DefaultServiceCaller));

                CheckBuiltInService(); // 对远程服务及远程方法进行注入处理

                AbstractExtend[] loadedExtends = AddExtends();

                string[] customServices = AddComponentFromConfiguration();

                object[] components = ActivatingComponents();

                WiringEvent();

                foreach (AbstractExtend extend in loadedExtends)
                {
                    extend.LoadFinished(components);
                }

                logger.Info("开始加载服务器网关服务");
                AddComponent("ServiceGateway", typeof(ServiceGateway));

                systemReady = true;
                logger.Info("服务启动完成");
            }
            catch (Exception ex)
            {
                logger.Fatal("注册组件时发生错误", ex);
            }
        }

        #region Assistant function

        /// <summary>
        /// Adds the extends.
        /// </summary>
        /// <returns></returns>
        private AbstractExtend[] AddExtends()
        {
            string extendsPath = "/System/Extends/";
            IConfigurationService configService = this[typeof(IConfigurationService)] as IConfigurationService;
            IConfiguration extends = configService.GetChildren(extendsPath) as IConfiguration;
            List<AbstractExtend> list = new List<AbstractExtend>();

            // 加载所有的扩展项
            foreach (IConfiguration extend in extends.Children) {
                if (extend.Attributes["class"] == null)
                    throw new ArgumentException("系统扩展项配置不正确，没有配置[class]属性。");

                string extendDef = extend.Attributes["class"];
                Type type = Type.GetType(extendDef);
                AbstractExtend ext = Activator.CreateInstance(type) as AbstractExtend;
                ext.ConfigPath = extendsPath + extend.Name;
                AddFacility(type.FullName, ext);
                list.Add(ext);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Activatings the components.
        /// </summary>
        /// <returns></returns>
        private object[] ActivatingComponents()
        {
            Castle.Core.GraphNode[] nodes = Kernel.GraphNodes;
            ArrayList components = new ArrayList();

            foreach (ComponentModel node in nodes)
            {
                try
                {
                    components.Add(Kernel[node.Service]);
                }
                catch (Exception ex)
                {
                    logger.Error("在激活组件 [" + node.Service + "] 的时候发生错误", ex);
                }
            }
            return components.ToArray();
        }

        /// <summary>
        /// Checks the built in service.
        /// </summary>
        private void CheckBuiltInService()
        {
            ISystemService systemService = this[typeof(ISystemService)] as ISystemService;
            systemService.InspectService(typeof(IEventDispatcher));
            systemService.InspectService(typeof(ISystemService));
        }
        /// <summary>
        /// Adds the component from configuration.
        /// </summary>
        /// <returns></returns>
        private string[] AddComponentFromConfiguration()
        {
            List<string> serviceList = new List<string>();

            string servicesPath = "/System/Services/";
            IConfigurationService configService = this[typeof(IConfigurationService)] as IConfigurationService;
            IConfiguration services = configService.GetChildren(servicesPath) as IConfiguration;

            foreach (IConfiguration service in services.Children) {
                try
                {
                    string inteDef = String.Empty;
                    string commDef = String.Empty;
                    Type inteType = null;
                    Type commType = null;

                    try {
                        inteDef = service.Attributes["interface"];
                        commDef = service.Attributes["Commponent"];
                    }
                    catch {
                        throw new ArgumentException(String.Format("服务 \"{0} \" 配置错误，请为其配置对应的[interface]及[commponent]属性。", service.Name));
                    }

                    if (!String.IsNullOrEmpty(inteDef)) {
                        inteType = Type.GetType(inteDef);
                        if (inteDef == null) throw new ArgumentException("无法获取接口 [" + inteDef + "] 的类型");
                    }
                    if (!String.IsNullOrEmpty(commDef)) {
                        commType = Type.GetType(commDef);
                        if (commType == null) throw new ArgumentException("无法获取组件 [" + commDef + "] 的类型");
                    }

                    // 向系统容器中注册服务
                    if (inteType != null) {
                        if (commType == null)
                            throw new ArgumentException("无法获取接口 [" + inteDef + "] 的实现类定义");
                        AddComponent(inteDef, inteType, commType);
                        serviceList.Add(inteDef);
                    }
                    else if (commType != null) {
                        AddComponent(commDef, commType);
                        serviceList.Add(commDef);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("在注册组件 [" + service.Name + "] 时发生错误", ex);
                }
            }

            return serviceList.ToArray();
        }

        /// <summary>
        /// Wirings the event.
        /// </summary>
        private void WiringEvent()
        {
            IEventDispatcher dispatcher = this[typeof(IEventDispatcher)] as IEventDispatcher;
            dispatcher.ConnectInnerEvent();
        }

        #endregion
    }
}
