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
	internal class WmiTraceListenerDataManageabilityProvider
		: TraceListenerDataManageabilityProvider<WmiTraceListenerData>
	{
		protected override void AddElementAdministrativeTemplateParts(AdmContentBuilder contentBuilder,
			WmiTraceListenerData configurationObject,
			IConfigurationSource configurationSource,
			String elementPolicyKeyName)
		{
			AddTraceOptionsPart(contentBuilder, configurationObject.TraceOutputOptions);
		}

		protected override void OverrideWithGroupPolicies(WmiTraceListenerData configurationObject, IRegistryKey policyKey)
		{
			TraceOptions? traceOutputOptionsOverride = policyKey.GetEnumValue<TraceOptions>(TraceOutputOptionsPropertyName);

			configurationObject.TraceOutputOptions = traceOutputOptionsOverride.Value;
		}

		protected override void GenerateWmiObjects(WmiTraceListenerData configurationObject,
			ICollection<ConfigurationSetting> wmiSettings)
		{
			wmiSettings.Add(
				new WmiTraceListenerSetting(configurationObject.Name,
						configurationObject.TraceOutputOptions.ToString()));
		}
	}
}
