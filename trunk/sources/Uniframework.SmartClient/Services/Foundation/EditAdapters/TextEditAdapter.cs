using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// DevExpress 编辑控件适配器
    /// </summary>
    public class TextEditAdapter : IEditHandler
    {
        private TextEdit adaptee;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextEditAdapter"/> class.
        /// </summary>
        /// <param name="adaptee">The adaptee.</param>
        public TextEditAdapter(TextEdit adaptee)
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
            get { return adaptee.CanUndo; }
        }

        public void Undo()
        {
            adaptee.Undo();
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
            get { return adaptee.SelectionLength > 0; }
        }

        public void Cut()
        {
            adaptee.Cut();
        }

        public bool CanCopy
        {
            get { return adaptee.SelectionLength > 0; }
        }

        public void Copy()
        {
            adaptee.Copy();
        }

        public bool CanPaste
        {
            get {
                IDataObject data = Clipboard.GetDataObject();
                try {
                    return data != null && data.GetDataPresent(DataFormats.Text);
                }
                catch {
                    return false;
                }
            }
        }

        public void Paste()
        {
            adaptee.Paste();
        }

        public bool CanDelete
        {
            get { return adaptee.SelectionLength > 0; }
        }

        public void Delete()
        {
            adaptee.SelectedText = "";
        }

        public bool CanSelectAll
        {
            get { return adaptee.Text.Length > 0; }
        }

        public void SelectAll()
        {
            adaptee.SelectAll();
        }

        public bool CanSearch
        {
            get { return false; }
        }

        public void Search()
        {
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
