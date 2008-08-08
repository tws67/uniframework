using System;
using System.Collections.Generic;
using System.Text;

using log4net;

namespace Uniframework.Services
{
    /// <summary>
    /// log4net日志记录器
    /// </summary>
    public class Log4NetLogger : ILogger
    {
        private ILog log;
        private string group;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetLogger"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="group">The group.</param>
        public Log4NetLogger(Type type, string group)
        {
            this.group = group;
            log = LogManager.GetLogger(type);
        }

        #region Assistant function

        string GetMessage(string msg)
        {
            return group + " " + msg;
        }

        #endregion

        #region ILogger Members

        /// <summary>
        /// 记录调试级别的日志
        /// </summary>
        /// <param name="message">日志信息</param>
        public void Debug(string message)
        {
            log.Debug(GetMessage(message));
        }

        /// <summary>
        /// 记录调试级别的日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="ex">异常</param>
        public void Debug(string message, Exception ex)
        {
            log.Debug(GetMessage(message), ex);
        }

        /// <summary>
        /// 记录消息级别的日志
        /// </summary>
        /// <param name="message">日志信息</param>
        public void Info(string message)
        {
            log.Info(GetMessage(message));
        }

        /// <summary>
        /// 记录消息级别的日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="ex">异常</param>
        public void Info(string message, Exception ex)
        {
            log.Info(GetMessage(message), ex);
        }

        /// <summary>
        /// 记录警告级别的日志
        /// </summary>
        /// <param name="message">日志信息</param>
        public void Warn(string message)
        {
            log.Warn(GetMessage(message));
        }

        /// <summary>
        /// 记录警告级别的日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="ex">异常</param>
        public void Warn(string message, Exception ex)
        {
            log.Warn(GetMessage(message), ex);
        }

        /// <summary>
        /// 记录错误级别的日志
        /// </summary>
        /// <param name="message">日志信息</param>
        public void Error(string message)
        {
            log.Error(GetMessage(message));
        }

        /// <summary>
        /// 记录错误级别的日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="ex">异常</param>
        public void Error(string message, Exception ex)
        {
            log.Error(GetMessage(message), ex);
        }

        /// <summary>
        /// 记录致命错误级别的日志
        /// </summary>
        /// <param name="message">日志信息</param>
        public void Fatal(string message)
        {
            log.Fatal(GetMessage(message));
        }

        /// <summary>
        /// 记录致命错误L级别的日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="ex">异常</param>
        public void Fatal(string message, Exception ex)
        {
            log.Fatal(GetMessage(message), ex);
        }

        #endregion
    }
}
