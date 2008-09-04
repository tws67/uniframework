using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;

namespace Uniframework.SmartClient.WorkItems.Setting
{
    /// <summary>
    /// 系统设置控制器
    /// </summary>
    public class SettingController : WorkItemController
    {
        /// <summary>
        /// Add custom services on this controller
        /// </summary>
        protected override void AddServices()
        {
            base.AddServices();

            WorkItem.Services.Add<ISettingService>(new SettingService());
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
        }
    }
}
