using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

using DevExpress.XtraEditors;
using DevExpress.XtraNavBar;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI.Services;
using Microsoft.Practices.CompositeUI.UIElements;
using Microsoft.Practices.CompositeUI.Utility;
using Microsoft.Practices.ObjectBuilder;

using Uniframework.Client;
using Uniframework.Client.ConnectionManagement;
using Uniframework.Client.OfflineProxy;
using Uniframework.Db4o;
using Uniframework.Services;
using Uniframework.SmartClient;
using Uniframework.XtraForms;
using Uniframework.XtraForms.Workspaces;
using Uniframework.SmartClient.WorkItems.Setting;
using Uniframework.StartUp.Strategies;

namespace Uniframework.StartUp
{
    public class ShellApplication : SmartFormApplication<ControlledWorkItem<RootController>, ShellForm>
    {
        private static readonly string IdentityPath = "/Shell/Bar/Standard/Identity";

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
            InitializeShell();

            RootWorkItem.Items.Add(Shell, UIExtensionSiteNames.Shell);

            Program.SetInitialState("加载应用模块……");
            addInTree = new AddInTree();
            RootWorkItem.Services.Add<AddInTree>(addInTree);
            RootWorkItem.Services.Add<IContentMenuService>(new XtraContentMenuService(RootWorkItem, Shell.barManager));
            
            RootWorkItem.Items.Add(Shell.BarManager, UIExtensionSiteNames.Shell_Bar_Manager);
            RootWorkItem.Items.Add(Shell.BarManager.MainMenu, UIExtensionSiteNames.Shell_Bar_Mainmenu);
            RootWorkItem.Items.Add(Shell.BarManager.StatusBar, UIExtensionSiteNames.Shell_Bar_Status);
            RootWorkItem.Items.Add(Shell.BarManager, UIExtensionSiteNames.Shell_Manager_BarManager);
            RootWorkItem.Items.Add(Shell.DockManager, UIExtensionSiteNames.Shell_Manager_DockManager);
            RootWorkItem.Items.Add(Shell.TabbedMdiManager, UIExtensionSiteNames.Shell_Manager_TabbedMdiManager);
            RootWorkItem.Items.Add(Shell.NaviWorkspace, UIExtensionSiteNames.Shell_NaviPane_Navibar);

            RootWorkItem.Workspaces.Add(new MdiWorkspace(Shell), UIExtensionSiteNames.Shell_Workspace_Main);
            RootWorkItem.Workspaces.Add(Shell.DockWorkspace, UIExtensionSiteNames.Shell_Workspace_Dockable);
            RootWorkItem.Workspaces.Add(Shell.NaviWorkspace, UIExtensionSiteNames.Shell_Workspace_NaviPane);
            RootWorkItem.Workspaces.Add(new XtraWindowWorkspace(Shell), UIExtensionSiteNames.Shell_Workspace_Window);

            Program.SetInitialState("正在初始化用户使用界面……");
            RegisterCommandHandler();
            RegisterViews();
            RegisterUIElements();
            RegisterUISite(); // 构建用户界面并添加UI构建服务

            Program.CloseLoginForm();
        }

        /// <summary>
        /// See <see cref="CabApplication{TWorkItem}.AddServices"/>
        /// </summary>
        protected override void AddServices()
        {
            base.AddServices();

            
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

            builder.Strategies.AddNew<EventConnectStrategy>(BuilderStage.Initialization); // 添加远程事件连接策略
        }

        #region Assistant functions

        /// <summary>
        /// 初始化框架系统环境
        /// </summary>
        private void InitializeEnvironment()
        {
            RemoteServiceRegister register = new RemoteServiceRegister();
            Program.Logger.Debug("将远程服务注入到本地容器中");
            Program.SetInitialState("检查远程服务……");
            Program.IncreaseProgressBar(10);
            register.RegisterRemoteServices(RootWorkItem, ServiceRepository.Instance);
            RegisterUserInfo(); // 注册用户信息需要放在其它服务启动之前

            //// 注册系统权限管理服务
            Program.SetInitialState("注册系统权限管理服务……");
            Program.Logger.Debug("加载系统权限管理服务");
            Program.IncreaseProgressBar(5);
            IAuthorizationService authorizationService = new AuthorizationService(RootWorkItem);
            RootWorkItem.Services.Add<IAuthorizationService>(authorizationService);

            // 注册自定义的UI组件
            Program.SetInitialState("注册自定义UI组件构建服务……");
            Program.IncreaseProgressBar(10);
            RootWorkItem.Services.Add<SmartClientEnvironment>(new SmartClientEnvironment());
            RootWorkItem.Services.Add<IBuilderService>(new BuilderService(RootWorkItem));

            // 添加系统自定义的默认服务
            Program.SetInitialState("注册本地默认服务……");
            Program.IncreaseProgressBar(10);
            RootWorkItem.Services.Add(typeof(log4net.ILog), Program.Logger);

            RootWorkItem.Services.Remove<IModuleEnumerator>(); // 删除系统默认的模块枚举服务

            // 从服务器下载配置模块信息
            Program.SetInitialState("从服务器下载客户端配置信息……");
            Program.IncreaseProgressBar(10);
            WebServiceModuleEnumerator ws = new WebServiceModuleEnumerator();
            ws.Load();

            IModuleLoaderService mls = RootWorkItem.Services.Get<IModuleLoaderService>();
            if (mls != null) {
                mls.ModuleLoaded += new EventHandler<DataEventArgs<LoadedModuleInfo>>(ModuleLoaderService_ModuleLoaded);
            }

            RootWorkItem.Services.Add<IModuleEnumerator>(ws);
            Program.IncreaseProgressBar(10);
        }

