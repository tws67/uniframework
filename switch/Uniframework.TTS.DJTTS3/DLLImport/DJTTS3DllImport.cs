using System;
using System.Runtime.InteropServices;

namespace Uniframework.Switch.TTS
{
    public static class TTS3
    {
        public const Int32 INFO_CONVERT_COMPLATE =           1;   // 语音文件转换完毕
        public const Int32 INFO_CONVERT_NOT_COMPLATE =       2;   // 语音文件转换未完毕
        public const Int32 INFO_PLAY_COMPLATE =              3;   // 转换播放完毕
        public const Int32 INFO_PLAY_NOT_COMPLATE =          4;   // 转换播放未完毕
        public const Int32 INFO_USE_LABLE =                  1;   // 使用使用转换标识
        public const Int32 INFO_NOTUSE_LABLE =               0;   // 不使用使用转换标识

        public const Int32 INFO_TEXT_BUFFER =                0;   // 文本字符串
        public const Int32 INFO_TEXT_FILENAME =             -1;   // 文本文件名

        public const Int32 OPERATE_SUCCESS =                 0;   // 操作成功
        public const Int32 ERR_NOT_INIT_TTS_CARD =          -1;   // 没有初始化语音卡 
        public const Int32 ERR_NOT_TTS_CARD =               -2;   // 不是TTS卡
        public const Int32 ERR_MEMERY_ASSIGN_FAIL =         -3;   // 内存分配失败
        public const Int32 ERR_NO_TTS_CHANNEL =             -4;   // 没有TTS通道资源
        public const Int32 ERR_INIT_TTS_CHANNEL_FAIL =      -5;   // 初始化TTS通道失败
        public const Int32 ERR_TTS_CHANNELNO_INVALID =      -6;   // 板卡的通道号无效
        public const Int32 ERR_TTS_CHANNEL_NO_FREE =        -7;   // TTS通道不空闲
        public const Int32 ERR_INPUTTEXTSTR_INVALID =       -8;   // 输入文本字符串为空
        public const Int32 ERR_INPUTTEXTFILE_NOT_EXISTS =   -9;   // 输入文本文件不存在
        public const Int32 ERR_TTS_CHANNEL_ISNOT_ENOUGH =   -10;  // TTS通道资源不够
        public const Int32 ERR_NOT_FIND_TRKDLL_OR_FILE =    -11;  // 没有找到指定DLL文件
        public const Int32 ERR_NOT_FIND_FUNC_IN_DLL =       -12;  // 在DLL中没有找到指定函数
        public const Int32 ERR_INIT_INDEX_FAIL =            -13;  // 初始化索引表失败
        public const Int32 ERR_PRE_INIT_TTS_RES_FAIL =      -14;  // 预创建TTS资源失败
        public const Int32 ERR_NO_MORE_TTS_RESOURCE =       -15;  // 无多余的TTS资源
        public const Int32 ERR_TTS_UNKNOWN_BOARD_TYPE	=	  -16;  // 未知的板卡类型
        public const Int32 ERR_TTS_UNBIND_TTS_RESOURCE =    -17;  // 没有绑定TTS资源
        public const Int32 ERR_UNKNOWN_TTS_ERROR =          -18;  // 未知的错误
        public const Int32 ERR_DELETE_TTS_RESOURCE_FAIL =   -19;  // 删除TTS资源失败，可能所有的TTS资源都在使用
        public const Int32 ERR_OPEN_FILE_FAIL =             -20;  //打开文件失败

        [DllImport("DJTTS3.dll", CharSet = CharSet.Auto)]
        /// <summary cref = "DJTTS3_Init">
        /// TTS初始化
        /// </summary>
        /// <returns>
        /// 大于0的数值                      •操作成功(返回可以使用语音通道数)
        /// ERR_NOT_INIT_TTS_CARD            •没有初始化语音卡
        /// ERR_NOT_TTS_CARD                 •不是TTS卡
        /// ERR_NO_TTS_CHANNEL               •没有TTS通道资源
        /// ERR_INIT_TTS_CHANNEL_FAIL        •初始化TTS通道失败
        /// <returns>
        /// <remarks>本函数首先创建转换线程并分配内存。本函数还要求在系统中，存在D161A-TTS卡并且已经成功的初始化了该卡（成功调用函数EnableCard）。</remarks>
        /// <seealso cref = "DJTTS3_Release"/>
        public static extern Int32 DJTTS3_Init();

