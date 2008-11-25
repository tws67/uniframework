using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;

namespace Uniframework.Switch
{
    /// <summary>
    /// 交换机控制器
    /// </summary>
    public class SwitchController : WorkItemController
    {
        public override void OnRunStarted()
        {
            base.OnRunStarted();

            IVirtualCTI cti = new DefaultVirtualCTI(WorkItem);
        }
    }
}
