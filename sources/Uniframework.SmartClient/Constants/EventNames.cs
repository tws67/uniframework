using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.SmartClient
{
    public static class EventNames
    {
        public const string Shell_PropertyChanged     = "topic://Shell/Foundation/PropertyChanged";
        public const string Shell_SettingViewChanged  = "topic://Shell/Foundation/SettingViewChanged";
        public const string Shell_SettingSaved        = "topic://Shell/Foundation/SettingSaved";
        public const string Shell_ShellClosing        = "topic://Shell/Foundation/ShellClosing";
        public const string Shell_DynamicHelpUpdated  = "topic://Shell/Foundation/DynamicHelpUpdated";

        public const string Shell_StatusUpdated       = "topic://Shell/StatusUpdate";
        public const string Shell_ProgressBarChanged  = "topic://Shell/ProgressBarChanged";
        public const string Shell_CustomPanel1Updated = "topic://Shell/CustomPanel1Updated";
        public const string Shell_CustomPanel2Updated = "topic://Shell/CustomPanel2Updated";
        public const string Shell_AddressUriChanged   = "topic://Shell/AddressUriChanged";
    }
}