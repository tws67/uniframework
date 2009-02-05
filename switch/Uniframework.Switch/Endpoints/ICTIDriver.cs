using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI;

namespace Uniframework.Switch
{
    public interface ICTIDriver
    {
        /// <summary>
        /// ����
        /// </summary>
        string Key { get; }
        /// <summary>
        /// �忨�������İ汾��Ϣ
        /// </summary>
        IVersionInfo VersionInfo { get; }
        /// <summary>
        /// ����/�رհ忨������
        /// </summary>
        bool Active { get; set; }
        /// <summary>
        /// ���ڱ�ʶ��ǰ�忨�������ɷ���
        /// </summary>
        bool CanWork { get; }
        /// <summary>
        /// ��ʱֵ
        /// </summary>
        int Timeout { get; set; }
        /// <summary>
        /// �������������
        /// </summary>
        WorkItem WorkItem { get; }
        /// <summary>
        /// ��ǰ��������ʼ��������ͨ��
        /// </summary>
        ChannelCollection Channels { get; }
        /// <summary>
        /// ͨ����
        /// </summary>
        int ChannelCount { get; }
        /// <summary>
        /// ��ȡָ�����ͻ�״̬��ͨ��
        /// </summary>
        /// <param name="chnlid">ͨ����ʶ</param>
        /// <param name="chnlType">ͨ������</param>
        /// <param name="chnlStatus">ͨ��״̬</param>
        /// <returns>���忨���������ò�ָ������Ӧ��ͨ����ʶ��ֱ�ӷ�����Ӧ��ͨ�����������ָ�����ͼ�״̬��ͨ��</returns>
        IChannel GetChannel(int chnlid, ChannelType chnlType, ChannelStatus chnlStatus);
        /// <summary>
        /// �������
        /// </summary>
        /// <param name="conf">�������</param>
        /// <param name="chnl">����ͨ��</param>
        /// <seealso cref="LeaveConf"/>
        /// <seealso cref="ConfCount"/>
        /// <seealso cref="GetConf"/>
        void JoinConf(int conf, IChannel chnl);
        /// <summary>
        /// �뿪����
        /// </summary>
        /// <param name="conf">�������</param>
        /// <param name="chnl">����ͨ��</param>
        /// <seealso cref="JoinConf"/>
        /// <seealso cref="ConfCount"/>
        /// <seealso cref="GetConf"/>
        void LeaveConf(int conf, IChannel chnl);
        /// <summary>
        /// ����ϵͳĿǰ�����Ļ�������
        /// </summary>
        /// <seealso cref="JoinConf"/>
        /// <seealso cref="LeaveConf"/>
        /// <seealso cref="GetConf"/>
        int ConfCount { get; }
        /// <summary>
        /// ȡ��ָ��������ĳ�Ա�б�
        /// </summary>
        /// <param name="confGroup">�������</param>
        /// <returns>����ָ����ŵĻ��飬��������ڵĻ��򷵻�null</returns>
        /// <remarks>Modified By JackyXU 2007-01-12</remarks>
        /// <seealso cref="JoinConf"/>
        /// <seealso cref="LeaveConf"/>
        /// <seealso cref="ConfCount"/>
        List<int> GetConf(int confGroup);

        event EventHandler<EventArgs<IChannel>> CreatedChannel;
    }
}
