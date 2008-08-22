using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Web;
using Castle.Windsor;

using Uniframework.Services;

namespace Uniframework.Communication
{
    /// <summary>
    /// 默认的Http应用程序服务器端实例，在ASP.NET应用中要从此类继承
    /// </summary>
    public class DefaultHttpApplication : HttpApplication
    {
        private static ILoggerFactory loggerFactory;
        private static ILogger logger;
        private static TcpServer tcpServer;
        private static DefaultContainer container;
        private static object syncObj = new object();

        /// <summary>
        /// Handles the OnStart event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void Application_OnStart(object sender, EventArgs e)
        {
            loggerFactory = new Log4NetLoggerFactory();
            logger = loggerFactory.CreateLogger<DefaultHttpApplication>("Framework");
            logger.Info("***********************应用程序服务器准备启动************************");
            DefaultContainer.LoggerFactory = loggerFactory;
            try
            {
                int port = Convert.ToInt32(ConfigurationManager.AppSettings["ServerSocketPort"]);
                int capacity = Convert.ToInt32(ConfigurationManager.AppSettings["Capactity"]);

                tcpServer = new TcpServer(port, capacity, logger);
                tcpServer.Start();
                logger.Info("TCP 服务在端口 [" + port + "] 开始监听");
            }
            catch (Exception ex)
            {
                logger.Error("TCP 服务启动失败", ex);
            }
        }

        /// <summary>
        /// Handles the OnEnd event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void Application_OnEnd(object sender, EventArgs e)
        {
            tcpServer.Stop();
        }

        /// <summary>
        /// Handles the Error event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            logger.Error("出现未处理异常", exception);
        }

        /// <summary>
        /// Gets the logger factory.
        /// </summary>
        /// <value>The logger factory.</value>
        public static ILoggerFactory LoggerFactory
        {
            get
            {
                return loggerFactory;
            }
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        public static DefaultContainer Container
        {
            get
            {
                if (container == null)
                {
                    lock (syncObj)
                    {
                        if (container == null) container = new DefaultContainer();
                    }
                }
                return container;
            }
        }
    }
}
