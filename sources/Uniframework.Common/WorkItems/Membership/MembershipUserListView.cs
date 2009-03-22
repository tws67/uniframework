using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.ObjectBuilder;
using Uniframework.SmartClient;

namespace Uniframework.Common.WorkItems.Membership
{
    /// <summary>
    /// �����û�������ͼ
    /// </summary>
    [Authorization("/Shell/Foundation/Membership/Users")]
    public partial class MembershipUserListView : DevExpress.XtraEditors.XtraUserControl, IDataListView
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
            set
            {
                presenter = value;
                presenter.View = this;
            }
        }

        /// <summary>
        /// ��ǰѡ����û��仯�¼�
        /// </summary>
        [EventPublication(EventNames.Membership_CurrentUserChanged, PublicationScope.WorkItem)]
        public event EventHandler<EventArgs<string>> CurrentUserChanged;
        protected void OnCurrentUserChanged(object sender, EventArgs<string> e)
        {
            Presenter.WorkItem.State.Remove(Constants.CurrentUser);
            Presenter.WorkItem.State[Constants.CurrentUser] = e.Data;

            // �����¼�
            if (CurrentUserChanged != null)
                CurrentUserChanged(sender, e);
        }

        public TreeList UsersList
        {
            get { return tlUser; }
        }

        public void SetDataSource(object datasource)
        {
            bsUser.DataSource = datasource;
        }

        private void MembershipUserListView_Load(object sender, EventArgs e)
        {
            Presenter.OnViewReady();
        }

        #region IDataListView Members

        public IDataListHandler DataListHandler
        {
            get {
                return Presenter as IDataListHandler;
            }
        }

        public bool ReadOnly
        {
            get { return false; }
        }

        #endregion

        /// <summary>
        /// ������ǰѡ����û��仯�¼�
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DevExpress.XtraTreeList.FocusedNodeChangedEventArgs"/> instance containing the event data.</param>
        private void tlUser_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            if (e.Node != null)
                OnCurrentUserChanged(this, new EventArgs<string>(e.Node.GetDisplayText(colUserName)));
        }
    }
}
