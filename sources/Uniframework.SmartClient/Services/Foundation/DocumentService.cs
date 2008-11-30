using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.Common;
using log4net;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 文档服务
    /// </summary>
    [Service]
    public class DocumentService : IDocumentService, IDisposable
    {
        private Dictionary<string, IDocumentFactory> documentFactories;
        private Dictionary<object, IDocumentHandler> handlers;
        private IDocumentHandler activeDocument = null;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        private WorkItem workItem;
        private ILog logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentService"/> class.
        /// </summary>
        public DocumentService()
        {
            documentFactories = new Dictionary<string, IDocumentFactory>();
            handlers = new Dictionary<object, IDocumentHandler>();
            openFileDialog = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog();

            Application.Idle += new EventHandler(Application_Idle);
        }

        #region Dependency services

        [ServiceDependency]
        public IAdapterFactoryCatalog<IDocumentHandler> FactoryCatalog
        {
            get;
            set;
        }

        [ServiceDependency]
        public WorkItem WorkItem
        {
            get { return workItem; }
            set {
                workItem = value;
                ActiveDocument = null;
                workItem.Commands[CommandHandlerNames.CMD_FILE_OPEN].Status = CommandStatus.Disabled;
            }
        }

        [ServiceDependency]
        public ILog Logger
        {
            set { logger = value; }
        }

        #endregion

        #region IDocumentService Members

        /// <summary>
        /// 返回目前支持的文档类型
        /// </summary>
        /// <value>文档类型列表.</value>
        public ReadOnlyCollection<IDocumentType> DocumentTypes
        {
            get
            {
                IDocumentType[] array = new IDocumentType[documentFactories.Count];
                int i = 0;
                foreach (IDocumentFactory factory in documentFactories.Values)
                {
                    array[i] = factory;
                    i++;
                }
                return new ReadOnlyCollection<IDocumentType>(array);
            }
        }

        /// <summary>
        /// 注册文档工厂
        /// </summary>
        /// <param name="documentFactory">文档工厂.</param>
        public void Register(IDocumentFactory documentFactory)
        {
            documentFactories.Add(documentFactory.Extension, documentFactory);
            openFileDialog.Filter = AppendFilter(openFileDialog.Filter, documentFactory);

            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_FILE_OPEN, documentFactories.Count > 0);
        }

        /// <summary>
        /// 注册文档处理组件
        /// </summary>
        /// <param name="uiElement">文档处理UI组件.</param>
        public void Register(object uiElement)
        {
            IDocumentHandler handler = FactoryCatalog.GetFactory(uiElement).GetAdapter(uiElement);
            if (handler != null) {
                handlers.Add(uiElement, handler);
                Register(handler);
            }
        }

        /// <summary>
        /// 注册文档处理器
        /// </summary>
        /// <param name="handler">文档处理器</param>
        public void Register(IDocumentHandler handler)
        {
            Guard.ArgumentNotNull(handler, "DocumentHandler");

            handler.Enter += OnEnter;
            handler.Leave += OnLeave;
        }

        /// <summary>
        /// 注销文档处理组件
        /// </summary>
        /// <param name="uiElement">文档处理UI组件.</param>
        public void UnRegister(object uiElement)
        {
            if (handlers.ContainsKey(uiElement)) {
                UnRegister(handlers[uiElement]);
                handlers.Remove(uiElement);
            }
        }

        /// <summary>
        /// 注销文档处理器
        /// </summary>
        /// <param name="handler">文档处理器.</param>
        public void UnRegister(IDocumentHandler handler)
        {
            Guard.ArgumentNotNull(handler, "DocumentHandler");

            handler.Enter -= OnEnter;
            handler.Leave -= OnLeave;
        }

        /// <summary>
        /// 新建指定文档类型的文档
        /// </summary>
        /// <param name="documentType">文档类型.</param>
        public void New(IDocumentType documentType)
        {
            IDocumentFactory factory = (IDocumentFactory)documentType;
            IDocumentHandler document = factory.New();
            if (document != null) { 
                InitializeDocument(document); 
            }
        }

        private IDocumentHandler ActiveDocument
        {
            get { return activeDocument; }
            set {
                activeDocument = value;
                UpdateCommandStatus();
            }
        }

        #endregion

        #region Command && Event handler

        /// <summary>
        /// 打开文档
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_FILE_OPEN)]
        public void OnOpen(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string extension = Path.GetExtension(openFileDialog.FileName);

                IDocumentFactory documentFactory = documentFactories[extension];
                IDocumentHandler document = documentFactory.Open(openFileDialog.FileName);

                if (document != null) { 
                    InitializeDocument(document); 
                }
            }
        }

        /// <summary>
        /// 保存文档
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_FILE_SAVE)]
        public void OnSave(object sender, EventArgs e)
        {
            Save(ActiveDocument.FileName);
        }

        /// <summary>
        /// 另存文档
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_FILE_SAVEAS)]
        public void OnSaveAs(object sender, EventArgs e)
        {
            Save(null);
        }

        #endregion

        #region Assistant functions

        private void OnEnter(object sender, EventArgs e)
        {
            Microsoft.Practices.CompositeUI.Utility.Guard.TypeIsAssignableFromType(sender.GetType(), typeof(IDocumentHandler), "sender");

            activeDocument = (IDocumentHandler)sender;
            UpdateCommandStatus();
        }

        private void OnLeave(object sender, EventArgs e)
        {
            activeDocument = null;
            UpdateCommandStatus();
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            UpdateCommandStatus();
        }

        /// <summary>
        /// Updates the command status.
        /// </summary>
        private void UpdateCommandStatus()
        {
            bool enabled = (activeDocument != null);

            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_FILE_OPEN, documentFactories.Count > 0);
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_FILE_SAVE, enabled && ActiveDocument.CanSave);
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_FILE_SAVEAS, enabled && ActiveDocument.CanSave);
        }

        private static string AppendFilter(string filter, IDocumentType extension)
        {
            if (!String.IsNullOrEmpty(filter)) { filter += "|"; }
            filter += extension.Description + "|*" + extension.Extension;
            return filter;
        }

        private static string AppendFilter(string filter, List<IDocumentType> extensions)
        {
            foreach (IDocumentType extension in extensions) {
                if (!String.IsNullOrEmpty(filter)) { filter += "|"; }
                filter += extension.Description + "|*" + extension.Extension;
            }
            return filter;
        }

        private void Save(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                saveFileDialog.Filter = AppendFilter(string.Empty, ActiveDocument.SupportTypes);

                if (saveFileDialog.ShowDialog() != DialogResult.OK) { return; }

                fileName = saveFileDialog.FileName;
            }

            ActiveDocument.Save(fileName);
        }

        private void InitializeDocument(IDocumentHandler document)
        {
            document.DocumentActivated += new EventHandler(DocumentActivated);
            document.DocumentDeactivated += new EventHandler(DocumentDeactivated);
            document.Disposed += new EventHandler(DocumentDisposed);
            ActiveDocument = document;
        }

        private void DocumentActivated(object sender, EventArgs e)
        {
            ActiveDocument = (IDocumentHandler)sender;
        }

        private void DocumentDeactivated(object sender, EventArgs e)
        {
            ActiveDocument = null;
        }

        private void DocumentDisposed(object sender, EventArgs e)
        {
            IDocumentHandler document = (IDocumentHandler)sender;
            document.DocumentActivated -= DocumentActivated;
            document.DocumentDeactivated -= DocumentDeactivated;
            document.Disposed -= DocumentDisposed;
            ActiveDocument = null;
        }

        #endregion

        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (openFileDialog != null) { openFileDialog.Dispose(); }
                if (saveFileDialog != null) { saveFileDialog.Dispose(); }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DocumentService()
        {
            Dispose(false);
        }

        #endregion

        private class NewDocumentEventHandler
        {
            private DocumentService documentService;
            private IDocumentType documentType;

            public NewDocumentEventHandler(DocumentService documentService, IDocumentType documentType)
            {
                this.documentService = documentService;
                this.documentType = documentType;
            }

            public void EventHandler(object sender, EventArgs e)
            {
                documentService.New(documentType);
            }
        }
    }
}
