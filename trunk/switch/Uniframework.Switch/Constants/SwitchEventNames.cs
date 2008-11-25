using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch
{
    public static class SwitchEventNames
    {
        public const string CreatedChannelEvent = "topic://SwitchEvent/CreatedChannel";

        public const string ChannelStatusChangedEvent = "topic://SwitchEvent/ChannelStatusChanged";
        public const string GetDTMFEvent = "topic://SwitchEvent/GetDTMF";
        public const string CallEvent = "topic://SwitchEvent/Call";
        public const string FaxingEvent = "topic://SwitchEvent/Faxing";
        public const string LinkingToChannelEvent = "topic://SwitchEvent/LinkingToChannel";
        public const string LinkedToChannelEvent = "topic://SwitchEvent/LinkedToChannel";
        public const string ResetedChannelEvent = "topic://SwitchEvent/ResetedChannel";
        public const string ProcessTimeoutEvent = "topic://SwitchEvent/ProcessTimeout";
    }
}
