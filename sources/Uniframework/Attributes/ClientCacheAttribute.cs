using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// �ͻ��˻������Ա�ǩ
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public class ClientCacheAttribute : Attribute
    {
        private string dataUpdateEvent;

        /// <summary>
        /// �޲ι��캯�������Ӵ˹��캯����Ҫ��Ϊ���ܹ���XmlSerializer�������л�
        /// </summary>
        public ClientCacheAttribute()
        { }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="dataUpdateEvent">�������ݸ����¼�</param>
        public ClientCacheAttribute(string dataUpdateEvent)
        {
            this.dataUpdateEvent = dataUpdateEvent;
        }

        /// <summary>
        /// ��ȡ���ݸ����¼�����
        /// </summary>
        public string DataUpdateEvent
        {
            get
            {
                return dataUpdateEvent;
            }
        }
    }
}
