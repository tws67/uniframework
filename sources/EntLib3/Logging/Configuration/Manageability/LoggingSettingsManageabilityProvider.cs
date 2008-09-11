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

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability
{
	/// <summary>
	/// Represents the behavior required to provide Group Policy updates and to publish the 
	/// <see cref="ConfigurationSetting"/> instances associated to a <see cref="LoggingSettings"/> instance
	/// and its internal configuration elements.
	/// </summary>
	public class LoggingSettingsManageabilityProvider
		: ConfigurationSectionManageabilityProviderBase<LoggingSettings>
	{
		internal const String DefaultCategoryPropertyName = "defaultCategory";
		internal const String TracingEnabledPropertyName = "tracingEnabled";
		internal const String LogWarningOnNoMatchPropertyName = "logWarningsWhenNoCategoriesMatch";

		internal const String CategorySourcesKeyName = "categorySources";
		internal const String SpecialSourcesKeyName = "specialSources";
		internal const String SpecialSourcesAllEventsKeyName = "allEvents";
		internal const String SpecialSourcesErrorsKeyName = "errors";
		internal const String SpecialSourcesNotProcessedKeyName = "notProcessed";

		internal const String SourceDefaultLevelPropertyName = "switchValue";
		internal const String SourceTraceListenersPropertyName = "traceListeners";

		internal const String SourceKindCategory = "Category";
		internal const String SourceKindAllEvents = "All events";
		internal const String SourceKindErrors = "Errors";
		internal const String SourceKindNotProcessed = "Not processed";

		internal const String LogFiltersKeyName = "logFilters";

		internal const String LogFormattersKeyName = "formatters";

		internal const String TraceListenersKeyName = "listeners";

		/// <summary>
		/// <para>This method supports the Enterprise Library Manageability Extensions infrastructure and is not intended to 
		/// be used directly from your code.</para>
		/// Initializes a new instance of the <see cref="LoggingSettingsManageabilityProvider"/> class with a 
		/// given set of manageability providers to use when dealing with the configuration for filters, formatters and trace listeners.
		/// </summary>
		/// <param name="subProviders">The mapping from configuration element type to
		/// <see cref="ConfigurationElementManageabilityProvider"/>.</param>
		public LoggingSettingsManageabilityProvider(IDictionary<Type, ConfigurationElementManageabilityProvider> subProviders)
			: base(subProviders)
		{ }

		#region ADM generation

		/// <summary>
		/// Adds the ADM instructions that describe the policies that can be used to override the configuration
		/// information represented by a configuration section.
		/// </summary>
		/// <param name="contentBuilder">The <see cref="AdmContentBuilder"/> to which the Adm instructions are to be appended.</param>
		/// <param name="configurationSection">The configuration section instance.</param>
		/// <param name="configurationSource">The configuration source from where to get additional configuration
		/// information, if necessary.</param>
		/// <param name="sectionKey">The root key for the section's policies.</param>
		protected override void AddAdministrativeTemplateDirectives(AdmContentBuilder contentBuilder,
			LoggingSettings configurationSection,
			IConfigurationSource configurationSource,
			String sectionKey)
		{
			AddBlockSettingsPolicy(contentBuilder, sectionKey, configurationSection);
			AddCategorySourcesPolicies(contentBuilder, sectionKey, configurationSection);
			AddSpecialSourcesPolicies(contentBuilder, sectionKey, configurationSection);
			AddElementsPolicies<LogFilterData>(contentBuilder,
				configurationSection.LogFilters,
				configurationSource,
				sectionKey + @"\" + LogFiltersKeyName,
				Resources.LogFiltersCategoryName);
			AddElementsPolicies<FormatterData>(contentBuilder,
				configurationSection.Formatters,
				configurationSource,
				sectionKey + @"\" + LogFormattersKeyName,
				Resources.LogFormattersCategoryName);
			AddElementsPolicies<TraceListenerData>(contentBuilder,
				configurationSection.TraceListeners,
				configurationSource,
				sectionKey + @"\" + TraceListenersKeyName,
				Resources.TraceListenersCategoryName);
		}

		/// <summary>
		/// Gets the name of the category that represents the whole configuration section.
		/// </summary>
		protected override string SectionCategoryName
		{
			get { return Resources.LoggingSectionCategoryName; }
		}

		/// <summary>
		/// Gets the name of the managed configuration section.
		/// </summary>
		protected override string SectionName
		{
			get { return LoggingSettings.SectionName; }
		}

		private static void AddBlockSettingsPolicy(AdmContentBuilder contentBuilder,
			String sectionKey,
			LoggingSettings configurationSection)
		{
			contentBuilder.StartPolicy(Resources.LoggingSettingsPolicyName, sectionKey);
			{
				contentBuilder.AddDropDownListPartForNamedElementCollection<TraceSourceData>(Resources.LoggingSettingsDefaultCategoryPartName,
					DefaultCategoryPropertyName,
					configurationSection.TraceSources,
					configurationSection.DefaultCategory,
					false);

				contentBuilder.AddCheckboxPart(Resources.LoggingSettingsLogWarningPartName,
					LogWarningOnNoMatchPropertyName,
					configurationSection.LogWarningWhenNoCategoriesMatch);

				contentBuilder.AddCheckboxPart(Resources.LoggingSettingsEnableTracingPartName,
					TracingEnabledPropertyName,
					configurationSection.TracingEnabled);
			}
			contentBuilder.EndPolicy();
		}

		private static void AddCategorySourcesPolicies(AdmContentBuilder contentBuilder,
			String sectionKey,
			LoggingSettings configurationSection)
		{
			String traceSourcesKey = sectionKey + @"\" + CategorySourcesKeyName;

			contentBuilder.StartCategory(Resources.CategorySourcesCategoryName);

			foreach (TraceSourceData traceSourceData in configurationSection.TraceSources)
			{
				AddTraceSourcePolicy(traceSourceData,
					traceSourceData.Name,
					traceSourcesKey,
					contentBuilder,
					configurationSection);
			}

			contentBuilder.EndCategory();
		}

		private static void AddSpecialSourcesPolicies(AdmContentBuilder contentBuilder,
			String sectionKey,
			LoggingSettings configurationSection)
		{
			String specialTraceSourcesKey = sectionKey + @"\" + SpecialSourcesKeyName;

			contentBuilder.StartCategory(Resources.SpecialSourcesCategoryName);

			AddTraceSourcePolicy(configurationSection.SpecialTraceSources.AllEventsTraceSource,
				SpecialSourcesAllEventsKeyName,
				specialTraceSourcesKey,
				contentBuilder,
				configurationSection);

			AddTraceSourcePolicy(configurationSection.SpecialTraceSources.NotProcessedTraceSource,
				SpecialSourcesNotProcessedKeyName,
				specialTraceSourcesKey,
				contentBuilder,
				configurationSection);

			AddTraceSourcePolicy(configurationSection.SpecialTraceSources.ErrorsTraceSource,
				SpecialSourcesErrorsKeyName,
				specialTraceSourcesKey,
				contentBuilder,
				configurationSection);

			contentBuilder.EndCategory();
		}

		private static void AddTraceSourcePolicy(TraceSourceData traceSourceData,
			String traceSourceName,
			String parentKey,
			AdmContentBuilder contentBuilder,
			LoggingSettings configurationSection)
		{
			String traceSourceKey = parentKey + @"\" + traceSourceName;

			contentBuilder.StartPolicy(String.Format(CultureInfo.InvariantCulture,
													Resources.TraceSourcePolicyNameTemplate,
													traceSourceName),
				traceSourceKey);
			{
				contentBuilder.AddDropDownListPartForEnumeration<SourceLevels>(Resources.TraceSourceDefaultLevelPartName,
					SourceDefaultLevelPropertyName,
					traceSourceData.DefaultLevel);

				contentBuilder.AddTextPart(Resources.TraceSourceListenersPartName);

				String traceSourceListenersKey = traceSourceKey + @"\" + SourceTraceListenersPropertyName;
				foreach (TraceListenerData traceListener in configurationSection.TraceListeners)
				{
					contentBuilder.AddCheckboxPart(traceListener.Name,
						traceSourceListenersKey,
						traceListener.Name,
						traceSourceData.TraceListeners.Contains(traceListener.Name),
						true,
						false);
				}
			}
			contentBuilder.EndPolicy();
		}

		#endregion

		#region Manageability support

		/// <summary>
		/// Overrides the <paramref name="configurationSection"/>'s properties with the Group Policy values from 
		/// the registry.
		/// </summary>
		/// <param name="configurationSection">The configuration section that must be managed.</param>
		/// <param name="policyKey">The <see cref="IRegistryKey"/> which holds the Group Policy overrides.</param>
		protected override void OverrideWithGroupPoliciesForConfigurationSection(LoggingSettings configurationSection, IRegistryKey policyKey)
		{
			String defaultCategoryOverride = policyKey.GetStringValue(DefaultCategoryPropertyName);
			bool? tracingEnabledEnabledOverride = policyKey.GetBoolValue(TracingEnabledPropertyName);
			bool? logWarningWhenNoCategoriesMatchOverride = policyKey.GetBoolValue(LogWarningOnNoMatchPropertyName);

			configurationSection.DefaultCategory = defaultCategoryOverride;
			configurationSection.TracingEnabled = tracingEnabledEnabledOverride.Value;
			configurationSection.LogWarningWhenNoCategoriesMatch = logWarningWhenNoCategoriesMatchOverride.Value;
		}

		/// <summary>
		/// Creates the <see cref="ConfigurationSetting"/> instances that describe the <paramref name="configurationSection"/>.
		/// </summary>
		/// <param name="configurationSection">The configuration section that must be managed.</param>
		/// <param name="wmiSettings">A collection to where the generated WMI objects are to be added.</param>
		protected override void GenerateWmiObjectsForConfigurationSection(LoggingSettings configurationSection,
			ICollection<ConfigurationSetting> wmiSettings)
		{
			wmiSettings.Add(
				new LoggingBlockSetting(
					configurationSection.DefaultCategory,
					configurationSection.LogWarningWhenNoCategoriesMatch,
					configurationSection.TracingEnabled));
		}

		/// <summary>
		/// Overrides the <paramref name="configurationSection"/>'s configuration elements' properties 
		/// with the Group Policy values from the registry, if any, and creates the <see cref="ConfigurationSetting"/> 
		/// instances that describe these configuration elements.
		/// </summary>
		/// <param name="configurationSection">The configuration section that must be managed.</param>
		/// <param name="readGroupPolicies"><see langword="true"/> if Group Policy overrides must be applied; otherwise, 
		/// <see langword="false"/>.</param>
		/// <param name="machineKey">The <see cref="IRegistryKey"/> which holds the Group Policy overrides for the 
		/// configuration section at the machine level, or <see langword="null"/> 
		/// if there is no such registry key.</param>
		/// <param name="userKey">The <see cref="IRegistryKey"/> which holds the Group Policy overrides for the 
		/// configuration section at the user level, or <see langword="null"/> 
		/// if there is no such registry key.</param>
		/// <param name="generateWmiObjects"><see langword="true"/> if WMI objects must be generated; otherwise, 
		/// <see langword="false"/>.</param>
		/// <param name="wmiSettings">A collection to where the generated WMI objects are to be added.</param>
		protected override void OverrideWithGroupPoliciesAndGenerateWmiObjectsForConfigurationElements(LoggingSettings configurationSection,
			bool readGroupPolicies, IRegistryKey machineKey, IRegistryKey userKey,
			bool generateWmiObjects, ICollection<ConfigurationSetting> wmiSettings)
		{
			OverrideWithGroupPoliciesAndGenerateWmiObjectsForCategoryTraceSources(configurationSection,
				readGroupPolicies, machineKey, userKey,
				generateWmiObjects, wmiSettings);
			OverrideWithGroupPoliciesAndGenerateWmiObjectsForSpecialTraceSources(configurationSection,
				readGroupPolicies, machineKey, userKey,
				generateWmiObjects, wmiSettings);
			OverrideWithGroupPoliciesAndGenerateWmiObjectsForElementCollection(configurationSection.LogFilters,
				LogFiltersKeyName,
				readGroupPolicies, machineKey, userKey,
				generateWmiObjects, wmiSettings);
			OverrideWithGroupPoliciesAndGenerateWmiObjectsForElementCollection(configurationSection.Formatters,
				LogFormattersKeyName,
				readGroupPolicies, machineKey, userKey,
				generateWmiObjects, wmiSettings);
			OverrideWithGroupPoliciesAndGenerateWmiObjectsForElementCollection(configurationSection.TraceListeners,
				TraceListenersKeyName,
				readGroupPolicies, machineKey, userKey,
				generateWmiObjects, wmiSettings);
		}

		private void OverrideWithGroupPoliciesAndGenerateWmiObjectsForCategoryTraceSources(LoggingSettings configurationSection,
			bool readGroupPolicies, IRegistryKey machineKey, IRegistryKey userKey,
			bool generateWmiObjects, ICollection<ConfigurationSetting> wmiSettings)
		{
			List<TraceSourceData> elementsToRemove = new List<TraceSourceData>();

			IRegistryKey machineSourcesKey = null;
			IRegistryKey userSourcesKey = null;

			try
			{
				LoadRegistrySubKeys(CategorySourcesKeyName,
					machineKey, userKey,
					out machineSourcesKey, out userSourcesKey);

				foreach (TraceSourceData traceSourceData in configurationSection.TraceSources)
				{
					IRegistryKey machineSourceKey = null;
					IRegistryKey userSourceKey = null;

					try
					{
						LoadRegistrySubKeys(traceSourceData.Name,
							machineSourcesKey, userSourcesKey,
							out machineSourceKey, out userSourceKey);

						if (!OverrideWithGroupPoliciesAndGenerateWmiObjectsForTraceSource(traceSourceData,
								readGroupPolicies, machineSourceKey, userSourceKey,
								generateWmiObjects, wmiSettings,
								SourceKindCategory))
						{
							elementsToRemove.Add(traceSourceData);
						}
					}
					finally
					{
						ReleaseRegistryKeys(machineSourceKey, userSourceKey);
					}
				}
			}
			finally
			{
				ReleaseRegistryKeys(machineSourcesKey, userSourcesKey);
			}

			// remove disabled elements
			foreach (TraceSourceData element in elementsToRemove)
			{
				configurationSection.TraceSources.Remove(element.Name);
			}
		}

		private void OverrideWithGroupPoliciesAndGenerateWmiObjectsForSpecialTraceSources(LoggingSettings configurationSection,
			bool readGroupPolicies,
			IRegistryKey machineKey,
			IRegistryKey userKey,
			bool generateWmiObjects,
			ICollection<ConfigurationSetting> wmiSettings)
		{
			IRegistryKey machineSpecialSourcesKey = null;
			IRegistryKey userSpecialSourcesKey = null;

			try
			{
				LoadRegistrySubKeys(SpecialSourcesKeyName,
					machineKey, userKey,
					out machineSpecialSourcesKey, out userSpecialSourcesKey);

				if (configurationSection.SpecialTraceSources.AllEventsTraceSource != null)
				{
					OverrideWithGroupPoliciesAndGenerateWmiObjectsForSpecialTraceSource(SpecialSourcesAllEventsKeyName,
						configurationSection.SpecialTraceSources.AllEventsTraceSource,
						readGroupPolicies, machineSpecialSourcesKey, userSpecialSourcesKey,
						generateWmiObjects, wmiSettings, SourceKindAllEvents);
				}
				if (configurationSection.SpecialTraceSources.NotProcessedTraceSource != null)
				{
					OverrideWithGroupPoliciesAndGenerateWmiObjectsForSpecialTraceSource(SpecialSourcesNotProcessedKeyName,
						configurationSection.SpecialTraceSources.NotProcessedTraceSource,
						readGroupPolicies, machineSpecialSourcesKey, userSpecialSourcesKey,
						generateWmiObjects, wmiSettings, SourceKindNotProcessed);
				}
				if (configurationSection.SpecialTraceSources.ErrorsTraceSource != null)
				{
					OverrideWithGroupPoliciesAndGenerateWmiObjectsForSpecialTraceSource(SpecialSourcesErrorsKeyName,
						configurationSection.SpecialTraceSources.ErrorsTraceSource,
						readGroupPolicies, machineSpecialSourcesKey, userSpecialSourcesKey,
						generateWmiObjects, wmiSettings,
						SourceKindErrors);
				}
			}
			finally
			{
				ReleaseRegistryKeys(machineSpecialSourcesKey, userSpecialSourcesKey);
			}
		}

		private void OverrideWithGroupPoliciesAndGenerateWmiObjectsForSpecialTraceSource(String sourceName,
			TraceSourceData traceSourceData,
			bool readGroupPolicies, IRegistryKey machineParentKey, IRegistryKey userParentKey,
			bool generateWmiObjects, ICollection<ConfigurationSetting> wmiSettings,
			String sourceKind)
		{
			if (readGroupPolicies)
			{
				IRegistryKey machineSourceKey = null;
				IRegistryKey userSourceKey = null;

				try
				{
					LoadRegistrySubKeys(sourceName,
						machineParentKey, userParentKey,
						out machineSourceKey, out userSourceKey);

					if (!OverrideWithGroupPoliciesAndGenerateWmiObjectsForTraceSource(traceSourceData,
							readGroupPolicies, machineSourceKey, userSourceKey,
							generateWmiObjects, wmiSettings,
							sourceKind))
					{
						// "special" trace sources cannot be removed because they aren't elements in collections
						// but a trace source is considered disabled if it has no trace listeners
						traceSourceData.TraceListeners.Clear();
					}
				}
				finally
				{
					ReleaseRegistryKeys(machineSourceKey, userSourceKey);
				}
			}
		}

		private bool OverrideWithGroupPoliciesAndGenerateWmiObjectsForTraceSource(TraceSourceData traceSourceData,
			bool readGroupPolicies, IRegistryKey machineKey, IRegistryKey userKey,
			bool generateWmiObjects, ICollection<ConfigurationSetting> wmiSettings,
			String sourceKind)
		{
			if (readGroupPolicies)
			{
				IRegistryKey policyKey = machineKey != null ? machineKey : userKey;
				if (policyKey != null)
				{
					if (policyKey.IsPolicyKey && !policyKey.GetBoolValue(PolicyValueName).Value)
					{
						return false;
					}
					try
					{
						SourceLevels? defaultLevelOverride = policyKey.GetEnumValue<SourceLevels>(SourceDefaultLevelPropertyName);

						// the key where the values for the source listeners are stored
						// might not exist if no listener is selected
						traceSourceData.TraceListeners.Clear();
						using (IRegistryKey listenersOverrideKey = policyKey.OpenSubKey(SourceTraceListenersPropertyName))
						{
							if (listenersOverrideKey != null)
							{
								foreach (String valueName in listenersOverrideKey.GetValueNames())
								{
									traceSourceData.TraceListeners.Add(new TraceListenerReferenceData(valueName));
								}
							}
						}
						traceSourceData.DefaultLevel = defaultLevelOverride.Value;
					}
					catch (RegistryAccessException ex)
					{
						LogExceptionWhileOverriding(ex);
					}
				}
			}
			if (generateWmiObjects)
			{
				String[] referencedTraceListeners = new String[traceSourceData.TraceListeners.Count];
				for (int i = 0; i < traceSourceData.TraceListeners.Count; i++)
				{
					referencedTraceListeners[i]
						= traceSourceData.TraceListeners.Get(i).Name;
				}

				wmiSettings.Add(
					new TraceSourceSetting(
						traceSourceData.Name,
						traceSourceData.DefaultLevel.ToString(),
						referencedTraceListeners,
						sourceKind));
			}

			return true;
		}

		#endregion
	}
}