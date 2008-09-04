using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraNavBar;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI.WinForms;
using Microsoft.Practices.ObjectBuilder;
using Uniframework.XtraForms.Workspaces;

namespace Uniframework.SmartClient.WorkItems.Setting
{
    [SmartPart]
    public partial class SettingView : DevExpress.XtraEditors.XtraUserControl
    {
        private SettingPresenter presenter;

        public SettingView()
        {
            InitializeComponent();
        }

        [CreateNew]
        public SettingPresenter Presenter
        {
            get { return presenter; }
            set {
                presenter = value;
                presenter.View = this;
            }
        }

        public NavBarGroup DefaultSettingGroup
        {
            get { return defaultGroup; }
        }

        public XtraNavBarWorkspace SettingNaviWorkspace
        {
            get { return naviWorkspace; }
        }

        public DeckWorkspace SettingDeckWorkspace
        {
            get { return deckWorkspace; }
        }

        #region Assistant functions

        private void SettingView_Load(object sender, EventArgs e)
        {
            btnOK.Click += new EventHandler(btnOK_Click);
            btnDefault.Click += new EventHandler(btnDefault_Click);
        }

        private void btnDefault_Click(object sender, EventArgs e)
        {
            Presenter.LoadDefault();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Presenter.Save();
        }

        #endregion
    }
}
