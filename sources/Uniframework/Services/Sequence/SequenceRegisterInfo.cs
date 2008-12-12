using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 序号注册表项
    /// </summary>
    [Serializable]
    public class SequenceRegisterInfo
    {
        private string appUri;
        private string objectName;
        private string extension;

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceRegisterInfo"/> class.
        /// </summary>
        /// <param name="appUri">The app URI.</param>
        /// <param name="objectName">Name of the object.</param>
        public SequenceRegisterInfo(string appUri, string objectName)
        {
            this.appUri = appUri;
            this.objectName = objectName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceRegisterInfo"/> class.
        /// </summary>
        /// <param name="appUri">The app URI.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="extension">The extension.</param>
        public SequenceRegisterInfo(string appUri, string objectName, string extension)
            : this(appUri, objectName)
        {
            this.extension = extension;
        }

        #region Members
        
        /// <summary>
        /// 需要使用序号的应用Uri
        /// </summary>
        public string AppUri
        {
            get { return AppUri; }
        }

        /// <summary>
        /// 使用序号的对象名称或字段名称
        /// </summary>
        public string ObjectName
        {
            get { return objectName; }
        }

        /// <summary>
        /// 扩展，针对某些一个字段可能需要多组序号的情况
        /// </summary>
        public string Extension
        {
            get { return extension; }
            set { extension = value; }
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return String.Format("SEQINFO : AppUri[{0}]; ObjectName[{1}]", appUri, objectName);
        }
    }
}

