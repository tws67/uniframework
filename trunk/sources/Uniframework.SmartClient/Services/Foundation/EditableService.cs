using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Uniframework.Security;
using Microsoft.Practices.CompositeUI.Services;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 文本编辑服务
    /// </summary>
    public class EditableService : IEditableService
    {
        private IEditHandler activeHandler = null;
        private Dictionary<object, IEditHandler> handlers = new Dictionary<object,IEditHandler>();

        private WorkItem workItem;
        private IAdapterFactoryCatalog<IEditHandler> factoryCatalog;

        #region Dependency services

        [ServiceDependency]
        public WorkItem WorkItem
        {
            get { return workItem; }
            set 
            { 
                workItem = value;
                UpdateCommandStatus();
            }
        }

        [ServiceDependency]
        public IAdapterFactoryCatalog<IEditHandler> FactoryCatalog
        {
            get { return factoryCatalog; }
            set { factoryCatalog = value; }
        }

        [ServiceDependency]
        public IAuthorizationService AuthorizationService
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableService"/> class.
        /// </summary>
        public EditableService()
        {
            Application.Idle += new EventHandler(Application_Idle);
        }

        #region IEditableService Members

        /// <summary>
        /// Register an EditHandler.
        /// </summary>
        /// <param name="editHandler"></param>
        public void Register(IEditHandler handler)
        {
            Guard.ArgumentNotNull(handler, "EditHandler");
            handler.Enter += new EventHandler(EditEnter);
            handler.Leave += new EventHandler(EditLeave);
        }

        /// <summary>
        /// Register an UI element which is wrapped by an adapter to support the IEditHandler interface.
        /// </summary>
        /// <param name="uiElement"></param>
        public void Register(object uiElement)
        {
            IEditHandler handler = FactoryCatalog.GetFactory(uiElement).GetAdapter(uiElement);
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
            Guard.ArgumentNotNull(handler, "EditHandler");
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
            if (handlers.ContainsKey(uiElement)) {
                UnRegister(handlers[uiElement]);
                handlers.Remove(uiElement);
            }
        }

        #endregion
        
        #region Assistant functions

        private void UpdateCommandStatus()
        {
            bool enabled = (activeHandler != null);

            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_EDIT_UNDO, enabled && activeHandler.CanUndo
                && CanExecute(CommandHandlerNames.CMD_EDIT_UNDO));
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_EDIT_REDO, enabled && activeHandler.CanRedo
                && CanExecute(CommandHandlerNames.CMD_EDIT_REDO));
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_EDIT_CUT, enabled && activeHandler.CanCut
                && CanExecute(CommandHandlerNames.CMD_EDIT_CUT));
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_EDIT_COPY, enabled && activeHandler.CanCopy
                && CanExecute(CommandHandlerNames.CMD_EDIT_COPY));
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_EDIT_PASTE, enabled && activeHandler.CanPaste
                && CanExecute(CommandHandlerNames.CMD_EDIT_PASTE));
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_EDIT_DELETE, enabled && activeHandler.CanDelete
                && CanExecute(CommandHandlerNames.CMD_EDIT_DELETE));
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_EDIT_SELECTALL, enabled && activeHandler.CanSelectAll
                && CanExecute(CommandHandlerNames.CMD_EDIT_SELECTALL));
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_EDIT_SEARCH, enabled && activeHandler.CanSearch
                && CanExecute(CommandHandlerNames.CMD_EDIT_SEARCH));
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_EDIT_REPLACE, enabled && activeHandler.CanReplace
                && CanExecute(CommandHandlerNames.CMD_EDIT_REPLACE));
        }

        private bool CanExecute(string command)
        {
            string authorizationUri = "";
            AuthorizationAttribute[] attrs = (AuthorizationAttribute[])activeHandler.GetType().GetCustomAttributes(typeof(AuthorizationAttribute), true);
            if (attrs.Length > 0)
                authorizationUri = attrs[0].AuthorizationUri;
            return AuthorizationService.CanExecute(SecurityUtility.HashObject(authorizationUri + command));
            return true;
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

        private void Application_Idle(object sender, EventArgs e)
        {
            UpdateCommandStatus();
        }

        [CommandHandler(CommandHandlerNames.CMD_EDIT_UNDO)]
        public void OnUndo(object sender, EventArgs e)
        {
            activeHandler.Undo();
        }

        [CommandHandler(CommandHandlerNames.CMD_EDIT_REDO)]
        public void OnRedo(object sender, EventArgs e)
        {
            activeHandler.Redo();
        }

        [CommandHandler(CommandHandlerNames.CMD_EDIT_CUT)]
        public void OnCut(object sender, EventArgs e)
        {
            activeHandler.Cut();
        }

        [CommandHandler(CommandHandlerNames.CMD_EDIT_COPY)]
        public void OnCopy(object sender, EventArgs e)
        {
            activeHandler.Copy();
        }

        [CommandHandler(CommandHandlerNames.CMD_EDIT_PASTE)]
        public void OnPaste(object sender, EventArgs e)
        {
            activeHandler.Paste();
        }

        [CommandHandler(CommandHandlerNames.CMD_EDIT_DELETE)]
        public void OnDelete(object sender, EventArgs e)
        {
            activeHandler.Delete();
        }

        [CommandHandler(CommandHandlerNames.CMD_EDIT_SELECTALL)]
        public void OnSelectAll(object sender, EventArgs e)
        {
            activeHandler.SelectAll();
        }

        [CommandHandler(CommandHandlerNames.CMD_EDIT_SEARCH)]
        public void OnSearch(object sender, EventArgs e)
        {
            activeHandler.Search();
        }

        [CommandHandler(CommandHandlerNames.CMD_EDIT_REPLACE)]
        public void OnReplace(object sender, EventArgs e)
        {
            activeHandler.Replace();
        }

        #endregion
    }
}
