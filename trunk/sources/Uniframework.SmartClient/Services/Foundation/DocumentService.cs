using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
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
        private IDocument activeDocument = null;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        private WorkItem workItem;
        private ILog logger;

        public DocumentService()
        {
            documentFactories = new Dictionary<string, IDocumentFactory>();
            openFileDialog = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog();

            Application.Idle += new EventHandler(Application_Idle);
        }

        #region Dependency services

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

        public void Register(IDocumentFactory documentFactory)
        {
            documentFactories.Add(documentFactory.Extension, documentFactory);
            openFileDialog.Filter = AppendFilter(openFileDialog.Filter, documentFactory);

            WorkItem.Commands[CommandHandlerNames.CMD_FILE_OPEN].Status = documentFactories.Count > 0 ? CommandStatus.Enabled : CommandStatus.Disabled;
        }

        public void New(IDocumentType documentType)
        {
            IDocumentFactory factory = (IDocumentFactory)documentType;
            IDocument document = factory.New();
            if (document != null) { 
                InitializeDocument(document); 
            }
        }

        private IDocument ActiveDocument
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
                IDocument document = documentFactory.Open(openFileDialog.FileName);

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

        /// <summary>
        /// 导入文件
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_FILE_IMPORT)]
        public void OnImport(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                ActiveDocument.Import(openFileDialog.FileName);
            }
        }

        /// <summary>
        /// 导出文件
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_FILE_EXPORT)]
        public void OnExport(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                ActiveDocument.Export(saveFileDialog.FileName);
            }
        }

        #endregion

        #region Assistant functions

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

            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_FILE_OPEN, documentFactories.Count >= 1);
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_FILE_SAVE, enabled && ActiveDocument.CanSave);
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_FILE_SAVEAS, enabled && ActiveDocument.CanSave);
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_FILE_IMPORT, enabled && ActiveDocument.CanImport);
            BuilderUtility.SetCommandStatus(WorkItem, CommandHandlerNames.CMD_FILE_EXPORT, enabled && ActiveDocument.CanExport);
        }

        private static string AppendFilter(string filter, IDocumentType extension)
        {
            if (!String.IsNullOrEmpty(filter)) { filter += "|"; }
            filter += extension.Description + "|*" + extension.Extension;
            return filter;
        }

        private void Save(string fileName)
        {
            if (fileName == null)
            {
                saveFileDialog.Filter = AppendFilter("", ActiveDocument.DocumentType);

                if (saveFileDialog.ShowDialog() != DialogResult.OK) { return; }

                fileName = saveFileDialog.FileName;
            }

            ActiveDocument.Save(fileName);
        }

        private void InitializeDocument(IDocument document)
        {
            document.DocumentActivated += new EventHandler(DocumentActivated);
            document.DocumentDeactivated += new EventHandler(DocumentDeactivated);
            document.Disposed += new EventHandler(DocumentDisposed);
            ActiveDocument = document;
        }

        private void DocumentActivated(object sender, EventArgs e)
        {
            ActiveDocument = (IDocument)sender;
        }

        private void DocumentDeactivated(object sender, EventArgs e)
        {
            ActiveDocument = null;
        }

        private void DocumentDisposed(object sender, EventArgs e)
        {
            IDocument document = (IDocument)sender;
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
