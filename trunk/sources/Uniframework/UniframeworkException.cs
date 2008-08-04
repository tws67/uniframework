using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// 统一开发框架异常基础类
    /// </summary>
    [Serializable]
    public class UniframeworkException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UniframeworkException"/> class.
        /// </summary>
        public UniframeworkException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniframeworkException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public UniframeworkException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniframeworkException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public UniframeworkException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniframeworkException"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected UniframeworkException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
