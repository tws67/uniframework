using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using Uniframework.Upgrade.Views;
using Microsoft.Practices.CompositeUI.SmartParts;
using Uniframework.SmartClient;
using Microsoft.Practices.CompositeUI.WinForms;

namespace Uniframework.Upgrade
{
    public class CommandHandlers : Controller
    {
        /// <summary>
        /// 显示系统升级项目创建视图
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.ShowUpgradeBuilder)]
        public void OnShowUpgradeBuilder(object sender, EventArgs e)
        {
            UpgradeBuilderView view = WorkItem.SmartParts.Get<UpgradeBuilderView>(SmartPartNames.SmartPart_Upgrade_UpgradeBuilderView);
            if (view == null)
                view = WorkItem.SmartParts.AddNew<UpgradeBuilderView>(SmartPartNames.SmartPart_Upgrade_UpgradeBuilderView);

            IWorkspace wp = WorkItem.Workspaces.Get(UIExtensionSiteNames.Shell_Workspace_Main);
            if (wp != null) {
                WindowSmartPartInfo spi = new WindowSmartPartInfo();
                spi.Title = "创建升级项目";
                view.Dock = System.Windows.Forms.DockStyle.Fill;

                wp.Show(view, spi);
            }
        }
    }
}