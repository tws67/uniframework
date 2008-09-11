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

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.Filters
{
	internal class LogEnabledFilterDataManageabilityProvider
		: ConfigurationElementManageabilityProviderBase<LogEnabledFilterData>
	{
		public const String EnabledPropertyName = "enabled";

		protected override void AddElementAdministrativeTemplateParts(AdmContentBuilder contentBuilder, 
			LogEnabledFilterData configurationObject, 
			IConfigurationSource configurationSource, 
			String elementPolicyKeyName)
		{
			contentBuilder.AddCheckboxPart(Resources.LogEnabledFilterEnabledPartName,
				EnabledPropertyName,
				configurationObject.Enabled);
		}

		protected override string ElementPolicyNameTemplate
		{
			get
			{
				return Resources.FilterPolicyNameTemplate;
			}
		}

		protected override void OverrideWithGroupPolicies(LogEnabledFilterData configurationObject, IRegistryKey policyKey)
		{
			bool? enabledOverride = policyKey.GetBoolValue(EnabledPropertyName);

			configurationObject.Enabled = enabledOverride.Value;
		}

		protected override void GenerateWmiObjects(LogEnabledFilterData configurationObject,
			ICollection<ConfigurationSetting> wmiSettings)
		{
			wmiSettings.Add(new LogEnabledFilterSetting(configurationObject.Name, configurationObject.Enabled));
		}
	}
}
