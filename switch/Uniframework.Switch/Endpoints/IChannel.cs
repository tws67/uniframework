using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using log4net;

using Uniframework.Services;

namespace Uniframework.Switch
{
    #region Call Type

    /// <summary>
    /// ��������
    /// </summary>
    public enum CallType
    { 
        /// <summary>
        /// δ֪
        /// </summary>
        UnKnown,
        /// <summary>
        /// ����
        /// </summary>
        In,
        /// <summary>
        /// ����
        /// </summary>
        Out,
        /// <summary>
        /// ת��
        /// </summary>
        Link,
        /// <summary>
        /// �û��Զ���
        /// </summary>
        Custom
    }

    #endregion

    #region Channel Common Event Delegates

    /// <summary>
    /// �����¼���������
    /// </summary>
    public class CallEventArgs : EventArgs
    {
        private string callNumber = string.Empty;
        private CallType callType = CallType.In;

        public CallEventArgs(CallType callType, string callNumber)
        {
            this.callType = callType;
            this.callNumber = callNumber;
        }

        public string CallNumber
        {
            get { return callNumber; }
            set { callNumber = value; }
        }

        public CallType CallType
        {
            get { return callType; }
            set { callType = value; }
        }
    }

    /// <summary>
    /// �����¼�����
    /// </summary>
    public class FaxEventArgs : EventArgs
    {
        private int faxChannelID = -1;
        private FaxMode faxMode = FaxMode.UNKNOWN;
        private int currPage = 0;
        private int totalPage = 0;
        private long processBytes = 0;
        private bool cancelProcess = false;

        public FaxEventArgs(int chnlid, FaxMode faxMode, int currPage, int totalPage, long processBytes)
        {
            this.faxChannelID = chnlid;
            this.faxMode = faxMode;
            this.currPage = currPage;
            this.totalPage = totalPage;
            this.processBytes = processBytes;
        }

        #region FaxEventArgs Members

        public int FaxChannelID
        {
            get { return faxChannelID; }
            set { faxChannelID = value; }
        }

        public FaxMode FaxMode
        {
            get { return faxMode; }
            set { faxMode = value; }
        }

        public int CurrPage
        {
            get { return currPage; }
            set { currPage = value; }
        }

        public int TotalPage
        {
            get { return totalPage; }
            set { totalPage = value; }
        }

        public long ProcessBytes
        {
            get { return processBytes; }
            set { processBytes = value; }
        }

        public bool CancelProcess
        {
            get { return cancelProcess; }
            set { cancelProcess = value; }
        }

        #endregion
    }

    /// <summary>
    /// ͨ�������¼���������
    /// </summary>
    public class LinkingToChannelEventArgs : EventArgs
    {
        private bool allowLink = true;
        private IChannel channel = null;

        public LinkingToChannelEventArgs(IChannel chnl, bool allowLink)
        {
            this.channel = chnl;
            this.allowLink = allowLink;
        }

        public IChannel Channel
        {
            get { return channel; }
            set { channel = value; }
        }

        public bool AllowLink
        {
            get { return allowLink; }
            set { allowLink = value; }
        }
    }

    #endregion

    #region Channel EventHandler Name

    public static class ChannelEventHandlerName
    {
        public readonly static string ChannelStatusChanged = "OnChannelStatusChanged";
        public readonly static string Call = "OnCall";
        public readonly static string Faxing = "OnFaxing";
        public readonly static string GetDTMF = "OnGetDTMF";
        public readonly static string LinkingToChannel = "OnLinkingToChannel";
        public readonly static string LinkedToChannel = "OnLinkedToChannel";
        public readonly static string ResetedChannel = "OnResetedChannel";
        public readonly static string ProcessTimeout = "OnProcessTimeout";
    }

    #endregion

    public interface IChannel
    {
        #region Channel Common function
        /// <summary>
        /// ע��ű��ļ�
        /// </summary>
        /// <param name="eventHandler">�¼��������</param>
        /// <param name="scriptFile">�ű��ļ�</param>
        void RegisgerScript(string eventHandler, string scriptFile);
        /// <summary>
        /// ע���ű��ļ�
        /// </summary>
        /// <param name="eventHandler">�¼��������</param>
        void UnRegisgerScript(string eventHandler);
        /// <summary>
        /// ע���ű��ļ�
        /// </summary>
        /// <param name="eventHandler">�¼��������</param>
        /// <param name="scriptFile">�ű��ļ�</param>
        void UnRegisterScript(string eventHandler, string scriptFile); 

