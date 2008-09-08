using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI.WinForms;

namespace Microsoft.Practices.CompositeUI.Common.Workspaces
{
    public class WindowWorkspace : Microsoft.Practices.CompositeUI.WinForms.WindowWorkspace
    {
        IWin32Window owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowWorkspace"/> class.
        /// </summary>
        public WindowWorkspace()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowWorkspace"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public WindowWorkspace(IWin32Window owner)
            : base(owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Called when [show].
        /// </summary>
        /// <param name="smartPart">The smart part.</param>
        /// <param name="smartPartInfo">The smart part info.</param>
        protected override void OnShow(Control smartPart, WindowSmartPartInfo smartPartInfo)
        {
            GetOrCreateForm(smartPart);
            base.OnShow(smartPart, smartPartInfo);
        }

        /// <summary>
        /// Gets the or create form.
        /// </summary>
        /// <param name="smartPart">The smart part.</param>
        /// <returns></returns>
        protected new Form GetOrCreateForm(Control smartPart)
        {
            Form form = base.GetOrCreateForm(smartPart);
            form.ShowInTaskbar = (owner == null);
            form.FormClosing += new FormClosingEventHandler(form_FormClosing);

            return form;
        }

        /// <summary>
        /// Handles the FormClosing event of the form control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.FormClosingEventArgs"/> instance containing the event data.</param>
        private void form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form form = (Form)sender;

            Control smartPart = GetSmartPart(form);

            if (form.Controls.Count > 0)
                form.Controls.Remove(smartPart);

            if (SmartPartClosed != null)
                SmartPartClosed(this, new WorkspaceEventArgs(smartPart));
        }

        /// <summary>
        /// Gets the smart part.
        /// </summary>
        /// <param name="containerForm">The container form.</param>
        /// <returns></returns>
        private Control GetSmartPart(Form containerForm)
        {
            foreach (KeyValuePair<Control, Form> pair in this.Windows)
            {
                if (pair.Value == containerForm)
                    return pair.Key;
            }

            return null;
        }

        /// <summary>
        /// Occurs when [smart part closed].
        /// </summary>
        public event EventHandler<WorkspaceEventArgs> SmartPartClosed;
    }
}
