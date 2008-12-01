using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using DevExpress.Skins;
using DevExpress.UserSkins;
using DevExpress.XtraEditors;
using log4net;
using log4net.Config;
using Uniframework.Client;
using Uniframework.Services;

namespace Uniframework.StartUp
{
    public class Program
    {
        private static readonly string LOGGER_CONFIGFILE = "log4net.config";
        private static ILog logger;

        private static string sessionID;
        private static EventDetector detector;
        private static frmLogin loginForm;
        private static Mutex mutex = new Mutex();
        private static Thread ms_oThread;

        #region Program Members

        public static log4net.ILog Logger
        {
            get
            {
                return logger;
            }
        }

        public static string SessionID
        {
            get
            {
                return sessionID;
            }
        }

        #endregion

        [STAThread]
        static void Main()
        {
            BonusSkins.Register();
            //OfficeSkins.Register();
            SkinManager.EnableFormSkins();
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("zh-CN");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");

            CheckRunning();
            sessionID = Guid.NewGuid().ToString();
            ChannelFactory.Url = ConfigurationManager.AppSettings["WebServiceUrl"];
            ChannelFactory.ServerAddress = ConfigurationManager.AppSettings["ServerAddress"];
            ChannelFactory.Port = int.Parse(ConfigurationManager.AppSettings["ServerPort"]);
            ChannelFactory.CommunicationChannel = (CommunicationChannel)Enum.Parse(typeof(CommunicationChannel), ConfigurationManager.AppSettings["CommunicationChannel"], true);
            ShowLoginForm(); // 显示登录窗口
            Thread.Sleep(100);
            mutex.WaitOne();

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            ShellApplication application = null;
            try
            {
                SetInitialState("加载日志组件……");
                XmlConfigurator.Configure(new FileInfo(LOGGER_CONFIGFILE));
                logger = LogManager.GetLogger(typeof(Program));
                logger.Info("****************应用程序启动，本次会话编号为[" + sessionID + "]****************");
                ClientEventDispatcher.Instance.Logger = logger;
                IncreaseProgressBar(10);

                SetInitialState("初始化通讯代理组件……");
                IncreaseProgressBar(10);

                SetInitialState("启动客户端事件探测器……");
                detector = new EventDetector(logger, Program.SessionID, ClientEventDispatcher.Instance);
                detector.Start();
                IncreaseProgressBar(10);

                application = new ShellApplication();
                application.Run();
            }
            catch (Exception ex)
            {
                logger.Fatal("运行客户端应用程序时发生错误", ex);
                XtraMessageBox.Show("运行应用程序时发生错误，异常信息为：" + ex.Message, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    if (application != null) ClearResource();
                }
                catch (Exception ex)
                {
                    logger.Warn("清理资源时发生错误", ex);
                }
                finally
                {
                    Environment.Exit(0);
                }
            }
        }

        /// <summary>
        /// 清理系统资源
        /// </summary>
        public static void ClearResource()
        {
            detector.Dispose(); // 销毁事件探测器
            ISystemService systemService = ServiceRepository.Instance.GetService<ISystemService>();
            systemService.UnRegisterSession(sessionID);
            ServiceRepository.Instance.Dispose(); // 注销远程服务
        }

        public static void SetInitialState(string state)
        {
            loginForm.SetLabel(state);
        }

        public static void IncreaseProgressBar(int i)
        {
            loginForm.IncreaceProgress(i);
        }

        public static void CloseLoginForm()
        {
            if (loginForm.InvokeRequired)
            {
                loginForm.Invoke(new MethodInvoker(CloseLoginForm));
            }
            else
                loginForm.Close();
        }

        #region Assistant functions

        /// <summary>
        /// Handles the AssemblyResolve event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.ResolveEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string[] asmFileNameTokens = args.Name.Split(", ".ToCharArray(), 5);
            string asmFile = asmFileNameTokens[0] + ".dll";

            // 从系统预定义的外部库路径加载文件以提高系统性能
            string libPath = FileUtility.GetParent(FileUtility.ApplicationRootPath) + @"\Libraries\";
            if(File.Exists(Path.Combine(libPath, asmFile)))
                return Assembly.LoadFile(Path.Combine(libPath, asmFile));

            List<string> files = FileUtility.SearchDirectory(FileUtility.GetParent(FileUtility.ApplicationRootPath), asmFile);
            if (files.Count > 0)
            {
                try
                {
                    AssemblyName asmName = AssemblyName.GetAssemblyName(files[0]);
                    if (asmName != null)
                        return Assembly.LoadFile(files[0]);
                }
                catch { }
            }
            throw new FileNotFoundException("未能加载程序集系统找不到指定的文件", asmFile);
        }

        private static void CheckRunning()
        {
            bool newMutexCreated = false;
            using (new Mutex(true, Assembly.GetExecutingAssembly().FullName, out newMutexCreated))
            {
                if (!newMutexCreated)
                {
                    XtraMessageBox.Show("已经有一个系统实例在运行。");
                    return;
                }
            }
        }

        private static void loginForm_Cancelled(object sender, EventArgs e)
        {
            try
            {
                ClearResource();
            }
            catch { }
            finally
            {
                Environment.Exit(0);
            }
        }

        private static void loginForm_Acceptted(object sender, EventArgs e)
        {
            mutex.ReleaseMutex();
        }

        private static void ShowLoginForm()
        {
            // Make sure it's only launched once.
            if (loginForm != null)
                return;
            ms_oThread = new Thread(new ThreadStart(ShowForm));
            ms_oThread.IsBackground = true;
            ms_oThread.Start();
        }

        private static void ShowForm()
        {
            mutex.WaitOne();
            loginForm = new frmLogin();
            loginForm.Acceptted += new EventHandler(loginForm_Acceptted);
            loginForm.Cancelled += new EventHandler(loginForm_Cancelled);
            loginForm.ShowDialog();
        }

        #endregion
    }
}
