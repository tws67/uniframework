namespace Uniframework.XtraForms
{
    using DevExpress.XtraBars.Customization;
    using System;
    using System.ComponentModel;

    public class ChineseXtraBarsCustomizatio : CustomizationControl
    {
        private Container components;

        public ChineseXtraBarsCustomizatio()
        {
            this.InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            base.tpToolbars.SuspendLayout();
            base.tpCommands.SuspendLayout();
            base.tpOptions.SuspendLayout();
            ((ISupportInitialize) base.toolBarsList).BeginInit();
            ((ISupportInitialize) base.lbCommands).BeginInit();
            ((ISupportInitialize) base.lbCategories).BeginInit();
            base.cbOptionsShowFullMenus.Properties.BeginInit();
            base.cbOptions_showFullMenusAfterDelay.Properties.BeginInit();
            base.cbOptions_showTips.Properties.BeginInit();
            base.cbOptions_ShowShortcutInTips.Properties.BeginInit();
            base.tabControl.BeginInit();
            base.tabControl.SuspendLayout();
            base.tbNBDlgName.Properties.BeginInit();
            base.pnlNBDlg.SuspendLayout();
            base.cbOptions_largeIcons.Properties.BeginInit();
            base.cbOptions_MenuAnimation.Properties.BeginInit();
            base.SuspendLayout();
            base.btClose.Text = "关闭";
            base.tpToolbars.Text = "工具栏 (&B)";
            base.tpCommands.Text = "命令 (&C)";
            base.tpOptions.Text = "选项 (&O)";
            base.lbToolbarCaption.Text = "工具栏:";
            base.btNewBar.Text = "新建 (&N)...";
            base.lbNBDlgCaption.Text = "工具栏名称:";
            base.btNBDlgOk.Text = "确定";
            base.btNBDlgCancel.Text = "取消";
            base.btRenameBar.Text = "重命名 (&E)...";
            base.btDeleteBar.Text = "删除 (&D)";
            base.btResetBar.Text = "重新设置 (&R)...";
            base.lbCategoriesCaption.Text = "类别 (&G):";
            base.lbCommandsCaption.Text = "命令 (&D):";
            base.lbDescCaption.Text = "说明";
            base.lbOptions_PCaption.Text = "个性化菜单和工具栏";
            base.cbOptionsShowFullMenus.Properties.Caption = "始终显示整个菜单";
            base.cbOptions_showFullMenusAfterDelay.Properties.Caption = "鼠标指针短暂停留后显示整个菜单";
            base.btOptions_Reset.Text = "重设惯用数据 (&R)";
            base.lbOptions_Other.Text = "其它";
            base.cbOptions_largeIcons.Properties.Caption = "大图标 (&L)";
            base.cbOptions_showTips.Properties.Caption = "显示关于工具栏的屏幕提示 (&T)";
            base.cbOptions_ShowShortcutInTips.Properties.Caption = "在屏幕提示中显示快捷键 (&H)";
            base.lbOptions_MenuAnimation.Text = "菜单打开方式 (&M):";
            base.Name = "ChineseXtraBarsCustomizatio_CHS";
            base.tpToolbars.ResumeLayout(false);
            base.tpCommands.ResumeLayout(false);
            base.tpOptions.ResumeLayout(false);
            ((ISupportInitialize) base.toolBarsList).EndInit();
            ((ISupportInitialize) base.lbCommands).EndInit();
            ((ISupportInitialize) base.lbCategories).EndInit();
            base.cbOptionsShowFullMenus.Properties.EndInit();
            base.cbOptions_showFullMenusAfterDelay.Properties.EndInit();
            base.cbOptions_showTips.Properties.EndInit();
            base.cbOptions_ShowShortcutInTips.Properties.EndInit();
            base.tabControl.EndInit();
            base.tabControl.ResumeLayout(false);
            base.tbNBDlgName.Properties.EndInit();
            base.pnlNBDlg.ResumeLayout(false);
            base.cbOptions_largeIcons.Properties.EndInit();
            base.cbOptions_MenuAnimation.Properties.EndInit();
            base.ResumeLayout(false);
        }
    }
}

