using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.SmartParts;

namespace Microsoft.Practices.CompositeUI.Common
{
    /// <summary>
    /// Implements a Workspace that shows smartparts in forms. In comparison to the WindowWorkspace, this
    /// docks the smartpart in "fill" mode and provides additional settings.
    /// </summary>
    public class FormWorkspace : IComposableWorkspace<Control, FormSmartPartInfo>
    {
        private readonly Dictionary<Control, Form> forms = new Dictionary<Control, Form>();
        private readonly IWorkspaceComposer<Control> composer;
    	readonly IWin32Window owner;

        /// <summary>
        /// Dependency injection setter property to get the WorkItem where the object is contained.
        /// </summary>
        [ServiceDependency]
        public WorkItem WorkItem
        {
            set { composer.WorkItem = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormWorkspace"/> class
        /// </summary>
        public FormWorkspace() : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormWorkspace"/> class
        /// </summary>
        public FormWorkspace(IWin32Window owner)
        {
            this.owner = owner;
            composer = CreateWorkspaceComposer();
        }

        /// <summary>
        /// Create a WorkspaceComposer. GoF Design Pattern: Factory Method.
        /// </summary>
        protected virtual IWorkspaceComposer<Control> CreateWorkspaceComposer()
        {
            return new WorkspaceComposerAdapter<Control, FormSmartPartInfo>(this);
        }

        #region private

        private void FormActivated(object sender, EventArgs e)
        {
            Control smartPart = GetSmartPart((Form)sender);
            
            if (smartPart != null) { Activate(smartPart); }
            else { composer.SetActiveSmartPart(null); }
        }

        private void FormClosing(object sender, FormClosingEventArgs e)
        {
            Control smartPart = GetSmartPart((Form)sender);
            if (smartPart != null)
            {
                WorkspaceCancelEventArgs wce = new WorkspaceCancelEventArgs(smartPart);
                OnSmartPartClosing(wce);
                e.Cancel = wce.Cancel;
            }
        }

        private void ControlDisposed(object sender, EventArgs e)
        {
            Control control = sender as Control;
            if (control != null)
            {
                composer.ForceClose(control);
            }
        }

        private Control GetSmartPart(Form form)
        {
            // Locate the smart part hosted in the form.
            foreach (KeyValuePair<Control, Form> pair in forms)
            {
                if (pair.Value == form) { return pair.Key; }
            }

            // not found
            return null;
        }

        #endregion

        #region Protected virtual implementation

        /// <summary>
        /// Create a Form object that hosts the smart part.
        /// </summary>
        protected virtual Form GetOrCreateForm(Control smartPart)
        {
            Form form = new Form();
            form.Controls.Add(smartPart);
            form.ClientSize = smartPart.PreferredSize;
            form.Text = smartPart.Text;
            smartPart.Dock = DockStyle.Fill;
            return form;
        }

        protected virtual void OnActivate(Control smartPart)
        {
            Form form = forms[smartPart];
            form.BringToFront();
        }

        protected virtual void OnApplySmartPartInfo(Control smartPart, FormSmartPartInfo smartPartInfo)
        {
            Form form = forms[smartPart];
            if (smartPartInfo.Title != null) { form.Text = smartPartInfo.Title; }
            if (smartPartInfo.Icon != null) { form.Icon = smartPartInfo.Icon; }

            if (smartPartInfo.Location != null) { form.Location = smartPartInfo.Location; }
            if (smartPartInfo.Height != 0) { form.Height = smartPartInfo.Height; }
            if (smartPartInfo.Width != 0) { form.Width = smartPartInfo.Width; }
            
            form.ControlBox = smartPartInfo.ControlBox;
            form.MaximizeBox = smartPartInfo.MaximizeBox;
            form.MinimizeBox = smartPartInfo.MinimizeBox;
            form.ShowIcon = smartPartInfo.ShowIcon;
            form.ShowInTaskbar = smartPartInfo.ShowInTaskBar;
        	form.StartPosition = smartPartInfo.StartPosition;
        	form.FormBorderStyle = smartPartInfo.FormBorderStyle;

            if (smartPartInfo.AcceptButton != null) { form.AcceptButton = smartPartInfo.AcceptButton; }
            if (smartPartInfo.CancelButton != null) { form.CancelButton = smartPartInfo.CancelButton; }
        }

        protected virtual void OnShow(Control smartPart, FormSmartPartInfo smartPartInfo)
        {
            Form form;
            
            // Get or create the form that host the smart part.
            if (!forms.ContainsKey(smartPart))
            {
                form = GetOrCreateForm(smartPart);
                forms.Add(smartPart, form);

                form.Activated += new EventHandler(FormActivated);
                form.FormClosing += new FormClosingEventHandler(FormClosing);
                smartPart.Disposed += new EventHandler(ControlDisposed);
            }
            else
            {
                form = forms[smartPart];
            }

            // Apply smart part info
            if (smartPartInfo != null)
            {
                OnApplySmartPartInfo(smartPart, smartPartInfo);
            }

            // Show the form with the smart part.
            if (smartPartInfo.ShowModal)
            {
                if (owner != null) { form.ShowDialog(owner); }
                else { form.ShowDialog(); }
            }
            else
            {
                if (owner != null) { form.Show(owner); }
                else { form.Show(); }
            }
        }

        protected virtual void OnHide(Control smartPart)
        {
            Form form = forms[smartPart];
            form.Hide();
        }

        protected virtual void OnClose(Control smartPart)
        {
            Form form = forms[smartPart];

            smartPart.Disposed -= ControlDisposed;
            form.FormClosing -= FormClosing;
            form.Activated -= FormActivated;

            form.Close();
            forms.Remove(smartPart);
        }

        protected virtual void OnSmartPartActivated(WorkspaceEventArgs e)
        {
            if (SmartPartActivated != null)
            {
                SmartPartActivated(this, e);
            }
        }

        protected virtual void OnSmartPartClosing(WorkspaceCancelEventArgs e)
        {
            if (SmartPartClosing != null)
            {
                SmartPartClosing(this, e);
            }
        }

        protected virtual FormSmartPartInfo OnConvertFrom(ISmartPartInfo source)
        {
            return FormSmartPartInfo.ConvertTo(source);
        }

        #endregion

        #region IComposableWorkspace<Control,FormSmartPartInfo> Members

        void IComposableWorkspace<Control, FormSmartPartInfo>.OnActivate(Control smartPart)
        {
            OnActivate(smartPart);
        }

        void IComposableWorkspace<Control, FormSmartPartInfo>.OnApplySmartPartInfo(Control smartPart, FormSmartPartInfo smartPartInfo)
        {
            OnApplySmartPartInfo(smartPart, smartPartInfo);
        }

        void IComposableWorkspace<Control, FormSmartPartInfo>.OnShow(Control smartPart, FormSmartPartInfo smartPartInfo)
        {
            OnShow(smartPart, smartPartInfo);
        }

        void IComposableWorkspace<Control, FormSmartPartInfo>.OnHide(Control smartPart)
        {
            OnHide(smartPart);
        }

        void IComposableWorkspace<Control, FormSmartPartInfo>.OnClose(Control smartPart)
        {
            OnClose(smartPart);
        }

        void IComposableWorkspace<Control, FormSmartPartInfo>.RaiseSmartPartActivated(WorkspaceEventArgs e)
        {
            OnSmartPartActivated(e);
        }

        void IComposableWorkspace<Control, FormSmartPartInfo>.RaiseSmartPartClosing(WorkspaceCancelEventArgs e)
        {
            OnSmartPartClosing(e);
        }

        FormSmartPartInfo IComposableWorkspace<Control, FormSmartPartInfo>.ConvertFrom(ISmartPartInfo source)
        {
            return OnConvertFrom(source);
        }

        #endregion

        #region IWorkspace Members

        public event EventHandler<WorkspaceCancelEventArgs> SmartPartClosing;

        public event EventHandler<WorkspaceEventArgs> SmartPartActivated;

        public ReadOnlyCollection<object> SmartParts
        {
            get { return composer.SmartParts; }
        }

        public object ActiveSmartPart
        {
            get { return composer.ActiveSmartPart; }
        }

    	public void Activate(object smartPart)
        {
            composer.Activate(smartPart);
        }

        public void ApplySmartPartInfo(object smartPart, ISmartPartInfo smartPartInfo)
        {
            composer.ApplySmartPartInfo(smartPart, smartPartInfo);
        }

        public void Close(object smartPart)
        {
            composer.Close(smartPart);
        }

        public void Hide(object smartPart)
        {
            composer.Hide(smartPart);
        }

        public void Show(object smartPart, ISmartPartInfo smartPartInfo)
        {
            composer.Show(smartPart, smartPartInfo);
        }

        public void Show(object smartPart)
        {
            composer.Show(smartPart);
        }

        #endregion
    }
}
