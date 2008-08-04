using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class UpgradeElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the product.
        /// </summary>
        /// <value>The product.</value>
        [ConfigurationProperty("Product", IsKey = true, IsRequired = true)]
        public string Product
        {
            get { return this["Product"] as String; }
            set { this["Product"] = value; }
        }

        /// <summary>
        /// Gets or sets the local version.
        /// </summary>
        /// <value>The local version.</value>
        [ConfigurationProperty("LocalVersion", IsRequired = true)]
        public string LocalVersion
        {
            get { return this["LocalVersion"] as String; }
            set { this["LocalVersion"] = value; }
        }

        /// <summary>
        /// Gets or sets the upgrade date.
        /// </summary>
        /// <value>The upgrade date.</value>
        [ConfigurationProperty("UpgradeDate", IsRequired = true)]
        public DateTime UpgradeDate
        {
            get { return Convert.ToDateTime(this["UpgradeDate"]); }
            set { this["UpgradeDate"] = value.ToLocalTime(); }
        }

    }
}
