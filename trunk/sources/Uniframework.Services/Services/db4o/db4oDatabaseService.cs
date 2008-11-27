using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;

using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;

namespace Uniframework.Services
{
    /// <summary>
    /// db4o面向对象数据库服务
    /// </summary>
    public sealed class db4oDatabaseService : IObjectDatabaseService, IDisposable
    {
        readonly string CONFIG_ROOTPATH = "System/Services/ObjectDatabaseService";
        readonly string DEFAULT_DBPATH = "~/App_Data/";
        readonly int MAX_TRYTIMES = 100;

        private string dbPath;
        private Dictionary<string, IObjectContainer> containers;
        private Dictionary<string, string> descriptions;
        private ILogger logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration">注册表服务</param>
        /// <param name="loggerFactory">日志工厂</param>
        public db4oDatabaseService(IConfigurationService configService, ILoggerFactory loggerFactory)
        {
            containers = new Dictionary<string, IObjectContainer>();
            descriptions = new Dictionary<string, string>();
            logger = loggerFactory.CreateLogger<db4oDatabaseService>("Framework");

            try
            {
                Guard.ArgumentNotNull(configService, "configService");
                IConfiguration config = null;
                if (configService.Exists(CONFIG_ROOTPATH))
                    config = new XMLConfiguration(configService.GetItem(CONFIG_ROOTPATH));
                dbPath = config != null ? config.Attributes["dbpath"] 
                    : (HttpContext.Current != null) ? DEFAULT_DBPATH : Path.Combine(FileUtility.GetParent(FileUtility.ApplicationRootPath), @"\Data\");
            }
            catch (Exception ex)
            {
                throw new ArgumentException("读取db4o对象数据库服务的配置信息出错", ex);
            }

            Db4oFactory.Configure().UpdateDepth(Int32.MaxValue);
            Db4oFactory.Configure().OptimizeNativeQueries(true);
            Db4oFactory.Configure().DetectSchemaChanges(true); // 自动探测数据库模式的变化
            Db4oFactory.Configure().ExceptionsOnNotStorable(true);
        }

        #region Assistant function
        /// <summary>
        /// 打开指定的数据库
        /// </summary>
        /// <param name="databaseName">数据库名</param>
        /// <returns>db4o数据库</returns>
        /// <remarks>
        /// 添加对相对路径的支持 Modified by Jacky 2006-09-26
        /// </remarks>
        private IObjectContainer OpenDatabaseFile(string databaseName)
        {
            IObjectContainer container = null;
            try
            {
                string filename = String.IsNullOrEmpty(Path.GetExtension(databaseName)) ? Path.Combine(dbPath, databaseName + ".yap") : Path.Combine(dbPath, databaseName);
                container = Db4oFactory.OpenFile(HttpContext.Current.Server.MapPath(filename));
            }
            catch { }
            return container;
        }

        /// <summary>
        /// 打开数据库
        /// </summary>
        /// <param name="databaseName">数据库名称</param>
        /// <param name="description">描述</param>
        /// <returns>返回打开的数据库</returns>
        private IObjectDatabase OpenDatabase(string databaseName, string description)
        {
            if (!containers.ContainsKey(databaseName))
            {
                IObjectContainer container = OpenDatabaseFile(databaseName);
                if (container == null)
                {
                    int counter = 0;
                    while (container == null)
                    {
                        container = OpenDatabaseFile(databaseName);
                        System.Threading.Thread.Sleep(1000);
                        counter++;
                        if (counter > MAX_TRYTIMES) throw new Exception("已经重试超过 " + MAX_TRYTIMES.ToString() + " 次，数据库依然无法打开");
                    }
                }
                containers.Add(databaseName, container);
                descriptions.Add(databaseName, description);
            }
            return new db4oDatabase(containers[databaseName]);
        }

        #endregion

        #region IObjectDatabaseService Members

        public IObjectDatabase OpenDatabase(string databaseName)
        {
            return OpenDatabase(databaseName, "");
        }

        public void CloseDatabase(string databaseName)
        {
            if (containers.ContainsKey(databaseName))
            {
                containers[databaseName].Close();
                containers.Remove(databaseName);
                descriptions.Remove(databaseName);
            }
        }

        public void DropDatabase(string databaseName)
        {
            if (containers.ContainsKey(databaseName))
                CloseDatabase(databaseName);
            string filename = String.IsNullOrEmpty(Path.GetExtension(databaseName)) ? databaseName + ".yap" : databaseName;

            if (File.Exists(HttpContext.Current.Server.MapPath(dbPath + filename)))
                File.Delete(HttpContext.Current.Server.MapPath(dbPath + filename));
        }

        public ObjectDatabaseInfo[] GetAllDatabaseInfo()
        {
            List<ObjectDatabaseInfo> list = new List<ObjectDatabaseInfo>();
            foreach (string dbname in containers.Keys)
            {
                FileInfo fi = new FileInfo(dbPath + dbname + ".yap");
                ObjectDatabaseInfo info = new ObjectDatabaseInfo(dbname, descriptions[dbname], (int)fi.Length);
                info.StoredClasses = containers[dbname].Ext().StoredClasses().Length;
                foreach (IStoredClass storedClass in containers[dbname].Ext().StoredClasses())
                {
                    info.Objects += storedClass.GetIDs().Length;
                }
                list.Add(info);
            }
            return list.ToArray();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// 关闭所有的对象数据库
        /// </summary>
        public void Dispose()
        {
            foreach (IObjectContainer container in containers.Values)
            {
                container.Close();
            }
        }

        #endregion
    }
}
