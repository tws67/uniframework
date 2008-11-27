using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.Practices.CompositeUI;
using Uniframework.Services;

namespace Uniframework.Switch.Endpoints.DB160X
{
    /// <summary>
    /// ����DBDKD160Xͨ��
    /// </summary>
    public class DB160XChannel : AbstractChannel
    {
        private readonly static int RingbackRate = 4000; // ����Ƶ��
        private readonly static int Defaultdelay = 425;  // ��ش����ӳ�ʱ�䣨���룩
        private readonly static int Onesecond = 1000;
        private object syncObj = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="DB160XChannel"/> class.
        /// </summary>
        /// <param name="driver">�忨������</param>
        /// <param name="channelID">ͨ�����</param>
        public DB160XChannel(ICTIDriver driver, int channelID)
            : base(driver, channelID)
        {
            #region ���ͨ������

            ChannelType[] types = {ChannelType.USER, ChannelType.TRUNK, ChannelType.EMPTY, ChannelType.RECORD, 
                ChannelType.VIRTUAL};
            int chnltype = D160X.CheckChType(channelID);
            ChannelType = chnltype >= 0 && chnltype <= 4 ? types[chnltype] : ChannelType.EMPTY;

            #endregion

            CurrentStatus = ChannelType == ChannelType.EMPTY ? ChannelStatus.RELEASE : ChannelStatus.IDLE;
        }

        #region Assistant function

        /// <summary>
        /// �����ս��յ���DTMF����ת��Ϊ��Ӧ���ַ���ʹ���ַ������switch��䣩
        /// </summary>
        /// <param name="dtmfcode">DTMF����</param>
        /// <returns>��Ӧ���ַ�</returns>
        private string Dtmfcode2String(int dtmfcode)
        { 
            string[] dtmf = {"~", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "*", "#", "A", "B", "C", "D"};
            return dtmfcode >= 1 && dtmfcode <= 16 ? dtmf[dtmfcode] : "~";
        }

        /// <summary>
        /// ֹͣ����
        /// </summary>
        private void StopPlaying()
        {
            lock (D160X.SyncObj)
            {
                D160X.StopIndexPlayFile(ChannelID);
                while (!D160X.CheckPlayEnd(ChannelID))
                {
                    Thread.Sleep(Defaultdelay); // �ȴ�ͨ��������������
                }
            }
        }

        /// <summary>
        /// ���ϵͳ�Ƿ��п��õ�TTSEngine
        /// </summary>
        private ITTSEngine CurrentTTS()
        {
            ITTSEngine tts = Driver.WorkItem.Services.Get<ITTSEngine>();
            if (tts == null)
            {
                Exception ex = new Exception("û��Ϊϵͳ����TTS���棬���ܽ���TTS����������");
                Logger.Error("û�п��õ�TTS����", ex);
                throw ex; 
            }
            return tts;
        }

        /// <summary>
        /// ��ȡͨ���������ʱ��״̬
        /// </summary>
        /// <param name="chnl">����ͨ��</param>
        /// <param name="confGroup">������</param>
        /// <param name="status">״̬</param>
        /// <returns>ͨ���������ʱ��״̬</returns>
        private string GetCreateConfStatus(IChannel chnl, int confGroup, int status)
        {
            string[] ConfStatus = new string[] {"�ɹ�", "ʧ�ܣ��������ConfNoԽ��", "ʧ�ܣ�ͨ����ChannelNoԽ��", "ʧ�ܣ�û�п��õĻ�����Դ" };
            return String.Format("ͨ�� {0} ��������� {1} ʱ{2}", chnl.ChannelID, confGroup, status >= 0 && status < 4 ? ConfStatus[status] : "ʧ�ܣ�δ֪�Ĵ���ԭ��");
        }

        /// <summary>
        /// ����������Ѿ����ڵĻ���
        /// </summary>
        /// <param name="chnl">����ͨ��</param>
        /// <param name="conf">�������</param>
        /// <returns>�ɹ��򷵻�true�����򷵻�false</returns>
        private bool JoinConf(IChannel chnl, ConferenceResource conf)
        {
            int addOK = -1;
            lock (D160X.SyncObj)
            {
                switch (conf.Confmode)
                {
                    case ConferenceType.JOIN:
                        addOK = D160X.AddChnl(conf.Confgroup, chnl.ChannelID, chnl.ChannelType == ChannelType.USER ? -0 : 0, 0x00);
                        break;

                    case ConferenceType.LISTEN:
                        addOK = D160X.AddListenChnl(conf.Confgroup, chnl.ChannelID);
                        break;

                    default:
                        return false;
                }
            }

            if (addOK == 0)
            {
                chnl.ConfResource.Confgroup = conf.Confgroup;
                chnl.ConfResource.Confmode = conf.Confmode;
                chnl.CurrentStatus = ChannelStatus.JOIN;
                chnl.Driver.JoinConf(conf.Confgroup, chnl);
            }
            Logger.Info(GetCreateConfStatus(chnl, conf.Confgroup, addOK));
            return addOK == 0;
        }

