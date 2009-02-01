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
        /// �����û�����
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            MembershipUserCollection users = MembershipService.FindUserByName(edtUser.UserName);
            if (users.Count > 0) {
                try {
                    MembershipUser user = users[edtUser.UserName]; // ��ȡָ���û�
                    user.Email = edtEmail.Text;
                    MembershipService.UpdateUser(user);
                }
                catch (Exception ex) {
                    XtraMessageBox.Show("�����û� \"" + edtUser.UserName + "\" ������ʱʧ�ܣ�" + ex.Message);
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
