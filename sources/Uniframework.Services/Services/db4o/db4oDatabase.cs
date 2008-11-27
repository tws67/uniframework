using System;
using System.Collections.Generic;
using System.Transactions;
using System.Threading;

using Db4objects.Db4o;

namespace Uniframework.Services
{
    /// <summary>
    /// �̰߳�ȫ��db4o���ݿ����
    /// </summary>
    public class db4oDatabase : IObjectDatabase, IDisposable
    {
        private Mutex mutex;
        private IObjectContainer db;

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="container"></param>
        public db4oDatabase(IObjectContainer container)
        {
            mutex = new Mutex(false, this.GetType().ToString() + container.GetHashCode().ToString());
            mutex.WaitOne();
            db = container;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="filename">���򿪵��ļ���</param>
        public db4oDatabase(string filename)
        {
            mutex = new Mutex(false, this.GetType().ToString() + filename.GetHashCode().ToString());
            mutex.WaitOne();
            db = db4oHttpModule.GetClient(filename, null);
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="filename">�ļ���</param>
        /// <param name="config">���ò���</param>
        public db4oDatabase(string filename, Db4objects.Db4o.Config.IConfiguration config)
        {
            mutex = new Mutex(false, this.GetType().ToString() + filename.GetHashCode().ToString());
            mutex.WaitOne();
            db = db4oHttpModule.GetClient(filename, config);
        }

        /// <summary>
        /// db4o���ݿ���������ͨ�������Խ���һЩ�ײ�����ݿ����
        /// </summary>
        public IObjectContainer dbService
        {
            get
            {
                return this.db;
            }
        }

        #region IDisposable ��Ա

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

        #region IObjectDatabase ��Ա

        /// <summary>
        /// �������ʵ�������ݿ�
        /// </summary>
        /// <param name="item">Ҫ����Ķ���ʵ��</param>
        public void Save(object item)
        {
            db4oEnlist enlist = new db4oEnlist(db, item);
            bool inTransaction = Enlist(enlist);
            db.Store(item);
            if (!inTransaction) db.Commit();
            db.Commit();
        }

        /// <summary>
        /// �����ݿ���ɾ������ʵ��
        /// </summary>
        /// <param name="item">Ҫɾ���Ķ���ʵ��</param>
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
        /// ��ȡָ������ʵ���ı�ʶ
        /// </summary>
        /// <param name="obj">Ҫ�����Ķ���ʵ��</param>
        /// <returns></returns>
        public string GetObjectKey(object obj)
        {
            return db.Ext().GetID(obj).ToString();
        }

        /// <summary>
        /// ���ݱ�ʶ��ȡ����ʵ��
        /// </summary>
        /// <param name="key">��ʶ</param>
        /// <returns></returns>
        public object GetObjectByKey(string key)
        {
            return db.Ext().GetByID(long.Parse(key));
        }

        /// <summary>
        /// �������ʵ��
        /// </summary>
        /// <param name="obj">Ҫ����Ķ���ʵ��</param>
        /// <param name="depth">��Ҫ�����Ķ�����</param>
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
