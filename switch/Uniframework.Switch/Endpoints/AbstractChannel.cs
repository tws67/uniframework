using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

using log4net;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;

using Uniframework.Services;
using Uniframework.SmartClient;

namespace Uniframework.Switch
{
    /// <summary>
    /// 抽象通道类
    /// </summary>
    public class AbstractChannel : IChannel, IDisposable
    {
        private string[] VoiceResourcePath = new string[] {"Standard", "Localize", "English"};

        private int channelID = -1;
        private string channelAlias = string.Empty;
        private char backspaceKey = '*';
        private ChannelType channelType = ChannelType.EMPTY;
        private ChannelState channelState = ChannelState.NEW;
        private ChannelStatus currentStatus = ChannelStatus.NEW;
        private ChannelStatus lastStatus = ChannelStatus.NEW;
        private VoiceResource voiceResource = VoiceResource.UNKNOWN;
        private int hitTimeout = 10;
        private bool enable = false;
        private string dtmf = string.Empty;
        private bool detectPolarity = false;
        private string faxLocalID = string.Empty;
        protected int linkedChanelID = -1;
        private ConferenceResource confResource = new ConferenceResource(ConferenceType.UNKOWN, -1); // 通道所加入的会议资源
        private ILog logger = null;
        private ICTIDriver driver = null;

        private Dictionary<string, List<IScriptLoader>> scriptLoaders;

        /// <summary>
        /// 抽象通道构造函数
        /// </summary>
        /// <param name="driver">板卡适配器</param>
        /// <param name="channelID">通道编号</param>
        public AbstractChannel(ICTIDriver driver, int channelID)
        {
            // 初始化日志组件
            logger = driver.WorkItem.Services.Get<ILog>();

            this.channelID = channelID;
            this.driver = driver;
            scriptLoaders = new Dictionary<string, List<IScriptLoader>>();
            CurrentStatus = ChannelStatus.INIT;
        }

        #region Assistant function

        /// <summary>
        /// 执行指定事件处理程序下挂接的脚本程序
        /// </summary>
        /// <param name="eventHandler">事件处理程序名称</param>
        private void RunScripts(string eventHandler)
        {
            List<IScriptLoader> list = scriptLoaders[eventHandler];
            if (list != null)
                foreach (IScriptLoader loader in list)
                {
                    // 采用线程加载脚本程序存在同时加载多个实例的现象 2006-12-26
                    //Thread thread = new Thread(new ThreadStart(loader.RunScript));
                    //thread.Start();
                    loader.RunScript();
                }
        }

        /// <summary>
        ///   检查文件是否存在，系统首先会在全局资源文件夹中查找该文件，如果不存在
        /// 就查询项目文件夹或系统安装目录。当前版本只在资源文件夹中进行搜索，没有
        /// 采用智能搜索的功能。
        /// </summary>
        /// <param name="filename">待检查的文件名</param>
        /// <returns>文件的绝对路径名称</returns>
        protected string CheckFile(string filename)
        {
            IVirtualCTI virtualCTI = driver.WorkItem.Services.Get<IVirtualCTI>();
            string dir = virtualCTI.GlobalVars[SwitchVariableNames.ResourceDir];
            string file = string.Empty;

            if(File.Exists(filename))
                return filename;
            else
            {
                
                dir = (dir != null && dir != string.Empty) ? dir : AppDomain.CurrentDomain.BaseDirectory;
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                switch(voiceResource)
                {
                    case VoiceResource.UNKNOWN :
                        file = dir + filename;
                        break;
                        
                    case VoiceResource.STANDARD :
                        file = Path.Combine(dir, VoiceResourcePath[0]) + Path.DirectorySeparatorChar + filename;
                        break;
                        
                    case VoiceResource.LOCALIZE :
                        file = Path.Combine(dir, VoiceResourcePath[1]) + Path.DirectorySeparatorChar + filename;
                        break;
                    
                    case VoiceResource.ENGLISH :
                        file = Path.Combine(dir, VoiceResourcePath[2]) + Path.DirectorySeparatorChar + filename;
                        break;
                }
                if (File.Exists(file))
                    return file;
            }
            return file;
        }

