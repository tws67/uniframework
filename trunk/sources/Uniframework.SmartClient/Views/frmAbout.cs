using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Practices.ObjectBuilder;

namespace Uniframework.SmartClient.Views
{
    public partial class frmAbout : DevExpress.XtraEditors.XtraForm
    {
        public frmAbout()
        {
            InitializeComponent();

            labProduct.Text = Application.ProductName;
            labVersion.Text = Application.ProductVersion;
            labCompany.Text = Application.CompanyName;
        }

        private AboutPresenter presenter;
        [CreateNew]
        public AboutPresenter Presenter
        {
            get { return presenter; }
            set {
                presenter = value;
                presenter.View = this;
                labNetVersion.Text = presenter.NetFrameworkVersion;
                labCopyright.Text = presenter.Copyright;
            }
        }

        public void AddModule(string name, string version)
        {
            //ListViewItem item = lvModules.Items.Add(name);
            //item.SubItems.Add(version);

            tlAddIns.AppendNode(new object[] { name, version }, null);
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            Presenter.OnViewReady();
        }

        private void btnCopyInfo_Click(object sender, EventArgs e)
        {
            Presenter.CopyInfo();
        }

        private void btnSysInfo_Click(object sender, EventArgs e)
        {
            Presenter.SystemInfo();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Presenter.Close();
        }
    }
}