using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using DevExpress.XtraBars;
using DevExpress.XtraNavBar;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI.WinForms;

using Uniframework.Services;
using Uniframework.SmartClient;
using Uniframework.SmartClient.WorkItems.Setting;
using Uniframework.Upgrade.Views;
using Uniframework.XtraForms.UIElements;

namespace Uniframework.Upgrade
{
    public class UpgradeController : WorkItemController
    {
        protected override void AddServices()
        {
            base.AddServices();

            WorkItem.Services.AddNew<LiveUpgradeService, ILiveUpgradeService>();
        }

        protected override void AddViews()
        {
            base.AddViews();

            UpgradeSettingView view = WorkItem.SmartParts.AddNew<UpgradeSettingView>("UpgradeSettingView");
            SettingService.RegisterSetting(view);
        }
        /// <summary>
        /// 添加菜单项到系统中
        /// </summary>
        protected override void AddUIElements()
        {
            base.AddUIElements();
            AddSettingItem();
        }

        #region Command handlers

        /// <summary>
        /// 显示系统升级项目创建视图
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_SHOWUPGRADEBUILDER)]
        public void OnShowUpgradeBuilder(object sender, EventArgs e)
        {
            ShowViewInWorkspace<UpgradeBuilderView>("UpgradeBuilder", UIExtensionSiteNames.Shell_Workspace_Main);
        }

        /// <summary>
        /// 显示系统更新设置面板
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_SHOWUPGRADESETTINGVIEW)]
        public void OnShowUpgradeSettingView(object sender, EventArgs e)
        {
            UpgradeSettingView view = WorkItem.SmartParts.Get<UpgradeSettingView>("UpgradeSettingView");
            if (view == null)
                view = WorkItem.SmartParts.AddNew<UpgradeSettingView>("UpgradeSettingView");

            IWorkspace wp = WorkItem.Workspaces.Get(UIExtensionSiteNames.Shell_Workspace_SettingDeck);
            if (wp != null)
            {
                wp.Show(view);
                view.BindingProperty();
            }
        }

        /// <summary>
        /// 检查服务端是否有新的更新
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(Uniframework.SmartClient.CommandHandlerNames.CMD_HELP_CHECKUPGRADE)]
        public void OnCheckUpgrade(object sender, EventArgs e)
        {
            ILiveUpgradeService upgradeService = WorkItem.Services.Get<ILiveUpgradeService>();
            if (upgradeService != null)
                using (WaitCursor cursor = new WaitCursor(true)) {
                    UpgradeProject proj = upgradeService.GetValidUpgradeProject();
                    if (proj != null)
                        upgradeService.UpgradeNotify(proj);
                }
        }

        #endregion

        #region Dependendcy services

        [ServiceDependency]
        public IImageService ImageService
        {
            get;
            set;
        }

        [ServiceDependency]
        public ISettingService SettingService
        {
            get;
            set;
        }

        #endregion

        private void AddSettingItem()
        {
            NavBarItem settingItem = new NavBarItem("更新");
            settingItem.LargeImage = ImageService.GetBitmap("download", new System.Drawing.Size(32, 32));
            settingItem.SmallImage = ImageService.GetBitmap("download", new System.Drawing.Size(16, 16));
            WorkItem.UIExtensionSites[UIExtensionSiteNames.Shell_UI_NaviPane_DefaultSetting].Add(settingItem);
            Command cmd = WorkItem.Commands[CommandHandlerNames.CMD_SHOWUPGRADESETTINGVIEW];
            if (cmd != null)
                cmd.AddInvoker(settingItem, "LinkClicked");
        }
    }
}
