using System;
using System.Runtime.InteropServices;

namespace Uniframework.Switch.Endpoints.DB160X
{
    #region ������ϵͳ��Ϣ�ṹ

    /// <summary cref = "TC_INI_TYPE">
    /// ������ϵͳ��Ϣ�ṹ
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
    /// ��������չϵͳ��Ϣ�ṹ
    /// </summary>
    public struct TC_INI_TYPE_MORE
    {
        public short wMemAddr;             // ��ռ�õĹ����ڴ��ַ
        public short wCardNum;             // ������
        /// <summary>
        /// Ŀǰ���ܵĿ�����
        /// #define	CARD_TYPE_D161A		16
        /// #define	CARD_TYPE_D080A		8
        /// </summary>
        public byte[] cbCardType;          // �������ͣ�Ҳ��ʾ�ÿ��ϵ�ͨ������
        /// <summary>
        /// ��ʶ�ÿ�������
        /// #define CHTYPE_USER         0
        /// #define CHTYPE_TRUNK        1
        /// #define CHTYPE_EMPTY        2
        /// </summary>
        public byte[] cbCardNeiWai;        // �ÿ����м̿������ߣ������û���������
        public Int32 wChnlNum;               // ���ϵ�ͨ������
        public byte[] cbChType;            // ��ͨ��������
        public byte[] cbChnlCardNo;        // ��ͨ�����ڿ���
        public byte[] cbChnlInternal;      // ��ͨ���ڿ��ڵ�ͨ����
        public byte[] cbConnectChnl;       // ����
        public byte[] cbConnectStream;     // ����
        public byte[] cbDtmfModeVal;       // ����
        public byte[] cbIsSupportCallerID; // ��ͨ���Ƿ�֧��Caller-ID��1��ʾ֧�֡�D161A���ϵ�ͨ����֧��Caller-ID��
    }
    
    #endregion

    public static class D160X
    {
        public static object SyncObj = new object(); // ͬ������

        #region ��������

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

        #region ��ʼ������

        /// <summary>
        /// ��ʼ���豸��������
        /// </summary>
        /// <returns>
        ///  0       �ɹ�
        /// -1   	 ���豸�����������
        /// -2   	 �ڶ�ȡTC08A-V.INI�ļ�ʱ����������
        /// -3  	 INI�ļ���������ʵ�ʵ�Ӳ����һ��ʱ����������
        /// </returns>
        /// <seealso cref = "FreeDRV"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern long LoadDRV();

        /// <summary cref = "FreeDRV">
        /// �ر���������
        /// </summary>
        /// <seealso cref = "LoadDRV"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void FreeDRV();

        /// <summary>
        /// ��ʼ���绰����Ӳ����Ϊÿ��ͨ������������������wUsedCh�����ú���CheckValidCh����á�
        /// </summary>
        /// <param name = "wUsedCh">��������ͨ����</param>
        /// <param name = "wFileBufLen">������Ϊÿͨ������������ڴ��С</param>
        /// <returns>
        ///  0       �ɹ�
        /// -1 ������LoadDRVû�гɹ�����ɱ���������ʧ�ܡ�
        /// -2       ���仺����ʧ�� 
        /// </returns>
        /// <remarks>
        /// �ڵ��ñ�����ʱ����Ϊÿ·����wFileBufLen ��С������������������wUsedCh * wFileBufLen�� �����벻�����򷵻�-2��buffer����Ϊ1024����������
        /// ���磺EnableCard ( 8, 1024*16 ); ��������128K���ڴ档
        /// </remarks>
        /// <seealso cref = "CheckValidCh"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern long EnableCard(Int32 wusedCh, Int32 wFileBufLen);

        /// <summary>
        /// �رյ绰����Ӳ�����ͷŻ��������������(���������Ͳ������˳�)ʱ����ô˺���
        /// </summary>
        /// <seealso cref="EnableCard"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void DisableCard();

        /// <summary cref = "GetSysInfo">
        /// ���ϵͳ���õ��й���Ϣ
        /// </summary>
        /// <param name = "TmpIni"> ָ��ṹTC_INI_TYPE</param>
        /// <remarks>�ڵ���ǰ����Ӧ�ó��������ṹTC_INI_TYPE�Ŀռ䣬Ȼ��ָ�봫������������������ɺ��ڽṹTC_INI_TYPE�н�����ϵͳ��Ϣ��</remarks>
        /// <seealso cref = "TC_INI_TYPE"/>
        /// <seealso cref = "GetSysInfoMore"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void GetSysInfo(ref TC_INI_TYPE TmpIni);
        
        /// <summary>
        /// ���ϵͳ���õĸ�����Ϣ�����ڿ�����һ̨�����ڻ��ʹ��D161A PCI��D080A PCI������ˣ�����Ҫ�õ������ϵͳ��Ϣʱ������ʹ�ñ�������
        /// </summary>
        /// <param name = "TmpMore">ָ��ṹTC_INI_TYPE_MORE</param>
        /// <remarks>�ڵ���ǰ����Ӧ�ó��������ṹTC_INI_TYPE_MORE�Ŀռ䣬Ȼ��ָ�봫������������������ɺ��ڽṹTC_INI_TYPE_MORE�н�����ϵͳ��Ϣ��</remarks>
        /// <seealso cref = "TC_INI_TYPE_MORE"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void GetSysInfoMore(ref TC_INI_TYPE_MORE TmpMore);
        
        /// <summary>
        /// ����ڵ�ǰ�����ڿ��õ�ͨ��������
        /// </summary>
        /// <returns>
        /// �ܵĿ���ͨ����
        /// </returns>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern short CheckValidCh();
        
        /// <summary>
        /// ���ͨ�����͡�
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <returns>
        /// CHTYPE_USER ��0��		   	����
        /// CHTYPE_TRUNK��1��		    ����
        /// CHTYPE_EMPTY��2��  		    ����
        /// </returns>
        /// <remarks>
        /// ���ĳ��ͨ�������͡�����D161A PCI���ϵĵ绰�ӿ�ģ������������ã���ˣ�����D161A PCI��,����16·ͨ������ΪTRUNK(����)��USER�����ߣ���¼��ģ���������ϡ�
        /// ���⣬��TC-08A V�Ϳ���ͬ���ǣ�D161A PCI�����Լ�⵽ĳ·û�в�ģ�飬���CHTYPE-EMPTY�����գ�������Ч���ˡ�
        /// </remarks>
        /// <seealso cref = "CheckChTypeNew"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern short CheckChType(Int32 wChnlNo);
        
        /// <summary>
        /// ���ĳ��ͨ�������͡�����D161A PCI���ϵĵ绰�ӿ�ģ������������ã���ˣ�����D161A PCI��,����16·ͨ������ΪTRUNK(����)��USER�����ߣ���¼��ģ���������ϡ�
        /// ���⣬��TC-08A V�Ϳ���ͬ���ǣ�D161A PCI�����Լ�⵽ĳ·û�в�ģ�飬���CHTYPE-EMPTY�����գ�������Ч���ˡ�
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <returns>
        /// CHTYPE_USER  ��0��		   	����
        /// CHTYPE_TRUNK ��1��		    ����
        /// CHTYPE_EMPTY ��2��  		����
        /// CHTYPE_RECORD��3��          ¼��
        /// </returns>
        /// <seealso cref = "CheckChType"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern short CheckChTypeNew(Int32 wChnlNo);
        
        /// <summary>
        /// �жϸÿ��Ƿ�֧��Caller-ID���ܡ�D161A PCI��������1��֧�֣���
        /// </summary>
        /// <returns>
        ///  1      ֧��
        ///  0      ��֧��
        /// </returns>
        /// <seealso cref = "GetSysInfoMore"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool IsSupportCallerID();
        
        /// <summary cref = "SetPackRate">
        /// ����ѹ�����ʡ����ñ�������, ¼�������Ը�ѹ�����ʽ��С�
        /// </summary>
        /// <param name = "wPackRate">ѹ�����ʣ� ��ֵΪ
        /// #define	PACK_64KBPS     0  (��ѹ��) ÿ��64K bits �� 8K bytes
        /// #define	PACK_32KBPS	    1  ÿ��32K bits �� 4K bytes
        /// </param>
        /// <remarks>����D161A PCI����Ŀǰֻ֧��32KBPS��ѹ����ʽ���������������ñ�������Ч�������������ڳ�ʼ������EnableCard�ɹ�֮����ò���Ч��</remarks>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void SetPackRate(Int32 wPackRate);
        
        /// <summary cref = "PUSH_PLAY">
        /// ά���ļ�¼�����ĳ������У����ڴ������Ĵ�ѭ���е��á�
        /// </summary>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void PUSH_PLAY();
        
        /// <summary cref = "SetBusyPara">
        /// �趨Ҫ���Ĺһ�æ���Ĳ�����
        /// </summary>
        /// <param name = "BusyLen">æ����ʱ�䳤�ȣ���λΪ���롣</param>
        /// <remarks>���磺�����й涨��0.7��æ���źţ�дΪSetBusyPara(700)��</remarks>
        /// <seealso cref="SetDialPara"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void SetBusyPara(Int32 BusyLen);
        
        /// <summary>
        /// �趨�����Ժ�Ҫ�����ź����Ĳ�����
        /// </summary>
        /// <param name = "RingBack">��������������ʱ�䳤�ȣ���λΪ���롣</param>
        /// <param name = "RingBack0"> ������������֮������ʱ�䳤�ȣ���λΪ���롣</param>
        /// <param name = "BusyLen">�Է�ռ��ʱ���ص�æ���źŵ�ʱ�䳤�ȡ�</param>
        /// <param name = "RingTimes">һ��������ٴ���Ϊ�����˽�����</param>
        /// <remarks>���磺�����й涨���ź�Ļ�����Ϊ��1�룬ֹͣ4�룬æ��Ϊ0.35�룬дΪ  SetDialPara(1000,4000,350,7)��</remarks>
        /// <seealso cref="SetBusyPara"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void SetDialPara(Int32 RingBack, Int32 RingBack0, Int32 BusyLen, Int32 RingTimes);
        
        /// <summary>
        /// ��ȡD161A PCI�������кš�������˾��Ʒ��D161A PCI��������һ��Ψһ�ı�ţ�ʮ������λ�������û������������ܡ�
        /// </summary>
        /// <param name = "wCardNo">����</param>
        /// <returns>
        /// �����������кš�
        /// </returns>
        /// <remarks>�����кű����ں���LoadDRV֮��EnableCard֮ǰ���뽫���кŷ������ı����С�</remarks>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern long NewReadPass(Int32 wCardNo);
        
        /// <summary>
        /// �趨ĳͨ���Ĺ���������
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <param name = "cbWorkMode">ѡ��Ҫ�趨�Ĺ���ģʽ</param>
        /// <param name = "cbModeVal">����ֵ</param>
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

        #region ���弰ժ�һ�����

        /// <summary>
        /// ��飨���ߣ��Ƿ��������źŻ����ߣ��Ƿ��������
        /// ����������1ʱ���������ߣ���ʱ���������OffHook���ɽ�ͨ��·�绰�����������衰����������ڽ�ͨ״̬��
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool RingDetect(Int32 wChnlNo);
        
        /// <summary>
        /// ����������������ߣ��˺����������á�
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <remarks>��Ҫ������ʹ�á�</remarks>
        /// <seealso cref = "HangUp"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void OffHook(Int32 wChnlNo);

        /// <summary>
        /// ���߹һ����������ߣ��˺����������á�
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <remarks>��Ҫ������ʹ�á�</remarks>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void HangUp(Int32 wChnlNo);

        #endregion

        #region ��������

        /// <summary>
        /// ָ��ͨ����ʼ��ͨ�ڴ�������������ĳ��ȣ�dwPlayLength������ϵͳ�������ĳ��ȣ��ں���EnableCard�ж��壩ʱ����Ҫ����PUSH_PLAY��ά��¼���ĳ�����
        /// ֹͣ���ַ�ʽ�ķ������ú���StopPlay������Ƿ������ϣ��ú���CheckPlayEnd��
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <param name = "PlayBuf">������������ַ</param>
        /// <param name = "dwStartPos">�ڻ������е�ƫ��</param>
        /// <param name = "dwPlayLen">�����ĳ���</param>
        /// <seealso cref = "StopPlay"/>
        /// <seealso cref="CheckPlayEnd"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void StartPlay(Int32 wChnlNo, byte[] PlayBuf, Int32 dwStartPos, Int32 dwPlayLen);
        
        /// <summary>
        /// ָ��ͨ��ֹͣ�ڴ����������������ֹͣ�ڴ���ͨ�������ڴ������������ڴ�ѭ��������
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <seealso cref = "StartPlay"/>
        /// <seealso cref = "CheckPlayEnd"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void StopPlay(Int32 wChnlNo);
        
        /// <summary>
        /// ���ָ��ͨ�������Ƿ����������������������ͨ�ڴ�����������ڴ������ѭ���ڴ�������ļ�������
        /// </summary>
        /// <param name = wChnlNo"">ͨ����</param>
        /// <seealso cref = "StartPlay"/>
        /// <seealso cref = "StartPlayIndex"/>
        /// <seealso cref = "StopPlay">
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool CheckPlayEnd(Int32 wChnlNo);

        /// <summary>
        /// ��ʼ�������ڴ滰����
        /// </summary>
        /// <seealso cref = "StartPlayIndex"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void ResetIndex();

        /// <summary>
        ///  ���������ڴ�����Ǽ��
        /// </summary>
        /// <param name = "VocBuf">ָ��Ҫ�Ǽǵ�����������ָ�롣</param>
        /// <param name = "dwVocLen">�������ȡ�</param>
        /// <returns>
        ///  0      �Ǽ�ʧ��,˵�������Ǽ�������
        ///  1      �Ǽǳɹ�
        /// </returns>
        /// <seealso cref = "ResetIndex"/>
        /// <seealso cref = "StartPlayIndex"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 SetIndex(byte[] VocBuf, Int32 dwVocLen);

        /// <summary>
        /// ��ʼ�ļ�������ֹͣ�÷�ʽ�ķ�����һ��Ҫ��StopPlayFile���������Ƿ��������CheckPlayEnd������
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <param name = "FileName">�����ļ�</param>
        /// <param name = "StartPos">��������ʼλ��</param>
        /// <returns>
        ///  true   �ɹ�����ʼ��������
        ///  false  ʧ�ܣ��������ļ������ڻ�����ԭ��
        /// </returns>
        /// <remarks>
        /// �ļ������ڱ�����������ѭ���ڴ������Ȼ�󣬲��ϵĸ��»�������PUSH_PLAY�����ĵ��ã��ܹ���֤�Է����������ĸ��£��Ӷ��ﵽ�������������ϵĽ��С�
        /// �����ñ����������ļ�����ʱ����StartPos��ָ����������ʼλ�ã����ʹ��Wave��ʽ��WaveFormat=1��2��3������ʵ�ʷ�������ʼλ���ǣ�StartPos + 58��
        /// ע��
        /// ������������ļ�ͷ�ĺϷ��ԣ������Ǽ򵥵�������ʼ��58���ֽڣ���ˣ����ڷ�Wave��ʽ��ԭ���������ļ���Ҳ��������������
        /// �����ļ�������������StartIndexPlayFile�������䱾���ϵ��õ���StartPlayFile����ˣ��䲥��ÿ�������ļ�ʱ��Ҳ�����Ǽ򵥵�����ÿ���ļ���ʼ��58���ֽڡ�
        /// ���������ķ����������磺StartPlay��StartPlayIndex��StartPlaySignal��SYS_StartLoopPlay�����ڲ����ļ��������书����Ȼͬ��ǰһ������������58���ֽڡ�
        /// </remarks>
        /// <seealso cref = "CheckPlayEnd"/>
        /// <seealso cref = "StopPlayFile"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool StartPlayFile(Int32 wChnlNo, byte[] FileName, Int32 StartPos);

        /// <summary>
        /// ��������ָ��ͨ��ֹͣ�ļ������������ú���StartPlayFile��ʼ�ķ����������ñ�������ֹͣ���������ܹر������ļ���
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <remarks>���ʹ��Wave��ʽ��WaveFormat=1��2��3�����ڱ������У��������ʵ��¼�������ݳ�����ɶ�Wave�ļ�ͷ����д�����������ļ����ܹ�����������ȷ�Ĳ��š����û�е��ñ����������п������������޷��������š�</remarks>
        /// <seealso cref = "StartPlayFile"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void StopPlayFile(Int32 wChnlNo);

