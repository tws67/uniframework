using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.XtraNavBar;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;

namespace Uniframework.SmartClient
{
    public class TaskbarPresenter : Presenter<TaskbarView>
    {
        #region Dependency services

        [ServiceDependency]
        public IUIExtensionService UIExtensionService
        {
            get;
            set;
        }

        #endregion

        public override void OnViewReady()
        {
            base.OnViewReady();
        }
    }
}
