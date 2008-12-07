using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// �Ự״̬�ӿ�
    /// </summary>
    public interface ISessionState
    {
        /// <summary>
        /// �Ự��ʶ
        /// </summary>
        string SessionId { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        /// <param name="key">��ʶ</param>
        /// <returns></returns>
        object this[object key] { get; set; }
        /// <summary>
        /// �Ƴ����лỰ״̬
        /// </summary>
        void RemoveAll();
        /// <summary>
        /// �Ƴ�ָ����ʶ��ʵ��
        /// </summary>
        /// <param name="key">��ʶ</param>
        void Remove(object key);
        /// <summary>
        /// �ж��Ƿ���ڸ�keyֵ
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Exists(object key);
    }
}
