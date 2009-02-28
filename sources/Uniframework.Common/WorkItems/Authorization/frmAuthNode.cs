using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Uniframework.Common.WorkItems.Authorization
{
    public partial class frmAuthNode : DevExpress.XtraEditors.XtraForm
    {
        public frmAuthNode()
        {
            InitializeComponent();
        }

        public frmAuthNode(string id, string name)
        {
            txtId.Text = id;
            txtName.Text = name;
        }

        public string AuthId
        {
            get { return txtId.Text; }
            set { txtId.Text = value; }
        }

        public string AuthName
        {
            get { return txtName.Text; }
            set { txtName.Text = value; }
        }

        public void EditMode(bool mode)
        {
            txtId.Properties.ReadOnly = mode;
        }

        private void frmAuthNode_Activated(object sender, EventArgs e)
        {
            txtId.Focus();
        }
    }
}