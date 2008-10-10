using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.ObjectBuilder;

namespace Uniframework.SmartClient.Views
{
    public partial class BrowserView : DevExpress.XtraEditors.XtraUserControl, IBrowser
    {
        public BrowserView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gotoes the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        public void Goto(Uri address)
        {
            webBrowser.Url = address;
            webBrowser.Focus();
        }

        #region Dependency services

        private BrowserPresenter presenter;
        [CreateNew]
        public BrowserPresenter Presenter
        {
            get { return presenter; }
            set
            {
                presenter = value;
                presenter.View = this;
            }
        }

        [ServiceDependency]
        public IBrowserService BrowserService
        {
            get;
            set;
        }

        #endregion

        #region IBrowser Members

        public event EventHandler Actived;

        public event EventHandler Deactived;

        public event EventHandler Closed;

        public bool CanBack
        {
            get { return webBrowser.CanGoBack; }
        }

        public void Back()
        {
            webBrowser.GoBack();
            webBrowser.Focus();
        }

        public bool CanForward
        {
            get { return webBrowser.CanGoForward; }
        }

        public void Forward()
        {
            webBrowser.GoForward();
            webBrowser.Focus();
        }

        public bool CanStop
        {
            get { return webBrowser.IsBusy; }
        }

        public void Stop()
        {
            webBrowser.Stop();
            webBrowser.Focus();
        }

        public bool CanHome
        {
            get { return String.IsNullOrEmpty(BrowserService.HomeUri); }
        }

        public void Home()
        {
            webBrowser.Url = new Uri(BrowserService.HomeUri);
            webBrowser.Focus();
        }

        public bool CanRefresh
        {
            get { return true; }
        }

        void IBrowser.Refresh()
        {
            webBrowser.Refresh();
            webBrowser.Focus();
        }

        #endregion

        private void webBrowser_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            Presenter.ShowProgress((int)(e.CurrentProgress / e.MaximumProgress));
        }

        private void webBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            Presenter.ShowProgress(-1);
        }

        private void BrowserView_Load(object sender, EventArgs e)
        {
            webBrowser.Dock = DockStyle.Fill;
        }
    }
}
