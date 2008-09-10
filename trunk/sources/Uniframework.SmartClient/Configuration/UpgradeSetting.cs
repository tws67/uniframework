using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 系统更新选项属性
    /// </summary>
    public class UpgradeSetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradeSetting"/> class.
        /// </summary>
        public UpgradeSetting()
        {
            UpgradeUrl = "localhost";
            ReciveUpgradeMessage = true;
            CheckInterval = 30;
        }

        /// <summary>
        /// Gets or sets the upgrade URL.
        /// </summary>
        /// <value>The upgrade URL.</value>
        public string UpgradeUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [recive upgrade message].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [recive upgrade message]; otherwise, <c>false</c>.
        /// </value>
        public bool ReciveUpgradeMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the check interval.
        /// </summary>
        /// <value>The check interval.</value>
        public int CheckInterval
        {
            get;
            set;
        }
    }
}
