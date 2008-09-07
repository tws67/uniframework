using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Practices.ObjectBuilder;

namespace Uniframework.Upgrade.Views
{
    public partial class UpgradeBuilderView : DevExpress.XtraEditors.XtraUserControl
    {
        public UpgradeBuilderView()
        {
            InitializeComponent();
        }

        private UpgradeBuilderPresenter presenter;
        [CreateNew]
        public UpgradeBuilderPresenter Presenter
        {
            get { return presenter; }
            set {
                presenter = value;
                presenter.View = this;
            }
        }
    }
}
