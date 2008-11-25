using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch.Endpoints.DB160X
{
    /// <summary>
    /// D160X包装器，为其相关的方法提供线程安全特性
    /// </summary>
    public class D160XWrapper
    {
        private static D160XWrapper instance;
        private static object syncObj = new object();

        protected D160XWrapper()
        {
        }

        public static D160XWrapper Intance()
        {
            if (instance == null)
            {
                lock (syncObj)
                {
                    if (instance == null)
                    {
                        instance = new D160XWrapper();
                    }
                }
            }
            return instance;
        }

        #region D160X Wrapper

        #region 系统初始化函数

        public long LoadDRV()
        {
            lock (syncObj)
            {
                return D160X.LoadDRV();
            }
        }

        public void FreeDRV()
        {
            lock (syncObj)
            {
                D160X.FreeDRV();
            }
        }

        public long EnableCard(Int32 wusedCh, Int32 wFileBufLen)
        {
            lock (syncObj)
            {
                return EnableCard(wusedCh, wFileBufLen);
            }
        }

        public void DisableCard()
        {
            lock (syncObj)
            {
                D160X.DisableCard();
            }
        }

        public void GetSysInfo(ref TC_INI_TYPE TmpIni)
        {
            lock (syncObj)
            {
                D160X.GetSysInfo(ref TmpIni);
            }
        }

        public void GetSysInfoMore(ref TC_INI_TYPE_MORE TmpMore)
        {
            lock (syncObj)
            {
                D160X.GetSysInfoMore(ref TmpMore);
            }
        }

        public Int32 CheckValidCh()
        {
            lock (syncObj)
            {
                return D160X.CheckValidCh();
            }
        }

        public Int32 CheckChType(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return D160X.CheckChType(wChnlNo);
            }
        }

        public Int32 CheckChTypeNew(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return D160X.CheckChTypeNew(wChnlNo);
            }
        }

        public bool IsSupportCallerID()
        {
            lock (syncObj)
            {
                return D160X.IsSupportCallerID();
            }
        }

        public void SetPackRate(Int32 wPackRate)
        {
            lock (syncObj)
            {
                D160X.SetPackRate(wPackRate);
            }
        }

        public void PUSH_PLAY()
        {
            lock (syncObj)
            {
                D160X.PUSH_PLAY();
            }
        }

        public void SetBusyPara(Int32 BusyLen)
        {
            lock (syncObj)
            {
                D160X.SetBusyPara(BusyLen);
            }
        }

        public void SetDialPara(Int32 RingBack, Int32 RingBack0, Int32 BusyLen, Int32 RingTimes)
        {
            lock (syncObj)
            {
                D160X.SetDialPara(RingBack, RingBack0, BusyLen, RingTimes);
            }
        }

        public long NewReadPass(Int32 wCardNo)
        {
            lock (syncObj)
            {
                return D160X.NewReadPass(wCardNo);
            }
        }

        public void D_SetWorkMode(Int32 wChnlNo, byte cbWorkMode, byte cbModeVal)
        {
            lock (syncObj)
            {
                D160X.D_SetWorkMode(wChnlNo, cbWorkMode, cbModeVal);
            }
        }

        #endregion

        #region 振铃及摘挂机函数

        public bool RingDetect(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return RingDetect(wChnlNo);
            }
        }

        public void OffHook(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                D160X.OffHook(wChnlNo);
            }
        }

        public void HangUp(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                D160X.HangUp(wChnlNo);
            }
        }

        #endregion

        #region 放音函数

        public void StartPlay(Int32 wChnlNo, byte[] PlayBuf, Int32 dwStartPos, Int32 dwPlayLen)
        {
            lock (syncObj)
            {
                D160X.StartPlay(wChnlNo, PlayBuf, dwStartPos, dwPlayLen);
            }
        }

        public void StopPlay(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                D160X.StopPlay(wChnlNo);
            }
        }

        public bool CheckPlayEnd(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return D160X.CheckPlayEnd(wChnlNo);
            }
        }

        public void ResetIndex()
        {
            lock (syncObj)
            {
                D160X.ResetIndex();
            }
        }

        public Int32 SetIndex(byte[] VocBuf, Int32 dwVocLen)
        {
            lock (syncObj)
            {
                return D160X.SetIndex(VocBuf, dwVocLen);
            }
        }

        public bool StartPlayFile(Int32 wChnlNo, byte[] FileName, Int32 StartPos)
        {
            lock (syncObj)
            {
                return StartPlayFile(wChnlNo, FileName, StartPos);
            }
        }

        public void StopPlayFile(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                D160X.StopPlayFile(wChnlNo);
            }
        }

        public void RsetIndexPlayFile(Int32 Line)
        {
            lock (syncObj)
            {
                D160X.RsetIndexPlayFile(Line);
            }
        }

        public bool AddIndexPlayFile(Int32 Line, byte[] FileName)
        {
            lock (syncObj)
            {
                return D160X.AddIndexPlayFile(Line, FileName);
            }
        }

        public bool StartPlayIndex(Int32 wChnlNo, Int32[] pIndexTable, Int32 wIndexLen)
        {
            lock (syncObj)
            {
                return StartPlayIndex(wChnlNo, pIndexTable, wIndexLen);
            }
        }

        public bool CheckIndexPlayFile(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return CheckIndexPlayFile(wChnlNo);
            }
        }

        public void StopIndexPlayFile(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                D160X.StopIndexPlayFile(wChnlNo);
            }
        }

        public bool StartIndexPlayFile(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return D160X.StartIndexPlayFile(wChnlNo);
            }
        }

        #endregion

        #region 录音函数

        public bool StartRecordFile(Int32 wChnlNo, byte[] FileName, long dwRecordLen)
        {
            lock (syncObj)
            {
                return D160X.StartRecordFile(wChnlNo, FileName, dwRecordLen);
            }
        }

        public bool StartRecordFile_Ex(Int32 wChnlNo, byte[] FileName, long dwRecordLen, bool IsShareOpen)
        {
            lock (syncObj)
            {
                return D160X.StartRecordFile_Ex(wChnlNo, FileName, dwRecordLen, IsShareOpen);
            }
        }

        public bool StartRecordFileNew(Int32 wChnlNo, byte[] FileName, long dwRecordLen, Int32 dwRecordStartPos)
        {
            lock (syncObj)
            {
                return D160X.StartRecordFileNew(wChnlNo, FileName, dwRecordLen, dwRecordStartPos);
            }
        }

        public bool StartRecordFileNew_Ex(Int32 wChnlNo, byte[] FileName, Int32 dwRecordLen, Int32 dwRecordStartPos, bool IsShareOpen)
        {
            lock (syncObj)
            {
                return StartRecordFileNew_Ex(wChnlNo, FileName, dwRecordLen, dwRecordStartPos, IsShareOpen);
            }
        }

        public Int32 CheckRecordEnd(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return D160X.CheckRecordEnd(wChnlNo);
            }
        }

        public void StopRecordFile(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                D160X.StopRecordFile(wChnlNo);
            }
        }

        #endregion

        #region 收码、拔号、信号音检测函数

        public void InitDtmfBuf(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                D160X.InitDtmfBuf(wChnlNo);
            }
        }

        public Int32 GetDtmfCode(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return D160X.GetDtmfCode(wChnlNo);
            }
        }

        public bool DtmfHit(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return D160X.DtmfHit(wChnlNo);
            }
        }

        public void SendDtmfBuf(Int32 wChnlNo, byte[] DialNum)
        { 
            lock(syncObj)
            {
                D160X.SendDtmfBuf(wChnlNo, DialNum);
            }
        }

        public bool CheckSendEnd(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return D160X.CheckSendEnd(wChnlNo);
            }
        }

        public Int32 SetSendPara(Int32 ToneLen, Int32 SilenceLen)
        {
            lock (syncObj)
            {
                return D160X.SetSendPara(ToneLen, SilenceLen);
            }
        }

        public void NewSendDtmfBuf(Int32 ChannelNo, byte[] DialNum)
        {
            lock (syncObj)
            {
                D160X.NewSendDtmfBuf(ChannelNo, DialNum);
            }
        }

        public Int32 NewCheckSendEnd(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return D160X.NewCheckSendEnd(wChnlNo);
            }
        }

        public void StartSigCheck(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                D160X.StartSigCheck(wChnlNo);
            }
        }

        public void StopSigCheck(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                D160X.StopSigCheck(wChnlNo);
            }
        }

        public Int32 ReadCheckResult(Int32 wChnlNo, Int32 wMode)
        {
            lock (syncObj)
            {
                return D160X.ReadCheckResult(wChnlNo, wMode);
            }
        }

        public Int32 ReadBusyCount()
        {
            lock (syncObj)
            {
                return D160X.ReadBusyCount();
            }
        }

        public bool CheckPolarity(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return D160X.CheckPolarity(wChnlNo);
            }
        }

        public Int32 CheckSilence(Int32 wChnlNo, Int32 wCheckNum)
        {
            lock (syncObj)
            {
                return D160X.CheckSilence(wChnlNo, wCheckNum);
            }
        }

        #endregion

        #region 内线振铃及检测函数

        public bool ReadGenerateSigBuf(byte[] lpFileName)
        {
            lock (syncObj)
            {
                return D160X.ReadGenerateSigBuf(lpFileName);
            }
        }

        public void FeedPower(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                D160X.FeedPower(wChnlNo);
            }
        }

        public void FeedRing(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                D160X.FeedRing(wChnlNo);
            }
        }

        public void FeedSigFunc()
        {
            lock (syncObj)
            {
                D160X.FeedSigFunc();
            }
        }

        public void FeedRealRing(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                D160X.FeedRealRing(wChnlNo);
            }
        }

        public bool OffHookDetect(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return D160X.OffHookDetect(wChnlNo);
            }
        }

        public void StartPlaySignal(Int32 wChnlNo, Int32 SigType)
        {
            lock (syncObj)
            {
                D160X.StartPlaySignal(wChnlNo, SigType);
            }
        }

        public void StartHangUpDetect(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                D160X.StartHangUpDetect(wChnlNo);
            }
        }

        public Int32 HangUpDetect(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return D160X.HangUpDetect(wChnlNo);
            }
        }

        public Int32 StartTimer(Int32 wChnlNo, Int32 ClockType)
        {
            lock (syncObj)
            {
                return D160X.StartTimer(wChnlNo, ClockType);
            }
        }

        public Int32 ElapseTime(Int32 wChnlNo, Int32 ClockType)
        {
            lock (syncObj)
            {
                return D160X.ElapseTime(wChnlNo, ClockType);
            }
        }

        #endregion

        #region 连通函数

        public Int32 SetLink(Int32 wOne, Int32 wAnOther)
        {
            lock (syncObj)
            {
                return D160X.SetLink(wOne, wAnOther);
            }
        }

        public Int32 ClearLink(Int32 wOne, Int32 wAnOther)
        {
            lock (syncObj)
            {
                return D160X.ClearLink(wOne, wAnOther);
            }
        }

        public Int32 LinkOneToAnother(Int32 wOne, Int32 wAnOther)
        {
            lock (syncObj)
            {
                return D160X.LinkOneToAnother(wOne, wAnOther);
            }
        }

        public Int32 ClearOneFromAnother(Int32 wOne, Int32 wAnOther)
        {
            lock (syncObj)
            {
                return D160X.ClearOneFromAnother(wOne, wAnOther);
            }
        }

        public Int32 LinkThree(Int32 wOne, Int32 wTwo, Int32 wThree)
        {
            lock (syncObj)
            {
                return D160X.LinkThree(wOne, wTwo, wThree);
            }
        }

        public Int32 ClearThree(Int32 wOne, Int32 wTwo, Int32 wThree)
        {
            lock (syncObj)
            {
                return D160X.ClearThree(wOne, wTwo, wThree);
            }
        }

        #endregion

        #region 收主叫号码有关的函数

        public void ResetCallerIDBuffer(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                D160X.ResetCallerIDBuffer(wChnlNo);
            }
        }

        public Int32 GetCallerIDStr(Int32 wChnlNo, byte[] IDStr)
        {
            lock (syncObj)
            {
                return D160X.GetCallerIDStr(wChnlNo, IDStr);
            }
        }

        public Int32 GetCallerIDStrEx(Int32 wChnlNo, byte[] strTime, byte[] strCallerID, byte[] strUser)
        {
            lock (syncObj)
            {
                return D160X.GetCallerIDStrEx(wChnlNo, strTime, strCallerID, strUser);
            }
        }

        public Int32 GetCallerIDRawStr(Int32 wChnlNo, byte[] IDRawStr)
        {
            lock (syncObj)
            {
                return D160X.GetCallerIDRawStr(wChnlNo, IDRawStr);
            }
        }

        #endregion

        #region 会议功能

        public Int32 DConf_EnableConfCard()
        {
            lock (syncObj)
            {
                return D160X.DConf_EnableConfCard();
            }
        }

        public void DConf_DisableConfCard()
        {
            lock (syncObj)
            {
                D160X.DConf_DisableConfCard();
            }
        }

        public Int32 DConf_GetResNumber()
        {
            lock (syncObj)
            {
                return D160X.DConf_GetResNumber();
            }
        }

        public Int32 DConf_Adjust_CtrlWord(long wChnl, Int32 wCtrl)
        {
            lock (syncObj)
            {
                return D160X.DConf_Adjust_CtrlWord(wChnl, wCtrl);
            }
        }

        public Int32 AddChnl(Int32 ConfNo, Int32 ChannelNo, Int32 ChnlAtte, Int32 NoiseSupp)
        {
            lock (syncObj)
            {
                return D160X.AddChnl(ConfNo, ChannelNo, ChnlAtte, NoiseSupp);
            }
        }

        public Int32 SubChnl(Int32 ConfNo, Int32 ChannelNo)
        {
            lock (syncObj)
            {
                return D160X.SubChnl(ConfNo, ChannelNo);
            }
        }

        public Int32 AddListenChnl(Int32 ConfNo, Int32 ChannelNo)
        {
            lock (syncObj)
            {
                return D160X.AddListenChnl(ConfNo, ChannelNo);
            }
        }

        public Int32 SubListenChnl(Int32 ConfNo, Int32 ChannelNo)
        {
            lock (syncObj)
            {
                return D160X.SubListenChnl(ConfNo, ChannelNo);
            }
        }

        public Int32 DConf_AddRecListenChnl(Int32 ConfNo, Int32 ChannelNo)
        {
            lock (syncObj)
            {
                return DConf_AddRecListenChnl(ConfNo, ChannelNo);
            }
        }

        public Int32 DConf_SubRecListenChnl(Int32 ConfNo, Int32 ChannelNo)
        {
            lock (syncObj)
            {
                return D160X.DConf_SubRecListenChnl(ConfNo, ChannelNo);
            }
        }

        public Int32 DConf_AddPlayChnl(Int32 ConfNo, Int32 ChannelNo, Int32 ChnlAtte, Int32 NoiseSupp)
        {
            lock (syncObj)
            {
                return D160X.DConf_AddPlayChnl(ConfNo, ChannelNo, ChnlAtte, NoiseSupp);
            }
        }

        public Int32 DConf_SubPlayChnl(Int32 ConfNo, Int32 ChannelNo)
        {
            lock (syncObj)
            {
                return D160X.DConf_SubPlayChnl(ConfNo, ChannelNo);
            }
        }

        public Int32 DConf_AddChnl_TimeSlot(Int32 ConfNo, long wTimeSlot, Int32 ChnlAtte, Int32 NoiseSupp, long[] TS_CONF)
        {
            lock (syncObj)
            {
                return D160X.DConf_AddChnl_TimeSlot(ConfNo, wTimeSlot, ChnlAtte, NoiseSupp, TS_CONF);
            }
        }

        public Int32 DConf_AddListenChnl_TimeSlot(Int32 ConfNo, long[] TS_CONF)
        {
            lock (syncObj)
            {
                return D160X.DConf_AddListenChnl_TimeSlot(ConfNo, TS_CONF);
            }
        }

        public Int32 DConf_SubChnl_TimeSlot(Int32 ConfNo, long TS_Out)
        {
            lock (syncObj)
            {
                return D160X.DConf_SubChnl_TimeSlot(ConfNo, TS_Out);
            }
        }

        #endregion

        #region 信号音函数

        public void Sig_Init(Int32 Times)
        {
            lock (syncObj)
            {
                D160X.Sig_Init(Times);
            }
        }

        public Int32 Sig_CheckBusy(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return D160X.Sig_CheckBusy(wChnlNo);
            }
        }

        public Int32 Sig_StartDial(Int32 wChnlNo, byte[] DialNum, byte[] PreNum, Int32 wMode)
        {
            lock (syncObj)
            {
                return D160X.Sig_StartDial(wChnlNo, DialNum, PreNum, wMode);
            }
        }

        public Int32 Sig_GetCadenceCount(Int32 wChNo, Int32 nCadenceType)
        {
            lock (syncObj)
            {
                return D160X.Sig_GetCadenceCount(wChNo, nCadenceType);
            }
        }

        public Int32 Sig_CheckDial(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return D160X.Sig_CheckDial(wChnlNo);
            }
        }

        public void Sig_ResetCheck(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                D160X.Sig_ResetCheck(wChnlNo);
            }
        }

        public Int32 SetGenerateSigParam(Int32 nSigType, Int32 nFreq1, Int32 nFreq2, double dbAmp1, double dbAmp2, Int32 nOnTime, Int32 nOffTime, Int32 iSampleRate)
        {
            lock (syncObj)
            {
                return D160X.SetGenerateSigParam(nSigType, nFreq1, nFreq2, dbAmp1, dbAmp2, nOnTime, nOffTime, iSampleRate);
            }
        }

        #endregion

        #region 传真函数

        public Int32 DJFax_DriverReady(Int32 buffSize)
        {
            lock (syncObj)
            {
                return D160X.DJFax_DriverReady(buffSize);
            }
        }

        public void DJFax_DisableCard()
        {
            lock (syncObj)
            {
                D160X.DJFax_DisableCard();
            }
        }

        public Int32 GetTotalFaxChnl()
        {
            lock(syncObj)
            {
                return D160X.GetTotalFaxChnl();
            }
        }

        public Int32 DJFax_SelfCheckSetLink(Int32 wChnlNo)
        {
            lock(syncObj)
            {
                return D160X.DJFax_SelfCheckSetLink(wChnlNo);
            }
        }

        public Int32 DJFax_SelfCheckBreakLink(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return D160X.DJFax_SelfCheckBreakLink(wChnlNo);
            }
        }

        public Int32 DJFax_SetLink(Int32 wFaxChnlNo, Int32 wVoiceChnlNo)
        {
            lock (syncObj)
            {
                return D160X.DJFax_SetLink(wFaxChnlNo, wVoiceChnlNo);
            }
        }

        public Int32 DJFax_ClearLink(Int32 wFaxChnlNo, Int32 wVoiceChnlNo)
        {
            lock (syncObj)
            {
                return D160X.DJFax_ClearLink(wFaxChnlNo, wVoiceChnlNo);
            }
        }

        public Int32 DJFax_GetVoiceChnlOfFaxChnl(Int32 wFaxChnlNo)
        {
            lock (syncObj)
            {
                return D160X.DJFax_GetVoiceChnlOfFaxChnl(wFaxChnlNo);
            }
        }

        public Int32 DJFax_GetFaxChnlOfVoiceChnl(Int32 wVoiceChnlNo)
        {
            lock (syncObj)
            {
                return D160X.DJFax_GetFaxChnlOfVoiceChnl(wVoiceChnlNo);
            }
        }

        public Int32 DJFax_GetOneFreeFaxChnl()
        {
            lock (syncObj)
            {
                return D160X.DJFax_GetOneFreeFaxChnl();
            }
        }

        public Int32 DJFax_GetOneFreeFaxChnlOld()
        {
            lock (syncObj)
            {
                return D160X.DJFax_GetOneFreeFaxChnlOld();
            }
        }

        public Int32 DJFax_SetResolution(Int32 wChnlNo, Int32 ResolutionFlag)
        {
            lock (syncObj)
            {
                return D160X.DJFax_SetResolution(wChnlNo, ResolutionFlag);
            }
        }

        public Int32 DJFax_SendFaxFile(Int32 wChnlNo, byte[] FileName)
        {
            lock (syncObj)
            {
                return D160X.DJFax_SendFaxFile(wChnlNo, FileName);
            }
        }

        public Int32 DJFax_SendFaxFileEx(Int32 wChnlNo, byte[] FileName, Int32 StartPage, Int32 EndPage)
        {
            lock (syncObj)
            {
                return D160X.DJFax_SendFaxFileEx(wChnlNo, FileName, StartPage, EndPage);
            }
        }

        public Int32 DJFax_RcvFaxFile(Int32 wChnlNo, byte[] FileName)
        {
            lock (syncObj)
            {
                return D160X.DJFax_RcvFaxFile(wChnlNo, FileName);
            }
        }

        public Int32 DJFax_CheckTransmit(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return D160X.DJFax_CheckTransmit(wChnlNo);
            }
        }

        public void DJFax_StopFax(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                D160X.DJFax_StopFax(wChnlNo);
            }
        }

        public Int32 DJFax_SetLocalID(Int32 wChnlNo, byte[] Str)
        {
            lock (syncObj)
            {
                return D160X.DJFax_SetLocalID(wChnlNo, Str);
            }
        }

        public Int32 DJFax_GetLocalID(Int32 wChnlNo, byte[] Str)
        {
            lock (syncObj)
            {
                return D160X.DJFax_GetLocalID(wChnlNo, Str);
            }
        }

        public long DJFax_GetRcvBytes(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return D160X.DJFax_GetRcvBytes(wChnlNo);
            }
        }

        public long DJFax_GetSendBytes(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return D160X.DJFax_GetSendBytes(wChnlNo);
            }
        }

        public Int32 DJFax_SetDialNo(Int32 wChnlNo, byte[] DialNo)
        {
            lock (syncObj)
            {
                return D160X.DJFax_SetDialNo(wChnlNo, DialNo);
            }
        }

        public Int32 DJFax_GetErrCode(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return DJFax_GetErrCode(wChnlNo);
            }
        }

        public Int32 DJFax_GetErrPhase(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return DJFax_GetErrPhase(wChnlNo);
            }
        }

        public Int32 DJFax_GetErrSubst(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return D160X.DJFax_GetErrSubst(wChnlNo);
            }
        }

        public Int32 DJFax_GetCurPage(Int32 WChnlNo)
        {
            lock (syncObj)
            {
                return D160X.DJFax_GetCurPage(WChnlNo);
            }
        }

        public Int32 DJCvt_InitConvert()
        {
            lock (syncObj)
            {
                return D160X.DJCvt_InitConvert();
            }
        }

        public void DJCvt_DisableConvert()
        {
            lock (syncObj)
            {
                D160X.DJCvt_DisableConvert();
            }
        }

        public Int32 DJCvt_Open(Int32 wChnlNo, byte[] cbFaxFileName, byte cbResolution, Int32 wPageLineNo)
        {
            lock (syncObj)
            {
                return D160X.DJCvt_Open(wChnlNo, cbFaxFileName, cbResolution, wPageLineNo);
            }
        }

        public Int32 DJCvt_Close(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return D160X.DJCvt_Close(wChnlNo);
            }
        }

        public Int32 DJCvt_DotLine(Int32 wChnl, byte[] cbDotStr, Int32 wDotSize, Int32 wDotFlag)
        {
            lock (syncObj)
            {
                return DJCvt_DotLine(wChnl, cbDotStr, wDotSize, wDotFlag);
            }
        }

        public Int32 DJCvt_LeftLine(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return D160X.DJCvt_LeftLine(wChnlNo);
            }
        }

        public Int32 DJCvt_TextLineA(Int32 wChnl, byte[] cbTextStr, Int32 DoubleBitFlag, Int32 DoubleLineFlag, Int32 FontSize)
        {
            lock (syncObj)
            {
                return D160X.DJCvt_TextLineA(wChnl, cbTextStr, DoubleBitFlag, DoubleLineFlag, FontSize);
            }
        }

        public Int32 DJCvt_TextLine(Int32 wChnl, byte[] cbTextStr)
        {
            lock (syncObj)
            {
                return D160X.DJCvt_TextLine(wChnl, cbTextStr);
            }
        }

        public Int32 DJCvt_BmpFileA(Int32 wChnl, byte[] cbBmpFileName, Int32 DoubleBitFlag)
        {
            lock (syncObj)
            {
                return DJCvt_BmpFileA(wChnl, cbBmpFileName, DoubleBitFlag);
            }
        }

        public Int32 DJCvt_BmpFile(Int32 wChnl, byte[] cbBmpFileName)
        {
            lock (syncObj)
            {
                return D160X.DJCvt_BmpFile(wChnl, cbBmpFileName);
            }
        }

        public Int32 DJCvt_Bfx2Tiff(byte[] Bfxfilename, byte[] Tifffilename)
        {
            lock (syncObj)
            {
                return D160X.DJCvt_Bfx2Tiff(Bfxfilename, Tifffilename);
            }
        }

        public Int32 DJCvt_Tiff2Bfx(byte[] Tifffilename, byte[] Bfxfilename)
        {
            lock (syncObj)
            {
                return D160X.DJCvt_Tiff2Bfx(Tifffilename, Bfxfilename);
            }
        }

        public Int32 DJCvt_Bfx2Bmp(byte[] Bfxfilename, byte[] Bmpfilename, Int32 PageMode, Int32 RotateMode)
        {
            lock (syncObj)
            {
                return D160X.DJCvt_Bfx2Bmp(Bfxfilename, Bmpfilename, PageMode, RotateMode);
            }
        }

        public void DJFax_StartCheckFaxTone(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                D160X.DJFax_StartCheckFaxTone(wChnlNo);
            }
        }

        public Int32 DJFax_FaxToneCheckResult(Int32 wChnlNo)
        {
            lock (syncObj)
            {
                return D160X.DJFax_FaxToneCheckResult(wChnlNo);
            }
        }

        public Int32 Sig_GetDialStep(Int32 wChNo)
        {
            lock (syncObj)
            {
                return D160X.Sig_GetDialStep(wChNo);
            }
        }

        #endregion

        #endregion
    }
}
