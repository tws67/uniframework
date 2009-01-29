using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Uniframework.Services;

namespace Uniframework.Common.WorkItems.Membership
{
    public class MembershipRoleListPresenter : Presenter<MembershipRoleListView>
    {
        [ServiceDependency]
        public IMembershipService MembershipService
        {
            get;
            set;
        }

        public string[] GetRoles()
        {
            return MembershipService.GetAllRoles();
        }
    }
}