        /// <summary>
        /// ���ӵ�ָ��ͨ��
        /// </summary>
        /// <param name="channelID">ͨ����ʶ</param>
        /// <param name="channelType">ͨ������</param>
        /// <param name="mode">�������ͣ���ϸ��Ϣ��ο�<seealso cref="ConferenceType"/></param>
        /// <returns>���ӳɹ����ش����Ļ������ţ����򷵻�-1</returns>
        private int LinkToChannel(int channelID, ChannelType channelType, ConferenceType mode)
        {
            if (channelType != ChannelType.TRUNK && channelType != ChannelType.USER)
            {
                ArgumentException ex = new ArgumentException("ϵͳ��֧�����ӵ�����Ϊ " + Enum.GetName(typeof(ChannelType), channelType) + " ��ͨ����");
                Logger.Error("���ܽ������Ӳ���", ex);
                throw ex;
            }

            // �Ի���ģʽ����ͨ��֮�������
            DB160XChannel chnl = Driver.GetChannel(channelID, ChannelType, ChannelStatus.IDLE) as DB160XChannel;
            if (chnl == null) return -1;
            LinkingToChannelEventArgs linkingArgs = new LinkingToChannelEventArgs(chnl, true); // ͨ������ǰ�¼�����

            // �����Ѿ������Ļ���
            if (chnl.ConfResource.Confgroup != -1)
            {
                OnLinkingToChannel(this, linkingArgs);
                if (linkingArgs.AllowLink == false)
                {
                    Logger.Info(String.Format("�ͻ�Ӧ�ó�����ֹ��ͨ�� {0} ��ͨ�� {1} �����Ӳ���", ChannelID, chnl.ChannelID));
                    return -1;
                }

                Logger.Debug(String.Format("ͨ�� {0} Ϊ��������� {1} ��׼��ֹͣ��ǰ�ķ�����������", ChannelID, chnl.ConfResource.Confgroup));
                if (CurrentStatus == ChannelStatus.PLAY) StopPlaying();
                lock (D160X.SyncObj)
                {
                    if (CurrentStatus == ChannelStatus.RECORD) D160X.StopRecordFile(ChannelID);
                }
                if (JoinConf(this, new ConferenceResource(mode, chnl.ConfResource.Confgroup)))
                {
                    OnLinkedToChannel(this, new EventArgs<IChannel>(chnl)); // ����ͨ�������¼�
                }
            }
            else
            {
                // ���ӿ��е�����ͨ�������������һ��Ϊҵ��ϵͳת�˹����񡣴˴������˶Կ�������ͨ���ļ������ܣ��ƺ�Ҳû���ⷽ��Ĺ�������:)
                if (chnl.ChannelType == ChannelType.USER && chnl.CurrentStatus == ChannelStatus.IDLE)
                {
                    Logger.Debug(String.Format("ͨ�� {0} Ϊ׼������������ֹͣ��ǰ�ķ�����������", ChannelID));
                    StopPlaying();

                    OnLinkingToChannel(this, linkingArgs);
                    if (linkingArgs.AllowLink == false)
                    {
                        Logger.Info(String.Format("�ͻ�Ӧ�ó�����ֹ��ͨ�� {0} ��ͨ�� {1} �����Ӳ���", ChannelID, chnl.ChannelID));
                        return -1;
                    }

                    lock (D160X.SyncObj)
                    {
                        Logger.Debug(String.Format("ͨ�� {0} Ϊ��������ͨ�� {1} �����ڸ�����ͨ�����塭��", ChannelID, chnl.ChannelID));
                        D160X.FeedRealRing(chnl.ChannelID); // ������ͨ������

                        long T = Environment.TickCount;
                        while ((Environment.TickCount - T < Driver.Timeout * Onesecond))
                        {
                            D160X.FeedSigFunc();
                            Thread.Sleep(Defaultdelay); // �����Ա�����ͨ�����Եõ���Ҫ�Ĵ�����ʱ��

                            Logger.Debug(String.Format("�������ͨ�� {0} �Ƿ�ժ��", chnl.ChannelID));
                            if (chnl.OffHookDetect())
                            {
                                Logger.Debug(String.Format("����ͨ�� {0} ժ��׼���������顭��", chnl.ChannelID));
                                D160X.StartHangUpDetect(chnl.ChannelID);
                                int confGroup = Driver.ConfCount + 1;

                                // ����Է�����򴴽����鲢��ͨ��ͨ��
                                if (JoinConf(this, new ConferenceResource(ConferenceType.JOIN, confGroup)))
                                {
                                    // �������з��������
                                    if (JoinConf(chnl, new ConferenceResource(ConferenceType.JOIN, confGroup)))
                                    {
                                        D160X.StartPlaySignal(chnl.ChannelID, D160X.SIG_STOP);
                                        OnLinkedToChannel(this, new EventArgs<IChannel>(chnl)); // ����ͨ�������¼�
                                        return confGroup;
                                    }
                                    else
                                    {
                                        UnLink(); // ����ʧ�ܲ������
                                        Logger.Info(String.Format("������ {0} ��������� {1} ʧ�ܶ��޷���������", chnl.ChannelID, confGroup));
                                        return -1;
                                    }
                                }
                            }
                        }

                        // ���ӵ����߳�ʱ
                        if (Environment.TickCount - T > Driver.Timeout * Onesecond)
                        {
                            Logger.Info(String.Format("�������� {0} ʱ�����˽�����ʱ", chnl.ChannelID));
                            OnProcessTimeout(this, null);
                            return -1;
                        }
                    }
                }
                else
                {
                    // ����ͨ��ֱ�Ӵ�������
                    if (CurrentStatus == ChannelStatus.PLAY) StopPlaying();
                    if (chnl.CurrentStatus == ChannelStatus.PLAY) StopPlaying();

                    OnLinkingToChannel(this, linkingArgs);
                    if (linkingArgs.AllowLink == false)
                    {
                        Logger.Info(String.Format("�ͻ�Ӧ�ó�����ֹ��ͨ�� {0} ��ͨ�� {1} �����Ӳ���", ChannelID, chnl.ChannelID));
                        return -1;
                    }

                    lock (D160X.SyncObj)
                    {
                        Logger.Debug(String.Format("ͨ�� {0} Ϊ׼������������ֹͣ��ǰ�ķ�����������", ChannelID));
                        if (CurrentStatus == ChannelStatus.RECORD) D160X.StopRecordFile(ChannelID);
                        if (chnl.CurrentStatus == ChannelStatus.RECORD) D160X.StopRecordFile(chnl.ChannelID);
                    }

                    int confGroup = Driver.ConfCount + 1;

                    switch (mode)
                    {
                        case ConferenceType.JOIN: // �������
                            if (JoinConf(this, new ConferenceResource(ConferenceType.JOIN, confGroup)))
                            {
                                if (JoinConf(chnl, new ConferenceResource(ConferenceType.JOIN, confGroup)))
                                {
                                    OnLinkedToChannel(this, new EventArgs<IChannel>(chnl)); // ����ͨ�������¼�
                                    return confGroup;
                                }
                                else
                                {
                                    UnLink();
                                    Logger.Info(String.Format("������ͨ�� {0} ������������뵽�� {1} �����ʧ�ܶ�δ�ܳɹ�", chnl.ChannelID, confGroup));
                                }
                            }
                            break;

                        case ConferenceType.LISTEN: // ��������
                            if (JoinConf(chnl, new ConferenceResource(ConferenceType.JOIN, confGroup)))
                            {
                                if (JoinConf(this, new ConferenceResource(ConferenceType.LISTEN, confGroup)))
                                {
                                    OnLinkedToChannel(this, new EventArgs<IChannel>(chnl));
                                    return confGroup;
                                }
                                else
                                {
                                    UnLink();
                                    Logger.Info(String.Format("������ͨ�� {0} ������������뵽�� {1} �����ʧ�ܶ�δ�ܳɹ�", chnl.ChannelID, confGroup));
                                }
                            }
                            break;

                        default:
                            Logger.Info("δָ����Ч������ģʽ");
                            return -1;
                    }
                }
            }
            return -1;
        }

        #endregion

        #region ���弰ժ/�һ�����

