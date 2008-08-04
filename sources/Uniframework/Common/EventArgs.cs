using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// �����¼������࣬�����ṩ��һ�����͵��¼�����
    /// </summary>
    /// <typeparam name="T">�¼�����</typeparam>
    [Serializable]
    public class EventArgs<T> : EventArgs
    {
        T data;
        
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="data"></param>
        public EventArgs(T data)
        {
            this.data = data;
        }

        /// <summary>
        /// ������������������
        /// </summary>
        public T Data
        {
            get
            {
                return data;
            }
        }
    }
}
