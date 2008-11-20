using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI.Commands;
using Uniframework.DemoCenter.Client.Database;
using Uniframework.SmartClient;
using Microsoft.Practices.CompositeUI.WinForms;

namespace Uniframework.DemoCenter.Client
{
    public class DemoCenterController : WorkItemController
    {
        protected override void AddServices()
        {
            base.AddServices();
        }

        protected override void AddUIElements()
        {
            base.AddUIElements();
        }

        #region Command handlers

        [CommandHandler(CommandHandlerNames.CMD_SHOWDATABASEVIEW)]
        public void OnShowDatabaseModule(object sender, EventArgs e)
        {
            WindowSmartPartInfo spi = new WindowSmartPartInfo();
            spi.Title = "数据服务演示";

            ShowViewInWorkspace<DatabaseView>("DatabaseView", UIExtensionSiteNames.Shell_Workspace_Main, spi);
        }

        #endregion
    }
}
