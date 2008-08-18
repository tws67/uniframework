using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 插件处理异常
    /// </summary>
    public class AddInException : Exception
    {
        public AddInException()
            : base()
        { }

        public AddInException(string message)
            : base(message)
        { }

        public AddInException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public AddInException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
