using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// 参数工具类
    /// </summary>
    public static class ArgumentHelper
    {
        /// <summary>
        /// 断言指定实例不为空
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="arg">实例参数</param>
        /// <param name="argName">实例名称</param>
        public static void AssertNotNull<T>(T arg, string argName) where T : class
        {
            if (arg == null)
            {
                throw new ArgumentNullException(argName);
            }
        }

        /// <summary>
        /// 断言Guid标识不为空
        /// </summary>
        /// <param name="id">Guid标识</param>
        /// <param name="argName">实例名称</param>
        public static void AssertGuidNotEmpty(Guid id, string argName)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException(argName + "不能为空（Empty）");
            }
        }

        /// <summary>
        /// 断言指定字符串不为空
        /// </summary>
        /// <param name="argName">实例参数</param>
        public static void AssertStringNotNull(string argName)
        {
            if (argName == null || argName == string.Empty)
            {
                throw new ArgumentNullException(argName);
            }
        }

        /// <summary>
        /// 断言枚举成员是否属于该枚举
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="enumValue">枚举值</param>
        public static void AssertEnumMember<TEnum>(TEnum enumValue) where TEnum : struct
        {
            if (!Enum.IsDefined(enumValue.GetType(), enumValue))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Enum value '{0}' is not defined for enumeration '{1}'.", enumValue, enumValue.GetType().FullName));
            }
        }
    }
}
