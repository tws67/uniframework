using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// 可授权资源
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple=true, Inherited=true)]
    public class AuthResourceAttribute : Attribute
    {
        private string name;
        private string path;
        private string catalog;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="path">路径</param>
        public AuthResourceAttribute(string name, string path)
        {
            this.name = name;
            this.path = path;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="path">路径</param>
        /// <param name="description">分组</param>
        public AuthResourceAttribute(string name, string path, string catalog)
            : this(name, path)
        {
            this.catalog = catalog;
        }

        /// <summary>
        /// 资源标识
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// 资源路径
        /// </summary>
        public string Path
        {
            get { return path; }
        }

        /// <summary>
        /// 资源名称/类别
        /// </summary>
        public string Catalog
        {
            get { return catalog; }
        }
    }
}
