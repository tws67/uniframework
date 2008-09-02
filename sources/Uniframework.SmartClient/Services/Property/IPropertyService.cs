using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 系统属性服务，提供保存、加载系统需要的各类属性值
    /// </summary>
    public interface IPropertyService
    {
        /// <summary>
        /// Deletes the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        void Delete(string property);
        /// <summary>
        /// Gets the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        object Get(string property);
        /// <summary>
        /// Gets the specified property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">The property.</param>
        /// <param name="defautValue">The defaut value.</param>
        /// <returns></returns>
        T Get<T>(string property, T defautValue);
        /// <summary>
        /// Sets the specified property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        void Set<T>(string property, T value);
    }
}
