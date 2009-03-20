using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web.Security;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Nodes;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.ObjectBuilder;
using Uniframework.Security;
using Uniframework.Services;
using Uniframework.SmartClient;
using Uniframework.XtraForms.SmartPartInfos;

namespace Uniframework.Common.WorkItems.Membership
{
    /// <summary>
    /// 角色编辑窗口
    /// </summary>
    [SmartPart]
    public partial class MembershipRoleView : DevExpress.XtraEditors.XtraUserControl
    {
        private AuthorizationStore authStore; 

        public MembershipRoleView()
        {
            InitializeComponent();
        }

        #region Dependency Services

        private MembershipRolePresenter presenter;
        [CreateNew]
        public MembershipRolePresenter Presenter
        {
            get { return presenter; }
            set {
                presenter = value;
                presenter.View = this;
            }
        }

        [ServiceDependency]
        public WorkItem WorkItem
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// 订阅当前角色变化事件
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Uniframework.EventArgs&lt;System.String&gt;"/> instance containing the event data.</param>
        [EventSubscription(EventNames.Membership_CurrentRoleChanged)]
        public void OnCurrentRoleChanged(object sender, EventArgs<string> e)
        {
            //RefreshCurrentRole(e.Data);

            //AuthorizationStore store = GetAuthorizationForRole(e.Data);
            //LoadAuthorizationNodes(store);
        }


        #region Assistant functions

        /// <summary>
        /// 获取指定角色的权限信息
        /// </summary>
        /// <param name="role">角色名称</param>
        private AuthorizationStore GetAuthorizationForRole(string role)
        {
            authStore = Presenter.AuthorizationStoreService.GetAuthorizationsByRole(role);
            if (authStore == null)
            {
                authStore = new AuthorizationStore(role);

                IList<AuthorizationNode> lns = Presenter.AuthorizationNodeService.GetAll();
                foreach (AuthorizationNode an in lns)
                    authStore.Store(an);
                Presenter.AuthorizationStoreService.SaveAuthorization(authStore); // 保存角色的权限信息
            }
            return authStore;
        }

        /// <summary>
        /// 加载所有的权限节点信息
        /// </summary>
        private void LoadAuthorizationNodes(AuthorizationStore store)
        {
            using (WaitCursor cursor = new WaitCursor(true))
            {
                tlAuth.BeginUpdate();
                tlAuth.Nodes.Clear();
                try
                {
                    AuthorizationNode authNode = new AuthorizationNode() {
                        Id = "Shell",
                        Name = "系统权限"
                    };
                    authNode.AuthorizationUri = GlobalConstants.Uri_Separator + "Shell";
                    TreeListNode tlNode = tlAuth.AppendNode(new object[] { authNode.Name, authNode.Id }, -1, authNode);
                    tlNode.Tag = authNode;
                    ((AuthorizationNode)tlNode.Tag).AuthorizationUri = GetAuthrizationNodePath(tlNode);
                    tlNode.ImageIndex = 0;
                    tlNode.SelectImageIndex = 1;

                    // 加载角色的相关权限节点
                    foreach (AuthorizationNode child in store.Nodes) {
                        LoadAuthorizationNode(child, tlAuth.Nodes[0]);
                    }
                    tlAuth.Nodes[0].ExpandAll(); // 展开所有子节点
                }
                finally
                {
                    tlAuth.EndUpdate();
                }
            }
        }

