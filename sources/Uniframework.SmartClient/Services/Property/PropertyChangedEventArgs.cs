﻿using System;
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
        Property property;
        object oldValue;

        #region Members

        /// <returns>
        /// returns the changed property object
        /// </returns>
        public Property Property
        {
            get
            {
                return property;
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
        public PropertyChangedEventArgs(Property property, object oldValue)
        {
            this.property = property;
            this.oldValue = oldValue;
        }
    }
}
