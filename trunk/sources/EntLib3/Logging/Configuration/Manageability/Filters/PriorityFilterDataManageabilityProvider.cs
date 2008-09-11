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
	internal class PriorityFilterDataManageabilityProvider
		: ConfigurationElementManageabilityProviderBase<PriorityFilterData>
	{
		public const String MaximumPriorityPropertyName = "maximumPriority";
		public const String MinimumPriorityPropertyName = "minimumPriority";

		public PriorityFilterDataManageabilityProvider()
			: base()
		{ }

		protected override void AddElementAdministrativeTemplateParts(AdmContentBuilder contentBuilder,
			PriorityFilterData configurationObject,
			IConfigurationSource configurationSource,
			String elementPolicyKeyName)
		{
			contentBuilder.AddNumericPart(Resources.PriorityFilterMaximumPriorityPartName,
				null,
				MaximumPriorityPropertyName,
				configurationObject.MaximumPriority,
				0,
				999999999);

			contentBuilder.AddNumericPart(Resources.PriorityFilterMinimumPriorityPartName,
				null,
				MinimumPriorityPropertyName,
				configurationObject.MinimumPriority,
				0,
				999999999);
		}

		protected override string ElementPolicyNameTemplate
		{
			get
			{
				return Resources.FilterPolicyNameTemplate;
			}
		}

		protected override void OverrideWithGroupPolicies(PriorityFilterData configurationObject, IRegistryKey policyKey)
		{
			int? minimumPriorityOverride = policyKey.GetIntValue(MinimumPriorityPropertyName);
			int? maximumPriorityOverride = policyKey.GetIntValue(MaximumPriorityPropertyName);

			configurationObject.MinimumPriority = minimumPriorityOverride.Value;
			configurationObject.MaximumPriority = maximumPriorityOverride.Value;
		}

		protected override void GenerateWmiObjects(PriorityFilterData configurationObject,
			ICollection<ConfigurationSetting> wmiSettings)
		{
			wmiSettings.Add(
				new PriorityFilterSetting(configurationObject.Name,
					configurationObject.MaximumPriority,
					configurationObject.MinimumPriority));
		}
	}
}