        /// <summary>
        /// 指定通道编号的通道连接到本通道，只是简单的设置一下LinkedChannelID及通道状态字
        /// </summary>
        /// <param name="channelID">通道标识</param>
        protected void LinkToChannel(int chnlid)
        {
            linkedChanelID = chnlid;
            CurrentStatus = ChannelStatus.JOIN;
        }

        /// <summary>
        /// 撤销对指定通道的连接
        /// </summary>
        protected void UnLinkToChannel()
        {
            linkedChanelID = -1;
            CurrentStatus = ChannelStatus.IDLE;
        }

        #endregion

        #region Channel Common function

        /// <summary>
        /// 注册脚本文件
        /// </summary>
        /// <param name="eventHandler">事件处理程序</param>
        /// <param name="scriptFile">脚本文件</param>
        public void RegisgerScript(string eventHandler, string scriptFile)
        {
            Uniframework.Guard.ArgumentNotNull(eventHandler, "eventHandler");
            Uniframework.Guard.ArgumentNotNull(scriptFile, "scriptFile");

            if (!scriptLoaders.ContainsKey(eventHandler))
                scriptLoaders.Add(eventHandler, new List<IScriptLoader>());
            scriptLoaders[eventHandler].Add(new ScriptLoader(scriptFile, string.Empty, this));
        }

        /// <summary>
        /// 注销脚本文件
        /// </summary>
        /// <param name="eventHandler">事件处理程序</param>
        public void UnRegisgerScript(string eventHandler)
        {
            List<IScriptLoader> list = scriptLoaders[eventHandler];
            if(list != null)
                list.Clear();
        }

        /// <summary>
        /// 注销脚本文件
        /// </summary>
        /// <param name="eventHandler">事件处理程序</param>
        /// <param name="scriptFile">脚本文件</param>
        public void UnRegisterScript(string eventHandler, string scriptFile)
        {
            List<IScriptLoader> list = scriptLoaders[eventHandler];
            if(list != null)
                foreach (IScriptLoader loader in list)
                {
                    if (loader.FileName == scriptFile)
                    {
                        list.Remove(loader);
                        return;
                    }
                }
        }

        #endregion

        #region Channel Common Events

        /// <summary>
        /// Occurs when [channel status changed].
        /// </summary>
        [EventPublication(SwitchEventNames.ChannelStatusChangedEvent, PublicationScope.Global)]
        public event EventHandler<EventArgs<ChannelStatus>> ChannelStatusChanged;
        /// <summary>
        /// Called when [channel status changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Uniframework.EventArgs&lt;Uniframework.Switch.ChannelStatus&gt;"/> instance containing the event data.</param>
        protected virtual void OnChannelStatusChanged(object sender, EventArgs<ChannelStatus> e)
        {
            if (scriptLoaders.ContainsKey(ChannelEventHandlerName.ChannelStatusChanged))
            {
                RunScripts(ChannelEventHandlerName.ChannelStatusChanged);
            }
            else
                if (this.ChannelStatusChanged != null)
                {
                    this.ChannelStatusChanged(sender, e);
                }
        }

        /// <summary>
        /// Occurs when [get DTMF].
        /// </summary>
        [EventPublication(SwitchEventNames.GetDTMFEvent, PublicationScope.Global)]
        public event EventHandler<EventArgs<string>> GetDTMF;
        /// <summary>
        /// Called when [get DTMF].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Uniframework.EventArgs&lt;System.String&gt;"/> instance containing the event data.</param>
        protected virtual void OnGetDTMF(object sender, EventArgs<string> e)
        {
            if (scriptLoaders.ContainsKey(ChannelEventHandlerName.GetDTMF))
            {
                RunScripts(ChannelEventHandlerName.GetDTMF);
            }
            else
                if (this.GetDTMF != null)
                {
                    this.GetDTMF(sender, e);
                }
        }

        /// <summary>
        /// Occurs when [call].
        /// </summary>
        [EventPublication(SwitchEventNames.CallEvent, PublicationScope.Global)]
        public event EventHandler<CallEventArgs> Call;
        /// <summary>
        /// Called when [call].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Uniframework.Switch.CallEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCall(object sender, CallEventArgs e)
        {
            if (scriptLoaders.ContainsKey(ChannelEventHandlerName.Call))
            {
                RunScripts(ChannelEventHandlerName.Call); 
            }
            else
                if (this.Call != null)
                {
                    this.Call(sender, e);
                }
        }

