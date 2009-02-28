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
            IList<AuthorizationNode> nodes = Presenter.AuthorizationStoreService.GetAuthorizationNodes();
            foreach (AuthorizationNode authNode in Presenter.AuthorizationStoreService.GetAuthorizationNodes()) { 

            }
            if (nodes.Count == 0) {
                AuthorizationNode authNode = new AuthorizationNode() { 
                    Id = "Shell",
                    Name = "ϵͳȨ��"
                };
                authNode.AuthorizationUri = GlobalConstants.Uri_Separator + "Shell";
                TreeListNode tlNode = tlAuth.AppendNode(new object[] { authNode.Name, authNode.Id }, -1, authNode);
                tlNode.Tag = authNode;
                tlNode.ImageIndex = 0;
                tlNode.SelectImageIndex = 1;
            }
            else {
                foreach (AuthorizationNode authNode in nodes) {
                    LoadAuthorizationNode(authNode, tlAuth.Nodes[0]);
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

        #region Assistant functions

        private void LoadAuthorizationNode(AuthorizationNode authNode, TreeListNode tlNode)
        {
            string[] authPath = authNode.AuthorizationUri.Split(new string[] {GlobalConstants.Uri_Separator}, StringSplitOptions.None);
            TreeListNode currentNode = tlNode;

            if (authPath.Length < 1)
                return;

            for (int i = 1; i < authPath.Length; ++i) {
                bool found = false;
                foreach (TreeListNode node in tlNode.Nodes) {
                    if (node.GetDisplayText(colId) == authPath[i]) {
                        currentNode = node;
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    if (i == authPath.Length - 1 && currentNode.Tag == null)
                        currentNode.Tag = authNode;
                }
                else {
                    TreeListNode newNode = tlAuth.AppendNode(new object[] { authNode.Name, authNode.Id }, currentNode, authNode);
                    newNode.ImageIndex = 2;
                    newNode.SelectImageIndex = 1;
                    if (i == authPath.Length - 1)
                        newNode.Tag = authNode;
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
            if (authNode != null) {
                foreach (AuthorizationCommand cmd in authNode.Commands) {
                    TreeListNode node = tlCommands.AppendNode(new object[] {cmd.Name, cmd.CommandUri, cmd.Image }, -1, cmd);
                    node.ImageIndex = 3;
                    node.SelectImageIndex = 3;
                }
            }
        }

        #endregion

    }
}