        /// <summary>
        /// ��ʼ�����ļ�������ÿ��ʼһ���µĶ��ļ�����ǰ���ô˺�����
        /// </summary>
        /// <remarks>
        /// Ϊ�˷����û��Ŀ��������ļ������Ļ������������ļ����������Ĺ��ܣ��ù��ܿ���һ�β������100���������ļ����ڽ�������ƴ�ӵ�ʱ��Ƚ����á�
        /// ��ϵͳ���ڲ�����Ӧÿ��ͨ����һ�������һ������������������Դ��100���ļ��������ñ�����RsetIndexPlayFile���Ὣ��������0�����ú���AddIndexPlayFile���������н��ļ�����¼������ͬʱ��������1�����ú���StartIndexPlayFile��ʼ���ŵ�һ�������ļ����˺���Ҫ��ͣ�ĵ��ú���CheckIndexPlayFile������������У������鵽��ǰ���ڷ������ļ��Ѿ����꣬�ͻ�������һ��������ֱ�����������е������ļ��������Ҫֹͣ��������������ʹ�ú���StopIndexPlayFile��
        /// </remarks>
        /// <seealso cref = "AddIndexPlayFile"/>
        /// <seealso cref = "StartIndexPlayFile"/>
        /// <seealso cref = "CheckIndexPlayFile"/>
        /// <seealso cref = "StopIndexPlayFile"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void RsetIndexPlayFile(Int32 Line);
        
        /// <summary>
        /// ���Ӷ��ļ������ķ����ļ�
        /// </summary>
        /// <returns>
        ///  true      �ɹ�
        ///  flase     ʧ��
        /// </returns>
        /// <seealso cref = "RsetIndexPlayFile"/>
        /// <seealso cref = "StartIndexPlayFile"/>
        /// <seealso cref = "CheckIndexPlayFile"/>
        /// <seealso cref = "StopIndexPlayFile"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool AddIndexPlayFile(Int32 Line, byte[] FileName);
        
        /// <summary>
        /// ��ʼһ���ڴ�����������
        /// SetIndex��StartPlayIndex����������������Ϊ�˷����û�����Ϸ����������ֵ���ϡ��ڳ�ʼ����ʱ����Ӧ�ó�����������ļ��������ڴ桢�����������ڴ��У���ÿһ�����������ڴ����Ҫ�ú���SetIndex�Ǽǣ����ѭ����
        /// ��ʼ����ɺ�����ͨ�������Թ�����Щ�����������������������磬����Ҫ����������һǧ����ʮ����ʱ�����������°취��
        /// Int32 NumStr[6] = { 1, 12, 0, 5,10, 3 };
        /// StartPlayIndex ( ChannelNo, NumStr, 6 );
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <param name = "pIndexTable">Ҫ�������������</param>
        /// <param name = "wIndexLen">Ҫ��������������</param>
        /// <seealso cref = "StopPlay"/>
        /// <seealso cref = "CheckPlayEnd"/>
        /// <seealso cref = "SetIndex"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool StartPlayIndex(Int32 wChnlNo, Int32[] pIndexTable, Int32 wIndexLen);
        
        /// <summary>
        /// �����ļ������Ƿ��������ά�����ļ������������ԡ������ж��ļ�����ʱ��������ñ��������Ա�֤���ļ������������ԡ�
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <returns>
        ///  ture   ����
        ///  false  δ����
        /// </returns>
        /// <seealso cref = "RsetIndexPlayFile"/>
        /// <seealso cref = "AddIndexPlayFile"/>
        /// <seealso cref = "StartIndexPlayFile"/>
        /// <seealso cref = "StopIndexPlayFile"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool CheckIndexPlayFile(Int32 wChnlNo);
        
        /// <summary>
        /// ֹͣ���ļ��������ú���ָֹͣ��ͨ���Ķ��ļ�����������ʹ��StartIndexPlayFile������ʼ�Ķ��ļ�����������ʱһ��Ҫ���ñ�������
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <seealso cref = "RsetIndexPlayFile"/>
        /// <seealso cref = "AddIndexPlayFile"/>
        /// <seealso cref = "StartIndexPlayFile"/>
        /// <seealso cref = "CheckIndexPlayFile"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void StopIndexPlayFile(Int32 wChnlNo);
        
        /// <summary>
        /// ��ʼһ�����ļ������������øú����ɹ��󣬱���ѭ������CheckIndexPlayFile�������������Ƿ��������ά�����ļ��������������С�
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <returns>
        ///  ture   �ɹ�
        ///  false  ʧ��
        /// </returns>
        /// <seealso cref = "RsetIndexPlayFile"/>
        /// <seealso cref = "AddIndexPlayFile"/>
        /// <seealso cref = "CheckIndexPlayFile"/>
        /// <seealso cref = "StopIndexPlayFile"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool StartIndexPlayFile(Int32 wChnlNo);

        #endregion

        #region ¼������

        /// <summary>
        /// ��ʼ�ļ�¼���������ñ����������ļ�¼��ʱ����dwRecordLen��ָ��¼������󳤶ȡ����ʹ��Wave��ʽ��WaveFormat=1��2��3������ʵ���ļ�����󳤶��ǣ�dwRecordLen + 58�����ɹ����ñ������󣬻��Զ����ļ�FileName�Ŀ�ͷԤ��д��58�ֽڡ�ֹͣ�÷�ʽ��¼����һ��Ҫ��StopRecordFile�����¼���Ƿ��������CheckRecordEnd������
        /// �ļ�¼���ڱ�����������ѭ���ڴ�¼����Ȼ�󣬲��ϵĸ��»�������PUSH_PLAY�����ĵ��ã��ܹ���֤¼�������ߣ��Ӷ��ﵽ¼���Ľ��С�
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <param name = "FileName">�ļ���</param>
        /// <param name = "dwRecordLen">�¼������</param>
        /// <returns>
        ///  ture   �ɹ�
        ///  flase  ʧ�ܣ��Ƿ����ļ�����·��
        /// </returns>
        /// <seealso cref = "StopRecordFile"/>
        /// <seealso cref = "StartRecordFile_Ex"/>
        /// <seealso cref = "CheckRecordEnd"/>
        /// <seealso cref = "StartRecordFileNew"/>
        /// <seealso cref = "StartRecordFileNew_Ex"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool StartRecordFile(Int32 wChnlNo, byte[] FileName, long dwRecordLen);

        /// <summary>
        /// ��ʼ�ļ�¼���������ñ����������ļ�¼��ʱ����dwRecordLen��ָ��¼������󳤶ȡ����ʹ��Wave��ʽ��WaveFormat=1��2��3������ʵ���ļ�����󳤶��ǣ�dwRecordLen + 58�����ɹ����ñ������󣬻��Զ����ļ�FileName�Ŀ�ͷԤ��д��58�ֽڡ�ֹͣ�÷�ʽ��¼����һ��Ҫ��StopRecordFile�����¼���Ƿ��������CheckRecordEnd������
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <param name = "FileName">�ļ���</param>
        /// <param name = "dwRecordLen">�ļ�¼�������</param>
        /// <param name = "IsShareOpen">�Ƿ��Թ���ʽ��д�ļ�</param>
        /// <returns>
        ///  ture   �ɹ�
        ///  flase  ʧ��
        /// </returns>
        /// <remarks>
        /// �ļ�¼���ڱ�����������ѭ���ڴ�¼����Ȼ�󣬲��ϵĸ��»�������PUSH_PLAY�����ĵ��ã��ܹ���֤¼�������ߣ��Ӷ��ﵽ¼���Ľ��С�
        /// ע��
        /// ������IsShareOpenΪFALSE��ʱ�򣬵�ͬ��StartRecordFile��ΪTRUEʱ��¼���ļ������ԡ������/����д����ʽ�򿪡�
        /// </remarks>
        /// <seealso cref = "StopRecordFile"/>
        /// <seealso cref = "StartRecordFile"/>
        /// <seealso cref = "CheckRecordEnd"/>
        /// <seealso cref = "StartRecordFileNew"/>
        /// <seealso cref = "StartRecordFileNew_Ex"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool StartRecordFile_Ex(Int32 wChnlNo, byte[] FileName, long dwRecordLen, bool IsShareOpen);

        /// <summary>
        /// �������Ƕ��ļ�¼������StartRecordFile�Ĳ��䡣��ʵ�ϣ�������������ȫ���溯��StartRecordFile��
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <param name = "FileName">�ļ���</param>
        /// <param name = "dwRecordLen">�ļ�¼�������</param>
        /// <param name = "dwRecordStartPos">¼���Ŀ�ʼλ��</param>
        /// <returns>
        ///  ture   �ɹ�
        ///  false  ʧ��
        /// </returns>
        /// <remarks>
        /// dwRecordStartPos��ָ��һ���ļ���¼���Ŀ�ʼλ�ã����磺�������Ҫ��һ���Ѵ����ļ��ĵ�5���λ�ÿ�ʼ¼������ָ��dwRecordStartPos = 8000*5�����ʹ��Wave��ʽ��WaveFormat=1��2��3�������������Զ���dwRecordStartPos ����58���乤����ʽ���£�
        /// ��dwRecordStartPos=0ʱ�����ú���StartRecordFile�������������ļ���¼����
        /// ��FileName������ʱ�����ú���StartRecordFile�������������ļ���¼����
        /// ��FileName�Ѿ����ڣ���dwRecordStartPos�����ļ��ĳ���ʱ�����ļ���β����ʼ¼������ˣ������Ҫ��һ���ļ���β������¼����������dwRecordStartPos=0xFFFFFFFFL����dwRecordStartPosС���ļ��ĳ���ʱ����dwRecordStartPos��λ�ÿ�ʼ¼����
        /// ¼���ĳ����ɱ���dwRecordLen��ȷ����
        /// </remarks>
        /// <seealso cref = "StopRecordFile"/>
        /// <seealso cref = "StartRecordFile"/>
        /// <seealso cref = "StartRecordFile_Ex"/>
        /// <seealso cref = "CheckRecordEnd"/>
        /// <seealso cref = "StartRecordFileNew_Ex"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool StartRecordFileNew(Int32 wChnlNo, byte[] FileName, long dwRecordLen, Int32 dwRecordStartPos);

        /// <summary>
        /// �������Ƕ��ļ�¼������StartRecordFile_Ex�Ĳ��䡣��ʵ�ϣ�������������ȫ���溯��StartRecordFile_Ex��dwRecordStartPos��ָ��һ���ļ���¼���Ŀ�ʼλ�ã����磺�������Ҫ��һ���Ѵ����ļ��ĵ�5���λ�ÿ�ʼ¼������ָ��dwRecordStartPos = 8000*5�����ʹ��Wave��ʽ��WaveFormat=1��2��3�������������Զ���dwRecordStartPos ����58��
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <param name = "FileName">�ļ���</param>
        /// <param name = "dwRecordLen">�ļ�¼�������</param>
        /// <param name = "dwRecordStartPos">¼���Ŀ�ʼλ��</param>
        /// <param name = "IsShareOpen">�Ƿ��Թ���ʽ��д�ļ�</param>
        /// <returns>
        ///  ture   �ɹ�
        ///  false  ʧ��
        /// </returns>
        /// <remarks>
        /// �乤����ʽ���£�
        /// ��dwRecordStartPos=0ʱ�����ú���StartRecordFile_Ex�������������ļ���¼����
        /// ��FileName������ʱ�����ú���StartRecordFile_Ex�������������ļ���¼����
        /// ��FileName�Ѿ����ڣ���dwRecordStartPos�����ļ��ĳ���ʱ�����ļ���β����ʼ¼������ˣ������Ҫ��һ���ļ���β������¼����������dwRecordStartPos=0xFFFFFFFFL����dwRecordStartPosС���ļ��ĳ���ʱ����dwRecordStartPos��λ�ÿ�ʼ¼����
        /// ¼���ĳ����ɱ���dwRecordLen��ȷ����
        /// ע��
        /// ������IsShareOpenΪFALSE��ʱ�򣬵�ͬ��StartRecordFileNew��ΪTRUEʱ��¼���ļ������ԡ������/����д����ʽ�򿪡�
        /// </remarks>
        /// <seealso cref = "StopRecordFile"/>
        /// <seealso cref = "StartRecordFile"/>
        /// <seealso cref = "StartRecordFile_Ex"/>
        /// <seealso cref = "CheckRecordEnd"/>
        /// <seealso cref = "StartRecordFileNew"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool StartRecordFileNew_Ex(Int32 wChnlNo, byte[] FileName, Int32 dwRecordLen, Int32 dwRecordStartPos, bool IsShareOpen);

        /// <summary>
        ///  ���ָ��ͨ��¼���Ƿ����(����������)��
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <param name = ""></param>
        /// <returns>
        ///  0      δ����
        ///  1      �ѽ���
        /// </returns>
        /// <seealso cref = "StartRecord"/>
        /// <seealso cref = "StartRecordFile"/>
        /// <seealso cref = "StartRecordFileNew"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 CheckRecordEnd(Int32 wChnlNo);

        /// <summary>
        /// �ú���ָֹͣ��ͨ�����ļ�¼��������StartRecordFile����������¼��, һ��Ҫ�ñ�������ֹͣ���������ܱ�֤�ر������ļ���
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <seealso cref = "StartRecordFile"/>
        /// <seealso cref = "StartRecordFileNew"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void StopRecordFile(Int32 wChnlNo);

        #endregion

        #region ���롢�κš��ź�����⺯��

        /// <summary>
        /// ���ϵͳ��DTMF������������ڻ���������DTMF������ֵ�����ᶪʧ��
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <seealso cref = "GetDtmfCode"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void InitDtmfBuf(Int32 wChnlNo);
        
        /// <summary>
        /// ȡ��ͨ���յ���DTMF���룬����ڻ���������DTMF���������ñ����������������һ��DTMF������ͬʱ���ð����ӻ���������ȥ������ڻ�������û���յ��κε�DTMF����������������-1��
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <param name = ""></param>
        /// <returns>
        /// -1    	��������û��DTMF����
        /// ����    ��DTMF����
        /// 1��9  	1��9��
        /// 10      0��
        /// 11    	*��
        /// 12    	#��
        /// 13    	A��
        /// 14    	B��
        /// 15    	C��
        /// 0     	D��
        /// </returns>
        /// <remarks>
        /// ��ϵͳ�У���Ӧÿһ��ͨ������һ��64�ֽڵ�DTMF�������������ú���InitDtmfBufʱ��ϵͳ���Զ���ո�DTMF���������Ա��û�����ʹ�ã����ײ����������⵽��DTMF����ʱ�����Զ����ð����ļ�ֵ���뻺�����ڡ��û����ñ������Ϳ��Եõ�DTMF����ֵ��
        /// </remarks>
        /// <seealso cref = "InitDtmfBuf"/>
        /// <seealso cref = "DtmfHit"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 GetDtmfCode(Int32 wChnlNo);
        
        /// <summary>
        /// �鿴ָ��ͨ���Ƿ���DTMF���������յ�һ����Ч��DTMF�����󣬱���������TRUE�������������Ὣ�������ڲ�����������ȥ������Ҫ��ȥ�ð�����Ҫ���ú���GetDtmfCode��
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <returns>
        ///  ture   ����������Dtmf����
        ///  false  ��������û��Dtmf����
        /// </returns>
        /// <seealso cref = "GetDtmfCode"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool DtmfHit(Int32 wChnlNo);

        /// <summary>
        /// ����DTMF(����)����,����ʾ�ڲ���ʱ����ʱ0.5�롣�磺��0��3323577����ʾ�Ȳ�һ��0��Ȼ����ʱ0.5�룬�ٲ�3323577�����͵�ÿ��DTMF����Ϊ125���룬���ҲΪ125���롣
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <param name = "DialNum">���ŵ��ַ�������ЧֵΪ��0��-��9��,��*��,��#������,�� ��ABCD��</param>
        /// <remarks>
        /// �������������������ڴ�����������ʵ�ֵġ���Ҫ��;ֹͣ���ţ������ú���StopPlay����Ⲧ���Ƿ���ɣ�ʹ�ú���CheckSendEnd�����Ҫ��������DTMF�����ʣ���ʹ�ú���NewSendDtmfBuf��
        /// ע�����
        /// һ�ο��Է��͵�DTMF�ַ�������󳤶�Ϊ64��������DTMF�ڱ�����Ҳ�Ƿ�������ˣ���Ҫ���ϵĵ��ú���PUSH_PLAY��
        /// </remarks>
        /// <seealso cref = "CheckSendEnd"/>
        /// <seealso cref = "StopPlay"/>
        /// <seealso cref = "NewSendDtmfBuf"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void SendDtmfBuf(Int32 wChnlNo, byte[] DialNum);

