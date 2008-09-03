using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 系统保存的属性值
    /// </summary>
    public class Property
    {
        private string name;
        private bool modified = false;
        private object current;
        private object defaultProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        public Property()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultProperty">The default property.</param>
        public Property(string name, object defaultProperty)
            : this()
        {
            this.name = name;
            this.defaultProperty = defaultProperty;
        }

        #region Members

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public object Current
        {
            get { return current; }
            set { 
                current = value;
                modified = true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Property"/> is modified.
        /// </summary>
        /// <value><c>true</c> if modified; otherwise, <c>false</c>.</value>
        public bool Modified
        {
            get { return modified; }
        }

        #endregion

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            modified = false;
        }

        /// <summary>
        /// Sets the default.
        /// </summary>
        public void SetDefault()
        {
            if (defaultProperty != null)
                current = defaultProperty;
        }

        public override string ToString()
        {
            return name + " : " + Current.ToString();
        }
    }
}
