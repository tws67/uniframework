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

namespace Uniframework.Common.WorkItems.Authorization
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthorizationStoreListPresenter : DataListPresenter<AuthorizationStoreListView>
    {
        private readonly string AuthNodeForm = "AuthNodeForm";

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
        /// 获取一个值决定当前可否编辑选定数据资料
        /// </summary>
        /// <value><c>true</c>如果可以编辑的话; 否则为, <c>false</c>.</value>
        /// 返回
        public override bool CanEdit
        {
            get
            {
                bool flag = base.CanEdit;
                flag &= View.TLCommands.Nodes.Count > 0 && View.TLCommands.Selection.Count > 0;
                return flag;
            }
        }

        /// <summary>
        /// 编辑选定数据资料
        /// </summary>
        public override void Edit()
        {
            base.Edit();
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
                flag &= View.TLCommands.Nodes.Count > 0 && View.TLCommands.Selection.Count > 0;
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
                    AuthorizationStoreService.Save(authNode); // 将变化保存回后端数据库
                }
            }
        }

        #region Assistant functions

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
                    AuthorizationStoreService.Delete(authNode);
                    //View.TLAuth.DeleteNode(node);
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
            using (frmAuthNode form = GetAuthNodeForm()) {
                AuthorizationNode authNode = CurrentAuthNode.Tag as AuthorizationNode;
                form.EditMode(true);
                form.AuthId = authNode.Id;
                form.AuthName = authNode.Name;
                if (form.ShowDialog() == DialogResult.OK) {
                    authNode.Id = form.AuthId;
                    authNode.Name = form.AuthName;
                }
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

        }

        #endregion

    }
}
