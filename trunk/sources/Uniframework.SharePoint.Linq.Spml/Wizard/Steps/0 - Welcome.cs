/*
 * LINQ to SharePoint
 * http://www.codeplex.com/LINQtoSharePoint
 * 
 * Copyright Bart De Smet (C) 2007
 * info@bartdesmet.net - http://blogs.bartdesmet.net/bart
 * 
 * This project is subject to licensing restrictions. Visit http://www.codeplex.com/LINQtoSharePoint/Project/License.aspx for more information.
 */

#region Namespace imports

using System;
using System.Windows.Forms;

#endregion

namespace Uniframework.SharePoint.Linq.Spml
{
    /// <summary>
    /// Welcome step of the entity generator wizard.
    /// </summary>
    partial class Welcome : UserControl, IWizardStep
    {
        #region Private members

        /// <summary>
        /// Wizard context.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private WizardContext ctx;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the welcome step.
        /// </summary>
        /// <param name="context">Wizard context.</param>
        public Welcome(WizardContext context)
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
            get { return "Welcome to the LINQ to SharePoint Entity Wizard"; }
        }

        /// <summary>
        /// Indicates whether the step allows to go to the next step currently.
        /// </summary>
        /// <remarks>Always returns true for this step.</remarks>
        public bool CanNext
        {
            get { return true; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event raised when the state of the step has changed, e.g. when CanNext has changed.
        /// </summary>
        /// <remarks>Never raised by this step.</remarks>
        public event EventHandler StateChanged;

        /// <summary>
        /// Event raised when the step is performing work.
        /// </summary>
        /// <remarks>Never raised by this step.</remarks>
        public event EventHandler Working;

        /// <summary>
        /// Event raised when the step has completed its work.
        /// </summary>
        /// <remarks>Never raised by this step.</remarks>
        public event EventHandler WorkCompleted;

        /// <summary>
        /// Raises the StateChanged event.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public void OnStateChanged()
        {
            if (StateChanged != null)
                StateChanged(this, new EventArgs());
        }

        /// <summary>
        /// Raises the Working event.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public void OnWorking()
        {
            if (Working != null)
                Working(this, new EventArgs());
        }

        /// <summary>
        /// Raises the WorkCompleted event.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public void OnWorkCompleted()
        {
            if (WorkCompleted != null)
                WorkCompleted(this, new EventArgs());
        }

        #endregion

        #region Methods

        /// <summary>
        /// Instructs the step to try to cancel a pending operation.
        /// </summary>
        /// <remarks>This step doesn't perform background work.</remarks>
        public void Cancel()
        {
        }

        #endregion

        #region Event handlers

        private void Welcome_Load(object sender, EventArgs e)
        {
            
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            new WizardOptions().ShowDialog();
        }

        #endregion
    }
}
