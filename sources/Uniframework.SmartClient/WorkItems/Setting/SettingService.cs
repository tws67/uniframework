using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;

namespace Uniframework.SmartClient.WorkItems.Setting
{
    public class SettingService : ISettingService
    {
        private IDictionary<string, ISetting> settings = new Dictionary<string, ISetting>();

        [EventPublication(EventNames.Shell_SettingViewChanged, PublicationScope.Global)]
        public event EventHandler<EventArgs<ISetting>> SettingViewChanged;
        [EventPublication(EventNames.Shell_SettingSaved, PublicationScope.Global)]
        public event EventHandler<EventArgs<ISetting>> SettingSaved;

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
                if (SettingSaved != null)
                    SettingSaved(this, new EventArgs<ISetting>(kv.Value));
            }
        }

        public void LoadDefault(ISetting setting)
        {
            Guard.ArgumentNotNull(setting, "Setting to load default value must be set.");
            setting.LoadDefault();
        }

        /// <summary>
        /// 触发当前设置面板改变事件
        /// </summary>
        /// <param name="setting">设置项</param>
        public void RaiseSettingViewChanged(ISetting setting)
        {
            if (SettingViewChanged != null)
                SettingViewChanged(this, new EventArgs<ISetting>(setting));
        }

        #endregion
    }
}
