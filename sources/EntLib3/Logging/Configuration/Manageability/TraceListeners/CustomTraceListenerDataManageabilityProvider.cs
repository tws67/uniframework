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

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.TraceListeners
{
	internal class CustomTraceListenerDataManageabilityProvider
		: BasicCustomTraceListenerDataManageabilityProvider<CustomTraceListenerData>
	{
		public const String FormatterPropertyName = "formatter";

		public CustomTraceListenerDataManageabilityProvider()
		{ }

		protected override void AddElementAdministrativeTemplateParts(AdmContentBuilder contentBuilder,
			CustomTraceListenerData configurationObject,
			IConfigurationSource configurationSource,
			String elementPolicyKeyName)
		{
			base.AddElementAdministrativeTemplateParts(contentBuilder,
				configurationObject,
				configurationSource,
				elementPolicyKeyName);

			LoggingSettings configurationSection = (LoggingSettings)configurationSource.GetSection(LoggingSettings.SectionName);

			contentBuilder.AddDropDownListPartForNamedElementCollection<FormatterData>(Resources.TraceListenerFormatterPartName,
				FormatterPropertyName,
				configurationSection.Formatters,
				configurationObject.Formatter,
				true);
		}
		
		protected override void OverrideWithGroupPolicies(CustomTraceListenerData configurationObject, IRegistryKey policyKey)
		{
			String formatterOverride = policyKey.GetStringValue(FormatterPropertyName);

			base.OverrideWithGroupPolicies(configurationObject, policyKey);

			configurationObject.Formatter = AdmContentBuilder.NoneListItem.Equals(formatterOverride) ? String.Empty : formatterOverride;
		}

		protected sealed override NamedConfigurationSetting CreateSetting(CustomTraceListenerData data, String[] attributes)
		{
			return new CustomTraceListenerSetting(data.Name,
				data.Type.AssemblyQualifiedName,
				data.InitData,
				attributes,
				data.TraceOutputOptions.ToString(),
				data.Formatter);
		}
	}
}
