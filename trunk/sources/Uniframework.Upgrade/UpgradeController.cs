using System;
using System.Collections.Generic;
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

namespace Uniframework.Upgrade
{
    public class UpgradeController : WorkItemController
    {
        protected override void AddServices()
        {
            base.AddServices();

            WorkItem.Services.Add<ILiveUpgradeService>(new LiveUpgradeService());
            WorkItem.Items.AddNew<CommandHandlers>("CommandHandlers");

            WindowWorkspace wp = WorkItem.Workspaces.Get(UIExtensionSiteNames.Shell_Workspace_Main) as WindowWorkspace;
            if (wp != null)
            {
                wp.SmartPartClosed += new EventHandler<Microsoft.Practices.CompositeUI.SmartParts.WorkspaceEventArgs>(wp_SmartPartClosed);
            }
        }

        private void wp_SmartPartClosed(object sender, WorkspaceEventArgs e)
        {
            if (e.SmartPart is UpgradeBuilderView)
            {
                if (WorkItem.SmartParts.Contains(SmartPartNames.SmartPart_Upgrade_UpgradeBuilderView))
                    WorkItem.SmartParts.Remove(e.SmartPart);
            }
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

            Command cmd = WorkItem.Commands[CommandHandlerNames.ShowUpgradeBuilder];
            if (cmd != null)
                cmd.AddInvoker(item, "ItemClick");
            if (WorkItem.UIExtensionSites.Contains(UIExtensionSiteNames.Shell_UI_Mainmenu_View))
                WorkItem.UIExtensionSites[UIExtensionSiteNames.Shell_UI_Mainmenu_View].Add(item);
        }

        #region Dependendcy services

        [ServiceDependency]
        public IImageService ImageService
        {
            get;
            set;
        }

        #endregion
    }
}
