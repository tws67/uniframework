using DevExpress.XtraBars.Localization;

namespace Uniframework.XtraForms
{
    public class ChineseXtraBarsCustomizationLocalizer : BarLocalizer {
       // Overrides the CreateCustomizationControl method.
        protected override DevExpress.XtraBars.Customization.CustomizationControl CreateCustomizationControl() { return new ChineseXtraBarsCustomizatio(); }
       // Overrides the GetLocalizedString method.
        public override string GetLocalizedString(BarString id)
        {
            switch (id)
            {
                case BarString.None:
                    return "";

                case BarString.AddOrRemove:
                    return "添加或删除按钮(&A)";

                case BarString.BarAllItems:
                    return "(所有项)";

                case BarString.BarUnassignedItems:
                    return "(未分配项)";

                case BarString.CustomizeButton:
                    return "自定义(&C)...";

                case BarString.CustomizeWindowCaption:
                    return "自定义";

                case BarString.MenuAnimationFade:
                    return "淡出";

                case BarString.MenuAnimationNone:
                    return "无效果";

                case BarString.MenuAnimationRandom:
                    return "随机";

                case BarString.MenuAnimationSlide:
                    return "滚动";

                case BarString.MenuAnimationSystem:
                    return "(系统默认值)";

                case BarString.MenuAnimationUnfold:
                    return "展开";

                case BarString.NewMenuName:
                    return "主菜单";

                case BarString.NewStatusBarName:
                    return "状态栏";

                case BarString.NewToolbarCaption:
                    return "新建工具栏";

                case BarString.NewToolbarCustomNameFormat:
                    return "自定义{0}";

                case BarString.NewToolbarName:
                    return "工具栏";

                case BarString.PopupMenuEditor:
                    return "弹出菜单编辑器";

                case BarString.RenameToolbarCaption:
                    return "重命名工具栏";

                case BarString.ResetBar:
                    return "是否确认要取消对“{0}” 工具栏所做修改吗？";

                case BarString.ResetBarCaption:
                    return "自定义";

                case BarString.ResetButton:
                    return "重设工具栏(&R)";

                case BarString.RibbonAllPages:
                    return "(所有页面)";

                case BarString.RibbonGalleryFilter:
                    return "所有组";

                case BarString.RibbonGalleryFilterNone:
                    return "无";

                case BarString.RibbonToolbarAbove:
                    return "在功能区上方显示快速访问工具栏(&P)";

                case BarString.RibbonToolbarAdd:
                    return "添加到快速访问工具栏(&A)";

                case BarString.RibbonToolbarBelow:
                    return "在功能区下方显示快速访问工具栏(&P)";

                case BarString.RibbonToolbarMinimizeRibbon:
                    return "功能区最小化(&N)";

                case BarString.RibbonToolbarRemove:
                    return "从快速访问工具栏删除(&R)";

                case BarString.RibbonUnassignedPages:
                    return "(未分配项)";

                case BarString.ToolBarMenu:
                    return "重设(&R)$删除(&D)$!名称(&N)$!默认风格(&L)$始终显示文本(&T)$仅显示文本(&O)$图象与文本(&A)$!开始一组(&G)$可见的(&V)$最近常用的(&M)";

                case BarString.ToolbarNameCaption:
                    return "工具栏名称(&T):";
            }
            return "";
        }
    }
}