        /// <summary>
        /// Occurs when [faxing].
        /// </summary>
        [EventPublication(SwitchEventNames.FaxingEvent, PublicationScope.Global)]
        public event EventHandler<FaxEventArgs> Faxing;
        /// <summary>
        /// Called when [faxing].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Uniframework.Switch.FaxEventArgs"/> instance containing the event data.</param>
        protected virtual void OnFaxing(object sender, FaxEventArgs e)
        {
            if (scriptLoaders.ContainsKey(ChannelEventHandlerName.Faxing))
            {
                RunScripts(ChannelEventHandlerName.Faxing);
            }
            else
                if (this.Faxing != null)
                    this.Faxing(sender, e);
        }

        /// <summary>
        /// Occurs when [linking to channel].
        /// </summary>
        [EventPublication(SwitchEventNames.LinkingToChannelEvent, PublicationScope.Global)]
        public event EventHandler<LinkingToChannelEventArgs> LinkingToChannel;
        /// <summary>
        /// Called when [linking to channel].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Uniframework.Switch.LinkingToChannelEventArgs"/> instance containing the event data.</param>
        protected virtual void OnLinkingToChannel(object sender, LinkingToChannelEventArgs e)
        {
            if (scriptLoaders.ContainsKey(ChannelEventHandlerName.LinkingToChannel))
            {
                RunScripts(ChannelEventHandlerName.LinkingToChannel);
            }
            else
                if (this.LinkingToChannel != null)
                {
                    this.LinkingToChannel(sender, e);
                }
        }

        /// <summary>
        /// Occurs when [linked to channel].
        /// </summary>
        [EventPublication(SwitchEventNames.LinkedToChannelEvent, PublicationScope.Global)]
        public event EventHandler<EventArgs<IChannel>> LinkedToChannel;
        /// <summary>
        /// Called when [linked to channel].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Uniframework.EventArgs&lt;Uniframework.Switch.IChannel&gt;"/> instance containing the event data.</param>
        protected virtual void OnLinkedToChannel(object sender, EventArgs<IChannel> e)
        {
            if (scriptLoaders.ContainsKey(ChannelEventHandlerName.LinkedToChannel))
            {
                RunScripts(ChannelEventHandlerName.LinkedToChannel);
            }
            else
                if (this.LinkedToChannel != null)
                {
                    this.LinkedToChannel(sender, e);
                }
        }

        /// <summary>
        /// Occurs when [reseted channel].
        /// </summary>
        [EventPublication(SwitchEventNames.ResetedChannelEvent, PublicationScope.Global)]
        public event EventHandler ResetedChannel;
        /// <summary>
        /// Called when [reseted channel].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnResetedChannel(object sender, EventArgs e)
        {
            if (scriptLoaders.ContainsKey(ChannelEventHandlerName.ResetedChannel))
            {
                RunScripts(ChannelEventHandlerName.ResetedChannel);
            }
            else
                if (this.ResetedChannel != null)
                {
                    this.ResetedChannel(sender, e);
                }
        }

        /// <summary>
        /// Occurs when [process timeout].
        /// </summary>
        [EventPublication(SwitchEventNames.ProcessTimeoutEvent, PublicationScope.Global)]
        public event EventHandler ProcessTimeout;
        /// <summary>
        /// Called when [process timeout].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnProcessTimeout(object sender, EventArgs e)
        {
            if (scriptLoaders.ContainsKey(ChannelEventHandlerName.ProcessTimeout))
            {
                RunScripts(ChannelEventHandlerName.ProcessTimeout);
            }
            else
                if (this.ProcessTimeout != null)
                {
                    this.ProcessTimeout(sender, e);
                }
        }

        #endregion 
                
        #region IChannel Members

        /// <summary>
        /// 通道标识，用于唯一标识每条通道
        /// </summary>
        /// <value></value>
        public int ChannelID
        {
            get { return channelID; }
        }

        /// <summary>
        /// 通道别名
        /// </summary>
        /// <value></value>
        public string ChannelAlias
        {
            get { return channelAlias; }
            set { channelAlias = value; }
        }

        /// <summary>
        /// 用户按键删除键，当适配器底层接收到系统定义的删除键时将从Dtmf缓冲中删去最后一个按键
        /// </summary>
        /// <value></value>
        public char BackspaceKey
        {
            get
            {
                return backspaceKey;
            }
            set
            {
                backspaceKey = value;
            }
        }

