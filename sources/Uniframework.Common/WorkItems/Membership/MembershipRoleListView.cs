using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Nodes;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.ObjectBuilder;
using Uniframework.SmartClient;
using Uniframework.XtraForms.SmartPartInfos;
using DevExpress.XtraTreeList;

namespace Uniframework.Common.WorkItems.Membership
{
    public partial class MembershipRoleListView : DevExpress.XtraEditors.XtraUserControl, IDataListView
    {
        public MembershipRoleListView()
        {
            InitializeComponent();
        }

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

        [EventPublication(EventNames.Membership_CurrentRoleChanged, PublicationScope.WorkItem)]
        public event EventHandler<EventArgs<string>> CurrentRoleChanged;
        protected void OnCurrentRoleChanged(string role)
        {
            Presenter.WorkItem.State.Remove(Constants.CurrentRole);
            Presenter.WorkItem.State[Constants.CurrentRole] = role;
            if (CurrentRoleChanged != null)
                CurrentRoleChanged(this, new EventArgs<string>(role));
        }

        public TreeList RoleList
        {
            get { return tlRole; }
        }

        private void MembershipRoleListView_Load(object sender, EventArgs e)
        {
            Presenter.Initilize();
        }

        private void tlRole_AfterFocusNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            if (e.Node != null)
                OnCurrentRoleChanged(e.Node.GetDisplayText(colRoleName));
        }

        #region IDataListView Members

        /// <summary>
        /// 数据列表处理器
        /// </summary>
        /// <value>The presenter.</value>
        public IDataListHandler DataListHandler
        {
            get { return Presenter as IDataListHandler; }
        }

        /// <summary>
        /// 数据列表视图只读属性
        /// </summary>
        /// <value><c>true</c> if [read only]; otherwise, <c>false</c>.</value>
        public bool ReadOnly
        {
            get { return false; }
        }

        #endregion
    }
}
