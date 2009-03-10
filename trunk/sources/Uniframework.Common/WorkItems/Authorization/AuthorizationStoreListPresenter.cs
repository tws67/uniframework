using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Nodes;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.EventBroker;
using Uniframework.Client;
using Uniframework.Security;
using Uniframework.SmartClient;
using Uniframework.XtraForms.SmartPartInfos;
using Microsoft.Practices.CompositeUI.SmartParts;

namespace Uniframework.Common.WorkItems.Authorization
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthorizationStoreListPresenter : DataListPresenter<AuthorizationStoreListView>
    {
        private readonly string AuthNodeForm = "AuthNodeForm";
        private readonly string RootPath = "Shell";
        private readonly string AuthCommandView = "AuthCommandView";
        private readonly string SelectCommandForm = "SelectCommandForm";

        #region Dependency Services

        /// <summary>
        /// Gets or sets the authorization store service.
        /// </summary>
        /// <value>The authorization store service.</value>
        [ServiceDependency]
        public IAuthorizationStoreService AuthorizationStoreService
        {
            get;
            set;
        }

        [ServiceDependency]
        public IAuthorizationCommandService AuthorizationCommandService
        {
            get;
            set;
        }

        [ServiceDependency]
        public IAuthorizationNodeService AuthorizationNodeService
        {
            get;
            set;
        }

        [ServiceDependency]
        public ISmartClient SmartClient
        {
            get;
            set;
        }

        #endregion

        public AuthorizationStoreListPresenter()
        {
            Application.Idle += new EventHandler(Application_Idle);
        }


        /// <summary>
        /// 视图准备好方法用于在Presenter中初始化视图
        /// </summary>
        public override void OnViewReady()
        {
            base.OnViewReady();
            Initilize();
        }
        /// <summary>
        /// 初始化数据列表操作只在数据列表第一次加载时使用
        /// </summary>
        public override void Initilize()
        {
            base.Initilize();
            View.LoadAuthorizationsNodes();
        }

        /// <summary>
        /// 获取一个值决定当前可否插入新的数据资料
        /// </summary>
        /// <value>返回<c>true</c>如果可以插入的话; 否则为, <c>false</c>.</value>
        public override bool CanInsert
        {
            get
            {
                bool flag = base.CanInsert;
                flag &= CurrentAuthNode != null && ((AuthorizationNode)CurrentAuthNode.Tag).Id != GlobalConstants.Uri_Separator + RootPath;
                return flag;
            }
        }

        /// <summary>
        /// 插入新的数据资料
        /// </summary>
        public override void Insert()
        {
            base.Insert();

            XtraWindowSmartPartInfo spi = new XtraWindowSmartPartInfo()
            {
                MaximizeBox = false,
                MinimizeBox = false,
                Modal = true,
                ShowInTaskbar = false,
                StartPosition = System.Windows.Forms.FormStartPosition.CenterParent,
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog,
                Title = "新建命令"
            };

            IWorkspace wp = WorkItem.Workspaces.Get(UIExtensionSiteNames.Shell_Workspace_Window);
            if (wp != null) {
                CommandView view = WorkItem.Items.Get<CommandView>(AuthCommandView);
                if (view == null)
                    view = WorkItem.Items.AddNew<CommandView>(AuthCommandView);

                view.AuthNode = CurrentAuthNode.Tag as AuthorizationNode;
                wp.Show(view, spi);

                View.ListAuthorizationCommands(CurrentAuthNode.Tag as AuthorizationNode); // 刷新操作列表
                UpdateRoleAuthorization(view.AuthNode);
            }
        }

        /// <summary>
        /// 获取一个值决定当前可否编辑选定数据资料
        /// </summary>
        /// <value><c>true</c>如果可以编辑的话; 否则为, <c>false</c>.</value>
        /// 返回
        public override bool CanEdit
        {
            get
            {
                bool flag = base.CanEdit;
                flag &= View.TLCommands.Nodes.Count > 0 && View.TLCommands.Selection.Count > 0 
                    && View.TLCommands.Selection[0].Tag != null;
                return flag;
            }
        }

        /// <summary>
        /// 编辑选定数据资料
        /// </summary>
        public override void Edit()
        {
            base.Edit();

            XtraWindowSmartPartInfo spi = new XtraWindowSmartPartInfo()
            {
                MaximizeBox = false,
                MinimizeBox = false,
                Modal = true,
                ShowInTaskbar = false,
                StartPosition = System.Windows.Forms.FormStartPosition.CenterParent,
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog,
                Title = "新建操作"
            };

            IWorkspace wp = WorkItem.Workspaces.Get(UIExtensionSiteNames.Shell_Workspace_Window);
            if (wp != null) {
                CommandView view = WorkItem.Items.Get<CommandView>(AuthCommandView);
                if (view == null)
                    view = WorkItem.Items.AddNew<CommandView>(AuthCommandView);

                view.AuthNode = CurrentAuthNode.Tag as AuthorizationNode;
                view.BindingCommand(View.TLCommands.Selection[0].Tag as AuthorizationCommand); // 绑定操作项
                wp.Show(view, spi);

                View.ListAuthorizationCommands(CurrentAuthNode.Tag as AuthorizationNode); // 刷新操作列表
                UpdateRoleAuthorization(view.AuthNode);
            }
        }

        /// <summary>
        /// 获取一个值决定当前可否删除选定数据资料
        /// </summary>
        /// <value>返回<c>true</c>如果可以删除的话; 否则为, <c>false</c>.</value>
        public override bool CanDelete
        {
            get
            {
                bool flag = base.CanDelete;
                flag &= View.TLCommands.Nodes.Count > 0 && View.TLCommands.Selection.Count > 0
                    && View.TLCommands.Selection[0].Tag != null;
                return flag;
            }
        }

        /// <summary>
        /// 删除选定数据资料
        /// </summary>
        public override void Delete()
        {
            if (XtraMessageBox.Show("你真的要删除选定的操作项吗？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                base.Delete();
                
                AuthorizationCommand command = View.TLCommands.Selection[0].Tag as AuthorizationCommand; // 获取准备删除的操作项

                if (CurrentAuthNode != null) {
                    AuthorizationNode authNode = CurrentAuthNode.Tag as AuthorizationNode;
                    authNode.RemoveCommand(command);
                    View.TLCommands.DeleteNode(View.TLCommands.Selection[0]);
                    CurrentAuthNode.Tag = authNode;
                    AuthorizationNodeService.Save(authNode); // 将变化保存回后端数据库

                    UpdateRoleAuthorization(authNode);
                }
            }
        }

        /// <summary>
        /// 获取一个值决定当前数据列表是否可以刷新
        /// </summary>
        /// <value>返回<c>true</c>如果可以刷新的话; 否则为, <c>false</c>.</value>
        public override bool CanRefreshDataSource
        {
            get
            {
                bool flag = base.CanRefreshDataSource;
                flag &= CurrentAuthNode != null;
                return flag;
            }
        }

        /// <summary>
        /// 刷新数据列表视图
        /// </summary>
        public override void RefreshDataSource()
        {
            base.RefreshDataSource();

            AuthorizationNode authNode = CurrentAuthNode.Tag as AuthorizationNode;
            if (authNode != null) {
                using (WaitCursor cursor = new WaitCursor(true)) {
                    View.ListAuthorizationCommands(authNode);
                }
            }
        }

        #region Assistant functions

        /// <summary>
        /// 更新角色的授权信息
        /// </summary>
        /// <param name="authNode">The auth node.</param>
        private void UpdateRoleAuthorization(AuthorizationNode authNode)
        {
            Guard.ArgumentNotNull(authNode, "Authorization node");

            IList<AuthorizationStore> stores = AuthorizationStoreService.GetAll();
            if (stores != null) {
                foreach (AuthorizationStore store in stores)
                {
                    store.Store(authNode);
                    AuthorizationStoreService.SaveAuthorization(store);
                }
            }
        }

        /// <summary>
        /// Gets the current auth node.
        /// </summary>
        /// <value>The current auth node.</value>
        private TreeListNode CurrentAuthNode
        {
            get {
                if (View.TLAuth.Nodes.Count == 0)
                    return null;
                return View.TLAuth.Selection.Count > 0 ? View.TLAuth.Selection[0] : null;
            }
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            UpdateCommandStatus();
        }

        private void UpdateCommandStatus()
        {
            bool enabled = true;

            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_COMM_AUTHORIZTION_NEWAUTHNODE, enabled && CurrentAuthNode != null);
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_COMM_AUTHORIZATION_DELETEAUTHNODE, enabled && CurrentAuthNode != null);
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_COMM_AUTHORIZTION_EDITAUTHNODE, enabled && CurrentAuthNode != null);
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_COMM_AUTHORIZATION_SELECTCOMMAND, enabled && CurrentAuthNode != null);
        }

        /// <summary>
        /// 获取权限节点编辑窗口
        /// </summary>
        /// <returns></returns>
        private frmAuthNode GetAuthNodeForm()
        {
            frmAuthNode form = WorkItem.Items.Get<frmAuthNode>(AuthNodeForm);
            if (form == null)
                form = WorkItem.Items.AddNew<frmAuthNode>(AuthNodeForm);
            return form;
        }

        /// <summary>
        /// 删除权限节点
        /// </summary>
        /// <param name="node">权限节点</param>
        private void DeleteAuthNode(TreeListNode node)
        {
            foreach (TreeListNode child in node.Nodes) {
                DeleteAuthNode(child);
            }

            AuthorizationNode authNode = node.Tag as AuthorizationNode;
            if (authNode != null)
                try {
                    AuthorizationNodeService.Delete(authNode);

                    // 更新角色的授权信息
                    IList<AuthorizationStore> stores = AuthorizationStoreService.GetAll();
                    if (stores != null) {
                        foreach (AuthorizationStore store in stores)
                        {
                            store.Remove(authNode);
                            AuthorizationStoreService.SaveAuthorization(store);
                        }
                    }
                }
                catch { }
        }

        #endregion

        #region Command handlers

        /// <summary>
        /// 新建权限节点
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_COMM_AUTHORIZTION_NEWAUTHNODE)]
        public void OnNewAuthNode(object sender, EventArgs e)
        {
            frmAuthNode form = GetAuthNodeForm();

            form.EditMode(false);
            form.AuthId = "";
            form.AuthName = "";
            if (form.ShowDialog() == DialogResult.OK) {
                TreeListNode node = View.TLAuth.AppendNode(new object[] {form.AuthName, form.AuthId}, CurrentAuthNode);
                node.Tag = new AuthorizationNode() { 
                    Id = form.AuthId,
                    Name = form.AuthName
                };

                AuthorizationNode authNode = node.Tag as AuthorizationNode;
                if (authNode != null) {
                    authNode.AuthorizationUri = View.GetAuthrizationNodePath(node);
                    AuthorizationNodeService.Save(authNode);
                    UpdateRoleAuthorization(authNode);
                }

                node.ImageIndex = 0;
                node.SelectImageIndex = 1;
                CurrentAuthNode.ExpandAll();
            }
        }

        /// <summary>
        /// 编辑权限节点
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_COMM_AUTHORIZTION_EDITAUTHNODE)]
        public void OnEditAuthNode(object sender, EventArgs e)
        {
            frmAuthNode form = GetAuthNodeForm();
            AuthorizationNode authNode = CurrentAuthNode.Tag as AuthorizationNode;
            form.EditMode(true);
            form.AuthId = authNode.Id;
            form.AuthName = authNode.Name;
            if (form.ShowDialog() == DialogResult.OK)
            {
                authNode.Id = form.AuthId;
                authNode.Name = form.AuthName;
                AuthorizationNodeService.Save(authNode);
                UpdateRoleAuthorization(authNode);
            }
        }

        /// <summary>
        /// 删除权限节点
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_COMM_AUTHORIZATION_DELETEAUTHNODE)]
        public void OnDeleteAuthNode(object sender, EventArgs e)
        {
            if (XtraMessageBox.Show("您真的要删除选定的节点及其下所有子节点吗？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                try {
                    View.TLAuth.BeginUpdate();
                    DeleteAuthNode(CurrentAuthNode);
                    View.TLAuth.DeleteNode(CurrentAuthNode);
                }
                finally {
                    View.TLAuth.EndUpdate();
                }
            }
        }

        /// <summary>
        /// 刷新权限节点
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_COMM_AUTHORIZATION_REFRESHAUTHNODE)]
        public void OnRefreshAuthNode(object sender, EventArgs e)
        {
            View.LoadAuthorizationsNodes();
        }

        /// <summary>
        /// 选择操作命令
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_COMM_AUTHORIZATION_SELECTCOMMAND)]
        public void OnSelectCommand(object sender, EventArgs e)
        {
            frmSelectCommand form = WorkItem.Items.Get<frmSelectCommand>(SelectCommandForm);
            if (form == null)
                form = WorkItem.Items.AddNew<frmSelectCommand>(SelectCommandForm);

            AuthorizationNode authNode = CurrentAuthNode.Tag as AuthorizationNode;
            form.AuthNode = authNode;
            form.RefreshList();

            // 为授权节点添加操作
            if (form.ShowDialog() == DialogResult.OK) {
                List<AuthorizationCommand> lcs = form.GetSelection();
                foreach (AuthorizationCommand ac in lcs) {
                    authNode.AddCommand(ac);
                    View.ListAuthorizationCommands(authNode); // 刷新操作列表
                    AuthorizationNodeService.Save(authNode);
                    CurrentAuthNode.Tag = authNode;
                }
            }
        }

        #endregion

    }
}
