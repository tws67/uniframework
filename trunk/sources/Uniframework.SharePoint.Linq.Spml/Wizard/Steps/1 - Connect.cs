/*
 * LINQ to SharePoint
 * http://www.codeplex.com/LINQtoSharePoint
 * 
 * Copyright Bart De Smet (C) 2007
 * info@bartdesmet.net - http://blogs.bartdesmet.net/bart
 * 
 * This project is subject to licensing restrictions. Visit http://www.codeplex.com/LINQtoSharePoint/Project/License.aspx for more information.
 */

/*
 * Version history:
 * 
 * 0.2.2 - Introduction of entity wizard
 */

#region Namespace imports

using System;
using System.ComponentModel;
using System.Windows.Forms;
using Uniframework.SharePoint.Linq.EntityGenerator;

#endregion

namespace Uniframework.SharePoint.Linq.Spml
{
    /// <summary>
    /// Connect step of the entity generator wizard.
    /// </summary>
    partial class Connect : UserControl, IWizardStep
    {
        #region Private members

        /// <summary>
        /// Wizard context.
        /// </summary>
        private WizardContext ctx;

        /// <summary>
        /// Indicates whether the wizard can go to the next step. Will be true once the connection test has passed.
        /// </summary>
        private bool _next;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the connect step.
        /// </summary>
        /// <param name="context">Wizard context.</param>
        public Connect(WizardContext context)
        {
            this.ctx = context;

            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Title of the wizard step.
        /// </summary>
        public string Title
        {
            get { return "Connect to the SharePoint site"; }
        }

        /// <summary>
        /// Indicates whether the step allows to go to the next step currently. Will be true once the connection test has passed.
        /// </summary>
        public bool CanNext
        {
            get { return _next; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event raised when the state of the step has changed, e.g. when CanNext has changed.
        /// </summary>
        public event EventHandler StateChanged;

        /// <summary>
        /// Event raised when the step is performing work.
        /// </summary>
        public event EventHandler Working;

        /// <summary>
        /// Event raised when the step has completed its work.
        /// </summary>
        public event EventHandler WorkCompleted;

        /// <summary>
        /// Raises the StateChanged event.
        /// </summary>
        public void OnStateChanged()
        {
            if (StateChanged != null)
                StateChanged(this, new EventArgs());
        }

        /// <summary>
        /// Raises the Working event.
        /// </summary>
        public void OnWorking()
        {
            if (Working != null)
                Working(this, new EventArgs());
        }

        /// <summary>
        /// Raises the WorkCompleted event.
        /// </summary>
        public void OnWorkCompleted()
        {
            if (WorkCompleted != null)
                WorkCompleted(this, new EventArgs());
        }

        #endregion

        #region Event handlers

        private void Connect_Load(object sender, EventArgs e)
        {
            //
            // Pre-populate fields if information present.
            //
            txtUser.Text = ctx.ResultContext.Connection.User ?? "";
            txtDomain.Text = ctx.ResultContext.Connection.Domain ?? "";

            //
            // Start input on txtUrl TextBox.
            //
            txtUrl.Focus();
            txtUrl.Select(txtUrl.Text.Length, 0);

            //
            // Test button becomes default button till connection test succeeds.
            //
            Form f = this.ParentForm;
            f.AcceptButton.NotifyDefault(false);
            f.AcceptButton = btnTest;
            f.AcceptButton.NotifyDefault(true);
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            //
            // Check for valid input.
            //
            if (this.ValidateChildren(ValidationConstraints.Enabled | ValidationConstraints.Selectable))
            {
                //
                // Clear error message and signal working.
                //
                lblError.Visible = false;
                OnWorking();

                //
                // Get data.
                //
                Uri url = new Uri(txtUrl.Text);
                string user = txtUser.Text;
                string password = txtPassword.Text;
                string domain = txtDomain.Text;

                //
                // Create connection object and connect in background.
                //
                Connection conn = new Connection() { Url = url, User = user, Password = password, Domain = domain, CustomAuthentication = radCustom.Checked };
                bgConnect.RunWorkerAsync(conn);
            }
        }

        private void bgConnect_DoWork(object sender, DoWorkEventArgs e)
        {
            //
            // Get the connection, store it and try to retrieve lists. These results are already useful for the next step.
            //
            Connection conn = (Connection)e.Argument;
            ctx.ResultContext.Connection
                = ctx.FullContext.Connection
                = conn;
            ctx.ResultContext.Url 
                = ctx.FullContext.Url
                = conn.Url;
            ctx.FullContext.Lists.Clear();
            foreach (List l in Helpers.GetLists(conn))
                ctx.FullContext.Lists.Add(l);
        }

        private void bgConnect_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                //
                // If no error has occurred, we can proceed to the next step; otherwise, an error will be displayed.
                //
                if (e.Error == null)
                {
                    _next = true;
                    OnStateChanged();
                }
                else
                    lblError.Visible = true;
            }

            //
            // Signal background work completed.
            //
            OnWorkCompleted();
        }

        #region Validation

        private void radCustom_CheckedChanged(object sender, EventArgs e)
        {
            panelCustom.Enabled = radCustom.Checked;

            if (!panelCustom.Enabled)
            {
                errors.SetError(txtUser, "");
                errors.SetError(txtPassword, "");
                errors.SetError(txtDomain, "");
            }

            Reset();
        }

        private void txtUrl_Validating(object sender, CancelEventArgs e)
        {
            if (!Uri.IsWellFormedUriString(txtUrl.Text, UriKind.Absolute))
            {
                e.Cancel = true;
                errors.SetError(txtUrl, "Invalid URL");
            }
            else
                errors.SetError(txtUrl, "");
        }

        private void txtUser_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUser.Text))
            {
                e.Cancel = true;
                errors.SetError(txtUser, "No user name specified");
            }
            else
                errors.SetError(txtUser, "");
        }

        private void txtUrl_TextChanged(object sender, EventArgs e)
        {
            Reset();
        }

        private void txtUser_TextChanged(object sender, EventArgs e)
        {
            Reset();
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            Reset();
        }

        private void txtDomain_TextChanged(object sender, EventArgs e)
        {
            Reset();
        }

        #endregion

        #region Auto-select textbox contents

        private void txtPassword_Enter(object sender, EventArgs e)
        {
            txtPassword.SelectAll();
        }

        private void txtUser_Enter(object sender, EventArgs e)
        {
            txtUser.SelectAll();
        }

        private void txtDomain_Enter(object sender, EventArgs e)
        {
            txtDomain.SelectAll();
        }

        private void txtUrl_Enter(object sender, EventArgs e)
        {
            txtUrl.SelectAll();
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Instructs the step to try to cancel a pending operation. This will try to stop a pending background connection operation, if any.
        /// </summary>
        public void Cancel()
        {
            if (bgConnect.IsBusy && !bgConnect.CancellationPending)
                bgConnect.CancelAsync();
        }

        #endregion

        #region Helpers methods

        /// <summary>
        /// Resets the state of the wizard when connection information has changed; this will cause the wizard to set CanNext to false and signal a state change.
        /// </summary>
        private void Reset()
        {
            _next = false;
            OnStateChanged();
        }

        #endregion
    }
}
