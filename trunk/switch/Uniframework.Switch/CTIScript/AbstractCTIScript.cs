using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch
{
    /// <summary>
    /// UniframeworkΩ≈±æ≥ÈœÛ¿‡
    /// </summary>
    public class AbstractCTIScript : ICTIScript
    {
        IChannel channel;

        #region ICTIScript Members

        public IChannel Channel
        {
            get { return channel; }
        }

        public virtual void Initialize(IChannel chnl)
        {
            this.channel = chnl;
        }

        public virtual void Run()
        {
        }

        public virtual void Run(object[] args)
        { 
        }

        public virtual void Terminate()
        {
        }

        #endregion
    }
}