        /// <summary>
        /// ���ĳ·����DTMF�Ƿ���ϡ�
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <param name = ""></param>
        /// <returns>
        ///  0      δ�������
        ///  1      �ѷ������
        /// </returns>
        /// <remarks></remarks>
        /// <seealso cref = "SendDtmfBuf"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool CheckSendEnd(Int32 wChnlNo);

        /// <summary>
        /// ���÷���DTMF������ʡ����Ҫ�ú���NewSendDtmfBuf������DTMF�룬�ڳ�ʼ��ʱ����ʹ�ñ����������÷��͵����ʡ�һ�㱾��������EnableCard֮�󼴿ɡ�
        /// </summary>
        /// <param name = "ToneLen">DTMF���ʱ�䳤�ȣ���λ�����룩������ܳ���125����</param>
        /// <param name = "SilenceLen">�����ʱ�䳤�ȣ���λ�����룩������ܳ���125����</param>
        /// <returns>
        ///  0      �ɹ�
        ///  1      ʧ��
        /// </returns>
        /// <seealso cref = "SendDtmfBuf_Ex"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 SetSendPara(Int32 ToneLen, Int32 SilenceLen);

        /// <summary>
        /// ��ʼ��ָ�������ʷ���DTMF����,����ʾ�ڲ���ʱ����ʱ0.5�롣
        /// �������������������ڴ������ʵ�ֵġ���Ҫ��;ֹͣ���ţ������ú���StopPlay����Ⲧ���Ƿ���ɣ�ʹ�ú���NewCheckSendEnd��
        /// </summary>
        /// <param name = "ChanelNo">ͨ����</param>
        /// <param name = "DialNum">���ŵ��ַ�������ЧֵΪ��0��-��9��,��*��,��#������,�� ��ABCD��</param>
        /// <remarks>
        /// ע�����
        /// ע���һ������������ΪInt32��32����������ͨ����WORD��16λ����   
        /// �������������������ڴ������ʵ�ֵġ����ԣ�һ�ο��Է��͵�DTMF�ַ�������󳤶���EnableCard��ÿ��ͨ�������������������С�йأ�Ҳ��SetSendPara���õ����ʿ����йء�
        /// ���磬�ڳ�ʼ��ʱ�趨EnableCard(8,1024*40)��SetSendPara(50,50)����ô��ÿ��DTMF��ʱ100���룬ÿ·����������40K���൱��5�룬���ԣ�DialNumһ����������50����
        /// </remarks>
        /// <seealso cref = "SetSendPara"/>
        /// <seealso cref = "NewCheckSendEnd"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void NewSendDtmfBuf(Int32 ChannelNo, byte[] DialNum);

        /// <summary>
        ///  ������NewSendDtmfBuf������ʼ�ķ���DTMF����������鷢���Ƿ���ϡ�
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <remarks>ע���һ������������ΪInt32��32����������ͨ����WORD��16λ����</remarks>
        /// <seealso cref = "NewSendDtmfBuf"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 NewCheckSendEnd(Int32 wChnlNo);

        /// <summary>
        /// ĳ·��ʼ�µ��ź�����⡣һ����ժ�����߹һ��󣬵��ñ���������ʼ�µ��ź�����⡣
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <seealso cref = "StopSigCheck"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void StartSigCheck(Int32 wChnlNo);

        /// <summary>
        /// ֹͣĳ·���ź�����⡣
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <remarks></remarks>
        /// <seealso cref = "StartSigCheck"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void StopSigCheck(Int32 wChnlNo);

        /// <summary>
        /// �ڵ�����StartSigCheck����֮��, �Ϳ��Ե��ñ�����������ź����ļ������
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <param name = "wMode">�����ź�������</param>
        /// <remarks>
        /// Mode��ָ����������µ��ź���������ܵ�ֵΪRECORD_CHECK,PLAY_CHECK, SEND_CHECK��
        /// RECORD_CHECK ��  PLAY_CHECK���Է��һ��źż�⡣��¼�����������պ�ʱ, ���Է����ź����, ���鿴�Է��Ƿ�һ���
        /// RECORD_CHECK��PLAY_CHECK�Ĺ�����ͬ��
        /// �÷���
        /// <example>ReadCheckResult (ChannelNo, RECORD_CHECK);</example>
        /// <example>ReadCheckResult (ChannelNo, PLAY_CHECK);</example>
        /// ����ֵ��
        /// R_BUSY    �� �Է��һ�����⵽æ��
        /// R_OTHER   �� δ��⵽�һ�����
        /// �ڵ��ñ�����֮ǰ������Ӧ�趨��Ҫ����æ��������ʹ�ú���SetBusyPara(700)����Ҫ�����æ��������ġ��Է��һ���⡱��
        /// SEND_CHECK : �����Ժ���ź�����⡣��������Ƚϸ���, Ҳ�����׳����⡣�����û�Ӧ��ϸ�����������ĺ��塣
        /// �÷���
        /// <example>ReadCheckResult(ChannelNo, SEND_CHECK);</example>
        /// ����ֵ��
        /// S_NORESULT�� ��δ�ó����
        /// S_BUSY    �� ��⵽�Է�ռ�ߵ�æ��
        /// S_CONNECT �� �Է�ժ�������Խ���ͨ��
        /// S_NOBODY  �� �������ɴΣ����˽����绰
        /// S_NOSIGNAL�� ����ź�û���κ��ź���
        /// �ڵ��ñ�����֮ǰ������Ӧ�趨��Ҫ����æ��������ʹ�ú���SetDialPara(1000,4000,350,7);
        /// ע��˺�����֧��450Hz�ź�����⣬�����ź���������á������� D161A���ź���������ָ���ĺ�����
        /// </remarks>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 ReadCheckResult(Int32 wChnlNo, Int32 wMode);

        /// <summary>
        /// ��ȡ��ǰ�������æ���ĸ������ú�������Ӧ��ͨ���Ǹղŵ���ReadCheckResult����ʱ��ͨ����
        /// </summary>
        /// <returns>
        /// ��ǰ�������æ���ĸ�����
        /// </returns>
        /// <seealso cref = "ReadCheckResult"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 ReadBusyCount();

        /// <summary>
        /// ���ĳһͨ���ļ��ԡ����Լ����������жϲ��ź�Է��Ƿ�ժ����
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <returns>
        ///  ture   �м���
        ///  false  �޼���
        /// </returns>
        /// <remarks>
        /// ����ĳͨ�����Ⲧ��ʱ������ժ������ʱһ�����ñ�������¼���Ե�ֵ��Ȼ�󲦺ţ�������Ϻ󣬵���⵽���Ըı�ʱ��˵���û�ժ����
        /// ע��һ����л���·�������з����ԵĹ��ܣ���Ҫ����ž����롣
        /// </remarks>
        /// <seealso cref = "StartSigCheck"/>
        /// <seealso cref = "StopSigCheck"/>
        /// <seealso cref = " ReadCheckResult"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool CheckPolarity(Int32 wChnlNo);

        /// <summary>
        /// �����·�ľ��������
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <param name = "wCheckNum">�����ź�����������ЧֵΪ1��511</param>
        /// <returns>
        /// -1      �ź����������еĸ���������wCheckNum��
        /// 0��wCheckNum wCheckNum���ź��������У�1�ĸ�����
        /// </returns>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 CheckSilence(Int32 wChnlNo, Int32 wCheckNum);

        #endregion

        #region �������弰��⺯��

        /// <summary>
        /// ��lpFileName��ȡ���ڲ�����������lpFileName ��Ӧ����һ���ź�����ϵͳ����ʹ�øö�������������������æ�������������ź�������ϵͳ�ڲ���һ��ȱʡ���ź������û�Ҳ��������¼��ϲ�����ź�����Ȼ���ñ��������滻���ȱʡ���ź�����
        /// </summary>
        /// <param name = "lpFileName">�ź����ļ���</param>
        /// <returns>
        ///  ture   �ɹ�
        ///  false  ʧ��
        /// </returns>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool ReadGenerateSigBuf(byte[] lpFileName);

        /// <summary>
        /// ��ĳһ·����ͨ�����磬ͬʱֹͣ��������
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <seealso cref = "FeedRing"/>
        /// <seealso cref = "FeedRealRing"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void FeedPower(Int32 wChnlNo);

        /// <summary>
        /// ��ĳһ·����ͨ�������������������ñ������󣬱�ͨ�������ӵĵ绰�����᲻ͣ�����壬ֱ�����ú���FeedPower�Ż�ֹͣ��
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <remarks>һ��Ҫ��ȷ�Ľ�������Դ���绰���������塣</remarks>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void FeedRing(Int32 wChnlNo);

        /// <summary>
        /// ά�ֶ������弰�ź����ĺ��������ڳ����ѭ���е��á�
        /// </summary>
        /// <seealso cref = "PUSH_PLAY"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void FeedSigFunc();

        /// <summary>
        /// ��ĳһ·����ͨ����������������������ʱ�䳤��Ϊ����0.75�룬ֹͣ3�롣��Ҫֹͣ������������ʹ�ú���FeedPower���ڱ�ͨ���������������£����ժ������ʹ�ú���OffHookDetect��������ʹ�ú���RingDetect��
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <remarks>һ��Ҫ���ϵĵ��ú���FeedSigFunc�����ܱ�֤����������������</remarks>
        /// <seealso cref = "OffHookDetect"/>
        /// <seealso cref = "FeedSigFunc"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void FeedRealRing(Int32 wChnlNo);

        /// <summary>
        /// ���ĳһ·����ͨ����ժ��״̬��������FeedRealRing������ʼһ������������������ñ����������ժ��״̬��
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <returns>
        ///  ture   ��ժ��
        ///  false  δժ��
        /// </returns>
        /// <seealso cref = "FeedRealRing"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern bool OffHookDetect(Int32 wChnlNo);

        /// <summary>
        /// �����ź����Ĳ��š�������ʵ����ʹ���ڴ�ѭ��������ʵ�ֵġ�
        /// ���У�
        /// ��������ʱ�䳤��Ϊ��0.75�룬ֹͣ3�룻
        /// æ��һ��ʱ�䳤��Ϊ��0.35�룬ֹͣ0.35�룻
        /// æ������ʱ�䳤��Ϊ��0.7�룬ֹͣ0.7�룻
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <param name = "SigType">�ź������ͣ�����������ֵ��
        /// SIG_STOP       ֹͣ�����ź���
        /// SIG_DIALTONE   ������
        /// SIG_BUSY1      æ��һ
        /// SIG_BUSY2      æ����
        /// SIG_RINGBACK   ������
        /// </param>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void StartPlaySignal(Int32 wChnlNo, Int32 SigType );

        /// <summary>
        /// ĳһͨ����ʼ�һ���⣻��ĳͨ��ժ���󣬿��Ե��ñ��������ú���ֻ������ͨ����Ч��
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <seealso cref = "HangUpDetect"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void StartHangUpDetect(Int32 wChnlNo);

        /// <summary>
        /// ���ĳһͨ���Ĺһ�״̬���ú�����Ҫ�ڵ��� StartHangUpDetect֮��ʹ�á��������Ҫ����Ĳ�ɣ���ʹ�ñ�������
        /// ���⣬�еĵ绰����ժ��ʱ�����ж��������ʹ�ú���RingDetect�������ժ���͹һ������ܻ���ָ�ժ���Ͷ��ߵ��������ʱ��Ҳ�����ñ��������������������
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <param name = ""></param>
        /// <returns>
        ///  HANG_UP_FLAG_FALSE  	û�йһ�
        ///  HANG_UP_FLAG_TRUE    ���Ѿ��һ����ӽ���HANG_UP_FLAG_START״̬��ʼ���һ�ʱ�����0.5�롣��
        ///  HANG_UP_FLAG_START  	��ʼ�һ�
        ///  HANG_UP_FLAG_PRESS_R   ����һ�²��
        /// </returns>
        /// <remarks> �ú���ֻ������ͨ����Ч��</remarks>
        /// <seealso cref = "StartHangUpDetect"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 HangUpDetect(Int32 wChnlNo);

        /// <summary>
        /// ĳͨ������һ����ʱ����
        /// </summary>
        /// <param name = "wChnlNo">ͨ������</param>
        /// <param name = "ClockType">��ʱ����(�û����õ�Ϊ3��9)</param>
        /// <remarks>����ʹ��ClockTypeΪ0~2�ļ�ʱ����</remarks>
        /// <seealso cref = ""/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 StartTimer(Int32 wChnlNo, Int32 ClockType);

        /// <summary>
        /// ���������شӼ�ʱ�����������ڵ�ʱ�䣬��λ0.01�롣
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <param name = "ClockType">��ʱ������</param>
        /// <returns>
        /// �Ӽ�ʱ�����������ڵ�ʱ�䣬��λ0.01�롣
        /// </returns>
        /// <remarks>����������ֵ�ĵ�λ��0.01�룬�����Ǻ��롣����Ϊ������ǰ��DOS��WIN31�µĺ������ݡ�</remarks>
        /// <seealso cref = "StartTimer"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 ElapseTime( Int32 wChnlNo, Int32 ClockType);

        #endregion

        #region ��ͨ����

        /// <summary>
        /// ��·��ͨ��  ��one��another����ͬһƬ����ʱ����INI��Ӧ�趨Connect=1�����ҿ��뿨֮��Ҫ�е������ӡ�
        /// </summary>
        /// <param name = "wOne">ͨ����һ(0~255)</param>
        /// <param name = "wAnOther">ͨ���Ŷ�(0~255)</param>
        /// <returns>
        ///  0     	�ɹ�
        /// -1    	ͨ����һ ������Χ 
        /// -2    	ͨ���Ŷ� ������Χ
        /// -3      ����֮��û�����ӵ���ʱ��ͨ����һ��ͨ���Ŷ�����ͬһ�鿨�ϣ��޷���ͨ
        /// -4   	ͨ����һ ���ڷ���
        /// -5    	ͨ���Ŷ� ���ڷ���
        /// </returns>
        /// <remarks>
        /// ����ͨ��������ͨʱ�������ĳһ��ͨ�����з���StartPlay��ֹͣ����StpPlay�������ɵ�����ͨ����ˣ��ڵ��ñ�����֮ǰ����������ֹͣ����������֤����ͨ�Ĺ����в�����һ��ͨ��������ֹͣ������
        /// �������Ⲧ�š������ź����ڱ����϶����ڷ�������ˣ����϶Է�����Ҫ�����Щ����Ҳͬ�����á�
        /// </remarks>
        /// <seealso cref = "ClearLink"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 SetLink(Int32 wOne, Int32 wAnOther);

        /// <summary>
        /// ��·�����ͨ��
        /// </summary>
        /// <param name = "wOne">ͨ����һ(0~255)</param>
        /// <param name = "wAnOther">ͨ���Ŷ�(0~255)</param>
        /// <returns>
        ///  0     	�ɹ�
        /// -1    	ͨ����һ ������Χ 
        /// -2    	ͨ���Ŷ� ������Χ
        /// -3      ����֮��û�����ӵ���ʱ��ͨ����һ��ͨ���Ŷ�����ͬһ�鿨�ϣ��޷���ͨ
        /// -4   	ͨ����һ ���ڷ���
        /// -5    	ͨ���Ŷ� ���ڷ���
        /// </returns>  
        /// <seealso cref = "SetLink"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 ClearLink(Int32 wOne, Int32 wAnOther);

