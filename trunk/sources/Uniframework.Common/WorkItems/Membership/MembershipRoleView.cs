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
using Uniframework.Services;
using Uniframework.XtraForms.SmartPartInfos;
using Microsoft.Practices.ObjectBuilder;
using Uniframework.SmartClient;

namespace Uniframework.Common.WorkItems.Membership
{
    /// <summary>
    /// 角色编辑窗口
    /// </summary>
    [SmartPart]
    public partial class MembershipRoleView : DevExpress.XtraEditors.XtraUserControl
    {
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

        [ServiceDependency]
        public IMembershipService MembershipService
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
            RefreshCurrentRole(e.Data);
        }

        #region Assistant functions

        private void RefreshCurrentRole(string role)
        {
            membershipRole.Role = role;
            using (WaitCursor cursor = new WaitCursor(true)) {
                tlMembers.BeginUpdate();
                tlMembers.ClearNodes();
                try {
                    string[] users = MembershipService.GetUsersForRole(role);
                    foreach (string user in MembershipService.GetUsersForRole(role)) {
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
            if (WorkItem.State["CurrentRole"] != null) {
                string role = (string)WorkItem.State["CurrentRole"];
                RefreshCurrentRole(role);
            }
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
            RefreshCurrentRole((string)WorkItem.State["CurrentRole"]); // 刷新当前角色下的成员
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
                if (MembershipService.IsUserInRole(user, role))
                    MembershipService.RemoveUserFromRole(user, role); // 将成员从角色中移除
            }
            RefreshCurrentRole(role);
        }

        #endregion
    }
}
