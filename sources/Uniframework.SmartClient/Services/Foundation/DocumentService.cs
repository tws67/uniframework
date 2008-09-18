using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 文档服务
    /// </summary>
    public class DocumentService : IDocumentService, IDisposable
    {
        private Dictionary<string, IDocumentFactory> documentFactories;
        private IDocument activeDocument;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        private WorkItem workItem;

        public DocumentService()
        {
            documentFactories = new Dictionary<string, IDocumentFactory>();
            openFileDialog = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog();
        }

        #region Dependency services

        [ServiceDependency]
        public WorkItem WorkItem
        {
            get { return workItem; }
            set
            {
                workItem = value;
                ActiveDocument = null;
                workItem.Commands[CommandHandlerNames.CMD_FILE_OPEN].Status = CommandStatus.Disabled;
            }
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
            if (document != null) { InitializeDocument(document); }
        }

        private IDocument ActiveDocument
        {
            get { return activeDocument; }
            set
            {
                activeDocument = value;

                CommandStatus status = CommandStatus.Disabled;
                if (activeDocument != null) { status = CommandStatus.Enabled; }

                WorkItem.Commands[CommandHandlerNames.CMD_FILE_SAVE].Status = status;
                WorkItem.Commands[CommandHandlerNames.CMD_FILE_SAVEAS].Status = status;
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
        public void Open(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string extension = Path.GetExtension(openFileDialog.FileName);

                IDocumentFactory documentFactory = documentFactories[extension];
                IDocument document = documentFactory.Open(openFileDialog.FileName);

                if (document != null) { InitializeDocument(document); }
            }
        }

        /// <summary>
        /// 保存文档
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_FILE_SAVE)]
        public void Save(object sender, EventArgs e)
        {
            Save(ActiveDocument.FileName);
        }

        /// <summary>
        /// 另存文档
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_FILE_SAVEAS)]
        public void SaveAs(object sender, EventArgs e)
        {
            Save(null);
        }

        #endregion

        #region Assistant functions

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
