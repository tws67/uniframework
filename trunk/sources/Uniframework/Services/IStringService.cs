using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// 字符串服务
    /// </summary>
    public interface IStringService
    {
        /// <summary>
        /// Registers the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void Register(string key, string value);
        /// <summary>
        /// Gets the <see cref="System.String"/> at the specified index.
        /// </summary>
        /// <value></value>
        string this[string index] { get; }
    }
}
