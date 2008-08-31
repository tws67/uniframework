using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 系统属性变化参数
    /// </summary>
    public class PropertyChangedEventArgs : EventArgs
    {
        Properties properties;
        string key;
        object newValue;
        object oldValue;

        #region Members

        /// <returns>
        /// returns the changed property object
        /// </returns>
        public Properties Properties
        {
            get
            {
                return properties;
            }
        }

        /// <returns>
        /// The key of the changed property
        /// </returns>
        public string Key
        {
            get
            {
                return key;
            }
        }

        /// <returns>
        /// The new value of the property
        /// </returns>
        public object NewValue
        {
            get
            {
                return newValue;
            }
        }

        /// <returns>
        /// The new value of the property
        /// </returns>
        public object OldValue
        {
            get
            {
                return oldValue;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedEventArgs"/> class.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="key">The key.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public PropertyChangedEventArgs(Properties properties, string key, object oldValue, object newValue)
        {
            this.properties = properties;
            this.key = key;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
    }
}
