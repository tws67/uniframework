using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// 字符串服务
    /// </summary>
    public class StringService : IStringService
    {
        private Dictionary<string, string> dictionary = new Dictionary<string, string>();

        #region IStringService Members

        /// <summary>
        /// Registers the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Register(string key, string value)
        {
            dictionary[key] = value;
        }

        /// <summary>
        /// Gets the <see cref="System.String"/> at the specified index.
        /// </summary>
        /// <value></value>
        public string this[string index]
        {
            get { return Get(index); }
        }

        #endregion

        #region Assistant functions

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private string Get(string key)
        {
            string innerKey = FileUtility.Parse(key, null);
            return dictionary.ContainsKey(innerKey) ? dictionary[innerKey] : String.Empty;
        }

        #endregion
    }
}