        [DllImport("DJTTS3.dll", CharSet = CharSet.Auto)]
        /// <summary cref = "GetTotalTTSChannel">
        /// 返回系统中激活的TTS通道资源的个数
        /// </summary>
        /// <returns>
        /// 系统中激活的TTS通道资源的个数
        /// <returns>
        /// <seealso cref = "DJTTS3_GetLeaveTTSChannel"/>
        public static extern Int32 DJTTS3_GetTotalTTSChannel();

        [DllImport("DJTTS3.dll", CharSet = CharSet.Auto)]
        /// <summary cref = "DJTTS3_GetLeaveTTSChannel">
        /// 返回系统中激活了未使用的TTS通道资源的个数
        /// </summary>
        /// <returns>
        /// 系统中激活了未使用的TTS通道资源的个数
        /// <returns>
        /// <remarks></remarks>
        /// <seealso cref = "DJTTS3_GetTotalTTSChannel"/>
        public static extern Int32 DJTTS3_GetLeaveTTSChannel();
                                    
        [DllImport("DJTTS3.dll", CharSet = CharSet.Auto)]
        /// <summary cref = "DJTTS3_AddTTSToChannel">
        /// 在DJTTS3_Init成功后调用，向可用的语音通道添加TTS引擎资源。为了使用TTS功能，必须成功调用本函数
        /// </summary>
        /// <param name = "wChnlNo">TTS通道号(取值范围：0至65535)</param>
        /// <returns>
        /// OPERATE_SUCCESS                  •操作成功 
        /// ERR_TTS_CHANNELNO_INVALID        •TTS通道号无效
        /// ERR_TTS_CHANNEL_ISNOT_ENOUGH     •TTS通道资源不够
        /// <returns>
        /// <remarks>建议只对需要TTS功能的语音通道（一般为外线通道）添加TTS引擎资源，以减少系统资源的使用和转换效率的提高。</remarks>
        /// <seealso cref = "DJTTS3_Init"/>
        public static extern Int32 DJTTS3_AddTTSToChannel(Int32 wChnlNo); 

        [DllImport("DJTTS3.dll", CharSet = CharSet.Auto)]
        /// <summary cref = "DelTTSFromChannel">
        /// 在DJTTS3_Release前调用，从可用的语音通道释放TTS引擎资源
        /// </summary>
        /// <param name = "wChnlNo">TTS通道号</param>
        /// <returns>
        /// OPERATE_SUCCESS                  •操作成功  
        /// ERR_TTS_CHANNELNO_INVALID        •TTS通道号无效
        /// <returns>
        /// <seealso cref = ""/>
        public static extern Int32 DelTTSFromChannel(Int32 wChnlNo);

        [DllImport("DJTTS3.dll", CharSet = CharSet.Auto)]
        /// <summary cref = "DJTTS3_StartPlayText">
        /// 某个通道开始一个TTS放音。当成功的调用了本函数后，应该不断的调用函数DJTTS3_CheckPlayTextEnd，以维持TTS放音的持续；如果需要停止TTS放音，可以调用函数DJTTS3_StopPlayText。
        /// </summary>
        /// <param name = "wChnlNo">TTS通道号</param>
        /// <param name = "pText">要转换的文本字符串或文本文件名(文件格式为: *.txt格式)</param>
        /// <param name = "InputType">当为INFO_TEXT_FILENAME时，表示pText是文本文件名，当为INFO_TEXT_BUFFER时，表示pText是需要播放的文本字符串</param>
        /// <param name = "iVoice">无效，默认为0</param>
        /// <param name = "iSpeed">语速参数(0至100)，默认为50</param>
        /// <param name = "iVolume">音量参数（0至100）, 默认为50</param>
        /// <param name = "iLableFlag">是否使用转换标识, 默认INFO_USE_LABLE使用, INFO_NOTUSE_LABLE不使用</param>
        /// <returns>
        /// OPERATE_SUCCESS               •操作成功
        /// ERR_TTS_CHANNELNO_INVALID     •TTS通道号无效
        /// ERR_NO_TTS_CHANNEL            •没有TTS引擎资源
        /// ERR_TTS_CHANNEL_NO_FREE       •TTS通道不空闲
        /// ERR_INPUTTEXTSTR_INVALID      •输入文本字符串为空
        /// ERR_INPUTTEXTFILE_NOT_EXISTS  •输入文本文件不存在
        /// <returns>
        /// <seealso cref = "DJTTS_CheckPlayTextEnd"/>
        /// <seealso cref = "DJTTS_StopPlayText"/>
        public static extern Int32 DJTTS3_StartPlayText(Int32 wChnlNo, byte[] pText, Int32 InputType, Int32 iVoice, Int32 iSpeed, Int32 iVolume, Int32 iLableFlag);

