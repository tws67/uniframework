using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.WinForms;
using Uniframework.SmartClient;

namespace Uniframework.Common.WorkItems.Authorization
{
    /// <summary>
    /// 系统授权管理控制器
    /// </summary>
    public class AuthorizationController : WorkItemController
    {
        /// <summary>
        /// 系统操作列表模板管理
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_COMM_AUTHORIZATION_COMMAND)]
        public void OnShowCommandView(object sender, EventArgs e)
        {
            WindowSmartPartInfo spi = new WindowSmartPartInfo() { 
                Title = "操作列表"
            };

            ShowViewInWorkspace<CommandListView>(SmartPartNames.AuthorizationCommandListView, UIExtensionSiteNames.Shell_Workspace_Main, spi);
        }

        /// <summary>
        /// 权限管理
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_COMM_AUTHORIZATION_STORE)]
        public void OnShowAuthorizationStore(object sender, EventArgs e)
        {
            WindowSmartPartInfo spi = new WindowSmartPartInfo() { 
                Title = "权限管理"
            };

            ShowViewInWorkspace<AuthorizationStoreListView>(SmartPartNames.AuthorizationStoreListView, UIExtensionSiteNames.Shell_Workspace_Main, spi);
        }
    }
}
