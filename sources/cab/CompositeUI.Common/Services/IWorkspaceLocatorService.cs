using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI.SmartParts;

namespace Microsoft.Practices.CompositeUI.Common.Services
{
    public interface IWorkspaceLocatorService
    {
        IWorkspace FindContainingWorkspace(WorkItem workItem, object smartPart);
    }
}
