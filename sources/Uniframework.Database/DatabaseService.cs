using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

using Lephone.Data;
using Lephone.Data.Builder.Clause;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.Data.Dialect;
using Lephone.Data.Driver;
using Lephone.Data.SqlEntry;
using Lephone.Util;

using Uniframework.Services;

namespace Uniframework.Database
{
    public class DatabaseService : IDatabaseService
    {
        private readonly static string DEFAULT_CONTEXT = "default";

        private Dictionary<string, DbContext> contexts = new Dictionary<string, DbContext>();
        private ILogger logger;
        private object syncObj = new object(); // 同步锁

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseService"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public DatabaseService(ILoggerFactory loggerFactory)
        {
            contexts.Add(DEFAULT_CONTEXT, new DbContext());
            logger = loggerFactory.CreateLogger<DatabaseService>("DatabaseService");
        }

        /// <summary>
        /// Gets the <see cref="Lephone.Data.DbContext"/> with the specified context.
        /// </summary>
        /// <value></value>
        public DbContext this[string context]
        {
            get {
                lock (syncObj) {
                    if (contexts.ContainsKey(context.ToLower()))
                        return contexts[context.ToLower()];
                    else
                        try {
                            DbContext dbContext = new DbContext(context.ToLower());
                            contexts.Add(context.ToLower(), dbContext);
                            return dbContext;
                        }
                        catch {
                            return null;
                        }
                }
            }
        }

        #region Assistant functions

        /// <summary>
        /// Gets the default context.
        /// </summary>
        /// <value>The default context.</value>
        private DbContext DefaultContext
        {
            get {
                return this[DEFAULT_CONTEXT];
            }
        }

        #endregion

        #region IDatabaseService Members

        /// <summary>
        /// 执行SQL语句将SQL结果返回到DataReader中
        /// </summary>
        /// <param name="Sql">SQL语句</param>
        /// <param name="callback">回调函数/委托</param>
        public void ExecuteDataReader(SqlStatement Sql, CallbackObjectHandler<IDataReader> callback)
        {
            DefaultContext.ExecuteDataReader(Sql, callback);
        }

        /// <summary>
        /// 执行SQL语句将SQL结果返回到DataReader中
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="Sql">SQL语句</param>
        /// <param name="callback">回调函数/委托</param>
        public void ExecuteDataReader(string context, SqlStatement Sql, CallbackObjectHandler<IDataReader> callback)
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            dbContext.ExecuteDataReader(Sql, callback);
        }

        /// <summary>
        /// 执行SQL语句将SQL结果返回到DataReader中
        /// </summary>
        /// <param name="Sql">SQL语句</param>
        /// <param name="behavior">命令执行行为</param>
        /// <param name="callback">回调函数/委托</param>
        public void ExecuteDataReader(SqlStatement Sql, CommandBehavior behavior, CallbackObjectHandler<IDataReader> callback)
        {
            DefaultContext.ExecuteDataReader(Sql, behavior, callback);
        }