        /// <summary>
        /// ��·������ͨ�����øú�����ʵ�ֵ�����ͨ��ͨ��One��������Another����������Another������One��������
        /// </summary>
        /// <param name = "wOne">ͨ����һ(0~255)</param>
        /// <param name = "wAnOther">ͨ���Ŷ�(0~255)</param>
        /// <returns>
        ///  0     	�ɹ�
        /// -1    	ͨ����һ ������Χ 
        /// -2    	ͨ���Ŷ� ������Χ
        /// -3      ����֮��û�����ӵ���ʱ��ͨ����һ��ͨ���Ŷ�����ͬһ�鿨�ϣ��޷���ͨ
        /// -4   	ͨ����һ ���ڷ���
        /// -5    	ͨ���Ŷ� ���ڷ���   
        /// </returns>
        /// <remarks>���ĺ���SetLink��ע�����</remarks>
        /// <seealso cref = "ClrOneFromAnother"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 LinkOneToAnother(Int32 wOne, Int32 wAnOther);

        
        /// <summary>
        /// ��·���������ͨ
        /// </summary>
        /// <param name = "wOne">ͨ����һ(0~255)</param>
        /// <param name = "wAnOther">ͨ���Ŷ�(0~255)</param>
        /// <returns>
        ///  0     	�ɹ�
        /// -1    	ͨ����һ ������Χ 
        /// -2    	ͨ���Ŷ� ������Χ
        /// -3      ����֮��û�����ӵ���ʱ��ͨ����һ��ͨ���Ŷ�����ͬһ�鿨�ϣ��޷���ͨ
        /// -4   	ͨ����һ ���ڷ���
        /// -5    	ͨ���Ŷ� ���ڷ���   
        /// </returns>
        /// <seealso cref = "LinkOneToAnother"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 ClearOneFromAnother(Int32 wOne, Int32 wAnOther);

        /// <summary>
        /// ������ͨ��������ʵ�����ǵ������ε�����ͨ����LinkOneToAnother��
        /// </summary>
        /// <param name = "wOne">��ͨ�ĵ�һ·ͨ����</param>
        /// <param name = "wTwo">��ͨ�ĵڶ�·ͨ����</param>
        /// <param name = "wThree">��ͨ�ĵ���·ͨ����</param>
        /// <returns>
        ///  0      �ɹ�
        /// -1    	ͨ����һ ������Χ��
        /// -2    	ͨ���Ŷ� ������Χ
        /// -3      ����֮��û�����ӵ���ʱ��ͨ����һ��ͨ���Ŷ���ͨ����������ͬһ�鿨�ϣ��޷���ͨ
        /// -4   	ͨ����һ ���ڷ���
        /// -5    	ͨ���Ŷ� ���ڷ���
        /// -6    	ͨ������ ������Χ
        /// -7    	ͨ������ ���ڷ���
        /// </returns>
        /// <remarks> ����������ͨ���õ�����ͨ��ʵ�ֵģ���ˣ�A��B��������Ƚϵ�С�����Ƶ����Ҳ�������B��C�ϡ������ʵ�ֶ෽ͨ������ο�������ʵ�֡���</remarks>
        /// <seealso cref = "ClearThree"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 LinkThree(Int32 wOne, Int32 wTwo, Int32 wThree);

        /// <summary>
        /// ���������ͨ��
        /// </summary>
        /// <param name = "wOne">��ͨ�ĵ�һ·ͨ����</param>
        /// <param name = "wTwo">��ͨ�ĵڶ�·ͨ����</param>
        /// <param name = "wThree">��ͨ�ĵ���·ͨ����</param>
        /// <returns>
        ///  0      �ɹ�
        /// -1    	ͨ����һ ������Χ��
        /// -2    	ͨ���Ŷ� ������Χ
        /// -3      ����֮��û�����ӵ���ʱ��ͨ����һ��ͨ���Ŷ���ͨ����������ͬһ�鿨�ϣ��޷���ͨ
        /// -4   	ͨ����һ ���ڷ���
        /// -5    	ͨ���Ŷ� ���ڷ���
        /// -6    	ͨ������ ������Χ
        /// -7    	ͨ������ ���ڷ���
        /// </returns>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 ClearThree(Int32 wOne, Int32 wTwo, Int32 wThree);

        #endregion

        #region �����к����йصĺ���

        /// <summary>
        /// ��ʼ��ĳ·��Caller-ID��������
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <seealso cref = "GetCallerIDStr"/>
        /// <seealso cref = "GetCallerIDRawStr"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void ResetCallerIDBuffer(Int32 wChnlNo);
        
        /// <summary>
        /// ���Caller-ID�����ݡ�IDStr�������û�������ռ䣬����֤�㹻������е����к��룬128���ֽ��Ǿ��԰�ȫ�ġ�
        /// ������ֵ����3��4ʱ��CallerIdStr �д��н��յ���CallerID��Ŀǰ�����й涨�ĵ�һ��ʽ�򸴺ϸ�ʽ��FSK���к��룬��������������ȷ�Ľ��ա�
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <param name = "IDStr">���յ������к�����Ϣ</param>
        /// <returns>
        /// ���յ������������
        /// #define ID_STEP_NONE          0       //δ�յ��κ���Ϣ
        /// #define ID_STEP_HEAD          1       //���ڽ���ͷ��Ϣ
        /// #define ID_STEP_ID            2       //���ڽ���ID����
        /// #define ID_STEP_OK            3       //������ϣ�У����ȷ
        /// #define ID_STEP_FAIL          4       //������ϣ�У�����
        /// </returns>
        /// <remarks>����FSK���ڵ�һ������֮���͵ģ�����Щ�ط��������ڵڶ�������֮���͵ģ�����ˣ�����һ��⵽�����ժ�����ڼ�⵽������ʱ���������ȵ���ResetCallerIDBuffer��������������3��4ʱ������ժ����OffHook�������⣬����Ҫ�趨��ʱ������һ��ʱ�����ղ������к���ʱ����ժ����</remarks>
        /// <seealso cref = "ResetCallerIDBuffer"/>
        /// <seealso cref = "GetCallerIDRawStr"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 GetCallerIDStr(Int32 wChnlNo, byte[] IDStr);
        
        /// <summary>
        /// ���������GetCallerIDStr�Ļ����ϣ���һ��������ʱ�䡢���к��롢�����û���Ϣ�ֽ������StrTime, strCallerID, strUser������������Ҫ�û��Լ������ַ�ռ䣬��ָ�봫����������
        /// ������ɺ󣬵õ�����Ϣ�ͱ�������Ӧ�Ļ������С����ĳ������������Ϊ�գ�����strCallIDΪ�գ�˵��û�н��յ�����Ϣ�����к���û�н��յ���ͨ������Ϊ�Է�����Ϊ�������к��롣
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <param name = "strTime">���յ�������ʱ����Ϣ</param>
        /// <param name = "strCallerID">���յ������к���</param>
        /// <param name = "strUser">���յ��������û���Ϣ</param>
        /// <returns>
     �� /// ���յ������������
        /// #define ID_STEP_NONE          0       //δ�յ��κ���Ϣ
        /// #define ID_STEP_HEAD          1       //���ڽ���ͷ��Ϣ
        /// #define ID_STEP_ID            2       //���ڽ���ID����
        /// #define ID_STEP_OK            3       //������ϣ�У����ȷ
        /// #define ID_STEP_FAIL          4       //������ϣ�У�����
        /// </returns>
        /// <remarks>����FSK���ڵ�һ������֮���͵ģ�����Щ�ط��������ڵڶ�������֮���͵ģ�����ˣ�����һ��⵽�����ժ�����ڼ�⵽������ʱ���������ȵ���ResetCallerIDBuffer��������������3��4ʱ������ժ����OffHook�������⣬����Ҫ�趨��ʱ������һ��ʱ�����ղ������к���ʱ����ժ����</remarks>
        /// <seealso cref = "ResetCallerIDBuffer"/>
        /// <seealso cref = "GetCallerIDRawStr"/>
        /// <seealso cref = "GetCallerIDStr"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 GetCallerIDStrEx(Int32 wChnlNo, byte[] strTime, byte[] strCallerID, byte[] strUser);
  
        /// <summary>
        /// ����յ���Caller-ID��ԭʼ���ݡ�IDRAWStr�������û�������ռ䣬����֤�㹻������е����к��룬128���ֽ��Ǿ��԰�ȫ�ġ�
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <param name = "IDRawStr">��Ž��յ���CallerIDԭʼ��Ϣ��</param>
        /// <returns>
        /// Ŀǰ�Ѿ��յ���CallerIDԭʼ��Ϣ���ĸ�����
        /// </returns>
        /// <seealso cref="ResetCallerIDBuffer"/>
        /// <seealso cref="GetCallerIDStr"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 GetCallerIDRawStr(Int32 wChnlNo, byte[] IDRawStr);

        #endregion

        #region ���鹦��

        /// <summary>
        /// ��ʼ�����鹦�ܣ����ñ���������ʼ��DLL���ڲ�������
        /// ��ϵͳ�У�һ����32��������Դ����������10����飬ÿ�������������6����Ա��
        /// </summary>
        /// <returns>
        /// 0						�� �ɹ�
        /// 1						�� ����D161A��
        /// 2						�� ��INI�У�Connect������1
        /// 3						�� �Ѿ�ʹ����ģ��Ļ��鿨�����ҳ�ʼ���ɹ�
        /// </returns>
        /// <seealso cref="DConf_DisableConfCard"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DConf_EnableConfCard();

        /// <summary>
        /// ��ֹ���鹦�ܣ������˳�ʱ���á�
        /// </summary>
        /// <seealso cref="DConf_EnableConfCard"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void DConf_DisableConfCard();

        /// <summary>
        /// �õ���ǰ�ܵĿ��õĻ�����Դ�����μ�����DConf_EnableConfCard�жԻ�����Դռ�õ�˵����
        /// </summary>
        /// <returns>���õĻ�����Դ��</returns>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DConf_GetResNumber();

        /// <summary>
        /// ʹ��DTMF����
        /// </summary>
        /// <param name="wChnl">ͨ����</param>
        /// <param name="wCtrl">0����1��1��ʹ��DTMF����</param>
        /// <returns>
        ///  0						�� �ɹ�
        /// -1						��  ͨ���Ŵ���
        /// -2						��  wCtrl����
        /// -3						��  ��ͨ��δ�������
        /// </returns>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DConf_Adjust_CtrlWord(long wChnl, Int32 wCtrl);

        /// <summary>
        ///     ��һ��ͨ������ĳ����顣���ñ�������ϵͳ����32��������Դ�з���һ����Դ��Ȼ��������
        /// ����Դ��ͨ�����뱾����顣һ��ͨ������ĳ�����󣬸�ͨ������������������ݣ���������
        /// Ҳ���Ա������ĳ�Ա������
        ///     ����ChnlAtte��ʾ�����������棬Ϊ�˷�ֹ�������������������һ������û�ͨ��Ҫ˥��-6db��
        /// Ϊ������ǰģ����鿨�ļ��ݣ����������Ϊ ATTE_MINUS_3DB(0X40) ��ATTE_MINUS_6DB(0X80)ʱ��
        /// ϵͳ���Զ�ת��Ϊ-3��-6��
        /// </summary>
        /// <param name="ConfNo">�������ţ���Чֵ1~10</param>
        /// <param name="ChannelNo">ͨ����</param>
        /// <param name="ChnlAtte">�����������Чֵ-20db��+20db</param>
        /// <param name="NoiseSupp">����0xCD����ʾ��ͨ��ֻ��˵������</param>
        /// <returns>
        /// 0							�� �ɹ�
        /// 1							�� ConfNoԽ��
        /// 2							�� ChannelNoԽ��
        /// 3							�� û�п��õ���Դ
        /// </returns>
        /// <seealso cref="SubChnl"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 AddChnl(Int32 ConfNo, Int32 ChannelNo, Int32 ChnlAtte, Int32 NoiseSupp);

        /// <summary>
        /// ��һ��ͨ����ĳ�������ȥ�������ñ��������ͷű�ͨ��ռ�õ�һ����Դ��
        /// </summary>
        /// <param name="ConfNo">�������ţ���Чֵ1~10</param>
        /// <param name="ChannelNo">ͨ����</param>
        /// <returns>
        /// 0							�� �ɹ�
        /// 1							�� ConfNoԽ��
        /// 2							�� ChannelNoԽ��
        /// 3							�� ����ChannelNo�ҵ�����Դ�Ƿ�
        /// </returns>
        /// <seealso cref="AddChnl"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 SubChnl(Int32 ConfNo, Int32 ChannelNo);

        /// <summary>
        /// һ��ͨ������ĳ����顣
        /// ����ĳ����飬����һ��ͨ�����ñ�����ʱ����ռ��һ����Դ���Ժ�����������������ͨ������
        /// ���ñ���Դ�����ԣ�ĳ��������������ͨ��ֻռ��һ����Դ��
        /// </summary>
        /// <param name="ConfNo">�������ţ���Чֵ1~10</param>
        /// <param name="ChannelNo">ͨ����</param>
        /// <returns>
        /// 0							�� �ɹ�
        /// 1							�� ConfNoԽ��
        /// 2							�� ChannelNoԽ��
        /// 3							�� û�п��õ���Դ
        /// </returns>
        /// <seealso cref="SubListenChnl"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 AddListenChnl(Int32 ConfNo, Int32 ChannelNo);

        /// <summary>
        /// һ��ͨ��ȥ����ĳ������������
        /// ����ĳ����飬��������ͨ��ֻռ��һ����Դ�������һ��������ͨ�����ñ�����ʱ�����ͷŸ���Դ��
        /// </summary>
        /// <param name="ConfNo">�������ţ���Чֵ1~10</param>
        /// <param name="ChannelNo"> ͨ����</param>
        /// <returns>
        /// 0							�� �ɹ�
        /// 1							�� ConfNoԽ��
        /// 2							�� ChannelNoԽ��
        /// 3							�� ����ChannelNo�ҵ�����Դ�Ƿ�
        /// </returns>
        /// <seealso cref="AddListenChnl"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 SubListenChnl(Int32 ConfNo, Int32 ChannelNo);

        /// <summary>
        /// ��һ��ͨ����¼��ת��Ϊ�Ի����¼����������ռ��һ����Դ��
        /// ���һ��ͨ��������ĳ����飬�������StartRecordFile�Ը�ͨ������¼��������¼�������ǶԸ�
        /// ͨ�����ڵĻ����¼������ʱ������Ե��ñ�������ǿ�ƶԻ������¼����
        /// </summary>
        /// <param name="ConfNo">�������ţ���Чֵ1~10</param>
        /// <param name="ChannelNo">ͨ����</param>
        /// <returns>
        /// 0							�� �ɹ�
        /// 1							�� ConfNoԽ��
        /// 2							�� ChannelNoԽ��
        /// 3							�� û�п��õ���Դ
        /// 4							�� ����ʹ��D161A���û��鹦��
        /// </returns>
        /// <remarks>�����������ں���StartRecordFile֮����á�</remarks>
        /// <seealso cref="DConf_SubRecListenChnl"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DConf_AddRecListenChnl(Int32 ConfNo, Int32 ChannelNo);

        /// <summary>
        /// ���Ի����¼��ȥ�����ָ�Ϊ��ͨ����¼��,�������ͷ�һ����Դ���ڶ�һ�������¼����������
        /// Ҫ���ñ�������
        /// </summary>
        /// <param name="ConfNo">�������ţ���Чֵ1~10</param>
        /// <param name="ChannelNo">ͨ����</param>
        /// <returns>
        /// 0							�� �ɹ�
        /// 1							�� ConfNoԽ��
        /// 2							�� ChannelNoԽ��
        /// 3							�� û�п��õ���Դ
        /// 4							�� ����ʹ��D161A���û��鹦��
        /// </returns>
        /// <remarks>�����������ں���StopRecordFile֮����á�</remarks>
        /// <seealso cref="DConf_AddRecListenChnl"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DConf_SubRecListenChnl(Int32 ConfNo, Int32 ChannelNo);

