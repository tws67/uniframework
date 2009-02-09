using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DevExpress.XtraBars;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Uniframework.Common.WorkItems.Membership;
using Uniframework.SmartClient;
using Uniframework.Common.WorkItems.Authorization;

namespace Uniframework.Common
{
    public class CommonController : WorkItemController
    {
        [ServiceDependency]
        public IUIExtensionService UIExtensionService
        {
            get;
            set;
        }

        protected override void AddServices()
        {
            base.AddServices();

            // 成员管理
            ControlledWorkItem<MembershipController> membershipWorkItem = WorkItem.WorkItems.AddNew<ControlledWorkItem<MembershipController>>("MembershipController");
            membershipWorkItem.Run();

            // 权限管理
            ControlledWorkItem<AuthorizationController> authWorkItem = WorkItem.WorkItems.AddNew<ControlledWorkItem<AuthorizationController>>("AuthorizationController");
            authWorkItem.Run();
        }

        protected override void AddUIElements()
        {
            base.AddUIElements();

            //BarItem item = UIExtensionService.AddButton("/Shell/Bar/Standard", "CurrentUser", Thread.CurrentPrincipal.Identity.Name, "", true);
            //item.Alignment = BarItemLinkAlignment.Right;
        }
    }
}
