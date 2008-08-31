using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraTabbedMdi;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.Services;
using Microsoft.Practices.ObjectBuilder;
using Uniframework.Client;
using Uniframework.Client.OfflineProxy;
using Uniframework.Services;
using Uniframework.SmartClient.Constants;
using Uniframework.XtraForms;
using Uniframework.XtraForms.Workspaces;
using Uniframework.StartUp.Properties;
using Uniframework.Client.ConnectionManagement;
using Uniframework.SmartClient;

namespace Uniframework.StartUp
{
    public partial class ShellForm : DevExpress.XtraEditors.XtraForm
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
            InitialNetworkState();

            IInitializeService initialService = ServiceRepository.Instance.GetService(typeof(IInitializeService)) as IInitializeService;
            UserInfo ui = initialService.GetUserInfo(CommunicateProxy.UserName);
            tlabUser.Caption = ui.DispalyName;
        }

        //[ServiceDependency]
        //public PropertyService PropertyService
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Gets or sets the current login user.
        /// </summary>
        /// <value>The current login user.</value>
        public string CurrentLoginUser
        {
            get { return tlabUser.Caption; }
            set { tlabUser.Caption = value; }
        }

        #region Shell form members

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

        public bool Online
        {
            get { return online; }
            set
            {
                if (online != value)
                {
                    online = value;
                    if (online)
                        ServiceRepository.Instance.GoOnline();
                    else
                        ServiceRepository.Instance.GoOffline();
                }
            }
        }

        #endregion

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
        /// Called when [request queue changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RequestQueueEventArgs"/> instance containing the event data.</param>
        public void OnRequestQueueChanged(object sender, RequestQueueEventArgs e)
        {
            ChangeRequestQueueSize(e.QueueSize);
        }

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
        private void InitialNetworkState()
        {
            if (CommunicateProxy.Ping())
            {
                online = true;
                tbtnNetworkStatus.Glyph = Resources.link;
                tbtnNetworkStatus.Hint = "当前状态 - 在线（双击鼠标强制系统离线）";
            }
            else
            {
                online = false;
                tbtnNetworkStatus.Glyph = Resources.link_delete;
                tbtnNetworkStatus.Hint = "当前状态 - 离线（双击鼠标强制系统在线）";
            }
        }

        /// <summary>
        /// Called when [connection state changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ConnectionStateChangedEventArgs"/> instance containing the event data.</param>
        public void OnConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            online = e.CurrentState == Uniframework.Client.ConnectionState.Online;
            if (e.CurrentState == Uniframework.Client.ConnectionState.Online)
            {
                this.Invoke(new EventHandler(delegate
                {
                    tbtnNetworkStatus.Glyph = Resources.link;
                    tbtnNetworkStatus.Hint = "当前状态 - 在线（双击鼠标强制系统离线）";
                }));

                // 重新注册事件，防止服务器重启丢失挂接的客户端事件
                ClientEventDispatcher.Instance.RereregisterAllEvent();
            }
            else
            {
                this.Invoke(new EventHandler(delegate
                {
                    tbtnNetworkStatus.Glyph = Resources.link_delete;
                    tbtnNetworkStatus.Hint = "当前状态 - 离线（双击鼠标强制系统在线）";
                }));
            }
            if (ConnectionStateChanged != null && (e.CurrentState != e.OriginalState))
                ConnectionStateChanged(e.CurrentState);
        }

        /// <summary>
        /// Handles the Load event of the ShellForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ShellForm_Load(object sender, EventArgs e)
        {
            BarLocalizer.Active = new ChineseXtraBarsCustomizationLocalizer();

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
            Online = Online ? false : true;
        }

        private void ShellForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //PropertyService.Set<Rectangle>("Shell.Bounds", Bounds);
        }

    }
}