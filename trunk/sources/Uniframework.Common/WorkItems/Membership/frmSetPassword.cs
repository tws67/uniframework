using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Practices.CompositeUI;
using Uniframework.Services;

namespace Uniframework.Common.WorkItems.Membership
{
    public partial class frmSetPassword : DevExpress.XtraEditors.XtraForm
    {
        public frmSetPassword()
        {
            InitializeComponent();
        }

        [ServiceDependency]
        public IMembershipService MembershipService
        {
            get;
            set;
        }

        public string UserName
        {
            get { return edtUser.UserName; }
            set { edtUser.UserName = value; }
        }

        /// <summary>
        /// 修改指定帐户的登录密码
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            try {
                string oldPassword = MembershipService.GetPassword(UserName);
                MembershipService.ChangePassword(UserName, oldPassword, edtPassword.Text);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(String.Format("修改帐户 \"{0}\" 的密码出错，", UserName) + ex.Message);
                btnOK.DialogResult = DialogResult.None;
            }
            btnOK.DialogResult = DialogResult.OK;
        }
    }
}