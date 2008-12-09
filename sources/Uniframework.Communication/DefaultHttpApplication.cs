using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
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
        private static ServiceHost host;
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
                StartHost();
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
            if (tcpServer != null)
                tcpServer.Stop();
            if (host != null && host.State == CommunicationState.Opened)
                host.Close();
            if (container != null) // 销毁容器
                container.Dispose();
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

        public static List<IInvokeCallback> CallBacks
        {
            get
            {
                return WcfService.CallBacks;
            }
        }

        /// <summary>
        /// 启动Wcf服务宿主
        /// </summary>
        private void StartHost()
        {
            bool starthost = false;
            try {
                starthost = bool.Parse(ConfigurationManager.AppSettings["Starthost"].ToString());
            }
            catch {
                starthost = false;
            }

            if (starthost)
            {
                StartHost(true);
            }
        }

        private void StartHost(bool handleFault)
        {
            host = new ServiceHost(typeof(WcfService), new Uri[0]);
            host.Opened += delegate {
                logger.Info("Wcf 服务启动成功");
            };

            if (handleFault) {
                host.Faulted += new EventHandler(host_Faulted);
            }
            try {
                host.Open();
            }
            catch (Exception ex) {
                logger.Error("创建Wcf服务宿主时出错", ex);
                throw;
            }
        }

        private void host_Faulted(object sender, EventArgs e)
        {
            logger.Error("WCF服务Faulted. 尝试重启...");
            host = null;
            this.StartHost(false);
        }



    }
}
