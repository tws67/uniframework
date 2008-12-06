using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;

namespace Uniframework.Db4o
{
    /// <summary>
    /// db4o数据库
    /// </summary>
    public interface IDb4oDatabase
    {
        /// <summary>
        /// Queries the by example.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns></returns>
        IObjectSet QueryByExample(object template);
        /// <summary>
        /// Stores the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        void Store(object obj);
        /// <summary>
        /// 删除数据库中指定的对象
        /// </summary>
        /// <param name="obj">待删除的数据对象</param>
        void Delete(object obj);
        /// <summary>
        /// Closes this instance.
        /// </summary>
        /// <returns></returns>
        bool Close();
        /// <summary>
        /// 查询符合条件的特定对象列表
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="match">查询谓词</param>
        /// <returns>如果存在指定条件的对象返回相应的列表，否则为空</returns>
        IList<T> Load<T>(Predicate<T> match) where T : class;
        /// <summary>
        /// 查询符合条件的对象
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="template">待查询对象示例</param>
        /// <returns>如果存在指定条件的对象返回相应的列表，否则为空</returns>
        IList<T> Load<T>(T template) where T : class;
        /// <summary>
        /// 查询指定类型所有对象到列表中
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <returns>如果存在指定类型的对象返回相应的列表，否则为空</returns>
        IList<T> Load<T>() where T : class;

        /// <summary>
        /// Activates the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="depth">The depth.</param>
        void Activate(object obj, int depth);
        /// <summary>
        /// Deactivates the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="depth">The depth.</param>
        void Deactivate(object obj, int depth);
        /// <summary>
        /// Exts this instance.
        /// </summary>
        /// <returns></returns>
        IExtObjectContainer Ext();
        /// <summary>
        /// Queries this instance.
        /// </summary>
        /// <returns></returns>
        IQuery Query();
        /// <summary>
        /// Queries the specified clazz.
        /// </summary>
        /// <param name="clazz">The clazz.</param>
        /// <returns></returns>
        IObjectSet Query(Type clazz);
        /// <summary>
        /// Queries the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        IObjectSet Query(Predicate predicate);
        /// <summary>
        /// Queries the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="comparator">The comparator.</param>
        /// <returns></returns>
        IObjectSet Query(Predicate predicate, IQueryComparator comparator);
        /// <summary>
        /// Queries the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns></returns>
        IObjectSet Query(Predicate predicate, IComparer comparer);
        /// <summary>
        /// Queries the specified match.
        /// </summary>
        /// <typeparam name="Extent">The type of the xtent.</typeparam>
        /// <param name="match">The match.</param>
        /// <returns></returns>
        IList<Extent> Query<Extent>(Predicate<Extent> match);
        /// <summary>
        /// Queries the specified match.
        /// </summary>
        /// <typeparam name="Extent">The type of the xtent.</typeparam>
        /// <param name="match">The match.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns></returns>
        IList<Extent> Query<Extent>(Predicate<Extent> match, IComparer<Extent> comparer);
        /// <summary>
        /// Queries the specified match.
        /// </summary>
        /// <typeparam name="Extent">The type of the xtent.</typeparam>
        /// <param name="match">The match.</param>
        /// <param name="comparison">The comparison.</param>
        /// <returns></returns>
        IList<Extent> Query<Extent>(Predicate<Extent> match, Comparison<Extent> comparison);
        /// <summary>
        /// Queries the specified extent.
        /// </summary>
        /// <typeparam name="ElementType">The type of the lement type.</typeparam>
        /// <param name="extent">The extent.</param>
        /// <returns></returns>
        IList<ElementType> Query<ElementType>(Type extent);
        /// <summary>
        /// Queries this instance.
        /// </summary>
        /// <typeparam name="Extent">The type of the xtent.</typeparam>
        /// <returns></returns>
        IList<Extent> Query<Extent>();
        /// <summary>
        /// Queries the specified comparer.
        /// </summary>
        /// <typeparam name="Extent">The type of the xtent.</typeparam>
        /// <param name="comparer">The comparer.</param>
        /// <returns></returns>
        IList<Extent> Query<Extent>(IComparer<Extent> comparer);

        /// <summary>
        /// Commits this transparent
        /// </summary>
        void Commit();
        /// <summary>
        /// Rollbacks this transparent
        /// </summary>
        void Rollback();
    }
}
