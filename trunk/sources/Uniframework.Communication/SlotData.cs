using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.Communication
{
    /// <summary>
    /// 数据槽结构
    /// </summary>
    public class SlotData
    {
        /// <summary>
        /// 回调接口
        /// </summary>
        public IInvokeCallback CallBack;
        /// <summary>
        /// 会话标识
        /// </summary>
        public string SessionId;
    }
}
