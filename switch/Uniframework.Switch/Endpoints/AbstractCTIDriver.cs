using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using Uniframework;
using Uniframework.Services;

namespace Uniframework.Switch
{
    /// <summary>
    /// 媒体终端接入设备基类
    /// </summary>
    public abstract class AbstractCTIDriver : ICTIDriver, IConfigurable, IDisposable
    {
        private readonly static int DefaultTimeout = 30; // 默认超时值为30秒
        private readonly static object syncObj = new object();

        private string key = string.Empty;
        private IVersionInfo versionInfo = new VersionInfo("CTICarddriver");
        protected Dictionary<int, List<int>> confGroups; // 会议资源

        private bool active = false;
        private bool canWork = false;
        private int timeout = DefaultTimeout; 
        private ChannelCollection channels = null;

        protected WorkItem workItem;
        protected ILog logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="workItem">组件容器</param>
        public AbstractCTIDriver(WorkItem workItem)
        {
            this.workItem = workItem;
            this.logger = this.workItem.Services.Get<ILog>();
            channels = null;
            confGroups = new Dictionary<int, List<int>>();
        }

        /// <summary>
        /// 日志记录组件
        /// </summary>
        public ILog Logger
        {
            get { return logger; }
        }

        #region ICTICardDriver Common Events

        // 通道创建事件，当创建一条通道时系统会将创建信息通知外部应用程序，以便有机会进行相关的处理
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

        /// <summary>
        /// 名字
        /// </summary>
        /// <value></value>
        public string Key
        {
            get { return key; }
            protected set { key = value; }
        }

        /// <summary>
        /// 板卡适配器的版本信息
        /// </summary>
        /// <value></value>
        public IVersionInfo VersionInfo
        {
            get { return versionInfo; }
        }

        /// <summary>
        /// 超时值
        /// </summary>
        /// <value></value>
        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        /// <summary>
        /// 工作项，本地容器
        /// </summary>
        /// <value></value>
        public WorkItem WorkItem
        {
            get { return workItem; }
        }

        /// <summary>
        /// 激活/关闭板卡适配器
        /// </summary>
        /// <value></value>
        public bool Active
        {
            get { return active; }
            set 
            {
                CreateChannels();
                //// 激活或关闭所有的事件分配器
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

        /// <summary>
        /// 用于标识当前板卡适配器可否工作
        /// </summary>
        /// <value></value>
        public bool CanWork
        {
            get { return canWork; }
            protected set
            {
                canWork = value;
            }
        }

        /// <summary>
        /// 当前适配器初始化的所有通道
        /// </summary>
        /// <value></value>
        public ChannelCollection Channels
        {
            get 
            {
                CreateChannels();
                return channels; 
            }
        }

        /// <summary>
        /// 通道数
        /// </summary>
        /// <value></value>
        public int ChannelCount
        {
            get 
            {
                CreateChannels();
                return channels.Count;
            }
        }

        /// <summary>
        /// 获取指定类型或状态的通道
        /// </summary>
        /// <param name="chnlid">通道标识</param>
        /// <param name="chnlType">通道类型</param>
        /// <param name="chnlSttatus">通道状态</param>
        /// <returns>
        /// 若板卡适配器可用并指定了相应的通道标识则直接返回相应的通道，否则查找指定类型及状态的通道
        /// </returns>
        public abstract IChannel GetChannel(int chnlid, ChannelType chnlType, ChannelStatus chnlSttatus);

        /// <summary>
        /// 加入会议
        /// </summary>
        /// <param name="conf">会议组号</param>
        /// <param name="chnl">语音通道</param>
        /// <seealso cref="LeaveConf"/>
        /// <seealso cref="ConfCount"/>
        /// <seealso cref="GetConf"/>
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
        /// 离开会议
        /// </summary>
        /// <param name="conf">会议组号</param>
        /// <param name="chnl">语音通道</param>
        /// <seealso cref="JoinConf"/>
        /// <seealso cref="ConfCount"/>
        /// <seealso cref="GetConf"/>
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
        /// 系统当前创建的会议组数
        /// </summary>
        /// <value></value>
        /// <seealso cref="JoinConf"/>
        /// <seealso cref="LeaveConf"/>
        /// <seealso cref="GetConf"/>
        public int ConfCount
        {
            get { return confGroups.Count; }
        }

        /// <summary>
        /// 获取指定组号的会议
        /// </summary>
        /// <param name="confGroup">会议组号</param>
        /// <returns>返回指定组号的会议，如果不存在的话则返回null</returns>
        /// <remarks>Modified By JackyXU 2007-01-12</remarks>
        /// <seealso cref="JoinConf"/>
        /// <seealso cref="LeaveConf"/>
        /// <seealso cref="ConfCount"/>
        public List<int> GetConf(int confGroup)
        {
            if (confGroups.ContainsKey(confGroup))
                return confGroups[confGroup];
            return null;
        }

        #endregion

        #region Assistant function

        /// <summary>
        /// 初始化板卡上的通道等信息
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
        /// 在子类中必须调用基类的方法以实例化Channels成员
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
                    if (confGroups != null) // 释放因加入会议而占用的系统资源
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

        /// <summary>
        /// Configurations the specified config.
        /// </summary>
        /// <param name="config">The config.</param>
        public virtual void Configuration(IConfiguration config)
        { 
        }

        #endregion
    }
}
