//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Exception Handling Application Block
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
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Logging.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Logging.Configuration.Manageability
{
	internal class LoggingExceptionHandlerDataManageabilityProvider
		: ConfigurationElementManageabilityProviderBase<LoggingExceptionHandlerData>
	{
		public const String EventIdPropertyName = "eventId";
		public const String FormatterTypePropertyName = "formatterType";
		public const String LogCategoryPropertyName = "logCategory";
		public const String PriorityPropertyName = "priority";
		public const String SeverityPropertyName = "severity";
		public const String TitlePropertyName = "title";

		protected override void AddAdministrativeTemplateDirectives(AdmContentBuilder contentBuilder,
			LoggingExceptionHandlerData configurationObject,
			IConfigurationSource configurationSource,
			String elementPolicyKeyName)
		{
			// directives are parts of the exception type policy
			AddElementAdministrativeTemplateParts(contentBuilder,
				configurationObject,
				configurationSource,
				elementPolicyKeyName);
		}

		protected override void AddElementAdministrativeTemplateParts(AdmContentBuilder contentBuilder,
			LoggingExceptionHandlerData configurationObject,
			IConfigurationSource configurationSource,
			String elementPolicyKeyName)
		{
			contentBuilder.AddTextPart(String.Format(CultureInfo.CurrentCulture,
													Resources.HandlerPartNameTemplate,
													configurationObject.Name));

			contentBuilder.AddEditTextPart(Resources.LoggingHandlerTitlePartName,
				elementPolicyKeyName,
				TitlePropertyName,
				configurationObject.Title,
				255,
				true);

			contentBuilder.AddNumericPart(Resources.LoggingHandlerEventIdPartName,
				elementPolicyKeyName,
				EventIdPropertyName,
				configurationObject.EventId);

			contentBuilder.AddDropDownListPartForEnumeration<TraceEventType>(Resources.LoggingHandlerSeverityPartName,
				elementPolicyKeyName,
				SeverityPropertyName,
				configurationObject.Severity);

			contentBuilder.AddNumericPart(Resources.LoggingHandlerPriorityPartName,
				elementPolicyKeyName,
				PriorityPropertyName,
				configurationObject.Priority);

			LoggingSettings loggingConfigurationSection
				= configurationSource.GetSection(LoggingSettings.SectionName) as LoggingSettings;

			contentBuilder.AddDropDownListPartForNamedElementCollection<TraceSourceData>(Resources.LoggingHandlerCategoryPartName,
				elementPolicyKeyName,
				LogCategoryPropertyName,
				loggingConfigurationSection.TraceSources,
				configurationObject.LogCategory,
				false);

			contentBuilder.AddComboBoxPart(Resources.LoggingHandlerFormatterPartName,
				elementPolicyKeyName,
				FormatterTypePropertyName,
				configurationObject.FormatterType.AssemblyQualifiedName,
				255,
				true,
				typeof(TextExceptionFormatter).AssemblyQualifiedName,
				typeof(XmlExceptionFormatter).AssemblyQualifiedName);
		}

		protected override string ElementPolicyNameTemplate
		{
			get { return null; }
		}

		protected override void OverrideWithGroupPolicies(LoggingExceptionHandlerData configurationObject, IRegistryKey policyKey)
		{
			int? eventIdOverride = policyKey.GetIntValue(EventIdPropertyName);
			Type formatterTypeOverride = policyKey.GetTypeValue(FormatterTypePropertyName);
			String logCategoryOverride = policyKey.GetStringValue(LogCategoryPropertyName);
			int? priorityOverride = policyKey.GetIntValue(PriorityPropertyName);
			TraceEventType? severityOverride = policyKey.GetEnumValue<TraceEventType>(SeverityPropertyName);
			String titleOverride = policyKey.GetStringValue(TitlePropertyName);

			configurationObject.EventId = eventIdOverride.Value;
			configurationObject.FormatterType = formatterTypeOverride;
			configurationObject.LogCategory = logCategoryOverride;
			configurationObject.Priority = priorityOverride.Value;
			configurationObject.Severity = severityOverride.Value;
			configurationObject.Title = titleOverride;
		}

		protected override void GenerateWmiObjects(LoggingExceptionHandlerData configurationObject,
			ICollection<ConfigurationSetting> wmiSettings)
		{
			wmiSettings.Add(
				new LoggingExceptionHandlerSetting(configurationObject.Name,
					configurationObject.EventId,
					configurationObject.FormatterType.AssemblyQualifiedName,
					configurationObject.LogCategory,
					configurationObject.Priority,
					configurationObject.Severity.ToString(),
					configurationObject.Title));
		}
	}
}