        #endregion

        #region ���ݳ�Ա
        /// <summary>
        /// ͨ����ʶ������Ψһ��ʶÿ��ͨ��
        /// </summary>
        int ChannelID { get; }
        /// <summary>
        /// ͨ������
        /// </summary>
        string ChannelAlias { get; set; }
        /// <summary>
        /// �û�����ɾ���������������ײ���յ�ϵͳ�����ɾ����ʱ����Dtmf������ɾȥ���һ������
        /// </summary>
        char BackspaceKey { get; set; }
        /// <summary>
        /// ͨ������
        /// </summary>
        ChannelType ChannelType { get; }
        /// <summary>
        /// ͨ��״̬
        /// </summary>
        ChannelState ChannelState { get; }
        /// <summary>
        /// ͨ����ǰ�ڲ�״̬���û��������ͨ��CurrentStatus��ȡͨ���ĵ�ǰ״̬����״̬��ϵͳ���и���
        /// </summary>
        ChannelStatus CurrentStatus { get; set; }
        /// <summary>
        /// ͨ���ϴε��ڲ�״̬�����û�������ʵ�ָ��ӵ�ҵ���߼�ʱʹ��
        /// </summary>
        ChannelStatus LastStatus { get; }
        /// <summary>
        /// ͨ�������Ļ�����Դ
        /// </summary>
        ConferenceResource ConfResource { get; }
        /// <summary>
        /// ������Դ����
        /// </summary>
        VoiceResource VoiceResource { get; set; }
        /// <summary>
        /// �û�����֮��ĳ�ʱֵ,��λ���룩
        /// </summary>
        int HitTimeout { get; set; }
        /// <summary>
        /// ͨ�����ʶ����ֵ��ӳͨ����ǰ�Ƿ����
        /// </summary>
        bool Enable { get; set; }
        /// <summary>
        /// Dtmf���壬ϵͳ��Է�������Dtmf�붼����������
        /// </summary>
        string Dtmf { get; set; }
        /// <summary>
        /// �Ƿ���м��Լ�⣬���������Ϊtrue��ϵͳ���м��Լ�����ж϶Է�ժ�����һ���
        /// </summary>
        bool DetectPolarity { get; set; }
        /// <summary>
        /// ���ش����ַ���
        /// </summary>
        string FaxLocalID { get; set; }
        /// <summary>
        /// �뵱ǰͨ�����ӵ�ͨ��
        /// </summary>
        IChannel LinkedChannel { get; }
        /// <summary>
        /// ϵͳ��־���
        /// </summary>
        ILog Logger { get;}
        /// <summary>
        /// ������������
        /// </summary>
        ICTIDriver Driver { get;}

        #endregion

        #region ���弰ժ/�һ�����

        /// <summary>
        /// ���庯�������ڼ�⵱ǰͨ���Ƿ��к���
        /// </summary>
        /// <param name="Times">�����������������˼��Լ��˲�����������Ч</param>
        /// <returns>����к��뷵��true�����򷵻�false</returns>
        /// <seealso cref="DetectPolarity"/>
        bool RingDetect(int Times);
        /// <summary>
        /// ժ�����
        /// </summary>
        /// <returns>���ժ������true�����򷵻�false</returns>
        bool OffHookDetect();
        /// <summary>
        /// ͨ����Ӧ����·ժ��
        /// </summary>
        void OffHook();
        /// <summary>
        /// �һ����
        /// </summary>
        /// <returns>���ͨ����Ӧ����·�һ�����true�����򷵻�false</returns>
        bool HangUpDetect();
        /// <summary>
        /// ͨ����Ӧ����·�һ�
        /// </summary>
        void HangUp();
        /// <summary>
        /// ����ͨ��
        /// </summary>
        /// <seealso cref="HangUpDetect"/>
        /// <seealso cref="HangUp"/>
        void ResetChannel();

        #endregion

        #region ���ż����뺯��

