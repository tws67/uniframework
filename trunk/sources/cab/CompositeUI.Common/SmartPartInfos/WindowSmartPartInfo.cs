using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.Practices.CompositeUI.Common.SmartPartInfo
{
    public class WindowSmartPartInfo : Microsoft.Practices.CompositeUI.WinForms.WindowSmartPartInfo
    {
        public bool ShowInTaskbar { get; set; }
        public FormWindowState WindowState { get; set; }
        public FormStartPosition StartPosition { get; set; }
        public IButtonControl AcceptButton { get; set; }
        public IButtonControl CancelButton { get; set; }

        public WindowSmartPartInfo()
            : base() 
        {
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            WindowState = FormWindowState.Normal;
        }

        public WindowSmartPartInfo(string title, string description)
            : this()
        {
            Title = title;
            Description = description;
        }
    }
}
