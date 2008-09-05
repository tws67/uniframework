using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using DevExpress.XtraNavBar;
using Microsoft.Practices.CompositeUI.Commands;

namespace Uniframework.SmartClient.WorkItems.Setting
{
    /// <summary>
    /// 系统设置控制器
    /// </summary>
    public class SettingController : WorkItemController
    {
        private ISettingService settingService;
        /// <summary>
        /// Add custom services on this controller
        /// </summary>
        protected override void AddServices()
        {
            base.AddServices();

            WorkItem.Services.Add<ISettingService>(new SettingService());
            settingService = WorkItem.Services.Get<ISettingService>();
        }

        /// <summary>
        /// Add custom views on this controller
        /// </summary>
        protected override void AddViews()
        {
            base.AddViews();

            SettingView view = WorkItem.SmartParts.AddNew<SettingView>(SmartPartNames.SmartPart_Shell_SettingView);
            WorkItem.Workspaces.Add(view.SettingDeckWorkspace, UIExtensionSiteNames.Shell_Workspace_SettingDeck);
            WorkItem.Workspaces.Add(view.SettingNaviWorkspace, UIExtensionSiteNames.Shell_Workspace_SettingNavi);
            WorkItem.UIExtensionSites.RegisterSite(UIExtensionSiteNames.Shell_UI_NaviPane_DefaultSetting, view.DefaultSettingGroup);

            //ISettingService settingService = WorkItem.Services.AddNew(typeof(ISettingService), typeof(SettingService)) as ISettingService;
            ShellLayoutSettingView layoutView = WorkItem.SmartParts.AddNew<ShellLayoutSettingView>(SmartPartNames.SmartPart_Shell_LayoutSettingView);
            settingService.RegisterSetting(layoutView);
        }

        protected override void AddUIElements()
        {
            base.AddUIElements();

            NavBarItem item = new NavBarItem("系统外观");
            item.LargeImage = ImageService.GetBitmap("cubes", new System.Drawing.Size(32, 32));
            item.SmallImage = ImageService.GetBitmap("cubes", new System.Drawing.Size(16, 16));
            WorkItem.UIExtensionSites[UIExtensionSiteNames.Shell_UI_NaviPane_DefaultSetting].Add(item);
            Command cmd = WorkItem.Commands[CommandHandlerNames.CMD_SHOW_SHELLL_AYOUTSETTING];
            if (cmd != null)
                cmd.AddInvoker(item, "LinkClicked");
            cmd.Execute();
        }

        [ServiceDependency]
        public IImageService ImageService
        {
            get;
            set;
        }
    }
}
