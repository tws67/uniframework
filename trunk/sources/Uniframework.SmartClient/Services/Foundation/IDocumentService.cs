using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 文档接口，用于实现文档操作的最简单集合
    /// </summary>
    public interface IDocument
    {
        /// <summary>
        /// 文档激活事件
        /// </summary>
        event EventHandler DocumentActivated;
        /// <summary>
        /// 离开文档事件
        /// </summary>
        event EventHandler DocumentDeactivated;
        /// <summary>
        /// 文档清理事件
        /// </summary>
        event EventHandler Disposed;
        /// <summary>
        /// 文档类型
        /// </summary>
        /// <value>文档类型.</value>
        IDocumentType DocumentType { get; }
        /// <summary>
        /// 文档名称
        /// </summary>
        /// <value>文档名称.</value>
        string FileName { get; }
        /// <summary>
        /// 返回一个值决定当前文档是否可以保存
        /// </summary>
        /// <value>如果可以保存返回<c>true</c>; 否则为, <c>false</c>.</value>
        bool CanSave { get; }
        /// <summary>
        /// 可在文档
        /// </summary>
        /// <param name="filename">文档名称.</param>
        void Save(string filename);
        /// <summary>
        /// 返回一个值决定是否可以从一个文件中导入文档
        /// </summary>
        /// <value>
        /// 	如果可以导入文档返回<c>true</c>; 否则为, <c>false</c>.
        /// </value>
        bool CanImport { get; }
        /// <summary>
        /// 从文件中导入文档
        /// </summary>
        /// <param name="filename">The filename.</param>
        void Import(string filename);
        /// <summary>
        /// 返回一个值决定是否可以将当前文档导出到文件
        /// </summary>
        /// <value>
        /// 	如果可以导出到文件返回<c>true</c>; 否则为, <c>false</c>.
        /// </value>
        bool CanExport { get; }
        /// <summary>
        /// 导出文档到特定文件.
        /// </summary>
        /// <param name="filename">The filename.</param>
        void Export(string filename);
    }

    /// <summary>
    /// 文档类型.
    /// </summary>
    public interface IDocumentType
    {
        /// <summary>
        /// 类型描述
        /// </summary>
        /// <value>描述信息.</value>
        string Description { get; }
        /// <summary>
        /// 扩展名
        /// </summary>
        /// <value>文档类型扩展名.</value>
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
        /// <summary>
        /// 返回目前支持的文档类型
        /// </summary>
        /// <value>文档类型列表.</value>
        ReadOnlyCollection<IDocumentType> DocumentTypes { get; }

        /// <summary>
        /// 注册文档工厂
        /// </summary>
        /// <param name="documentFactory">文档工厂.</param>
        void Register(IDocumentFactory documentFactory);
        /// <summary>
        /// 新建指定文档类型的文档
        /// </summary>
        /// <param name="documentType">文档类型.</param>
        void New(IDocumentType documentType);
    }
}
