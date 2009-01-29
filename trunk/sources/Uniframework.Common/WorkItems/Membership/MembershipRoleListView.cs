using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Practices.ObjectBuilder;
using Uniframework.SmartClient;

namespace Uniframework.Common.WorkItems.Membership
{
    public partial class MembershipRoleListView : DataListBaseView
    {
        private MembershipRoleListPresenter presenter;
        [CreateNew]
        public MembershipRoleListPresenter Presenter
        {
            get { return presenter; }
            set {
                presenter = value;
                presenter.View = this;
            }
        }

        #region Override Methods

        public override bool CanInsert
        {
            get
            {
                return true;
            }
        }

        public override void Insert()
        {
            base.Insert();
        }

        public override bool CanEdit
        {
            get
            {
                return true;
            }
        }

        public override void Edit()
        {
            base.Edit();
        }

        public override bool CanDelete
        {
            get
            {
                return true;
            }
        }

        public override void Delete()
        {
            base.Delete();
        }

        public override bool CanCollaspe
        {
            get
            {
                return false;
            }
        }

        public override void Collaspe()
        {
            base.Collaspe();
        }

        public override bool CanExpand
        {
            get
            {
                return false;
            }
        }

        public override void Expand()
        {
            base.Expand();
        }

        public override bool CanRefreshDataSource
        {
            get
            {
                return true;
            }
        }

        public override void RefreshDataSource()
        {
            bsRole.DataSource = Presenter.GetRoles();
        }

        #endregion

        private void MembershipRoleListView_Load(object sender, EventArgs e)
        {
            RefreshDataSource();
        }
    }
}
