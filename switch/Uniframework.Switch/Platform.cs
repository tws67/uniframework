using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch
{
    #region Switch Status

    /// <summary>
    /// Uniframework ���õ�״̬ö������
    /// </summary>
    public enum SwitchStatus
    {
        /// <summary>
        /// �����ɹ�
        /// </summary>
        SUCCESS,  // - General Success (common return value for most functions)
        /// <summary>
        /// ����ʧ��
        /// </summary>
        FAILURE,  // - General Falsehood
        /// <summary>
        /// ����ʱ
        /// </summary>
        TIMEOUT,  // - A Timeout has occured
        /// <summary>
        /// ��ʶ��Ҫ���������ղŵĲ���
        /// </summary>
        RESTART,  // - An indication to restart the previous operation
        /// <summary>
        /// ��ʶ������ֹ
        /// </summary>
        TERM,     // - An indication to terminate
        /// <summary>
        /// ��Ҫ����Դδʵ��
        /// </summary>
        NOTIMPL,  // - An indication that requested resource is not impelemented
        /// <summary>
        /// �ڴ����
        /// </summary>
        MEMERR,   // - General memory error
        /// <summary>
        /// 
        /// </summary>
        NOOP,     // - NOTHING
        /// <summary>
        /// 
        /// </summary>
        RESAMPLE, // - An indication that a resample has occured
        /// <summary>
        /// һ���Դ���
        /// </summary>
        GENERR,	  // - A general Error
        /// <summary>
        /// ��Ҫ����Դ��ռ��
        /// </summary>
        INUSE,    // - An indication that requested resource is in use
        /// <summary>
        /// �������ж�
        /// </summary>
        BREAK,    // - A non-fatal break of an operation
        /// <summary>
        /// ����Socket����
        /// </summary>
        SOCKERR,  // - A socket error
        /// <summary>
        /// ��Ҫ���������
        /// </summary>
        MOREDATA, // - Need More Data
        /// <summary>
        /// δ�ҵ�
        /// </summary>
        NOTFOUND, // - Not Found
        /// <summary>
        /// δ����
        /// </summary>
        UNLOAD    // - Unload
    }
    #endregion

    #region Switch Call Status

    /// <summary>
    /// ���н��״̬
    /// </summary>
    public enum CallStatus
    { 
        /// <summary>
        /// δ���
        /// </summary>
        UNALLOCATED = 0,
        /// <summary>
        /// �ɹ�
        /// </summary>
        SUCCESS = 1,
        /// <summary>
        /// ����·�ɲ��ɴ�
        /// </summary>
        NO_ROUTE_TRANSIT_NET = 2,
        /// <summary>
        /// Ŀ��·�ɲ��ɴ�
        /// </summary>
        NO_ROUTE_DESTINATION = 3,
        /// <summary>
        /// ͨ��������
        /// </summary>
        CHANNEL_UNACCEPTABLE = 6,
        /// <summary>
        /// ���в�����
        /// </summary>
        CALL_AWARDED_DELIVERED = 7,
        /// <summary>
        /// ��������
        /// </summary>
        NORMAL_CLEARING = 16,
        /// <summary>
        /// �û�/�Է�æ
        /// </summary>
        USER_BUSY = 17,
        /// <summary>
        /// ��·��Ӧ��
        /// </summary>
        NO_USER_RESPONSE = 18,
        /// <summary>
        /// �޲�����
        /// </summary>
        NO_ANSWER = 19,
        /// <summary>
        /// �����ڻ����ԡ����˽���
        /// </summary>
        SUBSCRIBER_ABSENT = 20,
        /// <summary>
        /// ���б��ܾ�
        /// </summary>
        CALL_REJECTED = 21,
        /// <summary>
        /// �����Ѿ��ı�
        /// </summary>
        NUMBER_CHANGED = 22,
        /// <summary>
        /// ����ת�ƣ����б��ض����µ�Ŀ��
        /// </summary>
        REDIRECTION_TO_NEW_DESTINATION = 23,
        /// <summary>
        /// ·��ѡ�����
        /// </summary>
        EXCHANGE_ROUTING_ERROR = 25,
        /// <summary>
        /// ����Ŀ�����
        /// </summary>
        DESTINATION_OUT_OF_ORDER = 27,
        /// <summary>
        /// ������Ч
        /// </summary>
        INVALID_NUMBER_FORMAT = 28,
        /// <summary>
        /// ��ʩ���ܾ�
        /// </summary>
        FACILITY_REJECTED = 29,
        /// <summary>
        /// ����
        /// </summary>
        RESPONSE_TO_STATUS_ENQUIRY = 30,
        /// <summary>
        /// 
        /// </summary>
        NORMAL_UNSPECIFIED = 31,
        /// <summary>
        /// ��·ӵ��
        /// </summary>
        NORMAL_CIRCUIT_CONGESTION = 34,
        /// <summary>
        /// �������
        /// </summary>
        NETWORK_OUT_OF_ORDER = 38,
        /// <summary>
        /// ��ʱʧЧ
        /// </summary>
        NORMAL_TEMPORARY_FAILURE = 41,
        /// <summary>
        /// ������ӵ��
        /// </summary>
        SWITCH_CONGESTION = 42,
        /// <summary>
        /// ������Ϣ��ʧ
        /// </summary>
        ACCESS_INFO_DISCARDED = 43,
        /// <summary>
        /// �������ͨ��������
        /// </summary>
        REQUESTED_CHAN_UNAVAIL = 44,
        /// <summary>
        /// 
        /// </summary>
        PRE_EMPTED = 45,
        /// <summary>
        /// ��ʩδԤ��
        /// </summary>
        FACILITY_NOT_SUBSCRIBED = 50,
        /// <summary>
        /// ��������ֹ
        /// </summary>
        OUTGOING_CALL_BARRED = 52,
        /// <summary>
        /// ���뱻��ֹ
        /// </summary>
        INCOMING_CALL_BARRED = 54,
        /// <summary>
        /// ���������δ����֤
        /// </summary>
        BEARERCAPABILITY_NOTAUTH = 57,
        /// <summary>
        /// �����������Ч
        /// </summary>
        BEARERCAPABILITY_NOTAVAIL = 58,
        /// <summary>
        /// ������Ч
        /// </summary>
        SERVICE_UNAVAILABLE = 63,
        /// <summary>
        /// ���������δʵ��
        /// </summary>
        BEARERCAPABILITY_NOTIMPL = 65,
        /// <summary>
        /// ͨ��δʵ��
        /// </summary>
        CHAN_NOT_IMPLEMENTED = 66,
        /// <summary>
        /// ��ʩδʵ��
        /// </summary>
        FACILITY_NOT_IMPLEMENTED = 69,
        /// <summary>
        /// ����δʵ��
        /// </summary>
        SERVICE_NOT_IMPLEMENTED = 79,
        /// <summary>
        /// ��Ч�ĺ�������
        /// </summary>
        INVALID_CALL_REFERENCE = 81,
        /// <summary>
        /// Ŀ�겻����
        /// </summary>
        INCOMPATIBLE_DESTINATION = 88,
        /// <summary>
        /// ��Ϣ��Ч����Ϣû����ϸָ��
        /// </summary>
        INVALID_MSG_UNSPECIFIED = 95,
        /// <summary>
        /// �������
        /// </summary>
        MANDATORY_IE_MISSING = 96,
        /// <summary>
        /// ��Ϣ���Ͳ�����
        /// </summary>
        MESSAGE_TYPE_NONEXIST = 97,
        /// <summary>
        /// ��Ϣ����
        /// </summary>
        WRONG_MESSAGE = 98,
        /// <summary>
        /// 
        /// </summary>
        IE_NONEXIST = 99,
        /// <summary>
        /// 
        /// </summary>
        INVALID_IE_CONTENTS = 100,
        /// <summary>
        /// ����ĺ���״̬
        /// </summary>
        WRONG_CALL_STATE = 101,
        /// <summary>
        /// �ָ���ʱ
        /// </summary>
        RECOVERY_ON_TIMER_EXPIRE = 102,
        /// <summary>
        /// 
        /// </summary>
        MANDATORY_IE_LENGTH_ERROR = 103,
        /// <summary>
        /// Э�����
        /// </summary>
        PROTOCOL_ERROR = 111,
        /// <summary>
        /// ��������
        /// </summary>
        INTERWORKING = 127,
        /// <summary>
        /// ����ȡ��
        /// </summary>
        ORIGINATOR_CANCEL = 487,
        /// <summary>
        /// ϵͳ����
        /// </summary>
        CRASH = 500,
        /// <summary>
        /// ϵͳֹͣ����
        /// </summary>
        SYSTEM_SHUTDOWN = 501,
        /// <summary>
        /// Ƶ�ʲ�ƥ��
        /// </summary>
        LOSE_RACE = 502,
        /// <summary>
        /// ��Ҫ����Ա��Ԥ
        /// </summary>
        MANAGER_REQUEST = 503,
        /// <summary>
        /// ת����Ч
        /// </summary>
        BLIND_TRANSFER = 600,
        /// <summary>
        /// ����ת��
        /// </summary>
        ATTENDED_TRANSFER = 601,
        /// <summary>
        /// ���䳬ʱ
        /// </summary>
        ALLOTTED_TIMEOUT = 602
    }

    #endregion

    #region Channel flags
    /// <summary>
    /// ͨ��״̬��־
    /// </summary>
    [Flags]
    public enum ChannelFlag
    {
        /// <summary>
        /// Channel is answered
        /// </summary>
        ANSERED = (1 << 0),
        /// <summary>
        /// Channel is an outbound channel
        /// </summary>
        OUTBOUND = (1 << 1),
        /// <summary>
        /// Channel is ready for audio before answer 
        /// </summary>
        EARLYMEDIA = (1 << 2),
        /// <summary>
        /// Channel is an originator
        /// </summary>
        ORIGINATOR = (1 << 3),
        /// <summary>
        /// Channel is being transfered
        /// </summary>
        TRANSFER = (1 << 4),
        /// <summary>
        /// Channel will accept CNG frames
        /// </summary>
        ACCEPTCNG = (1 << 5),
        /// <summary>
        /// Channel wants you to wait
        /// </summary>
        WAITFORME = (1 << 6),
        /// <summary>
        /// Channel in a bridge
        /// </summary>
        BRIDGED = (1 << 7),
        /// <summary>
        /// Channel is on hold
        /// </summary>
        HOLD = (1 << 8),
        /// <summary>
        /// Channel has a service thread
        /// </summary>
        SERVICE = (1 << 9),
        /// <summary>
        /// Channel is tagged
        /// </summary>
        TAGGED = (1 << 10),
        /// <summary>
        /// Channel is the winner
        /// </summary>
        WINNER = (1 << 11),
        /// <summary>
        /// Channel is under control
        /// </summary>
        CONTROLLED = (1 << 12),
        /// <summary>
        /// Channel has no media
        /// </summary>
        BYPASSMEDIA = (1 << 13),
        /// <summary>
        /// Suspend I/O
        /// </summary>
        SUPEND = (1 << 14),
        /// <summary>
        /// Suspend control events
        /// </summary>
        EVENTPARSE = (1 << 15),
        /// <summary>
        /// Tell the state machine to repeat a state
        /// </summary>
        REPEATSTATE = (1 << 16),
        /// <summary>
        /// Channel is generating it's own ringback
        /// </summary>
        GENRINGBACK = (1 << 17),
        /// <summary>
        /// Channel is ready to send ringback
        /// </summary>
        RINGREADY = (1 << 18),
        /// <summary>
        /// Channel should stop what it's doing
        /// </summary>
        BREAK = (1 << 19),
        /// <summary>
        /// Channel is broadcasting
        /// </summary>
        BROADCAST = (1 << 20),
        /// <summary>
        /// Channel has a unicast connection
        /// </summary>
        UNICAST = (1 << 21),
        /// <summary>
        /// Channel has video
        /// </summary>
        VIDEO = (1 << 22)
    }

    #endregion

    #region Channel State

    /// <summary>
    /// ͨ��״̬
    /// </summary>
    public enum ChannelState
    {
        /// <summary>
        /// ͨ���մ���
        /// </summary>
        NEW,
        /// <summary>
        /// ��ɳ�ʼ������
        /// </summary>
        INIT,
        /// <summary>
        /// �������岢�ڲ�ѯ�Ƿ����IVR���̻�ű�����
        /// </summary>
        RING,
        /// <summary>
        /// ͨ�����ں���ת��״̬
        /// </summary>
        TRANSFER,
        /// <summary>
        /// ͨ������ִ��IVR��ű�����
        /// </summary>
        EXECUTE,
        /// <summary>
        /// ͨ�����ڽ����յ�����Ϣԭ�ⲻ���ķ��ظ�������
        /// </summary>
        LOOPBACK,
        /// <summary>
        /// ͨ�����Է��Ѿ��һ�
        /// </summary>
        HANGUP,
        /// <summary>
        /// ͨ�������ͷţ����״̬�������й���׼���˳�
        /// </summary>
        DONE
    }

    #endregion

    #region Channel Status

    /// <summary>
    /// ͨ��״̬
    /// </summary>
    public enum ChannelStatus
    {
        /// <summary>
        /// ͨ���ձ�����
        /// </summary>
        NEW = 0,
        /// <summary>
        /// ͨ����ʼ�����
        /// </summary>
        INIT,
        /// <summary>
        /// ͨ������
        /// </summary>
        IDLE,
        /// <summary>
        /// ����
        /// </summary>
        RING,
        /// <summary>
        /// ռ��
        /// </summary>
        SEIZE,
        /// <summary>
        /// ժ��
        /// </summary>
        PICKUP,
        /// <summary>
        /// ��������
        /// </summary>
        INPUT,
        /// <summary>
        /// ��ȡ
        /// </summary>
        READ,
        /// <summary>
        /// ��ȡDtmf�롢ȡ��/���к���ȵײ����
        /// </summary>
        COLLECT,
        /// <summary>
        /// �Ų�����
        /// </summary>
        TONE,
        /// <summary>
        /// ��Dtmf��
        /// </summary>
        DTMF,
        /// <summary>
        /// ����
        /// </summary>
        PLAY,
        /// <summary>
        /// ¼��
        /// </summary>
        RECORD,
        /// <summary>
        /// ����
        /// </summary>
        JOIN,
        /// <summary>
        /// ����
        /// </summary>
        FAX,
        /// <summary>
        /// �ȴ�
        /// </summary>
        WAITING,
        /// <summary>
        /// ������
        /// </summary>
        CALLING,
        /// <summary>
        /// ����
        /// </summary>
        CONNECT,
        /// <summary>
        /// ��������
        /// </summary>
        RECONNECT,
        /// <summary>
        /// Ѱ��
        /// </summary>
        HUNTING,
        /// <summary>
        /// ����
        /// </summary>
        DIAL,
        /// <summary>
        /// ת�ӡ�����
        /// </summary>
        TRANSFER,
        /// <summary>
        /// ����
        /// </summary>
        HOLD,
        /// <summary>
        /// ִ���Զ������
        /// </summary>
        EXECUTE,
        /// <summary>
        /// ѭ��
        /// </summary>
        LOOPBACK,
        /// <summary>
        /// ���ߡ�����
        /// </summary>
        SLEEP,
        /// <summary>
        /// �һ�������һ�λỰ����սᣩ
        /// </summary>
        HANGUP,
        /// <summary>
        /// �˳��ڲ�״̬��
        /// </summary>
        DONE,
        /// <summary>
        /// ͨ�����ã��ͷŻỰռ�õ���Դ����Idle״̬
        /// </summary>
        RESET,
        /// <summary>
        /// ���ͨ����Դ
        /// </summary>
        RELEASE,
        /// <summary>
        /// ���ͨ���������ս�״̬
        /// </summary>
        FINAL
    }

    #endregion

    #region Channel Type

    /// <summary>
    /// ͨ������
    /// </summary>
    public enum ChannelType
    {
        /// <summary>
        /// ����ͨ��
        /// </summary>
        USER,

        /// <summary>
        /// ����ͨ��
        /// </summary>
        TRUNK,

        /// <summary>
        /// ͨ������
        /// </summary>
        EMPTY,

        /// <summary>
        /// ¼��ͨ��
        /// </summary>
        RECORD,

        /// <summary>
        /// ����ͨ��
        /// </summary>
        VIRTUAL
    }

    #endregion

    #region Call session type

    /// <summary>
    /// �Ự����
    /// </summary>
    public enum CallSessionType
    {
        /// <summary>
        /// �Ự����
        /// </summary>
        NONE,
        /// <summary>
        /// ����
        /// </summary>
        INCOMING,
        /// <summary>
        /// ����
        /// </summary>
        OUTGOING,
        /// <summary>
        /// ժ����׼��������������ͨ��
        /// </summary>
        PICKUP,
        /// <summary>
        /// ���߻����߽��к���ת��
        /// </summary>
        TRANSFER,
        /// <summary>
        /// �غ�
        /// </summary>
        RECCALL,
        /// <summary>
        /// ·��
        /// </summary>
        DIRECT,
        /// <summary>
        /// ���⡢�Զ���
        /// </summary>
        VIRTUAL
    }

    #endregion

    #region Call session message type
    /// <summary>
    /// �Ự��Ϣ����
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// Indication to redirect audio to another location if possible
        /// </summary>
	    REDIRECT_AUDIO,
        /// <summary>
        /// A text message
        /// </summary>
	    TRANSMIT_TEXT,
        /// <summary>
        /// indicate answer
        /// </summary>
	    INDICATE_ANSWER,
        /// <summary>
        /// indicate progress 
        /// </summary>
	    INDICATE_PROGRESS,
        /// <summary>
        /// indicate a bridge starting
        /// </summary>
	    INDICATE_BRIDGE,
        /// <summary>
        /// indicate a bridge ending
        /// </summary>
	    INDICATE_UNBRIDGE,
        /// <summary>
        /// indicate a transfer is taking place
        /// </summary>
	    INDICATE_TRANSFER,
        /// <summary>
        /// indicate media is required
        /// </summary>
	    INDICATE_MEDIA,
        /// <summary>
        /// indicate no-media is required
        /// </summary>
	    INDICATE_NOMEDIA,
        /// <summary>
        /// indicate hold
        /// </summary>
	    INDICATE_HOLD,
        /// <summary>
        /// indicate unhold
        /// </summary>
	    INDICATE_UNHOLD,
        /// <summary>
        /// indicate redirect
        /// </summary>
	    INDICATE_REDIRECT,
        /// <summary>
        /// indicate reject
        /// </summary>
	    INDICATE_REJECT,
        /// <summary>
        /// indicate media broadcast
        /// </summary>
	    INDICATE_BROADCAST
    }

    #endregion

    #region TTS Play Type

    /// <summary>
    /// TTS��������
    /// </summary>
    public enum TTSPlayType
    {
        /// <summary>
        /// ����
        /// </summary>
        NUMBER,
        /// <summary>
        /// ��Ŀ
        /// </summary>
        ITEMS,
        /// <summary>
        /// ����
        /// </summary>
        PERSONS,
        /// <summary>
        /// ��Ϣ
        /// </summary>
        MESSAGES,
        /// <summary>
        /// ���
        /// </summary>
        CURRENCY,
        /// <summary>
        /// ʱ�����
        /// </summary>
        TIME_MEASUREMENT,
        /// <summary>
        /// ��ǰ����
        /// </summary>
        CURRENT_DATE,
        /// <summary>
        /// ��ǰʱ��
        /// </summary>
        CURRENT_TIME,
        /// <summary>
        /// ��ǰ����ʱ��
        /// </summary>
        CURRENT_DATE_TIME,
        /// <summary>
        /// �绰����
        /// </summary>
        TELEPHONE_NUMBER,
        /// <summary>
        /// �绰������չ
        /// </summary>
        TELEPHONE_EXTENSION,
        /// <summary>
        /// URL
        /// </summary>
        URL,
        /// <summary>
        /// IP��ַ
        /// </summary>
        IP_ADDRESS,
        /// <summary>
        /// Email
        /// </summary>
        EMAIL_ADDRESS,
        /// <summary>
        /// ��������
        /// </summary>
        POSTAL_ADDRESS,
        /// <summary>
        /// �ʺ�
        /// </summary>
        ACCOUNT_NUMBER,
        /// <summary>
        /// ����ƴд
        /// </summary>
        NAME_SPELLED,
        /// <summary>
        /// ��������
        /// </summary>
        NAME_PHONETIC
    }

    #endregion

    #region Conference Type

    /// <summary>
    /// ͨ����������ģʽ
    /// </summary>
    public enum ConferenceType
    {
        /// <summary>
        /// δ֪
        /// </summary>
        UNKOWN,
        /// <summary>
        /// ���룬������Ҳ����˵
        /// </summary>
        JOIN,
        /// <summary>
        /// ������ֻ����˵
        /// </summary>
        LISTEN
    }

    #endregion

    #region Fax Mode

    /// <summary>
    /// ��������
    /// </summary>
    public enum FaxMode
    {
        /// <summary>
        /// δ֪
        /// </summary>
        UNKNOWN,
        /// <summary>
        /// ����
        /// </summary>
        SEND,
        /// <summary>
        /// ����
        /// </summary>
        RECEIVE
    }

    #endregion

    #region Voice Resource

    /// <summary>
    /// ������ʹ�õ���������Դ����
    /// </summary>
    public enum VoiceResource
    {
        /// <summary>
        /// δ֪
        /// </summary>
        UNKNOWN,

        /// <summary>
        /// ��ͨ��
        /// </summary>
        STANDARD,

        /// <summary>
        /// ����
        /// </summary>
        LOCALIZE,

        /// <summary>
        /// Ӣ��
        /// </summary>
        ENGLISH
    }

    #endregion
}