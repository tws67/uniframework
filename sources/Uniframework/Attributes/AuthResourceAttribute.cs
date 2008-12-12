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
        private string name;
        private string path;
        private string catalog;

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="name">����</param>
        /// <param name="path">·��</param>
        public AuthResourceAttribute(string name, string path)
        {
            this.name = name;
            this.path = path;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="name">����</param>
        /// <param name="path">·��</param>
        /// <param name="description">����</param>
        public AuthResourceAttribute(string name, string path, string catalog)
            : this(name, path)
        {
            this.catalog = catalog;
        }

        /// <summary>
        /// ��Դ��ʶ
        /// </summary>
        public string Name
        {
            get { return name; }
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