        /// <summary>
        /// ����һ��ͨ���ķ���ת��Ϊ�Ի���ķ�����������ռ��һ����Դ��
        /// ���һ��ͨ��������ĳ����飬����Ե��ñ��������ڴ�֮������з����������ض��򵽸�������С�
        /// �����������ں���StartPlayFile֮ǰ���á�
        /// </summary>
        /// <param name="ConfNo">�������ţ���Чֵ1~10</param>
        /// <param name="ChannelNo">ͨ����</param>
        /// <param name="ChnlAtte">�����������Чֵ-20db��+20db</param>
        /// <param name="NoiseSupp">����</param>
        /// <returns>
        /// 0							�� �ɹ�
        /// 1							�� ConfNoԽ��
        /// 2							�� ChannelNoԽ��
        /// 3							�� û�п��õ���Դ
        /// 4							�� ����ʹ��D161A���û��鹦��
        /// </returns>
        /// <remarks>
        ///     ʹ��ʱ���������龰���������Ϊ3��������ͨ��1��5��9�������Ҫ�Ի���������������
        /// 1��5��9������������ѡȡһ�����ٶ�ѡ��5����ô�����ú���DConf_AddPlayChnl(3,5���Ϳ��Խ�ͨ��
        /// 5�ķ����ض��򵽻�����3�У��Ժ����ж�ͨ��5�ķ������������ᱻ����������ĳ�Ա�����������
        /// ������Ҫ��ν��з���/ֹͣ�����Ĳ����������Ҫ��ͨ��5�趨�������ķ�ʽ������ú���
        /// DConf_SubPlayChnl(3,5)���˺�ķ��������ͨ��5���С�
        ///     ע�⣺ͬһ������ͨ�����ܼȷ�����¼������ˣ��ղŵ��龰�У���������ͨ��5�Ի�������Ժ�
        /// �������Ҫ����������¼������ô���ͱ���ʹ��ͨ��1��9�е�һ����
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
        /// ��ĳͨ���Ի���ķ���ȥ�����ָ�Ϊ��ͨ���ķ������������ͷ�һ����Դ��
        /// </summary>
        /// <param name="ConfNo">�������ţ���Чֵ1~10</param>
        /// <param name="ChannelNo">ͨ����</param>
        /// <returns>
        /// 0							�� �ɹ�
        /// 1							�� ConfoԽ��
        /// 2							�� ChannelNoԽ��
        /// 3							�� û�п��õ���Դ
        /// 4							�� ����ʹ��D161A���û��鹦��
        /// </returns>
        /// <example>
        ///	    if ( CheckPlayEnd ( chnl ) )
        ///	    {
        ///	        StopPlayFile ( chnl );
        ///	        ch = ChGroup[chnl];
        ///	        DConf_SubPlayChnl ( ch, chnl );
        ///	    }
        /// </example>
        /// <remarks>�����������ں���StopPlayFile֮����á�</remarks>
        /// <seealso cref="DConf_AddPlayChnl"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DConf_SubPlayChnl(Int32 ConfNo, Int32 ChannelNo);

        /// <summary>
        ///     ��һ��ʱ϶ͨ������ĳ����顣���ñ�������ϵͳ����32��������Դ�з���һ����Դ��Ȼ����
        /// �������Դ��ʱ϶ͨ�����뱾����顣
        ///     ���������ɹ��󣬶�Ӧ�����ʱ϶��������pTS_CONF��ָ���һ��WORD�����У�Ӧ�ó������
        /// ��ס��WORD���Ա����˳�����ʱʹ�á�
        ///     ����ChnlAtte��ʾ�����������棬��Чֵ-20db��+20db��
        /// </summary>
        /// <param name="ConfNo">�������ţ���Чֵ1~10</param>
        /// <param name="wTimeSlot">Ҫ��������ʱ϶��</param>
        /// <param name="ChnlAtte">�����������Чֵ-20db��+20db</param>
        /// <param name="NoiseSupp">����</param>
        /// <param name="TS_CONF">ָ�룬ָ�������ʱ϶��</param>
        /// <returns>
        /// 0							�� �ɹ�
        /// 1							�� ConfNoԽ��
        /// 2							�� wTimeSlotԽ��
        /// 3							�� û�п��õĻ�����Դ
        /// 4       					�� û�гɹ���ʼ��
        /// 5							�� ����PCI�ӿڵ�ģ��������
        /// 6							�� �������ʱʧ��
        /// </returns>
        /// <example>
        ///     Int32		TestConfGroup = 1;			// �����õĻ�����ţ�������ѡ1-10
        ///     WORD	TS_IP;
        ///     WORD	TS_CONF;
        ///     // ��ȡIPͨ��ʱ϶
        ///     // pIPChn->number��¼ʵ�ʵ�IPͨ������
        ///     TS_IP  = DJH323_GetTimeSlot(pIPChn->number);	
        ///     if (TS_IP < 0) 
        ///     {
        ///     	return -3;		//	printf("Get DIP-PCI timeslot fail!\n");
        ///     }
        ///     // ��IPͨ����ʱ϶�������
        ///     // ͨ��ʱ϶�������ɹ��󣬱���TS_CONF�д������ʱ϶
        ///     rrr = DConf_AddChnl_TimeSlot ( TestConfGroup, TS_IP, 0, NOISE_NONE, &TS_CONF );
        ///     if ( rrr != 0 )
        ///     {
        ///     	return -4;
        ///     }
        ///     //����IPͨ���ͻ�������ʱ϶
        ///     if (DJH323_ConnectFromTS (pIPChn->number, TS_CONF) < 0)
        ///     {
        ///     	return -5;
        ///     }
        ///     pIPChn->wTS_CONF_Out = TS_CONF;		// ��ס�����ʱ϶ 
        ///     pIPChn->tsConnected = TRUE;
        ///     return 0;		// OK
        /// </example>
        /// <seealso cref="DConf_SubChnl_TimeSlot"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DConf_AddChnl_TimeSlot(Int32 ConfNo, long wTimeSlot, Int32 ChnlAtte, Int32 NoiseSupp, long[] TS_CONF);

        /// <summary>
        /// ����ĳ����顣
        /// ���ñ�������ϵͳ����32��������Դ�з���һ����Դ��Ȼ�����������Դ�ѱ�����������һ��ʱ϶��
        /// �����������óɹ��󣬶�Ӧ�����ʱ϶��������pTS_CONF��ָ���һ��WORD�����У�Ӧ�ó�������ס
        /// ��WORD���Ա����˳�����ʱʹ�á������ж��ͨ�����ø����ʱ϶����������顣
        /// </summary>
        /// <param name="ConfNo">�������ţ���Чֵ1~10</param>
        /// <param name="TS_CONF">ָ�룬ָ�������ʱ϶��</param>
        /// <returns>
        /// 0							�� �ɹ�
        /// 1							�� ConfNoԽ��
        /// 2							�� ����
        /// 3							�� û�п��õĻ�����Դ
        /// 4 				    		�� û�гɹ���ʼ��
        /// 5							�� ����PCI�ӿڵ�ģ��������
        /// 6							�� �������ʱʧ��
        /// </returns>
        /// <example>
        ///     Int32		TestConfGroup = 1;			// �����õĻ�����ţ�������ѡ1-10
        ///     WORD	TS_CONF;
        ///     // �����飬�ɹ��󣬱���TS_CONF�д��иû�������ʱ϶
        ///     rrr = DConf_AddListenChnl_TimeSlot ( TestConfGroup, &TS_CONF );
        ///     if ( rrr != 0 )
        ///     {
        ///     	return -4;
        ///     }
        ///     //����IPͨ���ͻ�������ʱ϶
        ///     if (DJH323_ConnectFromTS (pIPChn->number, TS_CONF) < 0)
        ///     {
        ///     	return -5;
        ///     }
        ///     pIPChn->wTS_CONF_Out = TS_CONF;		// ��ס�����ʱ϶ 
        ///     pIPChn->tsConnected = TRUE;
        ///     return 0;		// OK
        /// </example>
        /// <seealso cref="DConf_SubChnl_TimeSlot"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DConf_AddListenChnl_TimeSlot(Int32 ConfNo, long[] TS_CONF);

        /// <summary>
        /// ��һ��ʱ϶ͨ����ĳ�������ȥ�������ñ��������ͷű�ʱ϶ͨ��ռ�õ�һ��������Դ��
        /// ����DConf_AddChnl_TimeSlot����DConf_AddListenChnl_TimeSlot���óɹ��󣬽�����һ�������ʱ
        /// ϶ͨ���š�������ʹ�ø�ʱ϶ͨ�������˳����顣
        /// </summary>
        /// <param name="ConfNo">�������ţ���Чֵ1~10</param>
        /// <param name="TS_Out">��������ʱ϶</param>
        /// <returns>
        /// 0							�� �ɹ�
        /// 1							�� ConfNoԽ��
        /// 2							�� wTS_OutԽ��
        /// 3                       	�� ����wTS_Out�ҵ�����Դ�Ƿ�
        /// 4                       	�� û�гɹ���ʼ��
        /// 5                       	�� ����PCI�ӿڵ�ģ��������
        /// </returns>
        /// <example>
        ///     Int32		TestConfGroup = 1;			// �����õĻ�����ţ�������ѡ1-10
        ///     // ʱ϶�ӻ����˳�
        ///     rrr = DConf_SubChnl_TimeSlot ( TestConfGroup, pIPChn->wTS_CONF_Out );
        ///     if ( rrr != 0 )
        ///     {
        ///     	return -4;
        ///     }
        ///     //�Ͽ�IPͨ���ͻ���ʱ϶������
        ///     DJH323_DisconnectTS(pIPChn->number);
        ///     pIPChn->tsConnected = FALSE;
        /// </example>
        /// <seealso cref="DConf_AddChnl_TimeSlot"/>
        /// <seealso cref="DConf_AddListenChnl_TimeSlot"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DConf_SubChnl_TimeSlot(Int32 ConfNo, long TS_Out);

        #endregion 

        #region �ź�������

        /// <summary>
        /// ����ź������ĳ�ʼ���������ú�������WindowsĿ¼�µ�NewSig.ini�ļ��ж�ȡ�ź�������������Ϣ���������ò����������Ӳ�����б�Ҫ�����ã����������û��������õģ����ź�����Ƶ�ʣ�����ֻ�õ����У�����Ҳ���ܣ������õ���
        /// </summary>
        /// <param name = "Times"> ȱʡֵΪ0</param>
        [DllImport("NewSig.dll", CharSet = CharSet.Auto)]
        public static extern void Sig_Init(Int32 Times);
        
        /// <summary>
        /// ��ĳͨ�����йһ�æ����⡣�ú�����ͬʱ��⣱�ֻ�2��æ������ֵ���������ļ������á����ֵ��Ϊ�������⵽�����κΣ���æ��ʱ�����᷵�أ���
        /// ���ĳ��æ������������Ƶ�ʡ��ż�ֵ���������ȡ��������ȡ����ȵ�ƫ�Χ������æ������������Ƶ�ʵ���������ֵ�����ż�ֵʱ����Ϊ��ʱ��Ϊ����������Ϊ������������������Ԥ�ڵķ�Χ�ڣ�����Ϊ��⵽�����������羲��������Ԥ�ڷ�Χ�ڣ�����Ϊ��⵽����������������⵽���������ﵽ���ò����е�����æ����ʱ���������أ�����Ϊ�Ѽ�⵽�һ�æ����
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <returns>
        ///  0      δ��⵽æ��
        ///  1      �Ѽ�⵽æ��
        /// </returns>
        /// <remarks>��ע������ڽ����ź������֮ǰ������ú���StartSigCheck����ĳͨ�����ź����ɼ����̣������ڳ������й�������ѭ�����ú���FeedSigFuncά���ź����ɼ����̡�</remarks>
        /// <seealso cref = "Sig_Init"/>
        /// <seealso cref = "Sig_ResetCheck"/>
        [DllImport("NewSig.dll", CharSet = CharSet.Auto)]
        public static extern Int32 Sig_CheckBusy(Int32 wChnlNo);
        
        /// <summary>
        /// ��ʼĳͨ���ĺ������̡��ú���ֻ������ͨ���ĺ����������������ĺ���������ѭ������Sig_CheckDial����������ɡ�
        /// �籾���绰Ϊ���ߵ绰�����Ȳ�ĳһ����(�硯9��)���ܲ����ߣ���ʱ�轫����PreDialNum��ΪҪ�Ȳ���ת���ߺ��룬�硯9��,DialNum����ֻ�贫��Ҫ���������ߺ��뼴�ɡ��粻���Ȳ�ĳ���뼴��ֱ�Ӳ����ߺ��룬�ɽ�����PreDialNum��Ϊ�մ���
        /// �����ļ��еĺ���ģʽ����һ������Ϊ3������ĳ�ֺ���ģʽ�����ǿ�����������Ƶ�ʡ��ż�ֵ��ռ��æ����ز�������������ز����Ȳ�����
        /// ��һ�æ����ⲻͬ���ǣ���������ļ��ͬʱֻ�ܰ�ĳ����ģʽ���м�⡣Sig_StartDial�еĵڣ�������wMode��ΪҪѡ��ļ��ģʽ����ȡ0��1��׼ģʽ������2˫������ģʽ��
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        /// <param name = "DialNum">��������</param>
        /// <param name = "PreNum">ǰ������</param>
        /// <param name = "wMode">��������ģʽѡ��</param>
        /// <returns>
        ///  0      ʧ��
        ///  1      �ɹ�
        /// </returns>
        /// <seealso cref = "Sig_Init"/>
        /// <seealso cref = "Sig_CheckDial"/>
        [DllImport("NewSig.dll", CharSet = CharSet.Auto)]
        public static extern Int32 Sig_StartDial(Int32 wChnlNo, byte[] DialNum, byte[] PreNum, Int32 wMode);
        
        /// <summary>
        /// ��ϵͳ�е��ô˺������Ը��ݲ���nCadenceType�����͵õ��������ź����ĸ�����
        /// </summary>
        /// <param name = "wChNo">ͨ����</param>
        /// <param name = "nCadenceType">�ź���������
        /// SIG_CADENCE_BUSY     1			æ��
        /// SIG_CADENCE_RINGBACK 2  		������
        /// </param>
        /// <returns>
        /// ����Ϊ��⵽��nCadenceType�ź����ĸ���
        /// </returns>
        [DllImport("NewSig.dll", CharSet = CharSet.Auto)]
        public static extern Int32 Sig_GetCadenceCount(Int32 wChNo, Int32 nCadenceType);

        /// <summary>
        /// �ڵ��ú���Sig_StartDial�������Ź��̺󣬾Ϳ���ѭ������Sig_CheckDial����ά�ֲ��Ź��̣����������Ľ����ֱ���õ����Ϊֹ��
        /// </summary>
        /// <param name = "wChnlNo"></param>
        /// <param name = ""></param>
        /// <returns>
        ///  S_NORESULT     ��δ�ó����
        ///  S_NODIALTONE   û�в�����
        ///  S_BUSY         ��⵽�Է�ռ�ߵ�æ��
        ///  S_CONNECT      �Է�ժ�������Խ���ͨ��
        ///  S_NOBODY       �������ɴΣ����˽����绰
        ///  S_NOSIGNAL     û���ź���
        /// </returns>
        /// <remarks>
        /// ���ŵ�һ�����Ϊ��
        /// 1�������PreDialNum��Ϊ�գ����ӳ٣���󲦳�PreDialNum,��PreDialNumΪ�գ���ֱ�ӽ��벽�裳��
        /// 2�����PreDialNum�Ƿ��ѷ��ꡣ���ѷ���ת�����裳��
        /// 3������Ƿ��в��������粦�������ȴﵽ������DialToneAfterOffHook����ֵ������DialNum�봮����ת�����裴�����ڴ˲����ѵȴ�������NoDialToneAfterOffHook�����ʱ�䳤����δ��⵽���������򷵻�S_NODIALTONE��
        /// 4�����DialNum���Ƿ��꣬���ѷ������ӳ�StartDelay�������ʱ�䳤�Ⱥ���벽�裵��
        /// 5����ӽ���˲������Ѿ���������RingLen�����ʱ�䳤�ȣ���������δֹͣ�򷵻�S_NOSIGNAL;���ڴ˲����ѵȴ�������NoRingLen�����ʱ�䳤����δ��⵽�������򷵻�S_NOSIGNAL;���⵽ռ��æ�����ﵽ������BusySigCount��������֣��򷵻�S_BUSY;���⵽�Է�ժ�����򷵻�S_CONNECT;�����˲����Ѿ���������Ringback_NoAnswerTime�����ʱ�䳤�ȣ������Ѽ�⵽���������򷵻�S_NOBODY�������������S_NORESULT��
        /// ��ע����ǣ��ڽ��к���������֮ǰ������ú���StartSigCheck�����ź����ɼ����̣������ڽ��к���������ʱ��Ҫѭ������FeedSigFunc����ά���ź����ɼ����̡�
        /// </remarks>
        /// <seealso cref = "Sig_StartDial"/>
        [DllImport("NewSig.dll", CharSet = CharSet.Auto)]
        public static extern Int32 Sig_CheckDial(Int32 wChnlNo);
        
