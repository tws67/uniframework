using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Practices.ObjectBuilder;
using DevExpress.XtraTreeList;

namespace Uniframework.SmartClient.Views
{
    public partial class MembershipUserView : DevExpress.XtraEditors.XtraUserControl
    {
        public MembershipUserView()
        {
            InitializeComponent();
        }

        private MembershipUserPresenter presenter;
        [CreateNew]
        public MembershipUserPresenter Presenter
        {
            get { return presenter; }
            set {
                presenter = value;
                presenter.View = this;
            }
        }

        public TreeList UsersList
        {
            get { return tlUser; }
        }

        public void SetDataSource(object datasource)
        {
            bsUser.DataSource = datasource;
        }

        private void MembershipUserView_Load(object sender, EventArgs e)
        {
            Presenter.RefreshMembershipUsers();
        }
    }
}
