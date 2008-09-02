using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Transactions;

using Db4objects.Db4o;

namespace Uniframework.Db4o
{
    /// <summary>
    /// db4o数据库
    /// </summary>
    public class Db4oDatabase : IDb4oDatabase, IDisposable
    {
        private IObjectContainer container;
        private Mutex mutex;

        /// <summary>
        /// Initializes a new instance of the <see cref="Db4oDatabase"/> class.
        /// </summary>
        /// <param name="dbName">Name of the db.</param>
        /// <param name="container">The container.</param>
        public Db4oDatabase(string dbName, IObjectContainer container) {
            this.container = container;
            this.mutex = new Mutex(false, this.ToString() + dbName.GetHashCode().ToString());
            this.mutex.WaitOne();
        }

        #region IDb4oDatabase Members

        /// <summary>
        /// 对象容器，用于存放db4o数据库内容
        /// </summary>
        /// <value></value>
        public IObjectContainer Container
        {
            get { return container; }
        }

        /// <summary>
        /// 保存数据对象到数据库中
        /// </summary>
        /// <param name="obj">待保存的数据对象</param>
        public void Save(object obj)
        {
            Db4oEnlist enlist = new Db4oEnlist(container, obj);
            bool inTransaction = Enlist(enlist);
            container.Store(obj);
            if (!inTransaction) container.Commit();
            container.Commit();
        }

        /// <summary>
        /// 保存列表到数据库中
        /// </summary>
        /// <param name="list">待保存的数据列表</param>
        public void Save(IList<object> list)
        {
            using (TransactionScope scope = new TransactionScope()) {
                foreach (object obj in list) {
                    container.Store(obj);
                }
                container.Commit();
                scope.Complete();
            }
        }

        /// <summary>
        /// 删除数据库中指定的对象
        /// </summary>
        /// <param name="obj">待删除的数据对象</param>
        public void Delete(object obj)
        {
            Db4oEnlist enlist = new Db4oEnlist(container, obj);
            bool inTransaction = Enlist(enlist);
            container.Delete(obj);
            if (!inTransaction) container.Commit();
            container.Commit();
        }

        /// <summary>
        /// 查询符合条件的特定对象列表
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="match">查询谓词</param>
        /// <returns>如果存在指定条件的对象返回相应的列表，否则为空</returns>
        public IList<T> Load<T>(Predicate<T> match) where T : class
        {
            IList<T> list = new List<T>(container.Query<T>(match));
            return list;
        }

        /// <summary>
        /// 查询符合条件的对象
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="template">待查询对象示例</param>
        /// <returns>如果存在指定条件的对象返回相应的列表，否则为空</returns>
        public IList<T> Load<T>(T template) where T : class
        {
            IObjectSet results = container.QueryByExample(template);
            IList<T> list = new List<T>();
            while (results.HasNext())
            {
                list.Add((T)results.Next());
            }
            return list;
        }

        /// <summary>
        /// 查询指定类型所有对象到列表中
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <returns>如果存在指定类型的对象返回相应的列表，否则为空</returns>
        public IList<T> Load<T>() where T : class
        {
            IObjectSet results = container.QueryByExample(typeof(T));
            IList<T> list = new List<T>();
            while (results.HasNext())
            {
                list.Add((T)results.Next());
            }
            return list;
        }

        #endregion

        #region IDisposable Members

        private bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                container = null; // 其它用户可能也在使用此数据库
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

        #region Assistant functions

        private bool Enlist(Db4oEnlist enlist)
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
