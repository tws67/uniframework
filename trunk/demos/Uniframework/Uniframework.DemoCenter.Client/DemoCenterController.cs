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
using Uniframework.DemoCenter.Client.Views;

namespace Uniframework.DemoCenter.Client
{
    /// <summary>
    /// 演示中心控制器
    /// </summary>
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

        /// <summary>
        /// Called when [show sample view].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_SAMPLESERVICE)]
        public void OnShowSampleView(object sender, EventArgs e)
        {
            WindowSmartPartInfo spi = new WindowSmartPartInfo();
            spi.Title = "基本功能演示";

            ShowViewInWorkspace<SampleView>("SampleView", UIExtensionSiteNames.Shell_Workspace_Main, spi);
        }

        /// <summary>
        /// Called when [show database module].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
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
