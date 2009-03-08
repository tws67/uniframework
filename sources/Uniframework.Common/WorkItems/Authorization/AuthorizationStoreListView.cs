using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Practices.ObjectBuilder;
using Uniframework.SmartClient;
using Uniframework.Security;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList;

namespace Uniframework.Common.WorkItems.Authorization
{
    /// <summary>
    /// ϵͳ��Ȩ�����б�
    /// </summary>
    public partial class AuthorizationStoreListView : DevExpress.XtraEditors.XtraUserControl, IDataListView
    {
        public AuthorizationStoreListView()
        {
            InitializeComponent();
        }

        private AuthorizationStoreListPresenter presenter;
        [CreateNew]
        public AuthorizationStoreListPresenter Presenter
        {
            get { return presenter; }
            set {
                presenter = value;
                presenter.View = this;
            }
        }

        #region IDataListView Members

        /// <summary>
        /// �����б�����
        /// </summary>
        /// <value>The presenter.</value>
        public IDataListHandler DataListHandler
        {
            get { return Presenter as IDataListHandler; }
        }

        /// <summary>
        /// �����б���ͼֻ������
        /// </summary>
        /// <value><c>true</c> if [read only]; otherwise, <c>false</c>.</value>
        public bool ReadOnly
        {
            get { return false; }
        }

        #endregion

        /// <summary>
        /// Get commands tree list
        /// </summary>
        /// <value>The TL commands.</value>
        public TreeList TLCommands
        {
            get { return tlCommands; }
        }

        /// <summary>
        /// Gets the TL auth.
        /// </summary>
        /// <value>The TL auth.</value>
        public TreeList TLAuth
        {
            get { return tlAuth; }
        }

        public void LoadAuthorizationsNodes()
        {
            using (WaitCursor cursor = new WaitCursor(true)) {
                tlAuth.BeginUpdate();
                tlAuth.FocusedNodeChanged -= new FocusedNodeChangedEventHandler(tlAuth_FocusedNodeChanged);
                tlAuth.Nodes.Clear();
                try
                {
                    IList<AuthorizationNode> nodes = Presenter.AuthorizationNodeService.GetAll();

                    AuthorizationNode authNode = new AuthorizationNode()
                    {
                        Id = "Shell",
                        Name = "ϵͳȨ��"
                    };
                    authNode.AuthorizationUri = GlobalConstants.Uri_Separator + "Shell";
                    TreeListNode tlNode = tlAuth.AppendNode(new object[] { authNode.Name, authNode.Id }, -1, authNode);
                    tlNode.Tag = authNode;
                    ((AuthorizationNode)tlNode.Tag).AuthorizationUri = GetAuthrizationNodePath(tlNode);
                    tlNode.ImageIndex = 0;
                    tlNode.SelectImageIndex = 1;

                    foreach (AuthorizationNode child in nodes)
                    {
                        LoadAuthorizationNode(child, tlAuth.Nodes[0]);
                    }
                    tlAuth.Nodes[0].ExpandAll(); // չ�������ӽڵ�
                }
                finally {
                    tlAuth.EndUpdate();
                    tlAuth.FocusedNodeChanged += new FocusedNodeChangedEventHandler(tlAuth_FocusedNodeChanged);
                }
            }

        }

