using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.ObjectBuilder;
using DevExpress.XtraGrid;
using DevExpress.XtraTreeList;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraCharts;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// DevExpress打印适配器工厂
    /// </summary>
    public class XtraPrintAdapterFactory : IAdapterFactory<IPrintHandler>
    {
        private WorkItem workItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="XtraPrintAdapterFactory"/> class.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        public XtraPrintAdapterFactory(WorkItem workItem)
        {
            this.workItem = workItem;
        }

        #region IAdapterFactory<IPrintHandler> Members

        public IPrintHandler GetAdapter(object element)
        {
            if (element is GridControl)
                return new XtraGridPrintAdapter(element as GridControl, workItem);
            else if (element is TreeList)
                return new XtraTreeListPrintAdapter(element as TreeList, workItem);

            throw new UniframeworkException(String.Format("不支持此种类型 \"{0}\" 控件的打印服务功能。",
                element.GetType().ToString()));

        }

        public bool Supports(object element)
        {
            return element is GridControl || element is TreeList || element is PivotGridControl || element is ChartControl;
        }

        #endregion
    }
}
