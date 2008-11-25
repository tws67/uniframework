using System;
using System.Runtime.InteropServices;

namespace Uniframework.Switch.Endpoints.DB160X
{
    #region 语音卡系统信息结构

    /// <summary cref = "TC_INI_TYPE">
    /// 语音卡系统信息结构
    /// </summary>
    public struct TC_INI_TYPE
    {
        public Int32 wCardNo;
        public Int32 wCardType;
        public Int32 wConnect;
        public short wIRQ;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] cbDir;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public short[] wAddress;
        public short wMajorVer;
        public short wMinorVer;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public Int32[] wChType;
    }

    /// <summary cref = "TC_INI_TYPE_MORE">
    /// 语音卡扩展系统信息结构
    /// </summary>
    public struct TC_INI_TYPE_MORE
    {
        public short wMemAddr;             // 卡占用的共享内存地址
        public short wCardNum;             // 卡总数
        /// <summary>
        /// 目前可能的卡类型
        /// #define	CARD_TYPE_D161A		16
        /// #define	CARD_TYPE_D080A		8
        /// </summary>
        public byte[] cbCardType;          // 卡的类型，也表示该卡上的通道总数
        /// <summary>
        /// 标识该卡的类型
        /// #define CHTYPE_USER         0
        /// #define CHTYPE_TRUNK        1
        /// #define CHTYPE_EMPTY        2
        /// </summary>
        public byte[] cbCardNeiWai;        // 该卡是中继卡（外线）还是用户卡（内线
        public Int32 wChnlNum;               // 卡上的通道总数
        public byte[] cbChType;            // 该通道的类型
        public byte[] cbChnlCardNo;        // 该通道所在卡号
        public byte[] cbChnlInternal;      // 该通道在卡内的通道号
        public byte[] cbConnectChnl;       // 保留
        public byte[] cbConnectStream;     // 保留
        public byte[] cbDtmfModeVal;       // 保留
        public byte[] cbIsSupportCallerID; // 该通道是否支持Caller-ID，1表示支持。D161A卡上的通道将支持Caller-ID。
    }
    
    #endregion

    public static class D160X
    {
        public static object SyncObj = new object(); // 同步对象

        #region 常量定义

        public const short MAX_CARD_NO = 32;
        public const short MAX_CHANNEL_NO = MAX_CARD_NO * 8;
        public const short LEN_FILEPATH = 128;

        public const short NODTMF = -1;
        public const short DTMF_CODE_0 = 10;
        public const short DTMF_CODE_1 = 1;
        public const short DTMF_CODE_2 = 2;
        public const short DTMF_CODE_3 = 3;
        public const short DTMF_CODE_4 = 4;
        public const short DTMF_CODE_5 = 5;
        public const short DTMF_CODE_6 = 6;
        public const short DTMF_CODE_7 = 7;
        public const short DTMF_CODE_8 = 8;
        public const short DTMF_CODE_9 = 9;
        public const short DTMF_CODE_STAR = 11;
        public const short DTMF_CODE_SHARP = 12;
        public const short DTMF_CODE_A = 13;
        public const short DTMF_CODE_B = 14;
        public const short DTMF_CODE_C = 15;
        public const short DTMF_CODE_D = 16;

        public const short S_NODIALTONE = 0x0F;
        public const short S_NORESULT = 0x10;
        public const short S_BUSY = 0x11;
        public const short S_NOBODY = 0x13;
        public const short S_CONNECT = 0x14;
        public const short S_NOSIGNAL = 0x15;
        public const short S_DIALSIG = 0x30;

        public const short PACK_64KBPS = 0;
        public const short PACK_32KBPS = 1;
        public const short PACK_16KBPS = 2;
        public const short PACK_8KBPS = 3;

        // NEW ADD for Feed and Signal

        public const short SIG_STOP = 0;
        public const short SIG_DIALTONE = 1;
        public const short SIG_BUSY1 = 2;
        public const short SIG_BUSY2 = 3;
        public const short SIG_RINGBACK = 4;
        public const short SIG_STOP_NEW = 10;

        #endregion

        #region 初始化函数

        /// <summary>
        /// 初始化设备驱动程序
        /// </summary>
        /// <returns>
        ///  0       成功
        /// -1   	 打开设备驱动程序错误
        /// -2   	 在读取TC08A-V.INI文件时，发生错误
        /// -3  	 INI文件的设置与实际的硬件不一致时，发生错误
        /// </returns>
        /// <seealso cref = "FreeDRV"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern long LoadDRV();

        /// <summary cref = "FreeDRV">
        /// 关闭驱动程序
        /// </summary>
        /// <seealso cref = "LoadDRV"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void FreeDRV();

        /// <summary>
        /// 初始化电话卡的硬件并为每个通道分配语音缓冲区。wUsedCh可以用函数CheckValidCh来获得。
        /// </summary>
        /// <param name = "wUsedCh">工作的总通道数</param>
        /// <param name = "wFileBufLen">驱动中为每通道分配的语音内存大小</param>
        /// <returns>
        ///  0       成功
        /// -1 　　　LoadDRV没有成功，造成本函数调用失败　
        /// -2       分配缓冲区失败 
        /// </returns>
        /// <remarks>
        /// 在调用本函数时，将为每路分配wFileBufLen 大小的语音缓冲区，共计wUsedCh * wFileBufLen， 若申请不到，则返回-2。buffer必须为1024的整数倍。
        /// 比如：EnableCard ( 8, 1024*16 ); 将会申请128K的内存。
        /// </remarks>
        /// <seealso cref = "CheckValidCh"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern long EnableCard(Int32 wusedCh, Int32 wFileBufLen);

        /// <summary>
        /// 关闭电话卡的硬件，释放缓冲区。程序结束(包括正常和不正常退出)时需调用此函数
        /// </summary>
        /// <seealso cref="EnableCard"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void DisableCard();

        /// <summary cref = "GetSysInfo">
        /// 获得系统配置的有关信息
        /// </summary>
        /// <param name = "TmpIni"> 指向结构TC_INI_TYPE</param>
        /// <remarks>在调用前，由应用程序负责分配结构TC_INI_TYPE的空间，然后将指针传给本函数。当调用完成后，在结构TC_INI_TYPE中将存有系统信息。</remarks>
        /// <seealso cref = "TC_INI_TYPE"/>
        /// <seealso cref = "GetSysInfoMore"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void GetSysInfo(ref TC_INI_TYPE TmpIni);
        
        /// <summary>
        /// 获得系统配置的更多信息。由于可以在一台机器内混合使用D161A PCI和D080A PCI卡，因此，当需要得到更多的系统信息时，必须使用本函数。
        /// </summary>
        /// <param name = "TmpMore">指向结构TC_INI_TYPE_MORE</param>
        /// <remarks>在调用前，由应用程序负责分配结构TC_INI_TYPE_MORE的空间，然后将指针传给本函数。当调用完成后，在结构TC_INI_TYPE_MORE中将存有系统信息。</remarks>
        /// <seealso cref = "TC_INI_TYPE_MORE"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void GetSysInfoMore(ref TC_INI_TYPE_MORE TmpMore);
        
        /// <summary>
        /// 检测在当前机器内可用的通道总数。
        /// </summary>
        /// <returns>
        /// 总的可用通道数
        /// </returns>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern short CheckValidCh();
        
        /// <summary>
        /// 检查通道类型。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <returns>
        /// CHTYPE_USER （0）		   	内线
        /// CHTYPE_TRUNK（1）		    外线
        /// CHTYPE_EMPTY（2）  		    悬空
        /// </returns>
        /// <remarks>
        /// 检测某个通道的类型。由于D161A PCI卡上的电话接口模块可以任意配置，因此，对于D161A PCI卡,其上16路通道可以为TRUNK(外线)、USER（内线）、录音模块的任意组合。
        /// 另外，与TC-08A V型卡不同的是，D161A PCI卡可以检测到某路没有插模块，因而CHTYPE-EMPTY（悬空）就是有效的了。
        /// </remarks>
        /// <seealso cref = "CheckChTypeNew"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern short CheckChType(Int32 wChnlNo);
        
        /// <summary>
        /// 检测某个通道的类型。由于D161A PCI卡上的电话接口模块可以任意配置，因此，对于D161A PCI卡,其上16路通道可以为TRUNK(外线)、USER（内线）、录音模块的任意组合。
        /// 另外，与TC-08A V型卡不同的是，D161A PCI卡可以检测到某路没有插模块，因而CHTYPE-EMPTY（悬空）就是有效的了。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <returns>
        /// CHTYPE_USER  （0）		   	内线
        /// CHTYPE_TRUNK （1）		    外线
        /// CHTYPE_EMPTY （2）  		悬空
        /// CHTYPE_RECORD（3）          录音
        /// </returns>
        /// <seealso cref = "CheckChType"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern short CheckChTypeNew(Int32 wChnlNo);
        
        /// <summary>
        /// 判断该卡是否支持Caller-ID功能。D161A PCI卡将返回1（支持）。
        /// </summary>
        /// <returns>
        ///  1      支持
        ///  0      不支持
        /// </returns>
        /// <seealso cref = "GetSysInfoMore"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool IsSupportCallerID();
        
        /// <summary cref = "SetPackRate">
        /// 设置压缩比率。调用本函数后, 录放音均以该压缩比率进行。
        /// </summary>
        /// <param name = "wPackRate">压缩比率， 其值为
        /// #define	PACK_64KBPS     0  (无压缩) 每秒64K bits 即 8K bytes
        /// #define	PACK_32KBPS	    1  每秒32K bits 即 4K bytes
        /// </param>
        /// <remarks>对于D161A PCI卡，目前只支持32KBPS的压缩方式，用其他参数调用本函数无效。本函数必须在初始化函数EnableCard成功之后调用才有效。</remarks>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void SetPackRate(Int32 wPackRate);
        
        /// <summary cref = "PUSH_PLAY">
        /// 维持文件录放音的持续进行，需在处理函数的大循环中调用。
        /// </summary>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void PUSH_PLAY();
        
        /// <summary cref = "SetBusyPara">
        /// 设定要检测的挂机忙音的参数。
        /// </summary>
        /// <param name = "BusyLen">忙音的时间长度，单位为毫秒。</param>
        /// <remarks>比如：国标中规定的0.7秒忙音信号，写为SetBusyPara(700)；</remarks>
        /// <seealso cref="SetDialPara"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void SetBusyPara(Int32 BusyLen);
        
        /// <summary>
        /// 设定拨号以后要检测的信号音的参数。
        /// </summary>
        /// <param name = "RingBack">回铃音中响声的时间长度，单位为毫秒。</param>
        /// <param name = "RingBack0"> 回铃音中两声之间间隔的时间长度，单位为毫秒。</param>
        /// <param name = "BusyLen">对方占线时返回的忙音信号的时间长度。</param>
        /// <param name = "RingTimes">一共响铃多少次认为是无人接听。</param>
        /// <remarks>比如：国标中规定拨号后的回铃音为响1秒，停止4秒，忙音为0.35秒，写为  SetDialPara(1000,4000,350,7)。</remarks>
        /// <seealso cref="SetBusyPara"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void SetDialPara(Int32 RingBack, Int32 RingBack0, Int32 BusyLen, Int32 RingTimes);
        
        /// <summary>
        /// 读取D161A PCI卡的序列号。东进公司出品的D161A PCI卡，都有一个唯一的编号（十进制六位数），用户可以用来加密。
        /// </summary>
        /// <param name = "wCardNo">卡号</param>
        /// <returns>
        /// 语音卡的序列号。
        /// </returns>
        /// <remarks>读序列号必须在函数LoadDRV之后，EnableCard之前。请将序列号放入程序的变量中。</remarks>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern long NewReadPass(Int32 wCardNo);
        
        /// <summary>
        /// 设定某通道的工作参数。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <param name = "cbWorkMode">选择要设定的工作模式</param>
        /// <param name = "cbModeVal">参数值</param>
        /// <remarks>
        /// cbWorkMode	            CbModeVal
        /// WORK_MODE_DTMF	        DTMF_MODE_VAL_NORMAL  (0) 
        ///                         DTMF_MODE_VAL_QUICK  (1)
        /// WORK_MODE_CHECK_RING	CHECK_RING_MODE_VAL_NEW  (0)    
        ///                         CHECK_RING_MODE_VAL_OLD  (1)
        /// WORK_MODE_REC_AGC	    REC_AGC_MODE_VAL_DISABLE  (0) 
        ///                         REC_AGC_MODE_VAL_ENABLE  (1)
        /// </remarks>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void D_SetWorkMode(Int32 wChnlNo, byte cbWorkMode, byte cbModeVal);

        #endregion

        #region 振铃及摘挂机函数

        /// <summary>
        /// 检查（外线）是否有振铃信号或（内线）是否有提机。
        /// 当函数返回1时，对于外线，此时“提机”（OffHook）可接通该路电话，而内线无需“提机”即处于接通状态。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool RingDetect(Int32 wChnlNo);
        
        /// <summary>
        /// 外线提机，对于内线，此函数不起作用。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <remarks>不要对内线使用。</remarks>
        /// <seealso cref = "HangUp"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void OffHook(Int32 wChnlNo);

        /// <summary>
        /// 外线挂机，对于内线，此函数不起作用。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <remarks>不要对内线使用。</remarks>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void HangUp(Int32 wChnlNo);

        #endregion

        #region 放音函数

        /// <summary>
        /// 指定通道开始普通内存放音。当放音的长度（dwPlayLength）大于系统缓冲区的长度（在函数EnableCard中定义）时，需要函数PUSH_PLAY来维持录音的持续。
        /// 停止该种方式的放音，用函数StopPlay；检查是否放音完毕，用函数CheckPlayEnd。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <param name = "PlayBuf">语音缓冲区地址</param>
        /// <param name = "dwStartPos">在缓冲区中的偏移</param>
        /// <param name = "dwPlayLen">放音的长度</param>
        /// <seealso cref = "StopPlay"/>
        /// <seealso cref="CheckPlayEnd"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void StartPlay(Int32 wChnlNo, byte[] PlayBuf, Int32 dwStartPos, Int32 dwPlayLen);
        
        /// <summary>
        /// 指定通道停止内存放音，本函数可以停止内存普通放音、内存索引放音、内存循环放音。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <seealso cref = "StartPlay"/>
        /// <seealso cref = "CheckPlayEnd"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void StopPlay(Int32 wChnlNo);
        
        /// <summary>
        /// 检查指定通道放音是否结束，本函数可以用于普通内存放音、索引内存放音、循环内存放音和文件放音。
        /// </summary>
        /// <param name = wChnlNo"">通道号</param>
        /// <seealso cref = "StartPlay"/>
        /// <seealso cref = "StartPlayIndex"/>
        /// <seealso cref = "StopPlay">
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool CheckPlayEnd(Int32 wChnlNo);

        /// <summary>
        /// 初始化索引内存话音。
        /// </summary>
        /// <seealso cref = "StartPlayIndex"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void ResetIndex();

        /// <summary>
        ///  设置索引内存放音登记项。
        /// </summary>
        /// <param name = "VocBuf">指向要登记的语音缓冲区指针。</param>
        /// <param name = "dwVocLen">语音长度。</param>
        /// <returns>
        ///  0      登记失败,说明索引登记项已满
        ///  1      登记成功
        /// </returns>
        /// <seealso cref = "ResetIndex"/>
        /// <seealso cref = "StartPlayIndex"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 SetIndex(byte[] VocBuf, Int32 dwVocLen);

        /// <summary>
        /// 开始文件放音。停止该方式的放音，一定要用StopPlayFile。检查放音是否结束，用CheckPlayEnd函数。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <param name = "FileName">语音文件</param>
        /// <param name = "StartPos">放音的起始位置</param>
        /// <returns>
        ///  true   成功，开始播放语音
        ///  false  失败，可能是文件不存在或其它原因
        /// </returns>
        /// <remarks>
        /// 文件放音在本质上是利用循环内存放音，然后，不断的更新缓冲区。PUSH_PLAY函数的调用，能够保证对放音缓冲区的更新，从而达到放音的连续不断的进行。
        /// 当调用本函数进行文件放音时，用StartPos来指定放音的起始位置；如果使能Wave格式（WaveFormat=1，2，3），则实际放音的起始位置是：StartPos + 58。
        /// 注：
        /// 本函数不检查文件头的合法性，仅仅是简单的跳过开始的58个字节，因此，对于非Wave格式的原来的语音文件，也可以正常放音。
        /// 对于文件索引放音函数StartIndexPlayFile，由于其本质上调用的是StartPlayFile，因此，其播放每个单独文件时，也仅仅是简单的跳过每个文件开始的58个字节。
        /// 对于其他的放音函数，如：StartPlay、StartPlayIndex、StartPlaySignal、SYS_StartLoopPlay，由于不是文件放音，其功能仍然同以前一样，不会跳过58个字节。
        /// </remarks>
        /// <seealso cref = "CheckPlayEnd"/>
        /// <seealso cref = "StopPlayFile"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool StartPlayFile(Int32 wChnlNo, byte[] FileName, Int32 StartPos);

        /// <summary>
        /// 本函数对指定通道停止文件放音。对于用函数StartPlayFile开始的放音，必须用本函数来停止，这样才能关闭语音文件。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <remarks>如果使能Wave格式（WaveFormat=1，2，3），在本函数中，将会根据实际录音的数据长度完成对Wave文件头的填写，这样，该文件才能够在声卡上正确的播放。如果没有调用本函数，则有可能在声卡上无法正常播放。</remarks>
        /// <seealso cref = "StartPlayFile"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void StopPlayFile(Int32 wChnlNo);

        /// <summary>
        /// 初始化多文件放音。每开始一个新的多文件放音前调用此函数。
        /// </summary>
        /// <remarks>
        /// 为了方便用户的开发，在文件放音的基础上增加了文件索引放音的功能，该功能可以一次播放最多100个的语音文件，在进行语音拼接的时候比较有用。
        /// 在系统的内部，对应每个通道有一个数组和一个计数器，该数组可以存放100个文件名。调用本函数RsetIndexPlayFile，会将计数器清0；调用函数AddIndexPlayFile会在数组中将文件名记录下来，同时计数器加1；调用函数StartIndexPlayFile开始播放第一个语音文件；此后，需要不停的调用函数CheckIndexPlayFile，在这个函数中，如果检查到当前正在放音的文件已经放完，就会启动下一个放音，直到处理完所有的语音文件。如果需要停止索引放音，可以使用函数StopIndexPlayFile。
        /// </remarks>
        /// <seealso cref = "AddIndexPlayFile"/>
        /// <seealso cref = "StartIndexPlayFile"/>
        /// <seealso cref = "CheckIndexPlayFile"/>
        /// <seealso cref = "StopIndexPlayFile"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void RsetIndexPlayFile(Int32 Line);
        
        /// <summary>
        /// 增加多文件放音的放音文件
        /// </summary>
        /// <returns>
        ///  true      成功
        ///  flase     失败
        /// </returns>
        /// <seealso cref = "RsetIndexPlayFile"/>
        /// <seealso cref = "StartIndexPlayFile"/>
        /// <seealso cref = "CheckIndexPlayFile"/>
        /// <seealso cref = "StopIndexPlayFile"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool AddIndexPlayFile(Int32 Line, byte[] FileName);
        
        /// <summary>
        /// 开始一个内存索引放音。
        /// SetIndex和StartPlayIndex两个函数的引入是为了方便用户的组合放音，如数字的组合。在初始化的时候，由应用程序负责打开语音文件、分配内存、将语音调入内存中，当每一个语音调入内存后，需要用函数SetIndex登记，如此循环。
        /// 初始化完成后，所有通道都可以共用这些语音来进行索引放音。比如，当需要播放语音“一千零五十三”时，可以用如下办法：
        /// Int32 NumStr[6] = { 1, 12, 0, 5,10, 3 };
        /// StartPlayIndex ( ChannelNo, NumStr, 6 );
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <param name = "pIndexTable">要放音的索引序号</param>
        /// <param name = "wIndexLen">要放音的索引长度</param>
        /// <seealso cref = "StopPlay"/>
        /// <seealso cref = "CheckPlayEnd"/>
        /// <seealso cref = "SetIndex"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool StartPlayIndex(Int32 wChnlNo, Int32[] pIndexTable, Int32 wIndexLen);
        
        /// <summary>
        /// 检查多文件放音是否结束，并维护多文件放音的连续性。当进行多文件放音时，必须调用本函数，以保证多文件放音的连续性。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <returns>
        ///  ture   结束
        ///  false  未结束
        /// </returns>
        /// <seealso cref = "RsetIndexPlayFile"/>
        /// <seealso cref = "AddIndexPlayFile"/>
        /// <seealso cref = "StartIndexPlayFile"/>
        /// <seealso cref = "StopIndexPlayFile"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool CheckIndexPlayFile(Int32 wChnlNo);
        
        /// <summary>
        /// 停止多文件放音。该函数停止指定通道的多文件放音，对于使用StartIndexPlayFile函数开始的多文件放音，结束时一定要调用本函数。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <seealso cref = "RsetIndexPlayFile"/>
        /// <seealso cref = "AddIndexPlayFile"/>
        /// <seealso cref = "StartIndexPlayFile"/>
        /// <seealso cref = "CheckIndexPlayFile"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void StopIndexPlayFile(Int32 wChnlNo);
        
        /// <summary>
        /// 开始一个多文件放音。当调用该函数成功后，必须循环调用CheckIndexPlayFile函数来检测放音是否结束，并维护多文件放音的连续进行。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <returns>
        ///  ture   成功
        ///  false  失败
        /// </returns>
        /// <seealso cref = "RsetIndexPlayFile"/>
        /// <seealso cref = "AddIndexPlayFile"/>
        /// <seealso cref = "CheckIndexPlayFile"/>
        /// <seealso cref = "StopIndexPlayFile"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool StartIndexPlayFile(Int32 wChnlNo);

        #endregion

        #region 录音函数

        /// <summary>
        /// 开始文件录音。当调用本函数进行文件录音时，用dwRecordLen来指定录音的最大长度。如果使能Wave格式（WaveFormat=1，2，3），则实际文件的最大长度是：dwRecordLen + 58。当成功调用本函数后，会自动在文件FileName的开头预先写下58字节。停止该方式的录音，一定要用StopRecordFile。检查录音是否结束，用CheckRecordEnd函数。
        /// 文件录音在本质上是利用循环内存录音，然后，不断的更新缓冲区。PUSH_PLAY函数的调用，能够保证录音被移走，从而达到录音的进行。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <param name = "FileName">文件名</param>
        /// <param name = "dwRecordLen">最长录音长度</param>
        /// <returns>
        ///  ture   成功
        ///  flase  失败，非法的文件名或路径
        /// </returns>
        /// <seealso cref = "StopRecordFile"/>
        /// <seealso cref = "StartRecordFile_Ex"/>
        /// <seealso cref = "CheckRecordEnd"/>
        /// <seealso cref = "StartRecordFileNew"/>
        /// <seealso cref = "StartRecordFileNew_Ex"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool StartRecordFile(Int32 wChnlNo, byte[] FileName, long dwRecordLen);

        /// <summary>
        /// 开始文件录音。当调用本函数进行文件录音时，用dwRecordLen来指定录音的最大长度。如果使能Wave格式（WaveFormat=1，2，3），则实际文件的最大长度是：dwRecordLen + 58。当成功调用本函数后，会自动在文件FileName的开头预先写下58字节。停止该方式的录音，一定要用StopRecordFile。检查录音是否结束，用CheckRecordEnd函数。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <param name = "FileName">文件名</param>
        /// <param name = "dwRecordLen">文件录音最长长度</param>
        /// <param name = "IsShareOpen">是否以共享方式读写文件</param>
        /// <returns>
        ///  ture   成功
        ///  flase  失败
        /// </returns>
        /// <remarks>
        /// 文件录音在本质上是利用循环内存录音，然后，不断的更新缓冲区。PUSH_PLAY函数的调用，能够保证录音被移走，从而达到录音的进行。
        /// 注：
        /// 当参数IsShareOpen为FALSE的时候，等同于StartRecordFile；为TRUE时，录音文件将会以“共享读/共享写”方式打开。
        /// </remarks>
        /// <seealso cref = "StopRecordFile"/>
        /// <seealso cref = "StartRecordFile"/>
        /// <seealso cref = "CheckRecordEnd"/>
        /// <seealso cref = "StartRecordFileNew"/>
        /// <seealso cref = "StartRecordFileNew_Ex"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool StartRecordFile_Ex(Int32 wChnlNo, byte[] FileName, long dwRecordLen, bool IsShareOpen);

        /// <summary>
        /// 本函数是对文件录音函数StartRecordFile的补充。事实上，本函数可以完全代替函数StartRecordFile。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <param name = "FileName">文件名</param>
        /// <param name = "dwRecordLen">文件录音最长长度</param>
        /// <param name = "dwRecordStartPos">录音的开始位置</param>
        /// <returns>
        ///  ture   成功
        ///  false  失败
        /// </returns>
        /// <remarks>
        /// dwRecordStartPos是指在一个文件中录音的开始位置，例如：如果你需要在一个已存在文件的第5秒的位置开始录音，则指定dwRecordStartPos = 8000*5；如果使能Wave格式（WaveFormat=1，2，3），本函数将自动将dwRecordStartPos 加上58。其工作方式如下：
        /// 当dwRecordStartPos=0时，调用函数StartRecordFile，即：创建新文件来录音；
        /// 当FileName不存在时，调用函数StartRecordFile，即：创建新文件来录音；
        /// 当FileName已经存在：若dwRecordStartPos大于文件的长度时，从文件的尾部开始录音；因此，如果需要从一个文件的尾部继续录音，可以令dwRecordStartPos=0xFFFFFFFFL；若dwRecordStartPos小于文件的长度时，从dwRecordStartPos的位置开始录音；
        /// 录音的长度由变量dwRecordLen来确定。
        /// </remarks>
        /// <seealso cref = "StopRecordFile"/>
        /// <seealso cref = "StartRecordFile"/>
        /// <seealso cref = "StartRecordFile_Ex"/>
        /// <seealso cref = "CheckRecordEnd"/>
        /// <seealso cref = "StartRecordFileNew_Ex"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool StartRecordFileNew(Int32 wChnlNo, byte[] FileName, long dwRecordLen, Int32 dwRecordStartPos);

        /// <summary>
        /// 本函数是对文件录音函数StartRecordFile_Ex的补充。事实上，本函数可以完全代替函数StartRecordFile_Ex。dwRecordStartPos是指在一个文件中录音的开始位置，例如：如果你需要在一个已存在文件的第5秒的位置开始录音，则指定dwRecordStartPos = 8000*5；如果使能Wave格式（WaveFormat=1，2，3），本函数将自动将dwRecordStartPos 加上58。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <param name = "FileName">文件名</param>
        /// <param name = "dwRecordLen">文件录音最长长度</param>
        /// <param name = "dwRecordStartPos">录音的开始位置</param>
        /// <param name = "IsShareOpen">是否以共享方式读写文件</param>
        /// <returns>
        ///  ture   成功
        ///  false  失败
        /// </returns>
        /// <remarks>
        /// 其工作方式如下：
        /// 当dwRecordStartPos=0时，调用函数StartRecordFile_Ex，即：创建新文件来录音；
        /// 当FileName不存在时，调用函数StartRecordFile_Ex，即：创建新文件来录音；
        /// 当FileName已经存在：若dwRecordStartPos大于文件的长度时，从文件的尾部开始录音；因此，如果需要从一个文件的尾部继续录音，可以令dwRecordStartPos=0xFFFFFFFFL；若dwRecordStartPos小于文件的长度时，从dwRecordStartPos的位置开始录音；
        /// 录音的长度由变量dwRecordLen来确定。
        /// 注：
        /// 当参数IsShareOpen为FALSE的时候，等同于StartRecordFileNew；为TRUE时，录音文件将会以“共享读/共享写”方式打开。
        /// </remarks>
        /// <seealso cref = "StopRecordFile"/>
        /// <seealso cref = "StartRecordFile"/>
        /// <seealso cref = "StartRecordFile_Ex"/>
        /// <seealso cref = "CheckRecordEnd"/>
        /// <seealso cref = "StartRecordFileNew"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool StartRecordFileNew_Ex(Int32 wChnlNo, byte[] FileName, Int32 dwRecordLen, Int32 dwRecordStartPos, bool IsShareOpen);

        /// <summary>
        ///  检查指定通道录音是否结束(缓冲区已满)。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <param name = ""></param>
        /// <returns>
        ///  0      未结束
        ///  1      已结束
        /// </returns>
        /// <seealso cref = "StartRecord"/>
        /// <seealso cref = "StartRecordFile"/>
        /// <seealso cref = "StartRecordFileNew"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 CheckRecordEnd(Int32 wChnlNo);

        /// <summary>
        /// 该函数停止指定通道的文件录音，对于StartRecordFile函数启动的录音, 一定要用本函数来停止，这样才能保证关闭语音文件。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <seealso cref = "StartRecordFile"/>
        /// <seealso cref = "StartRecordFileNew"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void StopRecordFile(Int32 wChnlNo);

        #endregion

        #region 收码、拔号、信号音检测函数

        /// <summary>
        /// 清空系统的DTMF缓冲区，如果在缓冲区中有DTMF按键的值，将会丢失。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <seealso cref = "GetDtmfCode"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void InitDtmfBuf(Int32 wChnlNo);
        
        /// <summary>
        /// 取该通道收到的DTMF编码，如果在缓冲区中有DTMF按键，调用本函数将返回最早的一个DTMF按键，同时将该按键从缓冲区中移去。如果在缓冲区中没有收到任何的DTMF按键，本函数返回-1。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <param name = ""></param>
        /// <returns>
        /// -1    	缓冲区中没有DTMF按键
        /// 其他    有DTMF按键
        /// 1～9  	1～9键
        /// 10      0键
        /// 11    	*键
        /// 12    	#键
        /// 13    	A键
        /// 14    	B键
        /// 15    	C键
        /// 0     	D键
        /// </returns>
        /// <remarks>
        /// 在系统中，对应每一个通道都有一个64字节的DTMF缓冲区，当调用函数InitDtmfBuf时，系统会自动清空该DTMF缓冲区，以便用户程序使用；当底层驱动程序检测到有DTMF按键时，会自动将该按键的键值放入缓冲区内。用户调用本函数就可以得到DTMF按键值。
        /// </remarks>
        /// <seealso cref = "InitDtmfBuf"/>
        /// <seealso cref = "DtmfHit"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 GetDtmfCode(Int32 wChnlNo);
        
        /// <summary>
        /// 查看指定通道是否有DTMF按键。当收到一个有效的DTMF按键后，本函数返回TRUE。本函数并不会将按键从内部缓冲区中移去。若想要移去该按键，要调用函数GetDtmfCode。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <returns>
        ///  ture   缓冲区中有Dtmf按键
        ///  false  缓冲区中没有Dtmf按键
        /// </returns>
        /// <seealso cref = "GetDtmfCode"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool DtmfHit(Int32 wChnlNo);

        /// <summary>
        /// 发送DTMF(拨号)。“,”表示在拨号时，延时0.5秒。如：“0，3323577”表示先拨一个0，然后延时0.5秒，再拨3323577。发送的每个DTMF声音为125毫秒，间隔也为125毫秒。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <param name = "DialNum">拨号的字符串，有效值为“0”-“9”,“*”,“#”，“,” “ABCD”</param>
        /// <remarks>
        /// 本函数本质上是利用内存索引放音来实现的。若要中途停止拨号，可以用函数StopPlay；检测拨号是否完成，使用函数CheckSendEnd。如果要调整发送DTMF的速率，请使用函数NewSendDtmfBuf。
        /// 注意事项：
        /// 一次可以发送的DTMF字符串的最大长度为64个。发送DTMF在本质上也是放音，因此，需要不断的调用函数PUSH_PLAY。
        /// </remarks>
        /// <seealso cref = "CheckSendEnd"/>
        /// <seealso cref = "StopPlay"/>
        /// <seealso cref = "NewSendDtmfBuf"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void SendDtmfBuf(Int32 wChnlNo, byte[] DialNum);

        /// <summary>
        /// 检测某路发送DTMF是否完毕。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <param name = ""></param>
        /// <returns>
        ///  0      未发送完毕
        ///  1      已发送完毕
        /// </returns>
        /// <remarks></remarks>
        /// <seealso cref = "SendDtmfBuf"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool CheckSendEnd(Int32 wChnlNo);

        /// <summary>
        /// 设置发送DTMF码的速率。如果要用函数NewSendDtmfBuf来发送DTMF码，在初始化时必须使用本函数来设置发送的速率。一般本函数放在EnableCard之后即可。
        /// </summary>
        /// <param name = "ToneLen">DTMF码的时间长度（单位：毫秒），最大不能超过125毫秒</param>
        /// <param name = "SilenceLen">间隔的时间长度（单位：毫秒），最大不能超过125毫秒</param>
        /// <returns>
        ///  0      成功
        ///  1      失败
        /// </returns>
        /// <seealso cref = "SendDtmfBuf_Ex"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 SetSendPara(Int32 ToneLen, Int32 SilenceLen);

        /// <summary>
        /// 开始用指定的速率发送DTMF。“,”表示在拨号时，延时0.5秒。
        /// 本函数本质上是利用内存放音来实现的。若要中途停止拨号，可以用函数StopPlay；检测拨号是否完成，使用函数NewCheckSendEnd。
        /// </summary>
        /// <param name = "ChanelNo">通道号</param>
        /// <param name = "DialNum">拨号的字符串，有效值为“0”-“9”,“*”,“#”，“,” “ABCD”</param>
        /// <remarks>
        /// 注意事项：
        /// 注意第一个参数的类型为Int32（32），而不是通常的WORD（16位）。   
        /// 本函数本质上是利用内存放音来实现的。所以，一次可以发送的DTMF字符串的最大长度与EnableCard给每个通道分配的语音缓冲区大小有关，也与SetSendPara设置的速率快慢有关。
        /// 比如，在初始化时设定EnableCard(8,1024*40)、SetSendPara(50,50)。那么，每个DTMF需时100毫秒，每路语音缓冲区40K，相当于5秒，所以，DialNum一次最多可以有50个。
        /// </remarks>
        /// <seealso cref = "SetSendPara"/>
        /// <seealso cref = "NewCheckSendEnd"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void NewSendDtmfBuf(Int32 ChannelNo, byte[] DialNum);

        /// <summary>
        ///  对于用NewSendDtmfBuf函数开始的发送DTMF，本函数检查发送是否完毕。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <remarks>注意第一个参数的类型为Int32（32），而不是通常的WORD（16位）。</remarks>
        /// <seealso cref = "NewSendDtmfBuf"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 NewCheckSendEnd(Int32 wChnlNo);

        /// <summary>
        /// 某路开始新的信号音检测。一般在摘机或者挂机后，调用本函数来开始新的信号音检测。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <seealso cref = "StopSigCheck"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void StartSigCheck(Int32 wChnlNo);

        /// <summary>
        /// 停止某路的信号音检测。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <remarks></remarks>
        /// <seealso cref = "StartSigCheck"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void StopSigCheck(Int32 wChnlNo);

        /// <summary>
        /// 在调用了StartSigCheck函数之后, 就可以调用本函数来获得信号音的检测结果。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <param name = "wMode">检测的信号音类型</param>
        /// <remarks>
        /// Mode是指检测何种情况下的信号音，其可能的值为RECORD_CHECK,PLAY_CHECK, SEND_CHECK。
        /// RECORD_CHECK 和  PLAY_CHECK：对方挂机信号检测。在录音、放音或收号时, 检测对方的信号情况, 即查看对方是否挂机。
        /// RECORD_CHECK和PLAY_CHECK的功能相同。
        /// 用法：
        /// <example>ReadCheckResult (ChannelNo, RECORD_CHECK);</example>
        /// <example>ReadCheckResult (ChannelNo, PLAY_CHECK);</example>
        /// 返回值：
        /// R_BUSY    ： 对方挂机，检测到忙音
        /// R_OTHER   ： 未检测到挂机音号
        /// 在调用本函数之前，首先应设定您要检测的忙音参数。使用函数SetBusyPara(700)；若要检测多个忙音，请参阅“对方挂机检测”。
        /// SEND_CHECK : 拨号以后的信号音检测。这种情况比较复杂, 也最容易出问题。所以用户应仔细理解各个参数的含义。
        /// 用法：
        /// <example>ReadCheckResult(ChannelNo, SEND_CHECK);</example>
        /// 返回值：
        /// S_NORESULT： 尚未得出结果
        /// S_BUSY    ： 检测到对方占线的忙音
        /// S_CONNECT ： 对方摘机，可以进行通话
        /// S_NOBODY  ： 振铃若干次，无人接听电话
        /// S_NOSIGNAL： 拨完号后，没有任何信号音
        /// 在调用本函数之前，首先应设定您要检测的忙音参数，使用函数SetDialPara(1000,4000,350,7);
        /// 注意此函数仅支持450Hz信号音检测，其它信号音检测请用“第七章 D161A新信号音函数”指定的函数。
        /// </remarks>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 ReadCheckResult(Int32 wChnlNo, Int32 wMode);

        /// <summary>
        /// 获取当前最大连续忙音的个数。该函数所对应的通道是刚才调用ReadCheckResult函数时的通道。
        /// </summary>
        /// <returns>
        /// 当前最大连续忙音的个数。
        /// </returns>
        /// <seealso cref = "ReadCheckResult"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 ReadBusyCount();

        /// <summary>
        /// 检查某一通道的极性。极性检查可以用来判断拨号后对方是否摘机。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <returns>
        ///  ture   有极性
        ///  false  无极性
        /// </returns>
        /// <remarks>
        /// 当用某通道向外拨号时，首先摘机，延时一秒后调用本函数记录极性的值；然后拨号，拨号完毕后，当检测到极性改变时，说明用户摘机。
        /// 注意一般的市话线路，不具有反极性的功能，需要向电信局申请。
        /// </remarks>
        /// <seealso cref = "StartSigCheck"/>
        /// <seealso cref = "StopSigCheck"/>
        /// <seealso cref = " ReadCheckResult"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool CheckPolarity(Int32 wChnlNo);

        /// <summary>
        /// 检测线路的静音情况。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <param name = "wCheckNum">检测的信号音个数，有效值为1～511</param>
        /// <returns>
        /// -1      信号音缓冲区中的个数还不足wCheckNum个
        /// 0～wCheckNum wCheckNum个信号音采样中，1的个数。
        /// </returns>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 CheckSilence(Int32 wChnlNo, Int32 wCheckNum);

        #endregion

        #region 内线振铃及检测函数

        /// <summary>
        /// 将lpFileName读取到内部缓冲区，在lpFileName 中应含有一段信号音，系统将会使用该段声音来产生拨号音、忙音、回铃音等信号音。在系统内部有一个缺省的信号音，用户也可以自行录制喜欢的信号音，然后用本函数来替换这个缺省的信号音。
        /// </summary>
        /// <param name = "lpFileName">信号音文件名</param>
        /// <returns>
        ///  ture   成功
        ///  false  失败
        /// </returns>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool ReadGenerateSigBuf(byte[] lpFileName);

        /// <summary>
        /// 对某一路内线通道馈电，同时停止馈铃流。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <seealso cref = "FeedRing"/>
        /// <seealso cref = "FeedRealRing"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void FeedPower(Int32 wChnlNo);

        /// <summary>
        /// 对某一路内线通道馈连续的铃流。调用本函数后，本通道所连接的电话机将会不停的振铃，直到调用函数FeedPower才会停止。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <remarks>一定要正确的接入铃流源，电话机才能振铃。</remarks>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void FeedRing(Int32 wChnlNo);

        /// <summary>
        /// 维持断续振铃及信号音的函数；请在程序大循环中调用。
        /// </summary>
        /// <seealso cref = "PUSH_PLAY"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void FeedSigFunc();

        /// <summary>
        /// 对某一路内线通道馈断续的铃流。断续的时间长度为响铃0.75秒，停止3秒。若要停止断续的铃流请使用函数FeedPower。在本通道正在振铃的情况下，检测摘机必须使用函数OffHookDetect，而不能使用函数RingDetect。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <remarks>一定要不断的调用函数FeedSigFunc，才能保证产生断续的铃流。</remarks>
        /// <seealso cref = "OffHookDetect"/>
        /// <seealso cref = "FeedSigFunc"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void FeedRealRing(Int32 wChnlNo);

        /// <summary>
        /// 检测某一路内线通道的摘机状态，当调用FeedRealRing函数开始一个断续的铃流后，请调用本函数来检测摘机状态。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <returns>
        ///  ture   已摘机
        ///  false  未摘机
        /// </returns>
        /// <seealso cref = "FeedRealRing"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool OffHookDetect(Int32 wChnlNo);

        /// <summary>
        /// 控制信号音的播放。本函数实质上使用内存循环放音来实现的。
        /// 其中：
        /// 拨号音的时间长度为响0.75秒，停止3秒；
        /// 忙音一的时间长度为响0.35秒，停止0.35秒；
        /// 忙音二的时间长度为响0.7秒，停止0.7秒；
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <param name = "SigType">信号音类型，可以有如下值：
        /// SIG_STOP       停止播放信号音
        /// SIG_DIALTONE   拨号音
        /// SIG_BUSY1      忙音一
        /// SIG_BUSY2      忙音二
        /// SIG_RINGBACK   回铃音
        /// </param>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void StartPlaySignal(Int32 wChnlNo, Int32 SigType );

        /// <summary>
        /// 某一通道开始挂机检测；当某通道摘机后，可以调用本函数。该函数只对内线通道有效。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <seealso cref = "HangUpDetect"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void StartHangUpDetect(Int32 wChnlNo);

        /// <summary>
        /// 检测某一通道的挂机状态；该函数需要在调用 StartHangUpDetect之后使用。如果你需要检测拍叉簧，请使用本函数。
        /// 另外，有的电话机在摘机时，会有抖动。如果使用函数RingDetect来检测其摘机和挂机，可能会出现刚摘机就断线的情况，此时，也可以用本函数来避免这种情况。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <param name = ""></param>
        /// <returns>
        ///  HANG_UP_FLAG_FALSE  	没有挂机
        ///  HANG_UP_FLAG_TRUE    　已经挂机（从进入HANG_UP_FLAG_START状态开始，挂机时间大于0.5秒。）
        ///  HANG_UP_FLAG_START  	开始挂机
        ///  HANG_UP_FLAG_PRESS_R   拍了一下叉簧
        /// </returns>
        /// <remarks> 该函数只对内线通道有效。</remarks>
        /// <seealso cref = "StartHangUpDetect"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 HangUpDetect(Int32 wChnlNo);

        /// <summary>
        /// 某通道启动一个计时器。
        /// </summary>
        /// <param name = "wChnlNo">通道类型</param>
        /// <param name = "ClockType">计时器号(用户可用的为3～9)</param>
        /// <remarks>不能使用ClockType为0~2的计时器。</remarks>
        /// <seealso cref = ""/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 StartTimer(Int32 wChnlNo, Int32 ClockType);

        /// <summary>
        /// 本函数返回从计时器启动到现在的时间，单位0.01秒。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <param name = "ClockType">计时器类型</param>
        /// <returns>
        /// 从计时器启动到现在的时间，单位0.01秒。
        /// </returns>
        /// <remarks>本函数返回值的单位是0.01秒，而不是毫秒。这是为了与以前在DOS和WIN31下的函数兼容。</remarks>
        /// <seealso cref = "StartTimer"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 ElapseTime( Int32 wChnlNo, Int32 ClockType);

        #endregion

        #region 连通函数

        /// <summary>
        /// 两路连通。  当one、another不在同一片卡上时，在INI中应设定Connect=1，并且卡与卡之间要有电缆连接。
        /// </summary>
        /// <param name = "wOne">通道号一(0~255)</param>
        /// <param name = "wAnOther">通道号二(0~255)</param>
        /// <returns>
        ///  0     	成功
        /// -1    	通道号一 超出范围 
        /// -2    	通道号二 超出范围
        /// -3      当卡之间没有连接电缆时，通道号一和通道号二不在同一块卡上，无法连通
        /// -4   	通道号一 正在放音
        /// -5    	通道号二 正在放音
        /// </returns>
        /// <remarks>
        /// 两个通道正在连通时，如果对某一个通道进行放音StartPlay或停止放音StpPlay，将会变成单向连通。因此，在调用本函数之前，必须首先停止放音，并保证在连通的过程中不对任一个通道放音或停止放音。
        /// 由于向外拨号、播放信号音在本质上都属于放音，因此，以上对放音的要求对这些函数也同样适用。
        /// </remarks>
        /// <seealso cref = "ClearLink"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 SetLink(Int32 wOne, Int32 wAnOther);

        /// <summary>
        /// 两路拆除连通。
        /// </summary>
        /// <param name = "wOne">通道号一(0~255)</param>
        /// <param name = "wAnOther">通道号二(0~255)</param>
        /// <returns>
        ///  0     	成功
        /// -1    	通道号一 超出范围 
        /// -2    	通道号二 超出范围
        /// -3      当卡之间没有连接电缆时，通道号一和通道号二不在同一块卡上，无法连通
        /// -4   	通道号一 正在放音
        /// -5    	通道号二 正在放音
        /// </returns>  
        /// <seealso cref = "SetLink"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 ClearLink(Int32 wOne, Int32 wAnOther);

        /// <summary>
        /// 两路单向连通。调用该函数后，实现单向连通；通道One可以听到Another的声音，但Another听不到One的声音。
        /// </summary>
        /// <param name = "wOne">通道号一(0~255)</param>
        /// <param name = "wAnOther">通道号二(0~255)</param>
        /// <returns>
        ///  0     	成功
        /// -1    	通道号一 超出范围 
        /// -2    	通道号二 超出范围
        /// -3      当卡之间没有连接电缆时，通道号一和通道号二不在同一块卡上，无法连通
        /// -4   	通道号一 正在放音
        /// -5    	通道号二 正在放音   
        /// </returns>
        /// <remarks>参阅函数SetLink的注意事项。</remarks>
        /// <seealso cref = "ClrOneFromAnother"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 LinkOneToAnother(Int32 wOne, Int32 wAnOther);

        
        /// <summary>
        /// 两路拆除单向连通
        /// </summary>
        /// <param name = "wOne">通道号一(0~255)</param>
        /// <param name = "wAnOther">通道号二(0~255)</param>
        /// <returns>
        ///  0     	成功
        /// -1    	通道号一 超出范围 
        /// -2    	通道号二 超出范围
        /// -3      当卡之间没有连接电缆时，通道号一和通道号二不在同一块卡上，无法连通
        /// -4   	通道号一 正在放音
        /// -5    	通道号二 正在放音   
        /// </returns>
        /// <seealso cref = "LinkOneToAnother"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 ClearOneFromAnother(Int32 wOne, Int32 wAnOther);

        /// <summary>
        /// 三方连通。本函数实质上是调用三次单向连通函数LinkOneToAnother。
        /// </summary>
        /// <param name = "wOne">连通的第一路通道号</param>
        /// <param name = "wTwo">连通的第二路通道号</param>
        /// <param name = "wThree">连通的第三路通道号</param>
        /// <returns>
        ///  0      成功
        /// -1    	通道号一 超出范围　
        /// -2    	通道号二 超出范围
        /// -3      当卡之间没有连接电缆时，通道号一、通道号二和通道号三不在同一块卡上，无法连通
        /// -4   	通道号一 正在放音
        /// -5    	通道号二 正在放音
        /// -6    	通道号三 超出范围
        /// -7    	通道号三 正在放音
        /// </returns>
        /// <remarks> 由于三方连通是用单向连通来实现的，因此，A听B的声音会比较的小；类似的情况也会出现在B和C上。如果想实现多方通话，请参考“会议实现”。</remarks>
        /// <seealso cref = "ClearThree"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 LinkThree(Int32 wOne, Int32 wTwo, Int32 wThree);

        /// <summary>
        /// 拆除三方连通。
        /// </summary>
        /// <param name = "wOne">连通的第一路通道号</param>
        /// <param name = "wTwo">连通的第二路通道号</param>
        /// <param name = "wThree">连通的第三路通道号</param>
        /// <returns>
        ///  0      成功
        /// -1    	通道号一 超出范围　
        /// -2    	通道号二 超出范围
        /// -3      当卡之间没有连接电缆时，通道号一、通道号二和通道号三不在同一块卡上，无法连通
        /// -4   	通道号一 正在放音
        /// -5    	通道号二 正在放音
        /// -6    	通道号三 超出范围
        /// -7    	通道号三 正在放音
        /// </returns>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 ClearThree(Int32 wOne, Int32 wTwo, Int32 wThree);

        #endregion

        #region 收主叫号码有关的函数

        /// <summary>
        /// 初始化某路的Caller-ID缓冲区。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <seealso cref = "GetCallerIDStr"/>
        /// <seealso cref = "GetCallerIDRawStr"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void ResetCallerIDBuffer(Int32 wChnlNo);
        
        /// <summary>
        /// 获得Caller-ID的内容。IDStr必须由用户来分配空间，并保证足够存放所有的主叫号码，128个字节是绝对安全的。
        /// 当返回值等于3或4时，CallerIdStr 中存有接收到的CallerID。目前国标中规定的单一格式或复合格式的FSK主叫号码，本函数都可以正确的接收。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <param name = "IDStr">接收到的主叫号码信息</param>
        /// <returns>
        /// 接收的主机号码情况
        /// #define ID_STEP_NONE          0       //未收到任何信息
        /// #define ID_STEP_HEAD          1       //正在接收头信息
        /// #define ID_STEP_ID            2       //正在接收ID号码
        /// #define ID_STEP_OK            3       //接收完毕，校验正确
        /// #define ID_STEP_FAIL          4       //接收完毕，校验错误
        /// </returns>
        /// <remarks>由于FSK是在第一声振铃之后发送的（在有些地方甚至是在第二声振铃之后发送的），因此，不能一检测到振铃就摘机。在检测到有振铃时，必须首先调用ResetCallerIDBuffer，当本函数返回3或4时，才能摘机（OffHook）。另外，还需要设定计时器，当一定时间内收不到主叫号码时，再摘机。</remarks>
        /// <seealso cref = "ResetCallerIDBuffer"/>
        /// <seealso cref = "GetCallerIDRawStr"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 GetCallerIDStr(Int32 wChnlNo, byte[] IDStr);
        
        /// <summary>
        /// 这个函数在GetCallerIDStr的基础上，进一步将日期时间、主叫号码、主叫用户信息分解出来。StrTime, strCallerID, strUser三个参数，需要用户自己分配地址空间，将指针传给本函数。
        /// 调用完成后，得到的信息就保存在相应的缓冲区中。如果某个缓冲区内容为空，比如strCallID为空，说明没有接收到该信息。主叫号码没有接收到，通常是因为对方设置为不发主叫号码。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <param name = "strTime">接收到的日期时间信息</param>
        /// <param name = "strCallerID">接收到的主叫号码</param>
        /// <param name = "strUser">接收到的主叫用户信息</param>
        /// <returns>
     　 /// 接收的主机号码情况
        /// #define ID_STEP_NONE          0       //未收到任何信息
        /// #define ID_STEP_HEAD          1       //正在接收头信息
        /// #define ID_STEP_ID            2       //正在接收ID号码
        /// #define ID_STEP_OK            3       //接收完毕，校验正确
        /// #define ID_STEP_FAIL          4       //接收完毕，校验错误
        /// </returns>
        /// <remarks>由于FSK是在第一声振铃之后发送的（在有些地方甚至是在第二声振铃之后发送的），因此，不能一检测到振铃就摘机。在检测到有振铃时，必须首先调用ResetCallerIDBuffer，当本函数返回3或4时，才能摘机（OffHook）。另外，还需要设定计时器，当一定时间内收不到主叫号码时，再摘机。</remarks>
        /// <seealso cref = "ResetCallerIDBuffer"/>
        /// <seealso cref = "GetCallerIDRawStr"/>
        /// <seealso cref = "GetCallerIDStr"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 GetCallerIDStrEx(Int32 wChnlNo, byte[] strTime, byte[] strCallerID, byte[] strUser);
  
        /// <summary>
        /// 获得收到的Caller-ID的原始内容。IDRAWStr必须由用户来分配空间，并保证足够存放所有的主叫号码，128个字节是绝对安全的。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <param name = "IDRawStr">存放接收到的CallerID原始信息串</param>
        /// <returns>
        /// 目前已经收到的CallerID原始信息串的个数。
        /// </returns>
        /// <seealso cref="ResetCallerIDBuffer"/>
        /// <seealso cref="GetCallerIDStr"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 GetCallerIDRawStr(Int32 wChnlNo, byte[] IDRawStr);

        #endregion

        #region 会议功能

        /// <summary>
        /// 初始化会议功能，调用本函数将初始化DLL的内部变量。
        /// 在系统中，一共有32个会议资源，最多可以有10组会议，每组会议最多可以由6个成员。
        /// </summary>
        /// <returns>
        /// 0						● 成功
        /// 1						● 不是D161A卡
        /// 2						● 在INI中，Connect必须是1
        /// 3						● 已经使用了模拟的会议卡，并且初始化成功
        /// </returns>
        /// <seealso cref="DConf_DisableConfCard"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DConf_EnableConfCard();

        /// <summary>
        /// 禁止会议功能，程序退出时调用。
        /// </summary>
        /// <seealso cref="DConf_EnableConfCard"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void DConf_DisableConfCard();

        /// <summary>
        /// 得到当前总的可用的会议资源数。参见函数DConf_EnableConfCard中对会议资源占用的说明。
        /// </summary>
        /// <returns>可用的会议资源数</returns>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DConf_GetResNumber();

        /// <summary>
        /// 使能DTMF抑制
        /// </summary>
        /// <param name="wChnl">通道号</param>
        /// <param name="wCtrl">0或者1，1表使能DTMF抑制</param>
        /// <returns>
        ///  0						● 成功
        /// -1						●  通道号错误
        /// -2						●  wCtrl错误
        /// -3						●  该通道未加入会议
        /// </returns>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DConf_Adjust_CtrlWord(long wChnl, Int32 wCtrl);

        /// <summary>
        ///     将一个通道加入某组会议。调用本函数后，系统会在32个会议资源中分配一个资源，然后利用这
        /// 个资源把通道加入本组会议。一个通道加入某组会议后，该通道可以听到会议的内容，它的声音
        /// 也可以被其他的成员听到。
        ///     参数ChnlAtte表示加入会议的增益，为了防止整个会议声音的溢出，一般对于用户通道要衰减-6db。
        /// 为了与以前模拟会议卡的兼容，如果本参数为 ATTE_MINUS_3DB(0X40) 或ATTE_MINUS_6DB(0X80)时，
        /// 系统会自动转换为-3或-6。
        /// </summary>
        /// <param name="ConfNo">会议的组号，有效值1~10</param>
        /// <param name="ChannelNo">通道号</param>
        /// <param name="ChnlAtte">增益调整，有效值-20db到+20db</param>
        /// <param name="NoiseSupp">等于0xCD，表示该通道只能说不能听</param>
        /// <returns>
        /// 0							● 成功
        /// 1							● ConfNo越界
        /// 2							● ChannelNo越界
        /// 3							● 没有可用的资源
        /// </returns>
        /// <seealso cref="SubChnl"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 AddChnl(Int32 ConfNo, Int32 ChannelNo, Int32 ChnlAtte, Int32 NoiseSupp);

        /// <summary>
        /// 将一个通道从某组会议中去掉。调用本函数将释放本通道占用的一个资源。
        /// </summary>
        /// <param name="ConfNo">会议的组号，有效值1~10</param>
        /// <param name="ChannelNo">通道号</param>
        /// <returns>
        /// 0							● 成功
        /// 1							● ConfNo越界
        /// 2							● ChannelNo越界
        /// 3							● 根据ChannelNo找到的资源非法
        /// </returns>
        /// <seealso cref="AddChnl"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 SubChnl(Int32 ConfNo, Int32 ChannelNo);

        /// <summary>
        /// 一个通道聆听某组会议。
        /// 对于某组会议，当第一个通道调用本函数时，将占用一个资源。以后，所有聆听本组会议的通道，将
        /// 共用本资源。所以，某组会议的所有聆听通道只占用一个资源。
        /// </summary>
        /// <param name="ConfNo">会议的组号，有效值1~10</param>
        /// <param name="ChannelNo">通道号</param>
        /// <returns>
        /// 0							● 成功
        /// 1							● ConfNo越界
        /// 2							● ChannelNo越界
        /// 3							● 没有可用的资源
        /// </returns>
        /// <seealso cref="SubListenChnl"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 AddListenChnl(Int32 ConfNo, Int32 ChannelNo);

        /// <summary>
        /// 一个通道去掉对某组会议的聆听。
        /// 对于某组会议，所有聆听通道只占用一个资源。当最后一个聆听的通道调用本函数时，将释放该资源。
        /// </summary>
        /// <param name="ConfNo">会议的组号，有效值1~10</param>
        /// <param name="ChannelNo"> 通道号</param>
        /// <returns>
        /// 0							● 成功
        /// 1							● ConfNo越界
        /// 2							● ChannelNo越界
        /// 3							● 根据ChannelNo找到的资源非法
        /// </returns>
        /// <seealso cref="AddListenChnl"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 SubListenChnl(Int32 ConfNo, Int32 ChannelNo);

        /// <summary>
        /// 将一个通道的录音转变为对会议的录音。本函数占用一个资源。
        /// 如果一个通道加入了某组会议，你可以用StartRecordFile对该通道进行录音，但本录音并不是对该
        /// 通道所在的会议的录音；此时，你可以调用本函数来强制对会议进行录音。
        /// </summary>
        /// <param name="ConfNo">会议的组号，有效值1~10</param>
        /// <param name="ChannelNo">通道号</param>
        /// <returns>
        /// 0							● 成功
        /// 1							● ConfNo越界
        /// 2							● ChannelNo越界
        /// 3							● 没有可用的资源
        /// 4							● 不是使用D161A内置会议功能
        /// </returns>
        /// <remarks>本函数必须在函数StartRecordFile之后调用。</remarks>
        /// <seealso cref="DConf_SubRecListenChnl"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DConf_AddRecListenChnl(Int32 ConfNo, Int32 ChannelNo);

        /// <summary>
        /// 将对会议的录音去掉，恢复为对通道的录音,本函数释放一个资源。在对一个会议的录音结束后，需
        /// 要调用本函数。
        /// </summary>
        /// <param name="ConfNo">会议的组号，有效值1~10</param>
        /// <param name="ChannelNo">通道号</param>
        /// <returns>
        /// 0							● 成功
        /// 1							● ConfNo越界
        /// 2							● ChannelNo越界
        /// 3							● 没有可用的资源
        /// 4							● 不是使用D161A内置会议功能
        /// </returns>
        /// <remarks>本函数必须在函数StopRecordFile之后调用。</remarks>
        /// <seealso cref="DConf_AddRecListenChnl"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DConf_SubRecListenChnl(Int32 ConfNo, Int32 ChannelNo);

        /// <summary>
        /// 将对一个通道的放音转变为对会议的放音。本函数占用一个资源。
        /// 如果一个通道加入了某组会议，你可以调用本函数，在此之后的所有放音操作将重定向到该组会议中。
        /// 本函数必须在函数StartPlayFile之前调用。
        /// </summary>
        /// <param name="ConfNo">会议的组号，有效值1~10</param>
        /// <param name="ChannelNo">通道号</param>
        /// <param name="ChnlAtte">增益调整，有效值-20db到+20db</param>
        /// <param name="NoiseSupp">保留</param>
        /// <returns>
        /// 0							● 成功
        /// 1							● ConfNo越界
        /// 2							● ChannelNo越界
        /// 3							● 没有可用的资源
        /// 4							● 不是使用D161A内置会议功能
        /// </returns>
        /// <remarks>
        ///     使用时考虑以下情景：会议组号为3，加入了通道1、5、9。如果需要对会议放音，则可以在
        /// 1、5、9这三个中任意选取一个，假定选择5。那么，调用函数DConf_AddPlayChnl(3,5…就可以将通道
        /// 5的放音重定向到会议组3中，以后所有对通道5的放音操作都将会被加入该组会议的成员听到，你可以
        /// 按照需要多次进行放音/停止放音的操作。如果需要将通道5设定回正常的方式，则调用函数
        /// DConf_SubPlayChnl(3,5)，此后的放音将会对通道5进行。
        ///     注意：同一个语音通道不能既放音又录音，因此，刚才的情景中，当采用了通道5对会议放音以后，
        /// 如果还需要对整个会议录音，那么，就必须使用通道1、9中的一个。
        /// </remarks>
        /// <example>
        /// 	AddChnl ( ch, chnl, 0, NOISE_NONE  );
        /// 	DConf_AddPlayChnl ( ch, chnl, -6, NOISE_NONE );
        /// 	sprintf(FileName, "%s\\bank.001", VoicePath);
        /// 	StartPlayFile ( chnl, FileName, 0 );
        /// </example>
        /// <seealso cref="DConf_SubPlayChnl"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DConf_AddPlayChnl(Int32 ConfNo, Int32 ChannelNo, Int32 ChnlAtte, Int32 NoiseSupp);
        
        /// <summary>
        /// 将某通道对会议的放音去掉，恢复为对通道的放音。本函数释放一个资源。
        /// </summary>
        /// <param name="ConfNo">会议的组号，有效值1~10</param>
        /// <param name="ChannelNo">通道号</param>
        /// <returns>
        /// 0							● 成功
        /// 1							● Confo越界
        /// 2							● ChannelNo越界
        /// 3							● 没有可用的资源
        /// 4							● 不是使用D161A内置会议功能
        /// </returns>
        /// <example>
        ///	    if ( CheckPlayEnd ( chnl ) )
        ///	    {
        ///	        StopPlayFile ( chnl );
        ///	        ch = ChGroup[chnl];
        ///	        DConf_SubPlayChnl ( ch, chnl );
        ///	    }
        /// </example>
        /// <remarks>本函数必须在函数StopPlayFile之后调用。</remarks>
        /// <seealso cref="DConf_AddPlayChnl"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DConf_SubPlayChnl(Int32 ConfNo, Int32 ChannelNo);

        /// <summary>
        ///     将一个时隙通道加入某组会议。调用本函数后，系统会在32个会议资源中分配一个资源，然后利
        /// 用这个资源把时隙通道加入本组会议。
        ///     当加入会议成功后，对应的输出时隙将返回在pTS_CONF所指向的一个WORD变量中，应用程序必须
        /// 记住该WORD，以便在退出会议时使用。
        ///     参数ChnlAtte表示加入会议的增益，有效值-20db到+20db。
        /// </summary>
        /// <param name="ConfNo">会议的组号，有效值1~10</param>
        /// <param name="wTimeSlot">要加入会议的时隙号</param>
        /// <param name="ChnlAtte">增益调整，有效值-20db到+20db</param>
        /// <param name="NoiseSupp">保留</param>
        /// <param name="TS_CONF">指针，指向输出的时隙号</param>
        /// <returns>
        /// 0							● 成功
        /// 1							● ConfNo越界
        /// 2							● wTimeSlot越界
        /// 3							● 没有可用的会议资源
        /// 4       					● 没有成功初始化
        /// 5							● 不是PCI接口的模拟语音卡
        /// 6							● 加入会议时失败
        /// </returns>
        /// <example>
        ///     Int32		TestConfGroup = 1;			// 测试用的会议组号，可以任选1-10
        ///     WORD	TS_IP;
        ///     WORD	TS_CONF;
        ///     // 获取IP通道时隙
        ///     // pIPChn->number记录实际的IP通道号码
        ///     TS_IP  = DJH323_GetTimeSlot(pIPChn->number);	
        ///     if (TS_IP < 0) 
        ///     {
        ///     	return -3;		//	printf("Get DIP-PCI timeslot fail!\n");
        ///     }
        ///     // 将IP通道的时隙加入会议
        ///     // 通道时隙加入会议成功后，变量TS_CONF中存有输出时隙
        ///     rrr = DConf_AddChnl_TimeSlot ( TestConfGroup, TS_IP, 0, NOISE_NONE, &TS_CONF );
        ///     if ( rrr != 0 )
        ///     {
        ///     	return -4;
        ///     }
        ///     //连接IP通道和会议的输出时隙
        ///     if (DJH323_ConnectFromTS (pIPChn->number, TS_CONF) < 0)
        ///     {
        ///     	return -5;
        ///     }
        ///     pIPChn->wTS_CONF_Out = TS_CONF;		// 记住该输出时隙 
        ///     pIPChn->tsConnected = TRUE;
        ///     return 0;		// OK
        /// </example>
        /// <seealso cref="DConf_SubChnl_TimeSlot"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DConf_AddChnl_TimeSlot(Int32 ConfNo, long wTimeSlot, Int32 ChnlAtte, Int32 NoiseSupp, long[] TS_CONF);

        /// <summary>
        /// 聆听某组会议。
        /// 调用本函数后，系统会在32个会议资源中分配一个资源，然后利用这个资源把本组会议输出到一个时隙。
        /// 当本函数调用成功后，对应的输出时隙将返回在pTS_CONF所指向的一个WORD变量中，应用程序必须记住
        /// 该WORD，以便在退出会议时使用。可以有多个通道利用该输出时隙来听本组会议。
        /// </summary>
        /// <param name="ConfNo">会议的组号，有效值1~10</param>
        /// <param name="TS_CONF">指针，指向输出的时隙号</param>
        /// <returns>
        /// 0							● 成功
        /// 1							● ConfNo越界
        /// 2							● 保留
        /// 3							● 没有可用的会议资源
        /// 4 				    		● 没有成功初始化
        /// 5							● 不是PCI接口的模拟语音卡
        /// 6							● 加入会议时失败
        /// </returns>
        /// <example>
        ///     Int32		TestConfGroup = 1;			// 测试用的会议组号，可以任选1-10
        ///     WORD	TS_CONF;
        ///     // 听会议，成功后，变量TS_CONF中存有该会议的输出时隙
        ///     rrr = DConf_AddListenChnl_TimeSlot ( TestConfGroup, &TS_CONF );
        ///     if ( rrr != 0 )
        ///     {
        ///     	return -4;
        ///     }
        ///     //连接IP通道和会议的输出时隙
        ///     if (DJH323_ConnectFromTS (pIPChn->number, TS_CONF) < 0)
        ///     {
        ///     	return -5;
        ///     }
        ///     pIPChn->wTS_CONF_Out = TS_CONF;		// 记住该输出时隙 
        ///     pIPChn->tsConnected = TRUE;
        ///     return 0;		// OK
        /// </example>
        /// <seealso cref="DConf_SubChnl_TimeSlot"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DConf_AddListenChnl_TimeSlot(Int32 ConfNo, long[] TS_CONF);

        /// <summary>
        /// 将一个时隙通道从某组会议中去掉。调用本函数将释放本时隙通道占用的一个会议资源。
        /// 函数DConf_AddChnl_TimeSlot或者DConf_AddListenChnl_TimeSlot调用成功后，将会有一个输出的时
        /// 隙通道号。本函数使用该时隙通道号来退出会议。
        /// </summary>
        /// <param name="ConfNo">会议的组号，有效值1~10</param>
        /// <param name="TS_Out">会议的输出时隙</param>
        /// <returns>
        /// 0							● 成功
        /// 1							● ConfNo越界
        /// 2							● wTS_Out越界
        /// 3                       	● 根据wTS_Out找到的资源非法
        /// 4                       	● 没有成功初始化
        /// 5                       	● 不是PCI接口的模拟语音卡
        /// </returns>
        /// <example>
        ///     Int32		TestConfGroup = 1;			// 测试用的会议组号，可以任选1-10
        ///     // 时隙从会议退出
        ///     rrr = DConf_SubChnl_TimeSlot ( TestConfGroup, pIPChn->wTS_CONF_Out );
        ///     if ( rrr != 0 )
        ///     {
        ///     	return -4;
        ///     }
        ///     //断开IP通道和会议时隙的连接
        ///     DJH323_DisconnectTS(pIPChn->number);
        ///     pIPChn->tsConnected = FALSE;
        /// </example>
        /// <seealso cref="DConf_AddChnl_TimeSlot"/>
        /// <seealso cref="DConf_AddListenChnl_TimeSlot"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DConf_SubChnl_TimeSlot(Int32 ConfNo, long TS_Out);

        #endregion 

        #region 信号音函数

        /// <summary>
        /// 完成信号音检测的初始化工作。该函数将从Windows目录下的NewSig.ini文件中读取信号音检测的配置信息，根据配置参数的情况对硬件进行必要的设置，即调整由用户自行设置的２个信号音的频率，可能只用到其中１个，也可能２个都用到。
        /// </summary>
        /// <param name = "Times"> 缺省值为0</param>
        [DllImport("NewSig.dll", CharSet = CharSet.Auto)]
        public static extern void Sig_Init(Int32 Times);
        
        /// <summary>
        /// 对某通道进行挂机忙音检测。该函数可同时检测１种或2种忙音，该值可在配置文件中设置。如该值设为２，则检测到其中任何１种忙音时，都会返回１。
        /// 针对某种忙音，需描述其频率、门槛值、响声长度、静音长度、长度的偏差范围及最少忙音数。当该种频率的声音能量值大于门槛值时，认为该时刻为响声，否则为静音。如响声长度在预期的范围内，则认为检测到１个响声，如静音长度在预期范围内，则认为检测到１个静音。连续检测到的响声数达到配置参数中的最少忙音数时，函数返回１，认为已检测到挂机忙音。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <returns>
        ///  0      未检测到忙音
        ///  1      已检测到忙音
        /// </returns>
        /// <remarks>需注意的是在进行信号音检测之前必须调用函数StartSigCheck启动某通道的信号音采集过程，并且在程序运行过程中需循环调用函数FeedSigFunc维持信号音采集过程。</remarks>
        /// <seealso cref = "Sig_Init"/>
        /// <seealso cref = "Sig_ResetCheck"/>
        [DllImport("NewSig.dll", CharSet = CharSet.Auto)]
        public static extern Int32 Sig_CheckBusy(Int32 wChnlNo);
        
        /// <summary>
        /// 开始某通道的呼出过程。该函数只是设置通道的呼出缓冲区，真正的呼出过程需循环调用Sig_CheckDial函数来逐步完成。
        /// 如本机电话为内线电话，需先拨某一号码(如’9’)才能拨外线，这时需将参数PreDialNum置为要先拨的转外线号码，如’9’,DialNum参数只需传递要呼出的外线号码即可。如不需先拨某号码即可直接拨外线号码，可将参数PreDialNum置为空串。
        /// 配置文件中的呼出模式数，一般设置为3。对于某种呼出模式，我们可以设置它的频率、门槛值、占线忙音相关参数、回铃音相关参数等参数。
        /// 与挂机忙音检测不同的是，呼出结果的检测同时只能按某１种模式进行检测。Sig_StartDial中的第４个参数wMode即为要选择的检测模式。可取0，1标准模式，或者2双回铃音模式。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        /// <param name = "DialNum">呼出号码</param>
        /// <param name = "PreNum">前导号码</param>
        /// <param name = "wMode">呼出检测的模式选择</param>
        /// <returns>
        ///  0      失败
        ///  1      成功
        /// </returns>
        /// <seealso cref = "Sig_Init"/>
        /// <seealso cref = "Sig_CheckDial"/>
        [DllImport("NewSig.dll", CharSet = CharSet.Auto)]
        public static extern Int32 Sig_StartDial(Int32 wChnlNo, byte[] DialNum, byte[] PreNum, Int32 wMode);
        
        /// <summary>
        /// 在系统中调用此函数可以根据参数nCadenceType的类型得到该类型信号音的个数。
        /// </summary>
        /// <param name = "wChNo">通道号</param>
        /// <param name = "nCadenceType">信号音的类型
        /// SIG_CADENCE_BUSY     1			忙音
        /// SIG_CADENCE_RINGBACK 2  		回铃音
        /// </param>
        /// <returns>
        /// 返回为检测到的nCadenceType信号音的个数
        /// </returns>
        [DllImport("NewSig.dll", CharSet = CharSet.Auto)]
        public static extern Int32 Sig_GetCadenceCount(Int32 wChNo, Int32 nCadenceType);

        /// <summary>
        /// 在调用函数Sig_StartDial启动拨号过程后，就可以循环调用Sig_CheckDial函数维持拨号过程，并检测呼出的结果，直至得到结果为止。
        /// </summary>
        /// <param name = "wChnlNo"></param>
        /// <param name = ""></param>
        /// <returns>
        ///  S_NORESULT     尚未得出结果
        ///  S_NODIALTONE   没有拨号音
        ///  S_BUSY         检测到对方占线的忙音
        ///  S_CONNECT      对方摘机，可以进行通话
        ///  S_NOBODY       振铃若干次，无人接听电话
        ///  S_NOSIGNAL     没有信号音
        /// </returns>
        /// <remarks>
        /// 拨号的一般过程为：
        /// 1、如参数PreDialNum不为空，则延迟１秒后拨出PreDialNum,如PreDialNum为空，则直接进入步骤３。
        /// 2、检测PreDialNum是否已发完。如已发完转至步骤３。
        /// 3、检测是否有拨号音，如拨号音长度达到配置项DialToneAfterOffHook的数值，则发送DialNum码串，并转至步骤４。如在此步骤已等待配置项NoDialToneAfterOffHook定义的时间长度仍未检测到拨号音，则返回S_NODIALTONE。
        /// 4、检测DialNum串是否发完，如已发完则延迟StartDelay配置项的时间长度后进入步骤５。
        /// 5、如从进入此步骤起已经过配置项RingLen定义的时间长度，拨号音仍未停止则返回S_NOSIGNAL;如在此步骤已等待配置项NoRingLen定义的时间长度仍未检测到回铃音则返回S_NOSIGNAL;如检测到占线忙音数达到配置项BusySigCount定义的数字，则返回S_BUSY;如检测到对方摘机，则返回S_CONNECT;如进入此步骤已经过配置项Ringback_NoAnswerTime定义的时间长度，并且已检测到回铃音，则返回S_NOBODY；其它情况返回S_NORESULT。
        /// 需注意的是，在进行呼出结果检测之前必须调用函数StartSigCheck启动信号音采集过程，并且在进行呼出结果检测时，要循环调用FeedSigFunc函数维持信号音采集过程。
        /// </remarks>
        /// <seealso cref = "Sig_StartDial"/>
        [DllImport("NewSig.dll", CharSet = CharSet.Auto)]
        public static extern Int32 Sig_CheckDial(Int32 wChnlNo);
        
        /// <summary>
        /// 清空忙音检测的缓冲区以及内部计数。当检测对方挂机的忙音后，必须调用本函数。
        /// </summary>
        /// <param name = "wChnlNo">通道号</param>
        [DllImport("NewSig.dll", CharSet = CharSet.Auto)]
        public static extern void Sig_ResetCheck(Int32 wChnlNo);
        
        /// <summary>
        /// 根据用户自己的要求，用户可以调用这个函数来生成指定类型的信号音。
        /// </summary>
        /// <param name = "nSigType">
        /// 信号音类型，目前只可以生成4种类型的信号音
        /// SIG_DIALTONE      拨号音  
        /// SIG_BUSY1         忙音1
        /// SIG_BUSY2         忙音2
        /// SIG_RINGBACK      回铃音
        /// </param>
        /// <param name = "nFreq1">信号音频率，单位HZ</param>
        /// <param name = "nFreq2">信号音频率，单位HZ，如果是单频信号音，这个参数设为0</param>
        /// <param name = "dbAmp1">信号音幅度，单位db, 对应的是nFreq1</param>
        /// <param name = "dbAmp2">信号音幅度，单位db, 对应的是nFreq2, 如果是单频信号音，这个参数设为0</param>
        /// <param name = "nOnTime">信号音一个周期中，响的时间长度， 单位是毫秒，本参数对SIG_DIALTONE无效</param>
        /// <param name = "nOffTime">信号音一个周期中，停的时间长度， 单位是毫秒，本参数对SIG_DIALTONE无效</param>
        /// <param name = "iSampleRate">采样率， 一秒钟对声音的采样次数，有效值4000到16000， 系统生成的默认信号音的采样率是8000</param>
        /// <returns>
        ///  0      函数成功执行
        /// -1      nSigType不对，不是指定的类型
        /// -2      nFreq1和nFreq2同时小于或等于0
        /// -3      dbAmp1和dbAmp2同时小于或等于-40
        /// -4      nOnTime和nOffTime同时小于或等于0
        /// -5      iSampleRate小于4000或大于16000
        /// </returns>
        /// <remarks>
        /// 如果用户不需要自己调用，系统已经默认为用户生成了4种信号音，只需调用函数StartPlaySignal()就可以放出来，默认信号音如下：
        /// SIG_DIALTONE   拨号音， 450HZ
        /// SIG_BUSY1      忙音1，  450HZ， 响350毫秒、停350毫秒
        /// SIG_BUSY1      忙音1，  450HZ， 响350毫秒、停350毫秒
        /// SIG_BUSY2      忙音2，  450HZ， 响700毫秒、停700毫秒
        /// SIG_RINGBACK   回铃音， 450HZ， 响1秒、    停4秒
        /// 本函数如果多次被调用，对于同一种类型，信号音是最后一次调用生成的。对StartPlaySignal函数的调用和停止，与原来一样，没有改变。
        /// </remarks>
        [DllImport("NewSig.dll", CharSet = CharSet.Auto)]
        public static extern Int32 SetGenerateSigParam(Int32 nSigType, Int32 nFreq1, Int32 nFreq2, double dbAmp1, double dbAmp2, Int32 nOnTime, Int32 nOffTime, Int32 iSampleRate);
        
        #endregion

        #region 传真函数

        /// <summary>
        /// 初始化FAX卡，使之能正常工作。本函数将为每个FAX通道分配一个缓冲区，其大小为wBuffSize字节。
        /// </summary>
        /// <param name="buffSize">每路FAX通道在接收和发送FAX文件时使用的缓冲区大小</param>
        /// <returns>
        ///  0  				● 初始化成功
        /// -1 					● 检测传真资源卡时，发生错误
        /// -2 					● 某片传真资源卡内存检测错误
        /// -3 					● INI配置文件错误
        /// -7 					● 分配内存失败
        /// </returns>
        /// <remarks>
        /// 在返回-1、-2、-3时，会在屏幕上弹出窗口，进一步说明错误的情况。
        /// A、[TC08A-V.INI]		ERR_FAX_INI_01: FaxCardNo=%d  在INI文件中，FaxCardNo的声明超出范围（应为1～8）
        /// B、[TC08A-V.INI]		ERR_FAX_INI_02: MemAddr=%X    在INI文件中，MemAddr的声明超出范围（应为D800、E000或E800）
        /// C、[TC08A-V.INI]		ERR_FAX_INI_03: Error Mode %d: Fax%d=%X 在INI文件中，Fax%d的声明非法
        /// D、[TC08A-V.INI]		ERR_FAX_INI_04: FaxStream=%d 在INI文件中，FaxStream的声明超出范围（应为大于10）
        /// E、[TC08A-V.INI]		ERR_CHK_FAX_01: FAX%d = %X 在检测时，地址为%X的FAX卡没有找到
        /// F、[TC08A-V.INI]		ERR_CHK_FAX_01: %X, Map memory fail 在检测时，映射内存地址失败
        /// G、[TC08A-V.INI]		ERR_ LOAD_FAX_PROG: [%s] 在向卡上装载程序时，打开文件错误[%s]
        /// 注意事项：
        ///     本函数一定要在语音卡初始化函数EnableCard成功之后调用。与DOS环境下的本函数不同的是，返回值0表示成功，而在DOS下，返回1表示成功。
        /// </remarks>
        /// <seealso cref="DJFax_DisableCard"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_DriverReady(Int32 buffSize);

        /// <summary>
        /// 禁止FAX卡工作，释放初始化时为每个FAX通道分配的内存。程序结束时调用此函数。
        /// </summary>
        /// <seealso cref="DJFax_DriverReady"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void DJFax_DisableCard();

        /// <summary>
        ///  取总的FAX通道数。如果在一台机内有一片传真资源卡，可用通道数为4；两片传真资源卡，可用通道数为8，以此类推。
        /// </summary>
        /// <returns>可用的FAX通道数</returns>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 GetTotalFaxChnl();

        /// <summary>
        /// 传真资源卡自检连通。可以用两个FAX通道相互连通起来，然后互相对测。本函数将通道wChnl和通道wChnl+1连接起来，因此，在调用时，应保证wChnl 等于0、2、4……。
        /// </summary>
        /// <param name="wChnlNo">FAX通道号</param>
        /// <returns>
        /// 1					● 连通成功
        /// 0					● 连通失败
        /// </returns>
        /// <seealso cref="DJFax_SelfCheckBreakLink"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_SelfCheckSetLink(Int32 wChnlNo);

        /// <summary>
        /// 传真资源卡断开自检连通。
        /// </summary>
        /// <param name="wChnlNo"></param>
        /// <returns>
        /// 1					● 断开成功
        /// 0			        ● 断开失败
        /// </returns>
        /// <seealso cref="DJFax_SelfCheckSetLink"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_SelfCheckBreakLink(Int32 wChnlNo);

        /// <summary>
        /// 连通传真资源的FAX通道和模拟卡的通道。只有调用了连通函数，FAX通道上的声音才能够通过模拟语音卡的外部接口发出，反之亦然。
        /// </summary>
        /// <param name="wFaxChnlNo">FAX通道号</param>
        /// <param name="wVoiceChnlNo">模拟卡通道号</param>
        /// <returns>
        /// 1					● 连通成功
        /// 0					● 连通失败（FAX通道号或模拟通道号超出合法范围）
        /// </returns>
        /// <remarks>
        /// 注意事项：
        ///     在调用本函数之前，一定要保证wVoiceChnl已经停止放音。对模拟卡通道停止放音，可以通过调用函数StopPlayFile来实现。
        /// </remarks>
        /// <seealso cref="DJFax_ClearLink"/>
        /// <seealso cref="DJFax_GetVoiceChnlOfFaxChnl"/>
        /// <seealso cref="DJFax_GetFaxChnlOfVoiceChnl"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_SetLink(Int32 wFaxChnlNo, Int32 wVoiceChnlNo);

        /// <summary>
        /// 拆除传真资源的FAX通道和模拟卡通道的连通。
        /// </summary>
        /// <param name="wFaxChnlNo">FAX通道号</param>
        /// <param name="wVoiceChnlNo">模拟卡通道号</param>
        /// <returns>
        /// 1					● 拆除成功
        /// 0   				● 拆除失败
        /// </returns>
        /// <seealso cref="DJFax_SetLink"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_ClearLink(Int32 wFaxChnlNo, Int32 wVoiceChnlNo);

        /// <summary>
        /// 取该FAX通道连接的模拟通道号。当调用了DJFax_SetLink后，你可以用本函数来得到与FAX通道相连通的模拟通道号。当没有模拟通道与该FAX通道相连通时，调用本函数将返回-1。
        /// </summary>
        /// <param name="wFaxChnlNo">FAX通道号</param>
        /// <returns>
        /// -1					● 没有连接的通道
        /// XX					● 模拟卡通道号
        /// </returns>
        /// <seealso cref="DJFax_SetLink"/>
        /// <seealso cref="DJFax_ClearLink"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_GetVoiceChnlOfFaxChnl(Int32 wFaxChnlNo);

        /// <summary>
        /// 取该模拟卡通道连接的FAX通道号。当调用了DJFax_SetLink后，你可以用本函数来得到与模拟卡通道相连通的FAX通道号。当没有FAX通道与该模拟通道相连通时，调用本函数将返回-1。
        /// </summary>
        /// <param name="wVoiceChnlNo"> 模拟卡通道号</param>
        /// <returns>
        /// -1					● 没有连接的通道
        /// XX					● FAX通道号
        /// </returns>
        /// <seealso cref="DJFax_SetLink"/>
        /// <seealso cref=" DJFax_ClearLink"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_GetFaxChnlOfVoiceChnl(Int32 wVoiceChnlNo);

        /// <summary>
        /// 取一个空闲的FAX通道。当某个模拟卡通道或数字卡的中继通道需要FAX资源的时候，首先需要调用本函数来获取一个可用的FAX通道。
        /// </summary>
        /// <returns>
        /// -1					● 没有可用的FAX通道
        /// XX					● 可用的FAX通道
        /// </returns>
        /// <remarks>
        /// 当调用本函数时，系统将按循环方式查找一个可用的FAX通道。比如：0~3路FAX通道均为空闲，则第一次调用本函数将分配通道0，第二次调用本函数将分配通道1，依次类推。这样做的好处是：当有一路FAX通道损坏或挂起时，系统仍可以正常工作。
        /// 在系统的内部，对应每个FAX通道有两个标志：FaxBusyFlag和GF_ChnlFlag。系统初始化的时候，这两个标志设定为0，当两个标志均为0时，表示该通道空闲。当调用函数DJFax_SetLink后，FaxBusyFlag将变为1；当调用函数DJFax_ClearLink后，FaxBusyFlag将变为0。当调用函数DJFax_SendFaxFile或DJFax_RcvFaxFile后，GF_ChnlFlag将变为1，表示该FAX通道正在工作状态，当发送或接收FAX完毕（包括正确结束和错误结束），GF_ChnlFlag将变为0；当调用函数DJFax_StopFax后，系统将努力使该FAX通道复位，当该通道复位后，GF_ChnlFlag将变为0。
        /// 为了编程时的方便，在函数DJFax_ClearLink里包含了对函数DJFax_StopFax的调用。
        /// 注意事项：
        ///     由于在一个系统中，FAX通道资源并非是足够的，于是可能会出现FAX通道不够的情况。此时，调用本函数将返回-1，表示没有可用的通道。应用程序应该能对这种情况做出处理，一种办法是放音乐等待，直至有可用的FAX通道；另一种办法是放语音提示，告知用户系统忙，然后请用户挂机。
        /// </remarks>
        /// <seealso cref="DJFax_GetOneFreeFaxChnlOld"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_GetOneFreeFaxChnl();

        /// <summary>
        /// 取一个空闲的FAX通道。本函数的功能与函数DJFax_GetOneFreeFaxChnl相同，参考其相应的函数说明。本函数将按顺序方式查找一个可用的FAX通道。比如：0~3路FAX通道均为空闲，则第一次调用本函数将分配通道0，第二次调用本函数仍然分配通道0，依次类推。
        /// </summary>
        /// <returns>
        /// -1					● 没有可用的FAX通道
        /// XX					● 可用的FAX通道
        /// </returns>
        /// <remarks>
        /// 本函数是为了兼容以前的函数。
        /// </remarks>
        /// <seealso cref="DJFax_GetOneFreeFaxChnl"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_GetOneFreeFaxChnlOld();

        /// <summary>
        /// 设置本次发送FAX文件的分辨率。一般情况下，FAX可以有两种分辨率，标准分辨率在垂直方向为每毫米3.85扫描行，高分辨率为每毫米7.7扫描行；所有分辨率在水平方向为每毫米8个点，因此，A4纸定义为1728个点。
        /// </summary>
        /// <param name="wChnlNo"> FAX通道号</param>
        /// <param name="ResolutionFlag">分辨率，可能的值为：HIGH_RESOLUTION（1）高分辨率LOW_RESOLUTION（0）标准分辨率</param>
        /// <returns>
        /// 1					● 成功
        /// XX					● 失败（FAX通道号超出范围）
        /// </returns>
        /// <remarks>
        /// 如果在使用函数DJCvt_Open生成FAX文件时，已经指定了分辨率，那么，在发送FAX时就不必再用本函数来设定分辨率。本函数提供了动态调整发送分辨率的一种方法，它仅仅影响本次发送时的分辨率。
        /// 注意事项：
        ///     如果需要用本函数来设定发送时的分辨率，一定要在函数DJFax_ SendFaxFile之后调用。
        /// </remarks>
        /// <seealso cref="DJFax_SendFaxFile"/>
        /// <seealso cref="DJCvt_Open"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_SetResolution(Int32 wChnlNo, Int32 ResolutionFlag);

        /// <summary>
        /// 某个FAX通道开始发送FAX文件，文件格式为BFX格式。请参阅“BFX的文件格式”。
        /// </summary>
        /// <param name="wChnlNo">FAX通道号</param>
        /// <param name="FileName">发送的FAX文件名</param>
        /// <returns>
        /// -1 					● 读FAX文件失败
        /// -2 					● 内存分配失败
        /// -3    				● FAX文件的总页数错误
        /// -4 					● FAX文件的页长度或页起始位置错误
        /// -5					● 读取文件错误
        /// >0		            ● 发送成功，返回要发送的总页数
        /// </returns>
        /// <remarks>
        /// 调用完本函数后，需要不断调用函数DJFax_CheckTransmit，以维持发送的继续；如果需要中途停止发送，请使用函数DJFax_StopFax。
        /// 在调用本函数时，发送的分辨率由跟对端的协商结果（对端DIS）来定。
        /// </remarks>
        /// <seealso cref="DJFax_RcvFaxFile"/>
        /// <seealso cref="DJFax_CheckTransmit"/>
        /// <seealso cref="DJFax_StopFax"/>
        /// <seealso cref="DJCvt_Open"/>
        /// <seealso cref="DJFax_SendFaxFileEx"/>
        /// <seealso cref="DJFax_Send"/>
        /// <seealso cref="DJFax_GetCurPage"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_SendFaxFile(Int32 wChnlNo, byte[] FileName);

        /// <summary>
        /// 某个FAX通道开始发送FAX文件，文件格式为BFX格式。请参阅“BFX的文件格式”。
        /// </summary>
        /// <param name="wChnlNo">FAX通道号</param>
        /// <param name="FileName">发送的FAX文件名</param>
        /// <param name="StartPage">发送起始页（范围0～最大页数，并且不能大于结束页）</param>
        /// <param name="EndPage">发送结束页（大于等于StartPage，可设为－1或者大于等于文件最大页，此时实际发送结束页为文件最后一页）</param>
        /// <returns>
        /// -1 					● 读FAX文件失败
        /// -2 					● 内存分配失败
        /// -3    				● FAX文件的总页数错误
        /// -4 					● FAX文件的页长度或页起始位置错误
        /// -5					● 读取文件错误
        /// >0		            ● 发送成功，返回要发送的总页数
        /// </returns>
        /// <remarks>
        /// 调用完本函数后，需要不断调用函数DJFax_CheckTransmit，以维持发送的继续；如果需要中途停止发送，请使用函数DJFax_StopFax。
        /// 在调用本函数时，发送的分辨率由跟对端的协商结果（对端DIS）来定。
        /// </remarks>
        /// <seealso cref="DJFax_RcvFaxFile"/>
        /// <seealso cref="DJFax_CheckTransmit"/>
        /// <seealso cref="DJFax_StopFax"/>
        /// <seealso cref="DJCvt_Open"/>
        /// <seealso cref="DJFax_SendFaxFile"/>
        /// <seealso cref="DJFax_Send"/>
        /// <seealso cref="DJFax_GetCurPage"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_SendFaxFileEx(Int32 wChnlNo, byte[] FileName, Int32 StartPage, Int32 EndPage);

        //[DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        //public static extern Int32 DJFax_Send(WORD wChnl,DJ_FAX_CTRL* pFaxCtrl);

        /// <summary>
        /// 某个FAX通道开始接收FAX文件，文件格式为BFX格式。请参阅“BFX的文件格式”。
        /// </summary>
        /// <param name="wChnlNo">FAX通道号</param>
        /// <param name="FileName">接收的FAX文件名</param>
        /// <returns>
        ///  1    			    ● 成功，开始接收文件
        /// -1					● 打开FAX文件失败
        /// -2					● 内存分配失败
        /// </returns>
        /// <remarks>
        /// 调用完本函数后，需要不断调用函数<seealso cref="DJFax_CheckTransmit"/>，以维持接收的继续；如果需要中途停止发送，请使用函数DJFax_StopFax。
        /// 本次接收的分辨率，放在BFX文件头中，请参考函数<seealso cref="DJCvt_Open"/>的说明。
        /// </remarks>
        /// <seealso cref="DJFax_SendFaxFile"/>
        /// <seealso cref="DJFax_SendFaxFileEx"/>
        /// <seealso cref="DJFax_Send"/>
        /// <seealso cref="DJFax_CheckTransmit"/>
        /// <seealso cref="DJFax_StopFax"/>
        /// <seealso cref="DJCvt_Open"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_RcvFaxFile(Int32 wChnlNo, byte[] FileName);

        /// <summary>
        /// 检查FAX文件的接收或发送是否结束。当用函数DJFax_SendFaxFile或函数DJFax_RcvFaxFile开始一个发送或接收后，需要不断的调用本函数，维持发送或接收的继续；同时，检测发送的结果，并作出相应的处理。
        /// </summary>
        /// <param name="wChnlNo">FAX通道号</param>
        /// <returns>
        ///  2  			    ● 某一页结束
        ///  1					● 所有的发送或接收正确结束
        ///  0					● 正在发送或接收         
        /// -2					● 读文件失败或命令交互错误
        /// </returns>
        /// <seealso cref="DJFax_SendFaxFile"/>
        /// <seealso cref="DJFax_SendFaxFileEx"/>
        /// <seealso cref="DJFax_Send"/>
        /// <seealso cref="DJFax_RcvFaxFile"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_CheckTransmit(Int32 wChnlNo);

        /// <summary>
        /// 停止接收或发送FAX文件。
        /// </summary>
        /// <param name="wChnlNo"> FAX通道号</param>
        /// <seealso cref="DJFax_SendFaxFile"/>
        /// <seealso cref="DJFax_SendFaxFileEx"/>
        /// <seealso cref="DJFax_Send"/>
        /// <seealso cref="DJFax_RcvFaxFile"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void DJFax_StopFax(Int32 wChnlNo);

        /// <summary>
        /// 设置本地的ID号。在收发FAX的过程中，可以设定本机的ID号，该ID号一般会在传真的顶行显示出来。
        /// </summary>
        /// <param name="wChnlNo">FAX通道号</param>
        /// <param name="Str">ID字符串</param>
        /// <returns>
        /// 1   				● 设置成功
        /// 0  					● 设置失败（FAX通道号超出范围）
        /// </returns>
        /// <remarks>
        /// 注意事项：
        ///     参数s是以’\0’结束的ASCII字符串，长度一定不能超过20个字符。
        /// </remarks>
        /// <seealso cref="DJFax_GetLocalID"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_SetLocalID ( Int32 wChnlNo, byte[] Str);

        /// <summary>
        /// 获取函数DJFax_SetLocalID所设定的本地ID号。
        /// </summary>
        /// <param name="wChnlNo">FAX通道号</param>
        /// <param name="Str">ID字符串</param>
        /// <returns>
        /// 1  					● 获取ID成功
        /// 0   				● 获取ID失败
        /// </returns>
        /// <remarks>
        /// 注意事项：
        ///     系统中的本地ID的长度为20个字符。参数s的空间必须由应用程序分配，并保证大于20个字符。
        /// </remarks>
        /// <seealso cref="DJFax_SetLocalID"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_GetLocalID ( Int32 wChnlNo, byte[] Str);

        /// <summary>
        /// 取当前页已经接收到的字节数。当调用函数DJFax_RcvFaxFile后，可以用本函数来查看当前页收到的字节数。
        /// </summary>
        /// <param name="wChnlNo">FAX通道号</param>
        /// <returns>
        /// 当前页已接收到的字节数
        /// </returns>
        /// <seealso cref="DJFax_RcvFaxFile"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern long DJFax_GetRcvBytes(Int32 wChnlNo);

        /// <summary>
        /// 取当前页已经发送的字节数。当调用函数DJFax_SendFaxFile后，可以用本函数来查看当前页已经发送的字节数。
        /// </summary>
        /// <param name="wChnlNo">FAX通道号</param>
        /// <returns>
        /// 当前页已发送的字节数
        /// </returns>
        /// <seealso cref="DJFax_SendFaxFile"/>
        /// <seealso cref="DJFax_SendFaxFileEx"/>
        /// <seealso cref="DJFax_Send"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern long DJFax_GetSendBytes(Int32 wChnlNo);

        /// <summary>
        /// 设置拨出号码。如果需要由FAX通道来启动一次呼出，那么就需要用本函数来设定呼出的电话号码。
        /// </summary>
        /// <param name="wChnlNo">FAX通道号</param>
        /// <param name="DialNo">拨出的号码</param>
        /// <returns>
        /// 1				    ● 设置成功
        /// 0					● 设置失败
        /// </returns>
        /// <seealso cref="DJFax_SendFaxFile"/>
        /// <seealso cref="DJFax_SendFaxFileEx"/>
        /// <seealso cref="DJFax_Send"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_SetDialNo(Int32 wChnlNo, byte[] DialNo);

        /// <summary>
        /// 取出错时的错误号。当函数DJFax_CheckTransmit的返回值为-2时，表示发送或接收FAX错误。
        /// </summary>
        /// <param name="wChnlNo">FAX通道号</param>
        /// <returns>
        /// 可以通过本函数得到具体的错误代码：
        /// 255     TIME_OUT
        ///   0     DJ_SUCCESS
        ///   1     ERROR，AT指令异常返回，一般不会返回此错误码
        ///   2     NO_CARRIER，按照T30协议等待接收对方信号的时候没有接收到信号超时退出，可结合出错子状态追溯到超时出错的位置
        ///   3     NOT USED
        ///   4     NO_DIALTONE，NOT USED
        ///   5     BUSY，NOT USED
        ///   6     RETRY_FAIL，达到T30协议规定的重试次数仍未能成功完成FAX中的握手过程，可结合出错子状态追溯到超时出错的位置
        ///   7     HDLC_UNEXP，接收到违反T30协议规定的命令或响应包
        ///   8     TRAIN_FAIL，NOT USED
        ///   9     HDLC_NOT_T4_REV，对方不支持T4（即G3类）传真
        ///         REMOTE_DCN，收到对方发来的DCN拆线命令
        ///         PRE_STOP，上层应用程序主动提前结束FAX过程
        ///         NON_FAX，对方不是FAX终端
        ///  16     NOT_V29_RECEIVER 13，对方不支持V29传真接收
        /// </returns>
        /// <seealso cref="DJFax_GetErrPhase"/>
        /// <seealso cref="DJFax_GetErrSubst"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_GetErrCode(Int32 wChnlNo);

        /// <summary>
        /// 取出错时的FAX_PHASE值。当函数DJFax_CheckTransmit的返回值为-2时，表示发送或接收FAX错误。此时，可以通过本函数得到具体的FAX_PHASE值。传真分为5个阶段，请参阅“通信过程”。
        /// </summary>
        /// <param name="wChnlNo">FAX通道号</param>
        /// <returns>出错时的FAX_PHASE值</returns>
        /// <seealso cref="DJFax_GetErrCode"/>
        /// <seealso cref="DJFax_GetErrSubst"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_GetErrPhase(Int32 wChnlNo);

        /// <summary>
        /// 取出错时的FAX_SUBST值，该值为传真资源卡系统内部使用，用户可以不必理会。
        /// </summary>
        /// <param name="wChnlNo">FAX通道号</param>
        /// <returns>
        /// 出错时的FAX_SUBST值
        /// </returns>
        /// <seealso cref="DJFax_GetErrCode"/>
        /// <seealso cref="DJFax_GetErrPhase"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_GetErrSubst(Int32 wChnlNo);

        /// <summary>
        /// 当前发送文件页码
        /// </summary>
        /// <param name="WChnlNo">FAX通道号</param>
        /// <returns>当前页</returns>
        /// <seealso cref="DJFax_RcvFaxFile"/>
        /// <seealso cref="DJFax_CheckTransmit"/>
        /// <seealso cref="DJFax_SendFaxFile"/>
        /// <seealso cref="DJFax_SendFaxFileEx"/>
        /// <seealso cref="DJFax_Send"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_GetCurPage(Int32 WChnlNo);

        /// <summary>
        /// 动态转换初始化。在本函数中将读入转换时必需的数据，初始化内部变量数组。该函数需要ASC16、ASC24、HZK16、HZK24、HUFFMAN.COD这几个文件在当前的目录下，否则会返回-1。
        /// </summary>
        /// <returns>
        ///  1  				● 初始化成功
        /// -1					● 打开文件失败
        /// -2					● 申请内存失败
        /// </returns>
        /// <remarks>
        /// 当返回-1或-2时，会在屏幕上弹出窗口，进一步说明错误的情况：
        /// A、"Open File Fail : [%s]"	打开文件错误，可能的文件名为ASC16、ASC24、HZK16、HZK24、HUFFMAN.COD； 如果你在语音卡安装目录下无法找到这几个文件，请在DOS安装盘上查找。
        /// B、"Alloc memory fail 1" 在为HUFFMAN.COD分配内存时，发生错误。
        /// C、"Alloc memory fail 2" 在为ASC16分配内存时，发生错误。
        /// D、"Alloc memory fail 3" 在为HZK16分配内存时，发生错误。
        /// E、"Alloc memory fail 4" 在为ASC24分配内存时，发生错误。
        /// F、"Alloc memory fail 5" 在为HZK24分配内存时，发生错误。
        /// G、"Alloc memory fail"   在分配其他内存时，发生错误。
        /// 在系统内，有一个大小为32的结构数组，可以同时维护最多32个FAX文件的转换，我们也可以将转换的功能看做是系统可用的资源，一共有32个FAX转换资源，以下称为转换通道。由于在一个系统内，最多有32个FAX通道，因此，转换通道的数目是足够的。
        /// 本节中的函数，一般都带有参数wChnl，我们称为转换通道，该参数对应于这个结构数组，wChnl的范围为0~31。如果wChnl越界，将会产生系统保护错。如果有两个模拟通道或中继通道使用同一个转换通道来进行转换，也会产生不确定的结果。
        /// 分配和使用转换通道的方法有以下几种：
        /// 当你的系统比较小时（即：保证模拟卡或数字卡的中继通道数小于等于32），可以用中继通道号直接带入转换函数。
        /// 当你可以确认每次分配的FAX通道是唯一的，也可以用FAX通道号带入转换函数，此时，需要先调用函数DJFax_GetOneFreeFaxChnl来找到一个空闲的FAX通道，然后使用函数DJFax_SetLink将该FAX通道占用，这时就可以使用FAX通道号来进行转换了；要注意的是，必须首先用函数DJFax_SetLink将该FAX通道占用。
        /// 最后，你也可以自己写一对函数来分配和释放这32个转换通道，这将是最安全的。
        /// 注意事项：
        ///     本节中的转换函数（DJCvt_）不涉及到传真资源硬件，因此，可以脱离传真资源的硬件独立运行。转换函数需要的TC08A32.DLL或TCE1_32.DLL来支持。
        /// 如果你需要使用本节中的转换函数，必须在程序初始化时，正确的调用了本函数。否则，将会出现系统保护错。
        /// </remarks>
        /// <seealso cref="DJCvt_DisableConvert"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJCvt_InitConvert();

        /// <summary>
        /// 释放动态转换所分配的资源，与DJCvt_InitConvert配对使用。系统退出时调用。
        /// </summary>
        /// <seealso cref="DJCvt_InitConvert"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void DJCvt_DisableConvert(); 

        /// <summary>
        ///  准备开始一个FAX转换。成功调用本函数后，会将本转换通道标记为占用。
        /// cbFaxFileName是准备写入FAX数据的FAX文件名，如果该文件不存在，将新建一个；如果该文件已存在，将重新改写。建立文件不成功，将返回-1。
        /// </summary>
        /// <param name="wChnlNo">转换通道号</param>
        /// <param name="cbFaxFileName">转换成的FAX文件名</param>
        /// <param name="cbResolution">转换分辨率HIGH_RESOLUTION（1）高分辨率STANDARD_RESOLUTION（2）标准分辨率</param>
        /// <param name="wPageLineNo">每页扫描行</param>
        /// <returns>
        ///  1    			    ● 成功
        /// -1					● 打开文件失败或本转换通道已经被占用
        /// </returns>
        /// <remarks>
        /// CbResolution用来设置转换的分辨率。一般情况下，FAX可以有两种分辨率，标准分辨率在垂直方向为每毫米3.85扫描行，高分辨率为每毫米7.7扫描行。分辨率将记录在生成的FAX文件头中，偏移1577的位置，如果这一个字节为A，当 A & 0x40 == 0x40，表示为高分辨率，否则表示为低分辨率。在使用函数DJFax_SendFaxFile发送FAX文件时，会根据这个字节的值来确定发送时应该使用的分辨率。对于使用函数DJFax_RcvFaxFile接收到的FAX文件，系统会自动在文件头填写相应的值，这样，如果再发送该FAX文件，将会按照正确的分辨率发送。分辨率参数还将影响转换函数中的DJCvt_TextLine。
        /// 最后一个参数wPageLineNo表示每页扫描行的总数，你可以自行设定。在标准分辨率下，一页A4纸的扫描行大约为1100行；在高分辨率下，一页A4纸的扫描行大约为2200行，当本函数调用成功后，你就可以根据需要调用函数DJCvt_TextLine、DJCvt_DotLine、DJCvt_BmpFile等来增加FAX的内容，当达到设定的每页扫描行总数时，会自动切换为下一页。
        /// 注意事项：
        ///     一定要检查本函数的返回值，当不成功时，要有相应的处理。否则，应用程序继续调用函数DJFax_SendFaxFile，会有可能将上次转换的临时FAX文件发送出去，从而产生严重的后果。
        /// </remarks>
        /// <seealso cref="DJCvt_Close"/>
        /// <seealso cref="DJCvt_TextLine"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJCvt_Open(Int32 wChnlNo, byte[] cbFaxFileName, byte cbResolution, Int32 wPageLineNo);

        /// <summary>
        ///  关闭正在转换的FAX文件。在转换结束后，一定要调用本函数，FAX文件才能正确的关闭。
        /// <param name="wChnlNo">转换通道号</param>
        /// <returns>
        ///  1  				● 成功
        /// -1					● 失败（关闭一个没有打开的转换通道）
        /// </returns>
        /// <seealso cref="DJCvt_Open"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJCvt_Close(Int32 wChnlNo);

        /// <summary>
        /// 向指定的转换通道增加一个扫描行。实质上，所有的转换函数最终都是调用本函数来实现的。
        /// </summary>
        /// <param name="wChnl">转换通道号</param>
        /// <param name="cbDotStr">点阵行的内容</param>
        /// <param name="wDotSize">点阵行的大小</param>
        /// <param name="wDotFlag">标志，可能的值为：DOT_0_IS_WHITE（0）0表示白点 DOT_1_IS_WHITE（1）1表示白点</param>
        /// <returns>
        ///  1   				● 成功
        /// -1					● 失败（转换文件没有打开）
        /// </returns>
        /// <remarks>
        /// 在字符串cbDotStr中，每个字节表示8个点；wDotSize表示点阵的比特数；标志wDotFlag，一般设为0，表示当某比特为0时，代表此位置无点（白点）。
        /// 比如，一个扫描行为16个白点，33个黑点，剩余的(1728-16-33)为白点，可以如下表示：
        /// BYTE DotStr[ ] =  “ 0x00, 0x00, 0xff, 0xff, 0xff, 0xff, 0x80”;
        /// DJCvt_DotLine ( wChnl, (char *)DotStr, 16+33, DOT_0_IS_WHITE ); 
        /// 也可以如下表示：
        /// BYTE DotStr[ ] =  “ 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x7f”;
        /// DJCvt_DotLine ( wChnl, (char *)DotStr, 16+33, DOT_1_IS_WHITE ); 
        /// 在转换时，系统会自动将剩余的点填充为白点。
        /// 注意事项：
        ///     每个字节可以表示8个点，高位比特代表左边的点。
        /// </remarks>
        /// <seealso cref="DJCvt_Open"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJCvt_DotLine(Int32 wChnl, byte[] cbDotStr, Int32 wDotSize, Int32 wDotFlag);

        /// <summary>
        /// 返回当前页还剩余的扫描行数目。当转换过程中，可以不断的调用本函数来检查是否快到一页的结束了。
        /// 一般情况下，你可以不停的向FAX文件增加新的内容，当一页转换完毕，系统会自动切换到下一页。但是，如果需要在每一页增加页眉和页脚，就需要使用本函数，以便当一页快结束时，做适当的处理。请参考FAXDEMO.CPP的源程序。
        /// </summary>
        /// <param name="wChnlNo">转换通道号</param>
        /// <returns>当前页还剩余的扫描行数目。</returns>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJCvt_LeftLine(Int32 wChnlNo);

        /// <summary>
        /// 向指定的转换通道增加一行文本。系统根据点阵文件转换出FAX数据。为了美观，系统会自动将TAB（’\t’）变为4个空格。
        /// 参数FontSize 表示在转换时使用的字体点阵大小，如果其值为16，使用点阵ASC16（8*16 ASCII字符）和HZK16（16*16汉字），在最左边加入5个空格；如果其值为24，使用点阵ASC24（12*24 ASCII字符）和HZK24（24*24汉字），在最左边加入3个空格。
        /// 标志DoubleBitFlag表示是否将1个点变成2个点，当DoubleBitFlag=0时，一个汉字在水平方向上将占用16个或24个点，一个ASCII字符将占用8个点或12个点；当DoubleBitFlag=1时，一个汉字在水平方向上将占用32个点或48个点，一个ASCII字符将占用16个点或24个点。由于一个FAX扫描行为1728个点，为了合乎比例，一般采用DoubleBitFlag=1。
        /// 标志DoubleLineFlag表示是否将1个点阵行变成2个点阵行。当DoubleLineFlag=0时，一行文本将占用17个扫描行（其中在底部有1个空扫描行）或25个扫描行；当DoubleLineFlag=1时，一行文本将占用34个扫描行（其中在底步有2个空扫描行）或68个扫描行。
        /// </summary>
        /// <param name="wChnl">转换通道号</param>
        /// <param name="cbTextStr">转换的文本串，可以有中文和ASCII字符</param>
        /// <param name="DoubleBitFlag">双点标志（0或1）</param>
        /// <param name="DoubleLineFlag">双行标志（0或1）</param>
        /// <param name="FontSize">使用的字体大小（可以为16或24）</param>
        /// <returns>
        ///  1   				● 成功
        /// XX 					● 失败
        /// </returns>
        /// <remarks>
        /// 注意事项：
        ///     在早期的版本中，只有函数DJCvt_TextLine，建议以后使用本函数来转换文本文件。
        /// 参数cbTextStr以’\0’结束，其长度不得超过180个字符。
        /// </remarks>
        /// <seealso cref="DJCvt_TextLine"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJCvt_TextLineA(Int32 wChnl, byte[] cbTextStr, Int32 DoubleBitFlag, Int32 DoubleLineFlag, Int32 FontSize);

        /// <summary>
        ///  向指定的转换通道增加一行文本。
        /// </summary>
        /// <param name="wChnl">转换通道号</param>
        /// <param name="cbTextStr">转换的文本串，可以有中文和ASCII字符</param>
        /// <returns>
        ///  1   				● 成功
        /// XX 					● 失败
        /// </returns>
        /// <remarks>
        /// 本函数相当于：
        /// if ( cbResolution == HIGH_RESOLUTION )			// 高分辨率
        /// 	return DJCvt_TextLineA ( wChnl, cbTextStr, 0, 0,16);	// 单点，单行，16点阵字体
        ///	else
        ///		return DJCvt_TextLineA ( wChnl, cbTextStr, 1, 0,16);	// 双点，单行，16点阵字体
        ///	请参阅函数的<seealso cref="DJCvt_ TextLineA"/>说明。
        /// </remarks>
        /// <seealso cref="DJCvt_TextLineA"/>
        /// <seealso cref="DJCvt_Open"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJCvt_TextLine(Int32 wChnl, byte[] cbTextStr);

        /// <summary>
        /// 转换黑白的BMP文件到FAX文件。为了美观，系统会自动在最左边加入80个空白点。
        /// 标志DoubleBitFlag表示是否将1个点变成2个点。由于一个FAX扫描行为1728个点，为了合乎比例，在标准分辨率时，一般采用DoubleBitFlag=1。比如，在屏幕上的一幅BMP图象文件，大小为800*600，转换完毕后变成1600*600，在标准分辨率下，刚好合乎比例。同样道理，在使用高分辨率时，一般采用DoubleBitFlag=0。
        /// </summary>
        /// <param name="wChnl">转换通道号</param>
        /// <param name="cbBmpFileName"> 要转换的黑白BMP文件名</param>
        /// <param name="DoubleBitFlag">双点标志（0或1）</param>
        /// <returns>
        ///  1  				● 成功
        /// -1					● 打开BMP文件失败或本转换通道没有打开
        /// -2					● 失败，该BMP文件不是黑白的BMP文件
        /// </returns>
        /// <remarks>
        /// 注意事项：
        ///     一定要保证BMP文件是单色（黑白）的BMP格式的图象文件。
        /// </remarks>
        /// <seealso cref="DJCvt_BmpFile"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJCvt_BmpFileA(Int32 wChnl, byte[] cbBmpFileName, Int32 DoubleBitFlag);

        /// <summary>
        /// 转换黑白的BMP文件到FAX文件。本函数相当于：
        ///   DJCvt_BmpFileA(wChnl, cbBmpFileName, 1);
        /// 请参阅函数的<seealso cref="DJCvt_BmpFileA"/>说明。
        /// </summary>
        /// <param name="wChnl">转换通道号</param>
        /// <param name="cbBmpFileName">要转换的黑白BMP文件名</param>
        /// <returns>
        ///  1    				● 成功
        /// -1 					● 打开BMP文件失败或本转换通道没有打开
        /// -2					● 失败，该BMP文件不是黑白的BMP文件
        /// </returns>
        /// <seealso cref="DJCvt_BmpFileA"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJCvt_BmpFile(Int32 wChnl, byte[] cbBmpFileName);

        /// <summary>
        /// 将Bfx文件转换成Tiff文件
        /// </summary>
        /// <param name="Bfxfilename">bfx文件名指针</param>
        /// <param name="Tifffilename">TIF文件名指针</param>
        /// <returns>
        ///  1                  ● 成功完成；
        /// -1                  ● 打开bfxfilename 失败
        /// -2                  ● 打开tifffilename 失败
        /// -3                  ● 转换失败
        /// </returns>
        /// <seealso cref="DJCvt_Tiff2Bfx"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJCvt_Bfx2Tiff(byte[] Bfxfilename, byte[] Tifffilename);

        /// <summary>
        /// 将TIF文件转换成bfx文件
        /// </summary>
        /// <param name="Tifffilename">TIF文件名指针</param>
        /// <param name="Bfxfilename">bfx文件名指针</param>
        /// <returns>
        ///  1                   ●   转换成功；
        /// -1                   ●   打开TIF文件Tifffilename失败；
        /// -2                   ●  打开bfx文件Bfxfilename失败；
        /// -3                   ●  转换失败。
        /// </returns>
        /// <seealso cref="DJCvt_Bfx2Tiff"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJCvt_Tiff2Bfx(byte[] Tifffilename, byte[] Bfxfilename);

        /// <summary>
        /// 把一个BFX格式的文件转换成一个或多个BMP格式的位图文件
        /// </summary>
        /// <param name="Bfxfilename"> 要转换BFX格式的文件名</param>
        /// <param name="Bmpfilename">转换后生成的BMP格式的文件名</param>
        /// <param name="PageMode"> 0—转换后生成一个BMP格式的文件;1—转换后每页生成一个BMP格式的文件.文件名Bmpfilename%</param>
        /// <param name="RotateMode">0—转换后位图直接存储数据.1—转换后位图旋转180度存储数据</param>
        /// <returns>
        ///  1                   ● 文件转换成功
        /// -1                   ● 参数错误,页模式必须为1或0
        /// -2                   ● 参数错误,旋转模式必须为1或0 
        /// -3                   ● 内存分配失败
        /// -4                   ● 打开bfx文件错误
        /// -5                   ● 不是bfx格式的文件
        /// -6                   ●  bfx页错误
        /// -7                   ●  打开bmp文件失败
        /// </returns>
        /// <remarks>
        /// 位图数据记录了位图的每一个像素值，记录顺序是在扫描行内是从左到右,扫描行之间是从下到上.如果不进行旋转存储数据,那么转换后生成的图像与原来的图像正好是相反的。
        /// </remarks>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32  DJCvt_Bfx2Bmp(byte[] Bfxfilename, byte[] Bmpfilename, Int32 PageMode, Int32 RotateMode);

        /// <summary>
        /// 开始对某一通道进行传真信号音的检测
        /// </summary>
        /// <param name="wChnlNo">通道号</param>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void DJFax_StartCheckFaxTone(Int32 wChnlNo); 

        /// <summary>
        /// 查看当前某一通道上是否检测到了传真信号音
        /// </summary>
        /// <param name="wChnlNo">通道号</param>
        /// <returns>
        /// 0 没有检测到传真信号音
        /// 1 检测到了传真信号音
        /// </returns>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_FaxToneCheckResult(Int32 wChnlNo);

        /// <summary>
        /// 获得指定通道的拨号步骤。
        /// </summary>
        /// <param name="wChNo">通道号</param>
        /// <returns>NewSig.dll中，将拨号过程分成了8个步骤，根据拨号所处的不同阶段，返回值可能是0到7中的一个。</returns>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 Sig_GetDialStep(Int32 wChNo);

        #endregion

    }
}
