using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch
{
    /// <summary>
    /// CTI脚本接口，所有需要支持语音操作的脚本都必须继承这个接口
    /// </summary>
    public interface ICTIScript
    {
        /// <summary>
        /// 脚本所引用的通道
        /// </summary>
        IChannel Channel { get; }
        /// <summary>
        /// 脚本初始化函数
        /// </summary>
        /// <param name="chnl"></param>
        void Initialize(IChannel chnl);
        /// <summary>
        /// 脚本执行体函数，所有的业务逻辑放在此处
        /// </summary>
        void Run();
        /// <summary>
        /// 脚本执行体重载版本，允许在执行时传入相应的参数
        /// </summary>
        /// <param name="args">参数列表</param>
        void Run(object[] args);
        /// <summary>
        /// 脚本结束清理函数
        /// </summary>
        void Terminate();
    }
}
