using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch
{
    /// <summary>
    /// ��ϯ�Զ����������������ʵ�ֺ����Զ��Ŷ�/�����Զ�������񣻶��з��飬������Կɶ��ƣ����������ṩ����ACD �Ŷӷ�ʽ���������㷨��
    /// ��ѭ�㷨�������ʱ�������㷨�����ٻش�ʱ�������㷨���ش�������������㷨�����м��������㷨��
    /// </summary>
    interface IAutoCallDispatcher
    {
        IChannel GetChannel(ChannelType channelType);
    }
}
