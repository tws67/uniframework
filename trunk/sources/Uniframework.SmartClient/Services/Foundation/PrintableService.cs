using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintableService"/> class.
        /// </summary>
        public PrintableService()
        {
            Application.Idle +=new EventHandler(Application_Idle);
        }

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
            Guard.ArgumentNotNull(handler, "PrintHandler");

            handler.Enter += new EventHandler(OnEnter);
            handler.Leave += new EventHandler(OnLeave);
        }

        public void Register(object uiElement)
        {
            IPrintHandler handler = FactoryCatalog.GetFactory(uiElement).GetAdapter(uiElement);
            handlers.Add(uiElement, handler);
            Register(handler);
        }

        public void UnRegister(IPrintHandler handler)
        {
            Guard.ArgumentNotNull(handler, "PrintHandler");

            handler.Enter -= OnEnter;
            handler.Leave -= OnLeave;
        }

        public void UnRegister(object uiElement)
        {
            if (handlers.ContainsKey(uiElement)) {
                UnRegister(handlers[uiElement]);
                handlers.Remove(uiElement);
            }
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

        private void Application_Idle(object sender, EventArgs e)
        {
            UpdateCommandStatus();
        }

        private void OnEnter(object sender, EventArgs e)
        {
            Microsoft.Practices.CompositeUI.Utility.Guard.TypeIsAssignableFromType(sender.GetType(), typeof(IPrintHandler), "sender");

            activeHandler = (IPrintHandler)sender;
            UpdateCommandStatus();
        }

        private void OnLeave(object sender, EventArgs e)
        {
            activeHandler = null;
            UpdateCommandStatus();
        }

        /// <summary>
        /// 更新命令处理器的状态
        /// </summary>
        private void UpdateCommandStatus()
        {
            bool enabled = (activeHandler != null);

            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_FILE_PRINT, enabled && activeHandler.CanPrint);
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_FILE_QUICKPRINT, enabled && activeHandler.CanQuickPrint);
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_FILE_PREVIEW, enabled && activeHandler.CanPreview);
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_FILE_PAGESETUP, enabled && activeHandler.CanPageSetup);
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_FILE_DESIGN, enabled && activeHandler.CanDesign);
        }

        #endregion
    }
}
