using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;

namespace Uniframework.SmartClient
{
    public class EditableService : IEditableService
    {
        private IEditHandler activeHandler;
        private Dictionary<object, IEditHandler> handlers = new Dictionary<object,IEditHandler>();

        private WorkItem workItem;
        private IAdapterFactoryCatalog<IEditHandler> factoryCatalog;

        #region Dependency services

        [ServiceDependency]
        public WorkItem WorkItem
        {
            set 
            { 
                workItem = value;
                UpdateCommandStatus();
            }
        }

        [ServiceDependency]
        public IAdapterFactoryCatalog<IEditHandler> FactoryCatalog
        {
            set { factoryCatalog = value; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableService"/> class.
        /// </summary>
        public EditableService()
        {
            Application.Idle += new EventHandler(ApplicationIdle);
        }

        #region IEditorManager Members

        /// <summary>
        /// Register an EditHandler.
        /// </summary>
        /// <param name="editHandler"></param>
        public void Register(IEditHandler handler)
        {
            Guard.ArgumentNotNull(handler, "editHandler");
            handler.Enter += new EventHandler(EditEnter);
            handler.Leave += new EventHandler(EditLeave);
        }

        /// <summary>
        /// Register an UI element which is wrapped by an adapter to support the IEditHandler interface.
        /// </summary>
        /// <param name="uiElement"></param>
        public void Register(object uiElement)
        {
            IEditHandler handler = factoryCatalog.GetFactory(uiElement).GetAdapter(uiElement);
            handlers.Add(uiElement, handler);
            Register(handler);
        }

        /// <summary>
        /// Deregister an EditHandler. The method does not throw an exception if it is called more than
        /// once for the same object.
        /// </summary>
        /// <param name="editHandler"></param>
        public void UnRegister(IEditHandler handler)
        {
            Guard.ArgumentNotNull(handler, "editHandler");
            handler.Enter -= EditEnter;
            handler.Leave -= EditLeave;
        }

        /// <summary>
        /// Deregister an UI element. The method does not throw an exception if it is called more than
        /// once for the same object.
        /// </summary>
        /// <param name="uiElement"></param>
        public void UnRegister(object uiElement)
        {
            if (handlers.ContainsKey(uiElement))
            {
                UnRegister(handlers[uiElement]);
                handlers.Remove(uiElement);
            }
        }

        #endregion
        
        #region Assistant functions

        private void UpdateCommandStatus()
        {
            bool enabled = (activeHandler != null);

            SetCommandStatus(CommandHandlerNames.CMD_EDIT_UNDO, enabled && activeHandler.CanUndo);
            SetCommandStatus(CommandHandlerNames.CMD_EDIT_REDO, enabled && activeHandler.CanRedo);
            SetCommandStatus(CommandHandlerNames.CMD_EDIT_CUT, enabled && activeHandler.CanCut);
            SetCommandStatus(CommandHandlerNames.CMD_EDIT_COPY, enabled && activeHandler.CanCopy);
            SetCommandStatus(CommandHandlerNames.CMD_EDIT_PASTE, enabled && activeHandler.CanPaste);
            SetCommandStatus(CommandHandlerNames.CMD_EDIT_DELETE, enabled && activeHandler.CanDelete);
            SetCommandStatus(CommandHandlerNames.CMD_EDIT_SELECTALL, enabled && activeHandler.CanSelectAll);
            SetCommandStatus(CommandHandlerNames.CMD_EDIT_FILTER, enabled && activeHandler.CanFilter);
            SetCommandStatus(CommandHandlerNames.CMD_EDIT_SEARCH, enabled && activeHandler.CanSearch);
            SetCommandStatus(CommandHandlerNames.CMD_EDIT_REPLACE, enabled && activeHandler.CanReplace);
        }

        private void SetCommandStatus(string commandName, bool enabled)
        {
            workItem.Commands[commandName].Status = (enabled) ? CommandStatus.Enabled : CommandStatus.Disabled;
        }

        #endregion

        #region Command and Event handler

        private void EditEnter(object sender, EventArgs e)
        {
            Microsoft.Practices.CompositeUI.Utility.Guard.TypeIsAssignableFromType(sender.GetType(), typeof(IEditHandler), "sender");

            activeHandler = (IEditHandler)sender;
            UpdateCommandStatus();
        }

        private void EditLeave(object sender, EventArgs e)
        {
            activeHandler = null;
            UpdateCommandStatus();
        }

        private void ApplicationIdle(object sender, EventArgs e)
        {
            UpdateCommandStatus();
        }

        [CommandHandler(CommandHandlerNames.CMD_EDIT_UNDO)]
        public void Undo(object sender, EventArgs e)
        {
            activeHandler.Undo();
        }

        [CommandHandler(CommandHandlerNames.CMD_EDIT_REDO)]
        public void Redo(object sender, EventArgs e)
        {
            activeHandler.Redo();
        }

        [CommandHandler(CommandHandlerNames.CMD_EDIT_CUT)]
        public void Cut(object sender, EventArgs e)
        {
            activeHandler.Cut();
        }

        [CommandHandler(CommandHandlerNames.CMD_EDIT_COPY)]
        public void Copy(object sender, EventArgs e)
        {
            activeHandler.Copy();
        }

        [CommandHandler(CommandHandlerNames.CMD_EDIT_PASTE)]
        public void Paste(object sender, EventArgs e)
        {
            activeHandler.Paste();
        }

        [CommandHandler(CommandHandlerNames.CMD_EDIT_DELETE)]
        public void Delete(object sender, EventArgs e)
        {
            activeHandler.Delete();
        }

        [CommandHandler(CommandHandlerNames.CMD_EDIT_SELECTALL)]
        public void SelectAll(object sender, EventArgs e)
        {
            activeHandler.SelectAll();
        }

        [CommandHandler(CommandHandlerNames.CMD_EDIT_FILTER)]
        public void Filter(object sender, EventArgs e)
        {
            activeHandler.Filter();
        }

        [CommandHandler(CommandHandlerNames.CMD_EDIT_SEARCH)]
        public void Search(object sender, EventArgs e)
        {
            activeHandler.Search();
        }

        [CommandHandler(CommandHandlerNames.CMD_EDIT_REPLACE)]
        public void Replace(object sender, EventArgs e)
        {
            activeHandler.Replace();
        }

        #endregion
    }
}
