using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.XtraBars;

namespace Uniframework.XtraForms
{
    /// <summary>
    /// 工具栏列表菜单项
    /// </summary>
    public class XtraBarListMenu : BarSubItem
    {
        private readonly Bar bar;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolbarsListMenu"/> class.
        /// </summary>
        /// <param name="bar">The bar.</param>
        public XtraBarListMenu(Bar bar)
        {
            this.bar = bar;
            Manager = bar.Manager;
            AddSubItem();
        }

        /// <summary>
        /// Adds the sub item.
        /// </summary>
        private void AddSubItem()
        {
            BarToolbarsListItem item = new BarToolbarsListItem();
            item.ShowDockPanels = true;
            AddItem(item);
        }
    }
}
