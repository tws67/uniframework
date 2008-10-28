using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI.WinForms;
using Uniframework.SmartClient.Views;
using DevExpress.XtraEditors;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 浏览器服务
    /// </summary>
    [Service]
    public class BrowserService : WorkItemController, IBrowserService
    {
        private IBrowser browser = null;
        private string homeUri = null;
        private WorkItem workItem;

        public BrowserService() {
            homeUri = ConfigurationManager.AppSettings["ShellHomeUri"];
            Application.Idle += new EventHandler(Application_Idle);
        }

        #region BrowserService members

        private IBrowser Browser
        {
            get { return browser; }
            set {
                browser = value;
                if (browser != null) {
                    browser.Actived += new EventHandler(Browser_Actived);
                    browser.Deactived += new EventHandler(Browser_Deactived);
                    browser.Closed += new EventHandler(Browser_Closed);
                    UpdateCommandStatus();
                }
            }
        }

        /// <summary>
        /// Gotoes the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        public void Goto(string address)
        {
            Uri uri = new Uri(address);
            BrowserView view = ShowViewInWorkspace<BrowserView>(SmartPartNames.SmartPart_Shell_BrowserView,
                UIExtensionSiteNames.Shell_Workspace_Main);

            try {
                view.Goto(uri);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        public string HomeUri
        {
            get { return homeUri; }
        }

        #endregion

        #region Command and event handler

        [CommandHandler(CommandHandlerNames.CMD_VIEW_BACK)]
        public void OnBack(object sender, EventArgs e)
        {
            Guard.ArgumentNotNull(Browser, "Browser");
            Browser.Back();
        }

        [CommandHandler(CommandHandlerNames.CMD_VIEW_FORWARD)]
        public void OnForward(object sender, EventArgs e)
        {
            Guard.ArgumentNotNull(Browser, "Browser");
            Browser.Forward();
        }

        [CommandHandler(CommandHandlerNames.CMD_VIEW_STOP)]
        public void OnStop(object sender, EventArgs e)
        {
            Guard.ArgumentNotNull(Browser, "Browser");
            Browser.Stop();
        }

        [CommandHandler(CommandHandlerNames.CMD_VIEW_HOME)]
        public void OnHome(object sender, EventArgs e)
        {
            Goto(HomeUri);
        }

        [CommandHandler(CommandHandlerNames.CMD_VIEW_REFRESH)]
        public void OnRefresh(object sender, EventArgs e)
        {
            Guard.ArgumentNotNull(Browser, "Browser");
            Browser.Refresh();
        }

        [EventSubscription(EventNames.Shell_AddressUriChanged, ThreadOption.UserInterface)]
        public void OnShellAddressUriChanged(object sender, EventArgs<string> e)
        {
            Goto(e.Data);
        }

        #endregion

        #region Assistant functions

        private void Application_Idle(object sender, EventArgs e)
        {
            UpdateCommandStatus();
        }

        private void Browser_Deactived(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Browser_Actived(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Browser_Closed(object sender, EventArgs e)
        {
            Browser.Actived -= Browser_Actived;
            Browser.Deactived -= Browser_Deactived;
            Browser.Closed -= Browser_Closed;
            Browser = null;
        }

        private void UpdateCommandStatus()
        {
            bool enabled = Browser != null;

            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_VIEW_BACK, enabled && Browser.CanBack);
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_VIEW_FORWARD, enabled && Browser.CanForward);
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_VIEW_STOP, enabled && Browser.CanStop);
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_VIEW_HOME, !String.IsNullOrEmpty(HomeUri));
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_VIEW_REFRESH, enabled && Browser.CanRefresh);
        }

        #endregion
    }
}
