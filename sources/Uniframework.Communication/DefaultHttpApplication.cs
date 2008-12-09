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
    /// Ĭ�ϵ�HttpӦ�ó����������ʵ������ASP.NETӦ����Ҫ�Ӵ���̳�
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
            logger.Info("***********************Ӧ�ó��������׼������************************");
            DefaultContainer.LoggerFactory = loggerFactory;
            try
            {
                StartHost();
                int port = Convert.ToInt32(ConfigurationManager.AppSettings["ServerSocketPort"]);
                int capacity = Convert.ToInt32(ConfigurationManager.AppSettings["Capactity"]);

                tcpServer = new TcpServer(port, capacity, logger);
                tcpServer.Start();
                logger.Info("TCP �����ڶ˿� [" + port + "] ��ʼ����");
            }
            catch (Exception ex)
            {
                logger.Error("TCP ��������ʧ��", ex);
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
            if (container != null) // ��������
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
            logger.Error("����δ�����쳣", exception);
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
        /// ����Wcf��������
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
                logger.Info("Wcf ���������ɹ�");
            };

            if (handleFault) {
                host.Faulted += new EventHandler(host_Faulted);
            }
            try {
                host.Open();
            }
            catch (Exception ex) {
                logger.Error("����Wcf��������ʱ����", ex);
                throw;
            }
        }

        private void host_Faulted(object sender, EventArgs e)
        {
            logger.Error("WCF����Faulted. ��������...");
            host = null;
            this.StartHost(false);
        }



    }
}
