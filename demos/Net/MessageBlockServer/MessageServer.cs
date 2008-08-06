using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Uniframework.Net;

namespace MessageBlockServer
{
    public class MessageServer : MessageBlockServer<MessageBlockSession>
    {
        /// <summary>
        /// 接收数据,逻辑处理需要重载该函数
        /// </summary>
        /// <param name="session">会话</param>
        /// <param name="dataBlock">数据</param>
        protected override void OnReceivedData(MessageBlockSession session, DataBlock dataBlock)
        {
            base.OnReceivedData(session, dataBlock);
            Send(session, dataBlock);
        }

        /// <summary>
        /// 通讯错误事件
        /// </summary>
        /// <param name="session"></param>
        /// <param name="e"></param>
        protected override void ReportError(MessageBlockSession session, Exception e)
        {
            NetDebuger.PrintDebugMessage(session, e.Message);
            base.ReportError(session, e);
        }
    }
}
