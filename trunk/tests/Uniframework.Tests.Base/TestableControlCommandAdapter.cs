using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.WinForms;

namespace Uniframework.Tests
{
    public class TestableControlCommandAdapter : ControlCommandAdapter
    {
        public static List<TestableControlCommandAdapter> Instances = new List<TestableControlCommandAdapter>();

        public Dictionary<object, string> InvokerObjects = new Dictionary<object, string>();

        public TestableControlCommandAdapter()
        {
            Instances.Add(this);
        }

        ~TestableControlCommandAdapter()
        {
            Instances.Remove(this);
        }

        public override void AddInvoker(object invoker, string eventName)
        {
            InvokerObjects.Add(invoker, eventName);
        }

        public override void RemoveInvoker(object invoker, string eventName)
        {
            InvokerObjects.Remove(invoker);
        }
    }
}
