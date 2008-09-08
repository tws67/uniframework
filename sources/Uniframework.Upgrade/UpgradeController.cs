using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using DevExpress.XtraBars;
using Uniframework.XtraForms.UIElements;
using Microsoft.Practices.CompositeUI.Commands;
using Uniframework.SmartClient;

namespace Uniframework.Upgrade
{
    public class UpgradeController : WorkItemController
    {
        protected override void AddServices()
        {
            base.AddServices();

            WorkItem.Services.Add<ILiveUpgradeService>(new LiveUpgradeService());
            WorkItem.Items.AddNew<CommandHandlers>("CommandHandlers");
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
