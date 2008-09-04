using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.CompositeUI;

namespace Uniframework.SmartClient.WorkItems.Setting
{
    public class SettingService : ISettingService
    {
        private IDictionary<string, ISetting> settings = new Dictionary<string, ISetting>();

        #region ISettingService Members

        /// <summary>
        /// 注册一个设置项目到服务
        /// </summary>
        /// <param name="setting">The setting.</param>
        public void RegisterSetting(ISetting setting)
        {
            if (!settings.ContainsKey(setting.Property.Name))
                settings.Add(setting.Property.Name, setting);
        }

        /// <summary>
        /// 保存当前所有的设置项目
        /// </summary>
        public void Save()
        {
            foreach (KeyValuePair<string, ISetting> kv in settings) {
                kv.Value.Save();
            }
        }

        public void LoadDefault(ISetting setting)
        {
            Guard.ArgumentNotNull(setting, "Setting to load default value must be set.");
            setting.LoadDefault();
        }

        #endregion
    }
}