        /// <summary>
        /// ���æ�����Ļ������Լ��ڲ������������Է��һ���æ���󣬱�����ñ�������
        /// </summary>
        /// <param name = "wChnlNo">ͨ����</param>
        [DllImport("NewSig.dll", CharSet = CharSet.Auto)]
        public static extern void Sig_ResetCheck(Int32 wChnlNo);
        
        /// <summary>
        /// �����û��Լ���Ҫ���û����Ե����������������ָ�����͵��ź�����
        /// </summary>
        /// <param name = "nSigType">
        /// �ź������ͣ�Ŀǰֻ��������4�����͵��ź���
        /// SIG_DIALTONE      ������  
        /// SIG_BUSY1         æ��1
        /// SIG_BUSY2         æ��2
        /// SIG_RINGBACK      ������
        /// </param>
        /// <param name = "nFreq1">�ź���Ƶ�ʣ���λHZ</param>
        /// <param name = "nFreq2">�ź���Ƶ�ʣ���λHZ������ǵ�Ƶ�ź��������������Ϊ0</param>
        /// <param name = "dbAmp1">�ź������ȣ���λdb, ��Ӧ����nFreq1</param>
        /// <param name = "dbAmp2">�ź������ȣ���λdb, ��Ӧ����nFreq2, ����ǵ�Ƶ�ź��������������Ϊ0</param>
        /// <param name = "nOnTime">�ź���һ�������У����ʱ�䳤�ȣ� ��λ�Ǻ��룬��������SIG_DIALTONE��Ч</param>
        /// <param name = "nOffTime">�ź���һ�������У�ͣ��ʱ�䳤�ȣ� ��λ�Ǻ��룬��������SIG_DIALTONE��Ч</param>
        /// <param name = "iSampleRate">�����ʣ� һ���Ӷ������Ĳ�����������Чֵ4000��16000�� ϵͳ���ɵ�Ĭ���ź����Ĳ�������8000</param>
        /// <returns>
        ///  0      �����ɹ�ִ��
        /// -1      nSigType���ԣ�����ָ��������
        /// -2      nFreq1��nFreq2ͬʱС�ڻ����0
        /// -3      dbAmp1��dbAmp2ͬʱС�ڻ����-40
        /// -4      nOnTime��nOffTimeͬʱС�ڻ����0
        /// -5      iSampleRateС��4000�����16000
        /// </returns>
        /// <remarks>
        /// ����û�����Ҫ�Լ����ã�ϵͳ�Ѿ�Ĭ��Ϊ�û�������4���ź�����ֻ����ú���StartPlaySignal()�Ϳ��Էų�����Ĭ���ź������£�
        /// SIG_DIALTONE   �������� 450HZ
        /// SIG_BUSY1      æ��1��  450HZ�� ��350���롢ͣ350����
        /// SIG_BUSY1      æ��1��  450HZ�� ��350���롢ͣ350����
        /// SIG_BUSY2      æ��2��  450HZ�� ��700���롢ͣ700����
        /// SIG_RINGBACK   �������� 450HZ�� ��1�롢    ͣ4��
        /// �����������α����ã�����ͬһ�����ͣ��ź��������һ�ε������ɵġ���StartPlaySignal�����ĵ��ú�ֹͣ����ԭ��һ����û�иı䡣
        /// </remarks>
        [DllImport("NewSig.dll", CharSet = CharSet.Auto)]
        public static extern Int32 SetGenerateSigParam(Int32 nSigType, Int32 nFreq1, Int32 nFreq2, double dbAmp1, double dbAmp2, Int32 nOnTime, Int32 nOffTime, Int32 iSampleRate);
        
        #endregion

        #region ���溯��

        /// <summary>
        /// ��ʼ��FAX����ʹ֮��������������������Ϊÿ��FAXͨ������һ�������������СΪwBuffSize�ֽڡ�
        /// </summary>
        /// <param name="buffSize">ÿ·FAXͨ���ڽ��պͷ���FAX�ļ�ʱʹ�õĻ�������С</param>
        /// <returns>
        ///  0  				�� ��ʼ���ɹ�
        /// -1 					�� ��⴫����Դ��ʱ����������
        /// -2 					�� ĳƬ������Դ���ڴ������
        /// -3 					�� INI�����ļ�����
        /// -7 					�� �����ڴ�ʧ��
        /// </returns>
        /// <remarks>
        /// �ڷ���-1��-2��-3ʱ��������Ļ�ϵ������ڣ���һ��˵������������
        /// A��[TC08A-V.INI]		ERR_FAX_INI_01: FaxCardNo=%d  ��INI�ļ��У�FaxCardNo������������Χ��ӦΪ1��8��
        /// B��[TC08A-V.INI]		ERR_FAX_INI_02: MemAddr=%X    ��INI�ļ��У�MemAddr������������Χ��ӦΪD800��E000��E800��
        /// C��[TC08A-V.INI]		ERR_FAX_INI_03: Error Mode %d: Fax%d=%X ��INI�ļ��У�Fax%d�������Ƿ�
        /// D��[TC08A-V.INI]		ERR_FAX_INI_04: FaxStream=%d ��INI�ļ��У�FaxStream������������Χ��ӦΪ����10��
        /// E��[TC08A-V.INI]		ERR_CHK_FAX_01: FAX%d = %X �ڼ��ʱ����ַΪ%X��FAX��û���ҵ�
        /// F��[TC08A-V.INI]		ERR_CHK_FAX_01: %X, Map memory fail �ڼ��ʱ��ӳ���ڴ��ַʧ��
        /// G��[TC08A-V.INI]		ERR_ LOAD_FAX_PROG: [%s] ������װ�س���ʱ�����ļ�����[%s]
        /// ע�����
        ///     ������һ��Ҫ����������ʼ������EnableCard�ɹ�֮����á���DOS�����µı�������ͬ���ǣ�����ֵ0��ʾ�ɹ�������DOS�£�����1��ʾ�ɹ���
        /// </remarks>
        /// <seealso cref="DJFax_DisableCard"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_DriverReady(Int32 buffSize);

        /// <summary>
        /// ��ֹFAX���������ͷų�ʼ��ʱΪÿ��FAXͨ��������ڴ档�������ʱ���ô˺�����
        /// </summary>
        /// <seealso cref="DJFax_DriverReady"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void DJFax_DisableCard();

        /// <summary>
        ///  ȡ�ܵ�FAXͨ�����������һ̨������һƬ������Դ��������ͨ����Ϊ4����Ƭ������Դ��������ͨ����Ϊ8���Դ����ơ�
        /// </summary>
        /// <returns>���õ�FAXͨ����</returns>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 GetTotalFaxChnl();

        /// <summary>
        /// ������Դ���Լ���ͨ������������FAXͨ���໥��ͨ������Ȼ����Բ⡣��������ͨ��wChnl��ͨ��wChnl+1������������ˣ��ڵ���ʱ��Ӧ��֤wChnl ����0��2��4������
        /// </summary>
        /// <param name="wChnlNo">FAXͨ����</param>
        /// <returns>
        /// 1					�� ��ͨ�ɹ�
        /// 0					�� ��ͨʧ��
        /// </returns>
        /// <seealso cref="DJFax_SelfCheckBreakLink"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_SelfCheckSetLink(Int32 wChnlNo);

        /// <summary>
        /// ������Դ���Ͽ��Լ���ͨ��
        /// </summary>
        /// <param name="wChnlNo"></param>
        /// <returns>
        /// 1					�� �Ͽ��ɹ�
        /// 0			        �� �Ͽ�ʧ��
        /// </returns>
        /// <seealso cref="DJFax_SelfCheckSetLink"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_SelfCheckBreakLink(Int32 wChnlNo);

        /// <summary>
        /// ��ͨ������Դ��FAXͨ����ģ�⿨��ͨ����ֻ�е�������ͨ������FAXͨ���ϵ��������ܹ�ͨ��ģ�����������ⲿ�ӿڷ�������֮��Ȼ��
        /// </summary>
        /// <param name="wFaxChnlNo">FAXͨ����</param>
        /// <param name="wVoiceChnlNo">ģ�⿨ͨ����</param>
        /// <returns>
        /// 1					�� ��ͨ�ɹ�
        /// 0					�� ��ͨʧ�ܣ�FAXͨ���Ż�ģ��ͨ���ų����Ϸ���Χ��
        /// </returns>
        /// <remarks>
        /// ע�����
        ///     �ڵ��ñ�����֮ǰ��һ��Ҫ��֤wVoiceChnl�Ѿ�ֹͣ��������ģ�⿨ͨ��ֹͣ����������ͨ�����ú���StopPlayFile��ʵ�֡�
        /// </remarks>
        /// <seealso cref="DJFax_ClearLink"/>
        /// <seealso cref="DJFax_GetVoiceChnlOfFaxChnl"/>
        /// <seealso cref="DJFax_GetFaxChnlOfVoiceChnl"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_SetLink(Int32 wFaxChnlNo, Int32 wVoiceChnlNo);

        /// <summary>
        /// ���������Դ��FAXͨ����ģ�⿨ͨ������ͨ��
        /// </summary>
        /// <param name="wFaxChnlNo">FAXͨ����</param>
        /// <param name="wVoiceChnlNo">ģ�⿨ͨ����</param>
        /// <returns>
        /// 1					�� ����ɹ�
        /// 0   				�� ���ʧ��
        /// </returns>
        /// <seealso cref="DJFax_SetLink"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_ClearLink(Int32 wFaxChnlNo, Int32 wVoiceChnlNo);

        /// <summary>
        /// ȡ��FAXͨ�����ӵ�ģ��ͨ���š���������DJFax_SetLink��������ñ��������õ���FAXͨ������ͨ��ģ��ͨ���š���û��ģ��ͨ�����FAXͨ������ͨʱ�����ñ�����������-1��
        /// </summary>
        /// <param name="wFaxChnlNo">FAXͨ����</param>
        /// <returns>
        /// -1					�� û�����ӵ�ͨ��
        /// XX					�� ģ�⿨ͨ����
        /// </returns>
        /// <seealso cref="DJFax_SetLink"/>
        /// <seealso cref="DJFax_ClearLink"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_GetVoiceChnlOfFaxChnl(Int32 wFaxChnlNo);

        /// <summary>
        /// ȡ��ģ�⿨ͨ�����ӵ�FAXͨ���š���������DJFax_SetLink��������ñ��������õ���ģ�⿨ͨ������ͨ��FAXͨ���š���û��FAXͨ�����ģ��ͨ������ͨʱ�����ñ�����������-1��
        /// </summary>
        /// <param name="wVoiceChnlNo"> ģ�⿨ͨ����</param>
        /// <returns>
        /// -1					�� û�����ӵ�ͨ��
        /// XX					�� FAXͨ����
        /// </returns>
        /// <seealso cref="DJFax_SetLink"/>
        /// <seealso cref=" DJFax_ClearLink"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_GetFaxChnlOfVoiceChnl(Int32 wVoiceChnlNo);

        /// <summary>
        /// ȡһ�����е�FAXͨ������ĳ��ģ�⿨ͨ�������ֿ����м�ͨ����ҪFAX��Դ��ʱ��������Ҫ���ñ���������ȡһ�����õ�FAXͨ����
        /// </summary>
        /// <returns>
        /// -1					�� û�п��õ�FAXͨ��
        /// XX					�� ���õ�FAXͨ��
        /// </returns>
        /// <remarks>
        /// �����ñ�����ʱ��ϵͳ����ѭ����ʽ����һ�����õ�FAXͨ�������磺0~3·FAXͨ����Ϊ���У����һ�ε��ñ�����������ͨ��0���ڶ��ε��ñ�����������ͨ��1���������ơ��������ĺô��ǣ�����һ·FAXͨ���𻵻����ʱ��ϵͳ�Կ�������������
        /// ��ϵͳ���ڲ�����Ӧÿ��FAXͨ����������־��FaxBusyFlag��GF_ChnlFlag��ϵͳ��ʼ����ʱ����������־�趨Ϊ0����������־��Ϊ0ʱ����ʾ��ͨ�����С������ú���DJFax_SetLink��FaxBusyFlag����Ϊ1�������ú���DJFax_ClearLink��FaxBusyFlag����Ϊ0�������ú���DJFax_SendFaxFile��DJFax_RcvFaxFile��GF_ChnlFlag����Ϊ1����ʾ��FAXͨ�����ڹ���״̬�������ͻ����FAX��ϣ�������ȷ�����ʹ����������GF_ChnlFlag����Ϊ0�������ú���DJFax_StopFax��ϵͳ��Ŭ��ʹ��FAXͨ����λ������ͨ����λ��GF_ChnlFlag����Ϊ0��
        /// Ϊ�˱��ʱ�ķ��㣬�ں���DJFax_ClearLink������˶Ժ���DJFax_StopFax�ĵ��á�
        /// ע�����
        ///     ������һ��ϵͳ�У�FAXͨ����Դ�������㹻�ģ����ǿ��ܻ����FAXͨ���������������ʱ�����ñ�����������-1����ʾû�п��õ�ͨ����Ӧ�ó���Ӧ���ܶ����������������һ�ְ취�Ƿ����ֵȴ���ֱ���п��õ�FAXͨ������һ�ְ취�Ƿ�������ʾ����֪�û�ϵͳæ��Ȼ�����û��һ���
        /// </remarks>
        /// <seealso cref="DJFax_GetOneFreeFaxChnlOld"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_GetOneFreeFaxChnl();

        /// <summary>
        /// ȡһ�����е�FAXͨ�����������Ĺ����뺯��DJFax_GetOneFreeFaxChnl��ͬ���ο�����Ӧ�ĺ���˵��������������˳��ʽ����һ�����õ�FAXͨ�������磺0~3·FAXͨ����Ϊ���У����һ�ε��ñ�����������ͨ��0���ڶ��ε��ñ�������Ȼ����ͨ��0���������ơ�
        /// </summary>
        /// <returns>
        /// -1					�� û�п��õ�FAXͨ��
        /// XX					�� ���õ�FAXͨ��
        /// </returns>
        /// <remarks>
        /// ��������Ϊ�˼�����ǰ�ĺ�����
        /// </remarks>
        /// <seealso cref="DJFax_GetOneFreeFaxChnl"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_GetOneFreeFaxChnlOld();

        /// <summary>
        /// ���ñ��η���FAX�ļ��ķֱ��ʡ�һ������£�FAX���������ֱַ��ʣ���׼�ֱ����ڴ�ֱ����Ϊÿ����3.85ɨ���У��߷ֱ���Ϊÿ����7.7ɨ���У����зֱ�����ˮƽ����Ϊÿ����8���㣬��ˣ�A4ֽ����Ϊ1728���㡣
        /// </summary>
        /// <param name="wChnlNo"> FAXͨ����</param>
        /// <param name="ResolutionFlag">�ֱ��ʣ����ܵ�ֵΪ��HIGH_RESOLUTION��1���߷ֱ���LOW_RESOLUTION��0����׼�ֱ���</param>
        /// <returns>
        /// 1					�� �ɹ�
        /// XX					�� ʧ�ܣ�FAXͨ���ų�����Χ��
        /// </returns>
        /// <remarks>
        /// �����ʹ�ú���DJCvt_Open����FAX�ļ�ʱ���Ѿ�ָ���˷ֱ��ʣ���ô���ڷ���FAXʱ�Ͳ������ñ��������趨�ֱ��ʡ��������ṩ�˶�̬�������ͷֱ��ʵ�һ�ַ�����������Ӱ�챾�η���ʱ�ķֱ��ʡ�
        /// ע�����
        ///     �����Ҫ�ñ��������趨����ʱ�ķֱ��ʣ�һ��Ҫ�ں���DJFax_ SendFaxFile֮����á�
        /// </remarks>
        /// <seealso cref="DJFax_SendFaxFile"/>
        /// <seealso cref="DJCvt_Open"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_SetResolution(Int32 wChnlNo, Int32 ResolutionFlag);