        [DllImport("DJTTS3.dll", CharSet = CharSet.Auto)]
        /// <summary cref = "DJTTS3_CheckPlayTextEnd">
        /// 检查通道TTS放音是否结束，并维持TTS放音的持续。在成功调用函数DJTTS3_StartPlayText开始一个TTS放音以后，必须不断的调用本函数
        /// </summary>
        /// <param name = "wChnlNo">TTS通道号</param>
        /// <returns>
        /// INFO_PLAY_COMPLATE              •转换播放完毕
        /// INFO_PLAY_NOT_COMPLATE          •转换播放未完毕
        /// ERR_TTS_CHANNELNO_INVALID       •TTS通道号无效
        /// ERR_NO_TTS_CHANNEL              •没有TTS引擎资源
        /// <returns>
        /// <seealso cref = ""/>
        public static extern Int32 DJTTS3_CheckPlayTextEnd(Int32 wChnlNo);

        [DllImport("DJTTS3.dll", CharSet = CharSet.Auto)]
        /// <summary cref = "DJTTS3_StopPlayText">
        /// 停止通道TTS放音
        /// </summary>
        /// <param name = "wChnlNo">TTS通道号</param>
        /// <returns>
        /// OPERATE_SUCCESS                 •操作成功
        /// ERR_TTS_CHANNELNO_INVALID       •TTS通道号无效
        /// ERR_NO_TTS_CHANNEL              •没有TTS引擎资源
        /// <returns>
        /// <seealso cref = "DJTTS3_StartPlayText"/>
        public static extern Int32 DJTTS3_StopPlayText(Int32 wChnlNo);

        [DllImport("DJTTS3.dll", CharSet = CharSet.Auto)]
        /// <summary cref = "DJTTS3_Release">
        /// 释放初始化函数占用的资源。本函数必须在DisableCard之前调用
        /// </summary>
        /// <seealso cref = "DJTTS3_Init"/>
        public static extern void DJTTS3_Release();

        [DllImport("DJTTS3.dll", CharSet = CharSet.Auto)]
        /// <summary cref = "DJTTS3_StartAudioPlayText">
        /// 开始使用声卡播放字符串或文本文件
        /// </summary>
        /// <param name = ""></param>
        /// <param name = "pText">要转换的文本字符串或文本文件名(文件格式为: *.txt格式)</param>
        /// <param name = "InputType">当为INFO_TEXT_FILENAME时，表示pText是文本文件名，当为INFO_TEXT_BUFFER时，表示pText是需要播放的文本字符串</param>
        /// <param name = "iVoice">无效，默认为0</param>
        /// <param name = "iSpeed">语速参数(0至100)，默认为50</param>
        /// <param name = "iVolume">音量参数（0至100）, 默认为50</param>
        /// <param name = "iLableFlag">是否使用转换标识, 默认INFO_USE_LABLE使用, INFO_NOTUSE_LABLE不使用</param>
        /// <returns>
        /// OPERATE_SUCCESS               •操作成功
        /// ERR_TTS_CHANNEL_NO_FREE       •TTS通道不空闲
        /// ERR_NO_TTS_CHANNEL            •没有TTS引擎资源
        /// <returns>
        /// <seealso cref = "DJTTS3_StopAudioPlayText"/>
        /// <seealso cref = "DJTTS3_CheckAudioPlayText"/>
        public static extern Int32 DJTTS3_StartAudioPlayText(byte[] pText, Int32 InputType, Int32 iVoice, Int32 iSpeed, Int32 iVolume, Int32 iLableFlag);