        /// <summary>
        /// 通道类型
        /// </summary>
        /// <value></value>
        public ChannelType ChannelType
        {
            get { return channelType; }
            protected set { channelType = value; }
        }

        /// <summary>
        /// 通道状态
        /// </summary>
        /// <value></value>
        public ChannelState ChannelState
        {
            get { return channelState; }
            internal set 
            {
                lock (this)
                {
                    channelState = value;
                }
            }
        }

        /// <summary>
        /// 通道当前内部状态，用户程序可以通过CurrentStatus获取通道的当前状态，此状态由系统进行更改
        /// </summary>
        /// <value></value>
        public ChannelStatus CurrentStatus
        {
            get { return currentStatus; }
            set 
            {
                if (currentStatus != value)
                { 
                    lastStatus = currentStatus;
                    currentStatus = value;
                    OnChannelStatusChanged(this, new EventArgs<ChannelStatus>(value));
                }
            }
        }

        /// <summary>
        /// 通道上次的内部状态，在用户程序中实现复杂的业务逻辑时使用
        /// </summary>
        /// <value></value>
        public ChannelStatus LastStatus
        {
            get { return lastStatus; }
        }

        /// <summary>
        /// 通道所属的会议资源
        /// </summary>
        /// <value></value>
        public ConferenceResource ConfResource
        {
            get { return confResource; }
            protected set { confResource = value; }
        }

        /// <summary>
        /// 语音资源类型
        /// </summary>
        /// <value></value>
        public VoiceResource VoiceResource
        {
            get
            {
                return voiceResource;
            }
            set
            {
                voiceResource = value;
            }
        }

        /// <summary>
        /// 用户按键之间的超时值,单位（秒）
        /// </summary>
        /// <value></value>
        public int HitTimeout
        {
            get
            {
                return hitTimeout;
            }
            set
            {
                hitTimeout = value;
            }
        }

        /// <summary>
        /// 通道活动标识，此值反映通道当前是否可用
        /// </summary>
        /// <value></value>
        public bool Enable
        {
            get
            {
                return enable;
            }
            set
            {
                if (enable != value)
                {
                    enable = value;
                    CurrentStatus = enable ? ChannelStatus.IDLE : ChannelStatus.RELEASE;
                }
            }
        }

        /// <summary>
        /// Dtmf缓冲，系统与对方交互的Dtmf码都将缓存至此
        /// </summary>
        /// <value></value>
        public string Dtmf
        {
            get
            {
                return dtmf;
            }
            set
            {
                if (dtmf != value)
                {
                    dtmf = value;
                }
            }
        }

        /// <summary>
        /// 是否进行极性检测，如果此属性为true则系统进行极性检测来判断对方摘机、挂机等
        /// </summary>
        /// <value></value>
        public bool DetectPolarity
        {
            get
            {
                return detectPolarity;
            }
            set
            {
                if (detectPolarity != value)
                    detectPolarity = value;
            }
        }

        /// <summary>
        /// 本地传真字符串
        /// </summary>
        /// <value></value>
        public string FaxLocalID
        {
            get { return faxLocalID; }
            set { faxLocalID = value; }
        }

        /// <summary>
        /// 与当前通道连接的通道
        /// </summary>
        /// <value></value>
        public IChannel LinkedChannel
        {
            get
            {
                return linkedChanelID >= 0 && linkedChanelID < Driver.ChannelCount ? Driver.Channels[linkedChanelID] : null;
            }
        }

        /// <summary>
        /// 系统日志组件
        /// </summary>
        /// <value></value>
        public ILog Logger
        {
            get { return logger; }
        }

        /// <summary>
        /// 驱动器适配器
        /// </summary>
        /// <value></value>
        public ICTIDriver Driver
        {
            get { return driver; }
        }

