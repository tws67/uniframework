using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Uniframework.Services;
using Uniframework.SmartClient;

namespace Uniframework.Switch.Endpoints.DB160X
{
    /// <summary>
    /// ����DJDBDK������֮D160Xģ�⿨���������ʺ���D081X��D0160Xϵ�е�ģ�⿨
    /// </summary>
    public class DB160XCTIDriver : AbstractCTIDriver, IDisposable
    {
        private readonly int FileBufferLength = 1024 * 16; // �������Ż����СΪ128K

        public DB160XCTIDriver(WorkItem workItem)
            : base(workItem)
        {
            this.Key = "DB160X";

            this.VersionInfo.Name = "����Dϵ��ģ�⿨";
            this.VersionInfo.Description = "�������ʺ�������Dϵ�е�ģ�⿨��Ҫ��װDJDBDK 3.3.2���ϵ���������";
            this.VersionInfo.Version = "3.3.2";
        }

        public override void Configuration(IConfiguration config)
        {
            // ��ʼ��TTS����
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
                        logger.Error(String.Format("��ʼ�� {0} TTS����ʧ��", tts.Attributes["name"]), ex);
                        throw ex;
                    }
            }

            // ��ʼ�����е�ͨ������
            foreach (IConfiguration conf in config.Children["Channels"].Children)
            {
                int chnlid = int.Parse(conf.Attributes["id"]);
                if (chnlid >= 0 && chnlid < ChannelCount)
                {
                    IChannel chnl = Channels[chnlid];
                    if (chnl != null)
                    {
                        logger.Info("���õ� " + chnl.ChannelID.ToString() + " ��ͨ�� : ");
                        chnl.ChannelAlias = conf.Attributes["alias"];
                        chnl.HitTimeout = int.Parse(conf.Attributes["HitTimeout"]);
                        chnl.DetectPolarity = bool.Parse(conf.Attributes["DetectPolarity"]);

                        // ע��ű��������
                        if (conf.Children.Count > 0 && conf.Children["Scripts"].Children.Count > 0)
                            foreach (IConfiguration script in conf.Children["Scripts"].Children)
                            {
                                if (script.Name.ToLower() == "add")
                                {
                                    if ((script.Attributes["EventHandler"] != null || script.Attributes["EventHandler"] != string.Empty) && (script.Attributes["filename"] != null || script.Attributes["filename"] != string.Empty))
                                    {
                                        logger.Info("ע��ű��������ע����¼���Ϊ " + script.Attributes["EventHandler"] + " ���ű��ļ�Ϊ \"" + script.Attributes["filename"] + "\"");
                                        chnl.RegisgerScript(script.Attributes["EventHandler"], script.Attributes["filename"]);
                                    }
                                }
                            }
                        chnl.Enable = bool.Parse(conf.Attributes["enable"]); // ���û����ͨ��
                    }
                }
            }
            logger.Info("��ɰ忨�������µ�����ͨ����Դ�ĳ�ʼ������");
            Active = bool.Parse(config.Attributes["active"]); // ����忨������
        }

        #region Assistant function

        /// <summary>
        /// ���ذ忨�������򲢳�ʼ����ǰ�忨�µ�����ͨ��
        /// </summary>
        /// <returns>�����ʼ�ɹ��򷵻�0�����򷵻���Ӧ�Ĵ������</returns>
        protected override long Initialize() 
        {
            base.Initialize(); // ִ�л���ĳ�ʼ������

            // ��Ӻ����¼�������
            Logger.Info("��ʼ���忨���õ��¼������� Call����");
            IEventService eventService = workItem.Services.Get<IEventService>();
            if (eventService != null)
            {
                eventService.RegisterEventDispatcher("Call", new CallDispatcher(typeof(ICallHandler), typeof(CallDispatcher), this));
            }

            Logger.Info(string.Format("׼����ʼ��{0}�忨����������", VersionInfo.Name));
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
                Logger.Fatal(string.Format("����{0}��������ʧ�ܣ��������Ϊ��{1}", VersionInfo.Name, loadDriverSucces), ex);
            }

            // ��ʼ���忨�Ļ�����Դ
            int Initconf = D160X.DConf_EnableConfCard();
            string[] InitconfResult = new string[] {"�ɹ�", "����D161A��", "��INI�У�Connect������������Ϊ1", "�Ѿ�ʹ����ģ��Ļ��鿨�����ҳ�ʼ���ɹ�"};
            Logger.Info(String.Format("��ʼ��{0}�Ļ�����Դ��{1}", VersionInfo.Name, InitconfResult[Initconf]));
            if (Initconf == 0)
            {
                confGroups = new Dictionary<int, List<int>>();
            }

            D160X.Sig_Init(0); // ��ʼ���ź������
            int chnlCount = D160X.CheckValidCh();
            for (int i = 0; i < chnlCount; i++)
            {
                // ��ʼ��ÿ��ͨ��
                AbstractChannel chnl = new DB160XChannel(this, i);
                if (chnl != null)
                {
                    ISubscripterRegister subscripter = new CallSubscripter(chnl); // Ԥ����ĺ����¼�������
                    if (eventService != null)
                        eventService.RegisterSubscripter(subscripter);

                    Channels.Add(chnl);
                    chnl.CurrentStatus = ChannelStatus.IDLE;
                    OnCreatedChannel(this, new EventArgs<IChannel>(chnl)); // ����ͨ�������¼�
                }
            }

            D160X.EnableCard(chnlCount, FileBufferLength);
            int initFax = D160X.DJFax_DriverReady(2048);
            if (initFax == 0)
            {
                Logger.Info("��ʼ��������Դ���ɹ�");
            }
            else
                Logger.Info("��Ϊ���Ǵ��濨�����ò���ȷ��ʼ���忨�Ĵ�����Դʧ�ܣ����صĴ������: " + initFax.ToString());
            Logger.Info(string.Format("���{0}�忨�������ĳ�ʼ������", VersionInfo.Name));

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

                // ֱ�ӷ���ָ��ͨ����ʶ��ͨ��
                if (chnlid != -1 && chnlStatus == ChannelStatus.IDLE)
                {
                    return chnlid < ChannelCount ? Channels[chnlid] : null;
                }

                // ��ȡָ�����ͼ�״̬��ͨ��
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
                    // �ر������¼�������
                    Active = false;
                    System.Threading.Thread.Sleep(5000);
                    logger.Info(String.Format("{0}�忨��������ϵͳ��ж�سɹ���\n", VersionInfo.Name));
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
