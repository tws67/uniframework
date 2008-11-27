using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using log4net;

using Microsoft.Practices.CompositeUI;
using Uniframework.Services;
using Uniframework.Switch;
using Uniframework.Switch.Endpoints.DB160X;

namespace Uniframework.Switch.TTS.DJTTS3
{
    /// <summary>
    /// ����TTS����
    /// </summary>
    public class DJTTS3Engine : AbstractTTSEngine, IDisposable
    {
        private readonly static Int32 Defaultdelay = 425;  // ��ش����ӳ�ʱ�䣨���룩
        private Int32 ttsChannelCount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="DJTTS3Engine"/> class.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <param name="threadNumber">The thread number.</param>
        public DJTTS3Engine(WorkItem workItem, Int32 threadNumber)
            : base(workItem, threadNumber)
        {
            ICTIDriver driver = workItem.Items.Get<ICTIDriver>("DB160X"); // ͨ�����������ֻ�ȡ�������Ķ���
            if (driver == null)
            {
                Logger.Fatal("û�г�ʼ����ȷ�İ忨����������TTS��Ҫ�붫�������������ʹ��");
                throw new Exception("����TTS����������������İ忨�����������ļ������ԡ�");
            }

            bool driverStatus = driver.Active;
            Logger.Info("׼����ʼ������TTS���桭��");
            try
            {
                canWork = false;
                driver.Active = true;
                Int32 initSucess = TTS3.DJTTS3_Init();
                if (initSucess > 0)
                {
                    ttsChannelCount = TTS3.DJTTS3_GetTotalTTSChannel();
                    Int32 j = 0;
                    for (short i = 0; i < driver.ChannelCount; i++)
                    {
                        // ֻ������ͨ������TTS����
                        if (driver.Channels[i].ChannelType == ChannelType.TRUNK)
                            try
                            {
                                TTS3.DJTTS3_AddTTSToChannel(i);
                                Logger.Info("Ϊ " + driver.VersionInfo.Name + " �ĵ� " + i.ToString() + " ���������TTS���ܣ��ɹ�");
                                // ����Ƿ�ʹ������õ�TTSͨ����
                                j++;
                                if (j == ttsChannelCount)
                                    break;
                            }
                            catch
                            {
                                Logger.Error("Ϊ " + driver.VersionInfo.Name + " �ĵ� " + i.ToString() + " ���������TTS���ܣ�ʧ��");
                                continue;
                            }
                    }
                    canWork = true;
                    Logger.Info("��ɶ���TTS�����ʼ������");
                }
                else
                    Logger.Info("��ʼ������TTS����ʧ�ܣ��������Ϊ: " + initSucess.ToString());
            }
            catch (Exception ex)
            {
                Logger.Error("��ʼ������TTS����ʱ��������", ex);
            }
            finally
            {
                driver.Active = driverStatus;
            }
        }

