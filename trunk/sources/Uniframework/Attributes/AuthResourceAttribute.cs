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
        private string module;
        private string path;
        private string catalog;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="path">路径</param>
        public AuthResourceAttribute(string module, string path)
        {
            this.module = module;
            this.path = path;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="path">路径</param>
        /// <param name="description">分组</param>
        public AuthResourceAttribute(string module, string path, string catalog)
            : this(module, path)
        {
            this.catalog = catalog;
        }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string Module
        {
            get { return module; }
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
