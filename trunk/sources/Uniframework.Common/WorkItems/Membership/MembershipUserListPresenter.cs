using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Uniframework.Services;

namespace Uniframework.Common.WorkItems.Membership
{
    public class MembershipUserListPresenter : Presenter<MembershipUserListView>
    {
        #region Dependency Services

        [ServiceDependency]
        public IMembershipService MembershipService
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Refreshes the membership users.
        /// </summary>
        public void RefreshMembershipUsers()
        {
            using (WaitCursor cursor = new WaitCursor(true)) {
                View.UsersList.BeginUpdate();
                try
                {
                    MembershipUserCollection users = MembershipService.GetAllUsers();
                    View.SetDataSource(users);
                }
                finally
                {
                    View.UsersList.EndUpdate();
                }
            }
        }
    }
}