        /// <summary>
        /// ����ͨ�����ź���
        /// </summary>
        /// <param name="prefixNum">Ԥ�����룬��Ӧ���м�������ͨ��ҵ����Ⲧ������Ҫ�ڲ��л�ǰ�Ȳ�"9"�ٲ�ʵ�ʵĵ绰����</param>
        /// <param name="dialNum">�������ĵ绰����</param>
        /// <returns>���ز��ŵĽ������Է�æ��û���˽�������ϸ����Ϣ��ο�<seealso cref="SignalType"/></returns>
        /// <seealso cref="RingDetect"/>
        /// <seealso cref="OffHook"/>
        CallStatus Dial(string prefixNum, string dialNum);
        /// <summary>
        /// ͨ����Ӧ����·�������
        /// </summary>
        /// <returns>�����·�Է��а����򷵻�true�����򷵻�false</returns>
        /// <seealso cref="GetDtmf"/>
        /// <seealso cref="ClearDtmf"/>
        /// <seealso cref="Dial"/>
        bool DtmfHit();
        /// <summary>
        /// ����������Ӧ�ĵײ�API�н���ָ�����ȵİ��������뵽ϵͳ��<seealso cref="Dtmf"/>������
        /// </summary>
        /// <param name="length">��ȡDtmf��ĳ���</param>
        /// <param name="suffix">������ȡ����������ǣ�ϵͳĬ��Ϊ"#"Ҳ����ͨ���˺�������ָ��</param>
        /// <param name="timeout">����֮���ʱ������������ΰ������������ʱ��ϵͳ���ᴥ��<seealso cref="ProcessTimeout"/>�¼�</param>
        /// <returns>�����յ�����Dtmf��</returns>
        /// <seealso cref="DtmfHit"/>
        /// <seealso cref="ClearDtmf"/>
        string GetDtmf(int length, char suffix, int timeout);
        /// <summary>
        /// ���ϵͳ���ײ��Dtmf��������
        /// </summary>
        void ClearDtmf();

        #endregion

        #region ȡ��/���к���

        /// <summary>
        /// ��ȡ���к���
        /// </summary>
        /// <returns>���к���</returns>
        /// <seealso cref="GetCalleeNumber"/>
        string GetCallerNumber();
        /// <summary>
        /// ��ȡ���к���
        /// </summary>
        /// <returns>���к���</returns>
        /// <seealso cref="GetCallerNumber"/>
        string GetCalleeNumber();

        #endregion

        #region ��/¼������

        /// <summary>
        /// ���������ļ���ͨ�������ṩ���ļ��б����ֱ�Ӳ�����Ӧ���ļ����ڿ�ܵĹ滮��֧�ֵ��ļ������ļ����ڴ������ļ��Ĳ���
        /// </summary>
        /// <param name="fileList">�������ļ��б�</param>
        /// <param name="allowBreak">�����������Ƿ������û��������</param>
        /// <returns>���������Ľ��״̬����ϸ�����ο�<seealso cref="PlayResult"/></returns>
        /// <seealso cref="PlayMessage"/>
        /// <seealso cref="PlayNumber"/>
        /// <seealso cref="PlayToFile"/>
        SwitchStatus PlayFile(string fileList, bool allowBreak);
        /// <summary>
        /// ͨ��<seealso cref="ITTSEngine"/>����ʵ�ִ��ı��������Ĳ��ţ�
        /// </summary>
        /// <param name="text">�����ŵ��ı�����</param>
        /// <param name="allowBreak">�����������Ƿ������û��������</param>
        /// <returns>���������Ľ��״̬����ϸ��Ϣ��ο�<seealso cref="PlayResult"/></returns>
        /// <seealso cref="PlayFile"/>
        /// <seealso cref="PlayNumber"/>
        /// <seealso cref="PlayToFile"/>
        SwitchStatus PlayMessage(string text, bool allowBreak);
        /// <summary>
        /// ͨ��<seealso cref="ITTSEngine"/>����ʵ�ִ��ı��������Ĳ��ţ��˺���ר�����ڶ����ֽ���ת���Ͳ�������Ӧ��TTS�����л�Դ˲��ֽ����Ż�����
        /// </summary>
        /// <param name="text">�����ŵ���������</param>
        /// <param name="allowBreak">�����������Ƿ������û��������</param>
        /// <param name="playType">TTS�������ͣ���ϸ��Ϣ��ο�<seealso cref="TTSPlayType"/></param>
        /// <returns>���������Ľ��״̬����ϸ��Ϣ��ο�<seealso cref="PlayResult"/></returns>
        /// <seealso cref="PlayFile"/>
        /// <seealso cref="PlayMessage"/>
        /// <seealso cref="PlayToFile"/>
        SwitchStatus PlayNumber(string text, bool allowBreak, TTSPlayType playType);
        /// <summary>
        /// ͨ��<seealso cref="ITTSEngine"/>����ʵ�ִ��ı����ݵ������ļ���ת����ת��ʱ�����ṩ���ļ���չ��ʵ����Ӧ��ʽ���Զ�֧��
        /// </summary>
        /// <param name="text">��ת�����ı�����</param>
        /// <param name="fileName">ת����������ļ���</param>
        /// <param name="voiceResource">������Դ��������ָ��ת�������֣���ϸ��Ϣ��ο�<seealso cref="VoiceResource"/></param>
        /// <returns>ת�������Ľ��״̬����ϸ��Ϣ��ο�<seealso cref="PlayResult"/></returns>
        /// <seealso cref="PlayFile"/>
        /// <seealso cref="PlayMessage"/>
        /// <seealso cref="PlayNumber"/>
        SwitchStatus PlayToFile(string text, string fileName, VoiceResource voiceResource);
        /// <summary>
        /// ��ͨ������¼����ע�����ͨ����ǰ���ڻ���ģʽ����Ҫ��ɶ����������¼����Ҫ�������ģʽ¼����ʽ������ֻ�Ա�ͨ������¼��
        /// </summary>
        /// <param name="fileName">¼����ŵ��ļ���</param>
        /// <param name="length">¼����������Ϊ��λ</param>
        /// <param name="allowBreak">¼���������Ƿ������û��������</param>
        /// <returns>¼���������״̬����ϸ��Ϣ��ο�<seealso cref="PlayResult"/></returns>
        SwitchStatus RecordFile(string fileName, long length, bool allowBreak);

