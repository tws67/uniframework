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
    public partial class MembershipUserEdit : DevExpress.XtraEditors.XtraUserControl
    {
        public MembershipUserEdit()
        {
            InitializeComponent();
        }

        [Category("Data")]
        public string UserName
        {
            get { return edtUserName.Text; }
            set { edtUserName.Text = value; }
        }
    }
}
