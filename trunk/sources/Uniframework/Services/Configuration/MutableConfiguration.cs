using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 可变配置项
    /// </summary>
    [Serializable]
    public class MutableConfiguration : AbstractConfiguration
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public MutableConfiguration()
            : this(string.Empty, null)
        { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name"></param>
        public MutableConfiguration(String name)
            : this(name, null)
        {
        }

        /// <summary>
        /// 构造函数（重载）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public MutableConfiguration(String name, String value)
        {
            base.internalName = name;
            base.internalValue = value;
        }
    }
}
