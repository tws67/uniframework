using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 系统配置项抽象类，所有实现类都要从此类继承
    /// </summary>
    [Serializable]
    public abstract class AbstractConfiguration : IConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        protected String internalName;
        /// <summary>
        /// 
        /// </summary>
        protected String internalValue;
        private NameValueCollection attributes = new NameValueCollection();
        private ConfigurationCollection children = new ConfigurationCollection();

        /// <summary>
        /// Gets the name of the <see cref="IConfiguration"/>.
        /// </summary>
        /// <value>
        /// The Name of the <see cref="IConfiguration"/>.
        /// </value>
        public virtual String Name
        {
            get { return internalName; }
        }

        /// <summary>
        /// Gets the value of <see cref="IConfiguration"/>.
        /// </summary>
        /// <value>
        /// The Value of the <see cref="IConfiguration"/>.
        /// </value>
        public virtual String Value
        {
            get { return internalValue; }
        }

        /// <summary>
        /// Gets all child nodes.
        /// </summary>
        /// <value>The <see cref="ConfigurationCollection"/> of child nodes.</value>
        public virtual ConfigurationCollection Children
        {
            get { return children; }

        }

        /// <summary>
        /// Gets node attributes.
        /// </summary>
        /// <value>
        /// All attributes of the node.
        /// </value>
        public virtual NameValueCollection Attributes
        {
            get { return attributes; }

        }

        /// <summary>
        /// Gets the value of the node and converts it
        /// into specified <see cref="System.Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="System.Type"/></param>
        /// <param name="defaultValue">
        /// The Default value returned if the convertion fails.
        /// </param>
        /// <returns>The Value converted into the specified type.</returns>
        public virtual object GetValue(Type type, object defaultValue)
        {
            try
            {
                return Convert.ChangeType(Value, type, null);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string tmp = "NAME: " + Name + ", VALUE: " + (String.IsNullOrEmpty(Value) ? "- " : Value) + " ATTRIBUTE: ";
            foreach (string key in Attributes.AllKeys)
            {
                tmp += String.Format("{0}[{1}], ", key, Attributes[key]);
            }
            if (tmp.EndsWith(", "))
                tmp = tmp.Substring(0, tmp.Length - 2);
            return tmp;
        }
    }
}
