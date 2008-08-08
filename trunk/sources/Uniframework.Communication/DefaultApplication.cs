using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

using Uniframework.Services;

namespace Uniframework.Communication
{
    /// <summary>
    /// 默认的服务器实例，此类用于构建Winform及Windows服务程序，在创建应用程序时需要首先初始一个本类的实例。
    /// </summary>
    public class DefaultApplication : DisposableBase
    {
        private static ILoggerFactory loggerFactory;
        private static ILogger logger;
        private static TcpServer tcpServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultApplication"/> class.
        /// </summary>
        public DefaultApplication()
        {
            loggerFactory = new Log4NetLoggerFactory();
            logger = loggerFactory.CreateLogger<DefaultHttpApplication>("Framework");
            logger.Info("***********************应用程序服务器准备启动************************");
            DefaultContainer.LoggerFactory = loggerFactory;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            try
            {
                int port = Convert.ToInt32(ConfigurationManager.AppSettings["ServerSocketPort"]);
                int capacity = Convert.ToInt32(ConfigurationManager.AppSettings["Capacity"]);

                tcpServer = new TcpServer(port, 100, logger);
                tcpServer.Start();
                logger.Info("TCP 服务在端口 [" + port + "] 开始监听");
            }
            catch (Exception ex)
            {
                logger.Error("TCP 服务启动失败", ex);
            }
        }

        #region Assistant functions

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
                logger.Error("出现未处理异常", ex);
            else
                logger.Error("出现未处理异常，系统无法得到细节信息。");
        }

        protected override void Free(bool dispodedByUser)
        {
            if (dispodedByUser)
                tcpServer.Stop();
            base.Free(dispodedByUser);
        }

        #endregion

        #region Members

        public static ILoggerFactory LoggerFactory
        {
            get
            {
                return loggerFactory;
            }
        }

        public static DefaultContainer Container
        {
            get
            {
                return Singleton<DefaultContainer>.Instance;
            }
        }

        #endregion
    }
}
