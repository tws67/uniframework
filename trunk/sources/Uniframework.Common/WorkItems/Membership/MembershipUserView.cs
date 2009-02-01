using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Practices.CompositeUI;
using Uniframework.Services;

namespace Uniframework.Common.WorkItems.Membership
{
    public partial class MembershipUserView : DevExpress.XtraEditors.XtraUserControl
    {
        public MembershipUserView()
        {
            InitializeComponent();
        }

        [ServiceDependency]
        public IMembershipService MembershipService
        {
            get;
            set;
        }

        private void txtEmail_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // 创建帐户
            try {
                MembershipService.CreateUser(txtUserName.Text, txtPassword.Text, txtEmail.Text);
            }
            catch(Exception ex) {
                XtraMessageBox.Show("创建用户登录帐户失败，" + ex.Message);
                btnOK.DialogResult = DialogResult.None;
            }
            btnOK.DialogResult = DialogResult.OK;
        }
    }
}
