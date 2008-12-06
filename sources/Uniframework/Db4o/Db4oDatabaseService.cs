using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using System.IO;

namespace Uniframework.Db4o
{
    /// <summary>
    /// db4o数据库服务
    /// </summary>
    public class Db4oDatabaseService : IDb4oDatabaseService, IDisposable
    {
        private Dictionary<string, IDb4oDatabase> databasese = new Dictionary<string, IDb4oDatabase>();
        private readonly static int MAX_TRYTIMES = 100;
        private readonly static string DB_EXT = ".yap";
        private string dbPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="Db4oDatabaseService"/> class.
        /// </summary>
        public Db4oDatabaseService()
        {
            if (HttpContext.Current != null)
                dbPath = HttpContext.Current.Server.MapPath("~/App_Data/");
            else {
                dbPath = FileUtility.GetParent(FileUtility.ApplicationRootPath) + @"\Data\";
                if (!Directory.Exists(dbPath))
                    Directory.CreateDirectory(dbPath);
            }

            Db4oFactory.Configure().UpdateDepth(Int32.MaxValue);
            Db4oFactory.Configure().OptimizeNativeQueries(true);
            Db4oFactory.Configure().DetectSchemaChanges(true); // 自动探测数据库模式的变化
            Db4oFactory.Configure().ExceptionsOnNotStorable(true);
        }

        #region IDb4oDatabaseService Members

        /// <summary>
        /// 打开数据库
        /// </summary>
        /// <param name="dbName">数据名称</param>
        /// <returns>打开的数据库对象</returns>
        public IDb4oDatabase Open(string dbName)
        {
            return Open(dbName, Db4oFactory.Configure());
        }

        /// <summary>
        /// 打开数据库
        /// </summary>
        /// <param name="dbName">数据库名称</param>
        /// <param name="config">数据库配置信息</param>
        /// <returns>打开的数据库对象<seealso cref="IDb4oDatabase"/></returns>
        public IDb4oDatabase Open(string dbName, IConfiguration config)
        {
            if (!databasese.ContainsKey(dbName)) {
                IDb4oDatabase db = OpenDatabase(dbName, config);
                if (db == null) {
                    int counter = 0;
                    while (db == null) {
                        db = OpenDatabase(dbName, config);
                        System.Threading.Thread.Sleep(1000);
                        counter++;
                        if (counter > MAX_TRYTIMES) 
                            throw new UniframeworkException(String.Format("已经重试超过 \"{0}\" 次, 数据库 \" {1} \"依然无法打开.", MAX_TRYTIMES, dbName));
                    }
                }
                databasese[dbName] = db;
            }
            return databasese[dbName];
        }

        /// <summary>
        /// 关闭指定的数据库
        /// </summary>
        /// <param name="dbName">数据库名称</param>
        public void Close(string dbName)
        {
            if (databasese.ContainsKey(dbName)) {
                databasese[dbName].Close();
            }
        }

        /// <summary>
        /// 删除指定数据库
        /// </summary>
        /// <param name="dbName">数据库名称</param>
        public void Drop(string dbName)
        {
            if (databasese.ContainsKey(dbName)) {
                Close(dbName);

                string filename = String.IsNullOrEmpty(Path.GetExtension(dbName)) ? Path.Combine(dbPath, dbName + DB_EXT) : Path.Combine(dbPath, dbName);
                if (File.Exists(filename))
                    File.Delete(filename);
            }
        }

        #endregion

        #region IDisposable Members
        
        private bool disposed = false;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing) {
                foreach (IObjectContainer container in databasese.Values) {
                    container.Close();
                }
                disposed = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Assistant functions

        /// <summary>
        /// Opens the database.
        /// </summary>
        /// <param name="dbName">Name of the db.</param>
        /// <param name="config">The config.</param>
        /// <returns></returns>
        private IDb4oDatabase OpenDatabase(string dbName, IConfiguration config)
        {
            try {
                string filename = String.IsNullOrEmpty(Path.GetExtension(dbName)) ? Path.Combine(dbPath, dbName + DB_EXT) : Path.Combine(dbPath, dbName);
                IObjectContainer container = Db4oFactory.OpenFile(config, filename);
                return new Db4oDatabase(filename, container);
            }
            catch { 
            }
            return null;
        }

        #endregion
    }
}
