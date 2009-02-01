using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using Uniframework.Services;
using System.Web.Security;

namespace Uniframework.Common.WorkItems.Membership
{
    public partial class MembershipUserEditView : DevExpress.XtraEditors.XtraUserControl
    {
        public MembershipUserEditView()
        {
            InitializeComponent();
        }

        [ServiceDependency]
        public IMembershipService MembershipService
        {
            get;
            set;
        }

        [ServiceDependency]
        public WorkItem WorkItem
        {
            get;
            set;
        }

        [EventSubscription(EventNames.Membership_CurrentUserChanged, ThreadOption.UserInterface)]
        public void OnCurrentUserChanged(object sender, EventArgs<string> e)
        {
            edtUser.UserName = e.Data;
        }

        /// <summary>
        /// 更新用户资料
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            MembershipUserCollection users = MembershipService.FindUserByName(edtUser.UserName);
            if (users.Count > 0) {
                try {
                    MembershipUser user = users[edtUser.UserName]; // 获取指定用户
                    user.Email = edtEmail.Text;
                    MembershipService.UpdateUser(user);
                }
                catch (Exception ex) {
                    XtraMessageBox.Show("更新用户 \"" + edtUser.UserName + "\" 的资料时失败，" + ex.Message);
                    btnOK.DialogResult = DialogResult.None;
                }
                btnOK.DialogResult = DialogResult.OK;
            }
        }

        private void MembershipUserEditView_Load(object sender, EventArgs e)
        {
            if (WorkItem.State[Constants.CurrentUser] != null)
                edtUser.UserName = WorkItem.State[Constants.CurrentUser] as String;
        }
    }
}
