using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.ObjectBuilder;

using DevExpress.XtraTreeList;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// DexExpress TreeList文档适配器
    /// </summary>
    public class TreeListDocumentAdapter : IDocumentHandler, IDisposable
    {
        private TreeList adaptee;
        private string filename = string.Empty;
        private List<IDocumentType> supportTypes = new List<IDocumentType>();

        public TreeListDocumentAdapter(TreeList adaptee)
        {
            this.adaptee = adaptee;

            supportTypes.Add(new PlainDocumentType("电子表格 (*.xls)", ".xls"));
            supportTypes.Add(new PlainDocumentType("XML文件 (*.xml)", ".xml"));
            supportTypes.Add(new PlainDocumentType("网页 (*.htm; *.html)", ".html"));
            supportTypes.Add(new PlainDocumentType("单个文件网页 (*.mht)", ".mht"));
            supportTypes.Add(new PlainDocumentType("PDF文件 (*.pdf)", ".pdf"));
            supportTypes.Add(new PlainDocumentType("富格式文本 (*.rtf)", ".rtf"));
            supportTypes.Add(new PlainDocumentType("纯文本 (*.txt)", ".txt"));

            RegisterEvent();
        }

        #region Assistant functions

        private void RegisterEvent()
        {
            adaptee.Enter += new EventHandler(adaptee_Enter);
            adaptee.Leave += new EventHandler(adaptee_Leave);
        }

        private void adaptee_Leave(object sender, EventArgs e)
        {
            OnLeave(e);
        }

        private void adaptee_Enter(object sender, EventArgs e)
        {
            OnEnter(e);
        }

        protected void OnEnter(EventArgs e)
        {
            if (Enter != null)
                Enter(this, e);

            if (DocumentActivated != null)
                DocumentActivated(this, e);
        }

        protected void OnLeave(EventArgs e)
        {
            if (Leave != null)
                Leave(this, e);

            if (DocumentDeactivated != null)
                DocumentDeactivated(this, e);
        }

        #endregion

        #region IDocumentHandler Members

        public event EventHandler Enter;

        public event EventHandler Leave;

        public event EventHandler DocumentActivated;

        public event EventHandler DocumentDeactivated;

        public event EventHandler Disposed;

        public List<IDocumentType> SupportTypes
        {
            get { return supportTypes; }
        }

        public string FileName
        {
            get { return filename; }
        }

        public bool CanSave
        {
            get { return adaptee.Nodes.Count > 0; }
        }

        public void Save(string filename)
        {
            filename = String.IsNullOrEmpty(Path.GetExtension(filename)) ? filename + ".xls" : filename;

            using (WaitCursor cursor = new WaitCursor(true))
            {
                switch (Path.GetExtension(filename))
                {
                    case ".xls":
                        adaptee.ExportToXls(filename);
                        break;

                    case ".xml":
                        adaptee.ExportToXml(filename);
                        break;

                    case ".htm":
                    case ".html":
                        adaptee.ExportToHtml(filename);
                        break;

                    case ".pdf":
                        adaptee.ExportToPdf(filename);
                        break;

                    case ".rtf":
                        adaptee.ExportToRtf(filename);
                        break;

                    case ".txt":
                        adaptee.ExportToText(filename);
                        break;

                    case ".mht":
                        adaptee.ExportToMht(filename, "UTF-8");
                        break;
                }
            }
            this.filename = filename;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (Disposed != null)
                Disposed(this, new EventArgs());
        }

        #endregion
    }
}
