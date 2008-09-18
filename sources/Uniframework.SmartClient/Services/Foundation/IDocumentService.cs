using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// Describes a document.
    /// </summary>
    public interface IDocument
    {
        event EventHandler DocumentActivated;
        event EventHandler DocumentDeactivated;
        event EventHandler Disposed;

        IDocumentType DocumentType { get; }
        string FileName { get; }

        void Save(string filename);
    }

    /// <summary>
    /// Describes a document type.
    /// </summary>
    public interface IDocumentType
    {
        string Description { get; }
        string Extension { get; }
    }

    /// <summary>
    /// Describes a document factory which is able to create new documents and
    /// open existing ones.
    /// </summary>
    public interface IDocumentFactory : IDocumentType
    {
        IDocument New();
        IDocument Open(string filename);
    }
    /// <summary>
    /// 文档服务接口
    /// </summary>
    public interface IDocumentService
    {
        ReadOnlyCollection<IDocumentType> DocumentTypes { get; }

        void Register(IDocumentFactory documentFactory);
        void New(IDocumentType documentType);
    }
}
