using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch
{
    /// <summary>
    /// ͨ�������Ļ�����Դ
    /// </summary>
    public class ConferenceResource
    {
        private ConferenceType confmode = ConferenceType.UNKOWN;
        private int confgroup = -1;

        public ConferenceResource(ConferenceType mode, int confGroup)
        {
            this.confmode = mode;
            this.confgroup = confGroup;
        }

        #region ConferenceResource Members

        public ConferenceType Confmode
        {
            get { return confmode; }
            set { confmode = value; }
        }

        public int Confgroup
        {
            get { return confgroup; }
            set { confgroup = value; }
        }

        #endregion
    }
}
