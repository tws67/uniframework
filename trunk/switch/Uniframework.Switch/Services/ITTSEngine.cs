using System;
using System.Collections.Generic;
using System.Text;

using log4net;
using Uniframework.Services;

namespace Uniframework.Switch
{
    public interface ITTSEngine
    {
        bool CanWork { get; }
        bool XmlTag { get; set; }
        ILog Logger { get; }

        SwitchStatus PlayMessage(IChannel channel, string text, bool allowBreak, TTSPlayType playType);
        bool PlayToFile(IChannel channel, string text, TTSPlayType playType, string fileName);
    }
}
