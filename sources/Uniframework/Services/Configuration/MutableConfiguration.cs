using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// �ɱ�������
    /// </summary>
    [Serializable]
    public class MutableConfiguration : AbstractConfiguration
    {
        /// <summary>
        /// �޲ι��캯��
        /// </summary>
        public MutableConfiguration()
            : this(string.Empty, null)
        { }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="name"></param>
        public MutableConfiguration(String name)
            : this(name, null)
        {
        }

        /// <summary>
        /// ���캯�������أ�
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
