using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.UserSkins;
using DevExpress.XtraEditors;
using DevExpress.XtraNavBar;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI.Services;
using Microsoft.Practices.CompositeUI.Utility;
using Microsoft.Practices.ObjectBuilder;
using Uniframework.SmartClient;
using Uniframework.SmartClient.WorkItems.Setting;
using Uniframework.XtraForms.Workspaces;


namespace Uniframework.LocalStartUp
{
    public class ShellApplication : SmartFormApplication<WorkItem, ShellForm>
    {
        private static readonly string IdentityPath = "/Shell/Bar/Standard/Identity";

        private AddInTree addInTree;

        [STAThread]
        static void Main() {
            BonusSkins.Register();
            OfficeSkins.Register();
            SkinManager.EnableFormSkins();
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("zh-CN");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            new ShellApplication().Run();
        }

        /// <summary>
        /// May be overridden in derived classes to perform activities just before the shell
        /// is created.
        /// </summary>
        protected override void BeforeShellCreated()
        {
            base.BeforeShellCreated();
        }

        /// <summary>
        /// See <see cref="CabShellApplication{T,S}.AfterShellCreated"/>
        /// </summary>
        protected override void AfterShellCreated()
        {
            base.AfterShellCreated();
            InitializeShell();

            RootWorkItem.Items.Add(Shell, UIExtensionSiteNames.Shell);

            //Program.SetInitialState("加载应用模块……");
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

            //Program.SetInitialState("正在初始化用户使用界面……");
            RegisterCommandHandler();
            RegisterViews();
            RegisterUIElements();
            RegisterUISite(); // 构建用户界面并添加UI构建服务
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
            string libPath = FileUtility.ConvertToFullPath(@"..\Libraries\");
            if (File.Exists(Path.Combine(libPath, asmFile)))
                return Assembly.LoadFile(Path.Combine(libPath, asmFile));

            List<string> files = FileUtility.SearchDirectory(FileUtility.ConvertToFullPath(@"..\"), asmFile);
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
            throw new FileNotFoundException("系统找不到指定的程序集文件", asmFile);
        }

        /// <summary>
        /// 解释加载插件描述文件
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Practices.CompositeUI.Utility.DataEventArgs&lt;Microsoft.Practices.CompositeUI.Services.LoadedModuleInfo&gt;"/> instance containing the event data.</param>
        private void ModuleLoaderService_ModuleLoaded(object sender, DataEventArgs<LoadedModuleInfo> e)
        {
            string addInfile = e.Data.Name.Split(',')[0] + ".addin";
            if (File.Exists(addInfile))
            {
                AddIn addIn = new AddIn(addInfile, RootWorkItem);
                addInTree.InsertAddIn(addIn);
            }
            else
            {
                List<string> files = FileUtility.SearchDirectory(FileUtility.ConvertToFullPath(@"..\AddIns\"), addInfile);
                if (files.Count > 0)
                {
                    AddIn addIn = new AddIn(files[0], RootWorkItem);
                    addInTree.InsertAddIn(addIn);
                }
            }
            //Program.Logger.Info(String.Format("完成加载应用系统插件 \"{0}\"", e.Data.Name.Split(',')[0] + ".dll"));
        }

        /// <summary>
        /// Initializes the shell.
        /// </summary>
        private void InitializeShell()
        {
            //ServiceRepository.Instance.RequestQueueChanged += new RequestQueueChangedEventHandler(Shell.OnRequestQueueChanged);
            //ServiceRepository.Instance.ConnectionStateChanged += new ConnectionStateChangedEventHandler(Shell.OnConnectionStateChanged);

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
            if (imageService != null)
            {
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
