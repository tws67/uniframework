using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Practices.ObjectBuilder;

namespace Uniframework.DemoCenter.Client.Database
{
    public partial class DatabaseView : DevExpress.XtraEditors.XtraUserControl
    {
        public DatabaseView()
        {
            InitializeComponent();
        }

        private DatabasePresenter presenter;
        [CreateNew]
        public DatabasePresenter Presenter
        {
            get { return presenter; }
            set {
                presenter = value;
                presenter.View = this;
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                dataGrid.BeginUpdate();
                bsDocument.DataSource = Presenter.GetDocuments().Tables[0];
                dataGrid.MainView.PopulateColumns();
            }
            finally
            {
                dataGrid.EndUpdate();
            }
        }
    }
}
