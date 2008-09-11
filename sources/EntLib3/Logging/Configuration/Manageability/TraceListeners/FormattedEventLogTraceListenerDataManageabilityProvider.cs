//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Logging Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.TraceListeners
{
	internal class FormattedEventLogTraceListenerDataManageabilityProvider
		: TraceListenerDataManageabilityProvider<FormattedEventLogTraceListenerData>
	{
		public const String LogPropertyName = "log";
		public const String MachineNamePropertyName = "machineName";
		public const String SourcePropertyName = "source";

		protected override void AddElementAdministrativeTemplateParts(AdmContentBuilder contentBuilder,
			FormattedEventLogTraceListenerData configurationObject,
			IConfigurationSource configurationSource,
			String elementPolicyKeyName)
		{
			contentBuilder.AddEditTextPart(Resources.EventLogTraceListenerSourcePartName,
				SourcePropertyName,
				configurationObject.Source,
				255,
				true);

			contentBuilder.AddEditTextPart(Resources.EventLogTraceListenerLogPartName,
				LogPropertyName,
				configurationObject.Log,
				255,
				false);

			contentBuilder.AddEditTextPart(Resources.EventLogTraceListenerMachineNamePartName,
				MachineNamePropertyName,
				configurationObject.MachineName,
				255,
				false);

			AddTraceOptionsPart(contentBuilder, configurationObject.TraceOutputOptions);

			AddFormattersPart(contentBuilder, configurationObject.Formatter, configurationSource);
		}

		protected override void OverrideWithGroupPolicies(FormattedEventLogTraceListenerData configurationObject, IRegistryKey policyKey)
		{
			String formatterOverride = GetFormatterPolicyOverride(policyKey);
			String logOverride = policyKey.GetStringValue(LogPropertyName);
			String machineNameOverride = policyKey.GetStringValue(MachineNamePropertyName);
			String sourceOverride = policyKey.GetStringValue(SourcePropertyName);
			TraceOptions? traceOutputOptionsOverride = policyKey.GetEnumValue<TraceOptions>(TraceOutputOptionsPropertyName);

			configurationObject.Formatter = formatterOverride;
			configurationObject.Log = logOverride;
			configurationObject.MachineName = machineNameOverride;
			configurationObject.Source = sourceOverride;
			configurationObject.TraceOutputOptions = traceOutputOptionsOverride.Value;
		}

		protected override void GenerateWmiObjects(FormattedEventLogTraceListenerData configurationObject,
			ICollection<ConfigurationSetting> wmiSettings)
		{
			wmiSettings.Add(
				new FormattedEventLogTraceListenerSetting(configurationObject.Name,
					configurationObject.Source,
					configurationObject.Log,
					configurationObject.MachineName,
					configurationObject.Formatter,
					configurationObject.TraceOutputOptions.ToString()));
		}
	}
}
