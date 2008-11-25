using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch.TTS
{
    public class TTS3Wrapper
    {
        private static TTS3Wrapper instance;
        private static object syncObj = new object();

        protected TTS3Wrapper()
        { }

        public static TTS3Wrapper Instance()
        {
            if (instance == null)
            {
                lock (syncObj)
                {
                    if (instance == null)
                    {
                        instance = new TTS3Wrapper();
                    }
                }
            }
            return instance;
        }

        #region TTS3 Wrapper

        public Int32 DJTTS3_Init()
        {
            lock (syncObj)
            {
                return TTS3.DJTTS3_Init();
            }
        }

        public Int32 DJTTS3_GetTotalTTSChannel()
        {
            lock (syncObj)
            {
                return TTS3.DJTTS3_GetTotalTTSChannel();
            }
        }

        public Int32 DJTTS3_GetLeaveTTSChannel()
        {
            lock (syncObj)
            {
                return TTS3.DJTTS3_GetLeaveTTSChannel();
            }
        }

        public Int32 DJTTS3_AddTTSToChannel(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return TTS3.DJTTS3_AddTTSToChannel(wChnlNo);
            }
        }

        public Int32 DelTTSFromChannel(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return TTS3.DelTTSFromChannel(wChnlNo);
            }
        }

        public Int32 DJTTS3_StartPlayText(Int32 wChnlNo, byte[] pText, Int32 InputType, Int32 iVoice, Int32 iSpeed, Int32 iVolume, Int32 iLableFlag)
        {
            lock (syncObj)
            {
                return TTS3.DJTTS3_StartPlayText(wChnlNo, pText, InputType, iVoice, iSpeed, iVolume, iLableFlag);
            }
        }

        public Int32 DJTTS3_CheckPlayTextEnd(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return TTS3.DJTTS3_CheckPlayTextEnd(wChnlNo);
            }
        }

        public Int32 DJTTS3_StopPlayText(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return TTS3.DJTTS3_StopPlayText(wChnlNo);
            }
        }

        public void DJTTS3_Release()
        {
            lock (syncObj)
            {
                TTS3.DJTTS3_Release();
            }
        }

        public Int32 DJTTS3_StartAudioPlayText(byte[] pText, Int32 InputType, Int32 iVoice, Int32 iSpeed, Int32 iVolume, Int32 iLableFlag)
        {
            lock (syncObj)
            {
                return TTS3.DJTTS3_StartAudioPlayText(pText, InputType, iVoice, iSpeed, iVolume, iLableFlag);
            }
        }

        public Int32 DJTTS3_StopAudioPlayText()
        {
            lock (syncObj)
            {
                return DJTTS3_StopAudioPlayText();
            }
        }

        public Int32 DJTTS3_CheckAudioPlayText()
        {
            lock (syncObj)
            {
                return DJTTS3_CheckAudioPlayText();
            }
        }

        public long DJTTS3_CvtTextToVocFile(byte[] text, Int32 voice, byte[] fileName, Int32 chnl, Int32 speed, Int32 volume, Int32 lableFlag)
        {
            lock (syncObj)
            {
                return DJTTS3_CvtTextToVocFile(text, voice, fileName, chnl, speed, volume, lableFlag);
            }
        }

        public long DJTTS3_CvtTextToWaveFile(byte[] text, Int32 voice, byte[] fileName, Int32 chnl, Int32 speed, Int32 volume, Int32 lableFlag)
        {
            lock (syncObj)
            {
                return DJTTS3_CvtTextToWaveFile(text, voice, fileName, chnl, speed, volume, lableFlag);
            }
        }

        public long DJTTS3_CvtTextFileToVocFile(byte[] text, Int32 voice, byte[] fileName, Int32 chnl, Int32 speed, Int32 volume, Int32 lableFlag)
        {
            lock (syncObj)
            {
                return TTS3.DJTTS3_CvtTextToVocFile(text, voice, fileName, chnl, speed, volume, lableFlag);
            }
        }

        public long DJTTS3_CvtTextFileToWaveFile(byte[] text, Int32 voice, byte[] fileName, Int32 chnl, Int32 speed, Int32 volume, Int32 lableFlag)
        {
            lock (syncObj)
            {
                return TTS3.DJTTS3_CvtTextFileToWaveFile(text, voice, fileName, chnl, speed, volume, lableFlag);
            }
        }

        public long DJTTS3_CheckCvtEnd(Int32 chnl)
        {
            lock (syncObj)
            {
                return TTS3.DJTTS3_CheckCvtEnd(chnl);
            }
        }

        public long DJTTS3_CvtStop(Int32 chnl)
        {
            lock (syncObj)
            {
                return TTS3.DJTTS3_CvtStop(chnl);
            }
        }

        #endregion
    }
}
