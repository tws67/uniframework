using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// ����Ȩ��Դ
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple=true, Inherited=true)]
    public class AuthResourceAttribute : Attribute
    {
        private string module;
        private string path;
        private string catalog;

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="name">����</param>
        /// <param name="path">·��</param>
        public AuthResourceAttribute(string module, string path)
        {
            this.module = module;
            this.path = path;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="name">����</param>
        /// <param name="path">·��</param>
        /// <param name="description">����</param>
        public AuthResourceAttribute(string module, string path, string catalog)
            : this(module, path)
        {
            this.catalog = catalog;
        }

        /// <summary>
        /// ģ������
        /// </summary>
        public string Module
        {
            get { return module; }
        }

        /// <summary>
        /// ��Դ·��
        /// </summary>
        public string Path
        {
            get { return path; }
        }

        /// <summary>
        /// ��Դ����/���
        /// </summary>
        public string Catalog
        {
            get { return catalog; }
        }
    }
}
