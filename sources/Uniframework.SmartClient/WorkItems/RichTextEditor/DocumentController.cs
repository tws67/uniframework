using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using System.IO;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 文档编辑控制器
    /// </summary>
    public class DocumentController : WorkItemController, IDocument
    {
        private IDocumentType documentType;
        private RichEditorView editorView;
        private string fileName = null;

        public string DocumentTitle
        {
            get {
                if (String.IsNullOrEmpty(fileName)) return "新建文档";
                return Path.GetFileName(fileName);
            }
        }

        #region IDocument Members

        public event EventHandler DocumentActivated;

        public event EventHandler DocumentDeactivated;

        public event EventHandler Disposed;

        public IDocumentType DocumentType
        {
            get { return documentType; }
            set { documentType = value; }
        }

        public string FileName
        {
            get { return fileName; }
        }

        public bool CanSave
        {
            get { return true; }
        }

        public void Save(string filename)
        {
            throw new NotImplementedException();
        }

        public bool CanImport
        {
            get { return false; }
        }

        public void Import(string filename)
        {
            throw new NotImplementedException();
        }

        public bool CanExport
        {
            get { return false; }
        }

        public void Export(string filename)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
