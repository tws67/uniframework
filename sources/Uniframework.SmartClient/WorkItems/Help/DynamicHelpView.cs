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

namespace Uniframework.SmartClient
{
    public partial class DynamicHelpView : DevExpress.XtraEditors.XtraUserControl, IDynamicHelpView
    {
        public DynamicHelpView()
        {
            InitializeComponent();
        }

        private DynamicHelpPresenter presenter;
        [CreateNew]
        public DynamicHelpPresenter Presenter
        {
            get { return presenter; }
            set {
                presenter = value;
                presenter.View = this;
            }
        }

        #region IDynamicHelpView Members

        public void UpdateHelpUrl(Uri url)
        {
            throw new NotImplementedException();
        }

        public void HomeUrlChanged()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Dependency services

        [ServiceDependency]
        public IImageService ImageService
        {
            get;
            set;
        }

        #endregion

        private void DynamicHelpView_Load(object sender, EventArgs e)
        {
            btnBack.Glyph = ImageService.GetBitmap("nav_left_green", new Size(16, 16));
            btnForward.Glyph = ImageService.GetBitmap("nav_right_green", new Size(16, 16));
            btnHome.Glyph = ImageService.GetBitmap("nav_plain_green", new Size(16, 16));
            btnRefresh.Glyph = ImageService.GetBitmap("refresh", new Size(16, 16));

            webBrowser.CanGoBackChanged +=new EventHandler(WebBrowserChanged);
            webBrowser.CanGoForwardChanged +=new EventHandler(WebBrowserChanged);
        }

        private void btnBack_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            webBrowser.GoBack();
            Focus();
        }

        private void btnForward_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            webBrowser.GoForward();
            Focus();
        }

        private void btnHome_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            webBrowser.Url = Presenter.HomeUrl;
            Focus();
        }

        private void WebBrowserChanged(object sender, EventArgs e)
        {
            UpdateToolStripButtons();
        }

        private void UpdateToolStripButtons()
        {
            btnBack.Enabled = webBrowser.CanGoBack;
            btnForward.Enabled = webBrowser.CanGoForward;
            btnHome.Enabled = (Presenter.HomeUrl != null);
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            webBrowser.Refresh();
            Focus();
        }

        private void btnSaveAs_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            webBrowser.ShowSaveAsDialog();
        }
    }
}
