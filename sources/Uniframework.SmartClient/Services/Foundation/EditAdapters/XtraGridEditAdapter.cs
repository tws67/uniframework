using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views;
using DevExpress.XtraGrid.Views.Base;

namespace Uniframework.SmartClient
{
    public class XtraGridEditAdapter : IEditHandler
    {
        private GridControl adaptee;

        public XtraGridEditAdapter(GridControl adaptee)
        {
            this.adaptee = adaptee;
            RegisterEvent();
        }

        /// <summary>
        /// Registers the event.
        /// </summary>
        private void RegisterEvent()
        {
            adaptee.Enter += new EventHandler(AdapterEnter);
            adaptee.Leave += new EventHandler(AdapterLeave);
        }

        private void AdapterEnter(object sender, EventArgs e)
        {
            OnEnter(e);
        }

        private void AdapterLeave(object sender, EventArgs e)
        {
            OnLeave(e);
        }

        protected void OnEnter(EventArgs e)
        {
            if (Enter != null)
                Enter(this, e);
        }

        protected void OnLeave(EventArgs e)
        {
            if (Leave != null)
                Leave(this, e);
        }

        #region IEditHandler Members

        public event EventHandler Enter;

        public event EventHandler Leave;

        public bool CanUndo
        {
            get { return false; }
        }

        public void Undo()
        {
        }

        public bool CanRedo
        {
            get { return false; }
        }

        public void Redo()
        {
        }

        public bool CanCut
        {
            get { return false; }
        }

        public void Cut()
        {
        }

        public bool CanCopy
        {
            get { return (adaptee.DefaultView as ColumnView).SelectedRowsCount > 0; }
        }

        public void Copy()
        {
            adaptee.DefaultView.CopyToClipboard();
        }

        public bool CanPaste
        {
            get { return false; }
        }

        public void Paste()
        {
        }

        public bool CanDelete
        {
            get { return false; }
        }

        public void Delete()
        {
        }

        public bool CanSelectAll
        {
            get { return !(adaptee.DefaultView as ColumnView).IsEmpty; }
        }

        public void SelectAll()
        {
            (adaptee.DefaultView as ColumnView).SelectAll();
        }

        public bool CanSearch
        {
            get { return !(adaptee.DefaultView as ColumnView).IsEmpty; }
        }

        public void Search()
        {
            ColumnView view = adaptee.DefaultView as ColumnView;
            if (view != null && view.Columns.Count > 0) {
                view.ShowFilterEditor(view.Columns[0]);
            }
        }

        public bool CanReplace
        {
            get { return false; }
        }

        public void Replace()
        {
        }

        #endregion
    }
}