        /// <summary>
        /// Plays the message.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="text">The text.</param>
        /// <param name="allowBreak">if set to <c>true</c> [allow break].</param>
        /// <param name="playType">Type of the play.</param>
        /// <returns></returns>
        public override SwitchStatus PlayMessage(IChannel channel, string text, bool allowBreak, TTSPlayType playType)
        {
            if (!canWork)
            {
                Logger.Warn("��TTS�����ʼ��ʧ�ܻ�����ԭ����ɲ����ã����ܽ���TTS����");
                return SwitchStatus.FAILURE;
            }

            if (channel == null)
            {
                Logger.Warn("����ͨ������δ��ʼ�����ܽ���TTS����");
                return SwitchStatus.FAILURE;
            }

            lock (D160X.SyncObj)
            {
                Logger.Info(string.Format("ͨ�� {0} ׼��TTS��������������Ϊ��{1}", channel.ChannelID, text));
                Int32 Playflag = TTS3.DJTTS3_StartPlayText(channel.ChannelID, Encoding.UTF8.GetBytes(text), TTS3.INFO_TEXT_BUFFER,
                    0, 45, 50, XmlTag ? TTS3.INFO_USE_LABLE : TTS3.INFO_NOTUSE_LABLE);
                Logger.Debug("TTS�������Ϊ��" + Playflag.ToString());
                D160X.InitDtmfBuf(channel.ChannelID);
            }

            // �ڷ������Ҫ���ϵļ�����Ƿ��Ѿ��������
            Int32 Playend = TTS3.INFO_PLAY_NOT_COMPLATE;
            while(Playend == TTS3.INFO_PLAY_NOT_COMPLATE)
            {
                lock (D160X.SyncObj)
                {
                    D160X.PUSH_PLAY();
                }
                System.Threading.Thread.Sleep(Defaultdelay);

                bool dtmfHited = false;
                lock (D160X.SyncObj)
                {
                    dtmfHited = D160X.DtmfHit(channel.ChannelID);
                }
                if (allowBreak && dtmfHited)
                {
                    lock (D160X.SyncObj)
                    {
                        TTS3.DJTTS3_StopPlayText(channel.ChannelID);
                    }
                    Logger.Info(string.Format("ͨ�� {0} TTS�������̱��Է������ж�", channel.ChannelID));
                    return SwitchStatus.BREAK;
                }

                if (channel.HangUpDetect())
                {
                    lock (D160X.SyncObj)
                    {
                        TTS3.DJTTS3_StopPlayText(channel.ChannelID);
                    }
                    Logger.Info(string.Format("ͨ�� {0} TTS�������̱��Է��һ��ж�", channel.ChannelID));
                    channel.ResetChannel();
                    return SwitchStatus.BREAK;
                }

                lock (D160X.SyncObj)
                {
                    Playend = TTS3.DJTTS3_CheckPlayTextEnd(channel.ChannelID);
                }
            }

            lock (D160X.SyncObj)
            {
                TTS3.DJTTS3_StopPlayText(channel.ChannelID);
            }
            Logger.Info(string.Format("ͨ�� {0} TTS��������", channel.ChannelID));
            return SwitchStatus.SUCCESS;
        }

        /// <summary>
        /// Plays to file.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="text">The text.</param>
        /// <param name="playType">Type of the play.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public override bool PlayToFile(IChannel channel, string text, TTSPlayType playType, string fileName)
        {
            if (!canWork)
            {
                Logger.Warn("��TTS�����ʼ��ʧ�ܻ�����ԭ����ɲ����ã����ܽ���TTS����");
                return false;
            }

            if (channel == null)
            {
                Logger.Warn("����ͨ������δ��ʼ�����ܽ���TTS����");
                return false;
            }

            string dir = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string fileext = Path.GetExtension(fileName);
            lock (D160X.SyncObj)
            {
                Logger.Info(string.Format("ͨ�� {0} ����TTSת������ {1} �������ļ� \"{2}\"", channel.ChannelID, text, fileName));
                // �����ļ���չ��ת��Ϊ��ͬ��ʽ�������ļ�
                switch (fileext.ToLower())
                {
                    case ".voc":
                        TTS3.DJTTS3_CvtTextToVocFile(Encoding.UTF8.GetBytes(text), 50, Encoding.UTF8.GetBytes(fileName), channel.ChannelID,
                            50, 50, XmlTag ? TTS3.INFO_USE_LABLE : TTS3.INFO_NOTUSE_LABLE);
                        break;

                    case ".wav":
                        TTS3.DJTTS3_CvtTextToWaveFile(Encoding.UTF8.GetBytes(text), 50, Encoding.UTF8.GetBytes(fileName), channel.ChannelID,
                            50, 50, XmlTag ? TTS3.INFO_USE_LABLE : TTS3.INFO_NOTUSE_LABLE);
                        break;

                    default:
                        TTS3.DJTTS3_CvtTextToVocFile(Encoding.UTF8.GetBytes(text), 50, Encoding.UTF8.GetBytes(fileName), channel.ChannelID,
                            50, 50, XmlTag ? TTS3.INFO_USE_LABLE : TTS3.INFO_NOTUSE_LABLE);
                        break;
                }

            }

            return true;
        }

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

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                ICTIDriver driver = workItem.Services.Get<ICTIDriver>();
                if (driver != null)
                {
                    // ��ͨ����ȥ��TTS����
                    for (short i = 0; i < driver.ChannelCount; i++)
                    {
                        if (driver.Channels[i].ChannelType == ChannelType.TRUNK)
                            TTS3.DelTTSFromChannel(i);
                    }
                }

                TTS3.DJTTS3_Release();
                disposed = true;
            }
        }

        ~DJTTS3Engine()
        {
            Dispose(false);
        }

        #endregion
    }
}
