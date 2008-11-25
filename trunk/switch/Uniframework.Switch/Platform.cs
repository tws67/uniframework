using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch
{
    #region Switch Status

    /// <summary>
    /// Uniframework 能用的状态枚举类型
    /// </summary>
    public enum SwitchStatus
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        SUCCESS,  // - General Success (common return value for most functions)
        /// <summary>
        /// 操作失败
        /// </summary>
        FAILURE,  // - General Falsehood
        /// <summary>
        /// 处理超时
        /// </summary>
        TIMEOUT,  // - A Timeout has occured
        /// <summary>
        /// 标识需要重新启动刚才的操作
        /// </summary>
        RESTART,  // - An indication to restart the previous operation
        /// <summary>
        /// 标识操作终止
        /// </summary>
        TERM,     // - An indication to terminate
        /// <summary>
        /// 需要的资源未实现
        /// </summary>
        NOTIMPL,  // - An indication that requested resource is not impelemented
        /// <summary>
        /// 内存错误
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
        /// 一般性错误
        /// </summary>
        GENERR,	  // - A general Error
        /// <summary>
        /// 需要的资源被占用
        /// </summary>
        INUSE,    // - An indication that requested resource is in use
        /// <summary>
        /// 操作被中断
        /// </summary>
        BREAK,    // - A non-fatal break of an operation
        /// <summary>
        /// 网络Socket错误
        /// </summary>
        SOCKERR,  // - A socket error
        /// <summary>
        /// 需要更多的数据
        /// </summary>
        MOREDATA, // - Need More Data
        /// <summary>
        /// 未找到
        /// </summary>
        NOTFOUND, // - Not Found
        /// <summary>
        /// 未加载
        /// </summary>
        UNLOAD    // - Unload
    }
    #endregion

    #region Switch Call Status

    /// <summary>
    /// 呼叫结果状态
    /// </summary>
    public enum CallStatus
    { 
        /// <summary>
        /// 未完成
        /// </summary>
        UNALLOCATED = 0,
        /// <summary>
        /// 成功
        /// </summary>
        SUCCESS = 1,
        /// <summary>
        /// 网络路由不可达
        /// </summary>
        NO_ROUTE_TRANSIT_NET = 2,
        /// <summary>
        /// 目标路由不可达
        /// </summary>
        NO_ROUTE_DESTINATION = 3,
        /// <summary>
        /// 通道不可用
        /// </summary>
        CHANNEL_UNACCEPTABLE = 6,
        /// <summary>
        /// 呼叫不可用
        /// </summary>
        CALL_AWARDED_DELIVERED = 7,
        /// <summary>
        /// 正常进行
        /// </summary>
        NORMAL_CLEARING = 16,
        /// <summary>
        /// 用户/对方忙
        /// </summary>
        USER_BUSY = 17,
        /// <summary>
        /// 线路无应答
        /// </summary>
        NO_USER_RESPONSE = 18,
        /// <summary>
        /// 无拨号音
        /// </summary>
        NO_ANSWER = 19,
        /// <summary>
        /// 无人在话机旁、无人接听
        /// </summary>
        SUBSCRIBER_ABSENT = 20,
        /// <summary>
        /// 呼叫被拒绝
        /// </summary>
        CALL_REJECTED = 21,
        /// <summary>
        /// 号码已经改变
        /// </summary>
        NUMBER_CHANGED = 22,
        /// <summary>
        /// 呼叫转移，呼叫被重定向到新的目标
        /// </summary>
        REDIRECTION_TO_NEW_DESTINATION = 23,
        /// <summary>
        /// 路由选择错误
        /// </summary>
        EXCHANGE_ROUTING_ERROR = 25,
        /// <summary>
        /// 呼叫目标故障
        /// </summary>
        DESTINATION_OUT_OF_ORDER = 27,
        /// <summary>
        /// 号码无效
        /// </summary>
        INVALID_NUMBER_FORMAT = 28,
        /// <summary>
        /// 设施被拒绝
        /// </summary>
        FACILITY_REJECTED = 29,
        /// <summary>
        /// 握手
        /// </summary>
        RESPONSE_TO_STATUS_ENQUIRY = 30,
        /// <summary>
        /// 
        /// </summary>
        NORMAL_UNSPECIFIED = 31,
        /// <summary>
        /// 线路拥挤
        /// </summary>
        NORMAL_CIRCUIT_CONGESTION = 34,
        /// <summary>
        /// 网络故障
        /// </summary>
        NETWORK_OUT_OF_ORDER = 38,
        /// <summary>
        /// 暂时失效
        /// </summary>
        NORMAL_TEMPORARY_FAILURE = 41,
        /// <summary>
        /// 交换机拥挤
        /// </summary>
        SWITCH_CONGESTION = 42,
        /// <summary>
        /// 访问信息丢失
        /// </summary>
        ACCESS_INFO_DISCARDED = 43,
        /// <summary>
        /// 被请求的通道不可用
        /// </summary>
        REQUESTED_CHAN_UNAVAIL = 44,
        /// <summary>
        /// 
        /// </summary>
        PRE_EMPTED = 45,
        /// <summary>
        /// 设施未预订
        /// </summary>
        FACILITY_NOT_SUBSCRIBED = 50,
        /// <summary>
        /// 呼出被禁止
        /// </summary>
        OUTGOING_CALL_BARRED = 52,
        /// <summary>
        /// 呼入被禁止
        /// </summary>
        INCOMING_CALL_BARRED = 54,
        /// <summary>
        /// 请求的能力未被验证
        /// </summary>
        BEARERCAPABILITY_NOTAUTH = 57,
        /// <summary>
        /// 请求的能力无效
        /// </summary>
        BEARERCAPABILITY_NOTAVAIL = 58,
        /// <summary>
        /// 服务无效
        /// </summary>
        SERVICE_UNAVAILABLE = 63,
        /// <summary>
        /// 请求的能力未实现
        /// </summary>
        BEARERCAPABILITY_NOTIMPL = 65,
        /// <summary>
        /// 通道未实现
        /// </summary>
        CHAN_NOT_IMPLEMENTED = 66,
        /// <summary>
        /// 设施未实现
        /// </summary>
        FACILITY_NOT_IMPLEMENTED = 69,
        /// <summary>
        /// 服务未实现
        /// </summary>
        SERVICE_NOT_IMPLEMENTED = 79,
        /// <summary>
        /// 无效的呼叫引用
        /// </summary>
        INVALID_CALL_REFERENCE = 81,
        /// <summary>
        /// 目标不兼容
        /// </summary>
        INCOMPATIBLE_DESTINATION = 88,
        /// <summary>
        /// 消息无效，消息没有详细指定
        /// </summary>
        INVALID_MSG_UNSPECIFIED = 95,
        /// <summary>
        /// 命令不完整
        /// </summary>
        MANDATORY_IE_MISSING = 96,
        /// <summary>
        /// 消息类型不存在
        /// </summary>
        MESSAGE_TYPE_NONEXIST = 97,
        /// <summary>
        /// 消息错误
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
        /// 错误的呼叫状态
        /// </summary>
        WRONG_CALL_STATE = 101,
        /// <summary>
        /// 恢复超时
        /// </summary>
        RECOVERY_ON_TIMER_EXPIRE = 102,
        /// <summary>
        /// 
        /// </summary>
        MANDATORY_IE_LENGTH_ERROR = 103,
        /// <summary>
        /// 协议错误
        /// </summary>
        PROTOCOL_ERROR = 111,
        /// <summary>
        /// 交互工作
        /// </summary>
        INTERWORKING = 127,
        /// <summary>
        /// 主叫取消
        /// </summary>
        ORIGINATOR_CANCEL = 487,
        /// <summary>
        /// 系统崩溃
        /// </summary>
        CRASH = 500,
        /// <summary>
        /// 系统停止运行
        /// </summary>
        SYSTEM_SHUTDOWN = 501,
        /// <summary>
        /// 频率不匹配
        /// </summary>
        LOSE_RACE = 502,
        /// <summary>
        /// 需要管理员干预
        /// </summary>
        MANAGER_REQUEST = 503,
        /// <summary>
        /// 转移无效
        /// </summary>
        BLIND_TRANSFER = 600,
        /// <summary>
        /// 正在转移
        /// </summary>
        ATTENDED_TRANSFER = 601,
        /// <summary>
        /// 分配超时
        /// </summary>
        ALLOTTED_TIMEOUT = 602
    }

    #endregion

    #region Channel flags
    /// <summary>
    /// 通道状态标志
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
    /// 通道状态
    /// </summary>
    public enum ChannelState
    {
        /// <summary>
        /// 通道刚创建
        /// </summary>
        NEW,
        /// <summary>
        /// 完成初始化工作
        /// </summary>
        INIT,
        /// <summary>
        /// 正在振铃并在查询是否存在IVR流程或脚本程序
        /// </summary>
        RING,
        /// <summary>
        /// 通道处于呼叫转移状态
        /// </summary>
        TRANSFER,
        /// <summary>
        /// 通道正在执行IVR或脚本程序
        /// </summary>
        EXECUTE,
        /// <summary>
        /// 通道正在将接收到的消息原封不动的发回给发送者
        /// </summary>
        LOOPBACK,
        /// <summary>
        /// 通道所对方已经挂机
        /// </summary>
        HANGUP,
        /// <summary>
        /// 通道可以释放，完成状态机的所有工作准备退出
        /// </summary>
        DONE
    }

    #endregion

    #region Channel Status

    /// <summary>
    /// 通道状态
    /// </summary>
    public enum ChannelStatus
    {
        /// <summary>
        /// 通道刚被创建
        /// </summary>
        NEW = 0,
        /// <summary>
        /// 通道初始化完成
        /// </summary>
        INIT,
        /// <summary>
        /// 通道空闲
        /// </summary>
        IDLE,
        /// <summary>
        /// 振铃
        /// </summary>
        RING,
        /// <summary>
        /// 占用
        /// </summary>
        SEIZE,
        /// <summary>
        /// 摘机
        /// </summary>
        PICKUP,
        /// <summary>
        /// 接受输入
        /// </summary>
        INPUT,
        /// <summary>
        /// 读取
        /// </summary>
        READ,
        /// <summary>
        /// 收取Dtmf码、取主/被叫号码等底层操作
        /// </summary>
        COLLECT,
        /// <summary>
        /// 放播号音
        /// </summary>
        TONE,
        /// <summary>
        /// 收Dtmf码
        /// </summary>
        DTMF,
        /// <summary>
        /// 放音
        /// </summary>
        PLAY,
        /// <summary>
        /// 录音
        /// </summary>
        RECORD,
        /// <summary>
        /// 加入
        /// </summary>
        JOIN,
        /// <summary>
        /// 传真
        /// </summary>
        FAX,
        /// <summary>
        /// 等待
        /// </summary>
        WAITING,
        /// <summary>
        /// 呼叫中
        /// </summary>
        CALLING,
        /// <summary>
        /// 连接
        /// </summary>
        CONNECT,
        /// <summary>
        /// 重新连接
        /// </summary>
        RECONNECT,
        /// <summary>
        /// 寻线
        /// </summary>
        HUNTING,
        /// <summary>
        /// 拨号
        /// </summary>
        DIAL,
        /// <summary>
        /// 转接、传送
        /// </summary>
        TRANSFER,
        /// <summary>
        /// 保持
        /// </summary>
        HOLD,
        /// <summary>
        /// 执行自定义操作
        /// </summary>
        EXECUTE,
        /// <summary>
        /// 循回
        /// </summary>
        LOOPBACK,
        /// <summary>
        /// 休眠、挂起
        /// </summary>
        SLEEP,
        /// <summary>
        /// 挂机（宣告一次会话活动的终结）
        /// </summary>
        HANGUP,
        /// <summary>
        /// 退出内部状态机
        /// </summary>
        DONE,
        /// <summary>
        /// 通道重置，释放会话占用的资源进入Idle状态
        /// </summary>
        RESET,
        /// <summary>
        /// 清除通道资源
        /// </summary>
        RELEASE,
        /// <summary>
        /// 完成通道清理后的终结状态
        /// </summary>
        FINAL
    }

    #endregion

    #region Channel Type

    /// <summary>
    /// 通道类型
    /// </summary>
    public enum ChannelType
    {
        /// <summary>
        /// 内线通道
        /// </summary>
        USER,

        /// <summary>
        /// 外线通道
        /// </summary>
        TRUNK,

        /// <summary>
        /// 通道悬空
        /// </summary>
        EMPTY,

        /// <summary>
        /// 录音通道
        /// </summary>
        RECORD,

        /// <summary>
        /// 虚拟通道
        /// </summary>
        VIRTUAL
    }

    #endregion

    #region Call session type

    /// <summary>
    /// 会话类型
    /// </summary>
    public enum CallSessionType
    {
        /// <summary>
        /// 会话空闲
        /// </summary>
        NONE,
        /// <summary>
        /// 呼入
        /// </summary>
        INCOMING,
        /// <summary>
        /// 呼出
        /// </summary>
        OUTGOING,
        /// <summary>
        /// 摘机，准备呼出用于内线通道
        /// </summary>
        PICKUP,
        /// <summary>
        /// 内线或外线进行呼叫转移
        /// </summary>
        TRANSFER,
        /// <summary>
        /// 重呼
        /// </summary>
        RECCALL,
        /// <summary>
        /// 路由
        /// </summary>
        DIRECT,
        /// <summary>
        /// 虚拟、自定义
        /// </summary>
        VIRTUAL
    }

    #endregion

    #region Call session message type
    /// <summary>
    /// 会话消息类型
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
    /// TTS放音类型
    /// </summary>
    public enum TTSPlayType
    {
        /// <summary>
        /// 数字
        /// </summary>
        NUMBER,
        /// <summary>
        /// 条目
        /// </summary>
        ITEMS,
        /// <summary>
        /// 人名
        /// </summary>
        PERSONS,
        /// <summary>
        /// 消息
        /// </summary>
        MESSAGES,
        /// <summary>
        /// 金额
        /// </summary>
        CURRENCY,
        /// <summary>
        /// 时间测量
        /// </summary>
        TIME_MEASUREMENT,
        /// <summary>
        /// 当前日期
        /// </summary>
        CURRENT_DATE,
        /// <summary>
        /// 当前时间
        /// </summary>
        CURRENT_TIME,
        /// <summary>
        /// 当前日期时间
        /// </summary>
        CURRENT_DATE_TIME,
        /// <summary>
        /// 电话号码
        /// </summary>
        TELEPHONE_NUMBER,
        /// <summary>
        /// 电话号码扩展
        /// </summary>
        TELEPHONE_EXTENSION,
        /// <summary>
        /// URL
        /// </summary>
        URL,
        /// <summary>
        /// IP地址
        /// </summary>
        IP_ADDRESS,
        /// <summary>
        /// Email
        /// </summary>
        EMAIL_ADDRESS,
        /// <summary>
        /// 邮政编码
        /// </summary>
        POSTAL_ADDRESS,
        /// <summary>
        /// 帐号
        /// </summary>
        ACCOUNT_NUMBER,
        /// <summary>
        /// 名字拼写
        /// </summary>
        NAME_SPELLED,
        /// <summary>
        /// 名字语音
        /// </summary>
        NAME_PHONETIC
    }

    #endregion

    #region Conference Type

    /// <summary>
    /// 通道加入会议的模式
    /// </summary>
    public enum ConferenceType
    {
        /// <summary>
        /// 未知
        /// </summary>
        UNKOWN,
        /// <summary>
        /// 加入，可以听也可以说
        /// </summary>
        JOIN,
        /// <summary>
        /// 监听，只听不说
        /// </summary>
        LISTEN
    }

    #endregion

    #region Fax Mode

    /// <summary>
    /// 传真类型
    /// </summary>
    public enum FaxMode
    {
        /// <summary>
        /// 未知
        /// </summary>
        UNKNOWN,
        /// <summary>
        /// 发送
        /// </summary>
        SEND,
        /// <summary>
        /// 接收
        /// </summary>
        RECEIVE
    }

    #endregion

    #region Voice Resource

    /// <summary>
    /// 放音所使用的语音库资源名称
    /// </summary>
    public enum VoiceResource
    {
        /// <summary>
        /// 未知
        /// </summary>
        UNKNOWN,

        /// <summary>
        /// 普通话
        /// </summary>
        STANDARD,

        /// <summary>
        /// 方言
        /// </summary>
        LOCALIZE,

        /// <summary>
        /// 英语
        /// </summary>
        ENGLISH
    }

    #endregion
}