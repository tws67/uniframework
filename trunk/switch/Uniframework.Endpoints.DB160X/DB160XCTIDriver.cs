using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Uniframework.Services;
using Uniframework.SmartClient;

namespace Uniframework.Switch.Endpoints.DB160X
{
    /// <summary>
    /// 东进DJDBDK开发包之D160X模拟卡适配器，适合于D081X、D0160X系列的模拟卡
    /// </summary>
    public class DB160XCTIDriver : AbstractCTIDriver, IDisposable
    {
        private readonly int FileBufferLength = 1024 * 16; // 语音播放缓冲大小为128K

        public DB160XCTIDriver(WorkItem workItem)
            : base(workItem)
        {
            this.Key = "DB160X";

            this.VersionInfo.Name = "东进D系列模拟卡";
            this.VersionInfo.Description = "此驱动适合于所有D系列的模拟卡，要求安装DJDBDK 3.3.2以上的驱动程序。";
            this.VersionInfo.Version = "3.3.2";
        }

        public override void Configuration(IConfiguration config)
        {
            // 初始化TTS引擎
            IConfiguration tts = config.Children["TTSEngine"];
            if (tts != null)
            {
                Type ttsType = Type.GetType(tts.Attributes["type"]);
                if (tts != null && ttsType != null)
                    try
                    {
                        int threadNumber = int.Parse(tts.Attributes["threadnumber"]);
                        ITTSEngine ttsEngine = Activator.CreateInstance(ttsType, new object[] { workItem, threadNumber }) as AbstractTTSEngine;
                        workItem.Services.Add<ITTSEngine>(ttsEngine);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(String.Format("初始化 {0} TTS引擎失败", tts.Attributes["name"]), ex);
                        throw ex;
                    }
            }

            // 初始化所有的通道对象
            foreach (IConfiguration conf in config.Children["Channels"].Children)
            {
                int chnlid = int.Parse(conf.Attributes["id"]);
                if (chnlid >= 0 && chnlid < ChannelCount)
                {
                    IChannel chnl = Channels[chnlid];
                    if (chnl != null)
                    {
                        logger.Info("配置第 " + chnl.ChannelID.ToString() + " 条通道 : ");
                        chnl.ChannelAlias = conf.Attributes["alias"];
                        chnl.HitTimeout = int.Parse(conf.Attributes["HitTimeout"]);
                        chnl.DetectPolarity = bool.Parse(conf.Attributes["DetectPolarity"]);

                        // 注册脚本处理程序
                        if (conf.Children.Count > 0 && conf.Children["Scripts"].Children.Count > 0)
                            foreach (IConfiguration script in conf.Children["Scripts"].Children)
                            {
                                if (script.Name.ToLower() == "add")
                                {
                                    if ((script.Attributes["EventHandler"] != null || script.Attributes["EventHandler"] != string.Empty) && (script.Attributes["filename"] != null || script.Attributes["filename"] != string.Empty))
                                    {
                                        logger.Info("注册脚本处理程序，注册的事件名为 " + script.Attributes["EventHandler"] + " ，脚本文件为 \"" + script.Attributes["filename"] + "\"");
                                        chnl.RegisgerScript(script.Attributes["EventHandler"], script.Attributes["filename"]);
                                    }
                                }
                            }
                        chnl.Enable = bool.Parse(conf.Attributes["enable"]); // 启用或禁用通道
                    }
                }
            }
            logger.Info("完成板卡适配器下的所有通道资源的初始化工作");
            Active = bool.Parse(config.Attributes["active"]); // 激活板卡适配器
        }

        #region Assistant function

