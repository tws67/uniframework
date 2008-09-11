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
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.TraceListeners
{
	internal class RollingFlatFileTraceListenerDataManageabilityProvider
		: TraceListenerDataManageabilityProvider<RollingFlatFileTraceListenerData>
	{
		public const String FileNamePropertyName = "fileName";
		public const String RollFileExistsBehaviorPropertyName = "rollFileExistsBehavior";
		public const String RollIntervalPropertyName = "rollInterval";
		public const String RollSizeKBPropertyName = "rollSizeKB";
		public const String TimeStampPatternPropertyName = "timeStampPattern";
		public const String HeaderPropertyName = "header";
		public const String FooterPropertyName = "footer";

		protected override void AddElementAdministrativeTemplateParts(AdmContentBuilder contentBuilder,
			RollingFlatFileTraceListenerData configurationObject,
			IConfigurationSource configurationSource,
			String elementPolicyKeyName)
		{
			contentBuilder.AddEditTextPart(Resources.RollingFlatFileTraceListenerFileNamePartName,
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

			contentBuilder.AddDropDownListPartForEnumeration<RollFileExistsBehavior>(Resources.RollingFlatFileTraceListenerRollFileExistsBehaviorPartName,
				RollFileExistsBehaviorPropertyName,
				configurationObject.RollFileExistsBehavior);

			contentBuilder.AddDropDownListPartForEnumeration<RollInterval>(Resources.RollingFlatFileTraceListenerRollIntervalPartName,
				RollIntervalPropertyName,
				configurationObject.RollInterval);

			contentBuilder.AddNumericPart(Resources.RollingFlatFileTraceListenerRollSizeKBPartName,
				RollSizeKBPropertyName,
				configurationObject.RollSizeKB);

			contentBuilder.AddEditTextPart(Resources.RollingFlatFileTraceListenerTimeStampPatternPartName,
				TimeStampPatternPropertyName,
				configurationObject.TimeStampPattern,
				512,
				true);

			AddTraceOptionsPart(contentBuilder, configurationObject.TraceOutputOptions);

			AddFormattersPart(contentBuilder, configurationObject.Formatter, configurationSource);
		}

		protected override void OverrideWithGroupPolicies(RollingFlatFileTraceListenerData configurationObject, IRegistryKey policyKey)
		{
			String fileNameOverride = policyKey.GetStringValue(FileNamePropertyName);
			String formatterOverride = GetFormatterPolicyOverride(policyKey);
			RollFileExistsBehavior? rollFileExistsBehaviorOverride = policyKey.GetEnumValue<RollFileExistsBehavior>(RollFileExistsBehaviorPropertyName);
			RollInterval? rollIntervalOverride = policyKey.GetEnumValue<RollInterval>(RollIntervalPropertyName);
			int? rollSizeKBOverride = policyKey.GetIntValue(RollSizeKBPropertyName);
			string timeStampPatternOverride = policyKey.GetStringValue(TimeStampPatternPropertyName);
			TraceOptions? traceOutputOptionsOverride = policyKey.GetEnumValue<TraceOptions>(TraceOutputOptionsPropertyName);
			string headerOverride = policyKey.GetStringValue(HeaderPropertyName);
			string footerOverride = policyKey.GetStringValue(FooterPropertyName);


			configurationObject.FileName = fileNameOverride;
			configurationObject.Header = headerOverride;
			configurationObject.Footer = footerOverride;
			configurationObject.Formatter = formatterOverride;
			configurationObject.RollFileExistsBehavior = rollFileExistsBehaviorOverride.Value;
			configurationObject.RollInterval = rollIntervalOverride.Value;
			configurationObject.RollSizeKB = rollSizeKBOverride.Value;
			configurationObject.TimeStampPattern = timeStampPatternOverride;
			configurationObject.TraceOutputOptions = traceOutputOptionsOverride.Value;
		}

		protected override void GenerateWmiObjects(RollingFlatFileTraceListenerData configurationObject,
			ICollection<ConfigurationSetting> wmiSettings)
		{
			wmiSettings.Add(
				new RollingFlatFileTraceListenerSetting(configurationObject.Name,
					configurationObject.FileName,
					configurationObject.Header,
					configurationObject.Footer,
					configurationObject.Formatter,
					configurationObject.RollFileExistsBehavior.ToString(),
					configurationObject.RollInterval.ToString(),
					configurationObject.RollSizeKB,
					configurationObject.TimeStampPattern,
					configurationObject.TraceOutputOptions.ToString()));
		}
	}
}