        /// <summary>
        /// ĳ��FAXͨ����ʼ����FAX�ļ����ļ���ʽΪBFX��ʽ������ġ�BFX���ļ���ʽ����
        /// </summary>
        /// <param name="wChnlNo">FAXͨ����</param>
        /// <param name="FileName">���͵�FAX�ļ���</param>
        /// <returns>
        /// -1 					�� ��FAX�ļ�ʧ��
        /// -2 					�� �ڴ����ʧ��
        /// -3    				�� FAX�ļ�����ҳ������
        /// -4 					�� FAX�ļ���ҳ���Ȼ�ҳ��ʼλ�ô���
        /// -5					�� ��ȡ�ļ�����
        /// >0		            �� ���ͳɹ�������Ҫ���͵���ҳ��
        /// </returns>
        /// <remarks>
        /// �����걾��������Ҫ���ϵ��ú���DJFax_CheckTransmit����ά�ַ��͵ļ����������Ҫ��;ֹͣ���ͣ���ʹ�ú���DJFax_StopFax��
        /// �ڵ��ñ�����ʱ�����͵ķֱ����ɸ��Զ˵�Э�̽�����Զ�DIS��������
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
        /// ĳ��FAXͨ����ʼ����FAX�ļ����ļ���ʽΪBFX��ʽ������ġ�BFX���ļ���ʽ����
        /// </summary>
        /// <param name="wChnlNo">FAXͨ����</param>
        /// <param name="FileName">���͵�FAX�ļ���</param>
        /// <param name="StartPage">������ʼҳ����Χ0�����ҳ�������Ҳ��ܴ��ڽ���ҳ��</param>
        /// <param name="EndPage">���ͽ���ҳ�����ڵ���StartPage������Ϊ��1���ߴ��ڵ����ļ����ҳ����ʱʵ�ʷ��ͽ���ҳΪ�ļ����һҳ��</param>
        /// <returns>
        /// -1 					�� ��FAX�ļ�ʧ��
        /// -2 					�� �ڴ����ʧ��
        /// -3    				�� FAX�ļ�����ҳ������
        /// -4 					�� FAX�ļ���ҳ���Ȼ�ҳ��ʼλ�ô���
        /// -5					�� ��ȡ�ļ�����
        /// >0		            �� ���ͳɹ�������Ҫ���͵���ҳ��
        /// </returns>
        /// <remarks>
        /// �����걾��������Ҫ���ϵ��ú���DJFax_CheckTransmit����ά�ַ��͵ļ����������Ҫ��;ֹͣ���ͣ���ʹ�ú���DJFax_StopFax��
        /// �ڵ��ñ�����ʱ�����͵ķֱ����ɸ��Զ˵�Э�̽�����Զ�DIS��������
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
        /// ĳ��FAXͨ����ʼ����FAX�ļ����ļ���ʽΪBFX��ʽ������ġ�BFX���ļ���ʽ����
        /// </summary>
        /// <param name="wChnlNo">FAXͨ����</param>
        /// <param name="FileName">���յ�FAX�ļ���</param>
        /// <returns>
        ///  1    			    �� �ɹ�����ʼ�����ļ�
        /// -1					�� ��FAX�ļ�ʧ��
        /// -2					�� �ڴ����ʧ��
        /// </returns>
        /// <remarks>
        /// �����걾��������Ҫ���ϵ��ú���<seealso cref="DJFax_CheckTransmit"/>����ά�ֽ��յļ����������Ҫ��;ֹͣ���ͣ���ʹ�ú���DJFax_StopFax��
        /// ���ν��յķֱ��ʣ�����BFX�ļ�ͷ�У���ο�����<seealso cref="DJCvt_Open"/>��˵����
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
        /// ���FAX�ļ��Ľ��ջ����Ƿ���������ú���DJFax_SendFaxFile����DJFax_RcvFaxFile��ʼһ�����ͻ���պ���Ҫ���ϵĵ��ñ�������ά�ַ��ͻ���յļ�����ͬʱ����ⷢ�͵Ľ������������Ӧ�Ĵ���
        /// </summary>
        /// <param name="wChnlNo">FAXͨ����</param>
        /// <returns>
        ///  2  			    �� ĳһҳ����
        ///  1					�� ���еķ��ͻ������ȷ����
        ///  0					�� ���ڷ��ͻ����         
        /// -2					�� ���ļ�ʧ�ܻ����������
        /// </returns>
        /// <seealso cref="DJFax_SendFaxFile"/>
        /// <seealso cref="DJFax_SendFaxFileEx"/>
        /// <seealso cref="DJFax_Send"/>
        /// <seealso cref="DJFax_RcvFaxFile"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_CheckTransmit(Int32 wChnlNo);

        /// <summary>
        /// ֹͣ���ջ���FAX�ļ���
        /// </summary>
        /// <param name="wChnlNo"> FAXͨ����</param>
        /// <seealso cref="DJFax_SendFaxFile"/>
        /// <seealso cref="DJFax_SendFaxFileEx"/>
        /// <seealso cref="DJFax_Send"/>
        /// <seealso cref="DJFax_RcvFaxFile"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void DJFax_StopFax(Int32 wChnlNo);

        /// <summary>
        /// ���ñ��ص�ID�š����շ�FAX�Ĺ����У������趨������ID�ţ���ID��һ����ڴ���Ķ�����ʾ������
        /// </summary>
        /// <param name="wChnlNo">FAXͨ����</param>
        /// <param name="Str">ID�ַ���</param>
        /// <returns>
        /// 1   				�� ���óɹ�
        /// 0  					�� ����ʧ�ܣ�FAXͨ���ų�����Χ��
        /// </returns>
        /// <remarks>
        /// ע�����
        ///     ����s���ԡ�\0��������ASCII�ַ���������һ�����ܳ���20���ַ���
        /// </remarks>
        /// <seealso cref="DJFax_GetLocalID"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_SetLocalID ( Int32 wChnlNo, byte[] Str);

        /// <summary>
        /// ��ȡ����DJFax_SetLocalID���趨�ı���ID�š�
        /// </summary>
        /// <param name="wChnlNo">FAXͨ����</param>
        /// <param name="Str">ID�ַ���</param>
        /// <returns>
        /// 1  					�� ��ȡID�ɹ�
        /// 0   				�� ��ȡIDʧ��
        /// </returns>
        /// <remarks>
        /// ע�����
        ///     ϵͳ�еı���ID�ĳ���Ϊ20���ַ�������s�Ŀռ������Ӧ�ó�����䣬����֤����20���ַ���
        /// </remarks>
        /// <seealso cref="DJFax_SetLocalID"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_GetLocalID ( Int32 wChnlNo, byte[] Str);

        /// <summary>
        /// ȡ��ǰҳ�Ѿ����յ����ֽ����������ú���DJFax_RcvFaxFile�󣬿����ñ��������鿴��ǰҳ�յ����ֽ�����
        /// </summary>
        /// <param name="wChnlNo">FAXͨ����</param>
        /// <returns>
        /// ��ǰҳ�ѽ��յ����ֽ���
        /// </returns>
        /// <seealso cref="DJFax_RcvFaxFile"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern long DJFax_GetRcvBytes(Int32 wChnlNo);

        /// <summary>
        /// ȡ��ǰҳ�Ѿ����͵��ֽ����������ú���DJFax_SendFaxFile�󣬿����ñ��������鿴��ǰҳ�Ѿ����͵��ֽ�����
        /// </summary>
        /// <param name="wChnlNo">FAXͨ����</param>
        /// <returns>
        /// ��ǰҳ�ѷ��͵��ֽ���
        /// </returns>
        /// <seealso cref="DJFax_SendFaxFile"/>
        /// <seealso cref="DJFax_SendFaxFileEx"/>
        /// <seealso cref="DJFax_Send"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern long DJFax_GetSendBytes(Int32 wChnlNo);

        /// <summary>
        /// ���ò������롣�����Ҫ��FAXͨ��������һ�κ�������ô����Ҫ�ñ��������趨�����ĵ绰���롣
        /// </summary>
        /// <param name="wChnlNo">FAXͨ����</param>
        /// <param name="DialNo">�����ĺ���</param>
        /// <returns>
        /// 1				    �� ���óɹ�
        /// 0					�� ����ʧ��
        /// </returns>
        /// <seealso cref="DJFax_SendFaxFile"/>
        /// <seealso cref="DJFax_SendFaxFileEx"/>
        /// <seealso cref="DJFax_Send"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_SetDialNo(Int32 wChnlNo, byte[] DialNo);

        /// <summary>
        /// ȡ����ʱ�Ĵ���š�������DJFax_CheckTransmit�ķ���ֵΪ-2ʱ����ʾ���ͻ����FAX����
        /// </summary>
        /// <param name="wChnlNo">FAXͨ����</param>
        /// <returns>
        /// ����ͨ���������õ�����Ĵ�����룺
        /// 255     TIME_OUT
        ///   0     DJ_SUCCESS
        ///   1     ERROR��ATָ���쳣���أ�һ�㲻�᷵�ش˴�����
        ///   2     NO_CARRIER������T30Э��ȴ����նԷ��źŵ�ʱ��û�н��յ��źų�ʱ�˳����ɽ�ϳ�����״̬׷�ݵ���ʱ�����λ��
        ///   3     NOT USED
        ///   4     NO_DIALTONE��NOT USED
        ///   5     BUSY��NOT USED
        ///   6     RETRY_FAIL���ﵽT30Э��涨�����Դ�����δ�ܳɹ����FAX�е����ֹ��̣��ɽ�ϳ�����״̬׷�ݵ���ʱ�����λ��
        ///   7     HDLC_UNEXP�����յ�Υ��T30Э��涨���������Ӧ��
        ///   8     TRAIN_FAIL��NOT USED
        ///   9     HDLC_NOT_T4_REV���Է���֧��T4����G3�ࣩ����
        ///         REMOTE_DCN���յ��Է�������DCN��������
        ///         PRE_STOP���ϲ�Ӧ�ó���������ǰ����FAX����
        ///         NON_FAX���Է�����FAX�ն�
        ///  16     NOT_V29_RECEIVER 13���Է���֧��V29�������
        /// </returns>
        /// <seealso cref="DJFax_GetErrPhase"/>
        /// <seealso cref="DJFax_GetErrSubst"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_GetErrCode(Int32 wChnlNo);

        /// <summary>
        /// ȡ����ʱ��FAX_PHASEֵ��������DJFax_CheckTransmit�ķ���ֵΪ-2ʱ����ʾ���ͻ����FAX���󡣴�ʱ������ͨ���������õ������FAX_PHASEֵ�������Ϊ5���׶Σ�����ġ�ͨ�Ź��̡���
        /// </summary>
        /// <param name="wChnlNo">FAXͨ����</param>
        /// <returns>����ʱ��FAX_PHASEֵ</returns>
        /// <seealso cref="DJFax_GetErrCode"/>
        /// <seealso cref="DJFax_GetErrSubst"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_GetErrPhase(Int32 wChnlNo);

        /// <summary>
        /// ȡ����ʱ��FAX_SUBSTֵ����ֵΪ������Դ��ϵͳ�ڲ�ʹ�ã��û����Բ�����ᡣ
        /// </summary>
        /// <param name="wChnlNo">FAXͨ����</param>
        /// <returns>
        /// ����ʱ��FAX_SUBSTֵ
        /// </returns>
        /// <seealso cref="DJFax_GetErrCode"/>
        /// <seealso cref="DJFax_GetErrPhase"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_GetErrSubst(Int32 wChnlNo);

        /// <summary>
        /// ��ǰ�����ļ�ҳ��
        /// </summary>
        /// <param name="WChnlNo">FAXͨ����</param>
        /// <returns>��ǰҳ</returns>
        /// <seealso cref="DJFax_RcvFaxFile"/>
        /// <seealso cref="DJFax_CheckTransmit"/>
        /// <seealso cref="DJFax_SendFaxFile"/>
        /// <seealso cref="DJFax_SendFaxFileEx"/>
        /// <seealso cref="DJFax_Send"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_GetCurPage(Int32 WChnlNo);

        /// <summary>
        /// ��̬ת����ʼ�����ڱ������н�����ת��ʱ��������ݣ���ʼ���ڲ��������顣�ú�����ҪASC16��ASC24��HZK16��HZK24��HUFFMAN.COD�⼸���ļ��ڵ�ǰ��Ŀ¼�£�����᷵��-1��
        /// </summary>
        /// <returns>
        ///  1  				�� ��ʼ���ɹ�
        /// -1					�� ���ļ�ʧ��
        /// -2					�� �����ڴ�ʧ��
        /// </returns>
        /// <remarks>
        /// ������-1��-2ʱ��������Ļ�ϵ������ڣ���һ��˵������������
        /// A��"Open File Fail : [%s]"	���ļ����󣬿��ܵ��ļ���ΪASC16��ASC24��HZK16��HZK24��HUFFMAN.COD�� ���������������װĿ¼���޷��ҵ��⼸���ļ�������DOS��װ���ϲ��ҡ�
        /// B��"Alloc memory fail 1" ��ΪHUFFMAN.COD�����ڴ�ʱ����������
        /// C��"Alloc memory fail 2" ��ΪASC16�����ڴ�ʱ����������
        /// D��"Alloc memory fail 3" ��ΪHZK16�����ڴ�ʱ����������
        /// E��"Alloc memory fail 4" ��ΪASC24�����ڴ�ʱ����������
        /// F��"Alloc memory fail 5" ��ΪHZK24�����ڴ�ʱ����������
        /// G��"Alloc memory fail"   �ڷ��������ڴ�ʱ����������
        /// ��ϵͳ�ڣ���һ����СΪ32�Ľṹ���飬����ͬʱά�����32��FAX�ļ���ת��������Ҳ���Խ�ת���Ĺ��ܿ�����ϵͳ���õ���Դ��һ����32��FAXת����Դ�����³�Ϊת��ͨ����������һ��ϵͳ�ڣ������32��FAXͨ������ˣ�ת��ͨ������Ŀ���㹻�ġ�
        /// �����еĺ�����һ�㶼���в���wChnl�����ǳ�Ϊת��ͨ�����ò�����Ӧ������ṹ���飬wChnl�ķ�ΧΪ0~31�����wChnlԽ�磬�������ϵͳ���������������ģ��ͨ�����м�ͨ��ʹ��ͬһ��ת��ͨ��������ת����Ҳ�������ȷ���Ľ����
        /// �����ʹ��ת��ͨ���ķ��������¼��֣�
        /// �����ϵͳ�Ƚ�Сʱ��������֤ģ�⿨�����ֿ����м�ͨ����С�ڵ���32�����������м�ͨ����ֱ�Ӵ���ת��������
        /// �������ȷ��ÿ�η����FAXͨ����Ψһ�ģ�Ҳ������FAXͨ���Ŵ���ת����������ʱ����Ҫ�ȵ��ú���DJFax_GetOneFreeFaxChnl���ҵ�һ�����е�FAXͨ����Ȼ��ʹ�ú���DJFax_SetLink����FAXͨ��ռ�ã���ʱ�Ϳ���ʹ��FAXͨ����������ת���ˣ�Ҫע����ǣ����������ú���DJFax_SetLink����FAXͨ��ռ�á�
        /// �����Ҳ�����Լ�дһ�Ժ�����������ͷ���32��ת��ͨ�����⽫���ȫ�ġ�
        /// ע�����
        ///     �����е�ת��������DJCvt_�����漰��������ԴӲ������ˣ��������봫����Դ��Ӳ���������С�ת��������Ҫ��TC08A32.DLL��TCE1_32.DLL��֧�֡�
        /// �������Ҫʹ�ñ����е�ת�������������ڳ����ʼ��ʱ����ȷ�ĵ����˱����������򣬽������ϵͳ������
        /// </remarks>
        /// <seealso cref="DJCvt_DisableConvert"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJCvt_InitConvert();

        /// <summary>
        /// �ͷŶ�̬ת�����������Դ����DJCvt_InitConvert���ʹ�á�ϵͳ�˳�ʱ���á�
        /// </summary>
        /// <seealso cref="DJCvt_InitConvert"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void DJCvt_DisableConvert(); 