        [DllImport("DJTTS3.dll", CharSet = CharSet.Auto)]
        /// <summary cref = "DJTTS3_StopAudioPlayText">
        /// 停止使用声卡播放字符串或文本文件
        /// </summary>
        /// <returns>
        /// OPERATE_SUCCESS               •操作成功
        /// ERR_NO_TTS_CHANNEL            •没有TTS引擎资源
        /// <returns>
        /// <seealso cref = "DJTTS3_StartAudioPlayText"/>
        /// <seealso cref = "DJTTS3_CheckAudioPlayText"/>
        public static extern Int32 DJTTS3_StopAudioPlayText();

        [DllImport("DJTTS3.dll", CharSet = CharSet.Auto)]
        /// <summary cref = "DJTTS3_CheckAudioPlayText">
        /// 检测使用声卡播放字符串或文本文件，是否播放完毕
        /// </summary>
        /// <returns>
        /// INFO_PLAY_COMPLATE              •声卡转换播放完毕
        /// INFO_PLAY_NOT_COMPLATE          •声卡转换播放未完毕
        /// <returns>
        public static extern Int32 DJTTS3_CheckAudioPlayText();

        #region 文本转换到文件

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="voice"></param>
        /// <param name="fileName"></param>
        /// <param name="chnl"></param>
        /// <param name="speed"></param>
        /// <param name="volume"></param>
        /// <param name="lableFlag"></param>
        /// <returns></returns>
        [DllImport("DJTTS3.dll", CharSet = CharSet.Auto)]
        public static extern long DJTTS3_CvtTextToVocFile(byte[] text, Int32 voice, byte[] fileName, Int32 chnl, Int32 speed, Int32 volume, Int32 lableFlag);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="voice"></param>
        /// <param name="fileName"></param>
        /// <param name="chnl"></param>
        /// <param name="speed"></param>
        /// <param name="volume"></param>
        /// <param name="lableFlag"></param>
        /// <returns></returns>
        [DllImport("DJTTS3.dll", CharSet = CharSet.Auto)]
        public static extern long DJTTS3_CvtTextToWaveFile(byte[] text, Int32 voice, byte[] fileName, Int32 chnl, Int32 speed, Int32 volume, Int32 lableFlag);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="voice"></param>
        /// <param name="fileName"></param>
        /// <param name="chnl"></param>
        /// <param name="speed"></param>
        /// <param name="volume"></param>
        /// <param name="lableFlag"></param>
        /// <returns></returns>
        [DllImport("DJTTS3.dll", CharSet = CharSet.Auto)]
        public static extern long DJTTS3_CvtTextFileToVocFile(byte[] text, Int32 voice, byte[] fileName, Int32 chnl, Int32 speed, Int32 volume, Int32 lableFlag);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="voice"></param>
        /// <param name="fileName"></param>
        /// <param name="chnl"></param>
        /// <param name="speed"></param>
        /// <param name="volume"></param>
        /// <param name="lableFlag"></param>
        /// <returns></returns>
        [DllImport("DJTTS3.dll", CharSet = CharSet.Auto)]
        public static extern long DJTTS3_CvtTextFileToWaveFile(byte[] text, Int32 voice, byte[] fileName, Int32 chnl, Int32 speed, Int32 volume, Int32 lableFlag);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chnl"></param>
        /// <returns></returns>
        [DllImport("DJTTS3.dll", CharSet = CharSet.Auto)]
        public static extern long DJTTS3_CheckCvtEnd(Int32 chnl);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chnl"></param>
        /// <returns></returns>
        [DllImport("DJTTS3.dll", CharSet = CharSet.Auto)]
        public static extern long DJTTS3_CvtStop(Int32 chnl);

        #endregion

    }
}