        /// <summary>
        /// 振铃函数，用于检测当前通道是否有呼入
        /// </summary>
        /// <param name="Times">振铃次数，如果启用了极性检测此参数将不再有效</param>
        /// <returns>如果有呼入返回true，否则返回false</returns>
        /// <seealso cref="DetectPolarity"/>
        public virtual bool RingDetect(int Times)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 摘机检测
        /// </summary>
        /// <returns>如果摘机返回true，否则返回false</returns>
        public virtual bool OffHookDetect()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 通道对应的线路摘机
        /// </summary>
        public virtual void OffHook()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 挂机检测
        /// </summary>
        /// <returns>如果通道对应的线路挂机返回true，否则返回false</returns>
        public virtual bool HangUpDetect()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 通道对应的线路挂机
        /// </summary>
        public virtual void HangUp()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 重置通道
        /// </summary>
        /// <seealso cref="HangUpDetect"/>
        /// <seealso cref="HangUp"/>
        public virtual void ResetChannel()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 外线通道拨号函数
        /// </summary>
        /// <param name="prefixNum">预拨号码，对应于中集、汇线通等业务的外拨，如需要在拨市话前先拨"9"再拨实际的电话号码</param>
        /// <param name="dialNum">待拨出的电话号码</param>
        /// <returns>
        /// 返回拨号的结果，如对方忙、没有人接听更详细的信息请参考<seealso cref="SignalType"/>
        /// </returns>
        /// <seealso cref="RingDetect"/>
        /// <seealso cref="OffHook"/>
        public virtual CallStatus Dial(string prefixNum, string dialNum)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 通道对应的线路按键检测
        /// </summary>
        /// <returns>如果线路对方有按键则返回true，否则返回false</returns>
        /// <seealso cref="GetDtmf"/>
        /// <seealso cref="ClearDtmf"/>
        /// <seealso cref="Dial"/>
        public virtual bool DtmfHit()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 从适配器对应的底层API中接收指定长度的按键并存入到系统的<seealso cref="Dtmf"/>缓存中
        /// </summary>
        /// <param name="length">获取Dtmf码的长度</param>
        /// <param name="suffix">按键获取操作结束标记，系统默认为"#"也可以通过此函数进行指定</param>
        /// <param name="timeout">按键之间的时间间隔，如果两次按键超过了这个时间系统将会触发<seealso cref="ProcessTimeout"/>事件</param>
        /// <returns>返回收到到的Dtmf码</returns>
        /// <seealso cref="DtmfHit"/>
        /// <seealso cref="ClearDtmf"/>
        public virtual string GetDtmf(int length, char suffix, int timeout)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 清除系统及底层的Dtmf缓存内容
        /// </summary>
        public virtual void ClearDtmf()
        {
            if (dtmf != null && dtmf != string.Empty)
                dtmf = string.Empty;
        }

