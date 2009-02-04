using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Uniframework.SmartClient;
using Microsoft.Practices.ObjectBuilder;

namespace Uniframework.Common.WorkItems.Authorization
{
    public partial class CommandListView : DevExpress.XtraEditors.XtraUserControl, IDataListView
    {
        public CommandListView()
        {
            InitializeComponent();
        }

        private CommandListPresenter presenter;
        [CreateNew]
        public CommandListPresenter Presenter
        {
            get { return presenter; }
            set {
                presenter = value;
                presenter.View = this;
            }
        }

        #region IDataListView Members

        public IDataListHandler DataListHandler
        {
            get { return Presenter as IDataListHandler; }
        }

        public bool ReadOnly
        {
            get { return false; }
        }

        #endregion
    }
}
