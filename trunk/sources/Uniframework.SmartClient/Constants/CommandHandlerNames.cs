using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 全局命令名称
    /// </summary>
    public static class CommandHandlerNames
    {
        public const string CMD_CLICKME                       = "/Shell/Foundation/ClickMe";
        public const string CMD_SHOW_LAYOUTSETTING            = "/Shell/Foundation/Setting/ShellLayout";
        
        // 文件菜单命令
        public const string CMD_FILE_NEW                      = "/Shell/Foundation/File/New";
        public const string CMD_FILE_OPEN                     = "/Shell/Foundation/File/Open";
        public const string CMD_FILE_SAVE                     = "/Shell/Foundation/File/Save";
        public const string CMD_FILE_SAVEAS                   = "/Shell/Foundation/File/SaveAs";
        public const string CMD_FILE_SAVEALL                  = "/Shell/Foundation/File/SaveAll";
        public const string CMD_FILE_CLOSE                    = "/Shell/Foundation/File/Close";
        public const string CMD_FILE_CLOSEALL                 = "/Shell/Foundation/File/CloseAll";
        public const string CMD_FILE_PRINT                    = "/Shell/Foundation/File/Print";
        public const string CMD_FILE_QUICKPRINT               = "/Shell/Foundation/File/QuickPrint";
        public const string CMD_FILE_PREVIEW                  = "/Shell/Foundation/File/Preview";
        public const string CMD_FILE_PAGESETUP                = "/Shell/Foundation/File/PageSetup";
        public const string CMD_FILE_DESIGN                   = "/Shell/Foundation/File/Design";
        public const string CMD_FILE_EXPORT                   = "/Shell/Foundation/File/Export";
        public const string CMD_FILE_IMPORT                   = "/Shell/Foundation/File/Import";
        public const string CMD_FILE_SENDEMAIL                = "/Shell/Foundation/File/SendEmail";
        public const string CMD_FILE_SENDFAX                  = "/Shell/Foundation/File/SendFax";
        public const string CMD_FILE_ATTRIBUTE                = "/Shell/Foundation/File/Attribute";
        public const string CMD_FILE_EXIT                     = "/Shell/Foundation/File/Exit";

        // 编辑菜单命令
        public const string CMD_EDIT_UNDO                     = "/Shell/Foundation/Edit/Undo";
        public const string CMD_EDIT_REDO                     = "/Shell/Foundation/Edit/Redo";
        public const string CMD_EDIT_CUT                      = "/Shell/Foundation/Edit/Cut";
        public const string CMD_EDIT_COPY                     = "/Shell/Foundation/Edit/Copy";
        public const string CMD_EDIT_PASTE                    = "/Shell/Foundation/Edit/Paste";
        public const string CMD_EDIT_DELETE                   = "/Shell/Foundation/Edit/Delete";
        public const string CMD_EDIT_SELECTALL                = "/Shell/Foundation/Edit/SelectAll";
        public const string CMD_EDIT_SEARCH                   = "/Shell/Foundation/Edit/Search";
        public const string CMD_EDIT_REPLACE                  = "/Shell/Foundation/Edit/Replace";

        // 视图菜单命令
        public const string CMD_VIEW_TASKBAR                  = "/Shell/Foundation/View/ShowTaskbarView";
        public const string CMD_VIEW_SETTING                  = "/Shell/Foundation/View/Setting";
        public const string CMD_VIEW_FULLSCREEN               = "/Shell/Foundation/View/FullScreen";
        public const string CMD_VIEW_HIDEN                    = "/Shell/Foundation/View/Hiden";
        public const string CMD_VIEW_BACK                     = "/Shell/Foundation/View/Back";
        public const string CMD_VIEW_FORWARD                  = "/Shell/Foundation/View/Forward";
        public const string CMD_VIEW_STOP                     = "/Shell/Foundation/View/Stop";
        public const string CMD_VIEW_REFRESH                  = "/Shell/Foundation/View/Refresh";
        public const string CMD_VIEW_HOME                     = "/Shell/Foundation/View/Home";

        // 工具菜单命令
        public const string CMD_TOOL_ADDINTREE                = "/Shell/Foundation/Tool/AddInTree";

        // 窗口菜单命令

        // 帮助菜单命令
        public const string CMD_HELP_GETHELP                  = "/Shell/Foundation/Help/GetHelp";
        public const string CMD_HELP_CONTENT                  = "/Shell/Foundation/Help/Content";
        public const string CMD_HELP_INDEX                    = "/Shell/Foundation/Help/Index";
        public const string CMD_HELP_DYNAMICHELP              = "/Shell/Foundation/Help/ShowDynamicHelp";
        public const string CMD_HELP_DAYTIPS                  = "/Shell/Foundation/Help/Daytips";
        public const string CMD_HELP_REGISTER                 = "/Shell/Foundation/Help/Register";
        public const string CMD_HELP_HOMEPAGE                 = "/Shell/Foundation/Help/HomePage";
        public const string CMD_HELP_CONTACTAUTHOR            = "/Shell/Foundation/Help/ContactAuthor";
        public const string CMD_HELP_SUBMITREPORT             = "/Shell/Foundation/Help/SubmitReport";
        public const string CMD_HELP_CHECKUPGRADE             = "/Shell/Foundation/Help/CheckUpgrade";
        public const string CMD_HELP_ABOUT                    = "/Shell/Foundation/Help/About";
        
        // 数据列表右键菜单命令
        public const string CMD_DATAGRID_INSERT               = "/Shell/Foundation/DataList/Insert";
        public const string CMD_DATAGRID_EDIT                 = "/Shell/Foundation/DataList/Edit";
        public const string CMD_DATAGRID_DELETE               = "/Shell/Foundation/DataList/Delete";
        public const string CMD_DATAGRID_EXPAND               = "/Shell/Foundation/DataList/Expand";
        public const string CMD_DATAGRID_COLLAPSE             = "/Shell/Foundation/DataList/Collapse";
        public const string CMD_DATAGRID_FILTER               = "/Shell/Foundation/DataList/Filter";
        public const string CMD_DATAGRID_REFRESH              = "/Shell/Foundation/DataList/Refresh";
        public const string CMD_DATAGRID_SETDETAILVIEW        = "/Shell/Foundation/DataList/SetDetailView";
        public const string CMD_DATAGRID_SETLAYOUTVIEW        = "/Shell/Foundation/DataList/SetLayoutView";
        public const string CMD_DATAGRID_SELECTLAYOUTVIEW     = "/Shell/Foundation/DataList/SelectLayoutView";
        public const string CMD_DATAGRID_SETTING              = "/Shell/Foundation/DataList/Setting";
        public const string CMD_DATAGRID_SHOWGROUPPANEL       = "/Shell/Foundation/DataList/ShowGroupPanel";
        public const string CMD_DATAGRID_SHOWFOOTER           = "/Shell/Foundation/DataList/ShowFooter";


    }
}
