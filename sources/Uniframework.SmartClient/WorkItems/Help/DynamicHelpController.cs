using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.SmartParts;
using Uniframework.XtraForms.SmartPartInfos;

namespace Uniframework.SmartClient
{
    public class DynamicHelpController : WorkItemController
    {
        protected override void AddServices()
        {
            base.AddServices();

            WorkItem.RootWorkItem.Services.AddNew<DynamicHelpService, IDynamicHelpService>();
        }
    }
}
