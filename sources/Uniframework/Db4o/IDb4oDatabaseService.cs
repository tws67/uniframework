using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Db4objects.Db4o;
using Db4objects.Db4o.Config;

namespace Uniframework.Db4o
{
    /// <summary>
    /// Db4o面向对象数据库服务
    /// </summary>
    public interface IDb4oDatabaseService
    {
        /// <summary>
        /// Opens the specified db name.
        /// </summary>
        /// <param name="dbName">Name of the db.</param>
        /// <returns></returns>
        IDb4oDatabase Open(string dbName);
        /// <summary>
        /// 打开数据库
        /// </summary>
        /// <param name="dbName">数据库名称</param>
        /// <param name="config">数据库配置信息</param>
        /// <returns>打开的数据库对象<seealso cref="IDb4oDatabase"/></returns>
        IDb4oDatabase Open(string dbName, IConfiguration config);
        /// <summary>
        /// 关闭指定的数据库
        /// </summary>
        /// <param name="dbName">数据库名称</param>
        void Close(string dbName);
        /// <summary>
        /// 删除指定数据库
        /// </summary>
        /// <param name="dbName">数据库名称</param>
        void Drop(string dbName);
    }
}
