using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraTabbedMdi;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.Services;
using Microsoft.Practices.ObjectBuilder;
using Uniframework.Client;
using Uniframework.SmartClient;
using Uniframework.XtraForms;
using Uniframework.XtraForms.Workspaces;

namespace Uniframework.LocalStartUp
{
    public partial class ShellForm : DevExpress.XtraEditors.XtraForm, ISmartClient
    {
        private bool online; // 与服务器的连接状态
        private readonly WorkItem workItem;
        private IWorkItemTypeCatalogService workItemTypeCatalog;
        private readonly DockManagerWorkspace dockManagerWorkspace;
        public event ConnectionStateChangeHandler ConnectionStateChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellForm"/> class.
        /// </summary>
        public ShellForm()
        {
            InitializeComponent();

            tlabStatus.Caption = String.Empty;
            barManager.ForceInitialize();
            dockManagerWorkspace = new DockManagerWorkspace(DockManager);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellForm"/> class.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <param name="workItemTypeCatalog">The work item type catalog.</param>
        [InjectionConstructor]
        public ShellForm([ServiceDependency]WorkItem workItem, IWorkItemTypeCatalogService workItemTypeCatalog)
            : this()
        {
            this.workItem = workItem;
            this.workItemTypeCatalog = workItemTypeCatalog;
            this.workItem.Services.Add<ISmartClient>(this); // 添加智能客户端服务
            //InitialNetworkState();

            //IInitializeService initialService = ServiceRepository.Instance.GetService(typeof(IInitializeService)) as IInitializeService;
        }

        #region Shell form members

        #region Dependency services

        [ServiceDependency]
        public IPropertyService PropertyService
        {
            get;
            set;
        }

        #endregion

        protected WorkItem WorkItem
        {
            get { return workItem; }
        }

        /// <summary>
        /// Called when [request queue changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RequestQueueEventArgs"/> instance containing the event data.</param>
        //public void OnRequestQueueChanged(object sender, RequestQueueEventArgs e)
        //{
        //    ChangeRequestQueueSize(e.QueueSize);
        //}

        /// <summary>
        /// Called when [connection state changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ConnectionStateChangedEventArgs"/> instance containing the event data.</param>
        //public void OnConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        //{
        //    online = e.CurrentState == Uniframework.Client.ConnectionState.Online;
        //    if (e.CurrentState == Uniframework.Client.ConnectionState.Online)
        //    {
        //        this.Invoke(new EventHandler(delegate
        //        {
        //            tbtnNetworkStatus.Glyph = Resources.link;
        //            tbtnNetworkStatus.Hint = "当前状态 - 在线（双击鼠标强制系统离线）";
        //        }));

        //        // 重新注册事件，防止服务器重启丢失挂接的客户端事件
        //        ClientEventDispatcher.Instance.RereregisterAllEvent();
        //    }
        //    else
        //    {
        //        this.Invoke(new EventHandler(delegate
        //        {
        //            tbtnNetworkStatus.Glyph = Resources.link_delete;
        //            tbtnNetworkStatus.Hint = "当前状态 - 离线（双击鼠标强制系统在线）";
        //        }));
        //    }
        //    if (ConnectionStateChanged != null && (e.CurrentState != e.OriginalState))
        //        ConnectionStateChanged(e.CurrentState);
        //}

        public BarManager BarManager
        {
            get { return barManager; }
        }

        public XtraNavBarWorkspace NaviWorkspace
        {
            get { return naviWorkspace; }
        }

        public DockManagerWorkspace DockWorkspace
        {
            get { return dockManagerWorkspace; }
        }

        public XtraTabbedMdiManager TabbedMdiManager
        {
            get { return tabbedMdiManager; }
        }

        //public bool Online
        //{
        //    get { return online; }
        //    set
        //    {
        //        if (online != value)
        //        {
        //            online = value;
        //            if (online)
        //                ServiceRepository.Instance.GoOnline();
        //            else
        //                ServiceRepository.Instance.GoOffline();
        //        }
        //    }
        //}

        #endregion

        #region Event brokers

        /// <summary>
        /// Called when [status updated].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Uniframework.EventArgs&lt;System.String&gt;"/> instance containing the event data.</param>
        [EventSubscription(EventNames.Shell_StatusUpdated, Thread = ThreadOption.UserInterface)]
        public void OnStatusUpdated(object sender, EventArgs<String> e)
        {
            tlabStatus.Caption = e.Data;
        }

        /// <summary>
        /// Called when [custom panel1 updated].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Uniframework.EventArgs&lt;System.String&gt;"/> instance containing the event data.</param>
        [EventSubscription(EventNames.Shell_CustomPanel1Updated, Thread = ThreadOption.UserInterface)]
        public void OnCustomPanel1Updated(object sender, EventArgs<String> e)
        {
            tlabCustomPanel1.Visibility = String.IsNullOrEmpty(e.Data) ? BarItemVisibility.Never : BarItemVisibility.Always;
            tlabCustomPanel1.Caption = e.Data;
        }

        /// <summary>
        /// Called when [custom panel2 updated].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Uniframework.EventArgs&lt;System.String&gt;"/> instance containing the event data.</param>
        [EventSubscription(EventNames.Shell_CustomPanel2Updated, Thread = ThreadOption.UserInterface)]
        public void OnCustomPanel2Updated(object sender, EventArgs<String> e)
        {
            tlabCustomPanel2.Visibility = String.IsNullOrEmpty(e.Data) ? BarItemVisibility.Never : BarItemVisibility.Always;
            tlabCustomPanel2.Caption = e.Data;
        }

        /// <summary>
        /// Called when [status progress changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Uniframework.EventArgs&lt;System.Int32&gt;"/> instance containing the event data.</param>
        [EventSubscription(EventNames.Shell_ProgressBarChanged, Thread = ThreadOption.UserInterface)]
        public void OnStatusProgressChanged(object sender, EventArgs<int> e)
        {
            ProgressBar.Visibility = (e.Data > 0 && e.Data < 100) ? BarItemVisibility.Always : BarItemVisibility.Never;
            ProgressBar.EditValue = e.Data;
        }

        /// <summary>
        /// 系统外壳关闭事件
        /// </summary>
        [EventPublication(EventNames.Shell_ShellClosing, PublicationScope.Global)]
        public event EventHandler<CancelEventArgs> ShellClosing;

        #endregion

        #region ISmartClient Members

        /// <summary>
        /// 显示状态栏的帮助信息.
        /// </summary>
        /// <param name="info">信息</param>
        public void ShowHint(string info)
        {
            tlabStatus.Caption = info;
        }

        /// <summary>
        /// 显示自定义面板1信息.
        /// </summary>
        /// <param name="info">信息</param>
        public void ShowCustomPanel1(string info)
        {
            tlabCustomPanel1.Visibility = String.IsNullOrEmpty(info) ? BarItemVisibility.Never : BarItemVisibility.Always;
            tlabCustomPanel1.Caption = info;
        }

        /// <summary>
        /// 显示自定义面板2信息
        /// </summary>
        /// <param name="info">信息</param>
        public void ShowCustomPanel2(string info)
        {
            tlabCustomPanel2.Visibility = String.IsNullOrEmpty(info) ? BarItemVisibility.Never : BarItemVisibility.Always;
            tlabCustomPanel2.Caption = info;
        }

        /// <summary>
        /// 显示进度条
        /// </summary>
        /// <param name="position">进度</param>
        public void ChangeProgress(int position)
        {
            ProgressBar.Visibility = (position <= 0) && (position > 100) ? BarItemVisibility.Never : BarItemVisibility.Always;
            ProgressBar.EditValue = position;
        }

        #endregion

        #region Assistant functions

        /// <summary>
        /// Changes the size of the request queue.
        /// </summary>
        /// <param name="queueSize">Size of the queue.</param>
        private void ChangeRequestQueueSize(int queueSize)
        {
            this.Invoke(new EventHandler(delegate
            {
                this.tlabRequestSize.Caption = String.Format("指令队列({0})", queueSize);
            }));
        }

        /// <summary>
        /// Initials the state of the network.
        /// </summary>
        //private void InitialNetworkState()
        //{
        //    if (CommunicateProxy.Ping())
        //    {
        //        online = true;
        //        tbtnNetworkStatus.Glyph = Resources.link;
        //        tbtnNetworkStatus.Hint = "当前状态 - 在线（双击鼠标强制系统离线）";
        //    }
        //    else
        //    {
        //        online = false;
        //        tbtnNetworkStatus.Glyph = Resources.link_delete;
        //        tbtnNetworkStatus.Hint = "当前状态 - 离线（双击鼠标强制系统在线）";
        //    }
        //}

        /// <summary>
        /// Stores the configuration.
        /// </summary>
        private void StoreConfiguration()
        {
            ShellLayout layout = new ShellLayout
            {
                Location = this.Location,
                Size = this.Size,
                WindowState = this.WindowState,
                NavPaneState = NaviWorkspace.OptionsNavPane.NavPaneState,
                NavPaintStyleName = NaviWorkspace.PaintStyleName,
                DefaultSkin = UserLookAndFeel.Default.ActiveSkinName,
                ShowStatusBar = StatusBar.Visible,
                WindowLayoutMode = TabbedMdiManager.MdiParent == null ? WindowLayoutMode.Windowed : WindowLayoutMode.Tabbed
            };
            PropertyService.Set<ShellLayout>(UIExtensionSiteNames.Shell_Property_ShellLayout, layout);
        }

        /// <summary>
        /// Restores the configuration.
        /// </summary>
        private void RestoreConfiguration()
        {
            // 设置窗口位置
            ShellLayout layout = PropertyService.Get<ShellLayout>(UIExtensionSiteNames.Shell_Property_ShellLayout, null);
            if (layout != null)
            {
                StartPosition = FormStartPosition.WindowsDefaultLocation;
                this.Location = layout.Location;
                this.Size = layout.Size;
                this.WindowState = layout.WindowState;
                this.NaviWorkspace.PaintStyleName = layout.NavPaintStyleName;
                this.NaviWorkspace.OptionsNavPane.NavPaneState = layout.NavPaneState;
                this.StatusBar.Visible = layout.ShowStatusBar;
                this.TabbedMdiManager.MdiParent = layout.WindowLayoutMode == WindowLayoutMode.Tabbed ? this : null;
                UserLookAndFeel.Default.SetSkinStyle(layout.DefaultSkin);
            }
        }

        /// <summary>
        /// Handles the Load event of the ShellForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ShellForm_Load(object sender, EventArgs e)
        {
            BarLocalizer.Active = new ChineseXtraBarsCustomizationLocalizer();
            RestoreConfiguration();

            base.Activate();
            base.BringToFront();
        }

        /// <summary>
        /// 强制系统当前在线或离线
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DevExpress.XtraBars.ItemClickEventArgs"/> instance containing the event data.</param>
        private void tbtnNetworkStatus_ItemDoubleClick(object sender, ItemClickEventArgs e)
        {
            //Online = Online ? false : true;
        }

        /// <summary>
        /// Handles the FormClosed event of the ShellForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosedEventArgs"/> instance containing the event data.</param>
        private void ShellForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            StoreConfiguration();
        }

        /// <summary>
        /// Handles the FormClosing event of the ShellForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
        private void ShellForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ShellClosing != null)
            {
                CancelEventArgs eventArgs = new CancelEventArgs();
                ShellClosing(this, eventArgs);
                e.Cancel = eventArgs.Cancel;
            }
        }

        #endregion
    }
}