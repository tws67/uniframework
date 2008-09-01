using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 窗口位置属性
    /// </summary>
    public class FormLayout
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormLocation"/> class.
        /// </summary>
        public FormLayout()
        { }

        public Point Location
        {
            get;
            set;
        }

        public Size Size
        {
            get;
            set;
        }

        public FormWindowState WindowState
        {
            get;
            set;
        }
    }
}
