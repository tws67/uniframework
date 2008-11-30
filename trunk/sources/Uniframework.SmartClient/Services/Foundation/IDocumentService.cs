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
    public interface IDocumentHandler
    {
        /// <summary>
        /// 获得焦点事件
        /// </summary>
        event EventHandler Enter;
        /// <summary>
        /// 失去焦点事件
        /// </summary>
        event EventHandler Leave;
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
        List<IDocumentType> SupportTypes { get; }
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
    /// 简单文档类型表示文档的描述及扩展名
    /// </summary>
    public class PlainDocumentType : IDocumentType
    {
        private string description;
        private string extension;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlainDocumentType"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="extension">The extension.</param>
        public PlainDocumentType(string description, string extension)
        {
            this.description = description;
            this.extension = extension;
        }

        #region IDocumentType Members

        public string Description
        {
            get { return description; }
        }

        public string Extension
        {
            get { return extension; }
        }

        #endregion
    }

    /// <summary>
    /// Describes a document factory which is able to create new documents and
    /// open existing ones.
    /// </summary>
    public interface IDocumentFactory : IDocumentType
    {
        IDocumentHandler New();
        IDocumentHandler Open(string filename);
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
        /// 注册文档处理组件
        /// </summary>
        /// <param name="uiElement">文档处理UI组件.</param>
        void Register(object uiElement);
        /// <summary>
        /// 注册文档处理器
        /// </summary>
        /// <param name="handler">文档处理器</param>
        void Register(IDocumentHandler handler);
        /// <summary>
        /// 注销文档处理组件
        /// </summary>
        /// <param name="uiElement">文档处理UI组件.</param>
        void UnRegister(object uiElement);
        /// <summary>
        /// 注销文档处理器
        /// </summary>
        /// <param name="handler">文档处理器.</param>
        void UnRegister(IDocumentHandler handler);
        /// <summary>
        /// 新建指定文档类型的文档
        /// </summary>
        /// <param name="documentType">文档类型.</param>
        void New(IDocumentType documentType);
    }
}
