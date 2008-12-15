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
 * 0.2.1 - Introduction of ParseError and ParseErrorCollection.
 */

#region Namespace imports

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;

#endregion

namespace Uniframework.SharePoint.Linq
{
    /// <summary>
    /// Represents a parse error with detailed error information.
    /// </summary>
    [Serializable]
    public sealed class ParseError
    {
        #region Private members

        /// <summary>
        /// Error identification code.
        /// </summary>
        private int _errorCode;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new parse error.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <param name="startIndex">Start position of the faulting expression in the original LINQ expression textual representation.</param>
        /// <param name="endIndex">End position of the faulting expression in the original LINQ expression textual representation.</param>
        public ParseError(int errorCode, string message, int startIndex, int endIndex)
        {
            _errorCode = errorCode;
            Message = message;
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Error code.
        /// </summary>
        public string ErrorCode
        {
            get
            {
                //
                // Prefix the SP identifier to the error code.
                //
                return String.Format(CultureInfo.InvariantCulture, "SP{0}", _errorCode.ToString(CultureInfo.InvariantCulture.NumberFormat).PadLeft(4, '0'));
            }
        }

        /// <summary>
        /// Error message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Start position of the faulting expression in the original LINQ expression textual representation.
        /// </summary>
        public int StartIndex { get; private set; }

        /// <summary>
        /// End position of the faulting expression in the original LINQ expression textual representation.
        /// </summary>
        public int EndIndex { get; private set; }

        /// <summary>
        /// Gets the help link for the error.
        /// </summary>
        public string HelpLink
        {
            get
            {
                return String.Format(CultureInfo.InvariantCulture, Errors.HelpLink, this.ErrorCode);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the error message including the error code.
        /// </summary>
        /// <returns>Error message prefixed with the error code.</returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}: {1}", this.ErrorCode, this.Message);
        }

        #endregion
    }

    /// <summary>
    /// Numbered dictionary collection of parse errors.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix"), Serializable]
    public sealed class ParseErrorCollection : Dictionary<int, ParseError>, ISerializable
    {
        #region Private members

        /// <summary>
        /// Identification code counter for unique ParseError instance identification.
        /// </summary>
        private int n = 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of ParseErrorCollection.
        /// </summary>
        public ParseErrorCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ParseErrorCollection class with serialized data.
        /// </summary>
        /// <param name="info">A System.Runtime.Serialization.SerializationInfo object containing the information required to serialize the ParseErrorCollection.</param>
        /// <param name="context">A System.Runtime.Serialization.StreamingContext structure containing the source and destination of the serialized stream associated with the ParseErrorCollection.</param>
        private ParseErrorCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Expression = info.GetString("Expression");
        }

        #endregion

        #region Properties

        /// <summary>
        /// Original complete LINQ expression textual representation.
        /// </summary>
        public string Expression { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a new parse error to the collection.
        /// </summary>
        /// <param name="error">Parse error to add.</param>
        /// <returns>Unique identification code assigned to the parse error.</returns>
        public int Add(ParseError error)
        {
            int i = ++n;
            this.Add(i, error);
            return i;
        }

        /// <summary>
        /// Implements the System.Runtime.Serialization.ISerializable interface and returns the data needed to serialize the ParseErrorCollection instance.
        /// </summary>
        /// <param name="info">A System.Runtime.Serialization.SerializationInfo object that contains the information required to serialize the ParseErrorCollection instance.</param>
        /// <param name="context">A System.Runtime.Serialization.StreamingContext structure that contains the source and destination of the serialized stream associated with the ParseErrorCollection instance.</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Expression", Expression);
        }

        #endregion
    }
}
