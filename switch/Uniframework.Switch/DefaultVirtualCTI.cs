using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using log4net;
using Microsoft.Practices.CompositeUI;
using Uniframework.Services;

namespace Uniframework.Switch
{
    public class DefaultVirtualCTI : IVirtualCTI
    {
        #region DefaultVirtualCTI fields

        private readonly static String sectionPath = "Uniframework/";
        private readonly static Int32 DefaultSessionLimit = 1000;
        private readonly static object SyncObj = new object();

        private String resourceDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
        private String projectsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Projects");

        private Dictionary<String, String> globalVars = new Dictionary<String, String>();
        private DateTime initialized = DateTime.Now;
        private String session_id = Guid.NewGuid().ToString().ToUpper();
        private Int32 sessionLimit = DefaultSessionLimit;
        private Boolean running = false;
        private bool shuttingdown = false;

        private WorkItem workItem;
        private ILog logger;

        #endregion

        /// <summary>
        /// Ĭ����������캯��
        /// </summary>
        /// <param name="workItem">ϵͳ�������</param>
        public DefaultVirtualCTI(WorkItem workItem)
        {
            this.workItem = workItem;
            this.workItem.Services.Add<IVirtualCTI>(this);
            logger = workItem.Services.Get<ILog>();

            IConfigurationService configService = workItem.Services.Get<IConfigurationService>();
            if (!configService.Exists(sectionPath))
            {
                String errStr = "��ϵͳ�����ļ����ļ�δ�ҵ� \"Uniframework\" ���ýڡ�";
                logger.Fatal(errStr);
                throw new Exception(errStr);
            }

            IConfiguration config = new XMLConfiguration(configService.GetItem(sectionPath));
            ConfigurationInterpreter confInterpreter = new ConfigurationInterpreter(workItem, config);
            confInterpreter.Parse(); // ����������Ϣ�������л�������
        }

        #region IVirtualCTI Members

        public void Start()
        {
            running = true;
            while (!shuttingdown)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

        public void Shutdown()
        {
            lock (SyncObj)
            {
                shuttingdown = true;
                running = false;
            }
        }

        public DateTime Initialized
        {
            get { return initialized; }
            set { initialized = value; }
        }

        public String Session_ID
        {
            get { return session_id; }
            internal set { session_id = value; }
        }

        public Int32 SessionLimit
        {
            get { return sessionLimit; }
            set { sessionLimit = value; }
        }

        public Boolean Running
        {
            get { return running; }
        }

        public WorkItem WorkItem
        {
            get { return workItem; }
        }

        /// <summary>
        /// �����ȫ�ֱ���
        /// </summary>
        public Dictionary<String, String> GlobalVars
        {
            get { return globalVars; }
        }

        #endregion 

        #region Assistant functions

        #endregion
    }
}
