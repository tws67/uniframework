using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;

using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views;

namespace Uniframework.SmartClient
{
    public class XtraGridEditAdapterFactory : IAdapterFactory<IEditHandler>
    {
        #region IAdapterFactory<XtraGridEditAdapter> Members

        public IEditHandler GetAdapter(object element)
        {
            if (element is GridControl)
                return new XtraGridEditAdapter(element as GridControl);

            throw new UniframeworkException(String.Format("不支持此种类型 \"{0}\" 控件的编辑服务功能。",
                element.GetType().ToString()));
        }

        public bool Supports(object element)
        {
            return element is GridControl;
        }

        #endregion
    }
}
