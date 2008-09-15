using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.Common;

namespace Uniframework.SmartClient
{
    public class PrintableService : IPrintableService
    {
        private IPrintHandler activeHandler = null;
        private Dictionary<object, IPrintHandler> handlers = new Dictionary<object, IPrintHandler>();
        private WorkItem workItem;

        #region Dependency services

        [ServiceDependency]
        public WorkItem WorkItem
        {
            get { return workItem; }
            set {
                workItem = value;
                UpdateCommandStatus();
            }
        }


        [ServiceDependency]
        public IAdapterFactoryCatalog<IPrintHandler> FactoryCatalog
        {
            get;
            set;
        }

        #endregion

        #region IPrintableService Members

        public void Register(IPrintHandler handler)
        {
            throw new NotImplementedException();
        }

        public void Register(object uiElement)
        {
            throw new NotImplementedException();
        }

        public void UnRegister(IPrintHandler handler)
        {
            throw new NotImplementedException();
        }

        public void UnRegister(object uiElement)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Command handlers

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_FILE_PRINT)]
        public void Print(object sender, EventArgs e)
        {
            activeHandler.Print();
        }

        /// <summary>
        /// 快速打印
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_FILE_QUICKPRINT)]
        public void QuickPrint(object sender, EventArgs e)
        {
            activeHandler.QuickPrint();
        }

        /// <summary>
        /// 打印预览
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_FILE_PREVIEW)]
        public void Preview(object sender, EventArgs e)
        {
            activeHandler.Preview();
        }

        /// <summary>
        /// 页面设置
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_FILE_PAGESETUP)]
        public void PageSetup(object sender, EventArgs e)
        {
            activeHandler.PageSetup();
        }

        /// <summary>
        /// 设计
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_FILE_DESIGN)]
        public void Design(object sender, EventArgs e)
        {
            activeHandler.Design();
        }

        #endregion

        #region Assistant functions

        /// <summary>
        /// 更新命令处理器的状态
        /// </summary>
        private void UpdateCommandStatus()
        {
            bool enabled = activeHandler != null;

            SetCommandStatus(CommandHandlerNames.CMD_FILE_PRINT, enabled && activeHandler.CanPrint);
            SetCommandStatus(CommandHandlerNames.CMD_FILE_QUICKPRINT, enabled && activeHandler.CanQuickPrint);
            SetCommandStatus(CommandHandlerNames.CMD_FILE_PREVIEW, enabled && activeHandler.CanPreview);
            SetCommandStatus(CommandHandlerNames.CMD_FILE_PAGESETUP, enabled && activeHandler.CanPageSetup);
            SetCommandStatus(CommandHandlerNames.CMD_FILE_DESIGN, enabled && activeHandler.CanDesign);
        }

        /// <summary>
        /// Sets the command status.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        private void SetCommandStatus(string cmd, bool enabled)
        {
            Command command = BuilderUtility.GetCommand(WorkItem, cmd);
            if (command != null) {
                command.Status = enabled ? CommandStatus.Enabled : CommandStatus.Disabled;
            }
        }

        #endregion
    }
}
