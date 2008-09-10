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
using Uniframework.Services;

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
            UpgradeBuilderView view = WorkItem.SmartParts.AddNew<UpgradeBuilderView>();
            IWorkspace wp = WorkItem.Workspaces.Get(UIExtensionSiteNames.Shell_Workspace_Main);
            if (wp != null) {
                WindowSmartPartInfo spi = new WindowSmartPartInfo();
                spi.Title = "创建升级包　";
                view.Dock = System.Windows.Forms.DockStyle.Fill;

                wp.Show(view, spi);
            }
        }

        /// <summary>
        /// 显示系统更新设置面板
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.ShowUpgradeSettingView)]
        public void OnShowUpgradeSettingView(object sender, EventArgs e)
        {
            UpgradeSettingView view = WorkItem.SmartParts.Get<UpgradeSettingView>("UpgradeSettingView");
            if (view == null)
                view = WorkItem.SmartParts.AddNew<UpgradeSettingView>("UpgradeSettingView");

            IWorkspace wp = WorkItem.Workspaces.Get(UIExtensionSiteNames.Shell_Workspace_SettingDeck);
            if (wp != null) {
                wp.Show(view);
                view.BindingProperty();
            }
        }

        /// <summary>
        /// 检查服务端是否有新的更新
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CheckUpgrade)]
        public void OnCheckUpgrade(object sender, EventArgs e)
        {
            ILiveUpgradeService upgradeService = WorkItem.Services.Get<ILiveUpgradeService>();
            if (upgradeService != null)
            {
                UpgradeProject proj = upgradeService.GetValidUpgradeProject();
                if (proj != null)
                    upgradeService.UpgradeNotify(proj);
            }
        }
    }
}