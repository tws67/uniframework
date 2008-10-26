using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.CompositeUI.SmartParts;

namespace Microsoft.Practices.CompositeUI.Common
{
    public class WorkspaceComposerAdapter<TSmartPart, TSmartPartInfo> : WorkspaceComposer<TSmartPart, TSmartPartInfo>, IWorkspaceComposer<TSmartPart>
        where TSmartPartInfo : ISmartPartInfo, new()
    {
        public WorkspaceComposerAdapter(IComposableWorkspace<TSmartPart, TSmartPartInfo> composedWorkspace)
            : base(composedWorkspace)
        {
        }
    }
}
