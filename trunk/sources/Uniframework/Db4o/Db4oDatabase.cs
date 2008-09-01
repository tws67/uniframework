using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Db4objects.Db4o;

namespace Uniframework.Db4o
{
    /// <summary>
    /// db4o数据库
    /// </summary>
    public sealed class Db4oDatabase : IDb4oDatabase, IDisposable
    {
        private IObjectContainer container;
        private Mutex mutex;

        /// <summary>
        /// Initializes a new instance of the <see cref="Db4oDatabase"/> class.
        /// </summary>
        /// <param name="dbName">Name of the db.</param>
        /// <param name="container">The container.</param>
        internal Db4oDatabase(string dbName, IObjectContainer container) {
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
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// 保存数据对象到数据库中
        /// </summary>
        /// <param name="obj">待保存的数据对象</param>
        public void Save(object obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 删除数据库中指定的对象
        /// </summary>
        /// <param name="obj">待删除的数据对象</param>
        public void Delete(object obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 查询符合条件的特定对象列表
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="match">查询谓词</param>
        /// <returns>如果存在指定条件的对象返回相应的列表，否则为空</returns>
        public IList<T> Load<T>(Predicate<T> match) where T : class
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 查询符合条件的对象
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="template">待查询对象示例</param>
        /// <returns>如果存在指定条件的对象返回相应的列表，否则为空</returns>
        public IList<T> Load<T>(T template) where T : class
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 查询指定类型所有对象到列表中
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <returns>如果存在指定类型的对象返回相应的列表，否则为空</returns>
        public IList<T> Load<T>() where T : class
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
