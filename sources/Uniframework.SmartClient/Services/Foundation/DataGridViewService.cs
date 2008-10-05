using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI.Commands;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 数据表格视图服务
    /// </summary>
    public class DataGridViewService : IDataGridViewService
    {
        private IDataGridViewHandler activeView = null;
        private Dictionary<object, IDataGridViewHandler> views = new Dictionary<object, IDataGridViewHandler>();

        private WorkItem workItem;
        private IAdapterFactoryCatalog<IDataGridViewHandler> factoryCatalog;
        
        #region Dependency services

        [ServiceDependency]
        public WorkItem WorkItem
        {
            get { return workItem; }
            set {
                workItem = value;
                UpdateCommandStatus();
            }
        }

        [ServiceDependency]
        public IAdapterFactoryCatalog<IDataGridViewHandler> FactoryCatalog
        {
            get { return factoryCatalog; }
            set { factoryCatalog = value; }
        }

        #endregion

        public DataGridViewService()
        {
            Application.Idle += new EventHandler(Application_Idle);
        }


        /// <summary>
        /// Gets or sets the active view.
        /// </summary>
        /// <value>The active view.</value>
        public IDataGridViewHandler ActiveView
        {
            get { return activeView; }
            protected set { 
                activeView = value;
                UpdateCommandStatus();
            }
        }

        #region IDataGridViewService Members

        public void Register(IDataGridViewHandler handler)
        {
            Guard.ArgumentNotNull(handler, "Data grid view handler");
            handler.DataGridActived += new EventHandler(DataGridActived);
            handler.DataGridDeactived += new EventHandler(DataGridDeactived);
        }


        public void Register(object datagrid)
        {
            IDataGridViewHandler handler = FactoryCatalog.GetFactory(datagrid).GetAdapter(datagrid);
            views.Add(datagrid, handler);
            Register(handler);
        }

        public void UnRegister(IDataGridViewHandler handler)
        {
            Guard.ArgumentNotNull(handler, "Data grid view handler");
            handler.DataGridActived -= DataGridActived;
            handler.DataGridDeactived -= DataGridDeactived;
        }

        public void UnRegister(object datagrid)
        {
            if (views.ContainsKey(datagrid)) {
                UnRegister(views[datagrid]);
                views.Remove(datagrid);
            }
        }

        #endregion

        #region Command and Event handler

        [CommandHandler(CommandHandlerNames.CMD_DATAGRID_INSERT)]
        public void OnInsert(object sender, EventArgs e)
        {
            Guard.ArgumentNotNull(ActiveView, "ActiveView");
            ActiveView.Insert();
        }

        [CommandHandler(CommandHandlerNames.CMD_DATAGRID_EDIT)]
        public void OnEdit(object sender, EventArgs e)
        {
            Guard.ArgumentNotNull(ActiveView, "ActiveView");
            ActiveView.Edit();
        }

        [CommandHandler(CommandHandlerNames.CMD_DATAGRID_DELETE)]
        public void OnDelete(object sender, EventArgs e)
        {
            Guard.ArgumentNotNull(ActiveView, "ActiveView");
            ActiveView.Delete();
        }

        [CommandHandler(CommandHandlerNames.CMD_DATAGRID_FILTER)]
        public void OnFilter(object sender, EventArgs e)
        {
            Guard.ArgumentNotNull(ActiveView, "ActiveView");
            ActiveView.Filter();
        }

        [CommandHandler(CommandHandlerNames.CMD_DATAGRID_REFRESH)]
        public void OnRefresh(object sender, EventArgs e)
        {
            Guard.ArgumentNotNull(ActiveView, "ActiveView");
            ActiveView.Refresh();
        }

        [CommandHandler(CommandHandlerNames.CMD_DATAGRID_EXPAND)]
        public void OnExpand(object sender, EventArgs e)
        {
            Guard.ArgumentNotNull(ActiveView, "ActiveView");
            ActiveView.Expand();
        }

        [CommandHandler(CommandHandlerNames.CMD_DATAGRID_COLLAPSE)]
        public void OnCollapse(object sender, EventArgs e)
        {
            Guard.ArgumentNotNull(ActiveView, "ActiveView");
            ActiveView.Collaspe();
        }

        [CommandHandler(CommandHandlerNames.CMD_DATAGRID_SHOWGROUPPANEL)]
        public void OnShowGroupPanel(object sender, EventArgs e)
        {
            Guard.ArgumentNotNull(ActiveView, "ActiveView");
            ActiveView.ShowGroupPanel();
        }

        [CommandHandler(CommandHandlerNames.CMD_DATAGRID_SHOWFOOTER)]
        public void OnShowFooter(object sender, EventArgs e)
        {
            Guard.ArgumentNotNull(ActiveView, "ActiveView");
            ActiveView.ShowFooter();
        }

        [CommandHandler(CommandHandlerNames.CMD_DATAGRID_SETDETAILVIEW)]
        public void OnSetDetailView(object sender, EventArgs e)
        {
            Guard.ArgumentNotNull(ActiveView, "ActiveView");
            ActiveView.SetDetailView();
        }

        [CommandHandler(CommandHandlerNames.CMD_DATAGRID_SETLAYOUTVIEW)]
        public void OnSetLayoutView(object sender, EventArgs e)
        {
            Guard.ArgumentNotNull(ActiveView, "ActiveView");
            ActiveView.SetLayoutView();
        }

        [CommandHandler(CommandHandlerNames.CMD_DATAGRID_SELECTLAYOUTVIEW)]
        public void OnSelectLayoutView(object sender, EventArgs e)
        {
            Guard.ArgumentNotNull(ActiveView, "ActiveView");
            ActiveView.SelectLayoutView();
        }

        [CommandHandler(CommandHandlerNames.CMD_DATAGRID_SETTING)]
        public void OnSetting(object sender, EventArgs e)
        {
            Guard.ArgumentNotNull(ActiveView, "ActiveView");
            ActiveView.Setting();
        }

        #endregion

        #region Assistant functions

        private void Application_Idle(object sender, EventArgs e)
        {
            UpdateCommandStatus();
        }

        private void SetCommandStatus(string command, bool enabled)
        {
            Command cmd = BuilderUtility.GetCommand(WorkItem, command);
            if (cmd != null)
                cmd.Status = (enabled) ? CommandStatus.Enabled : CommandStatus.Disabled;
        }

        private void UpdateCommandStatus()
        {
            bool enabled = ActiveView != null;

            SetCommandStatus(CommandHandlerNames.CMD_DATAGRID_INSERT, enabled && ActiveView.CanInsert);
            SetCommandStatus(CommandHandlerNames.CMD_DATAGRID_EDIT, enabled && ActiveView.CanEdit);
            SetCommandStatus(CommandHandlerNames.CMD_DATAGRID_DELETE, enabled && ActiveView.CanDelete);
            SetCommandStatus(CommandHandlerNames.CMD_DATAGRID_EXPAND, enabled && ActiveView.CanExpand);
            SetCommandStatus(CommandHandlerNames.CMD_DATAGRID_COLLAPSE, enabled && ActiveView.CanCollaspe);
            SetCommandStatus(CommandHandlerNames.CMD_DATAGRID_FILTER, enabled && ActiveView.CanFilter);
            SetCommandStatus(CommandHandlerNames.CMD_DATAGRID_REFRESH, enabled && ActiveView.CanRefresh);
            SetCommandStatus(CommandHandlerNames.CMD_DATAGRID_SETDETAILVIEW, enabled && ActiveView.CanSetDetailView);
            SetCommandStatus(CommandHandlerNames.CMD_DATAGRID_SETLAYOUTVIEW, enabled && ActiveView.CanSetLayoutView);
            SetCommandStatus(CommandHandlerNames.CMD_DATAGRID_SELECTLAYOUTVIEW, enabled && ActiveView.CanSelectLayoutView);
            SetCommandStatus(CommandHandlerNames.CMD_DATAGRID_SHOWGROUPPANEL, enabled && ActiveView.CanShowGroupPanel);
            SetCommandStatus(CommandHandlerNames.CMD_DATAGRID_SHOWFOOTER, enabled && ActiveView.CanShowFooter);
            SetCommandStatus(CommandHandlerNames.CMD_DATAGRID_SETTING, enabled && ActiveView.CanSetting);
        }

        private void DataGridDeactived(object sender, EventArgs e)
        {
            activeView = null;
            UpdateCommandStatus();
        }

        private void DataGridActived(object sender, EventArgs e)
        {
            Microsoft.Practices.CompositeUI.Utility.Guard.TypeIsAssignableFromType(sender.GetType(), typeof(IDataGridViewHandler), "sender");

            activeView = (IDataGridViewHandler)sender;
            UpdateCommandStatus();
        }

        #endregion
    }
}
