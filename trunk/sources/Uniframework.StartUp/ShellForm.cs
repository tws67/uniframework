using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DevExpress.XtraEditors;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.Services;
using Microsoft.Practices.ObjectBuilder;

using Uniframework.SmartClient.Constants;
using Uniframework.XtraForms.Workspaces;

namespace Uniframework.StartUp
{
    public partial class ShellForm : DevExpress.XtraEditors.XtraForm
    {
        private readonly WorkItem workItem;
        private IWorkItemTypeCatalogService workItemTypeCatalog;
        private readonly DockManagerWorkspace dockManagerWorkspace;

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
        }

        #region Shell form members

        public XtraNavBarWorkspace NaviWorkspace
        {
            get { return naviWorkspace; }
        }

        public DockManagerWorkspace DockWorkspace
        {
            get { return dockManagerWorkspace; }
        }

        #endregion

        /// <summary>
        /// Called when [status updated].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Uniframework.EventArgs&lt;System.String&gt;"/> instance containing the event data.</param>
        [EventSubscription(EventNames.StatusUpdate, Thread = ThreadOption.UserInterface)]
        public void OnStatusUpdated(object sender, EventArgs<String> e)
        {
            tlabStatus.Caption = e.Data;
        }

        /// <summary>
        /// Called when [status progress changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Uniframework.EventArgs&lt;System.Int32&gt;"/> instance containing the event data.</param>
        [EventSubscription(EventNames.StatusProgressChanged, Thread = ThreadOption.UserInterface)]
        public void OnStatusProgressChanged(object sender, EventArgs<int> e)
        {
            ProgressBar.EditValue = e.Data;
        }
    }
}