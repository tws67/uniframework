using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// ����������
    /// </summary>
    public static class ArgumentHelper
    {
        /// <summary>
        /// ����ָ��ʵ����Ϊ��
        /// </summary>
        /// <typeparam name="T">ʵ������</typeparam>
        /// <param name="arg">ʵ������</param>
        /// <param name="argName">ʵ������</param>
        public static void AssertNotNull<T>(T arg, string argName) where T : class
        {
            if (arg == null)
            {
                throw new ArgumentNullException(argName);
            }
        }

        /// <summary>
        /// ����Guid��ʶ��Ϊ��
        /// </summary>
        /// <param name="id">Guid��ʶ</param>
        /// <param name="argName">ʵ������</param>
        public static void AssertGuidNotEmpty(Guid id, string argName)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException(argName + "����Ϊ�գ�Empty��");
            }
        }

        /// <summary>
        /// ����ָ���ַ�����Ϊ��
        /// </summary>
        /// <param name="argName">ʵ������</param>
        public static void AssertStringNotNull(string argName)
        {
            if (argName == null || argName == string.Empty)
            {
                throw new ArgumentNullException(argName);
            }
        }

        /// <summary>
        /// ����ö�ٳ�Ա�Ƿ����ڸ�ö��
        /// </summary>
        /// <typeparam name="TEnum">ö������</typeparam>
        /// <param name="enumValue">ö��ֵ</param>
        public static void AssertEnumMember<TEnum>(TEnum enumValue) where TEnum : struct
        {
            if (!Enum.IsDefined(enumValue.GetType(), enumValue))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Enum value '{0}' is not defined for enumeration '{1}'.", enumValue, enumValue.GetType().FullName));
            }
        }
    }
}
