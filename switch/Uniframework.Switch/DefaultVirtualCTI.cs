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

        private readonly static String sectionPath = "LightweightCTI/";
        private readonly static Int32 DefaultSessionLimit = 1000;
        private readonly static object SyncObj = new object();

        private String resourceDir = FileUtility.GetParent(FileUtility.ApplicationRootPath) + @"\Resources\";
        private String projectsDir = FileUtility.GetParent(FileUtility.ApplicationRootPath) + @"\Projects\";

        private Dictionary<String, object> globalVars = new Dictionary<String, object>();
        private DateTime initialized = DateTime.Now;
        private String session_id = Guid.NewGuid().ToString().ToUpper();
        private Int32 sessionLimit = DefaultSessionLimit;
        private Boolean running = false;
        private bool shuttingdown = false;

        private WorkItem workItem;
        private ILog logger;

        #endregion

        /// <summary>
        /// 默认虚拟机构造函数
        /// </summary>
        /// <param name="workItem">系统组件容器</param>
        public DefaultVirtualCTI(WorkItem workItem)
        {
            this.workItem = workItem;
            this.workItem.Services.Add<IVirtualCTI>(this);
            logger = workItem.Services.Get<ILog>();

            IConfigurationService configService = workItem.Services.Get<IConfigurationService>();
            if (!configService.Exists(sectionPath))
            {
                String errStr = "在系统配置文件中文件未找到 \"Uniframework\" 配置节。";
                logger.Fatal(errStr);
                throw new Exception(errStr);
            }

            // 创建相关文件夹
            if (!Directory.Exists(resourceDir))
                Directory.CreateDirectory(resourceDir);
            if (!Directory.Exists(projectsDir))
                Directory.CreateDirectory(projectsDir);

            IConfiguration config = new XMLConfiguration(configService.GetItem(sectionPath));
            ConfigurationInterpreter confInterpreter = new ConfigurationInterpreter(workItem, config);
            confInterpreter.Parse(); // 解析配置信息并反序列化相关组件
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
        /// 虚拟机全局变量
        /// </summary>
        public Dictionary<String, object> GlobalVars
        {
            get { return globalVars; }
        }

        #endregion 

        #region Assistant functions

        #endregion
    }
}
