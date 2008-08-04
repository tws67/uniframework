using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// 客户端模块属性标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class ClientModuleAttribute : Attribute
    {
        private string assemblyFile;

        /// <summary>
        /// 无参构造函数，增加此构造函数主要是为了能够让XmlSerializer进行序列化
        /// </summary>
        public ClientModuleAttribute()
            : this(string.Empty)
        { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="assemblyFile">程序集文件</param>
        public ClientModuleAttribute(string assemblyFile)
        { 
            this.assemblyFile = assemblyFile;
        }

        /// <summary>
        /// 客户端模块程序集文件
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
