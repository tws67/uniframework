using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Uniframework.Common.WorkItems.Membership
{
    public partial class MembershipUserView : DevExpress.XtraEditors.XtraUserControl
    {
        public MembershipUserView()
        {
            InitializeComponent();
        }

        private void txtEmail_EditValueChanged(object sender, EventArgs e)
        {

        }
    }
}