        /// <summary>
        /// 加载板卡驱动程序并初始化当前板卡下的所有通道
        /// </summary>
        /// <returns>如果初始成功则返回0，否则返回相应的错误编码</returns>
        protected override long Initialize() 
        {
            base.Initialize(); // 执行基类的初始化工作

            // 添加呼入事件分配器
            Logger.Info("初始化板卡内置的事件分配器 Call……");
            IEventService eventService = workItem.Services.Get<IEventService>();
            if (eventService != null)
            {
                eventService.RegisterEventDispatcher("Call", new CallDispatcher(typeof(ICallHandler), typeof(CallDispatcher), this));
            }

            Logger.Info(string.Format("准备初始化{0}板卡适配器……", VersionInfo.Name));
            long loadDriverSucces = -1;
            try
            {
                loadDriverSucces = D160X.LoadDRV();
                if (loadDriverSucces != -1)
                {
                    CanWork = true;
                }
                else
                    return loadDriverSucces;
            }
            catch(Exception ex)
            {
                Logger.Fatal(string.Format("加载{0}驱动程序失败，错误代码为：{1}", VersionInfo.Name, loadDriverSucces), ex);
            }

            // 初始化板卡的会议资源
            int Initconf = D160X.DConf_EnableConfCard();
            string[] InitconfResult = new string[] {"成功", "不是D161A卡", "在INI中，Connect参数必须设置为1", "已经使用了模拟的会议卡，并且初始化成功"};
            Logger.Info(String.Format("初始化{0}的会议资源，{1}", VersionInfo.Name, InitconfResult[Initconf]));
            if (Initconf == 0)
            {
                confGroups = new Dictionary<int, List<int>>();
            }

            D160X.Sig_Init(0); // 初始化信号音检测
            int chnlCount = D160X.CheckValidCh();
            for (int i = 0; i < chnlCount; i++)
            {
                // 初始化每条通道
                AbstractChannel chnl = new DB160XChannel(this, i);
                if (chnl != null)
                {
                    ISubscripterRegister subscripter = new CallSubscripter(chnl); // 预定义的呼叫事件订阅器
                    if (eventService != null)
                        eventService.RegisterSubscripter(subscripter);

                    Channels.Add(chnl);
                    chnl.CurrentStatus = ChannelStatus.IDLE;
                    OnCreatedChannel(this, new EventArgs<IChannel>(chnl)); // 触发通道创建事件
                }
            }

            D160X.EnableCard(chnlCount, FileBufferLength);
            int initFax = D160X.DJFax_DriverReady(2048);
            if (initFax == 0)
            {
                Logger.Info("初始化传真资源，成功");
            }
            else
                Logger.Info("因为不是传真卡或配置不正确初始化板卡的传真资源失败，返回的错误代码: " + initFax.ToString());
            Logger.Info(string.Format("完成{0}板卡适配器的初始化工作", VersionInfo.Name));

            return loadDriverSucces;
        }

        public override IChannel GetChannel(int chnlid, ChannelType chnlType, ChannelStatus chnlStatus)
        {
            if (CanWork)
            {
                if (chnlid != -1 && Channels[chnlid].ChannelType == chnlType && Channels[chnlid].CurrentStatus == chnlStatus)
                {
                    return Channels[chnlid];
                }

                // 直接返回指定通道标识的通道
                if (chnlid != -1 && chnlStatus == ChannelStatus.IDLE)
                {
                    return chnlid < ChannelCount ? Channels[chnlid] : null;
                }

                // 获取指定类型及状态的通道
                foreach (IChannel channle in Channels)
                {
                    if (channle.ChannelType == chnlType && channle.CurrentStatus == chnlStatus)
                        return channle;
                }
            }
            return null;
        }

        #endregion

        #region IDisposable Members

        public void Close()
        {
            Dispose();
        }

        new public void Dispose()
        {
            Dispose(true);
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        private bool disposed = false;

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // 关闭所有事件分配器
                    Active = false;
                    System.Threading.Thread.Sleep(5000);
                    logger.Info(String.Format("{0}板卡适配器从系统中卸载成功。\n", VersionInfo.Name));
                }

                // Release unmanaged resources
                D160X.DConf_DisableConfCard();
                D160X.DisableCard();
                D160X.FreeDRV();
                D160X.DJFax_DisableCard();

                disposed = true;
            }
            base.Dispose(disposing);
        }

        ~DB160XCTIDriver()
        {
            Dispose(false);
        }

        #endregion
    }
}
