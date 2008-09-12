using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI.Commands;

using Uniframework.Client;
using Uniframework.SmartClient.WorkItems.Setting;
using Uniframework.SmartClient;
using DevExpress.XtraNavBar;
using Uniframework.Services;

namespace Uniframework.StartUp
{
    public class RootController : WorkItemController
    {
        public override void OnRunStarted()
        {
            WorkItem.Activated += new EventHandler(WorkItem_Activated);
            base.OnRunStarted();
        }

        protected override void AddServices()
        {
            base.AddServices();

            ControlledWorkItem<DynamicHelpController> dynamicWorkItem = WorkItem.WorkItems.AddNew<ControlledWorkItem<DynamicHelpController>>();
            dynamicWorkItem.Run();
        }

        #region Assistant functions

        private void WorkItem_Activated(object sender, EventArgs e)
        {
            Program.Logger.Debug("启动离线服务的连接管理线程");
            Program.SetInitialState("启动离线服务……");
            Program.IncreaseProgressBar(10);
            ServiceRepository.Instance.Start();
            Program.CloseLoginForm();
        }

        #endregion

    }
}
