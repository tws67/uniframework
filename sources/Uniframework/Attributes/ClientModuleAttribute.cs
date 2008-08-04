using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// �ͻ���ģ�����Ա�ǩ
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class ClientModuleAttribute : Attribute
    {
        private string assemblyFile;

        /// <summary>
        /// �޲ι��캯�������Ӵ˹��캯����Ҫ��Ϊ���ܹ���XmlSerializer�������л�
        /// </summary>
        public ClientModuleAttribute()
            : this(string.Empty)
        { }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="assemblyFile">�����ļ�</param>
        public ClientModuleAttribute(string assemblyFile)
        { 
            this.assemblyFile = assemblyFile;
        }

        /// <summary>
        /// �ͻ���ģ������ļ�
        /// </summary>
        public string AssemblyFile
        {
            get 
            {
                return assemblyFile;
            }
        }
    }
}
