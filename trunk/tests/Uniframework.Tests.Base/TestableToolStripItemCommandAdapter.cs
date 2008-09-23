using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.CompositeUI.WinForms;

namespace Uniframework.Tests
{
    public class TestableToolStripItemCommandAdapter : ToolStripItemCommandAdapter
    {
        public static List<TestableToolStripItemCommandAdapter> Instances = new List<TestableToolStripItemCommandAdapter>();

        public Dictionary<object, string> InvokerObjects = new Dictionary<object, string>();

        public TestableToolStripItemCommandAdapter()
        {
            Instances.Add(this);
        }

        public override void AddInvoker(object invoker, string eventName)
        {
            InvokerObjects.Add(invoker, eventName);
        }

        public override void RemoveInvoker(object invoker, string eventName)
        {
            InvokerObjects.Remove(invoker);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Instances.Remove(this);
            }
            base.Dispose(disposing);
        }
    }
}
