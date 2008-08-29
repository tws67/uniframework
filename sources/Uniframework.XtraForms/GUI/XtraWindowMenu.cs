using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows.Forms;

using DevExpress.XtraBars;
using DevExpress.XtraTabbedMdi;

using Uniframework.XtraForms.Properties;

namespace Uniframework.XtraForms
{
    /// <summary>
    /// 窗口列表及布局菜单
    /// </summary>
    public class XtraWindowMenu : BarSubItem
    {
        private Bar bar;
        private readonly XtraTabbedMdiManager mdiManager;
        private readonly Form shell;
        private WindowMenuUtility utility;

        /// <summary>
        /// Initializes a new instance of the <see cref="XtraWindowMenu"/> class.
        /// </summary>
        /// <param name="bar">The bar.</param>
        /// <param name="mdiManager">The MDI manager.</param>
        /// <param name="shell">The shell.</param>
    	public XtraWindowMenu(Bar bar, XtraTabbedMdiManager mdiManager, Form shell)
        {
            this.bar = bar;
            this.mdiManager = mdiManager;
            this.shell = shell;
            Manager = bar.Manager;
            utility = new WindowMenuUtility(mdiManager, shell);

            Caption = "窗口(&W)";
            AddAllMenuItems();
        }

        /// <summary>
        /// Adds all menu items.
        /// </summary>
        private void AddAllMenuItems()
        {
            BarButtonItem bbiTabbed = AddBarItem("选项卡式窗口布局(&T)", null, null, utility.MdiChangeMode, false);
            bbiTabbed.ButtonStyle = BarButtonStyle.Check;
            bbiTabbed.Down = true;

            AddBarItem("层叠(&C)", Resource.windows, null, utility.MdiLayoutCascade, true);
            AddBarItem("水表排列(&H)", Resource.window_split_hor, null, utility.MdiLayoutTileHorizontal, false);
            AddBarItem("垂直排列(&V)", Resource.window_split_ver, null, utility.MdiLayoutTileVertical, false);

            BarSubItem bsiWindows = new BarSubItem(Manager, "所有窗口(&W)");
            BarItemLink link = AddItem(bsiWindows);
            link.BeginGroup = true;
            bsiWindows.AddItem(new BarMdiChildrenListItem());
        }

        /// <summary>
        /// Adds the bar item.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="glyph">The glyph.</param>
        /// <param name="largeGlyph">The large glyph.</param>
        /// <param name="itemClickEventHandler">The item click event handler.</param>
        /// <param name="beginGroup">if set to <c>true</c> [begin group].</param>
        /// <returns></returns>
        private BarButtonItem AddBarItem(string caption, Image glyph, Image largeGlyph,
            ItemClickEventHandler itemClickEventHandler, bool beginGroup)
        {
            BarButtonItem item = new BarButtonItem(Manager, caption);
            item.Glyph = glyph;
            item.LargeGlyph = largeGlyph;
            item.ItemClick += itemClickEventHandler;
            BarItemLink link = AddItem(item);
            link.BeginGroup = beginGroup;
            return item;
        }
    }
}
