using System;
using System.Collections.Generic;
using System.Text;

using Uniframework.Services;

namespace Uniframework.Upgrade
{
    /// <summary>
    /// 本地系统更新服务
    /// </summary>
    public interface ILiveUpgradeService
    {
        /// <summary>
        /// 获取系统更新的配置文件定义的配置信息
        /// </summary>
        UpgradeProject GetValidUpgradeProject();
        /// <summary>
        /// 更新提示
        /// </summary>
        /// <param name="project">升级项目</param>
        void UpgradeNotify(UpgradeProject project);
    }
}
