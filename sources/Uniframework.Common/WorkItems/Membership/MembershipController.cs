using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.Common.SmartPartInfo;
using Uniframework.SmartClient;

namespace Uniframework.Common.WorkItems.Membership
{
    /// <summary>
    /// 框架成员管理控制器
    /// </summary>
    public class MembershipController : WorkItemController
    {
        /// <summary>
        /// 显示系统用户管理视图
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_COMM_MEMBERSHIPUSER)]
        public void OnShowMembershipUserList(object sender, EventArgs e)
        {
            WindowSmartPartInfo spi = new WindowSmartPartInfo();
            spi.Title = "用户管理";

            ShowViewInWorkspace<MembershipUserListView>(SmartPartNames.MembershipUserListView, UIExtensionSiteNames.Shell_Workspace_Main, spi);
        }

        /// <summary>
        /// 显示系统角色管理视图
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_COMM_MEMBERSHIPROLE)]
        public void OnShowMembershipRoleList(object sender, EventArgs e)
        {
            WindowSmartPartInfo spi = new WindowSmartPartInfo();
            spi.Title = "角色管理";

            ShowViewInWorkspace<MembershipRoleListView>(SmartPartNames.MembershipRoleListView, UIExtensionSiteNames.Shell_Workspace_Main, spi);
        }
    }
}