        /// <summary>
        /// 获取主叫号码
        /// </summary>
        /// <returns>主叫号码</returns>
        /// <seealso cref="GetCalleeNumber"/>
        public virtual string GetCallerNumber()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 获取被叫号码
        /// </summary>
        /// <returns>被叫号码</returns>
        /// <seealso cref="GetCallerNumber"/>
        public virtual string GetCalleeNumber()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 播放语音文件，通过函数提供的文件列表可以直接播放相应的文件，在框架的规划中支持单文件、多文件及内存索引文件的播放
        /// </summary>
        /// <param name="fileList">待播放文件列表</param>
        /// <param name="allowBreak">放音过程中是否允许用户按键打断</param>
        /// <returns>
        /// 播放语音的结果状态，详细情况请参考<seealso cref="PlayResult"/>
        /// </returns>
        /// <seealso cref="PlayMessage"/>
        /// <seealso cref="PlayNumber"/>
        /// <seealso cref="PlayToFile"/>
        public virtual SwitchStatus PlayFile(string fileList, bool allowBreak)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 通过<seealso cref="ITTSEngine"/>引擎实现从文本到语音的播放，
        /// </summary>
        /// <param name="text">待播放的文本内容</param>
        /// <param name="allowBreak">放音过程中是否允许用户按键打断</param>
        /// <returns>
        /// 播放语音的结果状态，详细信息请参考<seealso cref="PlayResult"/>
        /// </returns>
        /// <seealso cref="PlayFile"/>
        /// <seealso cref="PlayNumber"/>
        /// <seealso cref="PlayToFile"/>
        public virtual SwitchStatus PlayMessage(string text, bool allowBreak)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 通过<seealso cref="ITTSEngine"/>引擎实现从文本到语音的播放，此函数专门用于对数字进行转换和播放在相应的TTS引擎中会对此部分进行优化处理
        /// </summary>
        /// <param name="text">待播放的数字内容</param>
        /// <param name="allowBreak">放音过程中是否允许用户按键打断</param>
        /// <param name="playType">TTS放音类型，详细信息请参考<seealso cref="TTSPlayType"/></param>
        /// <returns>
        /// 播放语音的结果状态，详细信息请参考<seealso cref="PlayResult"/>
        /// </returns>
        /// <seealso cref="PlayFile"/>
        /// <seealso cref="PlayMessage"/>
        /// <seealso cref="PlayToFile"/>
        public virtual SwitchStatus PlayNumber(string text, bool allowBreak, TTSPlayType playType)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 通过<seealso cref="ITTSEngine"/>引擎实现从文本内容到语音文件的转换，转换时根据提供的文件扩展名实现相应格式的自动支持
        /// </summary>
        /// <param name="text">待转换的文本内容</param>
        /// <param name="fileName">转换后的语音文件名</param>
        /// <param name="voiceResource">语音资源类型用于指定转换的语种，详细信息请参考<seealso cref="VoiceResource"/></param>
        /// <returns>
        /// 转换操作的结果状态，详细信息请参考<seealso cref="PlayResult"/>
        /// </returns>
        /// <seealso cref="PlayFile"/>
        /// <seealso cref="PlayMessage"/>
        /// <seealso cref="PlayNumber"/>
        public virtual SwitchStatus PlayToFile(string text, string fileName, VoiceResource voiceResource)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 对通道进行录音，注意如果通道当前处于会议模式当中要完成对整个会议的录音需要激活会议模式录音方式，否则将只对本通道进行录音
        /// </summary>
        /// <param name="fileName">录音存放的文件名</param>
        /// <param name="length">录音长度以秒为单位</param>
        /// <param name="allowBreak">录音过程中是否允许用户按键打断</param>
        /// <returns>
        /// 录音操作结果状态，详细信息请参考<seealso cref="PlayResult"/>
        /// </returns>
        public virtual SwitchStatus RecordFile(string fileName, long length, bool allowBreak)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 连接到指定通道函数，此函数用于进行多个通道的连通功能，如果多次运用本函数连接多个通道便形成会议功能
        /// </summary>
        /// <param name="channelID">通道标识</param>
        /// <param name="channelType">通道类型</param>
        /// <returns>成功连接则返回所创建会议的组号，否则返回-1</returns>
        /// <seealso cref="UnLink"/>
        /// <seealso cref="UnLinkAll"/>
        /// <seealso cref="ListenTo"/>
        public virtual int LinkTo(int channelID, ChannelType channelType)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 删除到某一通道的连接，调用此函数将会使用本通道与函数指定的通道同时从会议中退出来，若只是通道单方从会议中退出请参考<seealso cref="UnLink"/>的重载版本
        /// </summary>
        /// <param name="channelID">连接的对方通道</param>
        /// <seealso cref="LinkTo"/>
        /// <seealso cref="UnLinkAll"/>
        /// <seealso cref="ListenTo"/>
        public virtual void UnLink(int channelID)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 删除通道的连接，可视为从会议中退出来
        /// </summary>
        /// <seealso cref="LinkTo"/>
        /// <seealso cref="UnLinkAll"/>
        /// <seealso cref="ListenTo"/>
        public virtual void UnLink()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 删除通道所在会议的所有成员并释放系统资源
        /// </summary>
        /// <seealso cref="LinkTo"/>
        /// <seealso cref="UnLink"/>
        /// /// 
        /// <seealso cref="ListenTo"/>
        public virtual void UnLinkAll()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 监听到一通道的通话，如果被监听的通道已经处理会议模式则形成对会议的监听
        /// </summary>
        /// <param name="channelID">通道标识</param>
        /// <param name="channelType">通道类型</param>
        /// <returns>成功则返回被监听的会议组号，否则返回-1</returns>
        public virtual int ListenTo(int channelID, ChannelType channelType)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Sends the fax.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public virtual bool SendFax(string fileName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Receives the fax.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public virtual bool ReceiveFax(string fileName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        public override string ToString()
        {
            return ChannelID.ToString().PadLeft(3, '0');
        }

        #region IDisposable Members

        public void Close()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (confResource != null && confResource.Confgroup != -1)
                        UnLink();

                    driver = null;
                }
                disposed = true;
            }
        }

        ~AbstractChannel()
        {
            Dispose(false);
        }

        #endregion
    }
}
