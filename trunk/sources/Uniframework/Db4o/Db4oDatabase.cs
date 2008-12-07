using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Transactions;

using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;
using System.Web;

namespace Uniframework.Db4o
{
    /// <summary>
    /// db4o数据库
    /// </summary>
    public class Db4oDatabase : IDb4oDatabase, IObjectContainer, IDisposable
    {
        private IObjectContainer container;
        private Mutex mutex;

        /// <summary>
        /// Initializes a new instance of the <see cref="Db4oDatabase"/> class.
        /// </summary>
        /// <param name="dbName">Name of the db.</param>
        public Db4oDatabase(string dbfile)
        {
            mutex = new Mutex(false, GetType() + dbfile.GetHashCode().ToString());
            mutex.WaitOne();
            dbfile = FileUtility.ConvertToFullPath(dbfile);
            if(!Directory.Exists(Path.GetDirectoryName(dbfile)))
                Directory.CreateDirectory(Path.GetDirectoryName(dbfile));

            container = Db4oFactory.OpenFile(dbfile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Db4oDatabase"/> class.
        /// </summary>
        /// <param name="dbName">Name of the db.</param>
        /// <param name="container">The container.</param>
        public Db4oDatabase(string dbfile, IObjectContainer container) {
            this.container = container;
            this.mutex = new Mutex(false, GetType() + dbfile.GetHashCode().ToString());
            this.mutex.WaitOne();
        }

        #region IDb4oDatabase Members

        /// <summary>
        /// 删除数据库中指定的对象
        /// </summary>
        /// <param name="obj">待删除的数据对象</param>
        public void Delete(object obj)
        {
            //Db4oEnlist enlist = new Db4oEnlist(container, obj);
            //bool inTransaction = Enlist(enlist);
            container.Delete(obj);
            //if (!inTransaction) container.Commit();
            //container.Commit();
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

        #region special for db4o 7.4 & C#3.0

        /// <summary>
        /// Activates the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="depth">The depth.</param>
        public void Activate(object obj, int depth)
        {
            container.Activate(obj, depth);
        }

        /// <summary>
        /// Deactivates the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="depth">The depth.</param>
        public void Deactivate(object obj, int depth)
        {
            container.Deactivate(obj, depth);
        }

        /// <summary>
        /// Exts this instance.
        /// </summary>
        /// <returns></returns>
        public IExtObjectContainer Ext()
        {
            return container.Ext();
        }

        /// <summary>
        /// Queries this instance.
        /// </summary>
        /// <returns></returns>
        public IQuery Query()
        {
            return container.Query();
        }

        /// <summary>
        /// Queries the specified clazz.
        /// </summary>
        /// <param name="clazz">The clazz.</param>
        /// <returns></returns>
        public IObjectSet Query(Type clazz)
        {
            return container.Query(clazz);
        }

        /// <summary>
        /// Queries the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public IObjectSet Query(Predicate predicate)
        {
            return container.Query(predicate);
        }

        /// <summary>
        /// Queries the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="comparator">The comparator.</param>
        /// <returns></returns>
        public IObjectSet Query(Predicate predicate, IQueryComparator comparator)
        {
            return container.Query(predicate, comparator);
        }

        /// <summary>
        /// Queries the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns></returns>
        public IObjectSet Query(Predicate predicate, IComparer comparer)
        {
            return container.Query(predicate, comparer);
        }

        /// <summary>
        /// Queries the specified match.
        /// </summary>
        /// <typeparam name="Extent">The type of the xtent.</typeparam>
        /// <param name="match">The match.</param>
        /// <returns></returns>
        public IList<Extent> Query<Extent>(Predicate<Extent> match)
        {
            return container.Query(match);
        }

        /// <summary>
        /// Queries the specified match.
        /// </summary>
        /// <typeparam name="Extent">The type of the xtent.</typeparam>
        /// <param name="match">The match.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns></returns>
        public IList<Extent> Query<Extent>(Predicate<Extent> match, IComparer<Extent> comparer)
        {
            return container.Query(match, comparer);
        }

        /// <summary>
        /// Queries the specified match.
        /// </summary>
        /// <typeparam name="Extent">The type of the xtent.</typeparam>
        /// <param name="match">The match.</param>
        /// <param name="comparison">The comparison.</param>
        /// <returns></returns>
        public IList<Extent> Query<Extent>(Predicate<Extent> match, Comparison<Extent> comparison)
        {
            return container.Query(match, comparison);
        }

        /// <summary>
        /// Queries the specified extent.
        /// </summary>
        /// <typeparam name="ElementType">The type of the lement type.</typeparam>
        /// <param name="extent">The extent.</param>
        /// <returns></returns>
        public IList<ElementType> Query<ElementType>(Type extent)
        {
            return container.Query<ElementType>(extent);
        }

        /// <summary>
        /// Queries this instance.
        /// </summary>
        /// <typeparam name="Extent">The type of the xtent.</typeparam>
        /// <returns></returns>
        public IList<Extent> Query<Extent>()
        {
            return container.Query<Extent>();
        }

        /// <summary>
        /// Queries the specified comparer.
        /// </summary>
        /// <typeparam name="Extent">The type of the xtent.</typeparam>
        /// <param name="comparer">The comparer.</param>
        /// <returns></returns>
        public IList<Extent> Query<Extent>(IComparer<Extent> comparer)
        {
            return container.Query<Extent>(comparer);
        }

        /// <summary>
        /// Commits this transparent
        /// </summary>
        public void Commit()
        {
            container.Commit();
        }

        /// <summary>
        /// Rollbacks this transparent
        /// </summary>
        public void Rollback()
        {
            container.Rollback();
        }
        
        #endregion

        #endregion

        #region IObjectContainer Members

        /// <summary>
        /// closes this IObjectContainer.
        /// </summary>
        /// <returns>
        /// success - true denotes that the last used instance of this container
        /// and the database file were closed.
        /// </returns>
        /// <remarks>
        /// closes this IObjectContainer.
        /// <br/><br/>A call to Close() automatically performs a
        /// <see cref="M:Db4objects.Db4o.IObjectContainer.Commit">Commit()</see>
        /// .
        /// <br/><br/>Note that every session opened with Db4o.OpenFile() requires one
        /// Close()call, even if the same filename was used multiple times.<br/><br/>
        /// Use <code>while(!Close()){}</code> to kill all sessions using this container.<br/><br/>
        /// </remarks>
        public bool Close()
        {
            return container.Close();
        }

        /// <summary>
        /// Query-By-Example interface to retrieve objects.
        /// </summary>
        /// <param name="template">object to be used as an example to find all matching objects.<br/><br/></param>
        /// <returns>
        /// 	<see cref="T:Db4objects.Db4o.IObjectSet">IObjectSet</see>
        /// containing all found objects.<br/><br/>
        /// </returns>
        /// <remarks>
        /// Query-By-Example interface to retrieve objects.
        /// <br/><br/><code>Get()</code> creates an
        /// <see cref="T:Db4objects.Db4o.IObjectSet">IObjectSet</see>
        /// containing
        /// all objects in the <code>IObjectContainer</code> that match the passed
        /// template object.<br/><br/>
        /// Calling <code>Get(NULL)</code> returns all objects stored in the
        /// <code>IObjectContainer</code>.<br/><br/><br/>
        /// 	<b>Query IEvaluation</b>
        /// 	<br/>All non-null members of the template object are compared against
        /// all stored objects of the same class.
        /// Primitive type members are ignored if they are 0 or false respectively.
        /// <br/><br/>Arrays and all supported <code>Collection</code> classes are
        /// evaluated for containment. Differences in <code>length/Size()</code> are
        /// ignored.
        /// <br/><br/>Consult the documentation of the IConfiguration package to
        /// configure class-specific behaviour.<br/><br/><br/>
        /// 	<b>Returned Objects</b><br/>
        /// The objects returned in the
        /// <see cref="T:Db4objects.Db4o.IObjectSet">IObjectSet</see>
        /// are instantiated
        /// and activated to the preconfigured depth of 5. The
        /// <see cref="M:Db4objects.Db4o.Config.IConfiguration.ActivationDepth(System.Int32)">activation depth</see>
        /// may be configured
        /// <see cref="M:Db4objects.Db4o.Config.IConfiguration.ActivationDepth(System.Int32)">globally</see>
        /// or
        /// <see cref="!:Db4objects.Db4o.Config.ObjectClass">individually for classes</see>
        /// .
        /// <br/><br/>
        /// db4o keeps track of all instantiatied objects. Queries will return
        /// references to these objects instead of instantiating them a second time.
        /// <br/><br/>
        /// Objects newly activated by <code>Get()</code> can respond to the callback
        /// method
        /// <see cref="!:Db4objects.Db4o.Ext.ObjectCallbacks.ObjectOnActivate">objectOnActivate</see>
        /// .
        /// <br/><br/>
        /// </remarks>
        /// <seealso cref="M:Db4objects.Db4o.Config.IConfiguration.ActivationDepth(System.Int32)">Why activation?</seealso>
        /// <seealso cref="!:Db4objects.Db4o.Ext.ObjectCallbacks">Using callbacks</seealso>
        [Obsolete("Use QueryByExample instead")]
        public IObjectSet Get(object template)
        {
            return container.Get(template);
        }

        /// <summary>
        /// Query-By-Example interface to retrieve objects.
        /// </summary>
        /// <param name="template">object to be used as an example to find all matching objects.<br/><br/></param>
        /// <returns>
        /// 	<see cref="T:Db4objects.Db4o.IObjectSet">IObjectSet</see>
        /// containing all found objects.<br/><br/>
        /// </returns>
        /// <remarks>
        /// Query-By-Example interface to retrieve objects.
        /// <br/><br/><code>Get()</code> creates an
        /// <see cref="T:Db4objects.Db4o.IObjectSet">IObjectSet</see>
        /// containing
        /// all objects in the <code>IObjectContainer</code> that match the passed
        /// template object.<br/><br/>
        /// Calling <code>Get(NULL)</code> returns all objects stored in the
        /// <code>IObjectContainer</code>.<br/><br/><br/>
        /// 	<b>Query IEvaluation</b>
        /// 	<br/>All non-null members of the template object are compared against
        /// all stored objects of the same class.
        /// Primitive type members are ignored if they are 0 or false respectively.
        /// <br/><br/>Arrays and all supported <code>Collection</code> classes are
        /// evaluated for containment. Differences in <code>length/Size()</code> are
        /// ignored.
        /// <br/><br/>Consult the documentation of the IConfiguration package to
        /// configure class-specific behaviour.<br/><br/><br/>
        /// 	<b>Returned Objects</b><br/>
        /// The objects returned in the
        /// <see cref="T:Db4objects.Db4o.IObjectSet">IObjectSet</see>
        /// are instantiated
        /// and activated to the preconfigured depth of 5. The
        /// <see cref="M:Db4objects.Db4o.Config.IConfiguration.ActivationDepth(System.Int32)">activation depth</see>
        /// may be configured
        /// <see cref="M:Db4objects.Db4o.Config.IConfiguration.ActivationDepth(System.Int32)">globally</see>
        /// or
        /// <see cref="!:Db4objects.Db4o.Config.ObjectClass">individually for classes</see>
        /// .
        /// <br/><br/>
        /// db4o keeps track of all instantiatied objects. Queries will return
        /// references to these objects instead of instantiating them a second time.
        /// <br/><br/>
        /// Objects newly activated by <code>Get()</code> can respond to the callback
        /// method
        /// <see cref="!:Db4objects.Db4o.Ext.ObjectCallbacks.ObjectOnActivate">objectOnActivate</see>
        /// .
        /// <br/><br/>
        /// </remarks>
        /// <seealso cref="M:Db4objects.Db4o.Config.IConfiguration.ActivationDepth(System.Int32)">Why activation?</seealso>
        /// <seealso cref="!:Db4objects.Db4o.Ext.ObjectCallbacks">Using callbacks</seealso>
        public IObjectSet QueryByExample(object template)
        {
            return container.QueryByExample(template);
        }

        /// <summary>
        /// newly stores objects or updates stored objects.
        /// </summary>
        /// <param name="obj">the object to be stored or updated.</param>
        /// <remarks>
        /// newly stores objects or updates stored objects.
        /// <br/><br/>An object not yet stored in the <code>IObjectContainer</code> will be
        /// stored when it is passed to <code>Set()</code>. An object already stored
        /// in the <code>IObjectContainer</code> will be updated.
        /// <br/><br/><b>Updates</b><br/>
        /// - will affect all simple type object members.<br/>
        /// - links to object members that are already stored will be updated.<br/>
        /// - new object members will be newly stored. The algorithm traverses down
        /// new members, as long as further new members are found.<br/>
        /// - object members that are already stored will <b>not</b> be updated
        /// themselves.<br/>Every object member needs to be updated individually with a
        /// call to <code>Set()</code> unless a deep
        /// <see cref="M:Db4objects.Db4o.Config.IConfiguration.UpdateDepth(System.Int32)">global</see>
        /// or
        /// <see cref="!:Db4objects.Db4o.Config.ObjectClass.UpdateDepth">class-specific</see>
        /// update depth was configured or cascaded updates were
        /// <see cref="!:Db4objects.Db4o.Config.ObjectClass.CascadeOnUpdate">defined in the class</see>
        /// or in
        /// <see cref="!:Db4objects.Db4o.Config.ObjectField.CascadeOnUpdate">one of the member fields</see>
        /// .
        /// <br/><br/><b>Examples: ../com/db4o/samples/update.</b><br/><br/>
        /// Depending if the passed object is newly stored or updated, the
        /// callback method
        /// <see cref="!:Db4objects.Db4o.Ext.ObjectCallbacks.ObjectOnNew">objectOnNew</see>
        /// or
        /// <see cref="!:Db4objects.Db4o.Ext.ObjectCallbacks.ObjectOnUpdate">objectOnUpdate</see>
        /// is triggered.
        /// <see cref="!:Db4objects.Db4o.Ext.ObjectCallbacks.ObjectOnUpdate">objectOnUpdate</see>
        /// might also be used for cascaded updates.<br/><br/>
        /// </remarks>
        /// <seealso cref="M:Db4objects.Db4o.Ext.IExtObjectContainer.Set(System.Object,System.Int32)">IExtObjectContainer#Set(object, depth)
        /// </seealso>
        /// <seealso cref="M:Db4objects.Db4o.Config.IConfiguration.UpdateDepth(System.Int32)">Db4objects.Db4o.Config.IConfiguration.UpdateDepth
        /// </seealso>
        /// <seealso cref="!:Db4objects.Db4o.Config.ObjectClass.UpdateDepth">Db4objects.Db4o.Config.ObjectClass.UpdateDepth
        /// </seealso>
        /// <seealso cref="!:Db4objects.Db4o.Config.ObjectClass.CascadeOnUpdate">Db4objects.Db4o.Config.ObjectClass.CascadeOnUpdate
        /// </seealso>
        /// <seealso cref="!:Db4objects.Db4o.Config.ObjectField.CascadeOnUpdate">Db4objects.Db4o.Config.ObjectField.CascadeOnUpdate
        /// </seealso>
        /// <seealso cref="!:Db4objects.Db4o.Ext.ObjectCallbacks">Using callbacks</seealso>
        [Obsolete("Use Store instead")]
        public void Set(object obj)
        {
            container.Set(obj);
        }

        /// <summary>
        /// Stores the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public void Store(object obj)
        {
            //Db4oEnlist enlist = new Db4oEnlist(container, obj);
            //bool inTransaction = Enlist(enlist);
            container.Store(obj);
            //if (!inTransaction) container.Commit();
            //container.Commit();
        }

        #endregion

        #region IDisposable Members

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                container.Dispose();
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
