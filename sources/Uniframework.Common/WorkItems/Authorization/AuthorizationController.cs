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
        [CommandHandler(CommandHandlerNames.CMD_COMM_AUTHORIZATION_COMMAND)]
        public void OnShowCommandView(object sender, EventArgs e)
        {
            WindowSmartPartInfo spi = new WindowSmartPartInfo() { 
                Title = "命令列表"
            };

            ShowViewInWorkspace<CommandListView>(SmartPartNames.AuthorizationCommandListView, UIExtensionSiteNames.Shell_Workspace_Main, spi);
        }
    }
}
