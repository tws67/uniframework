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
            // �����ʻ�
            try {
                MembershipService.CreateUser(txtUserName.Text, txtPassword.Text, txtEmail.Text);
            }
            catch(Exception ex) {
                XtraMessageBox.Show("�����û���¼�ʻ�ʧ�ܣ�" + ex.Message);
                btnOK.DialogResult = DialogResult.None;
            }
            btnOK.DialogResult = DialogResult.OK;
        }
    }
}