        /// <summary>
        /// ��ȡ��ǰ��Ȩ�ڵ��·��
        /// </summary>
        /// <param name="node">Tree list node</param>
        /// <returns>���شӸ��ڵ㵽��ǰ�ڵ��·��ֵ</returns>
        public string GetAuthrizationNodePath(TreeListNode node)
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
        /// Lists the authorization commands.
        /// </summary>
        /// <param name="authNode">The auth node.</param>
        public void ListAuthorizationCommands(AuthorizationNode authNode)
        {
            using (WaitCursor cursor = new WaitCursor(true))
            {
                Dictionary<string, TreeListNode> categories = new Dictionary<string, TreeListNode>(); // ����
                tlCommands.BeginUpdate();
                tlCommands.ClearNodes();
                try
                {
                    if (authNode != null)
                    {
                        foreach (AuthorizationCommand cmd in authNode.Commands)
                        {
                            if (!categories.ContainsKey(cmd.Category))
                                categories.Add(cmd.Category, GetCategoryNode(cmd.Category));

                            TreeListNode parent = categories[cmd.Category];
                            TreeListNode child = tlCommands.AppendNode(new object[] { cmd.Name, cmd.CommandUri, cmd.Image }, parent, cmd);
                            child.ImageIndex = 3;
                            child.SelectImageIndex = 3;
                        }
                    }
                }
                finally
                {
                    tlCommands.ExpandAll();
                    tlCommands.EndUpdate();
                }
            }
        }

        #region Assistant functions


        /// <summary>
        /// Gets the category node.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        private TreeListNode GetCategoryNode(string category)
        {
            if (String.IsNullOrEmpty(category) || category.Length == 0)
                category = Constants.DefaultCommandCategory;

            TreeListNode node = tlCommands.AppendNode(new object[] { category, "", "" }, -1);
            node.ImageIndex = 4;
            node.SelectImageIndex = 2;
            return node;
        }

        /// <summary>
        /// ����Ȩ�޽ڵ�
        /// </summary>
        /// <param name="authNode">Ȩ�޽ڵ�</param>
        /// <param name="tlNode">���ڵ�</param>
        private void LoadAuthorizationNode(AuthorizationNode authNode, TreeListNode tlNode)
        {
            string[] authPath = authNode.AuthorizationUri.Split(new string[] {GlobalConstants.Uri_Separator}, StringSplitOptions.None);
            TreeListNode current = tlNode;

            if (authPath.Length < 1)
                return;

            for (int i = 2; i < authPath.Length; ++i) {
                bool found = false;
                foreach (TreeListNode node in current.Nodes) {
                    string id = node.GetDisplayText(colId);
                    if (id == authPath[i]) {
                        current = node;
                        found = true;
                        break;
                    }
                }

                if (found) {
                    if (i == authPath.Length - 1 && current.Tag == null)
                    {
                        current.SetValue(colNode, authNode.Name);
                        current.Tag = authNode;
                    }
                }
                else {
                    TreeListNode child = tlAuth.AppendNode(new object[] { "tempnode", authPath[i] }, current); // ���Ǵ����Ŀ������м�ڵ�
                    child.ImageIndex = 0;
                    child.SelectImageIndex = 1;

                    // ��������Ҫ�����Ľڵ���������
                    if (i == authPath.Length - 1) {
                        child.SetValue(colNode, authNode.Name);
                        child.Tag = authNode;
                    }
                    current = child;
                }
            }
        }

        private void AuthorizationStoreListView_Load(object sender, EventArgs e)
        {
            Presenter.OnViewReady();
        }

        /// <summary>
        /// ��ǰ����ƶ�ʱ��ʾ��Ȩ�ڵ��·����Ϣ
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void tlAuth_MouseMove(object sender, MouseEventArgs e)
        {
            TreeListHitInfo hi = tlAuth.CalcHitInfo(new Point(e.X, e.Y));
            if (hi.Node != null)
            {
                string authPath = GetAuthrizationNodePath(hi.Node);
                Presenter.SmartClient.ShowHint(authPath);
            }
            else
                Presenter.SmartClient.ShowHint(String.Empty);
        }

        /// <summary>
        /// �г���ǰȨ�޽ڵ������еĲ���
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DevExpress.XtraTreeList.FocusedNodeChangedEventArgs"/> instance containing the event data.</param>
        private void tlAuth_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            AuthorizationNode authNode = e.Node.Tag as AuthorizationNode;
            ListAuthorizationCommands(authNode);
        }

        #endregion

    }
}
