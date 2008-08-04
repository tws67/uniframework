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
                        while (workItem != null)
            {
                foreach (KeyValuePair<string, IWorkspace> wks in workItem.Workspaces)
                {
                    if (wks.Value.SmartParts.Contains(smartPart))
                        return wks.Value;
                }
            }
            return null;
        }

        #endregion
    }
}
