using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Microsoft.Practices.CompositeUI.Common
{
    /// <summary>
    /// Provides information and an icon about a specific smartpart.
    /// </summary>
    public class IconSmartPartInfo : LightSmartPartInfo
    {
        private Icon icon;

        public Icon Icon
        {
            get { return icon; }
            set { icon = value; }
        }

        public IconSmartPartInfo() 
            : base() { }

        public IconSmartPartInfo(string title, string description) 
            : base(title, description) { }
    }
}
