using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.SmartClient.WorkItems.Setting
{
    /// <summary>
    /// 系统设置接口
    /// </summary>
    public interface ISetting
    {
        /// <summary>
        /// Occurs when [setting changed].
        /// </summary>
        event EventHandler SettingChanged;

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>The property.</value>
        Property Property { get; }

        /// <summary>
        /// Bindings the property.
        /// </summary>
        void BindingProperty();
        
        /// <summary>
        /// Saves this instance.
        /// </summary>
        void Save();

        /// <summary>
        /// Loads the default.
        /// </summary>
        void LoadDefault();
    }
}