        /// <summary>
        ///  ׼����ʼһ��FAXת�����ɹ����ñ������󣬻Ὣ��ת��ͨ�����Ϊռ�á�
        /// cbFaxFileName��׼��д��FAX���ݵ�FAX�ļ�����������ļ������ڣ����½�һ����������ļ��Ѵ��ڣ������¸�д�������ļ����ɹ���������-1��
        /// </summary>
        /// <param name="wChnlNo">ת��ͨ����</param>
        /// <param name="cbFaxFileName">ת���ɵ�FAX�ļ���</param>
        /// <param name="cbResolution">ת���ֱ���HIGH_RESOLUTION��1���߷ֱ���STANDARD_RESOLUTION��2����׼�ֱ���</param>
        /// <param name="wPageLineNo">ÿҳɨ����</param>
        /// <returns>
        ///  1    			    �� �ɹ�
        /// -1					�� ���ļ�ʧ�ܻ�ת��ͨ���Ѿ���ռ��
        /// </returns>
        /// <remarks>
        /// CbResolution��������ת���ķֱ��ʡ�һ������£�FAX���������ֱַ��ʣ���׼�ֱ����ڴ�ֱ����Ϊÿ����3.85ɨ���У��߷ֱ���Ϊÿ����7.7ɨ���С��ֱ��ʽ���¼�����ɵ�FAX�ļ�ͷ�У�ƫ��1577��λ�ã������һ���ֽ�ΪA���� A & 0x40 == 0x40����ʾΪ�߷ֱ��ʣ������ʾΪ�ͷֱ��ʡ���ʹ�ú���DJFax_SendFaxFile����FAX�ļ�ʱ�����������ֽڵ�ֵ��ȷ������ʱӦ��ʹ�õķֱ��ʡ�����ʹ�ú���DJFax_RcvFaxFile���յ���FAX�ļ���ϵͳ���Զ����ļ�ͷ��д��Ӧ��ֵ������������ٷ��͸�FAX�ļ������ᰴ����ȷ�ķֱ��ʷ��͡��ֱ��ʲ�������Ӱ��ת�������е�DJCvt_TextLine��
        /// ���һ������wPageLineNo��ʾÿҳɨ���е�����������������趨���ڱ�׼�ֱ����£�һҳA4ֽ��ɨ���д�ԼΪ1100�У��ڸ߷ֱ����£�һҳA4ֽ��ɨ���д�ԼΪ2200�У������������óɹ�����Ϳ��Ը�����Ҫ���ú���DJCvt_TextLine��DJCvt_DotLine��DJCvt_BmpFile��������FAX�����ݣ����ﵽ�趨��ÿҳɨ��������ʱ�����Զ��л�Ϊ��һҳ��
        /// ע�����
        ///     һ��Ҫ��鱾�����ķ���ֵ�������ɹ�ʱ��Ҫ����Ӧ�Ĵ�������Ӧ�ó���������ú���DJFax_SendFaxFile�����п��ܽ��ϴ�ת������ʱFAX�ļ����ͳ�ȥ���Ӷ��������صĺ����
        /// </remarks>
        /// <seealso cref="DJCvt_Close"/>
        /// <seealso cref="DJCvt_TextLine"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJCvt_Open(Int32 wChnlNo, byte[] cbFaxFileName, byte cbResolution, Int32 wPageLineNo);

        /// <summary>
        ///  �ر�����ת����FAX�ļ�����ת��������һ��Ҫ���ñ�������FAX�ļ�������ȷ�Ĺرա�
        /// <param name="wChnlNo">ת��ͨ����</param>
        /// <returns>
        ///  1  				�� �ɹ�
        /// -1					�� ʧ�ܣ��ر�һ��û�д򿪵�ת��ͨ����
        /// </returns>
        /// <seealso cref="DJCvt_Open"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJCvt_Close(Int32 wChnlNo);

        /// <summary>
        /// ��ָ����ת��ͨ������һ��ɨ���С�ʵ���ϣ����е�ת���������ն��ǵ��ñ�������ʵ�ֵġ�
        /// </summary>
        /// <param name="wChnl">ת��ͨ����</param>
        /// <param name="cbDotStr">�����е�����</param>
        /// <param name="wDotSize">�����еĴ�С</param>
        /// <param name="wDotFlag">��־�����ܵ�ֵΪ��DOT_0_IS_WHITE��0��0��ʾ�׵� DOT_1_IS_WHITE��1��1��ʾ�׵�</param>
        /// <returns>
        ///  1   				�� �ɹ�
        /// -1					�� ʧ�ܣ�ת���ļ�û�д򿪣�
        /// </returns>
        /// <remarks>
        /// ���ַ���cbDotStr�У�ÿ���ֽڱ�ʾ8���㣻wDotSize��ʾ����ı���������־wDotFlag��һ����Ϊ0����ʾ��ĳ����Ϊ0ʱ�������λ���޵㣨�׵㣩��
        /// ���磬һ��ɨ����Ϊ16���׵㣬33���ڵ㣬ʣ���(1728-16-33)Ϊ�׵㣬�������±�ʾ��
        /// BYTE DotStr[ ] =  �� 0x00, 0x00, 0xff, 0xff, 0xff, 0xff, 0x80��;
        /// DJCvt_DotLine ( wChnl, (char *)DotStr, 16+33, DOT_0_IS_WHITE ); 
        /// Ҳ�������±�ʾ��
        /// BYTE DotStr[ ] =  �� 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x7f��;
        /// DJCvt_DotLine ( wChnl, (char *)DotStr, 16+33, DOT_1_IS_WHITE ); 
        /// ��ת��ʱ��ϵͳ���Զ���ʣ��ĵ����Ϊ�׵㡣
        /// ע�����
        ///     ÿ���ֽڿ��Ա�ʾ8���㣬��λ���ش�����ߵĵ㡣
        /// </remarks>
        /// <seealso cref="DJCvt_Open"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJCvt_DotLine(Int32 wChnl, byte[] cbDotStr, Int32 wDotSize, Int32 wDotFlag);

        /// <summary>
        /// ���ص�ǰҳ��ʣ���ɨ������Ŀ����ת�������У����Բ��ϵĵ��ñ�����������Ƿ�쵽һҳ�Ľ����ˡ�
        /// һ������£�����Բ�ͣ����FAX�ļ������µ����ݣ���һҳת����ϣ�ϵͳ���Զ��л�����һҳ�����ǣ������Ҫ��ÿһҳ����ҳü��ҳ�ţ�����Ҫʹ�ñ��������Ա㵱һҳ�����ʱ�����ʵ��Ĵ�����ο�FAXDEMO.CPP��Դ����
        /// </summary>
        /// <param name="wChnlNo">ת��ͨ����</param>
        /// <returns>��ǰҳ��ʣ���ɨ������Ŀ��</returns>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJCvt_LeftLine(Int32 wChnlNo);

        /// <summary>
        /// ��ָ����ת��ͨ������һ���ı���ϵͳ���ݵ����ļ�ת����FAX���ݡ�Ϊ�����ۣ�ϵͳ���Զ���TAB����\t������Ϊ4���ո�
        /// ����FontSize ��ʾ��ת��ʱʹ�õ���������С�������ֵΪ16��ʹ�õ���ASC16��8*16 ASCII�ַ�����HZK16��16*16���֣���������߼���5���ո������ֵΪ24��ʹ�õ���ASC24��12*24 ASCII�ַ�����HZK24��24*24���֣���������߼���3���ո�
        /// ��־DoubleBitFlag��ʾ�Ƿ�1������2���㣬��DoubleBitFlag=0ʱ��һ��������ˮƽ�����Ͻ�ռ��16����24���㣬һ��ASCII�ַ���ռ��8�����12���㣻��DoubleBitFlag=1ʱ��һ��������ˮƽ�����Ͻ�ռ��32�����48���㣬һ��ASCII�ַ���ռ��16�����24���㡣����һ��FAXɨ����Ϊ1728���㣬Ϊ�˺Ϻ�������һ�����DoubleBitFlag=1��
        /// ��־DoubleLineFlag��ʾ�Ƿ�1�������б��2�������С���DoubleLineFlag=0ʱ��һ���ı���ռ��17��ɨ���У������ڵײ���1����ɨ���У���25��ɨ���У���DoubleLineFlag=1ʱ��һ���ı���ռ��34��ɨ���У������ڵײ���2����ɨ���У���68��ɨ���С�
        /// </summary>
        /// <param name="wChnl">ת��ͨ����</param>
        /// <param name="cbTextStr">ת�����ı��������������ĺ�ASCII�ַ�</param>
        /// <param name="DoubleBitFlag">˫���־��0��1��</param>
        /// <param name="DoubleLineFlag">˫�б�־��0��1��</param>
        /// <param name="FontSize">ʹ�õ������С������Ϊ16��24��</param>
        /// <returns>
        ///  1   				�� �ɹ�
        /// XX 					�� ʧ��
        /// </returns>
        /// <remarks>
        /// ע�����
        ///     �����ڵİ汾�У�ֻ�к���DJCvt_TextLine�������Ժ�ʹ�ñ�������ת���ı��ļ���
        /// ����cbTextStr�ԡ�\0���������䳤�Ȳ��ó���180���ַ���
        /// </remarks>
        /// <seealso cref="DJCvt_TextLine"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJCvt_TextLineA(Int32 wChnl, byte[] cbTextStr, Int32 DoubleBitFlag, Int32 DoubleLineFlag, Int32 FontSize);

        /// <summary>
        ///  ��ָ����ת��ͨ������һ���ı���
        /// </summary>
        /// <param name="wChnl">ת��ͨ����</param>
        /// <param name="cbTextStr">ת�����ı��������������ĺ�ASCII�ַ�</param>
        /// <returns>
        ///  1   				�� �ɹ�
        /// XX 					�� ʧ��
        /// </returns>
        /// <remarks>
        /// �������൱�ڣ�
        /// if ( cbResolution == HIGH_RESOLUTION )			// �߷ֱ���
        /// 	return DJCvt_TextLineA ( wChnl, cbTextStr, 0, 0,16);	// ���㣬���У�16��������
        ///	else
        ///		return DJCvt_TextLineA ( wChnl, cbTextStr, 1, 0,16);	// ˫�㣬���У�16��������
        ///	����ĺ�����<seealso cref="DJCvt_ TextLineA"/>˵����
        /// </remarks>
        /// <seealso cref="DJCvt_TextLineA"/>
        /// <seealso cref="DJCvt_Open"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJCvt_TextLine(Int32 wChnl, byte[] cbTextStr);

        /// <summary>
        /// ת���ڰ׵�BMP�ļ���FAX�ļ���Ϊ�����ۣ�ϵͳ���Զ�������߼���80���հ׵㡣
        /// ��־DoubleBitFlag��ʾ�Ƿ�1������2���㡣����һ��FAXɨ����Ϊ1728���㣬Ϊ�˺Ϻ��������ڱ�׼�ֱ���ʱ��һ�����DoubleBitFlag=1�����磬����Ļ�ϵ�һ��BMPͼ���ļ�����СΪ800*600��ת����Ϻ���1600*600���ڱ�׼�ֱ����£��պúϺ�������ͬ��������ʹ�ø߷ֱ���ʱ��һ�����DoubleBitFlag=0��
        /// </summary>
        /// <param name="wChnl">ת��ͨ����</param>
        /// <param name="cbBmpFileName"> Ҫת���ĺڰ�BMP�ļ���</param>
        /// <param name="DoubleBitFlag">˫���־��0��1��</param>
        /// <returns>
        ///  1  				�� �ɹ�
        /// -1					�� ��BMP�ļ�ʧ�ܻ�ת��ͨ��û�д�
        /// -2					�� ʧ�ܣ���BMP�ļ����Ǻڰ׵�BMP�ļ�
        /// </returns>
        /// <remarks>
        /// ע�����
        ///     һ��Ҫ��֤BMP�ļ��ǵ�ɫ���ڰף���BMP��ʽ��ͼ���ļ���
        /// </remarks>
        /// <seealso cref="DJCvt_BmpFile"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJCvt_BmpFileA(Int32 wChnl, byte[] cbBmpFileName, Int32 DoubleBitFlag);

        /// <summary>
        /// ת���ڰ׵�BMP�ļ���FAX�ļ����������൱�ڣ�
        ///   DJCvt_BmpFileA(wChnl, cbBmpFileName, 1);
        /// ����ĺ�����<seealso cref="DJCvt_BmpFileA"/>˵����
        /// </summary>
        /// <param name="wChnl">ת��ͨ����</param>
        /// <param name="cbBmpFileName">Ҫת���ĺڰ�BMP�ļ���</param>
        /// <returns>
        ///  1    				�� �ɹ�
        /// -1 					�� ��BMP�ļ�ʧ�ܻ�ת��ͨ��û�д�
        /// -2					�� ʧ�ܣ���BMP�ļ����Ǻڰ׵�BMP�ļ�
        /// </returns>
        /// <seealso cref="DJCvt_BmpFileA"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJCvt_BmpFile(Int32 wChnl, byte[] cbBmpFileName);

        /// <summary>
        /// ��Bfx�ļ�ת����Tiff�ļ�
        /// </summary>
        /// <param name="Bfxfilename">bfx�ļ���ָ��</param>
        /// <param name="Tifffilename">TIF�ļ���ָ��</param>
        /// <returns>
        ///  1                  �� �ɹ���ɣ�
        /// -1                  �� ��bfxfilename ʧ��
        /// -2                  �� ��tifffilename ʧ��
        /// -3                  �� ת��ʧ��
        /// </returns>
        /// <seealso cref="DJCvt_Tiff2Bfx"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJCvt_Bfx2Tiff(byte[] Bfxfilename, byte[] Tifffilename);

        /// <summary>
        /// ��TIF�ļ�ת����bfx�ļ�
        /// </summary>
        /// <param name="Tifffilename">TIF�ļ���ָ��</param>
        /// <param name="Bfxfilename">bfx�ļ���ָ��</param>
        /// <returns>
        ///  1                   ��   ת���ɹ���
        /// -1                   ��   ��TIF�ļ�Tifffilenameʧ�ܣ�
        /// -2                   ��  ��bfx�ļ�Bfxfilenameʧ�ܣ�
        /// -3                   ��  ת��ʧ�ܡ�
        /// </returns>
        /// <seealso cref="DJCvt_Bfx2Tiff"/>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJCvt_Tiff2Bfx(byte[] Tifffilename, byte[] Bfxfilename);

        /// <summary>
        /// ��һ��BFX��ʽ���ļ�ת����һ������BMP��ʽ��λͼ�ļ�
        /// </summary>
        /// <param name="Bfxfilename"> Ҫת��BFX��ʽ���ļ���</param>
        /// <param name="Bmpfilename">ת�������ɵ�BMP��ʽ���ļ���</param>
        /// <param name="PageMode"> 0��ת��������һ��BMP��ʽ���ļ�;1��ת����ÿҳ����һ��BMP��ʽ���ļ�.�ļ���Bmpfilename%</param>
        /// <param name="RotateMode">0��ת����λͼֱ�Ӵ洢����.1��ת����λͼ��ת180�ȴ洢����</param>
        /// <returns>
        ///  1                   �� �ļ�ת���ɹ�
        /// -1                   �� ��������,ҳģʽ����Ϊ1��0
        /// -2                   �� ��������,��תģʽ����Ϊ1��0 
        /// -3                   �� �ڴ����ʧ��
        /// -4                   �� ��bfx�ļ�����
        /// -5                   �� ����bfx��ʽ���ļ�
        /// -6                   ��  bfxҳ����
        /// -7                   ��  ��bmp�ļ�ʧ��
        /// </returns>
        /// <remarks>
        /// λͼ���ݼ�¼��λͼ��ÿһ������ֵ����¼˳������ɨ�������Ǵ�����,ɨ����֮���Ǵ��µ���.�����������ת�洢����,��ôת�������ɵ�ͼ����ԭ����ͼ���������෴�ġ�
        /// </remarks>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32  DJCvt_Bfx2Bmp(byte[] Bfxfilename, byte[] Bmpfilename, Int32 PageMode, Int32 RotateMode);

        /// <summary>
        /// ��ʼ��ĳһͨ�����д����ź����ļ��
        /// </summary>
        /// <param name="wChnlNo">ͨ����</param>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern void DJFax_StartCheckFaxTone(Int32 wChnlNo); 

        /// <summary>
        /// �鿴��ǰĳһͨ�����Ƿ��⵽�˴����ź���
        /// </summary>
        /// <param name="wChnlNo">ͨ����</param>
        /// <returns>
        /// 0 û�м�⵽�����ź���
        /// 1 ��⵽�˴����ź���
        /// </returns>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 DJFax_FaxToneCheckResult(Int32 wChnlNo);

        /// <summary>
        /// ���ָ��ͨ���Ĳ��Ų��衣
        /// </summary>
        /// <param name="wChNo">ͨ����</param>
        /// <returns>NewSig.dll�У������Ź��̷ֳ���8�����裬���ݲ��������Ĳ�ͬ�׶Σ�����ֵ������0��7�е�һ����</returns>
        [DllImport("Tc08a32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 Sig_GetDialStep(Int32 wChNo);

        #endregion

    }
}
