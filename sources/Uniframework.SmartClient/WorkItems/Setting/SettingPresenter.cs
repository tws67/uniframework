using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI.EventBroker;

namespace Uniframework.SmartClient.WorkItems.Setting
{
    public class SettingPresenter : Presenter<SettingView>
    {
        [ServiceDependency]
        public ISettingService SettingService
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current.
        /// </summary>
        /// <value>The current.</value>
        public ISetting Current { get; private set; }

        [EventSubscription(EventNames.Uniframework_ShowSettingView)]
        public void OnShowSettingView(object sender, EventArgs<ISetting> e)
        {
            if (e.Data != null) {
                Current = e.Data;
            }
        }

        /// <summary>
        /// 保存所有设置项目
        /// </summary>
        public void Save()
        {
            SettingService.Save();
        }

        /// <summary>
        /// 为当前设置项目加载默认的设置值
        /// </summary>
        public void LoadDefault()
        {
            if (Current != null)
                SettingService.LoadDefault(Current);
        }
    }
}
