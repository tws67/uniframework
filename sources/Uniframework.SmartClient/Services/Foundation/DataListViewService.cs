using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.Common;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 数据列表服务
    /// </summary>
    public class DataListViewService : IDataListViewService
    {
        private IDataListHandler activeView = null;
        private Dictionary<object, IDataListHandler> views = new Dictionary<object, IDataListHandler>();

        private WorkItem workItem;
        private IAdapterFactoryCatalog<IDataListHandler> factoryCatalog;
        
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
        public IAdapterFactoryCatalog<IDataListHandler> FactoryCatalog
        {
            get { return factoryCatalog; }
            set { factoryCatalog = value; }
        }

        #endregion

        public DataListViewService()
        {
            Application.Idle += new EventHandler(Application_Idle);
        }


        /// <summary>
        /// Gets or sets the active view.
        /// </summary>
        /// <value>The active view.</value>
        public IDataListHandler ActiveView
        {
            get { return activeView; }
            protected set { 
                activeView = value;
                UpdateCommandStatus();
            }
        }

        #region IDataGridViewService Members

        public void Register(IDataListHandler handler)
        {
            Guard.ArgumentNotNull(handler, "Data grid view handler");
            handler.Enter += new EventHandler(OnLeave);
            handler.Leave += new EventHandler(OnEnter);
        }


        public void Register(object dataList)
        {
            IDataListHandler handler = FactoryCatalog.GetFactory(dataList).GetAdapter(dataList);
            views.Add(dataList, handler);
            Register(handler);
        }

        public void UnRegister(IDataListHandler handler)
        {
            Guard.ArgumentNotNull(handler, "Data grid view handler");
            handler.Enter -= OnLeave;
            handler.Leave -= OnEnter;
        }

        public void UnRegister(object dataList)
        {
            if (views.ContainsKey(dataList)) {
                UnRegister(views[dataList]);
                views.Remove(dataList);
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

        [CommandHandler(CommandHandlerNames.CMD_DATAGRID_REFRESH)]
        public void OnRefresh(object sender, EventArgs e)
        {
            Guard.ArgumentNotNull(ActiveView, "ActiveView");
            ActiveView.RefreshDataSource();
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
            SetCommandStatus(CommandHandlerNames.CMD_DATAGRID_REFRESH, enabled && ActiveView.CanRefreshDataSource);
        }

        private void OnEnter(object sender, EventArgs e)
        {
            activeView = null;
            UpdateCommandStatus();
        }

        private void OnLeave(object sender, EventArgs e)
        {
            Microsoft.Practices.CompositeUI.Utility.Guard.TypeIsAssignableFromType(sender.GetType(), typeof(IDataListHandler), "sender");

            activeView = (IDataListHandler)sender;
            UpdateCommandStatus();
        }

        #endregion
    }
}
