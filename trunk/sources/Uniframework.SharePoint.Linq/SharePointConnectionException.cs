/*
 * LINQ to SharePoint
 * http://www.codeplex.com/LINQtoSharePoint
 * 
 * Copyright Bart De Smet (C) 2007
 * info@bartdesmet.net - http://blogs.bartdesmet.net/bart
 * 
 * This project is subject to licensing restrictions. Visit http://www.codeplex.com/LINQtoSharePoint/Project/License.aspx for more information.
 */

/*
 * Version history:
 * 
 * 0.2.1 - Part of refactoring
 */

#region Namespace imports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

#endregion

namespace Uniframework.SharePoint.Linq
{
    /// <summary>
    /// Exception for SharePoint connection failures, either via web services or via the object model.
    /// </summary>
    [Serializable]
    public class SharePointConnectionException : Exception
    {
        /// <summary>
        /// Creates a SharePoint connection exception object.
        /// </summary>
        public SharePointConnectionException() { }

        /// <summary>
        /// Creates a SharePoint connection exception object with the specified message.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public SharePointConnectionException(string message) : base(message) { }

        /// <summary>
        /// Creates a SharePoint connection exception object with the specified message and inner exception.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="inner">Inner exception.</param>
        public SharePointConnectionException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Creates a SharePoint connection exception object from serialization information.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Context for serialization.</param>
        protected SharePointConnectionException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context) { }
    }
}
