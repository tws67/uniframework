using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 序号生成器接口
    /// </summary>
    public interface ISequenceGenerater
    {
        /// <summary>
        /// 生成下一个可用的序号
        /// </summary>
        /// <returns>下一个可用的序号</returns>
        string GenerateNextID();
    }
}
