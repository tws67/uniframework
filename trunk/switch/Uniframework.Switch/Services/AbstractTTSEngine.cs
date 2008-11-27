using System;
using System.Collections.Generic;
using System.Text;

using log4net;

using Microsoft.Practices.CompositeUI;
using Uniframework.Services;

namespace Uniframework.Switch
{
    /// <summary>
    /// TTS“˝«Ê≥ÈœÛ¿‡
    /// </summary>
    public abstract class AbstractTTSEngine : ITTSEngine
    {
        protected WorkItem workItem;

        protected bool canWork = false;
        private bool xmlTag = true;
        private ILog logger = null;
        private int threadNumber = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractTTSEngine"/> class.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <param name="threadNumber">The thread number.</param>
        public AbstractTTSEngine(WorkItem workItem, int threadNumber)
        {
            this.workItem = workItem;
            this.logger = workItem.Services.Get<ILog>();
            this.threadNumber = threadNumber;
        }

        #region ITTSEngine Members

        /// <summary>
        /// Gets a value indicating whether this instance can work.
        /// </summary>
        /// <value><c>true</c> if this instance can work; otherwise, <c>false</c>.</value>
        public bool CanWork
        {
            get { return canWork; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [XML tag].
        /// </summary>
        /// <value><c>true</c> if [XML tag]; otherwise, <c>false</c>.</value>
        public bool XmlTag
        {
            get
            {
                return xmlTag;
            }
            set
            {
                xmlTag = value;
            }
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        public ILog Logger
        {
            get { return logger; }
        }

        /// <summary>
        /// Plays the message.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="text">The text.</param>
        /// <param name="allowBreak">if set to <c>true</c> [allow break].</param>
        /// <param name="playType">Type of the play.</param>
        /// <returns></returns>
        public virtual SwitchStatus PlayMessage(IChannel channel, string text, bool allowBreak, TTSPlayType playType)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Plays to file.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="text">The text.</param>
        /// <param name="playType">Type of the play.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public virtual bool PlayToFile(IChannel channel, string text, TTSPlayType playType, string fileName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
