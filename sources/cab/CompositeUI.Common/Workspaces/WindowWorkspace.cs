using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI.Utility;
using Microsoft.Practices.CompositeUI.WinForms;

namespace Microsoft.Practices.CompositeUI.Common
{
    public class WindowWorkspace : Workspace<Control, WindowSmartPartInfo>
    {
        private IWin32Window owner;
        private bool fireActivatedFromForm = true;
        private Dictionary<Control, Form> windows = new Dictionary<Control, Form>();

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
        {
            this.owner = owner;
        }

        public ReadOnlyDictionary<Control, Form> Windows
        {
            get { return new ReadOnlyDictionary<Control, Form>(windows); }
        }

        #region Protected methods

        protected override void OnActivate(Control smartPart)
        {
            try
            {
                fireActivatedFromForm = false;	// Prevent double firing from composer Workspace class and form
                Form form = windows[smartPart];
                form.BringToFront();
                form.Show();
            }
            finally
            {
                fireActivatedFromForm = true;
            }
        }

        protected override void OnApplySmartPartInfo(Control smartPart, WindowSmartPartInfo smartPartInfo)
        {
            Form form = windows[smartPart];
            SetWindowProperties(form, smartPartInfo);
            SetWindowLocation(form, smartPartInfo);
        }

        protected override void OnShow(Control smartPart, WindowSmartPartInfo smartPartInfo)
        {
            Form form = GetOrCreateForm(smartPart);
            smartPart.Show();
            ShowForm(form, smartPartInfo);
        }

        protected override void OnHide(Control smartPart)
        {
            Form form = windows[smartPart];
            form.Hide();
        }

        protected override void OnClose(Control smartPart)
        {
            Form form = windows[smartPart];
            form.Controls.Remove(smartPart);	// Remove the smartPart from the form to avoid disposing it.
            smartPart.Disposed -= ControlDisposed;

            form.Close();
            windows.Remove(smartPart);
        }

        /// <summary>
        /// 获取或创建SmartPart需要的窗体
        /// </summary>
        /// <param name="control">Smart part control.</param>
        /// <returns></returns>
        protected Form GetOrCreateForm(Control control)
        {
            WindowForm form = null;
            if (windows.ContainsKey(control)) {
                form = windows[control] as WindowForm;
            }
            else {
                form = new WindowForm();
                form.ShowInTaskbar = owner == null;
                windows.Add(control, form);
                form.Controls.Add(control);
                control.Dock = DockStyle.Fill;
                control.Disposed += ControlDisposed;
                WireUpForm(form);
            }

            return form;
        }

        protected void SetWindowProperties(Form form, WindowSmartPartInfo spi)
        {
            form.ControlBox = spi.ControlBox;
            form.MaximizeBox = spi.MaximizeBox;
            form.MinimizeBox = spi.MinimizeBox;
            form.Text = spi.Title;
            form.Icon = spi.Icon;
            form.Height = spi.Height;
            form.Width = spi.Width;
        }

        protected void SetWindowLocation(Form form, WindowSmartPartInfo spi)
        {
            if (form.StartPosition != FormStartPosition.CenterParent)
            {
                form.Location = spi.Location;
            }
        }

        #endregion

        #region Private

        private void ControlDisposed(object sender, EventArgs e)
        {
            Control control = sender as Control;
            if (control != null && SmartParts.Contains(sender))
            {
                CloseInternal(control);
            }
        }

        private void WireUpForm(WindowForm form)
        {
            form.WindowFormClosing += new EventHandler<WorkspaceCancelEventArgs>(WindowFormClosing);
            form.WindowFormClosed += new EventHandler<WorkspaceEventArgs>(WindowFormClosed);
            form.WindowFormActivated += new EventHandler<WorkspaceEventArgs>(WindowFormActivated);
        }

        private void WindowFormActivated(object sender, WorkspaceEventArgs e)
        {
            if (fireActivatedFromForm)
            {
                RaiseSmartPartActivated(e.SmartPart);
                SetActiveSmartPart(e.SmartPart);
            }
        }

        private void WindowFormClosed(object sender, WorkspaceEventArgs e)
        {
            windows.Remove((Control)e.SmartPart);
            InnerSmartParts.Remove((Control)e.SmartPart);
        }

        private void WindowFormClosing(object sender, WorkspaceCancelEventArgs e)
        {
            RaiseSmartPartClosing(e);
        }

        private void ShowForm(Form form, WindowSmartPartInfo spi)
        {
            SetWindowProperties(form, spi);

            if (spi.Modal) {
                SetWindowLocation(form, spi); // Argument can be null. It's the default for the other overload.
                form.ShowDialog(owner);
            }
            else {
                if (owner != null) {
                    form.Show(owner);
                }
                else {
                    form.Show();
                }
                SetWindowLocation(form, spi);
                form.BringToFront();
            }
        }

        #endregion

        #region Private Form Class

        /// <summary>
        /// WindowForm class
        /// </summary>
        private class WindowForm : Form
        {
            /// <summary>
            /// Fires when form is closing
            /// </summary>
            public event EventHandler<WorkspaceCancelEventArgs> WindowFormClosing;

            /// <summary>
            /// Fires when form is closed
            /// </summary>
            public event EventHandler<WorkspaceEventArgs> WindowFormClosed;

            /// <summary>
            /// Fires when form is activated
            /// </summary>
            public event EventHandler<WorkspaceEventArgs> WindowFormActivated;

            /// <summary>
            /// Handles Activated Event.
            /// </summary>
            /// <param name="e"></param>
            protected override void OnActivated(EventArgs e)
            {
                if (this.Controls.Count > 0 && WindowFormActivated != null)
                {
                    this.WindowFormActivated(this, new WorkspaceEventArgs(this.Controls[0]));
                }

                base.OnActivated(e);
            }


            /// <summary>
            /// Handles the Closing Event
            /// </summary>
            /// <param name="e"></param>
            protected override void OnClosing(CancelEventArgs e)
            {
                if (this.Controls.Count > 0)
                {
                    WorkspaceCancelEventArgs cancelArgs = FireWindowFormClosing(this.Controls[0]);
                    e.Cancel = cancelArgs.Cancel;

                    if (cancelArgs.Cancel == false) {
                        this.Controls[0].Hide();
                    }
                }

                base.OnClosing(e);
            }

            /// <summary>
            /// Handles the Closed Event
            /// </summary>
            /// <param name="e"></param>
            protected override void OnClosed(EventArgs e)
            {
                if ((WindowFormClosed != null) &&
                    (Controls.Count > 0))
                {
                    WindowFormClosed(this, new WorkspaceEventArgs(this.Controls[0]));
                }

                base.OnClosed(e);
            }

            private WorkspaceCancelEventArgs FireWindowFormClosing(object smartPart)
            {
                WorkspaceCancelEventArgs cancelArgs = new WorkspaceCancelEventArgs(smartPart);

                if (this.WindowFormClosing != null)
                {
                    this.WindowFormClosing(this, cancelArgs);
                }

                return cancelArgs;
            }
        }

        #endregion
    }
}
