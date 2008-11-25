using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch
{
    #region Call session flag

    public enum CallSessionFlag
    { 
        /// <summary>
        /// ��
        /// </summary>
        NONE = 0,
        /// <summary>
        /// �Ự�Ѿ�����
        /// </summary>
        RELEASED = 0 << 1
    }
    #endregion

    /// <summary>
    /// �Ự�ӿ�
    /// </summary>
    public interface ICallSession
    {
        /// <summary>
        /// �Ự��ʶ
        /// </summary>
        String CallID { get; }
        /// <summary>
        /// �Ự����
        /// </summary>
        String Name { get; set; }
        /// <summary>
        /// �Ự��־
        /// </summary>
        CallSessionFlag Sessionflags { get; }
        /// <summary>
        /// ��ǰ�Ự���е�ͨ��
        /// </summary>
        IChannel Channel { get; }
        /// <summary>
        /// �Ự�ն˽ӿ�
        /// </summary>
        //IEndpoint Endpoint { get; }
        /// <summary>
        /// ���������ļ�
        /// </summary>
        CallerProfile Profile { get; }
        /// <summary>
        /// �Ự�¼�������
        /// </summary>
        StateHandler StateHandler { get; }
    }
}
