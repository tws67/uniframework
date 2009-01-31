using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Uniframework.Common.WorkItems.Membership
{
    public partial class frmMembershipRole : DevExpress.XtraEditors.XtraForm
    {
        public frmMembershipRole()
        {
            InitializeComponent();
        }

        public string Role {
            get {
                return txtRole.Role;
            }
        }
    }
}