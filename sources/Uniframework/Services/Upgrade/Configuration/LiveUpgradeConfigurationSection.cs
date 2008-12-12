using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class LiveUpgradeConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// Gets the upgrade products.
        /// </summary>
        /// <value>The upgrade products.</value>
        [ConfigurationProperty("UpgradeElements")]
        public UpgradeElements UpgradeProducts
        {
            get { return this["UpgradeElements"] as UpgradeElements; }
        }
    }
}
