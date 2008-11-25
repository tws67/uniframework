using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch
{
    #region Caller profile flag

    /// <summary>
    /// 呼叫配置标志
    /// </summary>
    public enum CallerProfileFlag
    { 
        /// <summary>
        /// 显示所有信息
        /// </summary>
        Screen = 0 << 0,
        /// <summary>
        /// 隐藏名字
        /// </summary>
        Hidename = 0 << 1,
        /// <summary>
        /// 隐藏号码
        /// </summary>
        Hidenumber = 0 << 2
    }
    #endregion

    /// <summary>
    /// 呼叫配置文件
    /// </summary>
    public class CallerProfile
    {
        #region Members

        /// <summary>
        /// 用户名称
        /// </summary>
        public String UserName
        {
            get { return username; }
            set { username = value; }
        }

        /// <summary>
        /// dial plan
        /// </summary>
        public String Dialplan
        {
            get { return dialplan; }
            set { dialplan = value; }
        }

        /// <summary>
        /// CallerID name 
        /// </summary>
        public String CallerIDName
        {
            get { return callerIDName; }
            set { callerIDName = value; }
        }

        /// <summary>
        /// CallerID number
        /// </summary>
        public String CallerIDNumber
        {
            get { return callerIDNumber; }
            set { callerIDNumber = value; }
        }

        /// <summary>
        /// Caller Network Address (when applicable) 
        /// </summary>
        public String NetworkAddr
        {
            get { return networkAddr; }
            set { networkAddr = value; }
        }

        /// <summary>
        /// ANI (when applicable) 主叫号码
        /// </summary>
        public String Ani
        {
            get { return ani; }
            set { ani = value; }
        }

        /// <summary>
        /// ANI II (when applicable) 被叫号码
        /// </summary>
        public String Aniii
        {
            get { return aniii; }
            set { aniii = value; }
        }

        /// <summary>
        /// RDNIS
        /// </summary>
        public String Rdnis
        {
            get { return rdnis; }
            set { rdnis = value; }
        }

        /// <summary>
        /// Destination number
        /// </summary>
        public String DestinationNumber
        {
            get { return destinationNumber; }
            set { destinationNumber = value; }
        }

        /// <summary>
        /// Channel type
        /// </summary>
        public String Source
        {
            get { return source; }
            set { source = value; }
        }

        /// <summary>
        /// 通道名称
        /// </summary>
        public String ChannelName
        {
            get { return channelName; }
            set { channelName = value; }
        }

        /// <summary>
        /// unique id 
        /// </summary>
        public String UUID
        {
            get { return uuid; }
            set { uuid = value; }
        }

        /// <summary>
        /// context
        /// </summary>
        public String Context
        {
            get { return context; }
            set { context = value; }
        }

        /// <summary>
        /// 通道应答时间表
        /// </summary>
        public List<ChannelTimetable> Times
        {
            get { return times; }
        }

        /// <summary>
        /// 呼叫扩展
        /// </summary>
        public List<ICallerExtension> CallerExtensions
        {
            get { return extensions; }
        }

        #endregion

        #region Member fields

        private String username = String.Empty;
        private String dialplan = String.Empty;
        private String callerIDName = String.Empty;
        private String callerIDNumber = String.Empty;
        private String networkAddr = String.Empty;
        private String ani = String.Empty;
        private String aniii = String.Empty;
        private String rdnis = String.Empty;
        private String destinationNumber = String.Empty;
        private String source = String.Empty;
        private String channelName = String.Empty;
        private String uuid = String.Empty;
        private String context = String.Empty;
        private CallerProfileFlag flags = CallerProfileFlag.Screen;
        private CallerProfile originatorCallerProfile;
        private CallerProfile originateeCallerProfile;
        private List<ChannelTimetable> times = new List<ChannelTimetable>();
        private List<ICallerExtension> extensions = new List<ICallerExtension>();
        #endregion
    }
}
