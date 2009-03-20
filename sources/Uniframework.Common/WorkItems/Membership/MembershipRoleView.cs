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
    /// ��ɫ�༭����
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
        /// ���ĵ�ǰ��ɫ�仯�¼�
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
        /// ��ȡָ����ɫ��Ȩ����Ϣ
        /// </summary>
        /// <param name="role">��ɫ����</param>
        private AuthorizationStore GetAuthorizationForRole(string role)
        {
            authStore = Presenter.AuthorizationStoreService.GetAuthorizationsByRole(role);
            if (authStore == null)
            {
                authStore = new AuthorizationStore(role);

                IList<AuthorizationNode> lns = Presenter.AuthorizationNodeService.GetAll();
                foreach (AuthorizationNode an in lns)
                    authStore.Store(an);
                Presenter.AuthorizationStoreService.SaveAuthorization(authStore); // �����ɫ��Ȩ����Ϣ
            }
            return authStore;
        }

        /// <summary>
        /// �������е�Ȩ�޽ڵ���Ϣ
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
                        Name = "ϵͳȨ��"
                    };
                    authNode.AuthorizationUri = GlobalConstants.Uri_Separator + "Shell";
                    TreeListNode tlNode = tlAuth.AppendNode(new object[] { authNode.Name, authNode.Id }, -1, authNode);
                    tlNode.Tag = authNode;
                    ((AuthorizationNode)tlNode.Tag).AuthorizationUri = GetAuthrizationNodePath(tlNode);
                    tlNode.ImageIndex = 0;
                    tlNode.SelectImageIndex = 1;

                    // ���ؽ�ɫ�����Ȩ�޽ڵ�
                    foreach (AuthorizationNode child in store.Nodes) {
                        LoadAuthorizationNode(child, tlAuth.Nodes[0]);
                    }
                    tlAuth.Nodes[0].ExpandAll(); // չ�������ӽڵ�
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
                    TreeListNode child = tlAuth.AppendNode(new object[] { "tempnode", authPath[i] }, current); // ���Ǵ����Ŀ������м�ڵ�
                    child.ImageIndex = 0;
                    child.SelectImageIndex = 1;

                    // ��������Ҫ�����Ľڵ���������
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
        /// ����Ȩ�޽ڵ��µĲ�����
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="authNode">The auth node.</param>
        private void LoadAuthorizationCommandNode(TreeListNode parent, AuthorizationNode authNode)
        {
            Dictionary<string, TreeListNode> categories = new Dictionary<string, TreeListNode>(); // ����
            foreach (AuthorizationCommand cmd in authNode.Commands) {

                if (!categories.ContainsKey(cmd.Category))
                    categories[cmd.Category] = GetCategoryNode(cmd.Category, parent);
                TreeListNode category = categories[cmd.Category];
                TreeListNode child = tlAuth.AppendNode(new object[] { cmd.Name, cmd.CommandUri }, category, cmd);
                child.ImageIndex = 3;
                child.SelectImageIndex = 3;
                
                // ���õ�ǰ������Ȩ�޲���
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
        /// ��ȡ��ǰ��Ȩ�ڵ��·��
        /// </summary>
        /// <param name="node">Tree list node</param>
        /// <returns>���شӸ��ڵ㵽��ǰ�ڵ��·��ֵ</returns>
        private string GetAuthrizationNodePath(TreeListNode node)
        {
            string authPath = "";
            if (node.Tag != null)
            {
                AuthorizationNode authNode = node.Tag as AuthorizationNode; // ��ȡ�ڵ����Ȩ��Ϣ
                if (authNode == null)
                    return authPath;
                authPath = authNode.Id;
                TreeListNode curr = node.ParentNode;

                // �ݹ��ȡÿһ��ڵ��·����Ϣ
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
        /// ˢ�½�ɫ�ĳ�Ա��Ϣ
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
                        node.Tag = true; // ��ֵ�����ɫԭ����ӵ�еĳ�Ա
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
        /// Ϊ��ɫѡ���Ա
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
                Title = "ѡ���Ա"
            };

            Presenter.ShowViewInWorkspace<MembershipUserChoiseView>(SmartPartNames.MembershipUserChoiseView, 
                UIExtensionSiteNames.Shell_Workspace_Window, spi);
            RefreshCurrentRole((string)WorkItem.State[Constants.CurrentRole]); // ˢ�µ�ǰ��ɫ�µĳ�Ա
        }

        /// <summary>
        /// �ӽ�ɫ��ɾ��ָ���ĳ�Ա
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            string role = membershipRole.Role;
            foreach (TreeListNode node in tlMembers.Selection) {
                string user = node.GetDisplayText(colUserName);
                if (Presenter.MembershipService.IsUserInRole(user, role))
                    Presenter.MembershipService.RemoveUserFromRole(user, role); // ����Ա�ӽ�ɫ���Ƴ�
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
        /// ���浱ǰ��ɫ��Ȩ����Ϣ
        /// </summary>
        private void SaveAuthorizationForRole(TreeListNode authNode)
        {
            foreach (TreeListNode node in authNode.Nodes) {
                if (node.Tag != null && node.Tag is AuthorizationCommand) {
                    AuthorizationCommand command = node.Tag as AuthorizationCommand;
                    string authorizationUri = GetAuthorizationUri(node); // ��ȡ��ǰ������Ȩ��·��
                    authStore.Authorization(SecurityUtility.HashObject(authorizationUri + command.CommandUri), 
                        node.Checked ? AuthorizationAction.Allow : AuthorizationAction.Deny);
                }
                SaveAuthorizationForRole(node); // �����ӽڵ��Ȩ����Ȩ��Ϣ
            }

            // ���浽�������ݿ���
            Presenter.AuthorizationStoreService.SaveAuthorization(authStore);
        }

        /// <summary>
        /// ��ȡָ�������ڵ��Ȩ��·��
        /// </summary>
        /// <param name="node">�����ڵ�</param>
        /// <returns>Ȩ��·���ַ���</returns>
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
        /// ����Խ�ɫ��Ȩ�ޱ༭
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
