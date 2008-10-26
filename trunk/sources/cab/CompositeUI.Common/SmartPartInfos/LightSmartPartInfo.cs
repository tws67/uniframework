using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using Microsoft.Practices.CompositeUI.SmartParts;

namespace Microsoft.Practices.CompositeUI.Common
{
    /// <summary>
    /// Provides information about a specific smartpart.
    /// </summary>
    /// <remarks>
    /// In comparison to <see cref="SmartPartInfo"/> this class does not extend from <see cref="Component"/> 
    /// and therefore it does not provide designer support.
    /// </remarks>
    public class LightSmartPartInfo : ISmartPartInfo
    {
        private string title;
        private string description;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public LightSmartPartInfo() : this(string.Empty, string.Empty) { }

        public LightSmartPartInfo(string title, string description)
        {
            this.title = title;
            this.description = description;
        }
    }
}
