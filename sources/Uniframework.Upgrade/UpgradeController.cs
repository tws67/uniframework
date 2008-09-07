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

        protected override void AddUIElements()
        {
            base.AddUIElements();

            BarButtonItem item = new BarButtonItem();
            item.Caption = "创建系统升级项目(&B)...";
            BarItemExtend extend = new BarItemExtend();
            extend.BeginGroup = true;
            extend.InsertBefore = "选项(&O)...";
            item.Tag = extend;

            Command cmd = WorkItem.Commands[CommandHandlerNames.ShowUpgradeBuilder];
            if (cmd != null)
                cmd.AddInvoker(item, "ItemClick");
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
