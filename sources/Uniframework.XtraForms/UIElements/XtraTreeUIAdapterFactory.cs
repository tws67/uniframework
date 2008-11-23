using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.UIElements;

namespace Uniframework.XtraForms.UIElements
{
    public class XtraTreeUIAdapterFactory : IUIElementAdapterFactory
    {
        #region IUIElementAdapterFactory Members

        public IUIElementAdapter GetAdapter(object uiElement)
        {
            Guard.ArgumentNotNull(uiElement, "uiElement");

            //if(uiElement is XtraTreeList)
            //    return ((XtraTreeList)uiElement).

            //if (uiElement is XtraTreeListNode)
            //    return ((XtraTreeListNode)uiElement).Nodes;
            throw new NotImplementedException();
        }

        public bool Supports(object uiElement)
        {
            return uiElement is XtraTreeList || uiElement is XtraTreeListNode;
        }

        #endregion
    }
}
