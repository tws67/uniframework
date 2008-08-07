using System;
using System.Collections.Generic;
using System.Text;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using System.IO;

namespace Uniframework.Services.db4oService
{
    /// <summary>
    /// 客户端日志记录组件
    /// </summary>
    public class db4oDatabaseService : IObjectDatabaseService, IDisposable
    {
        private readonly static int MAX_TRYTIMES = 100;
        private string dbPath = string.Empty;
        private Dictionary<string, IObjectContainer> containers;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbPath">数据文件存放位置</param>
        /// <param name="logger">日志记录组件</param>
        public db4oDatabaseService(string dbPath)
        {
            this.dbPath = dbPath;
            if (!Directory.Exists(dbPath)) Directory.CreateDirectory(dbPath);
            containers = new Dictionary<string, IObjectContainer>();

            Db4oFactory.Configure().ActivationDepth(Int32.MaxValue);
            Db4oFactory.Configure().DetectSchemaChanges(true); // 自动探测数据库模式的变化
            Db4oFactory.Configure().ExceptionsOnNotStorable(true);
            Db4oFactory.Configure().OptimizeNativeQueries(true);
            Db4oFactory.Configure().UpdateDepth(Int32.MaxValue);
        }

        #region IObjectDatabaseService 成员

        /// <summary>
        /// 打开数据库对象
        /// </summary>
        /// <param name="databaseName">数据库名称</param>
        /// <returns>返回对象数据库接口</returns>
        public IObjectDatabase OpenDatabase(string databaseName)
        {
            OpenDatabaseFile(databaseName);
            return new db4oDatabase(containers[databaseName]);
        }

        /// <summary>
        /// 关闭指定数据库
        /// </summary>
        /// <param name="databaseName">数据库名称</param>
        public void CloseDatabase(string databaseName)
        {
            if (containers.ContainsKey(databaseName))
            {
                containers[databaseName].Close();
                containers.Remove(databaseName);
            }
        }

        /// <summary>
        /// 删除指定数据库
        /// </summary>
        /// <param name="databaseName">数据库名称</param>
        public void DropDatabase(string databaseName)
        {
            CloseDatabase(databaseName);

            string filename = GetdbFileName(databaseName);
            if (File.Exists(filename))
                File.Delete(filename);
        }

        /// <summary>
        /// 获取所有数据库信息
        /// </summary>
        /// <returns>返回数据库信息数组</returns>
       public ObjectDatabaseInfo[] GetAllDatabaseInfo()
        {
            List<ObjectDatabaseInfo> list = new List<ObjectDatabaseInfo>();
            foreach (string dbname in containers.Keys)
            {
                FileInfo fi = new FileInfo(GetdbFileName(dbname));
                ObjectDatabaseInfo info = new ObjectDatabaseInfo(dbname, "", (int)fi.Length);
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

        #region Assistant functions
        /// <summary>
        /// 打开db4o数据库文件
        /// </summary>
        /// <param name="databaseName">数据库名称</param>
        /// <returns>巳经打开的数据库，如果在打开db4o数据库的过程中出现错误则抛出异常</returns>
        private IObjectContainer OpenDatabaseFile(string databaseName)
        {
            IObjectContainer db = null;
            if (!containers.ContainsKey(databaseName))
            {
                string filename = GetdbFileName(databaseName);
                db = Db4oFactory.OpenFile(filename);
                if (db == null)
                {
                    int counter = 0;
                    while (db == null)
                    {
                        db = Db4oFactory.OpenFile(filename);
                        System.Threading.Thread.Sleep(1000);
                        counter++;
                        if (counter == MAX_TRYTIMES)
                            throw new Exception("已经重试超过 " + MAX_TRYTIMES.ToString() + " 次，数据库 \"" + filename + " \"依然无法打开。");
                    }
                }
                containers.Add(databaseName, db);
            }
            return containers[databaseName];
        }

        /// <summary>
        /// 获取数据库文件名
        /// </summary>
        /// <param name="databaseName">数据库名</param>
        /// <returns>以绝对路径表示的db4o数据库文件名</returns>
        private string GetdbFileName(string databaseName)
        {
            return Path.Combine(dbPath, String.IsNullOrEmpty(Path.GetExtension(databaseName)) ? databaseName + ".yap" : databaseName);
        }

        #endregion

        #region IDisposable 成员

        private bool disposed = false;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                foreach (IObjectContainer db in containers.Values)
                {
                    db.Close();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
