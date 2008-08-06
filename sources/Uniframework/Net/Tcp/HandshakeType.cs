// ***************************************************************
// version:  2.0    date: 04/08/2008
//  -------------------------------------------------------------
// 
//  -------------------------------------------------------------
// previous version:  1.4    date: 05/11/2006
//  -------------------------------------------------------------
//  (C) 2006-2008  Midapex All Rights Reserved
// ***************************************************************
using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Net
{
    /// <summary>
    /// ����ö��
    /// </summary>
    public enum HandshakeType
    {
        /// <summary>
        /// �ͻ���Hello 
        /// </summary>
        ClientHello,   
        /// <summary>
        /// ������Hello
        /// </summary>
        ServerHello,  

        /// <summary>
        /// ��������������
        /// </summary>
        ServerKeyExchange, 
        /// <summary>
        /// �����ͻ��˹���
        /// </summary>
        ClientKeyExchange, 

        /// <summary>
        /// �ԳƼ��ܷ���˽��
        /// </summary>
        ClientSymmetricKey,
        /// <summary>
        /// �������յ��ͻ��˶ԳƼ��ܷ���˽�׵Ļظ�
        /// </summary>
        ServerGetSymmetricKey, 

        /// <summary>
        /// �ԳƼ��ܷ�����ʼ������
        /// </summary>
        ClientSymmetricIV, 
        /// <summary>
        /// �������յ��ͻ��˶ԳƼ��ܷ�����ʼ�������Ļظ�
        /// </summary>
        ServerGetSymmetricIV,

        /// <summary>
        /// ��������֤����
        /// </summary>
        ServerFinished, ��
        /// <summary>
        /// �ͻ�����֤����
        /// </summary>
        ClientFinished,����

        /// <summary>
        /// ���ֳɹ�
        /// </summary>
        OK
    }
}
