using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI.EventBroker;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 关联任务导航栏工作项控制器
    /// </summary>
    public class TaskbarController : WorkItemController
    {
        /// <summary>
        /// Add custom services on this controller
        /// </summary>
        protected override void AddServices()
        {
            base.AddServices();

            WorkItem.RootWorkItem.Services.AddOnDemand<TaskbarService, ITaskbarService>();
        }

        /// <summary>
        /// Add custom views on this controller
        /// </summary>
        protected override void AddViews()
        {
            base.AddViews();

            TaskbarView view = WorkItem.SmartParts.AddNew<TaskbarView>(SmartPartNames.SmartPart_Shell_TaskbarView);
            WorkItem.RootWorkItem.Items.Add(view.TaskbarWorkspace, UIExtensionSiteNames.Shell_NaviPane_Taskbar);
            WorkItem.RootWorkItem.Workspaces.Add(view.TaskbarWorkspace, UIExtensionSiteNames.Shell_Workspace_Taskbar);
        }

        protected override void AddUIElements()
        {
            base.AddUIElements();
        }
   }
}
