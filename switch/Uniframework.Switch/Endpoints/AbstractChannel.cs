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
    /// ����ͨ����
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
        private ConferenceResource confResource = new ConferenceResource(ConferenceType.UNKOWN, -1); // ͨ��������Ļ�����Դ
        private ILog logger = null;
        private ICTIDriver driver = null;

        private Dictionary<string, List<IScriptLoader>> scriptLoaders;

        /// <summary>
        /// ����ͨ�����캯��
        /// </summary>
        /// <param name="driver">�忨������</param>
        /// <param name="channelID">ͨ�����</param>
        public AbstractChannel(ICTIDriver driver, int channelID)
        {
            // ��ʼ����־���
            logger = driver.WorkItem.Services.Get<ILog>();

            this.channelID = channelID;
            this.driver = driver;
            scriptLoaders = new Dictionary<string, List<IScriptLoader>>();
            CurrentStatus = ChannelStatus.INIT;
        }

        #region Assistant function

        /// <summary>
        /// ִ��ָ���¼���������¹ҽӵĽű�����
        /// </summary>
        /// <param name="eventHandler">�¼������������</param>
        private void RunScripts(string eventHandler)
        {
            List<IScriptLoader> list = scriptLoaders[eventHandler];
            if (list != null)
                foreach (IScriptLoader loader in list)
                {
                    // �����̼߳��ؽű��������ͬʱ���ض��ʵ�������� 2006-12-26
                    //Thread thread = new Thread(new ThreadStart(loader.RunScript));
                    //thread.Start();
                    loader.RunScript();
                }
        }

        /// <summary>
        ///   ����ļ��Ƿ���ڣ�ϵͳ���Ȼ���ȫ����Դ�ļ����в��Ҹ��ļ������������
        /// �Ͳ�ѯ��Ŀ�ļ��л�ϵͳ��װĿ¼����ǰ�汾ֻ����Դ�ļ����н���������û��
        /// �������������Ĺ��ܡ�
        /// </summary>
        /// <param name="filename">�������ļ���</param>
        /// <returns>�ļ��ľ���·������</returns>
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
        /// ָ��ͨ����ŵ�ͨ�����ӵ���ͨ����ֻ�Ǽ򵥵�����һ��LinkedChannelID��ͨ��״̬��
        /// </summary>
        /// <param name="channelID">ͨ����ʶ</param>
        protected void LinkToChannel(int chnlid)
        {
            linkedChanelID = chnlid;
            CurrentStatus = ChannelStatus.JOIN;
        }

        /// <summary>
        /// ������ָ��ͨ��������
        /// </summary>
        protected void UnLinkToChannel()
        {
            linkedChanelID = -1;
            CurrentStatus = ChannelStatus.IDLE;
        }

        #endregion

        #region Channel Common function

        /// <summary>
        /// ע��ű��ļ�
        /// </summary>
        /// <param name="eventHandler">�¼��������</param>
        /// <param name="scriptFile">�ű��ļ�</param>
        public void RegisgerScript(string eventHandler, string scriptFile)
        {
            Uniframework.Guard.ArgumentNotNull(eventHandler, "eventHandler");
            Uniframework.Guard.ArgumentNotNull(scriptFile, "scriptFile");

            if (!scriptLoaders.ContainsKey(eventHandler))
                scriptLoaders.Add(eventHandler, new List<IScriptLoader>());
            scriptLoaders[eventHandler].Add(new ScriptLoader(scriptFile, string.Empty, this));
        }

        /// <summary>
        /// ע���ű��ļ�
        /// </summary>
        /// <param name="eventHandler">�¼��������</param>
        public void UnRegisgerScript(string eventHandler)
        {
            List<IScriptLoader> list = scriptLoaders[eventHandler];
            if(list != null)
                list.Clear();
        }

        /// <summary>
        /// ע���ű��ļ�
        /// </summary>
        /// <param name="eventHandler">�¼��������</param>
        /// <param name="scriptFile">�ű��ļ�</param>
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
        /// ͨ����ʶ������Ψһ��ʶÿ��ͨ��
        /// </summary>
        /// <value></value>
        public int ChannelID
        {
            get { return channelID; }
        }

        /// <summary>
        /// ͨ������
        /// </summary>
        /// <value></value>
        public string ChannelAlias
        {
            get { return channelAlias; }
            set { channelAlias = value; }
        }

        /// <summary>
        /// �û�����ɾ���������������ײ���յ�ϵͳ�����ɾ����ʱ����Dtmf������ɾȥ���һ������
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
        /// ͨ������
        /// </summary>
        /// <value></value>
        public ChannelType ChannelType
        {
            get { return channelType; }
            protected set { channelType = value; }
        }

        /// <summary>
        /// ͨ��״̬
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
        /// ͨ����ǰ�ڲ�״̬���û��������ͨ��CurrentStatus��ȡͨ���ĵ�ǰ״̬����״̬��ϵͳ���и���
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
        /// ͨ���ϴε��ڲ�״̬�����û�������ʵ�ָ��ӵ�ҵ���߼�ʱʹ��
        /// </summary>
        /// <value></value>
        public ChannelStatus LastStatus
        {
            get { return lastStatus; }
        }

        /// <summary>
        /// ͨ�������Ļ�����Դ
        /// </summary>
        /// <value></value>
        public ConferenceResource ConfResource
        {
            get { return confResource; }
            protected set { confResource = value; }
        }

        /// <summary>
        /// ������Դ����
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
        /// �û�����֮��ĳ�ʱֵ,��λ���룩
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
        /// ͨ�����ʶ����ֵ��ӳͨ����ǰ�Ƿ����
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
        /// Dtmf���壬ϵͳ��Է�������Dtmf�붼����������
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
        /// �Ƿ���м��Լ�⣬���������Ϊtrue��ϵͳ���м��Լ�����ж϶Է�ժ�����һ���
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
        /// ���ش����ַ���
        /// </summary>
        /// <value></value>
        public string FaxLocalID
        {
            get { return faxLocalID; }
            set { faxLocalID = value; }
        }

        /// <summary>
        /// �뵱ǰͨ�����ӵ�ͨ��
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
        /// ϵͳ��־���
        /// </summary>
        /// <value></value>
        public ILog Logger
        {
            get { return logger; }
        }

        /// <summary>
        /// ������������
        /// </summary>
        /// <value></value>
        public ICTIDriver Driver
        {
            get { return driver; }
        }

        /// <summary>
        /// ���庯�������ڼ�⵱ǰͨ���Ƿ��к���
        /// </summary>
        /// <param name="Times">�����������������˼��Լ��˲�����������Ч</param>
        /// <returns>����к��뷵��true�����򷵻�false</returns>
        /// <seealso cref="DetectPolarity"/>
        public virtual bool RingDetect(int Times)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// ժ�����
        /// </summary>
        /// <returns>���ժ������true�����򷵻�false</returns>
        public virtual bool OffHookDetect()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// ͨ����Ӧ����·ժ��
        /// </summary>
        public virtual void OffHook()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// �һ����
        /// </summary>
        /// <returns>���ͨ����Ӧ����·�һ�����true�����򷵻�false</returns>
        public virtual bool HangUpDetect()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// ͨ����Ӧ����·�һ�
        /// </summary>
        public virtual void HangUp()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// ����ͨ��
        /// </summary>
        /// <seealso cref="HangUpDetect"/>
        /// <seealso cref="HangUp"/>
        public virtual void ResetChannel()
        {
            throw new Exception("The method or operation is not implemented.");
        }

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
        public virtual CallStatus Dial(string prefixNum, string dialNum)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// ͨ����Ӧ����·�������
        /// </summary>
        /// <returns>�����·�Է��а����򷵻�true�����򷵻�false</returns>
        /// <seealso cref="GetDtmf"/>
        /// <seealso cref="ClearDtmf"/>
        /// <seealso cref="Dial"/>
        public virtual bool DtmfHit()
        {
            throw new Exception("The method or operation is not implemented.");
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
        public virtual string GetDtmf(int length, char suffix, int timeout)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// ���ϵͳ���ײ��Dtmf��������
        /// </summary>
        public virtual void ClearDtmf()
        {
            if (dtmf != null && dtmf != string.Empty)
                dtmf = string.Empty;
        }

        /// <summary>
        /// ��ȡ���к���
        /// </summary>
        /// <returns>���к���</returns>
        /// <seealso cref="GetCalleeNumber"/>
        public virtual string GetCallerNumber()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// ��ȡ���к���
        /// </summary>
        /// <returns>���к���</returns>
        /// <seealso cref="GetCallerNumber"/>
        public virtual string GetCalleeNumber()
        {
            throw new Exception("The method or operation is not implemented.");
        }

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
        public virtual SwitchStatus PlayFile(string fileList, bool allowBreak)
        {
            throw new Exception("The method or operation is not implemented.");
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
        public virtual SwitchStatus PlayMessage(string text, bool allowBreak)
        {
            throw new Exception("The method or operation is not implemented.");
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
        public virtual SwitchStatus PlayNumber(string text, bool allowBreak, TTSPlayType playType)
        {
            throw new Exception("The method or operation is not implemented.");
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
        public virtual SwitchStatus PlayToFile(string text, string fileName, VoiceResource voiceResource)
        {
            throw new Exception("The method or operation is not implemented.");
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
        public virtual SwitchStatus RecordFile(string fileName, long length, bool allowBreak)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// ���ӵ�ָ��ͨ���������˺������ڽ��ж��ͨ������ͨ���ܣ����������ñ��������Ӷ��ͨ�����γɻ��鹦��
        /// </summary>
        /// <param name="channelID">ͨ����ʶ</param>
        /// <param name="channelType">ͨ������</param>
        /// <returns>�ɹ������򷵻��������������ţ����򷵻�-1</returns>
        /// <seealso cref="UnLink"/>
        /// <seealso cref="UnLinkAll"/>
        /// <seealso cref="ListenTo"/>
        public virtual int LinkTo(int channelID, ChannelType channelType)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// ɾ����ĳһͨ�������ӣ����ô˺�������ʹ�ñ�ͨ���뺯��ָ����ͨ��ͬʱ�ӻ������˳�������ֻ��ͨ�������ӻ������˳���ο�<seealso cref="UnLink"/>�����ذ汾
        /// </summary>
        /// <param name="channelID">���ӵĶԷ�ͨ��</param>
        /// <seealso cref="LinkTo"/>
        /// <seealso cref="UnLinkAll"/>
        /// <seealso cref="ListenTo"/>
        public virtual void UnLink(int channelID)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// ɾ��ͨ�������ӣ�����Ϊ�ӻ������˳���
        /// </summary>
        /// <seealso cref="LinkTo"/>
        /// <seealso cref="UnLinkAll"/>
        /// <seealso cref="ListenTo"/>
        public virtual void UnLink()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// ɾ��ͨ�����ڻ�������г�Ա���ͷ�ϵͳ��Դ
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
        /// ������һͨ����ͨ���������������ͨ���Ѿ��������ģʽ���γɶԻ���ļ���
        /// </summary>
        /// <param name="channelID">ͨ����ʶ</param>
        /// <param name="channelType">ͨ������</param>
        /// <returns>�ɹ��򷵻ر������Ļ�����ţ����򷵻�-1</returns>
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
