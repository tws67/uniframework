using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 控制器
    /// </summary>
    public class EditorController : WorkItemController, IDocumentFactory
    {
        #region Dependency services

        [ServiceDependency]
        public DocumentService DocumentService
        {
            get;
            set;
        }

        #endregion

        #region IDocumentFactory Members

        public IDocument New()
        {
            throw new NotImplementedException();
        }

        public IDocument Open(string filename)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDocumentType Members

        public string Description
        {
            get { return "RTF 文档 (*.rtf)"; }
        }

        public string Extension
        {
            get { return "*.rtf"; }
        }

        #endregion

        public override void OnRunStarted()
        {
            base.OnRunStarted();

            DocumentService.Register(this); // 注册自身
        }
    }
}
