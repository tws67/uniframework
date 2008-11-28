using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.CompositeUI.Common;

using DevExpress.XtraEditors;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 文本编辑器适配器工厂
    /// </summary>
    public class TextEditAdapterFactory : IAdapterFactory<IEditHandler>
    {
        #region IAdapterFactory<IEditHandler> Members

        public IEditHandler GetAdapter(object element)
        {
            if (element is TextEdit)
                return new TextEditAdapter(element as TextEdit);

            throw new UniframeworkException(String.Format("不支持此种类型 \"{0}\" 的编辑器。",
                element.GetType().ToString()));
        }

        public bool Supports(object element)
        {
            return element is TextEdit;
        }

        #endregion
    }
}
