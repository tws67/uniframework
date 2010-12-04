using System;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;

using Uniframework.SmartClient;

namespace Uniframework.LocalStartUp
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

            ControlledWorkItem<DataListController> datalistWorkItem = WorkItem.WorkItems.AddNew<ControlledWorkItem<DataListController>>();
            datalistWorkItem.Run();
        }

        #region Assistant functions

        private void WorkItem_Activated(object sender, EventArgs e)
        {
            //Program.Logger.Debug("启动离线服务的连接管理线程");
            //Program.SetInitialState("启动离线服务……");
            //Program.IncreaseProgressBar(10);
            //ServiceRepository.Instance.Start();
            //Program.CloseLoginForm();
        }

        #endregion

    }
}
