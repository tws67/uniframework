using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.Services;
using Microsoft.Practices.CompositeUI.UIElements;
using Microsoft.Practices.CompositeUI.WinForms;
using Microsoft.Practices.ObjectBuilder;

using Uniframework.Client;
using Uniframework.Client.ConnectionManagement;
using Uniframework.Client.OfflineProxy;
using Uniframework.Services;
using Uniframework.XtraForms;
using System.Configuration;
using System.IO;
using System.Drawing;

namespace Uniframework.SmartClient
{
    public class UniframeworkApplication<TWorkItem, TShell> : XtraFormApplicationBase<TWorkItem, TShell>
        where TWorkItem : WorkItem, new()
        where TShell : Form
    {
        private bool multiMainWorksapce;
        private AddInTree addInTree;

        /// <summary>
        /// May be overridden in derived classes to perform activities just before the shell
        /// is created.
        /// </summary>
        protected override void BeforeShellCreated()
        {
            base.BeforeShellCreated();

            InitializeEnvironment();
        }


        /// <summary>
        /// See <see cref="CabShellApplication{T,S}.AfterShellCreated"/>
        /// </summary>
        protected override void AfterShellCreated()
        {
            base.AfterShellCreated();

            Shell.Text = ConfigurationManager.AppSettings["ShellCaption"];
            string iconFile = ConfigurationManager.AppSettings["ShellIcon"];
            iconFile = File.Exists(iconFile) ? iconFile : Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory) + @"\Resources\" + iconFile;
            if (File.Exists(iconFile))
                Shell.Icon = new Icon(iconFile);
            RootWorkItem.Items.Add(Shell, "ShellForm");

            //Program.SetInitialState("加载应用模块……");
            //AddClientService();
            addInTree = new AddInTree();
            RootWorkItem.Services.Add<AddInTree>(addInTree);
            //RootWorkItem.Items.Add(addInTree, "AddInTree"); // 添加插件树到RootWorkItem
            RootWorkItem.Items.AddNew<CommandHandlers>("DefaultCommandHandlers"); // 创建框架通用的命令处理器
            RegisterUISite(); // 构建用户界面并添加UI构建服务
        }


        /// <summary>
        /// Must be overriden. This method is called when the application is fully created and
        /// ready to run.
        /// </summary>
        protected override void Start()
        {
            Application.Run(Shell);
        }

        /// <summary>
        /// Adds Windows Forms specific strategies to the builder.
        /// </summary>
        /// <param name="builder"></param>
        protected override void AddBuilderStrategies(Builder builder)
        {
            base.AddBuilderStrategies(builder);

        }

        #region Assistant functions

        /// <summary>
        /// 初始化框架系统环境
        /// </summary>
        private void InitializeEnvironment()
        {
            RemoteServiceRegister register = new RemoteServiceRegister();
            //Program.Logger.Debug("将远程服务注入到本地容器中");
            //Program.SetInitialState("检查远程服务……");
            //Program.IncreaseProgressBar(10);
            register.RegisterRemoteServices(RootWorkItem, ServiceRepository.Instance);
            RegisterUserInfo(); // 注册用户信息需要放在其它服务启动之前

            //// 注册系统权限管理服务
            //Program.SetInitialState("注册系统权限管理服务……");
            //Program.Logger.Debug("加载系统权限管理服务");
            //Program.IncreaseProgressBar(5);
            IAuthorizationService authorizationService = new AuthorizationService(RootWorkItem);
            RootWorkItem.Services.Add<IAuthorizationService>(authorizationService);

            // 注册自定义的UI组件
            //Program.SetInitialState("注册自定义的UI组件……");
            //Program.IncreaseProgressBar(10);
            RootWorkItem.Services.Add<SmartClientEnvironment>(new SmartClientEnvironment());
            RootWorkItem.Services.Add<IImageService>(new ImageService());
            RootWorkItem.Services.Add<IBuilderService>(new BuilderService(RootWorkItem));
            //RootWorkItem.Services.Add<IContentMenuBarService>(new ContentMenuBarService());

            // 添加系统自定义的默认服务
            //Program.SetInitialState("注册本地默认服务……");
            //Program.IncreaseProgressBar(10);
            //RootWorkItem.Services.Add(typeof(log4net.ILog), Program.Logger);

            RootWorkItem.Services.Remove<IModuleEnumerator>(); // 删除系统默认的模块枚举服务

            // 从服务器下载配置模块信息
            //Program.SetInitialState("从服务器下载客户端配置信息……");
            //Program.IncreaseProgressBar(10);
            WebServiceModuleEnumerator wsme = new WebServiceModuleEnumerator();
            wsme.Load();
            multiMainWorksapce = wsme.HasMultiMainWorkspace;

            RootWorkItem.Services.Add<IModuleEnumerator>(wsme);
            //Program.IncreaseProgressBar(10);
        }

        /// <summary>
        /// 注册用户信息
        /// </summary>
        private void RegisterUserInfo()
        {
            IInitializeService initService = RootWorkItem.Services.Get<IInitializeService>();
            if (initService != null)
            {
                UserInfo user = initService.GetUserInfo(CommunicateProxy.UserName);
                RootWorkItem.Items.Add(user, "CurrentUser");
                IMembershipService membershipServce = RootWorkItem.Services.Get<IMembershipService>();
                if (membershipServce != null)
                {
                    string[] roles = membershipServce.GetRolesForUser(CommunicateProxy.UserName);
                    Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(CommunicateProxy.UserName), roles);
                }
            }
        }

        /// <summary>
        /// 注册UI Site
        /// </summary>
        private void RegisterUISite()
        {
            string shellAddinfile = ConfigurationManager.AppSettings["ShellAddinfile"];
            if (addInTree != null && File.Exists(shellAddinfile))
            {
                AddIn addIn = new AddIn(shellAddinfile, RootWorkItem);
                addInTree.InsertAddIn(addIn);
            }
        }

        #endregion

        #region Unhandled Exception

        public override void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                MessageBox.Show(BuildExceptionString(ex));
            }
            else
            {
                throw new Exception("应用程序发生错误，无法得到细节信息");
            }
        }

        private string BuildExceptionString(Exception exception)
        {
            string errMessage = string.Empty;

            errMessage += exception.Message + Environment.NewLine + exception.StackTrace;

            while (exception.InnerException != null)
            {
                errMessage += BuildInnerExceptionString(exception.InnerException);
                exception = exception.InnerException;
            }

            return errMessage;
        }

        private string BuildInnerExceptionString(Exception innerException)
        {
            string errMessage = string.Empty;

            errMessage += Environment.NewLine + " InnerException ";
            errMessage += Environment.NewLine + innerException.Message + Environment.NewLine + innerException.StackTrace;

            return errMessage;
        }

        #endregion
    }
}
