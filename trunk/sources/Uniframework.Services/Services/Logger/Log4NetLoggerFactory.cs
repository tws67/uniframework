using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using log4net;
using log4net.Config;

namespace Uniframework.Services
{
    public class Log4NetLoggerFactory : ILoggerFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetLoggerFactory"/> class.
        /// </summary>
        public Log4NetLoggerFactory()
        {
            // 将日志配置信息整合到系统配置文件中
            FileInfo file;
            if (HttpContext.Current != null)
            {
                file = new FileInfo(HttpContext.Current.Request.PhysicalApplicationPath + "Web.config");
            }
            else
            {
                file = new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            }
            XmlConfigurator.ConfigureAndWatch(file);
        }

        #region ILoggerFactory Members

        /// <summary>
        /// Creates the logger.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loggingPath">The logging path.</param>
        /// <returns></returns>
        public ILogger CreateLogger<T>(string loggingPath)
        {
            return new Log4NetLogger(typeof(T), loggingPath);
        }

        /// <summary>
        /// Creates the logger.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ILogger CreateLogger<T>()
        {
            return new Log4NetLogger(typeof(T), String.Empty);
        }

        /// <summary>
        /// 创建一个日志记录类
        /// </summary>
        /// <param name="type">日志对象类型</param>
        /// <param name="loggingPath">配置文件中日志记录路径</param>
        /// <returns>日志对象</returns>
        public ILogger CreateLogger(Type type, string loggingPath)
        {
            return new Log4NetLogger(type, String.Empty);
        }

        /// <summary>
        /// 创建一个日志记录类
        /// </summary>
        /// <param name="type">日志对象类型</param>
        /// <returns>日志对象</returns>
        public ILogger CreateLogger(Type type)
        {
            return new Log4NetLogger(type, String.Empty);
        }

        #endregion
    }
}
