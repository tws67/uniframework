using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.CompositeUI.UIElements;

namespace Uniframework.Tests
{
    public class MockUIElementAdapterFactory : IUIElementAdapterFactory
    {
        public virtual IUIElementAdapter GetAdapter(object uiElement)
        {
            return new MockUIElementAdapter();
        }

        public virtual bool Supports(object uiElement)
        {
            return true;
        }
    }
}
