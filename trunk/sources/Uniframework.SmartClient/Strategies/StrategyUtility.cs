using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.ObjectBuilder;

namespace Uniframework.SmartClient.Strategies
{
    /// <summary>
    /// 策略工具类
    /// </summary>
    public static class StrategyUtility
    {
        /// <summary>
        /// Gets the work item.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static WorkItem GetWorkItem(IBuilderContext context, object item)
        {
            if (item is WorkItem)
                return item as WorkItem;

            return context.Locator.Get<WorkItem>(new DependencyResolutionLocatorKey(typeof(WorkItem), null));
        }
    }
}
