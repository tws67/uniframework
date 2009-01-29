using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Uniframework.Common.WorkItems.Membership;

namespace Uniframework.Common
{
    public class CommonController : WorkItemController
    {
        protected override void AddServices()
        {
            base.AddServices();

            ControlledWorkItem<MembershipController> membershipWorkItem = WorkItem.WorkItems.AddNew<ControlledWorkItem<MembershipController>>("MembershipController");
            membershipWorkItem.Run();
        }
    }
}
