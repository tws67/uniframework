using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using DevExpress.XtraBars;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI.SmartParts;
using Uniframework.SmartClient;
using Uniframework.XtraForms.UIElements;
using Uniframework.Upgrade.Views;
using Microsoft.Practices.CompositeUI.Common.Workspaces;
using DevExpress.XtraNavBar;
using Uniframework.SmartClient.WorkItems.Setting;
using Uniframework.Services;

namespace Uniframework.Upgrade
{
    public class UpgradeController : WorkItemController
    {
        protected override void AddServices()
        {
            base.AddServices();

            WorkItem.Services.AddNew<LiveUpgradeService, ILiveUpgradeService>();
            WorkItem.Items.AddNew<CommandHandlers>("CommandHandlers");
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

            BarButtonItem item = new BarButtonItem();
            item.Caption = "创建升级包(&B)";
            BarItemExtend extend = new BarItemExtend();
            extend.BeginGroup = true;
            extend.InsertBefore = "选项(&O)...";
            item.Tag = extend;

            Command cmd = WorkItem.Commands[CommandHandlerNames.CMD_SHOWUPGRADEBUILDER];
            if (cmd != null)
                cmd.AddInvoker(item, "ItemClick");
            if (WorkItem.UIExtensionSites.Contains(UIExtensionSiteNames.Shell_UI_Mainmenu_Tool))
                WorkItem.UIExtensionSites[UIExtensionSiteNames.Shell_UI_Mainmenu_Tool].Add(item);

            AddSettingItem();
        }

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
