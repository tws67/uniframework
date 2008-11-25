using System;
using System.Collections.Generic;
using System.Text;

using log4net;

using Microsoft.Practices.CompositeUI;
using Uniframework.Services;

namespace Uniframework.Switch
{
    public abstract class AbstractTTSEngine : ITTSEngine
    {
        protected WorkItem workItem;

        protected bool canWork = false;
        private bool xmlTag = true;
        private ILog logger = null;
        private int threadNumber = 1;

        public AbstractTTSEngine(WorkItem workItem, int threadNumber)
        {
            this.workItem = workItem;
            this.logger = workItem.Services.Get<ILog>();
            this.threadNumber = threadNumber;
        }

        #region ITTSEngine Members

        public bool CanWork
        {
            get { return canWork; }
        }

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

        public ILog Logger
        {
            get { return logger; }
        }

        public virtual SwitchStatus PlayMessage(IChannel channel, string text, bool allowBreak, TTSPlayType playType)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual bool PlayToFile(IChannel channel, string text, TTSPlayType playType, string fileName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
