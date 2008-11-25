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
        private string[] VoiceResourcePath = new string[]{"Standard", "Localize", "English"};

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
        /// <param name="workItem">�������</param>
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

        [EventPublication(SwitchEventNames.ChannelStatusChangedEvent, PublicationScope.Global)]
        public event EventHandler<EventArgs<ChannelStatus>> ChannelStatusChanged;
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

        [EventPublication(SwitchEventNames.GetDTMFEvent, PublicationScope.Global)]
        public event EventHandler<EventArgs<string>> GetDTMF;
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

        [EventPublication(SwitchEventNames.CallEvent, PublicationScope.Global)]
        public event EventHandler<CallEventArgs> Call;
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

        [EventPublication(SwitchEventNames.FaxingEvent, PublicationScope.Global)]
        public event EventHandler<FaxEventArgs> Faxing;
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

        [EventPublication(SwitchEventNames.LinkingToChannelEvent, PublicationScope.Global)]
        public event EventHandler<LinkingToChannelEventArgs> LinkingToChannel;
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

        [EventPublication(SwitchEventNames.LinkedToChannelEvent, PublicationScope.Global)]
        public event EventHandler<EventArgs<IChannel>> LinkedToChannel;
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

        [EventPublication(SwitchEventNames.ResetedChannelEvent, PublicationScope.Global)]
        public event EventHandler ResetedChannel;
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

        [EventPublication(SwitchEventNames.ProcessTimeoutEvent, PublicationScope.Global)]
        public event EventHandler ProcessTimeout;
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

        public int ChannelID
        {
            get { return channelID; }
        }

        public string ChannelAlias
        {
            get { return channelAlias; }
            set { channelAlias = value; }
        }

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

        public ChannelType ChannelType
        {
            get { return channelType; }
            protected set { channelType = value; }
        }

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

        public ChannelStatus LastStatus
        {
            get { return lastStatus; }
        }

        public ConferenceResource ConfResource
        {
            get { return confResource; }
            protected set { confResource = value; }
        }

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

        public string FaxLocalID
        {
            get { return faxLocalID; }
            set { faxLocalID = value; }
        }

        public IChannel LinkedChannel
        {
            get
            {
                return linkedChanelID >= 0 && linkedChanelID < Driver.ChannelCount ? Driver.Channels[linkedChanelID] : null;
            }
        }

        public ILog Logger
        {
            get { return logger; }
        }

        public ICTIDriver Driver
        {
            get { return driver; }
        }

        public virtual bool RingDetect(int Times)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual bool OffHookDetect()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual void OffHook()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual bool HangUpDetect()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual void HangUp()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual void ResetChannel()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual CallStatus Dial(string prefixNum, string dialNum)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual bool DtmfHit()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual string GetDtmf(int length, char suffix, int timeout)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual void ClearDtmf()
        {
            if (dtmf != null && dtmf != string.Empty)
                dtmf = string.Empty;
        }

        public virtual string GetCallerNumber()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual string GetCalleeNumber()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual SwitchStatus PlayFile(string fileList, bool allowBreak)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual SwitchStatus PlayMessage(string text, bool allowBreak)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual SwitchStatus PlayNumber(string text, bool allowBreak, TTSPlayType playType)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual SwitchStatus PlayToFile(string text, string fileName, VoiceResource voiceResource)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual SwitchStatus RecordFile(string fileName, long length, bool allowBreak)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual int LinkTo(int channelID, ChannelType channelType)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual void UnLink(int channelID)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual void UnLink()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual void UnLinkAll()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual int ListenTo(int channelID, ChannelType channelType)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual bool SendFax(string fileName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual bool ReceiveFax(string fileName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

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
