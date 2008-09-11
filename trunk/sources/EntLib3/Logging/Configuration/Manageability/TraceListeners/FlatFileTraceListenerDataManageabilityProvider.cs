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
	internal class FlatFileTraceListenerDataManageabilityProvider
		: TraceListenerDataManageabilityProvider<FlatFileTraceListenerData>
	{
		public const String FileNamePropertyName = "fileName";
		public const String FooterPropertyName = "footer";
		public const String HeaderPropertyName = "header";

		protected override void AddElementAdministrativeTemplateParts(AdmContentBuilder contentBuilder, 
			FlatFileTraceListenerData configurationObject, 
			IConfigurationSource configurationSource, 
			String elementPolicyKeyName)
		{
			contentBuilder.AddEditTextPart(Resources.FlatFileTraceListenerFileNamePartName,
				FileNamePropertyName,
				configurationObject.FileName,
				255,
				true);

			contentBuilder.AddEditTextPart(Resources.FlatFileTraceListenerHeaderPartName,
				HeaderPropertyName,
				configurationObject.Header,
				512,
				false);

			contentBuilder.AddEditTextPart(Resources.FlatFileTraceListenerFooterPartName,
				FooterPropertyName,
				configurationObject.Footer,
				512,
				false);

			AddTraceOptionsPart(contentBuilder, configurationObject.TraceOutputOptions);

			AddFormattersPart(contentBuilder, configurationObject.Formatter, configurationSource);
		}

		protected override void OverrideWithGroupPolicies(FlatFileTraceListenerData configurationObject, IRegistryKey policyKey)
		{
			String fileNameOverride = policyKey.GetStringValue(FileNamePropertyName);
			String footerOverride = policyKey.GetStringValue(FooterPropertyName);
			String formatterOverride = GetFormatterPolicyOverride(policyKey);
			String headerOverride = policyKey.GetStringValue(HeaderPropertyName);
			TraceOptions? traceOutputOptionsOverride = policyKey.GetEnumValue<TraceOptions>(TraceOutputOptionsPropertyName);

			configurationObject.FileName = fileNameOverride;
			configurationObject.Footer = footerOverride;
			configurationObject.Formatter = formatterOverride;
			configurationObject.Header = headerOverride;
			configurationObject.TraceOutputOptions = traceOutputOptionsOverride.Value;
		}

		protected override void GenerateWmiObjects(FlatFileTraceListenerData configurationObject,
			ICollection<ConfigurationSetting> wmiSettings)
		{
			wmiSettings.Add(
				new FlatFileTraceListenerSetting(configurationObject.Name,
					configurationObject.FileName,
					configurationObject.Header,
					configurationObject.Footer,
					configurationObject.Formatter,
					configurationObject.TraceOutputOptions.ToString()));
		}
	}
}