        #endregion

        #region ͨ����ͨ����

        /// <summary>
        /// ���ӵ�ָ��ͨ���������˺������ڽ��ж��ͨ������ͨ���ܣ����������ñ��������Ӷ��ͨ�����γɻ��鹦��
        /// </summary>
        /// <param name="channelID">ͨ����ʶ</param>
        /// <param name="channelType">ͨ������</param>
        /// <returns>�ɹ������򷵻��������������ţ����򷵻�-1</returns>
        /// <seealso cref="UnLink"/>
        /// <seealso cref="UnLinkAll"/>
        /// <seealso cref="ListenTo"/>
        int LinkTo(int channelID, ChannelType channelType);
        /// <summary>
        /// ɾ����ĳһͨ�������ӣ����ô˺�������ʹ�ñ�ͨ���뺯��ָ����ͨ��ͬʱ�ӻ������˳�������ֻ��ͨ�������ӻ������˳���ο�<seealso cref="UnLink"/>�����ذ汾
        /// </summary>
        /// <param name="channelID">���ӵĶԷ�ͨ��</param>
        /// <seealso cref="LinkTo"/>
        /// <seealso cref="UnLinkAll"/>
        /// <seealso cref="ListenTo"/>
        void UnLink(int channelID);
        /// <summary>
        /// ɾ��ͨ�������ӣ�����Ϊ�ӻ������˳���
        /// </summary>
        /// <seealso cref="LinkTo"/>
        /// <seealso cref="UnLinkAll"/>
        /// <seealso cref="ListenTo"/>
        void UnLink();
        /// <summary>
        /// ɾ��ͨ�����ڻ�������г�Ա���ͷ�ϵͳ��Դ
        /// </summary>
        /// <seealso cref="LinkTo"/>
        /// <seealso cref="UnLink"/>
        /// /// <seealso cref="ListenTo"/>
        void UnLinkAll();
        /// <summary>
        /// ������һͨ����ͨ���������������ͨ���Ѿ��������ģʽ���γɶԻ���ļ���
        /// </summary>
        /// <param name="channelID">ͨ����ʶ</param>
        /// <param name="channelType">ͨ������</param>
        /// <returns>�ɹ��򷵻ر������Ļ�����ţ����򷵻�-1</returns>
        int ListenTo(int channelID, ChannelType channelType);

        #endregion

        #region ���溯��

        bool SendFax(string fileName);
        bool ReceiveFax(string fileName);

        #endregion

        #region �����¼�

        event EventHandler<EventArgs<ChannelStatus>> ChannelStatusChanged;
        
        event EventHandler<EventArgs<string>> GetDTMF;
        
        event EventHandler<CallEventArgs> Call;
        
        event EventHandler<FaxEventArgs> Faxing;
        
        event EventHandler<LinkingToChannelEventArgs> LinkingToChannel;
        
        event EventHandler<EventArgs<IChannel>> LinkedToChannel;
        
        event EventHandler ResetedChannel;
        
        event EventHandler ProcessTimeout;

        #endregion
    }
}