        /// <summary>
        /// 执行SQL语句将SQL结果返回到DataReader中
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="Sql">SQL语句</param>
        /// <param name="behavior">命令执行行为</param>
        /// <param name="callback">回调函数/委托</param>
        public void ExecuteDataReader(string context, SqlStatement Sql, CommandBehavior behavior, CallbackObjectHandler<IDataReader> callback)
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            dbContext.ExecuteDataReader(Sql, behavior, callback);
        }

        /// <summary>
        /// 执行SQL语句将SQL结果返回到DataSet数据集中
        /// </summary>
        /// <param name="Sql">SQL语句</param>
        /// <returns>结果数据集<see cref="DataSet"/></returns>
        public DataSet ExecuteDataset(SqlStatement Sql)
        {
            return DefaultContext.ExecuteDataset(Sql);
        }

        /// <summary>
        /// 执行SQL语句将SQL结果返回到DataSet数据集中
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="Sql">SQL语句</param>
        /// <returns>结果集<see cref="DataSet"/></returns>
        public DataSet ExecuteDataset(string context, SqlStatement Sql)
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            return dbContext.ExecuteDataset(Sql);
        }

        /// <summary>
        /// 执行SQL语句将SQL结果返回到DataSet数据集中
        /// </summary>
        /// <param name="Sql">SQL语句</param>
        /// <param name="ReturnType">返回类型</param>
        /// <returns>结果集<see cref="DataSet"/></returns>
        /// <remarks>此函数只对Oracle有效</remarks>
        public DataSet ExecuteDataset(SqlStatement Sql, Type ReturnType)
        {
            return DefaultContext.ExecuteDataset(Sql, ReturnType);
        }

        /// <summary>
        /// 执行SQL语句将SQL结果返回到DataSet数据集中
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="Sql">SQL语句</param>
        /// <param name="ReturnType">返回类型</param>
        /// <returns>结果集<see cref="DataSet"/></returns>
        /// <remarks>此函数只对Oracle有效</remarks>
        public DataSet ExecuteDataset(string context, SqlStatement Sql, Type ReturnType)
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            return dbContext.ExecuteDataset(Sql, ReturnType);
        }

        /// <summary>
        /// 执行SQL语句将SQL结果返回到DataSet数据集中，利用此函数可以动态执行数据库中的存储过程
        /// </summary>
        /// <param name="SqlCommandText">SQL命令语句</param>
        /// <param name="os">参数</param>
        /// <returns>结果集<see cref="DataSet"/></returns>
        public DataSet ExecuteDataset(string SqlCommandText, params object[] os)
        {
            return DefaultContext.ExecuteDataset(SqlCommandText, os);
        }

        /// <summary>
        /// 执行SQL语句将SQL结果返回到DataSet数据集中，利用此函数可以动态执行数据库中的存储过程
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="SqlCommandText">SQL命令语句</param>
        /// <param name="os">参数</param>
        /// <returns>结果集<see cref="DataSet"/></returns>
        public DataSet ExecuteDataset(string context, string SqlCommandText, params object[] os)
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            return dbContext.ExecuteDataset(SqlCommandText, os);
        }

        /// <summary>
        /// 执行不需要返回结果集的SQL语句
        /// </summary>
        /// <param name="Sql">SQL语句</param>
        /// <returns>执行成功与否的返回值</returns>
        public int ExecuteNonQuery(SqlStatement Sql)
        {
            return DefaultContext.ExecuteNonQuery(Sql);
        }

        /// <summary>
        /// 执行不需要返回结果集的SQL语句
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="Sql">SQL语句</param>
        /// <returns>执行成功与否的返回值</returns>
        public int ExecuteNonQuery(string context, SqlStatement Sql)
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            return dbContext.ExecuteNonQuery(Sql);
        }

        /// <summary>
        /// 执行不需要返回结果集的SQL命令语句
        /// </summary>
        /// <param name="SqlCommandText">SQL命令语句</param>
        /// <param name="os">参数</param>
        /// <returns>执行成功与否的返回值</returns>
        public int ExecuteNonQuery(string SqlCommandText, params object[] os)
        {
            return DefaultContext.ExecuteNonQuery(SqlCommandText, os);
        }

        /// <summary>
        /// 执行不需要返回结果集的SQL命令语句
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="SqlCommandText">SQL命令语句</param>
        /// <param name="os">参数</param>
        /// <returns>执行成功与否的返回值</returns>
        public int ExecuteNonQuery(string context, string SqlCommandText, params object[] os)
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            return dbContext.ExecuteNonQuery(SqlCommandText, os);
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="Sql">SQL语句</param>
        /// <returns>结果对象</returns>
        public object ExecuteScalar(SqlStatement Sql)
        {
            return DefaultContext.ExecuteScalar(Sql);
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="Sql">SQL语句</param>
        /// <returns>结果对象</returns>
        public object ExecuteScalar(string context, SqlStatement Sql)
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            return dbContext.ExecuteScalar(Sql);
        }

        /// <summary>
        /// 执行SQL命令语句
        /// </summary>
        /// <param name="SqlCommandText">SQL命令语句</param>
        /// <param name="os">参数</param>
        /// <returns>结果对象</returns>
        public object ExecuteScalar(string SqlCommandText, params object[] os)
        {
            return DefaultContext.ExecuteScalar(SqlCommandText, os);
        }

        /// <summary>
        /// 执行SQL命令语句
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="SqlCommandText">SQL命令语句</param>
        /// <param name="os">参数</param>
        /// <returns>结果对象</returns>
        public object ExecuteScalar(string context, string SqlCommandText, params object[] os)
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            return dbContext.ExecuteScalar(SqlCommandText, os);
        }

        /// <summary>
        /// 根据SQL命令返回<see cref="SqlStatement"/>
        /// </summary>
        /// <param name="SqlStr">SQL命令</param>
        /// <param name="os">参数</param>
        /// <returns><see cref="SqlStatement"/></returns>
        public SqlStatement GetSqlStatement(string SqlStr, params object[] os)
        {
            return DefaultContext.GetSqlStatement(SqlStr, os);
        }

        /// <summary>
        /// 根据SQL命令返回<see cref="SqlStatement"/>
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="SqlStr">SQL命令</param>
        /// <param name="os">参数</param>
        /// <returns><see cref="SqlStatement"/></returns>
        public SqlStatement GetSqlStatement(string context, string SqlStr, params object[] os)
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            return dbContext.GetSqlStatement(SqlStr, os);
        }

        /// <summary>
        /// 获取数据库中包括的用户表列表
        /// </summary>
        /// <returns>用户表列表</returns>
        public List<string> GetTableNames()
        {
            return DefaultContext.GetTableNames();
        }

        /// <summary>
        /// 获取数据库中包括的用户表列表
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <returns>用户表列表</returns>
        public List<string> GetTableNames(string context)
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            return dbContext.GetTableNames();
        }

        /// <summary>
        /// 删除满足条件指定类型<see cref="T"/>的对象
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="iwc">条件</param>
        /// <returns>执行成功与否的返回值</returns>
        public int Delete<T>(WhereCondition iwc) where T : class, IDbObject
        {
            return DefaultContext.Delete<T>(iwc);
        }

        /// <summary>
        /// 删除满足条件指定类型<see cref="T"/>的对象
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="iwc">条件</param>
        /// <returns>执行成功与否的返回值</returns>
        public int Delete<T>(string context, WhereCondition iwc) where T : class, IDbObject
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            return dbContext.Delete<T>(iwc);
        }

        /// <summary>
        /// 删除特定对象
        /// </summary>
        /// <param name="obj">待删除对象</param>
        /// <returns>执行成功与否的返回值</returns>
        public int Delete(object obj)
        {
            return DefaultContext.Delete(obj);
        }

        /// <summary>
        /// 删除特定对象
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="obj">待删除对象</param>
        /// <returns>执行成功与否的返回值</returns>
        public int Delete(string context, object obj)
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            return dbContext.Delete(obj);
        }

        /// <summary>
        /// 执行SQL语句将结果放入<see cref="DbObjectList"/>列表中
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="Sql">SQL语句</param>
        /// <returns>执行SQL语句后的<see cref="DbObjectList"/>列表</returns>
        public DbObjectList<T> ExecuteList<T>(SqlStatement Sql) where T : class, IDbObject
        {
            return DefaultContext.ExecuteList<T>(Sql);
        }

        /// <summary>
        /// 执行SQL语句将结果放入<see cref="DbObjectList"/>列表中
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="Sql">SQL语句</param>
        /// <returns>执行SQL语句后的<see cref="DbObjectList"/>列表</returns>
        public DbObjectList<T> ExecuteList<T>(string context, SqlStatement Sql) where T : class, IDbObject
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            return dbContext.ExecuteList<T>(Sql);
        }

        /// <summary>
        /// 执行SQL命令语句将结果放入<see cref="DbObjectList"/>列表中.
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="SqlStr">SQL命令语句</param>
        /// <returns>执行SQL语句后的<see cref="DbObjectList"/>列表</returns>
        public DbObjectList<T> ExecuteList<T>(string SqlStr) where T : class, IDbObject
        {
            return DefaultContext.ExecuteList<T>(SqlStr);
        }

        /// <summary>
        /// 执行SQL命令语句将结果放入<see cref="DbObjectList"/>列表中
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="SqlStr">SQL命令语句</param>
        /// <returns>执行SQL语句后的<see cref="DbObjectList"/>列表</returns>
        public DbObjectList<T> ExecuteList<T>(string context, string SqlStr) where T : class, IDbObject
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            return dbContext.ExecuteList<T>(SqlStr);
        }

        /// <summary>
        /// 执行SQL命令语句将结果放入<see cref="DbObjectList"/>列表中
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="SqlStr">SQL命令语句</param>
        /// <param name="os">参数</param>
        /// <returns>执行SQL语句后的<see cref="DbObjectList"/>列表</returns>
        public DbObjectList<T> ExecuteList<T>(string SqlStr, params object[] os) where T : class, IDbObject
        {
            return DefaultContext.ExecuteList<T>(SqlStr, os);
        }

        /// <summary>
        /// 执行SQL命令语句将结果放入<see cref="DbObjectList"/>列表中
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="SqlStr">SQL命令语句</param>
        /// <param name="os">参数</param>
        /// <returns>执行SQL语句后的<see cref="DbObjectList"/>列表</returns>
        public DbObjectList<T> ExecuteList<T>(string context, string SqlStr, params object[] os) where T : class, IDbObject
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            return dbContext.ExecuteList<T>(SqlStr, os);
        }

        /// <summary>
        /// 填充集合
        /// </summary>
        /// <param name="list">集合</param>
        /// <param name="DbObjectType">数据对象类型</param>
        /// <param name="from">From子句</param>
        /// <param name="iwc">Where子句</param>
        /// <param name="oc">Order by子句</param>
        /// <param name="lc">范围子句</param>
        public void FillCollection(IList list, Type DbObjectType, FromClause from, WhereCondition iwc, OrderBy oc, Range lc)
        {
            DefaultContext.FillCollection(list, DbObjectType, from, iwc, oc, lc);
        }

        /// <summary>
        /// 填充集合
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="list">集合</param>
        /// <param name="DbObjectType">数据对象类型</param>
        /// <param name="from">From子句</param>
        /// <param name="iwc">Where子句</param>
        /// <param name="oc">Order by子句</param>
        /// <param name="lc">范围子句</param>
        public void FillCollection(string context, IList list, Type DbObjectType, FromClause from, WhereCondition iwc, OrderBy oc, Range lc)
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            dbContext.FillCollection(list, DbObjectType, from, iwc, oc, lc);
        }

        /// <summary>
        /// 填充集合
        /// </summary>
        /// <param name="list">集合</param>
        /// <param name="DbObjectType">数据对象类型</param>
        /// <param name="Sql">SQL语句</param>
        public void FillCollection(IList list, Type DbObjectType, SqlStatement Sql)
        {
            DefaultContext.FillCollection(list, DbObjectType, Sql);
        }

        /// <summary>
        /// 填充集合
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="list">集合</param>
        /// <param name="DbObjectType">数据对象类型</param>
        /// <param name="Sql">SQL语句</param>
        public void FillCollection(string context, IList list, Type DbObjectType, SqlStatement Sql)
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            dbContext.FillCollection(list, DbObjectType, Sql);
        }

        /// <summary>
        /// 填充集合
        /// </summary>
        /// <param name="list">集合</param>
        /// <param name="DbObjectType">数据对象类型</param>
        /// <param name="iwc">Where子句</param>
        /// <param name="oc">Order by子句</param>
        /// <param name="lc">范围子句</param>
        public void FillCollection(IList list, Type DbObjectType, WhereCondition iwc, OrderBy oc, Range lc)
        {
            DefaultContext.FillCollection(list, DbObjectType, iwc, oc, lc);
        }

        /// <summary>
        /// 填充集合
        /// </summary>
        /// <param name="context">数据连接上下文</param>
        /// <param name="list">集合</param>
        /// <param name="DbObjectType">数据对象类型</param>
        /// <param name="iwc">Where子句</param>
        /// <param name="oc">Order by子句</param>
        /// <param name="lc">范围子句</param>
        public void FillCollection(string context, IList list, Type DbObjectType, WhereCondition iwc, OrderBy oc, Range lc)
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            dbContext.FillCollection(list, DbObjectType, iwc, oc, lc);
        }

        /// <summary>
        /// 获取数据库服务器上当前的时间
        /// </summary>
        /// <returns>当前日期和时间</returns>
        public DateTime GetDatabaseTime()
        {
            return DefaultContext.GetDatabaseTime();
        }

        /// <summary>
        /// 获取数据库服务器上当前的时间
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <returns>当前日期和时间</returns>
        public DateTime GetDatabaseTime(string context)
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            return dbContext.GetDatabaseTime();
        }

        /// <summary>
        /// 获取指定类型的对象
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="key">主键</param>
        /// <returns>数据对象</returns>
        public T GetObject<T>(object key) where T : class, IDbObject
        {
            return DefaultContext.GetObject<T>(key);
        }

        /// <summary>
        /// 获取指定类型的对象
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="key">主键</param>
        /// <returns>数据对象</returns>
        public T GetObject<T>(string context, object key) where T : class, IDbObject
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            return dbContext.GetObject<T>(key);
        }

        /// <summary>
        /// 获取指定类型的对象
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="c">Where子句</param>
        /// <returns>数据对象</returns>
        public T GetObject<T>(WhereCondition c) where T : class, IDbObject
        {
            return DefaultContext.GetObject<T>(c);
        }

        /// <summary>
        /// 获取指定类型的对象
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="c">Where子句</param>
        /// <returns>数据对象</returns>
        public T GetObject<T>(string context, WhereCondition c) where T : class, IDbObject
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            return dbContext.GetObject<T>(c);
        }

        /// <summary>
        /// 获取指定类型的对象
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="c">Where子句</param>
        /// <param name="ob">Order by子句</param>
        /// <returns>数据对象</returns>
        public T GetObject<T>(WhereCondition c, OrderBy ob) where T : class, IDbObject
        {
            return DefaultContext.GetObject<T>(c, ob);
        }

        /// <summary>
        /// 获取指定类型的对象
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="c">Where子句</param>
        /// <param name="ob">Order by子句</param>
        /// <returns>数据对象</returns>
        public T GetObject<T>(string context, WhereCondition c, OrderBy ob) where T : class, IDbObject
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            return dbContext.GetObject<T>(c, ob);
        }

        /// <summary>
        /// 向数据库中插入对象
        /// </summary>
        /// <param name="obj">数据对象</param>
        public void Insert(object obj)
        {
            DefaultContext.Insert(obj);
        }

        /// <summary>
        /// 向数据库中插入对象
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="obj">数据对象</param>
        public void Insert(string context, object obj)
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            dbContext.Insert(obj);
        }

        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <param name="obj">待更新数据对象</param>
        public void Update(object obj)
        {
            DefaultContext.Update(obj);
        }

        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="obj">待更新数据对象</param>
        public void Update(string context, object obj)
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            dbContext.Update(obj);
        }

        /// <summary>
        /// 保存对象到数据库
        /// </summary>
        /// <param name="obj">待保存数据对象</param>
        public void Save(object obj)
        {
            DefaultContext.Save(obj);
        }

        /// <summary>
        /// 保存对象到数据库
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="obj">待保存数据对象</param>
        public void Save(string context, object obj)
        {
            DbContext dbContext = this[context];
            Guard.ArgumentNotNull(dbContext, "dbContext");

            dbContext.Save(obj);
        }

        #endregion
    }
}
