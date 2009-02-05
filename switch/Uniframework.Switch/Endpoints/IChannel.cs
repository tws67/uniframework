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
    /// 呼叫类型
    /// </summary>
    public enum CallType
    { 
        /// <summary>
        /// 未知
        /// </summary>
        UnKnown,
        /// <summary>
        /// 呼入
        /// </summary>
        In,
        /// <summary>
        /// 呼出
        /// </summary>
        Out,
        /// <summary>
        /// 转接
        /// </summary>
        Link,
        /// <summary>
        /// 用户自定义
        /// </summary>
        Custom
    }

    #endregion

    #region Channel Common Event Delegates

    /// <summary>
    /// 呼叫事件参数类型
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
    /// 传真事件参数
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
    /// 通道连接事件参数类型
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
        /// 注册脚本文件
        /// </summary>
        /// <param name="eventHandler">事件处理程序</param>
        /// <param name="scriptFile">脚本文件</param>
        void RegisgerScript(string eventHandler, string scriptFile);
        /// <summary>
        /// 注销脚本文件
        /// </summary>
        /// <param name="eventHandler">事件处理程序</param>
        void UnRegisgerScript(string eventHandler);
        /// <summary>
        /// 注销脚本文件
        /// </summary>
        /// <param name="eventHandler">事件处理程序</param>
        /// <param name="scriptFile">脚本文件</param>
        void UnRegisterScript(string eventHandler, string scriptFile); 

        #endregion

        #region 数据成员
        /// <summary>
        /// 通道标识，用于唯一标识每条通道
        /// </summary>
        int ChannelID { get; }
        /// <summary>
        /// 通道别名
        /// </summary>
        string ChannelAlias { get; set; }
        /// <summary>
        /// 用户按键删除键，当适配器底层接收到系统定义的删除键时将从Dtmf缓冲中删去最后一个按键
        /// </summary>
        char BackspaceKey { get; set; }
        /// <summary>
        /// 通道类型
        /// </summary>
        ChannelType ChannelType { get; }
        /// <summary>
        /// 通道状态
        /// </summary>
        ChannelState ChannelState { get; }
        /// <summary>
        /// 通道当前内部状态，用户程序可以通过CurrentStatus获取通道的当前状态，此状态由系统进行更改
        /// </summary>
        ChannelStatus CurrentStatus { get; set; }
        /// <summary>
        /// 通道上次的内部状态，在用户程序中实现复杂的业务逻辑时使用
        /// </summary>
        ChannelStatus LastStatus { get; }
        /// <summary>
        /// 通道所属的会议资源
        /// </summary>
        ConferenceResource ConfResource { get; }
        /// <summary>
        /// 语音资源类型
        /// </summary>
        VoiceResource VoiceResource { get; set; }
        /// <summary>
        /// 用户按键之间的超时值,单位（秒）
        /// </summary>
        int HitTimeout { get; set; }
        /// <summary>
        /// 通道活动标识，此值反映通道当前是否可用
        /// </summary>
        bool Enable { get; set; }
        /// <summary>
        /// Dtmf缓冲，系统与对方交互的Dtmf码都将缓存至此
        /// </summary>
        string Dtmf { get; set; }
        /// <summary>
        /// 是否进行极性检测，如果此属性为true则系统进行极性检测来判断对方摘机、挂机等
        /// </summary>
        bool DetectPolarity { get; set; }
        /// <summary>
        /// 本地传真字符串
        /// </summary>
        string FaxLocalID { get; set; }
        /// <summary>
        /// 与当前通道连接的通道
        /// </summary>
        IChannel LinkedChannel { get; }
        /// <summary>
        /// 系统日志组件
        /// </summary>
        ILog Logger { get;}
        /// <summary>
        /// 驱动器适配器
        /// </summary>
        ICTIDriver Driver { get;}

        #endregion

        #region 振铃及摘/挂机函数

        /// <summary>
        /// 振铃函数，用于检测当前通道是否有呼入
        /// </summary>
        /// <param name="Times">振铃次数，如果启用了极性检测此参数将不再有效</param>
        /// <returns>如果有呼入返回true，否则返回false</returns>
        /// <seealso cref="DetectPolarity"/>
        bool RingDetect(int Times);
        /// <summary>
        /// 摘机检测
        /// </summary>
        /// <returns>如果摘机返回true，否则返回false</returns>
        bool OffHookDetect();
        /// <summary>
        /// 通道对应的线路摘机
        /// </summary>
        void OffHook();
        /// <summary>
        /// 挂机检测
        /// </summary>
        /// <returns>如果通道对应的线路挂机返回true，否则返回false</returns>
        bool HangUpDetect();
        /// <summary>
        /// 通道对应的线路挂机
        /// </summary>
        void HangUp();
        /// <summary>
        /// 重置通道
        /// </summary>
        /// <seealso cref="HangUpDetect"/>
        /// <seealso cref="HangUp"/>
        void ResetChannel();

        #endregion

        #region 拨号及收码函数

        /// <summary>
        /// 外线通道拨号函数
        /// </summary>
        /// <param name="prefixNum">预拨号码，对应于中集、汇线通等业务的外拨，如需要在拨市话前先拨"9"再拨实际的电话号码</param>
        /// <param name="dialNum">待拨出的电话号码</param>
        /// <returns>返回拨号的结果，如对方忙、没有人接听更详细的信息请参考<seealso cref="SignalType"/></returns>
        /// <seealso cref="RingDetect"/>
        /// <seealso cref="OffHook"/>
        CallStatus Dial(string prefixNum, string dialNum);
        /// <summary>
        /// 通道对应的线路按键检测
        /// </summary>
        /// <returns>如果线路对方有按键则返回true，否则返回false</returns>
        /// <seealso cref="GetDtmf"/>
        /// <seealso cref="ClearDtmf"/>
        /// <seealso cref="Dial"/>
        bool DtmfHit();
        /// <summary>
        /// 从适配器对应的底层API中接收指定长度的按键并存入到系统的<seealso cref="Dtmf"/>缓存中
        /// </summary>
        /// <param name="length">获取Dtmf码的长度</param>
        /// <param name="suffix">按键获取操作结束标记，系统默认为"#"也可以通过此函数进行指定</param>
        /// <param name="timeout">按键之间的时间间隔，如果两次按键超过了这个时间系统将会触发<seealso cref="ProcessTimeout"/>事件</param>
        /// <returns>返回收到到的Dtmf码</returns>
        /// <seealso cref="DtmfHit"/>
        /// <seealso cref="ClearDtmf"/>
        string GetDtmf(int length, char suffix, int timeout);
        /// <summary>
        /// 清除系统及底层的Dtmf缓存内容
        /// </summary>
        void ClearDtmf();

        #endregion

        #region 取主/被叫函数

        /// <summary>
        /// 获取主叫号码
        /// </summary>
        /// <returns>主叫号码</returns>
        /// <seealso cref="GetCalleeNumber"/>
        string GetCallerNumber();
        /// <summary>
        /// 获取被叫号码
        /// </summary>
        /// <returns>被叫号码</returns>
        /// <seealso cref="GetCallerNumber"/>
        string GetCalleeNumber();

        #endregion

        #region 放/录音函数

        /// <summary>
        /// 播放语音文件，通过函数提供的文件列表可以直接播放相应的文件，在框架的规划中支持单文件、多文件及内存索引文件的播放
        /// </summary>
        /// <param name="fileList">待播放文件列表</param>
        /// <param name="allowBreak">放音过程中是否允许用户按键打断</param>
        /// <returns>播放语音的结果状态，详细情况请参考<seealso cref="PlayResult"/></returns>
        /// <seealso cref="PlayMessage"/>
        /// <seealso cref="PlayNumber"/>
        /// <seealso cref="PlayToFile"/>
        SwitchStatus PlayFile(string fileList, bool allowBreak);
        /// <summary>
        /// 通过<seealso cref="ITTSEngine"/>引擎实现从文本到语音的播放，
        /// </summary>
        /// <param name="text">待播放的文本内容</param>
        /// <param name="allowBreak">放音过程中是否允许用户按键打断</param>
        /// <returns>播放语音的结果状态，详细信息请参考<seealso cref="PlayResult"/></returns>
        /// <seealso cref="PlayFile"/>
        /// <seealso cref="PlayNumber"/>
        /// <seealso cref="PlayToFile"/>
        SwitchStatus PlayMessage(string text, bool allowBreak);
        /// <summary>
        /// 通过<seealso cref="ITTSEngine"/>引擎实现从文本到语音的播放，此函数专门用于对数字进行转换和播放在相应的TTS引擎中会对此部分进行优化处理
        /// </summary>
        /// <param name="text">待播放的数字内容</param>
        /// <param name="allowBreak">放音过程中是否允许用户按键打断</param>
        /// <param name="playType">TTS放音类型，详细信息请参考<seealso cref="TTSPlayType"/></param>
        /// <returns>播放语音的结果状态，详细信息请参考<seealso cref="PlayResult"/></returns>
        /// <seealso cref="PlayFile"/>
        /// <seealso cref="PlayMessage"/>
        /// <seealso cref="PlayToFile"/>
        SwitchStatus PlayNumber(string text, bool allowBreak, TTSPlayType playType);
        /// <summary>
        /// 通过<seealso cref="ITTSEngine"/>引擎实现从文本内容到语音文件的转换，转换时根据提供的文件扩展名实现相应格式的自动支持
        /// </summary>
        /// <param name="text">待转换的文本内容</param>
        /// <param name="fileName">转换后的语音文件名</param>
        /// <param name="voiceResource">语音资源类型用于指定转换的语种，详细信息请参考<seealso cref="VoiceResource"/></param>
        /// <returns>转换操作的结果状态，详细信息请参考<seealso cref="PlayResult"/></returns>
        /// <seealso cref="PlayFile"/>
        /// <seealso cref="PlayMessage"/>
        /// <seealso cref="PlayNumber"/>
        SwitchStatus PlayToFile(string text, string fileName, VoiceResource voiceResource);
        /// <summary>
        /// 对通道进行录音，注意如果通道当前处于会议模式当中要完成对整个会议的录音需要激活会议模式录音方式，否则将只对本通道进行录音
        /// </summary>
        /// <param name="fileName">录音存放的文件名</param>
        /// <param name="length">录音长度以秒为单位</param>
        /// <param name="allowBreak">录音过程中是否允许用户按键打断</param>
        /// <returns>录音操作结果状态，详细信息请参考<seealso cref="PlayResult"/></returns>
        SwitchStatus RecordFile(string fileName, long length, bool allowBreak);

        #endregion

        #region 通道连通函数

        /// <summary>
        /// 连接到指定通道函数，此函数用于进行多个通道的连通功能，如果多次运用本函数连接多个通道便形成会议功能
        /// </summary>
        /// <param name="channelID">通道标识</param>
        /// <param name="channelType">通道类型</param>
        /// <returns>成功连接则返回所创建会议的组号，否则返回-1</returns>
        /// <seealso cref="UnLink"/>
        /// <seealso cref="UnLinkAll"/>
        /// <seealso cref="ListenTo"/>
        int LinkTo(int channelID, ChannelType channelType);
        /// <summary>
        /// 删除到某一通道的连接，调用此函数将会使用本通道与函数指定的通道同时从会议中退出来，若只是通道单方从会议中退出请参考<seealso cref="UnLink"/>的重载版本
        /// </summary>
        /// <param name="channelID">连接的对方通道</param>
        /// <seealso cref="LinkTo"/>
        /// <seealso cref="UnLinkAll"/>
        /// <seealso cref="ListenTo"/>
        void UnLink(int channelID);
        /// <summary>
        /// 删除通道的连接，可视为从会议中退出来
        /// </summary>
        /// <seealso cref="LinkTo"/>
        /// <seealso cref="UnLinkAll"/>
        /// <seealso cref="ListenTo"/>
        void UnLink();
        /// <summary>
        /// 删除通道所在会议的所有成员并释放系统资源
        /// </summary>
        /// <seealso cref="LinkTo"/>
        /// <seealso cref="UnLink"/>
        /// /// <seealso cref="ListenTo"/>
        void UnLinkAll();
        /// <summary>
        /// 监听到一通道的通话，如果被监听的通道已经处理会议模式则形成对会议的监听
        /// </summary>
        /// <param name="channelID">通道标识</param>
        /// <param name="channelType">通道类型</param>
        /// <returns>成功则返回被监听的会议组号，否则返回-1</returns>
        int ListenTo(int channelID, ChannelType channelType);

        #endregion

        #region 传真函数

        bool SendFax(string fileName);
        bool ReceiveFax(string fileName);

        #endregion

        #region 公共事件

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
