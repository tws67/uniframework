using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Uniframework.Client;

namespace Uniframework.SmartClient.Views
{
    public class BrowserPresenter : Presenter<BrowserView>
    {
        [ServiceDependency]
        public ISmartClient SmartClient
        {
            get;
            set;
        }

        public void ShowProgress(int progress)
        {
            SmartClient.ChangeProgress(progress);
        }
    }
}