        private void LoadAuthorizationNode(AuthorizationNode authNode, TreeListNode tlNode)
        {
            string[] authPath = authNode.AuthorizationUri.Split(new string[] { GlobalConstants.Uri_Separator }, StringSplitOptions.None);
            TreeListNode current = tlNode;

            if (authPath.Length < 1)
                return;

            for (int i = 2; i < authPath.Length; ++i)
            {
                bool found = false;
                foreach (TreeListNode node in current.Nodes)
                {
                    string id = node.GetDisplayText(colId);
                    if (id == authPath[i])
                    {
                        current = node;
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    if (i == authPath.Length - 1 && current.Tag == null)
                    {
                        current.SetValue(colNode, authNode.Name);
                        current.Tag = authNode;
                    }
                }
                else
                {
                    TreeListNode child = tlAuth.AppendNode(new object[] { "tempnode", authPath[i] }, current); // 我们创建的可能是中间节点
                    child.ImageIndex = 0;
                    child.SelectImageIndex = 1;

                    // 设置最终要创建的节点的相关属性
                    if (i == authPath.Length - 1)
                    {
                        child.SetValue(colNode, authNode.Name);
                        child.Tag = authNode;
                        LoadAuthorizationCommandNode(child, authNode);
                    }
                    current = child;
                }
            }
        }

        /// <summary>
        /// 加载权限节点下的操作项
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="authNode">The auth node.</param>
        private void LoadAuthorizationCommandNode(TreeListNode parent, AuthorizationNode authNode)
        {
            Dictionary<string, TreeListNode> categories = new Dictionary<string, TreeListNode>(); // 分组
            foreach (AuthorizationCommand cmd in authNode.Commands) {

                if (!categories.ContainsKey(cmd.Category))
                    categories[cmd.Category] = GetCategoryNode(cmd.Category, parent);
                TreeListNode category = categories[cmd.Category];
                TreeListNode child = tlAuth.AppendNode(new object[] { cmd.Name, cmd.CommandUri }, category, cmd);
                child.ImageIndex = 3;
                child.SelectImageIndex = 3;
                
                // 设置当前操作的权限操作
                string authorizationUri = GetAuthorizationUri(child);
                child.Checked = authStore.CanExecute(SecurityUtility.HashObject(authorizationUri + cmd.CommandUri));
                SetCheckedParentNodes(child, child.CheckState); 
            }
        }

        /// <summary>
        /// Gets the category node.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        private TreeListNode GetCategoryNode(string category, TreeListNode parent)
        {
            if (String.IsNullOrEmpty(category) || category.Length == 0)
                category = Constants.DefaultCommandCategory;

            TreeListNode node = tlAuth.AppendNode(new object[] { category, "", "" }, parent);
            node.ImageIndex = 4;
            node.SelectImageIndex = 2;
            return node;
        }

        /// <summary>
        /// 获取当前授权节点的路径
        /// </summary>
        /// <param name="node">Tree list node</param>
        /// <returns>返回从根节点到当前节点的路径值</returns>
        private string GetAuthrizationNodePath(TreeListNode node)
        {
            string authPath = "";
            if (node.Tag != null)
            {
                AuthorizationNode authNode = node.Tag as AuthorizationNode; // 获取节点的授权信息
                if (authNode == null)
                    return authPath;
                authPath = authNode.Id;
                TreeListNode curr = node.ParentNode;

                // 递归获取每一层节点的路径信息
                while (curr != null)
                {
                    if (curr.Tag != null)
                    {
                        authNode = curr.Tag as AuthorizationNode;
                        if (authNode == null)
                            return authPath;
                        authPath = authNode.Id + authPath;
                    }
                    curr = curr.ParentNode;
                }
                return authPath;
            }
            return authPath;
        }

        /// <summary>
        /// 刷新角色的成员信息
        /// </summary>
        /// <param name="role">The role.</param>
        private void RefreshCurrentRole(string role)
        {
            membershipRole.Role = role;
            using (WaitCursor cursor = new WaitCursor(true)) {
                tlMembers.BeginUpdate();
                tlMembers.ClearNodes();
                try {
                    string[] users = Presenter.MembershipService.GetUsersForRole(role);
                    foreach (string user in Presenter.MembershipService.GetUsersForRole(role)) {
                        TreeListNode node = tlMembers.AppendNode(new object[] { user }, -1, 0, 0, 0);
                        node.Tag = true; // 此值代表角色原来就拥有的成员
                    }
                }
                finally {
                    tlMembers.EndUpdate();
                }
            }
        }

        private void MembershipRoleView_Load(object sender, EventArgs e)
        {
            //if (WorkItem.State[Constants.CurrentRole] != null) {
            //    string role = (string)WorkItem.State[Constants.CurrentRole];
            //    RefreshCurrentRole(role);

            //    AuthorizationStore store = GetAuthorizationForRole(role);
            //    LoadAuthorizationNodes(store);
            //}
        }

        /// <summary>
        /// 为角色选择成员
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            XtraWindowSmartPartInfo spi = new XtraWindowSmartPartInfo() { 
                MaximizeBox = false,
                MinimizeBox = false,
                Modal = true,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                Title = "选择成员"
            };

            Presenter.ShowViewInWorkspace<MembershipUserChoiseView>(SmartPartNames.MembershipUserChoiseView, 
                UIExtensionSiteNames.Shell_Workspace_Window, spi);
            RefreshCurrentRole((string)WorkItem.State[Constants.CurrentRole]); // 刷新当前角色下的成员
        }

