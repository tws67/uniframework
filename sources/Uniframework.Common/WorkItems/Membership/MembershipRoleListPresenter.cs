using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Uniframework.Services;
using Uniframework.SmartClient;
using DevExpress.XtraEditors;
using Uniframework.XtraForms.SmartPartInfos;
using DevExpress.XtraTreeList.Nodes;

namespace Uniframework.Common.WorkItems.Membership
{
    public class MembershipRoleListPresenter : DataListPresenter<MembershipRoleListView>
    {
        [ServiceDependency]
        public IMembershipService MembershipService
        {
            get;
            set;
        }

        public string[] GetRoles()
        {
            return MembershipService.GetAllRoles();
        }

        #region Override IDataListHandler

        public override void Initilize()
        {
            base.Initilize();
            RefreshDataSource();
        }
        /// <summary>
        /// 插入新的数据资料
        /// </summary>
        public override void Insert()
        {
            frmMembershipRole form = new frmMembershipRole();
            if (form.ShowDialog() == DialogResult.OK && !String.IsNullOrEmpty(form.Role))
            {
                // 检查待创建的角色名称是否已经存在
                if (MembershipService.RoleExists(form.Role))
                {
                    XtraMessageBox.Show("已经存在 \"" + form.Role + "\" 角色。");
                    return;
                }

                MembershipService.CreateRole(form.Role);
                View.RoleList.AppendNode(new object[] { form.Role }, -1, 0, 0, 0);
            }
        }

        /// <summary>
        /// 获取一个值决定当前可否编辑选定数据资料
        /// </summary>
        /// <value><c>true</c>如果可以编辑的话; 否则为, <c>false</c>.</value>
        /// 返回
        public override bool CanEdit
        {
            get { return View.RoleList.Selection.Count > 0; }
        }

        /// <summary>
        /// 编辑选定数据资料
        /// </summary>
        public override void Edit()
        {
            if (View.RoleList.Selection.Count > 0)
            {
                XtraWindowSmartPartInfo spi = new XtraWindowSmartPartInfo() {
                    MaximizeBox = false,
                    MinimizeBox = false,
                    Modal = true,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    StartPosition = FormStartPosition.CenterParent,
                    ShowInTaskbar = false,
                    Title = "属性"
                };
                WorkItem.State.Remove(Constants.CurrentRole);
                WorkItem.State[Constants.CurrentRole] = View.RoleList.FocusedNode.GetDisplayText(0);
                ShowViewInWorkspace<MembershipRoleView>(SmartPartNames.MembershipRoleView, UIExtensionSiteNames.Shell_Workspace_Window, spi);
            }
        }

        /// <summary>
        /// 获取一个值决定当前可否删除选定数据资料
        /// </summary>
        /// <value>返回<c>true</c>如果可以删除的话; 否则为, <c>false</c>.</value>
        public override bool CanDelete
        {
            get
            {
                bool flag = View.RoleList.Selection.Count > 0;
                flag &= View.RoleList.Selection[0].GetDisplayText(0) != Constants.DefaultAdminiStrators;
                return flag;
            }
        }

        /// <summary>
        /// 删除选定数据资料
        /// </summary>
        public override void Delete()
        {
            if (View.RoleList.Selection.Count > 0)
            {
                string role = View.RoleList.Selection[0].GetDisplayText(0); // 获取选定角色的名称
                if (XtraMessageBox.Show("您是否真的要从系统中删除角色 \"" + role + "\" ？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string[] users = MembershipService.GetUsersForRole(role);
                    if (users.Length > 0)
                        MembershipService.RemoveUsersFromRole(users, role); // 移除角色下的所有成员
                    MembershipService.DeleteRole(role);

                    // 更新视图
                    View.RoleList.DeleteNode(View.RoleList.Selection[0]); 
                }
            }
        }

        /// <summary>
        /// 刷新数据列表视图
        /// </summary>
        public override void RefreshDataSource()
        {
            using (WaitCursor cursor = new WaitCursor(true))
            {
                try
                {
                    View.RoleList.BeginUpdate();
                    View.RoleList.ClearNodes();
                    TreeListNode node;
                    foreach (string role in MembershipService.GetAllRoles())
                    {
                        node = View.RoleList.AppendNode(new object[] { role }, -1, 0, 0, 0);
                    }
                }
                finally
                {
                    View.RoleList.EndUpdate();
                }
            }
        }
        #endregion
    }
}
