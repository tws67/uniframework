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
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.Filters
{
	internal class CategoryFilterDataManageabilityProvider
		: ConfigurationElementManageabilityProviderBase<CategoryFilterData>
	{
		public const String CategoryFilterModePropertyName = "categoryFilterMode";
		public const String CategoryFiltersKeyName = "categoryFilters";

		protected override void AddElementAdministrativeTemplateParts(AdmContentBuilder contentBuilder, 
			CategoryFilterData configurationObject, 
			IConfigurationSource configurationSource, 
			String elementPolicyKeyName)
		{
			contentBuilder.AddDropDownListPartForEnumeration<CategoryFilterMode>(Resources.CategoryFilterFilterModePartName,
				CategoryFilterModePropertyName,
				configurationObject.CategoryFilterMode);

			contentBuilder.AddTextPart(Resources.CategoryFilterCategoriesPartName);

			LoggingSettings configurationSection
				= configurationSource.GetSection(LoggingSettings.SectionName) as LoggingSettings;
			String logFilterCategoriesKeyName
				= elementPolicyKeyName + @"\" + CategoryFiltersKeyName;
			foreach (TraceSourceData category in configurationSection.TraceSources)
			{
				contentBuilder.AddCheckboxPart(category.Name,
					logFilterCategoriesKeyName,
					category.Name,
					configurationObject.CategoryFilters.Contains(category.Name),
					true,
					false);
			}
		}

		protected override string ElementPolicyNameTemplate
		{
			get
			{
				return Resources.FilterPolicyNameTemplate;
			}
		}

		protected override void OverrideWithGroupPolicies(CategoryFilterData configurationObject, IRegistryKey policyKey)
		{
			CategoryFilterMode? categoryFilterModelOverride = policyKey.GetEnumValue<CategoryFilterMode>(CategoryFilterModePropertyName);
			
			// update the filters. it is ok to get the key here, because it's not mandatory.
			configurationObject.CategoryFilters.Clear();
			using (IRegistryKey categoryFiltersOverrideKey = policyKey.OpenSubKey(CategoryFiltersKeyName))
			{
				if (categoryFiltersOverrideKey != null)
				{
					foreach (String valueName in categoryFiltersOverrideKey.GetValueNames())
					{
						configurationObject.CategoryFilters.Add(new CategoryFilterEntry(valueName));
					}
				}
			}
			configurationObject.CategoryFilterMode = categoryFilterModelOverride.Value;
		}

		protected override void GenerateWmiObjects(CategoryFilterData configurationObject,
			ICollection<ConfigurationSetting> wmiSettings)
		{
			String[] categoryFilters = new String[configurationObject.CategoryFilters.Count];
			for (int i = 0; i < configurationObject.CategoryFilters.Count; i++)
			{
				categoryFilters[i]
					= configurationObject.CategoryFilters.Get(i).Name;
			}

			wmiSettings.Add(
				new CategoryFilterSetting(
					configurationObject.Name,
					configurationObject.CategoryFilterMode.ToString(),
					categoryFilters));
		}
	}
}