        /// <summary>
        /// 从角色中删除指定的成员
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            string role = membershipRole.Role;
            foreach (TreeListNode node in tlMembers.Selection) {
                string user = node.GetDisplayText(colUserName);
                if (Presenter.MembershipService.IsUserInRole(user, role))
                    Presenter.MembershipService.RemoveUserFromRole(user, role); // 将成员从角色中移除
            }
            RefreshCurrentRole(role);
        }

        /// <summary>
        /// Sets the checked parent nodes.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="check">The check.</param>
        private void SetCheckedParentNodes(TreeListNode node, CheckState check)
        {
            if (node.ParentNode != null)
            {
                bool b = false;
                CheckState state;
                for (int i = 0; i < node.ParentNode.Nodes.Count; i++)
                {
                    state = (CheckState)node.ParentNode.Nodes[i].CheckState;
                    if (!check.Equals(state))
                    {
                        b = !b;
                        break;
                    }
                }
                node.ParentNode.CheckState = b ? CheckState.Indeterminate : check;
                SetCheckedParentNodes(node.ParentNode, check);
            }
        }

        /// <summary>
        /// Sets the checked child nodes.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="check">The check.</param>
        private void SetCheckedChildNodes(TreeListNode node, CheckState check)
        {
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                node.Nodes[i].CheckState = check;
                SetCheckedChildNodes(node.Nodes[i], check);
            }
        }

        /// <summary>
        /// 保存当前角色的权限信息
        /// </summary>
        private void SaveAuthorizationForRole(TreeListNode authNode)
        {
            foreach (TreeListNode node in authNode.Nodes) {
                if (node.Tag != null && node.Tag is AuthorizationCommand) {
                    AuthorizationCommand command = node.Tag as AuthorizationCommand;
                    string authorizationUri = GetAuthorizationUri(node); // 获取当前操作的权限路径
                    authStore.Authorization(SecurityUtility.HashObject(authorizationUri + command.CommandUri), 
                        node.Checked ? AuthorizationAction.Allow : AuthorizationAction.Deny);
                }
                SaveAuthorizationForRole(node); // 保存子节点的权限授权信息
            }

            // 保存到对象数据库中
            Presenter.AuthorizationStoreService.SaveAuthorization(authStore);
        }

        /// <summary>
        /// 获取指定操作节点的权限路径
        /// </summary>
        /// <param name="node">操作节点</param>
        /// <returns>权限路径字符串</returns>
        private string GetAuthorizationUri(TreeListNode node)
        {
            string authorizationUri = "";
            TreeListNode current = node.ParentNode;
            while (current != null) {
                if (current.Tag != null && current.Tag is AuthorizationNode)
                    break;
                current = current.ParentNode;
            }
            authorizationUri = ((AuthorizationNode)current.Tag).AuthorizationUri;
            return authorizationUri;
        }

        #endregion

        private void tlAuth_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            SetCheckedChildNodes(e.Node, e.Node.CheckState);
            SetCheckedParentNodes(e.Node, e.Node.CheckState);
        }

        private void tlAuth_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
        }

        /// <summary>
        /// 保存对角色的权限编辑
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                SaveAuthorizationForRole(tlAuth.Nodes[0]);
            }
            catch {
            }
        }

        private void MembershipRoleView_Enter(object sender, EventArgs e)
        {
            if (WorkItem.State[Constants.CurrentRole] != null)
            {
                string role = (string)WorkItem.State[Constants.CurrentRole];
                RefreshCurrentRole(role);

                AuthorizationStore store = GetAuthorizationForRole(role);
                LoadAuthorizationNodes(store);
            }
        }
    }
}
