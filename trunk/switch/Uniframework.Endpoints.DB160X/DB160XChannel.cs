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
    /// 东进DBDKD160X通道
    /// </summary>
    public class DB160XChannel : AbstractChannel
    {
        private readonly static int RingbackRate = 4000; // 回铃频率
        private readonly static int Defaultdelay = 425;  // 相关处理延迟时间（毫秒）
        private readonly static int Onesecond = 1000;
        private object syncObj = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="DB160XChannel"/> class.
        /// </summary>
        /// <param name="driver">板卡适配器</param>
        /// <param name="channelID">通道编号</param>
        public DB160XChannel(ICTIDriver driver, int channelID)
            : base(driver, channelID)
        {
            #region 检测通道类型

            ChannelType[] types = {ChannelType.USER, ChannelType.TRUNK, ChannelType.EMPTY, ChannelType.RECORD, 
                ChannelType.VIRTUAL};
            int chnltype = D160X.CheckChType(channelID);
            ChannelType = chnltype >= 0 && chnltype <= 4 ? types[chnltype] : ChannelType.EMPTY;

            #endregion

            CurrentStatus = ChannelType == ChannelType.EMPTY ? ChannelStatus.RELEASE : ChannelStatus.IDLE;
        }

        #region Assistant function

        /// <summary>
        /// 将接收接收到的DTMF代码转换为相应的字符（使用字符表代替switch语句）
        /// </summary>
        /// <param name="dtmfcode">DTMF代码</param>
        /// <returns>对应的字符</returns>
        private string Dtmfcode2String(int dtmfcode)
        { 
            string[] dtmf = {"~", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "*", "#", "A", "B", "C", "D"};
            return dtmfcode >= 1 && dtmfcode <= 16 ? dtmf[dtmfcode] : "~";
        }

        /// <summary>
        /// 停止放音
        /// </summary>
        private void StopPlaying()
        {
            lock (D160X.SyncObj)
            {
                D160X.StopIndexPlayFile(ChannelID);
                while (!D160X.CheckPlayEnd(ChannelID))
                {
                    Thread.Sleep(Defaultdelay); // 等待通道放音真正结束
                }
            }
        }

        /// <summary>
        /// 检查系统是否有可用的TTSEngine
        /// </summary>
        private ITTSEngine CurrentTTS()
        {
            ITTSEngine tts = Driver.WorkItem.Services.Get<ITTSEngine>();
            if (tts == null)
            {
                Exception ex = new Exception("没有为系统配置TTS引擎，不能进行TTS放音操作。");
                Logger.Error("没有可用的TTS引擎", ex);
                throw ex; 
            }
            return tts;
        }

        /// <summary>
        /// 获取通道加入会议时的状态
        /// </summary>
        /// <param name="chnl">语音通道</param>
        /// <param name="confGroup">会议组</param>
        /// <param name="status">状态</param>
        /// <returns>通道加入会议时的状态</returns>
        private string GetCreateConfStatus(IChannel chnl, int confGroup, int status)
        {
            string[] ConfStatus = new string[] {"成功", "失败，会议组号ConfNo越界", "失败，通道号ChannelNo越界", "失败，没有可用的会议资源" };
            return String.Format("通道 {0} 加入会议组 {1} 时{2}", chnl.ChannelID, confGroup, status >= 0 && status < 4 ? ConfStatus[status] : "失败，未知的错误原因");
        }

        /// <summary>
        /// 创建或加入已经存在的会议
        /// </summary>
        /// <param name="chnl">语音通道</param>
        /// <param name="conf">会议分组</param>
        /// <returns>成功则返回true，否则返回false</returns>
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
        /// 连接到指定通道
        /// </summary>
        /// <param name="channelID">通道标识</param>
        /// <param name="channelType">通道类型</param>
        /// <param name="mode">连接类型，详细信息请参考<seealso cref="ConferenceType"/></param>
        /// <returns>连接成功返回创建的会议组编号，否则返回-1</returns>
        private int LinkToChannel(int channelID, ChannelType channelType, ConferenceType mode)
        {
            if (channelType != ChannelType.TRUNK && channelType != ChannelType.USER)
            {
                ArgumentException ex = new ArgumentException("系统不支持连接到类型为 " + Enum.GetName(typeof(ChannelType), channelType) + " 的通道。");
                Logger.Error("不能进行连接操作", ex);
                throw ex;
            }

            // 以会议模式创建通道之间的连接
            DB160XChannel chnl = Driver.GetChannel(channelID, ChannelType, ChannelStatus.IDLE) as DB160XChannel;
            if (chnl == null) return -1;
            LinkingToChannelEventArgs linkingArgs = new LinkingToChannelEventArgs(chnl, true); // 通道连接前事件参数

            // 加入已经创建的会议
            if (chnl.ConfResource.Confgroup != -1)
            {
                OnLinkingToChannel(this, linkingArgs);
                if (linkingArgs.AllowLink == false)
                {
                    Logger.Info(String.Format("客户应用程序阻止了通道 {0} 到通道 {1} 的连接操作", ChannelID, chnl.ChannelID));
                    return -1;
                }

                Logger.Debug(String.Format("通道 {0} 为加入会议组 {1} ，准备停止当前的放音操作……", ChannelID, chnl.ConfResource.Confgroup));
                if (CurrentStatus == ChannelStatus.PLAY) StopPlaying();
                lock (D160X.SyncObj)
                {
                    if (CurrentStatus == ChannelStatus.RECORD) D160X.StopRecordFile(ChannelID);
                }
                if (JoinConf(this, new ConferenceResource(mode, chnl.ConfResource.Confgroup)))
                {
                    OnLinkedToChannel(this, new EventArgs<IChannel>(chnl)); // 触发通道连接事件
                }
            }
            else
            {
                // 连接空闲的内线通道，这种情况下一般为业务系统转人工服务。此处忽略了对空闲内线通道的监听功能，似乎也没有这方面的功能需求:)
                if (chnl.ChannelType == ChannelType.USER && chnl.CurrentStatus == ChannelStatus.IDLE)
                {
                    Logger.Debug(String.Format("通道 {0} 为准备创建会议组停止当前的放音操作……", ChannelID));
                    StopPlaying();

                    OnLinkingToChannel(this, linkingArgs);
                    if (linkingArgs.AllowLink == false)
                    {
                        Logger.Info(String.Format("客户应用程序阻止了通道 {0} 到通道 {1} 的连接操作", ChannelID, chnl.ChannelID));
                        return -1;
                    }

                    lock (D160X.SyncObj)
                    {
                        Logger.Debug(String.Format("通道 {0} 为连接内线通道 {1} ，正在给内线通道振铃……", ChannelID, chnl.ChannelID));
                        D160X.FeedRealRing(chnl.ChannelID); // 给内线通道馈铃

                        long T = Environment.TickCount;
                        while ((Environment.TickCount - T < Driver.Timeout * Onesecond))
                        {
                            D160X.FeedSigFunc();
                            Thread.Sleep(Defaultdelay); // 休眠以便其它通道可以得到必要的处理器时间

                            Logger.Debug(String.Format("检测内线通道 {0} 是否摘机", chnl.ChannelID));
                            if (chnl.OffHookDetect())
                            {
                                Logger.Debug(String.Format("内线通道 {0} 摘机准备创建会议……", chnl.ChannelID));
                                D160X.StartHangUpDetect(chnl.ChannelID);
                                int confGroup = Driver.ConfCount + 1;

                                // 如果对方提机则创建会议并连通两通道
                                if (JoinConf(this, new ConferenceResource(ConferenceType.JOIN, confGroup)))
                                {
                                    // 将被呼叫方加入会议
                                    if (JoinConf(chnl, new ConferenceResource(ConferenceType.JOIN, confGroup)))
                                    {
                                        D160X.StartPlaySignal(chnl.ChannelID, D160X.SIG_STOP);
                                        OnLinkedToChannel(this, new EventArgs<IChannel>(chnl)); // 触发通道连接事件
                                        return confGroup;
                                    }
                                    else
                                    {
                                        UnLink(); // 加入失败拆除会议
                                        Logger.Info(String.Format("因内线 {0} 加入会议组 {1} 失败而无法建议连接", chnl.ChannelID, confGroup));
                                        return -1;
                                    }
                                }
                            }
                        }

                        // 连接到内线超时
                        if (Environment.TickCount - T > Driver.Timeout * Onesecond)
                        {
                            Logger.Info(String.Format("连接内线 {0} 时因无人接听超时", chnl.ChannelID));
                            OnProcessTimeout(this, null);
                            return -1;
                        }
                    }
                }
                else
                {
                    // 两条通道直接创建会议
                    if (CurrentStatus == ChannelStatus.PLAY) StopPlaying();
                    if (chnl.CurrentStatus == ChannelStatus.PLAY) StopPlaying();

                    OnLinkingToChannel(this, linkingArgs);
                    if (linkingArgs.AllowLink == false)
                    {
                        Logger.Info(String.Format("客户应用程序阻止了通道 {0} 到通道 {1} 的连接操作", ChannelID, chnl.ChannelID));
                        return -1;
                    }

                    lock (D160X.SyncObj)
                    {
                        Logger.Debug(String.Format("通道 {0} 为准备创建会议组停止当前的放音操作……", ChannelID));
                        if (CurrentStatus == ChannelStatus.RECORD) D160X.StopRecordFile(ChannelID);
                        if (chnl.CurrentStatus == ChannelStatus.RECORD) D160X.StopRecordFile(chnl.ChannelID);
                    }

                    int confGroup = Driver.ConfCount + 1;

                    switch (mode)
                    {
                        case ConferenceType.JOIN: // 加入会议
                            if (JoinConf(this, new ConferenceResource(ConferenceType.JOIN, confGroup)))
                            {
                                if (JoinConf(chnl, new ConferenceResource(ConferenceType.JOIN, confGroup)))
                                {
                                    OnLinkedToChannel(this, new EventArgs<IChannel>(chnl)); // 触发通道连接事件
                                    return confGroup;
                                }
                                else
                                {
                                    UnLink();
                                    Logger.Info(String.Format("创建到通道 {0} 的连接因其加入到第 {1} 组会议失败而未能成功", chnl.ChannelID, confGroup));
                                }
                            }
                            break;

                        case ConferenceType.LISTEN: // 监听会议
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
                                    Logger.Info(String.Format("创建到通道 {0} 的连接因其加入到第 {1} 组会议失败而未能成功", chnl.ChannelID, confGroup));
                                }
                            }
                            break;

                        default:
                            Logger.Info("未指定有效的连接模式");
                            return -1;
                    }
                }
            }
            return -1;
        }

        #endregion

        #region 振铃及摘/挂机函数

        // 振铃检测函数
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
                    Logger.Info(String.Format("检测到通道 {0} 有振铃声", ChannelID));
                    return true;
                }
                else
                {
                    if (D160X.RingDetect(ChannelID))
                    {
                        D160X.ResetCallerIDBuffer(ChannelID);
                        D160X.StartTimer(ChannelID, 4);

                        Logger.Info(String.Format("检测到通道 {0} 有振铃声", ChannelID));
                        return true;
                    }
                }
            }
            return false;
        }

        // 摘机检测
        public override bool OffHookDetect()
        {
            // 增加对内线摘机的处理
            lock (D160X.SyncObj)
            {
                switch (ChannelType)
                {
                    // 外线
                    case ChannelType.TRUNK:
                        return D160X.Sig_CheckBusy(ChannelID) == 1;

                    // 内线
                    case ChannelType.USER:
                        bool offHook = D160X.OffHookDetect(ChannelID);
                        if (offHook)
                            D160X.StartHangUpDetect(ChannelID);
                        return offHook;

                    default: return false;
                }
            }
        }

        // 摘机
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
            Logger.Info(String.Format("通道 {0} 摘机", ChannelID));
        }

        // 挂机检测
        public override bool HangUpDetect()
        {
            bool hangUp = false;

            switch (ChannelType)
            { 
                    // 外线通道
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
                    Logger.Debug(string.Format("检测外线通道 {0} 的对方是否已经挂机，结果为：{1}", ChannelID, hangUp == true ? "已经挂机" : "正在通话"));
                    return hangUp;

                    // 内线通道
                case ChannelType.USER :
                    lock (D160X.SyncObj)
                    {
                        hangUp = D160X.HangUpDetect(ChannelID) == 1;
                    }
                    if (hangUp)
                        CurrentStatus = ChannelStatus.HANGUP;
                    Logger.Debug(string.Format("检测内线通道 {0} 的对方是否已经挂机，结果为：{1}", ChannelID, hangUp == true ? "已经挂机" : "正在通话"));
                    return hangUp;

                default : return false;
            }
        }

        // 通道挂机
        public override void HangUp()
        {
            Logger.Info(String.Format("通道 {0} 挂机", ChannelID));
            lock (D160X.SyncObj)
            {
                D160X.HangUp(ChannelID);
            }
            Thread.Sleep(Defaultdelay);
            CurrentStatus = ChannelStatus.HANGUP;
        }

        // 重置通道
        public override void ResetChannel()
        {
            CurrentStatus = ChannelStatus.RESET;
            Logger.Info(String.Format("重置通道 {0}", ChannelID));
            lock (D160X.SyncObj)
            {
                switch (ChannelType)
                {
                    case ChannelType.TRUNK: // 外线
                        // 首先断开通道之间的连接
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

                    case ChannelType.USER: // 内线
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

        #region 拨号及收码函数

        // 拨号
        public override CallStatus Dial(string prefixNum, string dialNum)
        {
            long T = -1;
            CallStatus callStatus = CallStatus.UNALLOCATED;
            string callNumber = prefixNum != null && prefixNum != string.Empty ? prefixNum + "," + dialNum : dialNum;
            Logger.Debug("准备拨号……");

            switch (ChannelType)
            { 
                    // 外线
                case ChannelType.TRUNK :
                    ResetChannel();
                    // 触发呼入/呼出事件处理程序
                    OnCall(this, new CallEventArgs(CallType.Out, callNumber));
                    Logger.Info(String.Format("通道 {0} 对应的线路提机准备拨号，拨出的号码为: {1}", ChannelID, callNumber));
                    OffHook();
                    CurrentStatus = ChannelStatus.DIAL;

                    // 摘机后的延迟处理
                    T = Environment.TickCount;
                    while (Environment.TickCount - T < Defaultdelay) 
                    { 
                        D160X.FeedSigFunc();
                        Thread.Sleep(Defaultdelay);
                    }
                    bool polarity = D160X.CheckPolarity(ChannelID);

                    // 拨号
                    lock (D160X.SyncObj)
                    {
                        D160X.Sig_Init(0); // 初始化信号音检查
                        D160X.Sig_StartDial(ChannelID, Encoding.UTF8.GetBytes(dialNum), Encoding.UTF8.GetBytes(prefixNum), 0);
                    }

                    if (DetectPolarity) 
                    {
                        CurrentStatus = ChannelStatus.WAITING;
                        lock (D160X.SyncObj)
                        {
                            T = Environment.TickCount;
                            // 如果进行极性检查的话，则延时3.5秒钟时间
                            while ((Environment.TickCount - T < Defaultdelay * 10) && polarity == D160X.CheckPolarity(ChannelID))
                            {
                                D160X.FeedSigFunc();
                                D160X.PUSH_PLAY();
                                Thread.Sleep(Defaultdelay);
                            }

                            if (polarity != D160X.CheckPolarity(ChannelID))
                            {
                                Thread.Sleep(Defaultdelay); // 延迟
                                return CallStatus.SUCCESS;
                            }
                            else
                                return CallStatus.SUBSCRIBER_ABSENT;
                        }
                    }
                    else
                    {
                        Logger.Debug(String.Format("开始检查通道 {0} 拨号返回的结果信号……", ChannelID));
                        CurrentStatus = ChannelStatus.WAITING;
                        lock (D160X.SyncObj)
                        {
                            while (callStatus == CallStatus.UNALLOCATED)
                            {
                                D160X.FeedSigFunc();
                                D160X.PUSH_PLAY();
                                Thread.Sleep(Defaultdelay); // 休眠以便其它通道可以得到必要的处理器时间

                                #region 检测通道拨号返回的信号

                                switch (D160X.Sig_CheckDial(ChannelID))
                                {
                                    case D160X.S_NORESULT:
                                        callStatus = CallStatus.UNALLOCATED;
                                        break;

                                    case D160X.S_BUSY:
                                        Logger.Info("对方忙音");
                                        callStatus = CallStatus.USER_BUSY;
                                        break;

                                    case D160X.S_CONNECT:
                                        Logger.Info("对方摘机");
                                        callStatus = CallStatus.SUCCESS;
                                        CurrentStatus = ChannelStatus.CONNECT;
                                        break;

                                    case D160X.S_NOBODY:
                                        Logger.Info("无人接听");
                                        callStatus = CallStatus.SUBSCRIBER_ABSENT;
                                        break;

                                    case D160X.S_NODIALTONE:
                                        Logger.Info("无拨号音");
                                        callStatus = CallStatus.NO_ANSWER;
                                        break;

                                    case D160X.S_NOSIGNAL:
                                        Logger.Info("无信号音");
                                        callStatus = CallStatus.NO_USER_RESPONSE;
                                        break;
                                }

                                #endregion
                            }
                        }
                    }

                    // 延迟以便对方可以清晰的听到系统放音
                    if (callStatus == CallStatus.SUCCESS)
                    {
                        Thread.Sleep(Defaultdelay);
                    }
                    break;
            }
            return callStatus;
        }

        // 检测通道是否有按键
        public override bool DtmfHit()
        {
            lock (D160X.SyncObj)
            {
                return D160X.DtmfHit(ChannelID);
            }
        }

        // 收码
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
            K = timeout == 0 ? HitTimeout : timeout; // 设置两次按键之间的超时值
            CurrentStatus = ChannelStatus.DTMF;
            T = Environment.TickCount;
            N = 0;
            Logger.Debug(String.Format("通道 {0} 准备获取对方按键……", ChannelID));
            while (N < length + suffix.ToString().Length + 1)
            {
                lock (D160X.SyncObj)
                {
                    key = D160X.GetDtmfCode(ChannelID);
                }
                dtmfcode = Dtmfcode2String(key);

                // 超时检测，如果超时则返回已经收到的按键字符串
                if (Environment.TickCount - T > K * 1000) 
                {
                    // 超时处理
                    OnProcessTimeout(this, null);

                    if (suffix.ToString().Length > 0 && dtmfcode == suffix.ToString())
                    {
                        len = Dtmf.Length - suffix.ToString().Length > length ? length : Dtmf.Length - suffix.ToString().Length;
                        Dtmf = Dtmf.Substring(0, len);
                        OnGetDTMF(this, new EventArgs<string>(Dtmf));
                        return Dtmf;
                    }
                }

                // 转换接收到的按键，并将其存入DTMF缓存
                if ((key != -1 || key != 65535) && dtmfcode != "~")
                {
                    Logger.Debug(string.Format("通道 {0} 正在获取第 {1} 个按键值: {2}", ChannelID, N, dtmfcode));
                    Dtmf += dtmfcode;
                    N++;

                    // 若用户铵下了删除键，且Dtmf队列中的最后一个键不是删除键，则删除最后一个键
                    if(char.Parse(dtmfcode) == BackspaceKey && N >= 1 && Dtmf != BackspaceKey.ToString())
                    {
                        Dtmf = Dtmf.Substring(0, N - 2);
                        N -= 2;
                    }
                }
                T = Environment.TickCount; // 获取新按键后重置计时器

                // 如果按键够位数、按下结束键或挂机，则跳出循环
                if ((suffix == char.MinValue && Dtmf.Length == length) || Dtmf.Length == length + suffix.ToString().Length || char.Parse(dtmfcode) == suffix || HangUpDetect())
                {
                    CurrentStatus = HangUpDetect() ? ChannelStatus.HANGUP : CurrentStatus;
                    break;
                }

                Thread.Sleep(Defaultdelay); // 让出CPU时间片
            }

            // 截去字符串中的结束符号
            len = (Dtmf.Length - suffix.ToString().Length > length) || suffix == char.MinValue ? length : Dtmf.Length - suffix.ToString().Length;
            Dtmf = Dtmf.Substring(0, len);
            OnGetDTMF(this, new EventArgs<string>(Dtmf));
            Logger.Info(String.Format("通道 {0} 获取对方按键完成，按键值为: {1}",ChannelID, Dtmf));
            return Dtmf;
        }

        // 清除Dtmf按键缓存
        public override void ClearDtmf()
        {
            lock (D160X.SyncObj)
            {
                D160X.InitDtmfBuf(ChannelID);
            }
            Logger.Info(String.Format("清空通道 {0} 的Dtmf缓存", ChannelID));
            base.ClearDtmf();
        }

        #endregion

        #region 取主/被叫函数

        // 收主叫号码
        public override string GetCallerNumber()
        {
            byte[] callNumber = new byte[32 * sizeof(char)];
            Logger.Debug("准备获取主叫号码……");
            lock (D160X.SyncObj)
            {
                long T = Environment.TickCount;
                int result = D160X.GetCallerIDStr(ChannelID, callNumber);
                CurrentStatus = ChannelStatus.READ;

                // 只有当函数返回3或4时才表示接收完毕
                while (result != 3 && result != 4)
                {
                    if (Environment.TickCount - T > Driver.Timeout * 1000)
                    {
                        OnProcessTimeout(this, null);
                        Logger.Debug("获取主主叫号码超时");
                        return "";
                    }

                    // 获取主叫号码
                    result = D160X.GetCallerIDStr(ChannelID, callNumber);
                    Thread.Sleep(Defaultdelay);
                }
            }

            string Astr = Encoding.UTF8.GetString(callNumber);
            Astr = Astr.Substring(0, Astr.Length - 8); // 去除FSK码
            Logger.Info("获取的主叫号码为：" + Astr);
            return Astr;
        }

        // 收被叫号码
        public override string GetCalleeNumber()
        {
            throw new Exception(Driver.VersionInfo.Name + " 底层驱动未实现获取被叫号码功能。");
        }

        #endregion

        #region 放/录音函数

        // 放音
        public override SwitchStatus PlayFile(string fileList, bool allowBreak)
        {
            // 如果挂机则直接结束当前放音过程
            if (CurrentStatus == ChannelStatus.HANGUP)
                return SwitchStatus.BREAK;
                string[] filelst = fileList.Split(new char[] { ',', ';', ' ' });
            Logger.Info(String.Format("通道 {0} 准备文件放音……", ChannelID));
            Logger.Info(String.Format("通道 {0} 放音文件列表为: {1}", ChannelID, fileList));
            CurrentStatus = ChannelStatus.PLAY;

            Logger.Debug(String.Format("通道 {0} 开始语音文件放音……", ChannelID));
            lock (D160X.SyncObj)
            {
                D160X.RsetIndexPlayFile(ChannelID);
            }
            string file = string.Empty;

            // 添加待播放的语音文件
            for (int i = 0; i < filelst.Length; i++)
            {
                file = CheckFile(filelst[i]);
                Logger.Debug(String.Format("通道 {0} 添加第 {1} 个语音文件: {2}", ChannelID, i, file));
                if (file != string.Empty)
                    lock (D160X.SyncObj)
                    {
                        D160X.AddIndexPlayFile(ChannelID, Encoding.UTF8.GetBytes(file));
                    }
            }

            // 放音
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

                // 检查在放音过程中是否有按键，如果按键则结束当前放音
                bool dtmfHited = false;
                lock(D160X.SyncObj)
                {
                    dtmfHited = D160X.DtmfHit(ChannelID);
                }
                if (allowBreak && dtmfHited)
                {
                    Logger.Info(String.Format("通道 {0} 对应的线路因对方按键中断了语音文件的播放", ChannelID));
                    StopPlaying();
                    return SwitchStatus.BREAK;
                }

                // 检查在放音过程中是否有挂机，如果挂机则结束当前放音
                if (HangUpDetect())
                {
                    Logger.Info(String.Format("通道 {0} 对应的线路因对方挂机中断了语音文件的播放", ChannelID));
                    StopPlaying();
                    CurrentStatus = ChannelStatus.HANGUP;
                    return SwitchStatus.BREAK;
                }
                Playend = D160X.CheckIndexPlayFile(ChannelID);
            }
            StopPlaying();

            Logger.Info(String.Format("通道 {0} 结束文件放音", ChannelID));
            return SwitchStatus.SUCCESS;
        }

        // TTS放音
        public override SwitchStatus PlayMessage(string text, bool allowBreak)
        {
            if (CurrentStatus == ChannelStatus.HANGUP)
                return SwitchStatus.BREAK;

            ITTSEngine tts = CurrentTTS();

            // 由TTS引擎执行放音操作
            CurrentStatus = ChannelStatus.PLAY;
            return tts.PlayMessage(this as IChannel, text, allowBreak, TTSPlayType.MESSAGES);
        }

        // 采用TTS播放数字
        public override SwitchStatus PlayNumber(string text, bool allowBreak, TTSPlayType playType)
        {
            if (CurrentStatus == ChannelStatus.HANGUP)
                return SwitchStatus.BREAK;

            ITTSEngine tts = CurrentTTS();
            
            // 由TTS引擎执行放音操作
            CurrentStatus = ChannelStatus.PLAY;
            return tts.PlayMessage(this as IChannel, text, allowBreak, playType);
        }

        // 采用TTS播放语音到文件
        public override SwitchStatus PlayToFile(string text, string fileName, VoiceResource voiceResource)
        {
            if (CurrentStatus == ChannelStatus.HANGUP)
                return SwitchStatus.BREAK;

            ITTSEngine tts = CurrentTTS();

            tts.PlayToFile(this as IChannel, text, TTSPlayType.MESSAGES, fileName);
            return SwitchStatus.SUCCESS;
        }

        // 录音
        public override SwitchStatus RecordFile(string fileName, long length, bool allowBreak)
        {
            if (CurrentStatus == ChannelStatus.HANGUP)
                return SwitchStatus.BREAK;

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            if (CurrentStatus == ChannelStatus.PLAY)
                StopPlaying();

            Logger.Debug(String.Format("通道 {0} 准备进行录音……", ChannelID));
            lock (D160X.SyncObj)
            {
                D160X.InitDtmfBuf(ChannelID);
            }
            CurrentStatus = ChannelStatus.RECORD;

            Logger.Info(String.Format("通道 {0} 开始录音，录音文件为：{1}",ChannelID, fileName));
            lock (D160X.SyncObj)
            {
                D160X.StartRecordFile(ChannelID, Encoding.UTF8.GetBytes(fileName), length * 8000); // 开始录音
                while (D160X.CheckRecordEnd(ChannelID) == 0)
                {
                    D160X.PUSH_PLAY();
                    Thread.Sleep(Defaultdelay);

                    // 按键检测
                    if (allowBreak && D160X.DtmfHit(ChannelID))
                    {
                        Logger.Info(String.Format("通道 {0} 对应的线路因对方按键中断的录音操作", ChannelID));
                        D160X.StopRecordFile(ChannelID);
                        return SwitchStatus.BREAK;
                    }

                    // 挂机检测
                    if (HangUpDetect())
                    {
                        Logger.Info(String.Format("通道 {0} 对应的线路因对方挂机中断的录音操作", ChannelID));
                        D160X.StopRecordFile(ChannelID);
                        CurrentStatus = ChannelStatus.HANGUP;
                        return SwitchStatus.BREAK;
                    }
                }
                Logger.Info(String.Format("通道 {0} 完成通道录音操作", ChannelID));
                D160X.StopRecordFile(ChannelID);
            }

            return SwitchStatus.SUCCESS;
        }

        #endregion

        #region 通道连通函数

        // 连接到指定类型的通道
        public override int LinkTo(int channelID, ChannelType channelType)
        {
            try
            {
                return LinkToChannel(channelID, channelType, ConferenceType.JOIN);
            }
            catch (Exception ex)
            {
                Logger.Error(String.Format("连接到通道 {0} 失败", channelID), ex);
                UnLink();
                return -1;
            }

            #region 两通道直接连通

            //// 从适配器中获取一个可用的通道
            //IChannel linkedChannel = Driver.GetChannel(channelID, channelType, ChannelStatus.Free);
            //if (linkedChannel == null)
            //    return -1;

            //lock (D160X.SyncObj)
            //{
            //    switch (linkedChannel.ChannelType)
            //    { 
            //            // 连接到外线通道
            //        case ChannelType.TRUNK :
                        
            //            break;

            //            // 连接到内线通道
            //        case ChannelType.USER :
            //            D160X.StopIndexPlayFile(ChannelID);
            //            D160X.FeedRealRing(linkedChannel.ChannelID); // 给内线通道馈断续的铃流
            //            D160X.StartPlaySignal(linkedChannel.ChannelID, D160X.SIG_RINGBACK);

            //            long T = Environment.TickCount;
            //            while ((Environment.TickCount - T < Driver.Timeout * OneSecond))
            //            {
            //                D160X.FeedSigFunc();
            //                // 如果对方（内线）提机则连通两个通道
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
            //                    OnLinkedToChannel(this, new EventArgs<IChannel>(linkedChannel)); // 触发通道连接事件

            //                    logger.Info(String.Format("建立从通道 {0} 到 {1} 的连接", ChannelID, linkedChanelID));
            //                    return linkedChanelID;
            //                }
            //            }

            //            D160X.FeedPower(linkedChannel.ChannelID);
            //            logger.Info(String.Format("建立从通道 {0} 到 {1} 的连接时因超时无人接听失败", ChannelID, linkedChanelID));
            //            break;

            //        default :
            //            break;
            //    }
            //}

            #endregion
        }

        // 撤销通道之间的连接，此函数主要用于通过会议方式创建的连接
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
            Logger.Info(String.Format("通道 {0} 从会议 {1} 中退出", ChannelID, ConfResource.Confgroup));
            Driver.LeaveConf(ConfResource.Confgroup, this);
            ConfResource.Confgroup = -1;
            ConfResource.Confmode = ConferenceType.UNKOWN;
        }

        // 撤销通道之间的连接
        public override void UnLink(int channelID)
        {
            IChannel chnl = Driver.Channels[channelID];
            if (chnl != null)
                chnl.UnLink();
            UnLink(); // 将自己从会议中拆除

            #region 撤除两通道之间的直接连接

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
            //                logger.Info(String.Format("拆除从 {0} 到 {1} 的连接", ChannelID, linkedChanelID));
            //                break;

            //            default :
            //                break;
            //        }
            //    }
            //}

            #endregion
        }

        // 撤销通道所参加会议的其它所有通道之间的连接
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

        // 监听某一通道
        public override int ListenTo(int channelID, ChannelType channelType)
        {
            try
            {
                return LinkToChannel(channelID, channelType, ConferenceType.LISTEN);
            }
            catch (Exception ex)
            {
                Logger.Error(String.Format("监听指定通道 {0} 失败", channelID), ex);
                UnLink();
                return -1;
            }
        }

        #endregion

        #region 收发传真函数

        // 发送传真
        public override bool SendFax(string fileName)
        {
            if (ChannelType != ChannelType.TRUNK)
            {
                string info = "只有外线通道才可以收发传真";
                Logger.Error(info);
                throw new Exception(info);
            }

            StopPlaying();
            bool Faxflag = true; // 发送成功标志
            int faxchnl = -1;
            lock (D160X.SyncObj)
            {
                faxchnl = D160X.DJFax_GetOneFreeFaxChnl();
            }
            if (faxchnl == -1)
            {
                Logger.Info("系统当前没有空闲的传真通道可用");
                return false;
            }

            // 连接传真通道与语音通道成功后开始发送传真
            lock (D160X.SyncObj)
            {
                if (D160X.DJFax_SetLink(faxchnl, ChannelID) == 1)
                {
                    try
                    {
                        Logger.Info(String.Format("通道 {0} 准备发送传真，传真文件为 \"{1}\"", ChannelID, fileName));
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
                                    Logger.Debug(String.Format("通道 {0} 发送传真时因\"读文件失败或命令交互错误\"而失败", ChannelID));
                                    Faxflag = false;
                                    break;
                                }

                                if (sendStatus == 2)
                                    currPage++;
                                FaxEventArgs FaxingArgs = new FaxEventArgs(faxchnl, FaxMode.SEND, currPage, totalPage, D160X.DJFax_GetSendBytes(faxchnl));
                                OnFaxing(this, FaxingArgs); // 触发传真事件
                                if (FaxingArgs.CancelProcess == true)
                                {
                                    Logger.Info(String.Format("用户阻止了通道 {0} 传真文件的继续发送", ChannelID));
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

                        // 停止发送传真
                        D160X.DJFax_StopFax(faxchnl);
                        D160X.DJFax_ClearLink(faxchnl, ChannelID);
                        Thread.Sleep(Defaultdelay); // 延时以便系统真正结束传真
                        CurrentStatus = ChannelStatus.IDLE;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(String.Format("通道 {0} 发送传真时因出现错误而失败", ChannelID), ex);
                        Faxflag = false;
                    }
                }
                else
                    Faxflag = false;
            }

            Logger.Info(String.Format("通道 {0} 发送传真，{1}", ChannelID, Faxflag ? "成功" : "失败"));
            return Faxflag;
        }

        // 接收传真
        public override bool ReceiveFax(string fileName)
        {
            if (ChannelType != ChannelType.TRUNK)
            {
                string info = "只有外线通道才可以收发传真";
                Logger.Error(info);
                throw new Exception(info);
            }

            StopPlaying();
            bool Faxflag = true; // 传真发送标志
            int faxchnl = -1;
            lock (D160X.SyncObj)
            {
                faxchnl = D160X.DJFax_GetOneFreeFaxChnl();
            }
            if (faxchnl == -1)
            {
                Logger.Info("系统当前没有空闲的传真通道可用");
                return false;
            }

            // 连接传真通道与语音通道
            lock (D160X.SyncObj)
            {
                if (D160X.DJFax_SetLink(faxchnl, ChannelID) == 1)
                    try
                    {
                        Logger.Info(String.Format("通道 {0} 准备接收传真，传真文件为 \"{1}\"", ChannelID, fileName));
                        if (D160X.DJFax_RcvFaxFile(faxchnl, Encoding.UTF8.GetBytes(fileName)) == 1)
                        {
                            CurrentStatus = ChannelStatus.FAX;
                            int currPage = 1;
                            int receiveStatus = D160X.DJFax_CheckTransmit(faxchnl);
                            while (receiveStatus != 1)
                            {
                                if (receiveStatus == -2)
                                {
                                    Logger.Debug(String.Format("通道 {0} 接收传真时因\"读文件失败或命令交互错误\"而失败", ChannelID));
                                    Faxflag = false;
                                    break;
                                }

                                if (receiveStatus == 2)
                                    currPage++;
                                // 在接收传真时无法获取总的传真页数，所以总页数为-1
                                FaxEventArgs FaxingArgs = new FaxEventArgs(faxchnl, FaxMode.RECEIVE, currPage, -1, D160X.DJFax_GetRcvBytes(faxchnl));
                                OnFaxing(this, FaxingArgs); // 触发传真事件
                                if (FaxingArgs.CancelProcess == true)
                                {
                                    Logger.Info(String.Format("用户阻止了通道 {0} 传真文件的继续接收", ChannelID));
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

                        // 停止接收传真
                        D160X.DJFax_StopFax(faxchnl);
                        D160X.DJFax_ClearLink(faxchnl, ChannelID);
                        Thread.Sleep(Defaultdelay);
                        CurrentStatus = ChannelStatus.IDLE;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(String.Format("通道 {0} 接收传真时因出现错误而失败", ChannelID), ex);
                        Faxflag = false;
                    }
                else
                    Faxflag = false;
            }

            Logger.Info(String.Format("通道 {0} 接收传真，{1}", ChannelID, Faxflag ? "成功" : "失败"));
            return Faxflag;
        }

        #endregion
    }
}
