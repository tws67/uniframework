using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Uniframework.Common.WorkItems.Membership
{
    /// <summary>
    /// ½ÇÉ«±à¼­¿Ø¼þ
    /// </summary>
    public partial class MembershipRoleEdit : DevExpress.XtraEditors.XtraUserControl
    {
        public MembershipRoleEdit()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>The role.</value>
        [Category("Data")]
        public string Role {
            get { return txtRole.Text; }
            set { txtRole.Text = value; }
        }

        [Category("Behavior")]
        public bool Editable
        {
            get { return txtRole.Properties.TextEditStyle == TextEditStyles.Standard; }
            set { 
                txtRole.Properties.TextEditStyle = value == true ? TextEditStyles.Standard : TextEditStyles.DisableTextEditor;
            }
        }
    }
}
