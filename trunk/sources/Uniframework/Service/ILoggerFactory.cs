using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 日志工厂类
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// 创建一个日志记录类
        /// </summary>
        /// <typeparam name="T">日志记录者类型</typeparam>
        /// <param name="loggingPath">配置文件中日志记录路径</param>
        /// <returns>日志对象</returns>
        ILogger CreateLogger<T>(string loggingPath);

        /// <summary>
        /// 创建一个日志记录类
        /// </summary>
        /// <typeparam name="T">日志记录类型</typeparam>
        /// <returns>日志对象</returns>
        ILogger CreateLogger<T>();

        /// <summary>
        /// 创建一个日志记录类
        /// </summary>
        /// <param name="type">日志对象类型</param>
        /// <param name="loggingPath">配置文件中日志记录路径</param>
        /// <returns>日志对象</returns>
        ILogger CreateLogger(Type type, string loggingPath);

        /// <summary>
        /// 创建一个日志记录类
        /// </summary>
        /// <param name="type">日志对象类型</param>
        /// <returns>日志对象</returns>
        ILogger CreateLogger(Type type);
    }

    #region ILogger
    /// <summary>
    /// 日志记录者接口
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 记录调试级别的日志
        /// </summary>
        /// <param name="message">日志信息</param>
        void Debug(string message);

        /// <summary>
        /// 记录调试级别的日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="ex">异常</param>
        void Debug(string message, Exception ex);

        /// <summary>
        /// 记录消息级别的日志
        /// </summary>
        /// <param name="message">日志信息</param>
        void Info(string message);

        /// <summary>
        /// 记录消息级别的日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="ex">异常</param>
        void Info(string message, Exception ex);

        /// <summary>
        /// 记录警告级别的日志
        /// </summary>
        /// <param name="message">日志信息</param>
        void Warn(string message);

        /// <summary>
        /// 记录警告级别的日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="ex">异常</param>
        void Warn(string message, Exception ex);

        /// <summary>
        /// 记录错误级别的日志
        /// </summary>
        /// <param name="message">日志信息</param>
        void Error(string message);

        /// <summary>
        /// 记录错误级别的日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="ex">异常</param>
        void Error(string message, Exception ex);

        /// <summary>
        /// 记录致命错误级别的日志
        /// </summary>
        /// <param name="message">日志信息</param>
        void Fatal(string message);

        /// <summary>
        /// 记录致命错误L级别的日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="ex">异常</param>
        void Fatal(string message, Exception ex);
    }
    #endregion
}
