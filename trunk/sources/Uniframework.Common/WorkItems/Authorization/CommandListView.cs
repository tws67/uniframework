using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Uniframework.SmartClient;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.ObjectBuilder;
using DevExpress.XtraGrid;
using Uniframework.Security;

namespace Uniframework.Common.WorkItems.Authorization
{
    [AuthResource("权限管理", "/Shell/Module/Foundation/Common/Authorization/Command")]
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

        public GridControl DataGrid
        {
            get { return dataGrid; }
        }

        public BindingSource DataSource
        {
            get { return bsCommands; }
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

        /// <summary>
        /// 触发当前命令项变化事件
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void bsCommands_CurrentChanged(object sender, EventArgs e)
        {
            AuthorizationCommand cmd = bsCommands.Current as AuthorizationCommand;
            if (cmd != null) {
                Presenter.CurrentCommand = cmd;
                if (CurrentCommandChanged != null)
                    CurrentCommandChanged(this, new EventArgs<AuthorizationCommand>(cmd));

                Presenter.WorkItem.State.Remove(Constants.CurrentCommand);
                Presenter.WorkItem.State[Constants.CurrentCommand] = cmd;
            }
        }

        [EventPublication(EventNames.Authorization_CurrentCommandChanged, PublicationScope.WorkItem)]
        public event EventHandler<EventArgs<AuthorizationCommand>> CurrentCommandChanged;
    }
}
