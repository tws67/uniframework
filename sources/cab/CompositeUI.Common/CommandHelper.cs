using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.CompositeUI.Commands;
using System.Windows.Forms;

namespace Microsoft.Practices.CompositeUI.Common
{
    /// <summary>
    /// 命令帮助类
    /// </summary>
    public static class CommandHelper
    {
        /// <summary>
        /// 为命令添加事件处理程序
        /// </summary>
        public static void AddInvoker(WorkItem workItem, string name, params object[] items)
        {
            AddInvoker(workItem, name, "Click", items);
        }

        /// <summary>
        /// 为命令添加事件处理程序
        /// </summary>
        public static void AddInvoker(WorkItem workItem, string name, string eventName, params object[] items)
        {
            bool enabled = (workItem.Commands[name].Status == CommandStatus.Enabled) ? true : false;
            foreach (object item in items)
            {
                Command cmd = GetCommand(workItem, name);
                if (cmd != null)
                    cmd.AddInvoker(item, eventName);

                if (item is Control) { ((Control)item).Enabled = enabled; }
                if (item is ToolStripItem) { ((ToolStripItem)item).Enabled = enabled; }
            }
        }

        /// <summary>
        /// 设置命令项的状态.
        /// </summary>
        /// <param name="workItem">工作项.</param>
        /// <param name="name">命令名称.</param>
        /// <param name="enabled">如果enabled为<c>true</c>设置命令项为<c>CommandStatus.Enabled</c>否则为<c>CommandStatus.Disabled</c>.</param>
        public static void SetStatus(WorkItem workItem, string name, bool enabled)
        {
            CommandStatus status = enabled ? CommandStatus.Enabled : CommandStatus.Disabled;
            SetStatus(workItem, name, status);
        }
        
        /// <summary>
        /// 设置命令项的状态.
        /// </summary>
        /// <param name="workItem">工作项.</param>
        /// <param name="commandName">命令名称.</param>
        /// <param name="status">命令状态.</param>
        public static void SetStatus(WorkItem workItem, string name, CommandStatus status)
        {
            Command cmd = GetCommand(workItem, name);
            if (cmd != null)
                cmd.Status = status;
        }

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
    }
}
