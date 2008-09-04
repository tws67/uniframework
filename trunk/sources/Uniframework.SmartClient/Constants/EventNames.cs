using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.SmartClient
{
    public static class EventNames
    {
        public const string Uniframework_PropertyChanged = "topic://Uniframework/PropertyChanged";
        public const string Uniframework_ShowSettingView = "topic://Uniframework/ShowSettingView";

        public const string Shell_StatusUpdated       = "topic://Uniframework/Shell/StatusUpdate";
        public const string Shell_ProgressBarChanged  = "topic://Uniframework/Shell/ProgressBarChanged";
        public const string Shell_CustomPanel1Updated = "topic://Uniframework/Shell/CustomPanel1Updated";
        public const string Shell_CustomPanel2Updated = "topic://Uniframework/Shell/CustomPanel2Updated";
    }
}