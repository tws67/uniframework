using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Uniframework.Services;
using Uniframework.SmartClient;
using Uniframework.XtraForms.SmartPartInfos;
using Microsoft.Practices.CompositeUI.Commands;
using DevExpress.XtraEditors;

namespace Uniframework.Common.WorkItems.Membership
{
    public class MembershipUserListPresenter : DataListPresenter<MembershipUserListView>
    {
        #region Dependency Services

        [ServiceDependency]
        public IMembershipService MembershipService
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Refreshes the membership users.
        /// </summary>
        public void RefreshMembershipUsers()
        {
            using (WaitCursor cursor = new WaitCursor(true)) {
                View.UsersList.BeginUpdate();
                try
                {
                    MembershipUserCollection users = MembershipService.GetAllUsers();
                    View.SetDataSource(users);
                }
                finally
                {
                    View.UsersList.EndUpdate();
                }
            }
        }

        public override void OnViewReady()
        {
            base.OnViewReady();
            RefreshMembershipUsers();
        }

        /// <summary>
        /// 刷新数据列表视图
        /// </summary>
        public override void RefreshDataSource()
        {
            base.RefreshDataSource();
            RefreshMembershipUsers();
        }

        /// <summary>
        /// 插入新的数据资料
        /// </summary>
        public override void Insert()
        {
            base.Insert();

            XtraWindowSmartPartInfo spi = new XtraWindowSmartPartInfo() { 
                MaximizeBox = false,
                MinimizeBox = false,
                Modal = true,
                ShowInTaskbar = false,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                Title = "新建"
            };
            ShowViewInWorkspace<MembershipUserView>(SmartPartNames.MembershipUserView, UIExtensionSiteNames.Shell_Workspace_Window, spi);
            RefreshDataSource();
        }

        /// <summary>
        /// 获取一个值决定当前可否编辑选定数据资料
        /// </summary>
        /// <value><c>true</c>如果可以编辑的话; 否则为, <c>false</c>.</value>
        /// 返回
        public override bool CanEdit
        {
            get
            {
                bool flag = base.CanEdit;
                flag &= View.UsersList.Nodes.Count > 0 && View.UsersList.Selection.Count > 0;
                return flag;
            }
        }

        /// <summary>
        /// 编辑选定数据资料
        /// </summary>
        public override void Edit()
        {
            base.Edit();

            XtraWindowSmartPartInfo spi = new XtraWindowSmartPartInfo() { 
                MaximizeBox = false,
                MinimizeBox = false,
                Modal = true,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                Title = "属性",
                ShowInTaskbar = false
            };

            WorkItem.State.Remove(Constants.CurrentUser);
            WorkItem.State[Constants.CurrentUser] = View.UsersList.Selection[0].GetDisplayText(0); // 获取当前选定的用户名称
            ShowViewInWorkspace<MembershipUserEditView>(SmartPartNames.MembershipUserEditView, UIExtensionSiteNames.Shell_Workspace_Window, spi);
        }

        /// <summary>
        /// 获取一个值决定当前可否删除选定数据资料
        /// </summary>
        /// <value>返回<c>true</c>如果可以删除的话; 否则为, <c>false</c>.</value>
        public override bool CanDelete
        {
            get
            {
                bool flag = base.CanDelete;
                flag &= View.UsersList.Nodes.Count > 0 && View.UsersList.Selection.Count > 0;
                if (View.UsersList.Selection.Count > 0) {
                    flag &= View.UsersList.Selection[0].GetDisplayText(0) != Constants.DefaultAdministrator;
                }
                return flag;
            }
        }

        /// <summary>
        /// 删除选定数据资料
        /// </summary>
        public override void Delete()
        {
            base.Delete();

            if (View.UsersList.Selection.Count > 0) {
                string username = View.UsersList.Selection[0].GetDisplayText(0); // 获取当前帐户名称
                if (XtraMessageBox.Show("您真的要删除用户 \"" + username + "\" 的资料及其关联的角色信息吗？", "提示", 
                    MessageBoxButtons.YesNo) == DialogResult.Yes) {
                        string[] roles = MembershipService.GetRolesForUser(username);
                        try
                        {
                            if (roles.Length > 0)
                                MembershipService.RemoveUserFromRoles(username, roles);
                            MembershipService.DeleteUser(username);
                        }
                        catch (Exception ex) {
                            XtraMessageBox.Show(String.Format("删除用户资料 \"{0}\" 出错，{1}", username, ex.Message));
                        }

                        // 从当前视图中删除用户节点
                        View.UsersList.DeleteNode(View.UsersList.Selection[0]); 
                }
            }
        }
        /// <summary>
        /// 修改指定帐户的密码
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_COMM_MEMBERSHIP_SETPASSWORD)]
        public void OnSetPassword(object sender, EventArgs e)
        {
            if (View.UsersList.Selection.Count > 0) {
                frmSetPassword form = WorkItem.Items.Get<frmSetPassword>(Constants.SetPasswordForm);
                if (form == null)
                    form = WorkItem.Items.AddNew<frmSetPassword>(Constants.SetPasswordForm);
                form.UserName = View.UsersList.Selection[0].GetDisplayText(0); // 直接获取当前选定帐户的名称
                form.ShowDialog();
            }
        }
    }
}
