using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;

using DevExpress.XtraGrid;
using DevExpress.XtraTreeList;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraCharts;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 文档服务适配器工厂
    /// </summary>
    public class XtraDocumentAdapterFactory : IAdapterFactory<IDocumentHandler>
    {
        #region IAdapterFactory<IDocument> Members

        public IDocumentHandler GetAdapter(object element)
        {
            if (element is GridControl)
                return new XtraGridDocumentAdapter(element as GridControl);
            else if (element is TreeList)
                return new TreeListDocumentAdapter(element as TreeList);

            throw new UniframeworkException(String.Format("不支持此种类型 \"{0}\" 控件的文档服务功能。",
                element.GetType().ToString()));
        }

        public bool Supports(object element)
        {
            return element is GridControl || element is TreeList || element is PivotGridControl;
        }

        #endregion
    }
}
