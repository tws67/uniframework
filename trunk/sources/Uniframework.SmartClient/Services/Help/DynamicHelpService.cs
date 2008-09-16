using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using System.IO;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 动太帮助服务
    /// </summary>
    public class DynamicHelpService : IDynamicHelpService
    {
        [ServiceDependency]
        public WorkItem WorkItem
        {
            get;
            set;
        }

        #region IDynamicHelpService Members

        public void ShowHelp(Assembly assembly, string helpUrl)
        {
            Uri url = TryCreateUri(helpUrl);

            if (url == null) {
                helpUrl = "file://" + Path.GetDirectoryName(assembly.Location) + "/" + helpUrl;
                url = TryCreateUri(helpUrl);
            }

            if (url == null) {
                helpUrl = "file://" + FileUtility.GetParent(FileUtility.ApplicationRootPath) + "/Help/" + helpUrl;
                url = TryCreateUri(helpUrl);
                if (url == null)
                    throw new ArgumentException("Help url invalid.");
            }

            OnHelpUriUpdated(new EventArgs<Uri>(url));    
        }

        private Uri TryCreateUri(string helpUrl)
        {
            Uri uri = null;
            try
            {
                uri = new Uri(helpUrl); ;
            }
            catch (UriFormatException) { }

            return uri;
        }

        #endregion

        [EventPublication(EventNames.Shell_DynamicHelpUpdated, PublicationScope.Global)]
        public event EventHandler<EventArgs<Uri>> HelpUriUpdated;

        protected void OnHelpUriUpdated(EventArgs<Uri> args)
        {
            if (HelpUriUpdated != null)
                HelpUriUpdated(this, args);
        }
    }
}
