using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.XtraBars;

namespace Uniframework.XtraForms
{
    public class XtraSkinMenu : BarSubItem
    {
        private readonly Bar bar;

        /// <summary>
        /// Initializes a new instance of the <see cref="XtraSkinMenu"/> class.
        /// </summary>
        /// <param name="bar">The bar.</param>
        public XtraSkinMenu(Bar bar)
        {
            this.bar = bar;
            Manager = bar.Manager;
            AddAllMenuItems();
        }

        /// <summary>
        /// Adds all skin menu items.
        /// </summary>
        private void AddAllMenuItems()
        {
            Caption = "系统界面皮肤(&K)";

            foreach (string skinName in GetSortedSkinNames())
            {
                BarItem menuItem = new BarCheckItem(bar.Manager, SkinManager.DefaultSkinName == skinName ? true : false) { Caption = skinName };
                menuItem.ItemClick += OnSwitchSkin;
                AddItem(menuItem);
            }
        }

        /// <summary>
        /// Gets the sorted skin names.
        /// </summary>
        /// <returns>the list of current skins.</returns>
        private static List<string> GetSortedSkinNames()
        {
            var skinNames = new List<string>(SkinManager.Default.Skins.Count);

            foreach (SkinContainer skinContainer in SkinManager.Default.Skins)
                skinNames.Add(skinContainer.SkinName);

            skinNames.Sort();
            return skinNames;
        }

        /// <summary>
        /// Called when [switch skin].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DevExpress.XtraBars.ItemClickEventArgs"/> instance containing the event data.</param>
        private static void OnSwitchSkin(object sender, ItemClickEventArgs e)
        {
            var item = e.Item as BarCheckItem;
            if (item == null) return;
            UserLookAndFeel.Default.SetSkinStyle(item.Caption);
        }

        /// <summary>
        /// Called when [popup].
        /// </summary>
        protected override void OnPopup()
        {
            base.OnPopup();
            foreach (BarItemLink item in ItemLinks)
            {
                if (item.Item is BarCheckItem)
                    ((BarCheckItem)item.Item).Checked = UserLookAndFeel.Default.ActiveSkinName == item.Caption;
            }
        }
    }
}
