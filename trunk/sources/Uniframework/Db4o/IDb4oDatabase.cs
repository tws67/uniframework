using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Db4objects.Db4o;

namespace Uniframework.Db4o
{
    /// <summary>
    /// db4o数据库
    /// </summary>
    public interface IDb4oDatabase
    {
        /// <summary>
        /// 对象容器，用于存放db4o数据库内容
        /// </summary>
        IObjectContainer Container { get; }
        /// <summary>
        /// 保存数据对象到数据库中
        /// </summary>
        /// <param name="obj">待保存的数据对象</param>
        void Save(object obj);
        /// <summary>
        /// 保存列表到数据库中
        /// </summary>
        /// <param name="list">待保存的数据列表</param>
        void Save(IList<object> list);
        /// <summary>
        /// 删除数据库中指定的对象
        /// </summary>
        /// <param name="obj">待删除的数据对象</param>
        void Delete(object obj);
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
    }
}
