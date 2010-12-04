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
using Lephone.Core;

namespace Uniframework.Database
{
    /// <summary>
    /// 数据库服务
    /// </summary>
    [RemoteService("数据库服务", ServiceType.Infrustructure)]
    public interface IDatabaseService
    {
        // DbProvider

        /// <summary>
        /// 执行SQL语句将SQL结果返回到DataReader中
        /// </summary>
        /// <param name="Sql">SQL语句</param>
        /// <param name="callback">回调函数/委托</param>
        [RemoteMethod]
        void ExecuteDataReader(SqlStatement Sql, CallbackObjectHandler<IDataReader> callback);
        /// <summary>
        /// 执行SQL语句将SQL结果返回到DataReader中
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="Sql">SQL语句</param>
        /// <param name="callback">回调函数/委托</param>
        [RemoteMethod]
        void ExecuteDataReader(string context, SqlStatement Sql, CallbackObjectHandler<IDataReader> callback);
        /// <summary>
        /// 执行SQL语句将SQL结果返回到DataReader中
        /// </summary>
        /// <param name="Sql">SQL语句</param>
        /// <param name="behavior">命令执行行为</param>
        /// <param name="callback">回调函数/委托</param>
        [RemoteMethod]
        void ExecuteDataReader(SqlStatement Sql, CommandBehavior behavior, CallbackObjectHandler<IDataReader> callback);
        /// <summary>
        /// 执行SQL语句将SQL结果返回到DataReader中
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="Sql">SQL语句</param>
        /// <param name="behavior">命令执行行为</param>
        /// <param name="callback">回调函数/委托</param>
        [RemoteMethod]
        void ExecuteDataReader(string context, SqlStatement Sql, CommandBehavior behavior, CallbackObjectHandler<IDataReader> callback);
        /// <summary>
        /// 执行SQL语句将SQL结果返回到DataSet数据集中
        /// </summary>
        /// <param name="Sql">SQL语句</param>
        /// <returns>结果数据集<see cref="DataSet"/></returns>
        [RemoteMethod]
        DataSet ExecuteDataset(SqlStatement Sql);
        /// <summary>
        /// 执行SQL语句将SQL结果返回到DataSet数据集中
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="Sql">SQL语句</param>
        /// <returns>结果集<see cref="DataSet"/></returns>
        [RemoteMethod]
        DataSet ExecuteDataset(string context, SqlStatement Sql);
        /// <summary>
        /// 执行SQL语句将SQL结果返回到DataSet数据集中
        /// </summary>
        /// <param name="Sql">SQL语句</param>
        /// <param name="ReturnType">返回类型</param>
        /// <returns>结果集<see cref="DataSet"/></returns>
        /// <remarks>此函数只对Oracle有效</remarks>
        [RemoteMethod]
        DataSet ExecuteDataset(SqlStatement Sql, Type ReturnType);
        /// <summary>
        /// 执行SQL语句将SQL结果返回到DataSet数据集中
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="Sql">SQL语句</param>
        /// <param name="ReturnType">返回类型</param>
        /// <returns>结果集<see cref="DataSet"/></returns>
        /// <remarks>此函数只对Oracle有效</remarks>
        [RemoteMethod]
        DataSet ExecuteDataset(string context, SqlStatement Sql, Type ReturnType);
        /// <summary>
        /// 执行SQL语句将SQL结果返回到DataSet数据集中，利用此函数可以动态执行数据库中的存储过程
        /// </summary>
        /// <param name="SqlCommandText">SQL命令语句</param>
        /// <param name="os">参数</param>
        /// <returns>结果集<see cref="DataSet"/></returns>
        [RemoteMethod]
        DataSet ExecuteDataset(string SqlCommandText, params object[] os);
        /// <summary>
        /// 执行SQL语句将SQL结果返回到DataSet数据集中，利用此函数可以动态执行数据库中的存储过程
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="SqlCommandText">SQL命令语句</param>
        /// <param name="os">参数</param>
        /// <returns>结果集<see cref="DataSet"/></returns>
        [RemoteMethod]
        DataSet ExecuteDataset(string context, string SqlCommandText, params object[] os);
        /// <summary>
        /// 执行不需要返回结果集的SQL语句
        /// </summary>
        /// <param name="Sql">SQL语句</param>
        /// <returns>执行成功与否的返回值</returns>
        [RemoteMethod]
        int ExecuteNonQuery(SqlStatement Sql);
        /// <summary>
        /// 执行不需要返回结果集的SQL语句
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="Sql">SQL语句</param>
        /// <returns>执行成功与否的返回值</returns>
        [RemoteMethod]
        int ExecuteNonQuery(string context, SqlStatement Sql);
        /// <summary>
        /// 执行不需要返回结果集的SQL命令语句
        /// </summary>
        /// <param name="SqlCommandText">SQL命令语句</param>
        /// <param name="os">参数</param>
        /// <returns>执行成功与否的返回值</returns>
        [RemoteMethod]
        int ExecuteNonQuery(string SqlCommandText, params object[] os);
        /// <summary>
        /// 执行不需要返回结果集的SQL命令语句
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="SqlCommandText">SQL命令语句</param>
        /// <param name="os">参数</param>
        /// <returns>执行成功与否的返回值</returns>
        [RemoteMethod]
        int ExecuteNonQuery(string context, string SqlCommandText, params object[] os);
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="Sql">SQL语句</param>
        /// <returns>结果对象</returns>
        [RemoteMethod]
        object ExecuteScalar(SqlStatement Sql);
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="Sql">SQL语句</param>
        /// <returns>结果对象</returns>
        [RemoteMethod]
        object ExecuteScalar(string context, SqlStatement Sql);
        /// <summary>
        /// 执行SQL命令语句
        /// </summary>
        /// <param name="SqlCommandText">SQL命令语句</param>
        /// <param name="os">参数</param>
        /// <returns>结果对象</returns>
        [RemoteMethod]
        object ExecuteScalar(string SqlCommandText, params object[] os);
        /// <summary>
        /// 执行SQL命令语句
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="SqlCommandText">SQL命令语句</param>
        /// <param name="os">参数</param>
        /// <returns>结果对象</returns>
        [RemoteMethod]
        object ExecuteScalar(string context, string SqlCommandText, params object[] os);
        /// <summary>
        /// 根据SQL命令返回<see cref="SqlStatement"/>
        /// </summary>
        /// <param name="SqlStr">SQL命令</param>
        /// <param name="os">参数</param>
        /// <returns><see cref="SqlStatement"/></returns>
        [RemoteMethod]
        SqlStatement GetSqlStatement(string SqlStr, params object[] os);
        /// <summary>
        /// 根据SQL命令返回<see cref="SqlStatement"/>
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="SqlStr">SQL命令</param>
        /// <param name="os">参数</param>
        /// <returns><see cref="SqlStatement"/></returns>
        [RemoteMethod]
        SqlStatement GetSqlStatement(string context, string SqlStr, params object[] os);
        /// <summary>
        /// 获取数据库中包括的用户表列表
        /// </summary>
        /// <returns>用户表列表</returns>
        [RemoteMethod]
        List<string> GetTableNames();
        /// <summary>
        /// 获取数据库中包括的用户表列表
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <returns>用户表列表</returns>
        [RemoteMethod]
        List<string> GetTableNames(string context);

