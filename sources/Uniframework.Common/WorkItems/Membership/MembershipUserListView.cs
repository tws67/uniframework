using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using Microsoft.Practices.ObjectBuilder;

namespace Uniframework.Common.WorkItems.Membership
{
    [AuthResource("ϵͳ����ģ��", "/Shell/Module/Foundation/MembershipUser")] 
    public partial class MembershipUserListView : DevExpress.XtraEditors.XtraUserControl
    {
        public MembershipUserListView()
        {
            InitializeComponent();
        }

        private MembershipUserListPresenter presenter;
        [CreateNew]
        public MembershipUserListPresenter Presenter
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
