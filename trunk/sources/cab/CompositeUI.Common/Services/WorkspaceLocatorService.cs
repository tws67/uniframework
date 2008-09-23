using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI.SmartParts;

namespace Microsoft.Practices.CompositeUI.Common.Services
{
    public class WorkspaceLocatorService : IWorkspaceLocatorService
    {
        #region IWorkspaceLocatorService 成员

        public IWorkspace FindContainingWorkspace(WorkItem workItem, object smartPart)
        {
            while (workItem != null) {
                foreach (KeyValuePair<string, IWorkspace> namedWorkspace in workItem.Workspaces) {
                    if (namedWorkspace.Value.SmartParts.Contains(smartPart))
                        return namedWorkspace.Value;
                }
                workItem = workItem.Parent;
            }
            return null;
        }

        #endregion
    }
}
