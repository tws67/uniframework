using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch
{
    /// <summary>
    /// 通道执行事件时间表
    /// </summary>
    public class ChannelTimetable
    {
        #region Members

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Created
        {
            get { return created; }
            set { created = value; }
        }

        /// <summary>
        /// 应答时间
        /// </summary>
        public DateTime Answered
        {
            get { return answered; }
            set { answered = value; }
        }

        /// <summary>
        /// 挂机时间
        /// </summary>
        public DateTime Hangup
        {
            get { return hangup; }
            set { hangup = value; }
        }

        /// <summary>
        /// 转接时间
        /// </summary>
        public DateTime Transferred
        {
            get { return transferred; }
            set { transferred = value; }
        }
        #endregion

        #region Member fields

        private DateTime created = DateTime.MinValue;
        private DateTime answered = DateTime.MinValue;
        private DateTime hangup = DateTime.MinValue;
        private DateTime transferred = DateTime.MinValue;
        #endregion
    }
}
