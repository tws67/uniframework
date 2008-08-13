using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Client.ConnectionManagement
{
    /// <summary>
    /// Windows网络连通状态检测策略类，此类在后期应该可以通过系统配置节来进行配置
    /// </summary>
    public class WinNetDetectionStrategy : IConnectionDetectionStrategy
    {
        private int pollInterval = 1;

        #region IConnectionDetectionStrategy Members

        /// <summary>
        /// 判断当前网络的连通状态
        /// </summary>
        /// <returns>如果连通则返回true，否则返回false</returns>
        public bool IsConnected()
        {
            return CommunicateProxy.Ping();
        }

        /// <summary>
        /// 检测的间隔时间（秒）
        /// </summary>
        public int PollInterval
        {
            get { return pollInterval; }
        }

        #endregion
    }
}
