using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.ObjectBuilder;

namespace Uniframework.SmartClient.Strategies
{
    /// <summary>
    /// 数据列表连接器
    /// </summary>
    public class DataListStrategy : BuilderStrategy
    {
        /// <summary>
        /// See <see cref="IBuilderStrategy.BuildUp"/> for more information.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="typeToBuild"></param>
        /// <param name="existing"></param>
        /// <param name="idToBuild"></param>
        /// <returns></returns>
        public override object BuildUp(IBuilderContext context, Type typeToBuild, object existing, string idToBuild)
        {
            WorkItem workItem = StrategyUtility.GetWorkItem(context, existing);
            if (workItem != null) {
                IDataListViewService dataService = workItem.Services.Get<IDataListViewService>();
                // 如果视图实现了数据列表服务则在系统中注册它
                if (dataService != null && existing is IDataListHandler) {
                    IDataListHandler handler = existing as IDataListHandler;
                    dataService.Register(handler);
                }
            }

            return base.BuildUp(context, typeToBuild, existing, idToBuild);
        }

        /// <summary>
        /// See <see cref="IBuilderStrategy.TearDown"/> for more information.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public override object TearDown(IBuilderContext context, object item)
        {
            WorkItem workItem = StrategyUtility.GetWorkItem(context, item);
            if (workItem != null) {
                IDataListViewService dataService = workItem.Services.Get<IDataListViewService>();
                if (dataService != null && item is IDataListHandler) {
                    IDataListHandler handler = item as IDataListHandler;
                    dataService.UnRegister(handler);
                }
            }

            return base.TearDown(context, item);
        }
    }
}
