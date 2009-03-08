using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Uniframework.Security;
using Uniframework.Services;

namespace Uniframework.Common.WorkItems.Membership
{
    public class MembershipRolePresenter : Presenter<MembershipRoleView>
    {
        [ServiceDependency]
        public IMembershipService MembershipService
        {
            get;
            set;
        }

        [ServiceDependency]
        public IAuthorizationStoreService AuthorizationStoreService
        {
            get;
            set;
        }

        [ServiceDependency]
        public IAuthorizationNodeService AuthorizationNodeService
        {
            get;
            set;
        }
    }
}
