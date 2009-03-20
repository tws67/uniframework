using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI.Services;
using Uniframework.Security;
using log4net;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 数据列表视图服务
    /// </summary>
    public class DataListViewService : IDataListViewService
    {
        private IDataListHandler handler = null;
        private Dictionary<object, IDataListView> views = new Dictionary<object, IDataListView>();

        private WorkItem workItem;
        private ILog logger;
        private IAdapterFactoryCatalog<IDataListView> factoryCatalog;
        
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
        public IAdapterFactoryCatalog<IDataListView> FactoryCatalog
        {
            get { return factoryCatalog; }
            set { factoryCatalog = value; }
        }

        [ServiceDependency]
        public IAuthorizationService AuthorizationService
        {
            get;
            set;
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
        public IDataListHandler Handler
        {
            get { return handler; }
            protected set { 
                handler = value;
                UpdateCommandStatus();
            }
        }

        #region IDataGridViewService Members

        public void Register(IDataListView handler)
        {
            Guard.ArgumentNotNull(handler, "Data grid view handler");
            handler.Enter += new EventHandler(OnEnter);
            handler.Leave += new EventHandler(OnLeave);
        }


        public void Register(object dataList)
        {
            IDataListView handler = FactoryCatalog.GetFactory(dataList).GetAdapter(dataList);
            views.Add(dataList, handler);
            Register(handler);
        }

        public void UnRegister(IDataListView handler)
        {
            Guard.ArgumentNotNull(handler, "Data grid view handler");
            handler.Enter -= OnEnter;
            handler.Leave -= OnLeave;
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
            Guard.ArgumentNotNull(Handler, "Handler");
            Handler.Insert();
        }

        [CommandHandler(CommandHandlerNames.CMD_DATAGRID_EDIT)]
        public void OnEdit(object sender, EventArgs e)
        {
            Guard.ArgumentNotNull(Handler, "Handler");
            Handler.Edit();
        }

        [CommandHandler(CommandHandlerNames.CMD_DATAGRID_DELETE)]
        public void OnDelete(object sender, EventArgs e)
        {
            Guard.ArgumentNotNull(Handler, "Handler");
            Handler.Delete();
        }

        [CommandHandler(CommandHandlerNames.CMD_DATAGRID_REFRESH)]
        public void OnRefresh(object sender, EventArgs e)
        {
            Guard.ArgumentNotNull(Handler, "Handler");
            Handler.RefreshDataSource();
        }

        [CommandHandler(CommandHandlerNames.CMD_DATAGRID_EXPAND)]
        public void OnExpand(object sender, EventArgs e)
        {
            Guard.ArgumentNotNull(Handler, "Handler");
            Handler.Expand();
        }

        [CommandHandler(CommandHandlerNames.CMD_DATAGRID_COLLAPSE)]
        public void OnCollapse(object sender, EventArgs e)
        {
            Guard.ArgumentNotNull(Handler, "Handler");
            Handler.Collaspe();
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

        /// <summary>
        /// 更新操作的状态
        /// </summary>
        private void UpdateCommandStatus()
        {
            bool enabled = Handler != null;

            SetCommandStatus(CommandHandlerNames.CMD_DATAGRID_INSERT, enabled && Handler.CanInsert 
                && CanExecute(CommandHandlerNames.CMD_DATAGRID_INSERT));
            SetCommandStatus(CommandHandlerNames.CMD_DATAGRID_EDIT, enabled && Handler.CanEdit
                && CanExecute(CommandHandlerNames.CMD_DATAGRID_EDIT));
            SetCommandStatus(CommandHandlerNames.CMD_DATAGRID_DELETE, enabled && Handler.CanDelete
                && CanExecute(CommandHandlerNames.CMD_DATAGRID_DELETE));
            SetCommandStatus(CommandHandlerNames.CMD_DATAGRID_EXPAND, enabled && Handler.CanExpand
                && CanExecute(CommandHandlerNames.CMD_DATAGRID_EXPAND));
            SetCommandStatus(CommandHandlerNames.CMD_DATAGRID_COLLAPSE, enabled && Handler.CanCollaspe
                && CanExecute(CommandHandlerNames.CMD_DATAGRID_COLLAPSE));
            SetCommandStatus(CommandHandlerNames.CMD_DATAGRID_REFRESH, enabled && Handler.CanRefreshDataSource
                && CanExecute(CommandHandlerNames.CMD_DATAGRID_REFRESH));
        }

        private void OnEnter(object sender, EventArgs e)
        {
            Microsoft.Practices.CompositeUI.Utility.Guard.TypeIsAssignableFromType(sender.GetType(), typeof(IDataListView), "sender");

            // 获得当前视图的控制器
            handler = null;
            IDataListView view = sender as IDataListView;
            if (view != null)
                handler = view.DataListHandler;
            UpdateCommandStatus(); // 更新相关命令项的状态
        }

        private void OnLeave(object sender, EventArgs e)
        {
            handler = null;
            UpdateCommandStatus();
        }

        /// <summary>
        /// 检查当前用户是否可以执行指定的操作
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can execute the specified command; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecute(string command)
        {
            //logger = WorkItem.Services.Get<ILog>();
            string authorizationUri = GetAuthorizationPath();
            string actionKey = SecurityUtility.HashObject(authorizationUri + command);
            //if (logger != null) {
            //    logger.Debug("Authorization Path: " + authorizationUri + " , Command: " + command);
            //    logger.Debug("Action Key:" + actionKey);
            //}
            return AuthorizationService.CanExecute(actionKey);
        }

        /// <summary>
        /// 获取当前处理器的授权路径
        /// </summary>
        /// <returns>如果处理器定义了<see cref="AuthorizationAttribute"/>属性则返回其授权路径，否则为空</returns>
        private string GetAuthorizationPath()
        {
            String authPath = String.Empty;
            if (handler != null) {
                AuthorizationAttribute[] attrs = (AuthorizationAttribute[])handler.GetType().GetCustomAttributes(typeof(AuthorizationAttribute), true);
                if (attrs.Length > 0)
                    authPath = attrs[0].AuthorizationUri;
            }
            return authPath;
        }

        #endregion
    }
}
