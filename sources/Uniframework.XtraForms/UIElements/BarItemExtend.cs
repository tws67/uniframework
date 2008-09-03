using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.XtraForms.UIElements
{
    /// <summary>
    /// BarItem扩展，用于创建菜单时可以进行分组、或直接插入到某菜单项前面
    /// </summary>
    public class BarItemExtend
    {
        private bool beginGroup = false;
        private string insertBefore = String.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarItemExtend"/> class.
        /// </summary>
        public BarItemExtend()
        { }

        /// <summary>
        /// Gets or sets a value indicating whether [begin group].
        /// </summary>
        /// <value><c>true</c> if [begin group]; otherwise, <c>false</c>.</value>
        public bool BeginGroup
        {
            get { return beginGroup; }
            set { beginGroup = value; }
        }

        /// <summary>
        /// Gets or sets the insert before.
        /// </summary>
        /// <value>The insert before.</value>
        public string InsertBefore
        {
            get { return insertBefore; }
            set { insertBefore = value; }
        }
    }
}
