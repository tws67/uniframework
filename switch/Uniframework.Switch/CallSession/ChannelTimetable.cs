using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch
{
    /// <summary>
    /// ͨ��ִ���¼�ʱ���
    /// </summary>
    public class ChannelTimetable
    {
        #region Members

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime Created
        {
            get { return created; }
            set { created = value; }
        }

        /// <summary>
        /// Ӧ��ʱ��
        /// </summary>
        public DateTime Answered
        {
            get { return answered; }
            set { answered = value; }
        }

        /// <summary>
        /// �һ�ʱ��
        /// </summary>
        public DateTime Hangup
        {
            get { return hangup; }
            set { hangup = value; }
        }

        /// <summary>
        /// ת��ʱ��
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
