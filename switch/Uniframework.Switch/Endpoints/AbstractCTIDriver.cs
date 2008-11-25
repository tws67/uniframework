using System;
using System.Collections.Generic;
using System.Text;

using log4net;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;

using Uniframework;
using Uniframework.SmartClient;

namespace Uniframework.Switch
{
    /// <summary>
    /// ý���ն˽����豸����
    /// </summary>
    public abstract class AbstractCTIDriver : ICTIDriver, IConfigurable, IDisposable
    {
        private readonly static int DefaultTimeout = 30; // Ĭ�ϳ�ʱֵΪ30��
        private readonly static object syncObj = new object();

        private string key = string.Empty;
        private IVersionInfo versionInfo = new VersionInfo("CTICarddriver");
        protected Dictionary<int, List<int>> confGroups; // ������Դ

        private bool active = false;
        private bool canWork = false;
        private int timeout = DefaultTimeout; 
        private ChannelCollection channels = null;

        protected WorkItem workItem;
        protected ILog logger;

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="workItem">�������</param>
        public AbstractCTIDriver(WorkItem workItem)
        {
            this.workItem = workItem;
            this.logger = this.workItem.Services.Get<ILog>();
            channels = null;
            confGroups = new Dictionary<int, List<int>>();
        }

        /// <summary>
        /// ��־��¼���
        /// </summary>
        public ILog Logger
        {
            get { return logger; }
        }

        #region ICTICardDriver Common Events

        // ͨ�������¼���������һ��ͨ��ʱϵͳ�Ὣ������Ϣ֪ͨ�ⲿӦ�ó����Ա��л��������صĴ���
        [EventPublication(SwitchEventNames.CreatedChannelEvent, PublicationScope.Global)]
        public event EventHandler<EventArgs<IChannel>> CreatedChannel;
        protected virtual void OnCreatedChannel(object sender, EventArgs<IChannel> e)
        {
            if (CreatedChannel != null)
            {
                EventInspector.Register(e.Data, workItem);
                CreatedChannel(sender, e);
            }
        }

        #endregion

        #region ICTIDriver Members

        public string Key
        {
            get { return key; }
            protected set { key = value; }
        }

        public IVersionInfo VersionInfo
        {
            get { return versionInfo; }
        }

        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        public WorkItem WorkItem
        {
            get { return workItem; }
        }

        public bool Active
        {
            get { return active; }
            set 
            {
                CreateChannels();
                //// �����ر����е��¼�������
                //foreach (EventDispatcher dispatcher in eventDispatchers.Values)
                //{
                //    if (value == true)
                //        dispatcher.Start();
                //    else
                //        dispatcher.Stop();
                //}
                active = value; 
            }
        }

        public bool CanWork
        {
            get { return canWork; }
            protected set
            {
                canWork = value;
            }
        }

        public ChannelCollection Channels
        {
            get 
            {
                CreateChannels();
                return channels; 
            }
        }

        public int ChannelCount
        {
            get 
            {
                CreateChannels();
                return channels.Count;
            }
        }

        /// <summary>
        /// ��ȡָ�����ͻ�״̬��ͨ��
        /// </summary>
        /// <param name="chnlid">ͨ����ʶ</param>
        /// <param name="chnlType">ͨ������</param>
        /// <param name="chnlSttatus">ͨ��״̬</param>
        /// <returns>���忨���������ò�ָ������Ӧ��ͨ����ʶ��ֱ�ӷ�����Ӧ��ͨ�����������ָ�����ͼ�״̬��ͨ��</returns>
        public abstract IChannel GetChannel(int chnlid, ChannelType chnlType, ChannelStatus chnlSttatus);

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="conf">�������</param>
        /// <param name="chnl">����ͨ��</param>
        public void JoinConf(int conf, IChannel chnl)
        {
            if (confGroups != null)
            {
                if (!confGroups.ContainsKey(conf))
                    confGroups.Add(conf, new List<int>());
                List<int> confs = confGroups[conf];
                confs.Add(chnl.ChannelID);
            }
        }

        /// <summary>
        /// �뿪����
        /// </summary>
        /// <param name="conf">�������</param>
        /// <param name="chnl">����ͨ��</param>
        public void LeaveConf(int conf, IChannel chnl)
        {
            if (confGroups != null)
            {
                if (confGroups.ContainsKey(conf))
                {
                    List<int> confs = confGroups[conf];
                    confs.Remove(chnl.ChannelID);
                    if (confs.Count == 0)
                        confGroups.Remove(conf);
                }
            }
        }

        /// <summary>
        /// ϵͳ��ǰ�����Ļ�������
        /// </summary>
        public int ConfCount
        {
            get { return confGroups.Count; }
        }

        /// <summary>
        /// ��ȡָ����ŵĻ���
        /// </summary>
        public List<int> GetConf(int confGroup)
        {
            if (confGroups.ContainsKey(confGroup))
                return confGroups[confGroup];
            return null;
        }

        #endregion

        #region Assistant function

        /// <summary>
        /// ��ʼ���忨�ϵ�ͨ������Ϣ
        /// </summary>
        private void CreateChannels()
        {
            if (channels == null)
            {
                lock (syncObj)
                {
                    if (channels == null)
                    {
                        channels = new ChannelCollection();
                        Initialize();
                    }
                }
            }
        }

        /// <summary>
        /// �������б�����û���ķ�����ʵ����Channels��Ա
        /// </summary>
        /// <returns></returns>
        protected virtual long Initialize()
        {
            return 0;
        }

        #endregion
        
        #region IDisposable Members

        public void Close()
        {
            Dispose();
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
                    if (confGroups != null) // �ͷ����������ռ�õ�ϵͳ��Դ
                        foreach (List<int> confs in confGroups.Values)
                        {
                            foreach (int chnlid in confs)
                            {
                                if (Channels[chnlid] != null)
                                    Channels[chnlid].UnLink();
                            }
                        }
                    disposed = true;
                }
            }
        }

        ~AbstractCTIDriver()
        {
            Dispose(false);
        }

        #endregion

        #region IConfigurable Members

        public virtual void Configuration(Uniframework.Services.IConfiguration config)
        { 
        }

        #endregion
    }
}
