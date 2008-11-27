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
    /// 东进TTS引擎
    /// </summary>
    public class DJTTS3Engine : AbstractTTSEngine, IDisposable
    {
        private readonly static Int32 Defaultdelay = 425;  // 相关处理延迟时间（毫秒）
        private Int32 ttsChannelCount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="DJTTS3Engine"/> class.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <param name="threadNumber">The thread number.</param>
        public DJTTS3Engine(WorkItem workItem, Int32 threadNumber)
            : base(workItem, threadNumber)
        {
            ICTIDriver driver = workItem.Items.Get<ICTIDriver>("DB160X"); // 通过适配器名字获取其依赖的对象
            if (driver == null)
            {
                Logger.Fatal("没有初始化正确的板卡适配器，此TTS需要与东进的语音卡配合使用");
                throw new Exception("东进TTS必须依赖于其自身的板卡，请检查配置文件后再试。");
            }

            bool driverStatus = driver.Active;
            Logger.Info("准备初始化东进TTS引擎……");
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
                        // 只对外线通道附加TTS功能
                        if (driver.Channels[i].ChannelType == ChannelType.TRUNK)
                            try
                            {
                                TTS3.DJTTS3_AddTTSToChannel(i);
                                Logger.Info("为 " + driver.VersionInfo.Name + " 的第 " + i.ToString() + " 条外线添加TTS功能，成功");
                                // 检查是否使用完可用的TTS通道数
                                j++;
                                if (j == ttsChannelCount)
                                    break;
                            }
                            catch
                            {
                                Logger.Error("为 " + driver.VersionInfo.Name + " 的第 " + i.ToString() + " 条外线添加TTS功能，失败");
                                continue;
                            }
                    }
                    canWork = true;
                    Logger.Info("完成东进TTS引擎初始化工作");
                }
                else
                    Logger.Info("初始化东进TTS引擎失败，错误代码为: " + initSucess.ToString());
            }
            catch (Exception ex)
            {
                Logger.Error("初始化东进TTS引擎时发生错误", ex);
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
                Logger.Warn("因TTS引擎初始化失败或其它原因造成不可用，不能进行TTS放音");
                return SwitchStatus.FAILURE;
            }

            if (channel == null)
            {
                Logger.Warn("传入通道参数未初始化不能进行TTS放音");
                return SwitchStatus.FAILURE;
            }

            lock (D160X.SyncObj)
            {
                Logger.Info(string.Format("通道 {0} 准备TTS放音，放音内容为：{1}", channel.ChannelID, text));
                Int32 Playflag = TTS3.DJTTS3_StartPlayText(channel.ChannelID, Encoding.UTF8.GetBytes(text), TTS3.INFO_TEXT_BUFFER,
                    0, 45, 50, XmlTag ? TTS3.INFO_USE_LABLE : TTS3.INFO_NOTUSE_LABLE);
                Logger.Debug("TTS放音结果为：" + Playflag.ToString());
                D160X.InitDtmfBuf(channel.ChannelID);
            }

            // 在放音后就要不断的检查其是否已经播放完毕
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
                    Logger.Info(string.Format("通道 {0} TTS放音过程被对方按键中断", channel.ChannelID));
                    return SwitchStatus.BREAK;
                }

                if (channel.HangUpDetect())
                {
                    lock (D160X.SyncObj)
                    {
                        TTS3.DJTTS3_StopPlayText(channel.ChannelID);
                    }
                    Logger.Info(string.Format("通道 {0} TTS放音过程被对方挂机中断", channel.ChannelID));
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
            Logger.Info(string.Format("通道 {0} TTS放音结束", channel.ChannelID));
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
                Logger.Warn("因TTS引擎初始化失败或其它原因造成不可用，不能进行TTS放音");
                return false;
            }

            if (channel == null)
            {
                Logger.Warn("传入通道参数未初始化不能进行TTS放音");
                return false;
            }

            string dir = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string fileext = Path.GetExtension(fileName);
            lock (D160X.SyncObj)
            {
                Logger.Info(string.Format("通道 {0} 利用TTS转换文字 {1} 到语音文件 \"{2}\"", channel.ChannelID, text, fileName));
                // 根据文件扩展名转换为不同格式的语音文件
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
                    // 从通道中去除TTS功能
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
