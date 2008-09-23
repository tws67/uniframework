using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using Microsoft.Practices.CompositeUI.SmartParts;

namespace Uniframework.Tests
{
    public class MockWorkspace : IWorkspace
    {
        public event EventHandler<WorkspaceEventArgs> SmartPartActivated;
        public event EventHandler<WorkspaceCancelEventArgs> SmartPartClosing;
        
        private List<object> smartParts = new List<object>();
        private object activeSmartPart = null;
        
        public object ClosedSmartPart = null;
        public object HiddenSmartPart = null;
        public object ShownSmartPart = null;

        public ReadOnlyCollection<object> SmartParts
        {
            get { return new ReadOnlyCollection<object>(smartParts); }
        }

        public void Activate(object smartPart)
        {
            activeSmartPart = smartPart;
        }

        public object ActiveSmartPart
        {
            get { return activeSmartPart; }
        }

        public void ApplySmartPartInfo(object smartPart, ISmartPartInfo smartPartInfo)
        {
        }

        public void Close(object smartPart)
        {
            smartParts.Remove(smartPart);
            ClosedSmartPart = smartPart;
        }

        public void Hide(object smartPart)
        {
            HiddenSmartPart = smartPart;
        }

        public void Show(object smartPart)
        {
            smartParts.Add(smartPart);
            ShownSmartPart = smartPart;
        }

        public void Show(object smartPart, ISmartPartInfo smartPartInfo)
        {
            smartParts.Add(smartPart);
            ShownSmartPart = smartPart;
        }

        public void FireSmartPartActivated(object smartPart)
        {
            if (SmartPartActivated != null)
            {
                SmartPartActivated(this, new WorkspaceEventArgs(smartPart));
            }
        }

        public void FireSmartPartClosing(object smartPart)
        {
            if (SmartPartClosing != null)
            {
                SmartPartClosing(this, new WorkspaceCancelEventArgs(smartPart));
            }
        }
    }
}
