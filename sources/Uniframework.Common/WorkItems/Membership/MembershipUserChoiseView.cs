using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Nodes;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using Uniframework.Services;
using System.Web.Security;

namespace Uniframework.Common.WorkItems.Membership
{
    public partial class MembershipUserChoiseView : DevExpress.XtraEditors.XtraUserControl
    {
        public MembershipUserChoiseView()
        {
            InitializeComponent();
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

        /// <summary>
        /// 获取当前选取的帐户资料
        /// </summary>
        /// <value>The selected users.</value>
        public string[] SelectedUsers {
            get {
                List<string> users = new List<string>();
                foreach (TreeListNode node in tlUsers.Selection) {
                    users.Add(node.GetDisplayText(0));
                }
                return users.ToArray();
            }
        }

        [EventSubscription(EventNames.Membership_CurrentRoleChanged)]
        public void OnCurrentRoleChanged(object sender, EventArgs<string> e)
        {
            string role = e.Data;
            GetUsersNotInRole(role);
        }

        private void GetUsersNotInRole(string role)
        {
            using (WaitCursor cursor = new WaitCursor(true))
            {
                tlUsers.ClearNodes();
                tlUsers.BeginUpdate();
                try
                {
                    foreach (MembershipUser user in MembershipService.GetAllUsers())
                    {
                        if (!MembershipService.IsUserInRole(user.UserName, role))
                        {
                            tlUsers.AppendNode(new object[] { user.UserName }, -1, 0, 0, 0);
                        }
                    }
                }
                finally
                {
                    tlUsers.EndUpdate();
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string role = WorkItem.State[Constants.CurrentRole] as String;
            if (!String.IsNullOrEmpty(role)) {
                foreach (string user in SelectedUsers) {
                    if (!MembershipService.IsUserInRole(user, role))
                        MembershipService.AddUserToRole(user, role);
                }
            }
        }

        private void MembershipUserChoiseView_Enter(object sender, EventArgs e)
        {
            if (WorkItem.State[Constants.CurrentRole] != null)
                GetUsersNotInRole(WorkItem.State[Constants.CurrentRole] as String);
        }
    }
}
