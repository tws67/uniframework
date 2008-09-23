using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.CompositeUI.UIElements;

namespace Uniframework.Tests
{
    public class MockUIElementAdapter : IUIElementAdapter
    {
        private List<object> uiElements = new List<object>();

        public IList<object> UIElements
        {
            get { return uiElements; }
        }

        public virtual object Add(object uiElement)
        {
            uiElements.Add(uiElement);
            return uiElement;
        }

        public virtual void Remove(object uiElement)
        {
            uiElements.Remove(uiElement);
        }
    }
}
