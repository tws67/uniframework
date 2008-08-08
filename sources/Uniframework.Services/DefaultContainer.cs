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
            // ��ʼ����־ϵͳ��������
            try
            {
                if (loggerFactory == null) throw new Exception("��־����û�б���ʼ��");
                IConfigurationInterpreter interpreter = new XmlInterpreter();
                interpreter.ProcessResource(interpreter.Source, Kernel.ConfigurationStore);
                this.Kernel.AddComponentInstance("logger", typeof(ILoggerFactory), loggerFactory);
                logger = loggerFactory.CreateLogger<DefaultContainer>("Framework");
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("Framework", "��־��������" + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                throw;
            }

            // ���ط������˵ķ���
            try
            {
                logger.Info("��ʼ����ע������");
                //AddComponent("RegisterService", typeof(IRegisterService), typeof(XmlRegisterService));
                AddComponent("ConfigurationService", typeof(IConfigurationService), typeof(XMLConfigurationService));

                logger.Info("��ʼ����Ƕ��ʽ�������ݿ����");
                AddComponent("ObjectDatabaseService", typeof(IObjectDatabaseService), typeof(db4oDatabaseService));

                logger.Info("��ʼ�����¼��ַ�����");
                AddFacility("eventautowiring", new EventAutoWiringFacility());

                logger.Info("��ʼ���ػỰ�������");
                AddComponent("SessionService", typeof(ISessionService), typeof(SessionService));

                logger.Info("��ʼ����ϵͳ�������");
                AddFacility("System.Facility", new SystemFacility());

                logger.Info("��ʼ���ؿͻ��˳�ʼ������");
                AddComponent("InitializeService", typeof(IInitializeService), typeof(InitializeService));

                logger.Info("��ʼ����Զ�̵��÷���");
                AddComponent("DefaultServiceCaller", typeof(IServiceCaller), typeof(DefaultServiceCaller));

                CheckBuiltInService(); // ��Զ�̷���Զ�̷�������ע�봦��

                AbstractExtend[] loadedExtends = AddExtends();

                string[] customServices = AddComponentFromConfiguration();

                object[] components = ActivatingComponents();

                WiringEvent();

                foreach (AbstractExtend extend in loadedExtends)
                {
                    extend.LoadFinished(components);
                }

                logger.Info("��ʼ���ط��������ط���");
                AddComponent("ServiceGateway", typeof(ServiceGateway));

                systemReady = true;
                logger.Info("�����������");
            }
            catch (Exception ex)
            {
                logger.Fatal("ע�����ʱ��������", ex);
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

            // �������е���չ��
            foreach (IConfiguration extend in extends.Children) {
                if (extend.Attributes["class"] == null)
                    throw new ArgumentException("ϵͳ��չ�����ò���ȷ��û������[class]���ԡ�");

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
                    logger.Error("�ڼ������ [" + node.Service + "] ��ʱ��������", ex);
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
                        throw new ArgumentException(String.Format("���� \"{0} \" ���ô�����Ϊ�����ö�Ӧ��[interface]��[commponent]���ԡ�", service.Name));
                    }

                    if (!String.IsNullOrEmpty(inteDef)) {
                        inteType = Type.GetType(inteDef);
                        if (inteDef == null) throw new ArgumentException("�޷���ȡ�ӿ� [" + inteDef + "] ������");
                    }
                    if (!String.IsNullOrEmpty(commDef)) {
                        commType = Type.GetType(commDef);
                        if (commType == null) throw new ArgumentException("�޷���ȡ��� [" + commDef + "] ������");
                    }

                    // ��ϵͳ������ע�����
                    if (inteType != null) {
                        if (commType == null)
                            throw new ArgumentException("�޷���ȡ�ӿ� [" + inteDef + "] ��ʵ���ඨ��");
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
                    logger.Error("��ע����� [" + service.Name + "] ʱ��������", ex);
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
