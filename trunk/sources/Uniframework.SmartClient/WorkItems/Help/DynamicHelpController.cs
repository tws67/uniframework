using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.SmartParts;
using Uniframework.XtraForms.SmartPartInfos;

namespace Uniframework.SmartClient
{
    public class DynamicHelpController : WorkItemController
    {
        protected override void AddServices()
        {
            base.AddServices();

            WorkItem.RootWorkItem.Services.AddNew<DynamicHelpService, IDynamicHelp>();
        }

        /// <summary>
        /// 显示动态帮助视图
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_SHOW_DYNAMICHELP)]
        public void OnShowDynamicHelp(object sender, EventArgs e)
        {
            DynamicHelpView view = WorkItem.SmartParts.Get<DynamicHelpView>(SmartPartNames.SmartPart_Shell_DynamicHelp);
            if (view == null)
                view = WorkItem.SmartParts.AddNew<DynamicHelpView>(SmartPartNames.SmartPart_Shell_DynamicHelp);

            IWorkspace wp = WorkItem.Workspaces[UIExtensionSiteNames.Shell_Workspace_Dockable];
            if (wp != null) {
                DockManagerSmartPartInfo spi = new DockManagerSmartPartInfo();
                spi.Title = "动态帮助";
                spi.ImageFile = "help";
                spi.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;

                wp.Show(view, spi);
            }
        }
    }
}
