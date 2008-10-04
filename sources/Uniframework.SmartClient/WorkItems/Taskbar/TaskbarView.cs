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
using Microsoft.Practices.ObjectBuilder;
using Uniframework.XtraForms.Workspaces;

namespace Uniframework.SmartClient
{
    [SmartPart]
    public partial class TaskbarView : DevExpress.XtraEditors.XtraUserControl
    {
        public TaskbarView()
        {
            InitializeComponent();
        }

        public XtraNavBarWorkspace TaskbarWorkspace
        {
            get { return taskbarWorkspace; }
        }

        private TaskbarPresenter presenter;
        [CreateNew]
        public TaskbarPresenter Presenter
        {
            get { return presenter; }
            set {
                presenter = value;
                presenter.View = this;
            }
        }

        private void TaskbarView_Load(object sender, EventArgs e)
        {
            Presenter.OnViewReady();
        }
    }
}
