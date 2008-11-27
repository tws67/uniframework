using System;
using System.Collections.Generic;
using System.Transactions;
using System.Threading;

using Db4objects.Db4o;

namespace Uniframework.Services
{
    /// <summary>
    /// 线程安全的db4o数据库对象
    /// </summary>
    public class db4oDatabase : IObjectDatabase, IDisposable
    {
        private Mutex mutex;
        private IObjectContainer db;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="container"></param>
        public db4oDatabase(IObjectContainer container)
        {
            mutex = new Mutex(false, this.GetType().ToString() + container.GetHashCode().ToString());
            mutex.WaitOne();
            db = container;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filename">待打开的文件名</param>
        public db4oDatabase(string filename)
        {
            mutex = new Mutex(false, this.GetType().ToString() + filename.GetHashCode().ToString());
            mutex.WaitOne();
            db = db4oHttpModule.GetClient(filename, null);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="config">配置参数</param>
        public db4oDatabase(string filename, Db4objects.Db4o.Config.IConfiguration config)
        {
            mutex = new Mutex(false, this.GetType().ToString() + filename.GetHashCode().ToString());
            mutex.WaitOne();
            db = db4oHttpModule.GetClient(filename, config);
        }

        /// <summary>
        /// db4o数据库容器对象，通过它可以进行一些底层的数据库操作
        /// </summary>
        public IObjectContainer dbService
        {
            get
            {
                return this.db;
            }
        }

        #region IDisposable 成员

        private bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                //db.Dispose();
                db = null;
                mutex.ReleaseMutex();
                mutex = null;
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region IObjectDatabase 成员

        /// <summary>
        /// 保存对象实例到数据库
        /// </summary>
        /// <param name="item">要保存的对象实例</param>
        public void Save(object item)
        {
            db4oEnlist enlist = new db4oEnlist(db, item);
            bool inTransaction = Enlist(enlist);
            db.Store(item);
            if (!inTransaction) db.Commit();
            db.Commit();
        }

        /// <summary>
        /// 从数据库中删除对象实例
        /// </summary>
        /// <param name="item">要删除的对象实例</param>
        public void Delete(object item)
        {
            db4oEnlist enlist = new db4oEnlist(db, item);
            bool inTransaction = Enlist(enlist);
            db.Delete(item);
            if (!inTransaction) db.Commit();
            db.Commit();
        }

        /// <summary>
        /// Loads the specified match.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="match">The match.</param>
        /// <returns></returns>
        public T[] Load<T>(Predicate<T> match) where T : class
        {
            List<T> list = new List<T>(db.Query<T>(match));
            return list.ToArray();
        }

        /// <summary>
        /// Loads the specified template.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template">The template.</param>
        /// <returns></returns>
        public T[] Load<T>(T template) where T : class
        {
            IObjectSet results = db.QueryByExample(template);
            List<T> list = new List<T>();
            while (results.HasNext())
            {
                list.Add((T)results.Next());
            }
            return list.ToArray();
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] Load<T>() where T : class
        {
            IObjectSet results = db.QueryByExample(typeof(T));
            List<T> list = new List<T>();
            while (results.HasNext())
            {
                list.Add((T)results.Next());
            }
            return list.ToArray();
        }

        /// <summary>
        /// 获取指定对象实例的标识
        /// </summary>
        /// <param name="obj">要操作的对象实例</param>
        /// <returns></returns>
        public string GetObjectKey(object obj)
        {
            return db.Ext().GetID(obj).ToString();
        }

        /// <summary>
        /// 根据标识获取对象实例
        /// </summary>
        /// <param name="key">标识</param>
        /// <returns></returns>
        public object GetObjectByKey(string key)
        {
            return db.Ext().GetByID(long.Parse(key));
        }

        /// <summary>
        /// 激活对象实例
        /// </summary>
        /// <param name="obj">要激活的对象实例</param>
        /// <param name="depth">需要操作的对象层次</param>
        public void Activate(object obj, int depth)
        {
            db.Activate(obj, depth);
        }

        #endregion

        #region Assistant function

        /// <summary>
        /// Enlists the specified enlist.
        /// </summary>
        /// <param name="enlist">The enlist.</param>
        /// <returns></returns>
        private bool Enlist(IEnlistmentNotification enlist)
        {
            System.Transactions.Transaction currentTx = System.Transactions.Transaction.Current;
            if (currentTx != null)
            {
                currentTx.EnlistVolatile(enlist, EnlistmentOptions.None);
                return true;
            }
            return false;
        }

        #endregion
    }
}
