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

#endregion

namespace Uniframework.SharePoint.Linq.Spml
{
    /// <summary>
    /// Interface for wizard steps.
    /// </summary>
    interface IWizardStep
    {
        /// <summary>
        /// Title of the wizard step.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Indicates whether the step allows to go to the next step currently.
        /// </summary>
        bool CanNext { get; }

        /// <summary>
        /// Event raised when the state of the step has changed, e.g. when CanNext has changed.
        /// </summary>
        event EventHandler StateChanged;

        /// <summary>
        /// Event raised when the step is performing work.
        /// </summary>
        event EventHandler Working;

        /// <summary>
        /// Event raised when the step has completed its work.
        /// </summary>
        event EventHandler WorkCompleted;

        /// <summary>
        /// Instructs the step to try to cancel a pending operation.
        /// </summary>
        void Cancel();
    }
}