        /// <summary>
        /// ���庯�������ڼ�⵱ǰͨ���Ƿ��к���
        /// </summary>
        /// <param name="Times">�����������������˼��Լ��˲�����������Ч</param>
        /// <returns>����к��뷵��true�����򷵻�false</returns>
        /// <seealso cref="DetectPolarity"/>
        public override bool RingDetect(int Times)
        {
            int RingTimes = Times < 0 ? 0 : Times;
            long T;
            lock (D160X.SyncObj)
            {
                if (RingTimes > 0 && D160X.RingDetect(ChannelID))
                {
                    D160X.ResetCallerIDBuffer(ChannelID);
                    T = Environment.TickCount;
                    while (Environment.TickCount - T > (RingTimes * RingbackRate))
                    {
                        D160X.FeedSigFunc();
                        Thread.Sleep(Defaultdelay);
                    }
                    D160X.StartTimer(ChannelID, 4);
                    Logger.Info(String.Format("��⵽ͨ�� {0} ��������", ChannelID));
                    return true;
                }
                else
                {
                    if (D160X.RingDetect(ChannelID))
                    {
                        D160X.ResetCallerIDBuffer(ChannelID);
                        D160X.StartTimer(ChannelID, 4);

                        Logger.Info(String.Format("��⵽ͨ�� {0} ��������", ChannelID));
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// ժ�����
        /// </summary>
        /// <returns>���ժ������true�����򷵻�false</returns>
        public override bool OffHookDetect()
        {
            // ���Ӷ�����ժ���Ĵ���
            lock (D160X.SyncObj)
            {
                switch (ChannelType)
                {
                    // ����
                    case ChannelType.TRUNK:
                        return D160X.Sig_CheckBusy(ChannelID) == 1;

                    // ����
                    case ChannelType.USER:
                        bool offHook = D160X.OffHookDetect(ChannelID);
                        if (offHook)
                            D160X.StartHangUpDetect(ChannelID);
                        return offHook;

                    default: return false;
                }
            }
        }

        /// <summary>
        /// ͨ����Ӧ����·ժ��
        /// </summary>
        public override void OffHook()
        {
            long T;
            lock (D160X.SyncObj)
            {
                D160X.OffHook(ChannelID);
                T = Environment.TickCount;
                while (Environment.TickCount - T < Defaultdelay) 
                {
                    Thread.Sleep(Defaultdelay);
                }
                D160X.StartSigCheck(ChannelID);
            }
            CurrentStatus = ChannelStatus.PICKUP;
            Logger.Info(String.Format("ͨ�� {0} ժ��", ChannelID));
        }

        /// <summary>
        /// �һ����
        /// </summary>
        /// <returns>���ͨ����Ӧ����·�һ�����true�����򷵻�false</returns>
        public override bool HangUpDetect()
        {
            bool hangUp = false;

            switch (ChannelType)
            { 
                    // ����ͨ��
                case ChannelType.TRUNK :
                    lock (D160X.SyncObj)
                    {
                        D160X.StartSigCheck(ChannelID);
                        long T = Environment.TickCount;
                        while (Environment.TickCount - T < Defaultdelay) 
                        {
                            Thread.Sleep(Defaultdelay);
                        }
                        hangUp = D160X.Sig_CheckBusy(ChannelID) == 1;
                    }
                    if (hangUp)
                        CurrentStatus = ChannelStatus.HANGUP;
                    Logger.Debug(string.Format("�������ͨ�� {0} �ĶԷ��Ƿ��Ѿ��һ������Ϊ��{1}", ChannelID, hangUp == true ? "�Ѿ��һ�" : "����ͨ��"));
                    return hangUp;

                    // ����ͨ��
                case ChannelType.USER :
                    lock (D160X.SyncObj)
                    {
                        hangUp = D160X.HangUpDetect(ChannelID) == 1;
                    }
                    if (hangUp)
                        CurrentStatus = ChannelStatus.HANGUP;
                    Logger.Debug(string.Format("�������ͨ�� {0} �ĶԷ��Ƿ��Ѿ��һ������Ϊ��{1}", ChannelID, hangUp == true ? "�Ѿ��һ�" : "����ͨ��"));
                    return hangUp;

                default : return false;
            }
        }

        /// <summary>
        /// ͨ����Ӧ����·�һ�
        /// </summary>
        public override void HangUp()
        {
            Logger.Info(String.Format("ͨ�� {0} �һ�", ChannelID));
            lock (D160X.SyncObj)
            {
                D160X.HangUp(ChannelID);
            }
            Thread.Sleep(Defaultdelay);
            CurrentStatus = ChannelStatus.HANGUP;
        }

        /// <summary>
        /// ����ͨ��
        /// </summary>
        /// <seealso cref="HangUpDetect"/>
        /// <seealso cref="HangUp"/>
        public override void ResetChannel()
        {
            CurrentStatus = ChannelStatus.RESET;
            Logger.Info(String.Format("����ͨ�� {0}", ChannelID));
            lock (D160X.SyncObj)
            {
                switch (ChannelType)
                {
                    case ChannelType.TRUNK: // ����
                        // ���ȶϿ�ͨ��֮�������
                        if (linkedChanelID != -1)
                        {
                            IChannel chnl = Driver.Channels[linkedChanelID] as IChannel;
                            if (chnl != null && chnl.ChannelType == ChannelType.USER)
                            {
                                D160X.FeedPower(chnl.ChannelID);
                            }
                            D160X.ClearLink(ChannelID, linkedChanelID);
                            linkedChanelID = -1;
                            (chnl as DB160XChannel).UnLinkToChannel();
                        }

                        //UnLink();
                        HangUp();
                        CurrentStatus = ChannelStatus.IDLE;
                        Dtmf = string.Empty;

                        D160X.StartSigCheck(ChannelID);
                        D160X.Sig_ResetCheck(ChannelID);
                        D160X.ResetCallerIDBuffer(ChannelID);
                        break;

                    case ChannelType.USER: // ����
                        if (linkedChanelID != -1)
                        {
                            D160X.ClearLink(ChannelID, linkedChanelID);
                            linkedChanelID = -1;
                        }

                        //UnLink();
                        D160X.FeedPower(ChannelID);
                        D160X.StartPlaySignal(ChannelID, D160X.SIG_BUSY1);
                        CurrentStatus = ChannelStatus.IDLE;
                        break;

                    default:
                        break;

                }

                D160X.InitDtmfBuf(ChannelID);
            }
        }

        #endregion

        #region ���ż����뺯��

        /// <summary>
        /// ����ͨ�����ź���
        /// </summary>
        /// <param name="prefixNum">Ԥ�����룬��Ӧ���м�������ͨ��ҵ����Ⲧ������Ҫ�ڲ��л�ǰ�Ȳ�"9"�ٲ�ʵ�ʵĵ绰����</param>
        /// <param name="dialNum">�������ĵ绰����</param>
        /// <returns>
        /// ���ز��ŵĽ������Է�æ��û���˽�������ϸ����Ϣ��ο�<seealso cref="SignalType"/>
        /// </returns>
        /// <seealso cref="RingDetect"/>
        /// <seealso cref="OffHook"/>
        public override CallStatus Dial(string prefixNum, string dialNum)
        {
            long T = -1;
            CallStatus callStatus = CallStatus.UNALLOCATED;
            string callNumber = prefixNum != null && prefixNum != string.Empty ? prefixNum + "," + dialNum : dialNum;
            Logger.Debug("׼�����š���");

            switch (ChannelType)
            { 
                    // ����
                case ChannelType.TRUNK :
                    ResetChannel();
                    // ��������/�����¼��������
                    OnCall(this, new CallEventArgs(CallType.Out, callNumber));
                    Logger.Info(String.Format("ͨ�� {0} ��Ӧ����·���׼�����ţ������ĺ���Ϊ: {1}", ChannelID, callNumber));
                    OffHook();
                    CurrentStatus = ChannelStatus.DIAL;

                    // ժ������ӳٴ���
                    T = Environment.TickCount;
                    while (Environment.TickCount - T < Defaultdelay) 
                    { 
                        D160X.FeedSigFunc();
                        Thread.Sleep(Defaultdelay);
                    }
                    bool polarity = D160X.CheckPolarity(ChannelID);

                    // ����
                    lock (D160X.SyncObj)
                    {
                        D160X.Sig_Init(0); // ��ʼ���ź������
                        D160X.Sig_StartDial(ChannelID, Encoding.UTF8.GetBytes(dialNum), Encoding.UTF8.GetBytes(prefixNum), 0);
                    }

                    if (DetectPolarity) 
                    {
                        CurrentStatus = ChannelStatus.WAITING;
                        lock (D160X.SyncObj)
                        {
                            T = Environment.TickCount;
                            // ������м��Լ��Ļ�������ʱ3.5����ʱ��
                            while ((Environment.TickCount - T < Defaultdelay * 10) && polarity == D160X.CheckPolarity(ChannelID))
                            {
                                D160X.FeedSigFunc();
                                D160X.PUSH_PLAY();
                                Thread.Sleep(Defaultdelay);
                            }

                            if (polarity != D160X.CheckPolarity(ChannelID))
                            {
                                Thread.Sleep(Defaultdelay); // �ӳ�
                                return CallStatus.SUCCESS;
                            }
                            else
                                return CallStatus.SUBSCRIBER_ABSENT;
                        }
                    }
                    else
                    {
                        Logger.Debug(String.Format("��ʼ���ͨ�� {0} ���ŷ��صĽ���źš���", ChannelID));
                        CurrentStatus = ChannelStatus.WAITING;
                        lock (D160X.SyncObj)
                        {
                            while (callStatus == CallStatus.UNALLOCATED)
                            {
                                D160X.FeedSigFunc();
                                D160X.PUSH_PLAY();
                                Thread.Sleep(Defaultdelay); // �����Ա�����ͨ�����Եõ���Ҫ�Ĵ�����ʱ��

                                #region ���ͨ�����ŷ��ص��ź�

                                switch (D160X.Sig_CheckDial(ChannelID))
                                {
                                    case D160X.S_NORESULT:
                                        callStatus = CallStatus.UNALLOCATED;
                                        break;

                                    case D160X.S_BUSY:
                                        Logger.Info("�Է�æ��");
                                        callStatus = CallStatus.USER_BUSY;
                                        break;

                                    case D160X.S_CONNECT:
                                        Logger.Info("�Է�ժ��");
                                        callStatus = CallStatus.SUCCESS;
                                        CurrentStatus = ChannelStatus.CONNECT;
                                        break;

                                    case D160X.S_NOBODY:
                                        Logger.Info("���˽���");
                                        callStatus = CallStatus.SUBSCRIBER_ABSENT;
                                        break;

                                    case D160X.S_NODIALTONE:
                                        Logger.Info("�޲�����");
                                        callStatus = CallStatus.NO_ANSWER;
                                        break;

                                    case D160X.S_NOSIGNAL:
                                        Logger.Info("���ź���");
                                        callStatus = CallStatus.NO_USER_RESPONSE;
                                        break;
                                }

                                #endregion
                            }
                        }
                    }

                    // �ӳ��Ա�Է���������������ϵͳ����
                    if (callStatus == CallStatus.SUCCESS)
                    {
                        Thread.Sleep(Defaultdelay);
                    }
                    break;
            }
            return callStatus;
        }

        /// <summary>
        /// ͨ����Ӧ����·�������
        /// </summary>
        /// <returns>�����·�Է��а����򷵻�true�����򷵻�false</returns>
        /// <seealso cref="GetDtmf"/>
        /// <seealso cref="ClearDtmf"/>
        /// <seealso cref="Dial"/>
        public override bool DtmfHit()
        {
            lock (D160X.SyncObj)
            {
                return D160X.DtmfHit(ChannelID);
            }
        }

        /// <summary>
        /// ����������Ӧ�ĵײ�API�н���ָ�����ȵİ��������뵽ϵͳ��<seealso cref="Dtmf"/>������
        /// </summary>
        /// <param name="length">��ȡDtmf��ĳ���</param>
        /// <param name="suffix">������ȡ����������ǣ�ϵͳĬ��Ϊ"#"Ҳ����ͨ���˺�������ָ��</param>
        /// <param name="timeout">����֮���ʱ������������ΰ������������ʱ��ϵͳ���ᴥ��<seealso cref="ProcessTimeout"/>�¼�</param>
        /// <returns>�����յ�����Dtmf��</returns>
        /// <seealso cref="DtmfHit"/>
        /// <seealso cref="ClearDtmf"/>
        public override string GetDtmf(int length, char suffix, int timeout)
        {
            if (CurrentStatus == ChannelStatus.HANGUP)
            {
                return "";
            }

            long T = -1;
            string dtmfcode = string.Empty;
            int N, K, len;
            int key = -1;
            
            Dtmf = "";
            K = timeout == 0 ? HitTimeout : timeout; // �������ΰ���֮��ĳ�ʱֵ
            CurrentStatus = ChannelStatus.DTMF;
            T = Environment.TickCount;
            N = 0;
            Logger.Debug(String.Format("ͨ�� {0} ׼����ȡ�Է���������", ChannelID));
            while (N < length + suffix.ToString().Length + 1)
            {
                lock (D160X.SyncObj)
                {
                    key = D160X.GetDtmfCode(ChannelID);
                }
                dtmfcode = Dtmfcode2String(key);

                // ��ʱ��⣬�����ʱ�򷵻��Ѿ��յ��İ����ַ���
                if (Environment.TickCount - T > K * 1000) 
                {
                    // ��ʱ����
                    OnProcessTimeout(this, null);

                    if (suffix.ToString().Length > 0 && dtmfcode == suffix.ToString())
                    {
                        len = Dtmf.Length - suffix.ToString().Length > length ? length : Dtmf.Length - suffix.ToString().Length;
                        Dtmf = Dtmf.Substring(0, len);
                        OnGetDTMF(this, new EventArgs<string>(Dtmf));
                        return Dtmf;
                    }
                }

                // ת�����յ��İ��������������DTMF����
                if ((key != -1 || key != 65535) && dtmfcode != "~")
                {
                    Logger.Debug(string.Format("ͨ�� {0} ���ڻ�ȡ�� {1} ������ֵ: {2}", ChannelID, N, dtmfcode));
                    Dtmf += dtmfcode;
                    N++;

                    // ���û������ɾ��������Dtmf�����е����һ��������ɾ��������ɾ�����һ����
                    if(char.Parse(dtmfcode) == BackspaceKey && N >= 1 && Dtmf != BackspaceKey.ToString())
                    {
                        Dtmf = Dtmf.Substring(0, N - 2);
                        N -= 2;
                    }
                }
                T = Environment.TickCount; // ��ȡ�°��������ü�ʱ��

                // ���������λ�������½�������һ���������ѭ��
                if ((suffix == char.MinValue && Dtmf.Length == length) || Dtmf.Length == length + suffix.ToString().Length || char.Parse(dtmfcode) == suffix || HangUpDetect())
                {
                    CurrentStatus = HangUpDetect() ? ChannelStatus.HANGUP : CurrentStatus;
                    break;
                }

                Thread.Sleep(Defaultdelay); // �ó�CPUʱ��Ƭ
            }

            // ��ȥ�ַ����еĽ�������
            len = (Dtmf.Length - suffix.ToString().Length > length) || suffix == char.MinValue ? length : Dtmf.Length - suffix.ToString().Length;
            Dtmf = Dtmf.Substring(0, len);
            OnGetDTMF(this, new EventArgs<string>(Dtmf));
            Logger.Info(String.Format("ͨ�� {0} ��ȡ�Է�������ɣ�����ֵΪ: {1}",ChannelID, Dtmf));
            return Dtmf;
        }

        /// <summary>
        /// ���ϵͳ���ײ��Dtmf��������
        /// </summary>
        public override void ClearDtmf()
        {
            lock (D160X.SyncObj)
            {
                D160X.InitDtmfBuf(ChannelID);
            }
            Logger.Info(String.Format("���ͨ�� {0} ��Dtmf����", ChannelID));
            base.ClearDtmf();
        }

        #endregion

        #region ȡ��/���к���

        /// <summary>
        /// ��ȡ���к���
        /// </summary>
        /// <returns>���к���</returns>
        /// <seealso cref="GetCalleeNumber"/>
        public override string GetCallerNumber()
        {
            byte[] callNumber = new byte[32 * sizeof(char)];
            Logger.Debug("׼����ȡ���к��롭��");
            lock (D160X.SyncObj)
            {
                long T = Environment.TickCount;
                int result = D160X.GetCallerIDStr(ChannelID, callNumber);
                CurrentStatus = ChannelStatus.READ;

                // ֻ�е���������3��4ʱ�ű�ʾ�������
                while (result != 3 && result != 4)
                {
                    if (Environment.TickCount - T > Driver.Timeout * 1000)
                    {
                        OnProcessTimeout(this, null);
                        Logger.Debug("��ȡ�����к��볬ʱ");
                        return "";
                    }

                    // ��ȡ���к���
                    result = D160X.GetCallerIDStr(ChannelID, callNumber);
                    Thread.Sleep(Defaultdelay);
                }
            }

            string Astr = Encoding.UTF8.GetString(callNumber);
            Astr = Astr.Substring(0, Astr.Length - 8); // ȥ��FSK��
            Logger.Info("��ȡ�����к���Ϊ��" + Astr);
            return Astr;
        }

        /// <summary>
        /// ��ȡ���к���
        /// </summary>
        /// <returns>���к���</returns>
        /// <seealso cref="GetCallerNumber"/>
        public override string GetCalleeNumber()
        {
            throw new Exception(Driver.VersionInfo.Name + " �ײ�����δʵ�ֻ�ȡ���к��빦�ܡ�");
        }

        #endregion

        #region ��/¼������

        /// <summary>
        /// ���������ļ���ͨ�������ṩ���ļ��б����ֱ�Ӳ�����Ӧ���ļ����ڿ�ܵĹ滮��֧�ֵ��ļ������ļ����ڴ������ļ��Ĳ���
        /// </summary>
        /// <param name="fileList">�������ļ��б�</param>
        /// <param name="allowBreak">�����������Ƿ������û��������</param>
        /// <returns>
        /// ���������Ľ��״̬����ϸ�����ο�<seealso cref="PlayResult"/>
        /// </returns>
        /// <seealso cref="PlayMessage"/>
        /// <seealso cref="PlayNumber"/>
        /// <seealso cref="PlayToFile"/>
        public override SwitchStatus PlayFile(string fileList, bool allowBreak)
        {
            // ����һ���ֱ�ӽ�����ǰ��������
            if (CurrentStatus == ChannelStatus.HANGUP)
                return SwitchStatus.BREAK;
                string[] filelst = fileList.Split(new char[] { ',', ';', ' ' });
            Logger.Info(String.Format("ͨ�� {0} ׼���ļ���������", ChannelID));
            Logger.Info(String.Format("ͨ�� {0} �����ļ��б�Ϊ: {1}", ChannelID, fileList));
            CurrentStatus = ChannelStatus.PLAY;

            Logger.Debug(String.Format("ͨ�� {0} ��ʼ�����ļ���������", ChannelID));
            lock (D160X.SyncObj)
            {
                D160X.RsetIndexPlayFile(ChannelID);
            }
            string file = string.Empty;

            // ��Ӵ����ŵ������ļ�
            for (int i = 0; i < filelst.Length; i++)
            {
                file = CheckFile(filelst[i]);
                Logger.Debug(String.Format("ͨ�� {0} ��ӵ� {1} �������ļ�: {2}", ChannelID, i, file));
                if (file != string.Empty)
                    lock (D160X.SyncObj)
                    {
                        D160X.AddIndexPlayFile(ChannelID, Encoding.UTF8.GetBytes(file));
                    }
            }

            // ����
            lock (D160X.SyncObj)
            {
                D160X.StartIndexPlayFile(ChannelID);
                D160X.InitDtmfBuf(ChannelID);
            }
            bool Playend = false;
            while (!Playend)
            {
                lock (D160X.SyncObj)
                {
                    D160X.PUSH_PLAY();
                }
                Thread.Sleep(Defaultdelay);

                // ����ڷ����������Ƿ��а�������������������ǰ����
                bool dtmfHited = false;
                lock(D160X.SyncObj)
                {
                    dtmfHited = D160X.DtmfHit(ChannelID);
                }
                if (allowBreak && dtmfHited)
                {
                    Logger.Info(String.Format("ͨ�� {0} ��Ӧ����·��Է������ж��������ļ��Ĳ���", ChannelID));
                    StopPlaying();
                    return SwitchStatus.BREAK;
                }

                // ����ڷ����������Ƿ��йһ�������һ��������ǰ����
                if (HangUpDetect())
                {
                    Logger.Info(String.Format("ͨ�� {0} ��Ӧ����·��Է��һ��ж��������ļ��Ĳ���", ChannelID));
                    StopPlaying();
                    CurrentStatus = ChannelStatus.HANGUP;
                    return SwitchStatus.BREAK;
                }
                Playend = D160X.CheckIndexPlayFile(ChannelID);
            }
            StopPlaying();

            Logger.Info(String.Format("ͨ�� {0} �����ļ�����", ChannelID));
            return SwitchStatus.SUCCESS;
        }

        /// <summary>
        /// ͨ��<seealso cref="ITTSEngine"/>����ʵ�ִ��ı��������Ĳ��ţ�
        /// </summary>
        /// <param name="text">�����ŵ��ı�����</param>
        /// <param name="allowBreak">�����������Ƿ������û��������</param>
        /// <returns>
        /// ���������Ľ��״̬����ϸ��Ϣ��ο�<seealso cref="PlayResult"/>
        /// </returns>
        /// <seealso cref="PlayFile"/>
        /// <seealso cref="PlayNumber"/>
        /// <seealso cref="PlayToFile"/>
        public override SwitchStatus PlayMessage(string text, bool allowBreak)
        {
            if (CurrentStatus == ChannelStatus.HANGUP)
                return SwitchStatus.BREAK;

            ITTSEngine tts = CurrentTTS();

            // ��TTS����ִ�з�������
            CurrentStatus = ChannelStatus.PLAY;
            return tts.PlayMessage(this as IChannel, text, allowBreak, TTSPlayType.MESSAGES);
        }

        /// <summary>
        /// ͨ��<seealso cref="ITTSEngine"/>����ʵ�ִ��ı��������Ĳ��ţ��˺���ר�����ڶ����ֽ���ת���Ͳ�������Ӧ��TTS�����л�Դ˲��ֽ����Ż�����
        /// </summary>
        /// <param name="text">�����ŵ���������</param>
        /// <param name="allowBreak">�����������Ƿ������û��������</param>
        /// <param name="playType">TTS�������ͣ���ϸ��Ϣ��ο�<seealso cref="TTSPlayType"/></param>
        /// <returns>
        /// ���������Ľ��״̬����ϸ��Ϣ��ο�<seealso cref="PlayResult"/>
        /// </returns>
        /// <seealso cref="PlayFile"/>
        /// <seealso cref="PlayMessage"/>
        /// <seealso cref="PlayToFile"/>
        public override SwitchStatus PlayNumber(string text, bool allowBreak, TTSPlayType playType)
        {
            if (CurrentStatus == ChannelStatus.HANGUP)
                return SwitchStatus.BREAK;

            ITTSEngine tts = CurrentTTS();
            
            // ��TTS����ִ�з�������
            CurrentStatus = ChannelStatus.PLAY;
            return tts.PlayMessage(this as IChannel, text, allowBreak, playType);
        }

        /// <summary>
        /// ͨ��<seealso cref="ITTSEngine"/>����ʵ�ִ��ı����ݵ������ļ���ת����ת��ʱ�����ṩ���ļ���չ��ʵ����Ӧ��ʽ���Զ�֧��
        /// </summary>
        /// <param name="text">��ת�����ı�����</param>
        /// <param name="fileName">ת����������ļ���</param>
        /// <param name="voiceResource">������Դ��������ָ��ת�������֣���ϸ��Ϣ��ο�<seealso cref="VoiceResource"/></param>
        /// <returns>
        /// ת�������Ľ��״̬����ϸ��Ϣ��ο�<seealso cref="PlayResult"/>
        /// </returns>
        /// <seealso cref="PlayFile"/>
        /// <seealso cref="PlayMessage"/>
        /// <seealso cref="PlayNumber"/>
        public override SwitchStatus PlayToFile(string text, string fileName, VoiceResource voiceResource)
        {
            if (CurrentStatus == ChannelStatus.HANGUP)
                return SwitchStatus.BREAK;

            ITTSEngine tts = CurrentTTS();

            tts.PlayToFile(this as IChannel, text, TTSPlayType.MESSAGES, fileName);
            return SwitchStatus.SUCCESS;
        }

        /// <summary>
        /// ��ͨ������¼����ע�����ͨ����ǰ���ڻ���ģʽ����Ҫ��ɶ����������¼����Ҫ�������ģʽ¼����ʽ������ֻ�Ա�ͨ������¼��
        /// </summary>
        /// <param name="fileName">¼����ŵ��ļ���</param>
        /// <param name="length">¼����������Ϊ��λ</param>
        /// <param name="allowBreak">¼���������Ƿ������û��������</param>
        /// <returns>
        /// ¼���������״̬����ϸ��Ϣ��ο�<seealso cref="PlayResult"/>
        /// </returns>
        public override SwitchStatus RecordFile(string fileName, long length, bool allowBreak)
        {
            if (CurrentStatus == ChannelStatus.HANGUP)
                return SwitchStatus.BREAK;

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            if (CurrentStatus == ChannelStatus.PLAY)
                StopPlaying();

            Logger.Debug(String.Format("ͨ�� {0} ׼������¼������", ChannelID));
            lock (D160X.SyncObj)
            {
                D160X.InitDtmfBuf(ChannelID);
            }
            CurrentStatus = ChannelStatus.RECORD;

            Logger.Info(String.Format("ͨ�� {0} ��ʼ¼����¼���ļ�Ϊ��{1}",ChannelID, fileName));
            lock (D160X.SyncObj)
            {
                D160X.StartRecordFile(ChannelID, Encoding.UTF8.GetBytes(fileName), length * 8000); // ��ʼ¼��
                while (D160X.CheckRecordEnd(ChannelID) == 0)
                {
                    D160X.PUSH_PLAY();
                    Thread.Sleep(Defaultdelay);

                    // �������
                    if (allowBreak && D160X.DtmfHit(ChannelID))
                    {
                        Logger.Info(String.Format("ͨ�� {0} ��Ӧ����·��Է������жϵ�¼������", ChannelID));
                        D160X.StopRecordFile(ChannelID);
                        return SwitchStatus.BREAK;
                    }

                    // �һ����
                    if (HangUpDetect())
                    {
                        Logger.Info(String.Format("ͨ�� {0} ��Ӧ����·��Է��һ��жϵ�¼������", ChannelID));
                        D160X.StopRecordFile(ChannelID);
                        CurrentStatus = ChannelStatus.HANGUP;
                        return SwitchStatus.BREAK;
                    }
                }
                Logger.Info(String.Format("ͨ�� {0} ���ͨ��¼������", ChannelID));
                D160X.StopRecordFile(ChannelID);
            }

            return SwitchStatus.SUCCESS;
        }

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
        public override int LinkTo(int channelID, ChannelType channelType)
        {
            try
            {
                return LinkToChannel(channelID, channelType, ConferenceType.JOIN);
            }
            catch (Exception ex)
            {
                Logger.Error(String.Format("���ӵ�ͨ�� {0} ʧ��", channelID), ex);
                UnLink();
                return -1;
            }

            #region ��ͨ��ֱ����ͨ

            //// ���������л�ȡһ�����õ�ͨ��
            //IChannel linkedChannel = Driver.GetChannel(channelID, channelType, ChannelStatus.Free);
            //if (linkedChannel == null)
            //    return -1;

            //lock (D160X.SyncObj)
            //{
            //    switch (linkedChannel.ChannelType)
            //    { 
            //            // ���ӵ�����ͨ��
            //        case ChannelType.TRUNK :
                        
            //            break;

            //            // ���ӵ�����ͨ��
            //        case ChannelType.USER :
            //            D160X.StopIndexPlayFile(ChannelID);
            //            D160X.FeedRealRing(linkedChannel.ChannelID); // ������ͨ��������������
            //            D160X.StartPlaySignal(linkedChannel.ChannelID, D160X.SIG_RINGBACK);

            //            long T = Environment.TickCount;
            //            while ((Environment.TickCount - T < Driver.Timeout * OneSecond))
            //            {
            //                D160X.FeedSigFunc();
            //                // ����Է������ߣ��������ͨ����ͨ��
            //                if (D160X.RingDetect(linkedChannel.ChannelID))
            //                {
            //                    linkedChanelID = linkedChannel.ChannelID;
            //                    CurrentStatus = ChannelStatus.Link;
            //                    (linkedChannel as DB160XChannel).LinkToChannel(ChannelID);
            //                    D160X.InitDtmfBuf(ChannelID);
            //                    D160X.InitDtmfBuf(linkedChannel.ChannelID);
            //                    D160X.FeedPower(linkedChannel.ChannelID);
            //                    D160X.StartHangUpDetect(linkedChannel.ChannelID);

            //                    D160X.StartPlaySignal(ChannelID, D160X.SIG_STOP);
            //                    D160X.StartPlaySignal(linkedChannel.ChannelID, D160X.SIG_STOP);
            //                    D160X.StopIndexPlayFile(ChannelID);
            //                    D160X.StopIndexPlayFile(linkedChannel.ChannelID);

            //                    D160X.SetLink(ChannelID, linkedChannel.ChannelID);
            //                    OnLinkedToChannel(this, new EventArgs<IChannel>(linkedChannel)); // ����ͨ�������¼�

            //                    logger.Info(String.Format("������ͨ�� {0} �� {1} ������", ChannelID, linkedChanelID));
            //                    return linkedChanelID;
            //                }
            //            }

            //            D160X.FeedPower(linkedChannel.ChannelID);
            //            logger.Info(String.Format("������ͨ�� {0} �� {1} ������ʱ��ʱ���˽���ʧ��", ChannelID, linkedChanelID));
            //            break;

            //        default :
            //            break;
            //    }
            //}

            #endregion
        }

        /// <summary>
        /// ɾ��ͨ�������ӣ�����Ϊ�ӻ������˳���
        /// </summary>
        /// <seealso cref="LinkTo"/>
        /// <seealso cref="UnLinkAll"/>
        /// <seealso cref="ListenTo"/>
        public override void UnLink()
        {
            if (ConfResource.Confgroup == -1)
                return;

            lock (D160X.SyncObj)
            {
                switch (ConfResource.Confmode)
                {
                    case ConferenceType.JOIN:
                        D160X.SubChnl(ConfResource.Confgroup, ChannelID);
                        break;

                    case ConferenceType.LISTEN:
                        D160X.SubListenChnl(ConfResource.Confgroup, ChannelID);
                        break;

                    default:
                        break;
                }
            }
            Logger.Info(String.Format("ͨ�� {0} �ӻ��� {1} ���˳�", ChannelID, ConfResource.Confgroup));
            Driver.LeaveConf(ConfResource.Confgroup, this);
            ConfResource.Confgroup = -1;
            ConfResource.Confmode = ConferenceType.UNKOWN;
        }

        /// <summary>
        /// ɾ����ĳһͨ�������ӣ����ô˺�������ʹ�ñ�ͨ���뺯��ָ����ͨ��ͬʱ�ӻ������˳�������ֻ��ͨ�������ӻ������˳���ο�<seealso cref="UnLink"/>�����ذ汾
        /// </summary>
        /// <param name="channelID">���ӵĶԷ�ͨ��</param>
        /// <seealso cref="LinkTo"/>
        /// <seealso cref="UnLinkAll"/>
        /// <seealso cref="ListenTo"/>
        public override void UnLink(int channelID)
        {
            IChannel chnl = Driver.Channels[channelID];
            if (chnl != null)
                chnl.UnLink();
            UnLink(); // ���Լ��ӻ����в��

            #region ������ͨ��֮���ֱ������

            //if (linkedChanelID == channelID && linkedChanelID != -1)
            //{
            //    IChannel linkedChannel = Driver.Channels[linkedChanelID];
            //    if (linkedChannel != null)
            //    {
            //        switch (linkedChannel.ChannelType)
            //        { 
            //            case ChannelType.TRUNK :
            //                D160X.ClearLink(ChannelID, linkedChanelID);
            //                (linkedChannel as DB160XChannel).UnLinkToChannel();
            //                break;

            //            case ChannelType.USER :
            //                D160X.StartPlaySignal(ChannelID, D160X.SIG_STOP);
            //                D160X.StartPlaySignal(linkedChanelID, D160X.SIG_STOP);
            //                D160X.ClearLink(ChannelID, linkedChanelID);
            //                linkedChanelID = -1;
            //                (linkedChannel as DB160XChannel).UnLinkToChannel();
            //                logger.Info(String.Format("����� {0} �� {1} ������", ChannelID, linkedChanelID));
            //                break;

            //            default :
            //                break;
            //        }
            //    }
            //}

            #endregion
        }

        /// <summary>
        /// ɾ��ͨ�����ڻ�������г�Ա���ͷ�ϵͳ��Դ
        /// </summary>
        /// <seealso cref="LinkTo"/>
        /// <seealso cref="UnLink"/>
        /// ///
        /// <seealso cref="ListenTo"/>
        public override void UnLinkAll()
        {
            if(ConfResource.Confgroup == -1)
                return;

            List<int> confs = Driver.GetConf(ConfResource.Confgroup);
            if (confs == null) return;
            foreach (int chnlid in confs)
            {
                if (Driver.Channels[chnlid] != null)
                    Driver.Channels[chnlid].UnLink();
            }
        }

        /// <summary>
        /// ������һͨ����ͨ���������������ͨ���Ѿ��������ģʽ���γɶԻ���ļ���
        /// </summary>
        /// <param name="channelID">ͨ����ʶ</param>
        /// <param name="channelType">ͨ������</param>
        /// <returns>�ɹ��򷵻ر������Ļ�����ţ����򷵻�-1</returns>
        public override int ListenTo(int channelID, ChannelType channelType)
        {
            try
            {
                return LinkToChannel(channelID, channelType, ConferenceType.LISTEN);
            }
            catch (Exception ex)
            {
                Logger.Error(String.Format("����ָ��ͨ�� {0} ʧ��", channelID), ex);
                UnLink();
                return -1;
            }
        }

        #endregion

        #region �շ����溯��

        // ���ʹ���
        public override bool SendFax(string fileName)
        {
            if (ChannelType != ChannelType.TRUNK)
            {
                string info = "ֻ������ͨ���ſ����շ�����";
                Logger.Error(info);
                throw new Exception(info);
            }

            StopPlaying();
            bool Faxflag = true; // ���ͳɹ���־
            int faxchnl = -1;
            lock (D160X.SyncObj)
            {
                faxchnl = D160X.DJFax_GetOneFreeFaxChnl();
            }
            if (faxchnl == -1)
            {
                Logger.Info("ϵͳ��ǰû�п��еĴ���ͨ������");
                return false;
            }

            // ���Ӵ���ͨ��������ͨ���ɹ���ʼ���ʹ���
            lock (D160X.SyncObj)
            {
                if (D160X.DJFax_SetLink(faxchnl, ChannelID) == 1)
                {
                    try
                    {
                        Logger.Info(String.Format("ͨ�� {0} ׼�����ʹ��棬�����ļ�Ϊ \"{1}\"", ChannelID, fileName));
                        D160X.DJFax_SetLocalID(faxchnl, Encoding.UTF8.GetBytes(FaxLocalID));
                        int totalPage = D160X.DJFax_SendFaxFile(faxchnl, Encoding.UTF8.GetBytes(fileName));
                        if (totalPage > 0)
                        {
                            CurrentStatus = ChannelStatus.FAX;
                            int currPage = 1;
                            int sendStatus = D160X.DJFax_CheckTransmit(faxchnl);
                            while (sendStatus != 1)
                            {
                                if (sendStatus == -2)
                                {
                                    Logger.Debug(String.Format("ͨ�� {0} ���ʹ���ʱ��\"���ļ�ʧ�ܻ����������\"��ʧ��", ChannelID));
                                    Faxflag = false;
                                    break;
                                }

                                if (sendStatus == 2)
                                    currPage++;
                                FaxEventArgs FaxingArgs = new FaxEventArgs(faxchnl, FaxMode.SEND, currPage, totalPage, D160X.DJFax_GetSendBytes(faxchnl));
                                OnFaxing(this, FaxingArgs); // ���������¼�
                                if (FaxingArgs.CancelProcess == true)
                                {
                                    Logger.Info(String.Format("�û���ֹ��ͨ�� {0} �����ļ��ļ�������", ChannelID));
                                    Faxflag = false;
                                    break;
                                }
                                Thread.Sleep(Defaultdelay);
                                sendStatus = D160X.DJFax_CheckTransmit(faxchnl);
                            }

                            if (sendStatus == 1)
                                OnFaxing(this, new FaxEventArgs(faxchnl, FaxMode.SEND, totalPage, totalPage, D160X.DJFax_GetSendBytes(faxchnl)));
                        }
                        else
                            Faxflag = false;

                        // ֹͣ���ʹ���
                        D160X.DJFax_StopFax(faxchnl);
                        D160X.DJFax_ClearLink(faxchnl, ChannelID);
                        Thread.Sleep(Defaultdelay); // ��ʱ�Ա�ϵͳ������������
                        CurrentStatus = ChannelStatus.IDLE;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(String.Format("ͨ�� {0} ���ʹ���ʱ����ִ����ʧ��", ChannelID), ex);
                        Faxflag = false;
                    }
                }
                else
                    Faxflag = false;
            }

            Logger.Info(String.Format("ͨ�� {0} ���ʹ��棬{1}", ChannelID, Faxflag ? "�ɹ�" : "ʧ��"));
            return Faxflag;
        }

        // ���մ���
        public override bool ReceiveFax(string fileName)
        {
            if (ChannelType != ChannelType.TRUNK)
            {
                string info = "ֻ������ͨ���ſ����շ�����";
                Logger.Error(info);
                throw new Exception(info);
            }

            StopPlaying();
            bool Faxflag = true; // ���淢�ͱ�־
            int faxchnl = -1;
            lock (D160X.SyncObj)
            {
                faxchnl = D160X.DJFax_GetOneFreeFaxChnl();
            }
            if (faxchnl == -1)
            {
                Logger.Info("ϵͳ��ǰû�п��еĴ���ͨ������");
                return false;
            }

            // ���Ӵ���ͨ��������ͨ��
            lock (D160X.SyncObj)
            {
                if (D160X.DJFax_SetLink(faxchnl, ChannelID) == 1)
                    try
                    {
                        Logger.Info(String.Format("ͨ�� {0} ׼�����մ��棬�����ļ�Ϊ \"{1}\"", ChannelID, fileName));
                        if (D160X.DJFax_RcvFaxFile(faxchnl, Encoding.UTF8.GetBytes(fileName)) == 1)
                        {
                            CurrentStatus = ChannelStatus.FAX;
                            int currPage = 1;
                            int receiveStatus = D160X.DJFax_CheckTransmit(faxchnl);
                            while (receiveStatus != 1)
                            {
                                if (receiveStatus == -2)
                                {
                                    Logger.Debug(String.Format("ͨ�� {0} ���մ���ʱ��\"���ļ�ʧ�ܻ����������\"��ʧ��", ChannelID));
                                    Faxflag = false;
                                    break;
                                }

                                if (receiveStatus == 2)
                                    currPage++;
                                // �ڽ��մ���ʱ�޷���ȡ�ܵĴ���ҳ����������ҳ��Ϊ-1
                                FaxEventArgs FaxingArgs = new FaxEventArgs(faxchnl, FaxMode.RECEIVE, currPage, -1, D160X.DJFax_GetRcvBytes(faxchnl));
                                OnFaxing(this, FaxingArgs); // ���������¼�
                                if (FaxingArgs.CancelProcess == true)
                                {
                                    Logger.Info(String.Format("�û���ֹ��ͨ�� {0} �����ļ��ļ�������", ChannelID));
                                    Faxflag = false;
                                    break;
                                }
                                Thread.Sleep(Defaultdelay);
                                receiveStatus = D160X.DJFax_CheckTransmit(faxchnl);
                            }
                            if (receiveStatus == 1)
                                OnFaxing(this, new FaxEventArgs(faxchnl, FaxMode.RECEIVE, currPage, -1, D160X.DJFax_GetRcvBytes(faxchnl)));
                        }
                        else
                            Faxflag = false;

                        // ֹͣ���մ���
                        D160X.DJFax_StopFax(faxchnl);
                        D160X.DJFax_ClearLink(faxchnl, ChannelID);
                        Thread.Sleep(Defaultdelay);
                        CurrentStatus = ChannelStatus.IDLE;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(String.Format("ͨ�� {0} ���մ���ʱ����ִ����ʧ��", ChannelID), ex);
                        Faxflag = false;
                    }
                else
                    Faxflag = false;
            }

            Logger.Info(String.Format("ͨ�� {0} ���մ��棬{1}", ChannelID, Faxflag ? "�ɹ�" : "ʧ��"));
            return Faxflag;
        }

        #endregion
    }
}