        // DbContext

        /// <summary>
        /// 删除满足条件指定类型<see cref="T"/>的对象
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="iwc">条件</param>
        /// <returns>执行成功与否的返回值</returns>
        [RemoteMethod]
        int Delete<T>(Condition iwc) where T : class, IDbObject;
        /// <summary>
        /// 删除满足条件指定类型<see cref="T"/>的对象
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="iwc">条件</param>
        /// <returns>执行成功与否的返回值</returns>
        [RemoteMethod]
        int Delete<T>(string context, Condition iwc) where T : class, IDbObject;
        /// <summary>
        /// 删除特定对象
        /// </summary>
        /// <param name="obj">待删除对象</param>
        /// <returns>执行成功与否的返回值</returns>
        [RemoteMethod]
        int Delete(object obj);
        /// <summary>
        /// 删除特定对象
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="obj">待删除对象</param>
        /// <returns>执行成功与否的返回值</returns>
        [RemoteMethod]
        int Delete(string context, object obj);
        /// <summary>
        /// 执行SQL语句将结果放入<see cref="DbObjectList"/>列表中
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="Sql">SQL语句</param>
        /// <returns>执行SQL语句后的<see cref="DbObjectList"/>列表</returns>
        [RemoteMethod]
        DbObjectList<T> ExecuteList<T>(SqlStatement Sql) where T : class, IDbObject;
        /// <summary>
        /// 执行SQL语句将结果放入<see cref="DbObjectList"/>列表中
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="Sql">SQL语句</param>
        /// <returns>执行SQL语句后的<see cref="DbObjectList"/>列表</returns>
        [RemoteMethod]
        DbObjectList<T> ExecuteList<T>(string context, SqlStatement Sql) where T : class, IDbObject;
        /// <summary>
        /// 执行SQL命令语句将结果放入<see cref="DbObjectList"/>列表中.
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="SqlStr">SQL命令语句</param>
        /// <returns>执行SQL语句后的<see cref="DbObjectList"/>列表</returns>
        [RemoteMethod]
        DbObjectList<T> ExecuteList<T>(string SqlStr) where T : class, IDbObject;
        /// <summary>
        /// 执行SQL命令语句将结果放入<see cref="DbObjectList"/>列表中
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="SqlStr">SQL命令语句</param>
        /// <returns>执行SQL语句后的<see cref="DbObjectList"/>列表</returns>
        [RemoteMethod]
        DbObjectList<T> ExecuteList<T>(string context, string SqlStr) where T : class, IDbObject;
        /// <summary>
        /// 执行SQL命令语句将结果放入<see cref="DbObjectList"/>列表中
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="SqlStr">SQL命令语句</param>
        /// <param name="os">参数</param>
        /// <returns>执行SQL语句后的<see cref="DbObjectList"/>列表</returns>
        [RemoteMethod]
        DbObjectList<T> ExecuteList<T>(string SqlStr, params object[] os) where T : class, IDbObject;
        /// <summary>
        /// 执行SQL命令语句将结果放入<see cref="DbObjectList"/>列表中
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="SqlStr">SQL命令语句</param>
        /// <param name="os">参数</param>
        /// <returns>执行SQL语句后的<see cref="DbObjectList"/>列表</returns>
        [RemoteMethod]
        DbObjectList<T> ExecuteList<T>(string context, string SqlStr, params object[] os) where T : class, IDbObject;
        /// <summary>
        /// 填充集合
        /// </summary>
        /// <param name="context">数据连接上下文</param>
        /// <param name="list">集合</param>
        /// <param name="DbObjectType">数据对象类型</param>
        /// <param name="iwc">Where子句</param>
        /// <param name="oc">Order by子句</param>
        /// <param name="lc">范围子句</param>
        [RemoteMethod]
        void FillCollection(IList list, Type returnType, Type dbObjectType, FromClause from, Condition iwc, OrderBy oc, Range lc, bool isDistinct);
        [RemoteMethod]
        void FillCollection(string context, IList list, Type returnType, Type dbObjectType, FromClause from, Condition iwc, OrderBy oc, Range lc, bool isDistinct);
        /// <summary>
        /// 获取数据库服务器上当前的时间
        /// </summary>
        /// <returns>当前日期和时间</returns>
        [RemoteMethod]
        DateTime GetDatabaseTime();
        /// <summary>
        /// 获取数据库服务器上当前的时间
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <returns>当前日期和时间</returns>
        [RemoteMethod]
        DateTime GetDatabaseTime(string context);
        /// <summary>
        /// 获取指定类型的对象
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="key">主键</param>
        /// <returns>数据对象</returns>
        [RemoteMethod]
        T GetObject<T>(object key) where T : class, IDbObject;
        /// <summary>
        /// 获取指定类型的对象
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="key">主键</param>
        /// <returns>数据对象</returns>
        [RemoteMethod]
        T GetObject<T>(string context, object key) where T : class, IDbObject;
        /// <summary>
        /// 获取指定类型的对象
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="c">Where子句</param>
        /// <returns>数据对象</returns>
        [RemoteMethod]
        T GetObject<T>(Condition c) where T : class, IDbObject;
        /// <summary>
        /// 获取指定类型的对象
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="c">Where子句</param>
        /// <returns>数据对象</returns>
        [RemoteMethod]
        T GetObject<T>(string context, Condition c) where T : class, IDbObject;
        /// <summary>
        /// 获取指定类型的对象
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="c">Where子句</param>
        /// <param name="ob">Order by子句</param>
        /// <returns>数据对象</returns>
        [RemoteMethod]
        T GetObject<T>(Condition c, OrderBy ob) where T : class, IDbObject;
        /// <summary>
        /// 获取指定类型的对象
        /// </summary>
        /// <typeparam name="T">泛型类型<see cref="T"/></typeparam>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="c">Where子句</param>
        /// <param name="ob">Order by子句</param>
        /// <returns>数据对象</returns>
        [RemoteMethod]
        T GetObject<T>(string context, Condition c, OrderBy ob) where T : class, IDbObject;
        /// <summary>
        /// 向数据库中插入对象
        /// </summary>
        /// <param name="obj">数据对象</param>
        [RemoteMethod]
        void Insert(object obj);
        /// <summary>
        /// 向数据库中插入对象
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="obj">数据对象</param>
        [RemoteMethod]
        void Insert(string context, object obj);
        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <param name="obj">待更新数据对象</param>
        [RemoteMethod]
        void Update(object obj);
        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="obj">待更新数据对象</param>
        [RemoteMethod]
        void Update(string context, object obj);
        /// <summary>
        /// 保存对象到数据库
        /// </summary>
        /// <param name="obj">待保存数据对象</param>
        [RemoteMethod]
        void Save(object obj);
        /// <summary>
        /// 保存对象到数据库
        /// </summary>
        /// <param name="context">数据库连接上下文</param>
        /// <param name="obj">待保存数据对象</param>
        [RemoteMethod]
        void Save(string context, object obj);
    }
}
