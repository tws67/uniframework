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

namespace Uniframework.Common.WorkItems.Membership
{
    public partial class MembershipRoleListView : DevExpress.XtraEditors.XtraUserControl, IDataListHandler
    {
        private static readonly string Admin_Role = "Administrators";

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
            if (CurrentRoleChanged != null)
                CurrentRoleChanged(this, new EventArgs<string>(role));
        }

        #region IDataListHandler Members


        public void Initilize()
        {
            RefreshDataSource();
        }

        public bool CanInsert
        {
            get { return true; }
        }

        public void Insert()
        {
            frmMembershipRole form = new frmMembershipRole();
            if (form.ShowDialog() == DialogResult.OK && !String.IsNullOrEmpty(form.Role)) {
                // ���������Ľ�ɫ�����Ƿ��Ѿ�����
                if (Presenter.MembershipService.RoleExists(form.Role)) {
                    XtraMessageBox.Show("�Ѿ����� \"" + form.Role + "\" ��ɫ��");
                    return;
                }

                Presenter.MembershipService.CreateRole(form.Role);
                tlRole.AppendNode(new object[] { form.Role }, -1, 0, 0, 0);
            }
        }

        public bool CanEdit
        {
            get { return tlRole.Selection.Count > 0; }
        }

        public void Edit()
        {
            if (tlRole.Selection.Count > 0) {
                XtraWindowSmartPartInfo spi = new XtraWindowSmartPartInfo();
                spi.FormBorderStyle = FormBorderStyle.FixedDialog;
                spi.StartPosition = FormStartPosition.CenterParent;
                spi.MaximizeBox = false;
                spi.MinimizeBox = false;
                spi.Modal = true;
                spi.Title = "����";
                Presenter.WorkItem.State.Remove("CurrentRole");
                Presenter.WorkItem.State["CurrentRole"] = tlRole.FocusedNode.GetDisplayText(colRoleName);
                Presenter.ShowViewInWorkspace<MembershipRoleView>(SmartPartNames.MembershipRoleView, UIExtensionSiteNames.Shell_Workspace_Window, spi);
            }
        }

        public bool CanDelete
        {
            get { 
                bool result = tlRole.Selection.Count > 0;
                foreach (TreeListNode node in tlRole.Selection) {
                    result &= node.ToString() != Admin_Role;
                }
                return result;
            }
        }

        public void Delete()
        {
            if (tlRole.Selection.Count > 0) {
                string role = tlRole.Selection[0].GetDisplayText(colRoleName);
                if (XtraMessageBox.Show("���Ƿ����Ҫ��ϵͳ��ɾ����ɫ \"" + role + "\" ��", "��ʾ", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    string[] users = Presenter.MembershipService.GetUsersForRole(role);
                    if (users.Length > 0)
                        Presenter.MembershipService.RemoveUsersFromRole(users, role); // �Ƴ���ɫ�µ����г�Ա
                    Presenter.MembershipService.DeleteRole(role);
                    tlRole.DeleteNode(tlRole.Selection[0]); // ������ͼ
                }
            }
        }

        public bool CanExpand
        {
            get { return false; }
        }

        public void Expand()
        {
            throw new NotImplementedException();
        }

        public bool CanCollaspe
        {
            get { return false; }
        }

        public void Collaspe()
        {
            throw new NotImplementedException();
        }

        public bool CanRefreshDataSource
        {
            get { return true; }
        }

        public void RefreshDataSource()
        {
            using (WaitCursor cursor = new WaitCursor(true)) {
                try
                {
                    tlRole.BeginUpdate();
                    tlRole.ClearNodes();
                    TreeListNode node;
                    foreach (string role in Presenter.GetRoles())
                    {
                        node = tlRole.AppendNode(new object[] { role }, -1, 0, 0, 0);
                    }
                }
                finally {
                    tlRole.EndUpdate();
                }
            }
        }

        public event EventHandler<CancelDataHandlerEventArgs> DataInserting;

        public event EventHandler<DataHandlerEventArgs> DataInserted;

        public event EventHandler<CancelDataHandlerEventArgs> DataEditing;

        public event EventHandler<DataHandlerEventArgs> DataEdited;

        public event EventHandler<CancelDataHandlerEventArgs> DataDeleting;

        public event EventHandler<DataHandlerEventArgs> DataDeleted;

        public event EventHandler<EventArgs> DataRefreshed;

        #endregion

        private void MembershipRoleListView_Load(object sender, EventArgs e)
        {
            Initilize();
        }

        private void tlRole_AfterFocusNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            if (e.Node != null)
                OnCurrentRoleChanged(e.Node.GetDisplayText(colRoleName));
        }
    }
}
