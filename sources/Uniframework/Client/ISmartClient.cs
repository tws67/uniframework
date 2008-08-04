using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Client
{
    /// <summary>
    /// ��������״̬
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// ����
        /// </summary>
        Online = 0,
        /// <summary>
        /// ����
        /// </summary>
        Offline = 1,
        /// <summary>
        /// δ֪
        /// </summary>
        Unknown = -1
    }

    /// <summary>
    /// ��������״̬�ı��ί��
    /// </summary>
    /// <param name="state"></param>
    public delegate void ConnectionStateChangeHandler(ConnectionState state);

    /// <summary>
    /// SmartClient�������Խӿ�
    /// </summary>
    public interface ISmartClient
    {
        /// <summary>
        /// ����״̬�仯�¼�
        /// </summary>
        event ConnectionStateChangeHandler ConnectionStateChanged;
    }
}
