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
using System.Globalization;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.Formatters
{
	internal class TextFormatterDataManageabilityProvider
		: ConfigurationElementManageabilityProviderBase<TextFormatterData>
	{
		public const String TemplatePropertyName = "template";

		protected override void AddElementAdministrativeTemplateParts(AdmContentBuilder contentBuilder,
			TextFormatterData configurationObject,
			IConfigurationSource configurationSource,
			String elementPolicyKeyName)
		{
			contentBuilder.AddEditTextPart(Resources.TextFormatterTemplatePartName,
				TemplatePropertyName,
				"",
				1024,
				true);
			contentBuilder.AddTextPart(Resources.TextFormatterEscapeInstructions_1);
			contentBuilder.AddTextPart(Resources.TextFormatterEscapeInstructions_2);
		}

		protected override string ElementPolicyNameTemplate
		{
			get
			{
				return Resources.FormatterPolicyNameTemplate;
			}
		}

		protected override void OverrideWithGroupPolicies(TextFormatterData configurationObject, IRegistryKey policyKey)
		{
			String templateOverride = UnescapeString(policyKey.GetStringValue(TemplatePropertyName));

			configurationObject.Template = templateOverride;
		}

		protected override void GenerateWmiObjects(TextFormatterData configurationObject,
			ICollection<ConfigurationSetting> wmiSettings)
		{
			wmiSettings.Add(new TextFormatterSetting(configurationObject.Name, configurationObject.Template));
		}

		public static string EscapeString(string text)
		{
			string result = text.Replace("\\", @"\\");
			result = result.Replace("\n", @"\n");
			result = result.Replace("\r", @"\r");

			return result;
		}

		public static string UnescapeString(string text)
		{
			string result = text.Replace(@"\r", "\r");
			result = result.Replace(@"\n", "\n");
			result = result.Replace(@"\\", "\\");

			return result;
		}
	}
}
