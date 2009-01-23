using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;

using Uniframework.Services;
using System.Web.Security;

namespace Uniframework.SmartClient.Views
{
    public class MembershipUserPresenter : Presenter<MembershipUserView>
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
                    //View.UsersList.DataSource = users;
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
