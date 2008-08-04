using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 序号服务，生成系统需要的各种序号并维护当前序号池
    /// </summary>
    public interface ISequenceService
    {
        /// <summary>
        /// 向系统注册一项序号序列
        /// </summary>
        /// <param name="seqInfo">注册项</param>
        /// <param name="sequence">序号定义类</param>
        void RegisterSequence(SequenceRegisterInfo seqInfo, Sequence sequence);
        /// <summary>
        /// 注销一项序号序列
        /// </summary>
        /// <param name="seqInfo"></param>
        void UnRegisterSequence(SequenceRegisterInfo seqInfo);
        /// <summary>
        /// 获取一个序号
        /// </summary>
        string GetSEQID(SequenceRegisterInfo seqInfo);
        /// <summary>
        /// 向序号归还一个序号
        /// </summary>
        /// <param name="seqInfo">注册项</param>
        /// <param name="SEQID">序号值</param>
        void ReturnSEQID(SequenceRegisterInfo seqInfo, string SEQID);

        event EventHandler<EventArgs<SequenceRegisterInfo>> ReceivedSEQID;
    }
}
