using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;

namespace Uniframework.SmartClient.AddIns.Builders
{
    public class BuilderUtility
    {
        private static readonly String ResourcePath = @"\Resources\";
        private static readonly String SHELL_FORM = "ShellForm";

        /// <summary>
        /// 获取指定名称的Command组件
        /// </summary>
        /// <param name="workItem">工作项</param>
        /// <param name="name">命令名称</param>
        /// <returns>返回指定名称的命令组件，否则为空</returns>
        public static Command GetCommand(WorkItem workItem, string name)
        {
            Command result = workItem.Commands.Get<Command>(name);
            if (result != null)
                return result;
            List<WorkItem> WorkItems = (List<WorkItem>)workItem.WorkItems.FindByType<WorkItem>();
            foreach (WorkItem item in WorkItems)
            {
                result = GetCommand(item, name);
                if (result != null)
                    break;
            }
            return result;
        }

        /// <summary>
        /// 返回系统资源存放路径
        /// </summary>
        /// <returns></returns>
        public static String GetResourcePath()
        {
            return FileUtility.GetParent(FileUtility.ApplicationRootPath) + ResourcePath;
        }

        /// <summary>
        /// 合成路径
        /// </summary>
        /// <param name="parentPath">上级路径</param>
        /// <param name="id">当前标识</param>
        /// <returns>路径</returns>
        public static String CombinPath(string parentPath, string id)
        {
            if (parentPath.EndsWith("/"))
                return parentPath + id;
            else
                return parentPath + "/" + id;
        }


    }
}
