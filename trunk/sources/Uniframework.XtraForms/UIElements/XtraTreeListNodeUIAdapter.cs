using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.CompositeUI.UIElements;

namespace Uniframework.XtraForms.UIElements
{
    public class XtraTreeListNodeUIAdapter : UIElementAdapter<XtraTreeListNode>
    {
        private readonly XtraTreeListNode parent;

        public XtraTreeListNodeUIAdapter(XtraTreeListNode parent)
        {
            Guard.ArgumentNotNull(parent, "parent");
        }

        protected override XtraTreeListNode Add(XtraTreeListNode uiElement)
        {
            Guard.ArgumentNotNull(uiElement, "uiElement");

            int index = GetInsertIndex(uiElement);
            throw new NotImplementedException();
        }

        private int GetInsertIndex(XtraTreeListNode uiElement)
        {
            for (int i = 0; i < parent.Nodes.Count; i++) { 
                //if(parent.Nodes[i]
                //XtraTreeListNode node = new XtraTreeListNode(
            }
            return parent.Nodes.Count;
        }

        protected override void Remove(XtraTreeListNode uiElement)
        {
            Guard.ArgumentNotNull(uiElement, "uiElement");

            parent.Nodes.Remove(uiElement);
        }

        
    }
}
