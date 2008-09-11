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
using System.Diagnostics;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.TraceListeners
{
	internal abstract class BasicCustomTraceListenerDataManageabilityProvider<T>
		: CustomProviderDataManageabilityProvider<T>
		where T : BasicCustomTraceListenerData
	{
		public const String InitDataPropertyName = "initData";
		public const String TraceOutputOptionsPropertyName = "traceOutputOptions";
		public new const String AttributesPropertyName =
			CustomProviderDataManageabilityProvider<CustomTraceListenerData>.AttributesPropertyName;
		public new const String ProviderTypePropertyName =
			CustomProviderDataManageabilityProvider<CustomTraceListenerData>.ProviderTypePropertyName;

		protected BasicCustomTraceListenerDataManageabilityProvider()
			: base(Resources.TraceListenerPolicyNameTemplate)
		{ }

		protected override void AddElementAdministrativeTemplateParts(AdmContentBuilder contentBuilder, T configurationObject, IConfigurationSource configurationSource, string elementPolicyKeyName)
		{
			base.AddElementAdministrativeTemplateParts(contentBuilder, 
				configurationObject, 
				configurationSource, 
				elementPolicyKeyName);

			contentBuilder.AddEditTextPart(Resources.CustomTraceListenerInitializationPartName,
				InitDataPropertyName,
				configurationObject.InitData,
				1024,
				false);

			contentBuilder.AddDropDownListPartForEnumeration<TraceOptions>(Resources.TraceListenerTraceOptionsPartName,
				TraceOutputOptionsPropertyName,
				configurationObject.TraceOutputOptions);
		}

		protected override void OverrideWithGroupPolicies(T configurationObject, IRegistryKey policyKey)
		{
			String initDataOverride = policyKey.GetStringValue(InitDataPropertyName);
			TraceOptions? traceOutputOptionsOverride = policyKey.GetEnumValue<TraceOptions>(TraceOutputOptionsPropertyName);

			base.OverrideWithGroupPolicies(configurationObject, policyKey);

			configurationObject.InitData = initDataOverride;
			configurationObject.TraceOutputOptions = traceOutputOptionsOverride.Value;
		}
	}
}
