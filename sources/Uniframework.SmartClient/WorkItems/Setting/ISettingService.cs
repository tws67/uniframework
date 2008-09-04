using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.SmartClient.WorkItems.Setting
{
    /// <summary>
    /// 系统设置服务接口
    /// </summary>
    public interface ISettingService
    {
        /// <summary>
        /// 注册一个设置项目到服务
        /// </summary>
        /// <param name="setting">The setting.</param>
        void RegisterSetting(ISetting setting);
        /// <summary>
        /// 保存当前所有的设置项目
        /// </summary>
        void Save();
        /// <summary>
        /// 为指定设置加载默认设置
        /// </summary>
        /// <param name="setting">设置项</param>
        void LoadDefault(ISetting setting);
    }
}