        /// <summary>
        /// 解释加载插件描述文件
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Practices.CompositeUI.Utility.DataEventArgs&lt;Microsoft.Practices.CompositeUI.Services.LoadedModuleInfo&gt;"/> instance containing the event data.</param>
        private void ModuleLoaderService_ModuleLoaded(object sender, DataEventArgs<LoadedModuleInfo> e)
        {
            string addInfile = e.Data.Name.Split(',')[0] + ".addin";
            if (File.Exists(addInfile)) {
                AddIn addIn = new AddIn(addInfile, RootWorkItem);
                addInTree.InsertAddIn(addIn);
            }
            else {
                List<string> files = FileUtility.SearchDirectory(FileUtility.ConvertToFullPath(@"..\AddIns\"), addInfile);
                if (files.Count > 0) {
                    AddIn addIn = new AddIn(files[0], RootWorkItem);
                    addInTree.InsertAddIn(addIn);
                }
            }
            Program.Logger.Info(String.Format("完成加载应用系统插件 \"{0}\"", e.Data.Name.Split(',')[0] + ".dll"));
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
                    IStringService strService = RootWorkItem.Services.Get<IStringService>();
                    if (strService != null)
                        strService.Register("CurrentUser", CommunicateProxy.UserName);
                }
                // 为环境变量赋值
                SmartClientEnvironment scEnvironment = RootWorkItem.Services.Get<SmartClientEnvironment>();
                if (scEnvironment != null)
                {
                    scEnvironment.CurrentUser = user;
                }
            }
        }

        /// <summary>
        /// Initializes the shell.
        /// </summary>
        private void InitializeShell()
        {
            ServiceRepository.Instance.RequestQueueChanged += new RequestQueueChangedEventHandler(Shell.OnRequestQueueChanged);
            ServiceRepository.Instance.ConnectionStateChanged += new ConnectionStateChangedEventHandler(Shell.OnConnectionStateChanged);

            Shell.Text = ConfigurationManager.AppSettings["ShellCaption"];
            string iconFile = ConfigurationManager.AppSettings["ShellIcon"];
            iconFile = File.Exists(iconFile) ? iconFile : Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory) + @"\Resources\" + iconFile;
            if (File.Exists(iconFile))
                Shell.Icon = new Icon(iconFile);

            // 注册系统主菜单及状态栏
            RootWorkItem.UIExtensionSites.RegisterSite(UIExtensionSiteNames.Shell_Bar_Manager, Shell.BarManager);
            RootWorkItem.UIExtensionSites.RegisterSite(UIExtensionSiteNames.Shell_Bar_Mainmenu, Shell.BarManager.MainMenu);
            RootWorkItem.UIExtensionSites.RegisterSite(UIExtensionSiteNames.Shell_Bar_Status, Shell.BarManager.StatusBar);
        }

        /// <summary>
        /// 注册系统默认的命令处理器
        /// </summary>
        private void RegisterCommandHandler()
        {
            RootWorkItem.Items.AddNew<CommandHandlers>("DefaultCommandHandler");

            RootWorkItem.Services.AddNew<EditableService, IEditableService>();
            RootWorkItem.Services.AddNew<DocumentService, IDocumentService>();
            RootWorkItem.Services.AddNew<PrintableService, IPrintableService>();
            RootWorkItem.Services.AddNew<DataListViewService, IDataListViewService>();
        }

        /// <summary>
        /// 注册框架默认的视图
        /// </summary>
        private void RegisterViews()
        {
            SettingView view = RootWorkItem.SmartParts.AddNew<SettingView>(SmartPartNames.SmartPart_Shell_SettingView);
            RootWorkItem.Workspaces.Add(view.SettingDeckWorkspace, UIExtensionSiteNames.Shell_Workspace_SettingDeck);
            RootWorkItem.Workspaces.Add(view.SettingNaviWorkspace, UIExtensionSiteNames.Shell_Workspace_SettingNavi);
            RootWorkItem.UIExtensionSites.RegisterSite(UIExtensionSiteNames.Shell_UI_NaviPane_DefaultSetting, view.DefaultSettingGroup);

            ShellLayoutSettingView layoutView = RootWorkItem.SmartParts.AddNew<ShellLayoutSettingView>(SmartPartNames.SmartPart_Shell_LayoutSettingView);
            ISettingService settingService = RootWorkItem.Services.Get<ISettingService>();
            if (settingService != null)
                settingService.RegisterSetting(layoutView);
        }

        /// <summary>
        /// 注册系统默认的UI组件
        /// </summary>
        private void RegisterUIElements()
        {
            IImageService imageService = RootWorkItem.Services.Get<IImageService>();
            if (imageService != null) {
                NavBarItem item = new NavBarItem("系统外观");
                item.LargeImage = imageService.GetBitmap("cubes", new System.Drawing.Size(32, 32));
                item.SmallImage = imageService.GetBitmap("cubes", new System.Drawing.Size(16, 16));
                RootWorkItem.UIExtensionSites[UIExtensionSiteNames.Shell_UI_NaviPane_DefaultSetting].Add(item);
                Microsoft.Practices.CompositeUI.Commands.Command cmd = RootWorkItem.Commands[CommandHandlerNames.CMD_SHOW_LAYOUTSETTING];
                if (cmd != null)
                    cmd.AddInvoker(item, "LinkClicked");
                cmd.Execute();
            }
        }

        /// <summary>
        /// 注册UI Site
        /// </summary>
        private void RegisterUISite()
        {
            string shellAddinfile = ConfigurationManager.AppSettings["ShellAddinfile"];
            if (addInTree != null && File.Exists(shellAddinfile)) {
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
                XtraMessageBox.Show(BuildExceptionString(ex));
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
