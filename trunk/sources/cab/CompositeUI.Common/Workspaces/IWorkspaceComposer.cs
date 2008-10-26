using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI;

namespace Microsoft.Practices.CompositeUI.Common
{
    /// <summary>
    /// Represents an WorkspaceComposer.
    /// </summary>
    public interface IWorkspaceComposer<TSmartPart> : IWorkspace
    {
        /// <summary>
        /// Set the <see cref="WorkItem"/> where the object is contained.
        /// </summary>
        WorkItem WorkItem { set; }

        /// <summary>
        /// Sets the active smart part in the workspace.
        /// </summary>
        void SetActiveSmartPart(TSmartPart smartPart);

        /// <summary>
        /// Forcedly closes the smart part, without raising the <see cref="IWorkspace.SmartPartClosing"/> event.
        /// </summary>
        void ForceClose(TSmartPart smartPart);
    }
}
