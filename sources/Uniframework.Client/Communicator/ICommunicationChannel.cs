using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Client
{
    /// <summary>
    /// ͨ��ͨ������ͻ��˵��������˽���ͨ�ŵ�ͨ��
    /// </summary>
    public interface ICommunicationChannel
    {
        /// <summary>
        /// ���÷������˵ķ���
        /// </summary>
        /// <param name="data">�������õ��ֽ���</param>
        /// <returns>���������ص��ֽ���</returns>
        byte[] Invoke(byte[] data);
    }
}
